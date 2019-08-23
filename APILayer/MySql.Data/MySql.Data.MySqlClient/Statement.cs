using MySql.Data.MySqlClient.Properties;
using System;
using System.Collections.Generic;

namespace MySql.Data.MySqlClient
{
	internal abstract class Statement
	{
		protected MySqlCommand command;

		protected string commandText;

		private List<MySqlPacket> buffers;

		public virtual string ResolvedCommandText
		{
			get
			{
				return this.commandText;
			}
		}

		protected Driver Driver
		{
			get
			{
				return this.command.Connection.driver;
			}
		}

		protected MySqlConnection Connection
		{
			get
			{
				return this.command.Connection;
			}
		}

		protected MySqlParameterCollection Parameters
		{
			get
			{
				return this.command.Parameters;
			}
		}

		private Statement(MySqlCommand cmd)
		{
			this.command = cmd;
			this.buffers = new List<MySqlPacket>();
		}

		public Statement(MySqlCommand cmd, string text) : this(cmd)
		{
			this.commandText = text;
		}

		public virtual void Close(MySqlDataReader reader)
		{
		}

		public virtual void Resolve(bool preparing)
		{
		}

		public virtual void Execute()
		{
			this.BindParameters();
			this.ExecuteNext();
		}

		public virtual bool ExecuteNext()
		{
			if (this.buffers.Count == 0)
			{
				return false;
			}
			MySqlPacket p = this.buffers[0];
			this.Driver.SendQuery(p);
			this.buffers.RemoveAt(0);
			return true;
		}

		protected virtual void BindParameters()
		{
			MySqlParameterCollection parameters = this.command.Parameters;
			int i = 0;
			while (true)
			{
				this.InternalBindParameters(this.ResolvedCommandText, parameters, null);
				if (this.command.Batch == null)
				{
					break;
				}
				while (i < this.command.Batch.Count)
				{
					MySqlCommand mySqlCommand = this.command.Batch[i++];
					MySqlPacket mySqlPacket = this.buffers[this.buffers.Count - 1];
					long num = mySqlCommand.EstimatedSize();
					if ((long)(mySqlPacket.Length - 4) + num > this.Connection.driver.MaxPacketSize)
					{
						parameters = mySqlCommand.Parameters;
						break;
					}
					this.buffers.RemoveAt(this.buffers.Count - 1);
					string resolvedCommandText = this.ResolvedCommandText;
					if (resolvedCommandText.StartsWith("(", StringComparison.Ordinal))
					{
						mySqlPacket.WriteStringNoNull(", ");
					}
					else
					{
						mySqlPacket.WriteStringNoNull("; ");
					}
					this.InternalBindParameters(resolvedCommandText, mySqlCommand.Parameters, mySqlPacket);
					if ((long)(mySqlPacket.Length - 4) > this.Connection.driver.MaxPacketSize)
					{
						parameters = mySqlCommand.Parameters;
						break;
					}
				}
				if (i == this.command.Batch.Count)
				{
					return;
				}
			}
		}

		private void InternalBindParameters(string sql, MySqlParameterCollection parameters, MySqlPacket packet)
		{
			bool sqlServerMode = this.command.Connection.Settings.SqlServerMode;
			if (packet == null)
			{
				packet = new MySqlPacket(this.Driver.Encoding);
				packet.Version = this.Driver.Version;
				packet.WriteByte(0);
			}
			MySqlTokenizer mySqlTokenizer = new MySqlTokenizer(sql);
			mySqlTokenizer.ReturnComments = true;
			mySqlTokenizer.SqlServerMode = sqlServerMode;
			int num = 0;
			string text = mySqlTokenizer.NextToken();
			int num2 = 0;
			while (text != null)
			{
				packet.WriteStringNoNull(sql.Substring(num, mySqlTokenizer.StartIndex - num));
				num = mySqlTokenizer.StopIndex;
				if (MySqlTokenizer.IsParameter(text))
				{
					if ((!parameters.containsUnnamedParameters && text.Length == 1 && num2 > 0) || (parameters.containsUnnamedParameters && text.Length > 1))
					{
						throw new MySqlException(Resources.MixedParameterNamingNotAllowed);
					}
					parameters.containsUnnamedParameters = (text.Length == 1);
					if (this.SerializeParameter(parameters, packet, text, num2))
					{
						text = null;
					}
					num2++;
				}
				if (text != null)
				{
					if (sqlServerMode && mySqlTokenizer.Quoted && text.StartsWith("[", StringComparison.Ordinal))
					{
						text = string.Format("`{0}`", text.Substring(1, text.Length - 2));
					}
					packet.WriteStringNoNull(text);
				}
				text = mySqlTokenizer.NextToken();
			}
			this.buffers.Add(packet);
		}

		protected virtual bool ShouldIgnoreMissingParameter(string parameterName)
		{
			return this.Connection.Settings.AllowUserVariables || parameterName.StartsWith("@_cnet_param_", StringComparison.OrdinalIgnoreCase) || (parameterName.Length > 1 && (parameterName[1] == '`' || parameterName[1] == '\''));
		}

		private bool SerializeParameter(MySqlParameterCollection parameters, MySqlPacket packet, string parmName, int parameterIndex)
		{
			MySqlParameter mySqlParameter;
			if (!parameters.containsUnnamedParameters)
			{
				mySqlParameter = parameters.GetParameterFlexible(parmName, false);
			}
			else
			{
				if (parameterIndex > parameters.Count)
				{
					throw new MySqlException(Resources.ParameterIndexNotFound);
				}
				mySqlParameter = parameters[parameterIndex];
			}
			if (mySqlParameter != null)
			{
				mySqlParameter.Serialize(packet, false, this.Connection.Settings);
				return true;
			}
			if (parmName.StartsWith("@", StringComparison.Ordinal) && this.ShouldIgnoreMissingParameter(parmName))
			{
				return false;
			}
			throw new MySqlException(string.Format(Resources.ParameterMustBeDefined, parmName));
		}
	}
}
