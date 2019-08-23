using MySql.Data.MySqlClient.Properties;
using System;
using System.IO;
using System.Text;

namespace MySql.Data.MySqlClient
{
    internal class MySqlStream
    {
        private byte sequenceByte;

        private int maxBlockSize;

        private ulong maxPacketSize;

        private byte[] packetHeader = new byte[4];

        private MySqlPacket packet;

        private TimedStream timedStream;

        private Stream inStream;

        private Stream outStream;

        internal Stream BaseStream
        {
            get
            {
                return this.timedStream;
            }
        }

        public Encoding Encoding
        {
            get
            {
                return this.packet.Encoding;
            }
            set
            {
                this.packet.Encoding = value;
            }
        }

        public byte SequenceByte
        {
            get
            {
                return this.sequenceByte;
            }
            set
            {
                this.sequenceByte = value;
            }
        }

        public int MaxBlockSize
        {
            get
            {
                return this.maxBlockSize;
            }
            set
            {
                this.maxBlockSize = value;
            }
        }

        public ulong MaxPacketSize
        {
            get
            {
                return this.maxPacketSize;
            }
            set
            {
                this.maxPacketSize = value;
            }
        }

        public MySqlStream(Encoding encoding)
        {
            this.maxPacketSize = 18446744073709551615uL;
            this.maxBlockSize = 2147483647;
            this.packet = new MySqlPacket(encoding);
        }

        public MySqlStream(Stream baseStream, Encoding encoding, bool compress)
            : this(encoding)
        {
            this.timedStream = new TimedStream(baseStream);
            Stream stream;
            if (compress)
            {
                stream = new CompressedStream(this.timedStream);
            }
            else
            {
                stream = this.timedStream;
            }
            this.inStream = new BufferedStream(stream);
            this.outStream = stream;
        }

        public void Close()
        {
            this.outStream.Close();
            this.inStream.Close();
            this.timedStream.Close();
        }

        public void ResetTimeout(int timeout)
        {
            this.timedStream.ResetTimeout(timeout);
        }

        public MySqlPacket ReadPacket()
        {
            this.LoadPacket();
            if (this.packet.Buffer[0] == 255)
            {
                this.packet.ReadByte();
                int errno = this.packet.ReadInteger(2);
                string text = string.Empty;
                if (this.packet.Version.isAtLeast(5, 5, 0))
                {
                    text = this.packet.ReadString(Encoding.UTF8);
                }
                else
                {
                    text = this.packet.ReadString();
                }
                if (text.StartsWith("#", StringComparison.Ordinal))
                {
                    text.Substring(1, 5);
                    text = text.Substring(6);
                }
                throw new MySqlException(text, errno);
            }
            return this.packet;
        }

        internal static void ReadFully(Stream stream, byte[] buffer, int offset, int count)
        {
            int num = 0;
            int num2;
            for (int i = count; i > 0; i -= num2)
            {
                num2 = stream.Read(buffer, offset + num, i);
                if (num2 == 0)
                {
                    throw new EndOfStreamException();
                }
                num += num2;
            }
        }

        public void LoadPacket()
        {
            try
            {
                this.packet.Length = 0;
                int num = 0;
                int num2;
                do
                {
                    MySqlStream.ReadFully(this.inStream, this.packetHeader, 0, 4);
                    this.sequenceByte += this.packetHeader[3];
                    this.sequenceByte += 1;
                    num2 = (int)this.packetHeader[0] + ((int)this.packetHeader[1] << 8) + ((int)this.packetHeader[2] << 16);
                    this.packet.Length += num2;
                    MySqlStream.ReadFully(this.inStream, this.packet.Buffer, num, num2);
                    num += num2;
                }
                while (num2 >= this.maxBlockSize);
                this.packet.Position = 0;
            }
            catch (IOException inner)
            {
                throw new MySqlException(Resources.ReadFromStreamFailed, true, inner);
            }
        }

        public void SendPacket(MySqlPacket packet)
        {
            byte[] buffer = packet.Buffer;
            int i = packet.Position - 4;
            if ((ulong)i > this.maxPacketSize)
            {
                throw new MySqlException(Resources.QueryTooLarge, 1153);
            }
            int num = 0;
            while (i > 0)
            {
                int num2 = (i > this.maxBlockSize) ? this.maxBlockSize : i;
                buffer[num] = (byte)(num2 & 255);
                buffer[num + 1] = (byte)(num2 >> 8 & 255);
                buffer[num + 2] = (byte)(num2 >> 16 & 255);
                byte[] arg_83_0 = buffer;
                int arg_83_1 = num + 3;
                byte b;
                this.sequenceByte += (b = this.sequenceByte);
                this.sequenceByte += 1;
                arg_83_0[arg_83_1] = b;
                this.outStream.Write(buffer, num, num2 + 4);
                this.outStream.Flush();
                i -= num2;
                num += num2;
            }
        }

        public void SendEntirePacketDirectly(byte[] buffer, int count)
        {
            buffer[0] = (byte)(count & 255);
            buffer[1] = (byte)(count >> 8 & 255);
            buffer[2] = (byte)(count >> 16 & 255);
            int arg_3A_1 = 3;
            byte b;
            this.sequenceByte += (b = this.sequenceByte);
            this.sequenceByte += 1;
            buffer[arg_3A_1] = b;
            this.outStream.Write(buffer, 0, count + 4);
            this.outStream.Flush();
        }
    }
}
