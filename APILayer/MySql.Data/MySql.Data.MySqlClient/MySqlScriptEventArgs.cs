using System;

namespace MySql.Data.MySqlClient
{
	public class MySqlScriptEventArgs : EventArgs
	{
		private ScriptStatement statement;

		internal ScriptStatement Statement
		{
			set
			{
				this.statement = value;
			}
		}

		public string StatementText
		{
			get
			{
				return this.statement.text;
			}
		}

		public int Line
		{
			get
			{
				return this.statement.line;
			}
		}

		public int Position
		{
			get
			{
				return this.statement.position;
			}
		}
	}
}
