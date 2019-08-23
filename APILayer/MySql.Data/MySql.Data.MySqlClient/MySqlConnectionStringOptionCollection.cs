using System;
using System.Collections.Generic;

namespace MySql.Data.MySqlClient
{
	internal class MySqlConnectionStringOptionCollection : Dictionary<string, MySqlConnectionStringOption>
	{
		private List<MySqlConnectionStringOption> options;

		internal List<MySqlConnectionStringOption> Options
		{
			get
			{
				return this.options;
			}
		}

		internal MySqlConnectionStringOptionCollection() : base(StringComparer.OrdinalIgnoreCase)
		{
			this.options = new List<MySqlConnectionStringOption>();
		}

		internal void Add(MySqlConnectionStringOption option)
		{
			this.options.Add(option);
			base.Add(option.Keyword, option);
			if (option.Synonyms != null)
			{
				for (int i = 0; i < option.Synonyms.Length; i++)
				{
					base.Add(option.Synonyms[i], option);
				}
			}
		}

		internal MySqlConnectionStringOption Get(string keyword)
		{
			MySqlConnectionStringOption result = null;
			base.TryGetValue(keyword, out result);
			return result;
		}
	}
}
