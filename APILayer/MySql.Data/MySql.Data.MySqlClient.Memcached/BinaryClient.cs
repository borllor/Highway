using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MySql.Data.MySqlClient.Memcached
{
	public class BinaryClient : Client
	{
		private enum OpCodes : byte
		{
			Get,
			Set,
			Add,
			Replace,
			Delete,
			Increment,
			Decrement,
			Quit,
			Flush,
			GetK = 12,
			GetKQ,
			Append,
			Prepend,
			SASL_list_mechs = 32,
			SASL_Auth,
			SASL_Step
		}

		private enum MagicByte : byte
		{
			Request = 128,
			Response
		}

		private enum ResponseStatus : ushort
		{
			NoError,
			KeyNotFound,
			KeyExists,
			ValueTooLarge,
			InvalidArguments,
			ItemNotStored,
			IncrDecrOnNonNumericValue,
			VbucketBelongsToAnotherServer,
			AuthenticationError,
			AuthenticationContinue,
			UnknownCommand = 129,
			OutOfMemory,
			NotSupported,
			InternalError,
			Busy,
			TemporaryFailure
		}

		private Encoding encoding;

		public BinaryClient(string server, uint port) : base(server, port)
		{
			this.encoding = Encoding.UTF8;
		}

		public override void Add(string key, object data, TimeSpan expiration)
		{
			this.SendCommand(128, 2, key, data, expiration, true);
		}

		public override void Append(string key, object data)
		{
			this.SendCommand(128, 14, key, data, TimeSpan.Zero, false);
		}

		public override void Cas(string key, object data, TimeSpan expiration, ulong casUnique)
		{
			throw new NotImplementedException("Not available in binary protocol");
		}

		public override void Decrement(string key, int amount)
		{
			this.SendCommand(128, 6, key, amount);
		}

		public override void Delete(string key)
		{
			this.SendCommand(128, 4, key);
		}

		public override void FlushAll(TimeSpan delay)
		{
			this.SendCommand(128, 8, delay);
		}

		public override KeyValuePair<string, object> Get(string key)
		{
			string value;
			this.SendCommand(128, 0, key, out value);
			return new KeyValuePair<string, object>(key, value);
		}

		public override void Increment(string key, int amount)
		{
			this.SendCommand(128, 5, key, amount);
		}

		public override void Prepend(string key, object data)
		{
			this.SendCommand(128, 15, key, data, TimeSpan.Zero, false);
		}

		public override void Replace(string key, object data, TimeSpan expiration)
		{
			this.SendCommand(128, 3, key, data, expiration, true);
		}

		public override void Set(string key, object data, TimeSpan expiration)
		{
			this.SendCommand(128, 1, key, data, expiration, true);
		}

		private void SendCommand(byte magic, byte opcode, string key, object data, TimeSpan expiration, bool hasExtra)
		{
			byte[] array = this.EncodeStoreCommand(magic, opcode, key, data, expiration, hasExtra);
			this.stream.Write(array, 0, array.Length);
			this.GetResponse();
		}

		private void SendCommand(byte magic, byte opcode, string key, out string value)
		{
			byte[] array = this.EncodeGetCommand(magic, opcode, key);
			this.stream.Write(array, 0, array.Length);
			byte[] response = this.GetResponse();
			byte[] array2 = new byte[(int)(response[4] - 4)];
			Array.Copy(response, 28, array2, 0, (int)(response[4] - 4));
			value = this.encoding.GetString(array2, 0, array2.Length);
		}

		private void SendCommand(byte magic, byte opcode, string key)
		{
			byte[] array = this.EncodeGetCommand(magic, opcode, key);
			this.stream.Write(array, 0, array.Length);
			this.GetResponse();
		}

		private void SendCommand(byte magic, byte opcode, TimeSpan expiration)
		{
			byte[] array = this.EncodeFlushCommand(magic, opcode, expiration);
			this.stream.Write(array, 0, array.Length);
			this.GetResponse();
		}

		private void SendCommand(byte magic, byte opcode, string key, int amount)
		{
			byte[] array = this.EncodeIncrCommand(magic, opcode, key, amount);
			this.stream.Write(array, 0, array.Length);
			this.GetResponse();
		}

		private byte[] GetResponse()
		{
			byte[] array = new byte[24];
			this.stream.Read(array, 0, array.Length);
			this.ValidateResponse(array);
			return array;
		}

		private void ValidateResponse(byte[] res)
		{
			ushort num = (ushort)((int)res[6] << 8 | (int)res[7]);
			if (num != 0)
			{
				throw new MemcachedException(((BinaryClient.ResponseStatus)num).ToString());
			}
		}

		private byte[] EncodeStoreCommand(byte magic, byte opcode, string key, object data, TimeSpan expiration, bool hasExtra)
		{
			byte[] bytes = this.encoding.GetBytes(key);
			byte[] bytes2 = this.encoding.GetBytes(data.ToString());
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.WriteByte(magic);
			memoryStream.WriteByte(opcode);
			this.WriteToMemoryStream(BitConverter.GetBytes((ushort)bytes.Length), memoryStream);
			memoryStream.WriteByte(8);
			memoryStream.WriteByte(0);
			memoryStream.WriteByte(0);
			memoryStream.WriteByte(0);
			this.WriteToMemoryStream(BitConverter.GetBytes((uint)(bytes.Length + bytes2.Length + (hasExtra ? 8 : 0))), memoryStream);
			this.WriteToMemoryStream(BitConverter.GetBytes(0u), memoryStream);
			this.WriteToMemoryStream(BitConverter.GetBytes(0), memoryStream);
			if (hasExtra)
			{
				memoryStream.Write(new byte[4], 0, 4);
				this.WriteToMemoryStream(BitConverter.GetBytes((uint)expiration.TotalSeconds), memoryStream);
			}
			memoryStream.Write(bytes, 0, bytes.Length);
			memoryStream.Write(bytes2, 0, bytes2.Length);
			return memoryStream.ToArray();
		}

		private byte[] EncodeGetCommand(byte magic, byte opcode, string key)
		{
			byte[] bytes = this.encoding.GetBytes(key);
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.WriteByte(magic);
			memoryStream.WriteByte(opcode);
			this.WriteToMemoryStream(BitConverter.GetBytes((ushort)bytes.Length), memoryStream);
			memoryStream.WriteByte(8);
			memoryStream.WriteByte(0);
			memoryStream.WriteByte(0);
			memoryStream.WriteByte(0);
			this.WriteToMemoryStream(BitConverter.GetBytes((ushort)bytes.Length), memoryStream);
			this.WriteToMemoryStream(BitConverter.GetBytes(0u), memoryStream);
			this.WriteToMemoryStream(BitConverter.GetBytes(0u), memoryStream);
			memoryStream.Write(bytes, 0, bytes.Length);
			return memoryStream.ToArray();
		}

		private byte[] EncodeFlushCommand(byte magic, byte opcode, TimeSpan expiration)
		{
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.WriteByte(magic);
			memoryStream.WriteByte(opcode);
			memoryStream.WriteByte(0);
			memoryStream.WriteByte(0);
			memoryStream.WriteByte(4);
			memoryStream.WriteByte(0);
			memoryStream.WriteByte(0);
			memoryStream.WriteByte(0);
			this.WriteToMemoryStream(BitConverter.GetBytes(4), memoryStream);
			this.WriteToMemoryStream(BitConverter.GetBytes(0u), memoryStream);
			this.WriteToMemoryStream(BitConverter.GetBytes(0u), memoryStream);
			this.WriteToMemoryStream(BitConverter.GetBytes((uint)expiration.TotalSeconds), memoryStream);
			return memoryStream.ToArray();
		}

		private byte[] EncodeIncrCommand(byte magic, byte opcode, string key, int amount)
		{
			byte[] bytes = this.encoding.GetBytes(key);
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.WriteByte(magic);
			memoryStream.WriteByte(opcode);
			this.WriteToMemoryStream(BitConverter.GetBytes((ushort)bytes.Length), memoryStream);
			memoryStream.WriteByte(20);
			memoryStream.WriteByte(0);
			memoryStream.WriteByte(0);
			memoryStream.WriteByte(0);
			this.WriteToMemoryStream(BitConverter.GetBytes((ushort)(bytes.Length + 20)), memoryStream);
			this.WriteToMemoryStream(BitConverter.GetBytes(0u), memoryStream);
			this.WriteToMemoryStream(BitConverter.GetBytes(0u), memoryStream);
			long num = (long)amount;
			if (opcode == 6)
			{
				num *= -1L;
			}
			this.WriteToMemoryStream(BitConverter.GetBytes(0L), memoryStream);
			this.WriteToMemoryStream(BitConverter.GetBytes((uint)TimeSpan.Zero.TotalSeconds), memoryStream);
			memoryStream.Write(bytes, 0, bytes.Length);
			return memoryStream.ToArray();
		}

		private void WriteToMemoryStream(byte[] data, MemoryStream ms)
		{
			Array.Reverse(data);
			ms.Write(data, 0, data.Length);
		}
	}
}
