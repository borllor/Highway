using MySql.Data.MySqlClient.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MySql.Data.Common
{
	internal class QueryNormalizer
	{
		private static List<string> keywords;

		private List<Token> tokens = new List<Token>();

		private int pos;

		private string fullSql;

		private string queryType;

		public string QueryType
		{
			get
			{
				return this.queryType;
			}
		}

		static QueryNormalizer()
		{
			QueryNormalizer.keywords = new List<string>();
			StringReader stringReader = new StringReader(Resources.keywords);
			for (string item = stringReader.ReadLine(); item != null; item = stringReader.ReadLine())
			{
				QueryNormalizer.keywords.Add(item);
			}
		}

		public string Normalize(string sql)
		{
			this.tokens.Clear();
			StringBuilder stringBuilder = new StringBuilder();
			this.fullSql = sql;
			this.TokenizeSql(sql);
			this.DetermineStatementType(this.tokens);
			this.ProcessMathSymbols(this.tokens);
			this.CollapseValueLists(this.tokens);
			this.CollapseInLists(this.tokens);
			this.CollapseWhitespace(this.tokens);
			foreach (Token current in this.tokens)
			{
				if (current.Output)
				{
					stringBuilder.Append(current.Text);
				}
			}
			return stringBuilder.ToString();
		}

		private void DetermineStatementType(List<Token> tok)
		{
			foreach (Token current in tok)
			{
				if (current.Type == TokenType.Keyword)
				{
					this.queryType = current.Text.ToUpperInvariant();
					break;
				}
			}
		}

		private void ProcessMathSymbols(List<Token> tok)
		{
			Token token = null;
			foreach (Token current in tok)
			{
				if (current.Type == TokenType.Symbol && (current.Text == "-" || current.Text == "+") && token != null && token.Type != TokenType.Number && token.Type != TokenType.Identifier && (token.Type != TokenType.Symbol || token.Text != ")"))
				{
					current.Output = false;
				}
				if (current.IsRealToken)
				{
					token = current;
				}
			}
		}

		private void CollapseWhitespace(List<Token> tok)
		{
			Token token = null;
			foreach (Token current in tok)
			{
				if (current.Output && current.Type == TokenType.Whitespace && token != null && token.Type == TokenType.Whitespace)
				{
					current.Output = false;
				}
				if (current.Output)
				{
					token = current;
				}
			}
		}

		private void CollapseValueLists(List<Token> tok)
		{
			int num = -1;
			while (++num < tok.Count)
			{
				Token token = tok[num];
				if (token.Type == TokenType.Keyword && token.Text.StartsWith("VALUE", StringComparison.OrdinalIgnoreCase))
				{
					this.CollapseValueList(tok, ref num);
				}
			}
		}

		private void CollapseValueList(List<Token> tok, ref int pos)
		{
			List<int> list = new List<int>();
			while (true)
			{
				if (++pos >= tok.Count || (tok[pos].Type == TokenType.Symbol && tok[pos].Text == ")") || pos == tok.Count - 1)
				{
					list.Add(pos);
					while (++pos < tok.Count && !tok[pos].IsRealToken)
					{
					}
					if (pos == tok.Count)
					{
						goto IL_A1;
					}
					if (tok[pos].Text != ",")
					{
						break;
					}
				}
			}
			pos--;
			IL_A1:
			if (list.Count < 2)
			{
				return;
			}
			int i = list[0];
			tok[++i] = new Token(TokenType.Whitespace, " ");
			tok[++i] = new Token(TokenType.Comment, "/* , ... */");
			i++;
			while (i <= list[list.Count - 1])
			{
				tok[i++].Output = false;
			}
		}

		private void CollapseInLists(List<Token> tok)
		{
			int num = -1;
			while (++num < tok.Count)
			{
				Token token = tok[num];
				if (token.Type == TokenType.Keyword && token.Text == "IN")
				{
					this.CollapseInList(tok, ref num);
				}
			}
		}

		private Token GetNextRealToken(List<Token> tok, ref int pos)
		{
			while (++pos < tok.Count)
			{
				if (tok[pos].IsRealToken)
				{
					return tok[pos];
				}
			}
			return null;
		}

		private void CollapseInList(List<Token> tok, ref int pos)
		{
			if (this.GetNextRealToken(tok, ref pos) == null)
			{
				return;
			}
			Token token = this.GetNextRealToken(tok, ref pos);
			if (token == null || token.Type == TokenType.Keyword)
			{
				return;
			}
			int num = pos;
			while (++pos < tok.Count)
			{
				token = tok[pos];
				if (token.Type == TokenType.CommandComment)
				{
					return;
				}
				if (token.IsRealToken)
				{
					if (token.Text == "(")
					{
						return;
					}
					if (token.Text == ")")
					{
						break;
					}
				}
			}
			int num2 = pos;
			for (int i = num2; i > num; i--)
			{
				tok.RemoveAt(i);
			}
			tok.Insert(++num, new Token(TokenType.Whitespace, " "));
			tok.Insert(++num, new Token(TokenType.Comment, "/* , ... */"));
			tok.Insert(++num, new Token(TokenType.Whitespace, " "));
			tok.Insert(num + 1, new Token(TokenType.Symbol, ")"));
		}

		private void TokenizeSql(string sql)
		{
			this.pos = 0;
			while (this.pos < sql.Length)
			{
				char c = sql[this.pos];
				if (!this.LetterStartsComment(c) || !this.ConsumeComment())
				{
					if (char.IsWhiteSpace(c))
					{
						this.ConsumeWhitespace();
					}
					else if (c == '\'' || c == '"' || c == '`')
					{
						this.ConsumeQuotedToken(c);
					}
					else if (!this.IsSpecialCharacter(c))
					{
						this.ConsumeUnquotedToken();
					}
					else
					{
						this.ConsumeSymbol();
					}
				}
			}
		}

		private bool LetterStartsComment(char c)
		{
			return c == '#' || c == '/' || c == '-';
		}

		private bool IsSpecialCharacter(char c)
		{
			return !char.IsLetterOrDigit(c) && c != '$' && c != '_' && c != '.';
		}

		private bool ConsumeComment()
		{
			char c = this.fullSql[this.pos];
			if (c == '/' && (this.pos + 1 >= this.fullSql.Length || this.fullSql[this.pos + 1] != '*'))
			{
				return false;
			}
			if (c == '-' && (this.pos + 2 >= this.fullSql.Length || this.fullSql[this.pos + 1] != '-' || this.fullSql[this.pos + 2] != ' '))
			{
				return false;
			}
			string text = "\n";
			if (c == '/')
			{
				text = "*/";
			}
			int num = this.fullSql.IndexOf(text, this.pos);
			if (num == -1)
			{
				num = this.fullSql.Length - 1;
			}
			else
			{
				num += text.Length;
			}
			string text2 = this.fullSql.Substring(this.pos, num - this.pos);
			if (text2.StartsWith("/*!", StringComparison.Ordinal))
			{
				this.tokens.Add(new Token(TokenType.CommandComment, text2));
			}
			this.pos = num;
			return true;
		}

		private void ConsumeSymbol()
		{
			char c = this.fullSql[this.pos++];
			this.tokens.Add(new Token(TokenType.Symbol, c.ToString()));
		}

		private void ConsumeQuotedToken(char c)
		{
			bool flag = false;
			int num = this.pos;
			this.pos++;
			while (this.pos < this.fullSql.Length)
			{
				char c2 = this.fullSql[this.pos];
				if (c2 == c && !flag)
				{
					break;
				}
				if (flag)
				{
					flag = false;
				}
				else if (c2 == '\\')
				{
					flag = true;
				}
				this.pos++;
			}
			this.pos++;
			if (c == '\'')
			{
				this.tokens.Add(new Token(TokenType.String, "?"));
				return;
			}
			this.tokens.Add(new Token(TokenType.Identifier, this.fullSql.Substring(num, this.pos - num)));
		}

		private void ConsumeUnquotedToken()
		{
			int num = this.pos;
			while (this.pos < this.fullSql.Length && !this.IsSpecialCharacter(this.fullSql[this.pos]))
			{
				this.pos++;
			}
			string text = this.fullSql.Substring(num, this.pos - num);
			double num2;
			if (double.TryParse(text, out num2))
			{
				this.tokens.Add(new Token(TokenType.Number, "?"));
				return;
			}
			Token token = new Token(TokenType.Identifier, text);
			if (this.IsKeyword(text))
			{
				token.Type = TokenType.Keyword;
				token.Text = token.Text.ToUpperInvariant();
			}
			this.tokens.Add(token);
		}

		private void ConsumeWhitespace()
		{
			this.tokens.Add(new Token(TokenType.Whitespace, " "));
			while (this.pos < this.fullSql.Length && char.IsWhiteSpace(this.fullSql[this.pos]))
			{
				this.pos++;
			}
		}

		private bool IsKeyword(string word)
		{
			return QueryNormalizer.keywords.Contains(word.ToUpperInvariant());
		}
	}
}
