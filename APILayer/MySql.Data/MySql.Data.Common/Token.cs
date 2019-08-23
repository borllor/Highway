using System;

namespace MySql.Data.Common
{
	internal class Token
	{
		public TokenType Type;

		public string Text;

		public bool Output;

		public bool IsRealToken
		{
			get
			{
				return this.Type != TokenType.Comment && this.Type != TokenType.CommandComment && this.Type != TokenType.Whitespace && this.Output;
			}
		}

		public Token(TokenType type, string text)
		{
			this.Type = type;
			this.Text = text;
			this.Output = true;
		}
	}
}
