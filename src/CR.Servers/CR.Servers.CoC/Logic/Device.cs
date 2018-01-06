﻿namespace CR.Servers.CoC.Logic
{
    using System;
    using System.Linq;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic.Mode;
    using CR.Servers.CoC.Packets;
    using CR.Servers.CoC.Packets.Stream;
    using CR.Servers.Extensions.Binary;
    using CR.Servers.Logic.Enums;

    public class Device : IDisposable
    {
        internal Account Account;
        internal Chat.Chat Chat;

        internal bool Disposed;

        internal int EncryptionSeed;

        internal GameMode GameMode;
        internal DeviceInfo Info;
        internal DateTime LastKeepAlive;

        internal StreamEncrypter ReceiveEncrypter;
        internal StreamEncrypter SendEncrypter;

        internal State State = State.DISCONNECTED;

        internal Token Token;
        internal bool UseRC4;

        internal Device()
        {
            this.GameMode = new GameMode(this);
            this.LastKeepAlive = DateTime.UtcNow;
        }

        internal int Checksum
        {
            get
            {
                int Checksum = 0;

                Checksum += this.GameMode.Time.SubTick;
                Checksum += this.GameMode.Time.TotalSecs;

                if (this.GameMode.Level.Player != null)
                {
                    Checksum += this.GameMode.Level.Player.Checksum;

                    if (this.GameMode.Level.GameObjectManager != null)
                    {
                        Checksum += this.GameMode.Level.GameObjectManager.Checksum;
                    }
                }

                // Visitor

                Checksum += Checksum;

                return Checksum;
            }
        }

        internal long TimeSinceLastKeepAliveMs => (long) DateTime.UtcNow.Subtract(this.LastKeepAlive).TotalMilliseconds;

        internal bool Connected => !this.Disposed && this.Token.Connected;

        internal string OS => this.Info.Android ? "Android" : "iOS";

        public void Dispose()
        {
            if (!this.Disposed)
            {
                this.Disposed = true;
                this.State = State.DISCONNECTED;

                this.Chat?.Quit(this);

                if (this.Account != null)
                {
                    if (this.Account.DefenseAccount != null)
                    {
                        this.Account.DefenseAccount.InBattle = false;
                        this.Account.DefenseAccount = null;
                    }

                    if (this.Account.Player != null)
                    {
                        Resources.Accounts.SavePlayer(this.Account.Player);

                        if (this.GameMode?.Level != null && this.Account.Player.BattleIdV2 > 0)
                        {
                            Resources.BattlesV2.Dequeue(this.GameMode.Level);
                        }

                        this.Account.Player.Alliance?.DecrementTotalConnected();
                    }

                    if (this.Account.Home != null)
                    {
                        Resources.Accounts.SaveHome(this.Account.Home);
                    }

                    this.Account.Device = null;
                }

                if (this.GameMode?.CommandManager != null)
                {
                    if (this.GameMode.CommandManager.ServerCommands != null)
                    {
                        foreach (Command Command in this.GameMode.CommandManager.ServerCommands.Values.ToArray())
                        {
                            Command.Execute();
                        }
                    }
                    else
                    {
                        Logging.Error(this.GetType(), "CommandManager != null but ServerCommands == null");
                    }
                }
            }
        }

        internal void Process(byte[] buffer)
        {
            if (this.State != State.DISCONNECTED)
            {
                if (buffer.Length >= 7)
                {
                    int messageType = buffer[1] | (buffer[0] << 8);
                    int messageLength = buffer[4] | (buffer[3] << 8) | (buffer[2] << 16);
                    int messageVersion = buffer[6] | (buffer[5] << 8);

                    if (messageLength < 0x800000)
                    {
                        if (buffer.Length - 7 >= messageLength)
                        {
                            byte[] messageBytes = new byte[messageLength];
                            Array.Copy(buffer, 7, messageBytes, 0, messageLength);

                            if (this.ReceiveEncrypter != null)
                            {
                                messageBytes = this.ReceiveEncrypter.Decrypt(messageBytes);
                            }
                            else
                            {
                                if (messageType == 10101)
                                {
                                    this.UseRC4 = true;
                                    this.InitRC4Encrypters(Factory.RC4Key, "nonce");

                                    messageBytes = this.ReceiveEncrypter.Decrypt(messageBytes);
                                }
                            }

                            if (messageBytes != null)
                            {
                                Message message = Factory.CreateMessage((short) messageType, this, new Reader(messageBytes));

                                if (message != null)
                                {
                                    message.Version = (short) messageVersion;
                                    Resources.PacketManager.ReceiveMessageQueue.Enqueue(message);
                                }
                                else
                                {
                                    Logging.Info(this.GetType(), "Message type " + messageType + " not exist.");
                                }
                            }
                            else
                            {
                                Logging.Error(this.GetType(), "Unable to decrypt message type " + messageType + ". Encrypter: " + this.ReceiveEncrypter + ".");
                            }

                            this.Token.Packet.RemoveRange(0, messageLength + 7);

                            if (buffer.Length - 7 - messageLength >= 7)
                            {
                                byte[] nextPacket = new byte[buffer.Length - 7 - messageLength];
                                Array.Copy(buffer, messageLength + 7, nextPacket, 0, nextPacket.Length);
                                this.Process(nextPacket);
                            }
                        }
                    }
                    else
                    {
                        Resources.Gateway.Disconnect(this.Token.Args);
                    }
                }
            }
        }

        internal void InitRC4Encrypters(string key, string nonce)
        {
            this.ReceiveEncrypter = new RC4Encrypter(key, nonce);
            this.SendEncrypter = new RC4Encrypter(key, nonce);
        }

        internal struct DeviceInfo
        {
            internal bool Android;
            internal bool Advertising;

            internal string UDID;
            internal string OpenUDID;
            internal string AndroidID;
            internal string DeviceModel;
            internal string ADID;
            internal string OSVersion;
            internal string PreferredLanguage;
            internal string[] ClientVersion;

            internal int Locale;

            /*
            internal void ShowValues()
            {
                foreach (var Field in GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    if (Field != null)
                        Logging.Info(GetType(),
                            ConsolePad.Padding(Field.Name) + " : " +
                            ConsolePad.Padding(!string.IsNullOrEmpty(Field.Name) ? (Field.GetValue(this) != null ? Field.GetValue(this).ToString() : "(null)") : "(null)", 40));
            }
            */
        }
    }
}