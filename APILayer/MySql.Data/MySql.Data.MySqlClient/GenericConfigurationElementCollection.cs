using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;

namespace MySql.Data.MySqlClient
{
	public sealed class GenericConfigurationElementCollection<T> : ConfigurationElementCollection, IEnumerable<T>, IEnumerable where T : ConfigurationElement, new()
	{
		private List<T> _elements = new List<T>();

		protected override ConfigurationElement CreateNewElement()
		{
			T t = Activator.CreateInstance<T>();
			this._elements.Add(t);
			return t;
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return this._elements.Find((T e) => e.Equals(element));
		}

		public new IEnumerator<T> GetEnumerator()
		{
			return this._elements.GetEnumerator();
		}
	}
}
