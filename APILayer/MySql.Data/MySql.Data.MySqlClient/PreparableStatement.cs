using MySql.Data.MySqlClient.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MySql.Data.MySqlClient
{
	internal class PreparableStatement : Statement
	{
		private int executionCount;

		private int statementId;

		private BitArray nullMap;

		private List<MySqlParameter> parametersToSend = new List<MySqlParameter>();

		private MySqlPacket packet;

		private int dataPosition;

		private int nullMapPosition;

		public int ExecutionCount
		{
			get
			{
				return this.executionCount;
			}
			set
			{
				this.executionCount = value;
			}
		}

		public bool IsPrepared
		{
			get
			{
				return this.statementId > 0;
			}
		}

		public int StatementId
		{
			get
			{
				return this.statementId;
			}
		}

		public PreparableStatement(MySqlCommand command, string text) : base(command, text)
		{
		}

		public virtual void Prepare()
		{
			string sql;
			List<string> list = this.PrepareCommandText(out sql);
			MySqlField[] array = null;
			this.statementId = base.Driver.PrepareStatement(sql, ref array);
			for (int i = 0; i < list.Count; i++)
			{
				string text = list[i];
				MySqlParameter parameterFlexible = base.Parameters.GetParameterFlexible(text, false);
				if (parameterFlexible == null)
				{
					throw new InvalidOperationException(string.Format(Resources.ParameterNotFoundDuringPrepare, text));
				}
				parameterFlexible.Encoding = array[i].Encoding;
				this.parametersToSend.Add(parameterFlexible);
			}
			int num = 0;
			if (array != null && array.Length > 0)
			{
				this.nullMap = new BitArray(array.Length);
				num = (this.nullMap.Count + 7) / 8;
			}
			this.packet = new MySqlPacket(base.Driver.Encoding);
			this.packet.WriteByte(0);
			this.packet.WriteInteger((long)this.statementId, 4);
			this.packet.WriteByte(0);
			this.packet.WriteInteger(1L, 4);
			this.nullMapPosition = this.packet.Position;
			this.packet.Position += num;
			this.packet.WriteByte(1);
			foreach (MySqlParameter current in this.parametersToSend)
			{
				this.packet.WriteInteger((long)current.GetPSType(), 2);
			}
			this.dataPosition = this.packet.Position;
		}

		public override void Execute()
		{
			if (!this.IsPrepared)
			{
				base.Execute();
				return;
			}
			this.packet.Position = this.dataPosition;
			for (int i = 0; i < this.parametersToSend.Count; i++)
			{
				MySqlParameter mySqlParameter = this.parametersToSend[i];
				this.nullMap[i] = (mySqlParameter.Value == DBNull.Value || mySqlParameter.Value == null || mySqlParameter.Direction == ParameterDirection.Output);
				if (!this.nullMap[i])
				{
					this.packet.Encoding = mySqlParameter.Encoding;
					mySqlParameter.Serialize(this.packet, true, base.Connection.Settings);
				}
			}
			if (this.nullMap != null)
			{
				this.nullMap.CopyTo(this.packet.Buffer, this.nullMapPosition);
			}
			this.executionCount++;
			base.Driver.ExecuteStatement(this.packet);
		}

		public override bool ExecuteNext()
		{
			return !this.IsPrepared && base.ExecuteNext();
		}

		private List<string> PrepareCommandText(out string stripped_sql)
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<string> list = new List<string>();
			int num = 0;
			string resolvedCommandText = this.ResolvedCommandText;
			MySqlTokenizer mySqlTokenizer = new MySqlTokenizer(resolvedCommandText);
			for (string text = mySqlTokenizer.NextParameter(); text != null; text = mySqlTokenizer.NextParameter())
			{
				if (text.IndexOf("_cnet_param_") == -1)
				{
					stringBuilder.Append(resolvedCommandText.Substring(num, mySqlTokenizer.StartIndex - num));
					stringBuilder.Append("?");
					list.Add(text);
					num = mySqlTokenizer.StopIndex;
				}
			}
			stringBuilder.Append(resolvedCommandText.Substring(num));
			stripped_sql = stringBuilder.ToString();
			return list;
		}

		public virtual void CloseStatement()
		{
			if (!this.IsPrepared)
			{
				return;
			}
			base.Driver.CloseStatement(this.statementId);
			this.statementId = 0;
		}
	}
}
