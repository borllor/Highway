using System;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Collections;
using System.Runtime.Serialization;

namespace JinRi.Notify.Frame
{
     public class NameObjectCollection : NameObjectCollectionBase
    {
        private object[] _all;
        private string[] _allKeys;

        public NameObjectCollection()
        {
        }

        public NameObjectCollection(IEqualityComparer equalityComparer) 
            : base(equalityComparer)
        {
        }

        public NameObjectCollection(int capacity) 
            : base(capacity)
        {
        }

        public NameObjectCollection(int capacity, IEqualityComparer equalityComparer)
            : base(capacity, equalityComparer)
        {
        }

        protected NameObjectCollection(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }

        public void Add(NameObjectCollection c)
        {
            if (c == null)
            {
                throw new ArgumentNullException("c");
            }
            this.InvalidateCachedArrays();
            int count = c.Count;
            for (int i = 0; i < count; i++)
            {
                string key = c.GetKey(i);
                object[] values = c.GetValues(i);
                if (values != null)
                {
                    for (int j = 0; j < values.Length; j++)
                    {
                        this.Add(key, values[j]);
                    }
                }
                else
                {
                    this.Add(key, null);
                }
            }
        }

        public virtual void Add(string name, object value)
        {
            if (base.IsReadOnly)
            {
                throw new NotSupportedException("数据集为只读状态，不能修改");
            }
            this.InvalidateCachedArrays();
            ArrayList list = (ArrayList) base.BaseGet(name);
            if (list == null)
            {
                list = new ArrayList(1);
                if (value != null)
                {
                    list.Add(value);
                }
                base.BaseAdd(name, list);
            }
            else if (value != null)
            {
                list.Add(value);
            }
        }

        public virtual void Clear()
        {
            if (base.IsReadOnly)
            {
                throw new NotSupportedException("数据集为只读状态，不能修改");
            }
            this.InvalidateCachedArrays();
            base.BaseClear();
        }

        public void CopyTo(Array dest, int index)
        {
            if (dest == null)
            {
                throw new ArgumentNullException("dest");
            }
            if (dest.Rank != 1)
            {
                throw new ArgumentException("目标数组不是一维数组");
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("索引越界");
            }
            if ((dest.Length - index) < this.Count)
            {
                throw new ArgumentException("要复制到的目标数组空间不够");
            }
            int count = this.Count;
            if (this._all == null)
            {
                this._all = new string[count];
                for (int i = 0; i < count; i++)
                {
                    this._all[i] = this.Get(i);
                    dest.SetValue(this._all[i], (int) (i + index));
                }
            }
            else
            {
                for (int j = 0; j < count; j++)
                {
                    dest.SetValue(this._all[j], (int) (j + index));
                }
            }
        }

        public virtual object Get(int index)
        {
            ArrayList list = (ArrayList) base.BaseGet(index);
            return GetAsOneObject(list);
        }

        public virtual object Get(string name)
        {
            ArrayList list = (ArrayList) base.BaseGet(name);
            return GetAsOneObject(list);
        }

        private static object GetAsOneObject(ArrayList list)
        {
            int num = (list != null) ? list.Count : 0;
            if (num == 1)
            {
                return list[0];
            }
            if (num <= 1)
            {
                return null;
            }
            object[] array = new object[num];
            list.CopyTo(0, array, 0, num);
            return array;
        }

        private static object[] GetAsObjectArray(ArrayList list)
        {
            int count = (list != null) ? list.Count : 0;
            if (count == 0)
            {
                return null;
            }
            object[] array = new object[count];
            list.CopyTo(0, array, 0, count);
            return array;
        }

        public virtual string GetKey(int index)
        {
            return base.BaseGetKey(index);
        }

        public virtual object[] GetValues(int index)
        {
            ArrayList list = (ArrayList) base.BaseGet(index);
            return GetAsObjectArray(list);
        }

        public virtual object[] GetValues(string name)
        {
            ArrayList list = (ArrayList) base.BaseGet(name);
            return GetAsObjectArray(list);
        }

        public bool HasKeys()
        {
            return this.InternalHasKeys();
        }

        internal virtual bool InternalHasKeys()
        {
            return base.BaseHasKeys();
        }

        protected void InvalidateCachedArrays()
        {
            this._all = null;
            this._allKeys = null;
        }

        public virtual void Remove(string name)
        {
            this.InvalidateCachedArrays();
            base.BaseRemove(name);
        }

        public virtual void Set(string name, object value)
        {
            if (base.IsReadOnly)
            {
                throw new NotSupportedException("数据集为只读状态，不能修改");
            }
            this.InvalidateCachedArrays();
            ArrayList list = new ArrayList(1);
            list.Add(value);
            base.BaseSet(name, list);
        }

        public virtual string[] AllKeys
        {
            get
            {
                if (this._allKeys == null)
                {
                    this._allKeys = base.BaseGetAllKeys();
                }
                return this._allKeys;
            }
        }

        public object this[string name]
        {
            get
            {
                return this.Get(name);
            }
            set
            {
                this.Set(name, value);
            }
        }

        public object this[int index]
        {
            get
            {
                return this.Get(index);
            }
        }

        #region 获取特定类型数据

        public NameObjectCollection NameObject(string name)
        {
            object obj = Get(name);
            return obj != null ? obj as NameObjectCollection : null;
        }

        /// <summary>
        /// 取项值作为一个字符返回
        /// </summary>
        /// <param name="name">项名</param>
        /// <returns></returns>
        public bool Bool(string name)
        {
            string temp = String(name);
            if (!string.IsNullOrEmpty(temp))
            {
                temp = temp.ToLower();
                if (temp == "true" ||
                    temp == "1" ||
                    temp == "t" ||
                    temp == "y" ||
                    temp == "s" ||
                    temp == "success"
                    )
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 取项值作为一个字符返回
        /// </summary>
        /// <param name="name">项名</param>
        /// <returns></returns>
        public char Char(string name)
        {
            object obj = Get(name);
            string tmep = string.Format("{0}", obj);
            return string.IsNullOrEmpty(tmep) ? (char)Null.NullByte : tmep[0];
        }

        /// <summary>
        /// 取项值作为一个字符串返回
        /// </summary>
        /// <param name="name">项名</param>
        /// <returns></returns>
        public string String(string name)
        {
            object obj = Get(name);
            return string.Format("{0}", obj);
        }

        /// <summary>
        /// 取项值作为一个整数返回
        /// </summary>
        /// <param name="name">项名</param>
        /// <returns></returns>
        public int Int(string name)
        {
            object obj = Get(name);
            string tmep = string.Format("{0}", obj);
            int ret = Null.NullInteger;
            int.TryParse(tmep, out ret);
            return ret;
        }

        /// <summary>
        /// 取项值作为一个浮点数返回
        /// </summary>
        /// <param name="name">项名</param>
        /// <returns></returns>
        public float Float(string name)
        {
            object obj = Get(name);
            string tmep = string.Format("{0}", obj);
            float ret = Null.NullSingle;
            float.TryParse(tmep, out ret);
            return ret;
        }

        /// <summary>
        /// 取项值作为一个日期返回
        /// </summary>
        /// <param name="name">项名</param>
        /// <returns></returns>
        public DateTime DateTime(string name)
        {
            object obj = Get(name);
            string tmep = string.Format("{0}", obj);
            DateTime ret = Null.NullDate;
            System.DateTime.TryParse(tmep, out ret);
            return ret;
        }

        #endregion
    }
}
