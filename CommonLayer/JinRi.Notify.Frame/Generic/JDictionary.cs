using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace JinRi.Notify.Frame
{
    public class JDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public JDictionary()
            : base()
        {
        }

        public JDictionary(IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        {
        }


        public JDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }


        public JDictionary(int capacity)
            : base(capacity)
        {
        }


        public JDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
            : base(dictionary, comparer)
        {
        }


        public JDictionary(int capacity, IEqualityComparer<TKey> comparer)
            : base(capacity, comparer)
        {
        }


        protected JDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// 从行中获取给定列的字符串值
        /// </summary>
        public string String(TKey key)
        {
            return String(key, string.Empty);
        }

        /// <summary>
        /// 从行中获取给定列的字符串值，如果为空，则取给定的默认值
        /// </summary>
        public string String(TKey key, string defaultValue)
        {
            TValue val = this[key];
            return Null.IsNull(val) ? defaultValue : Convert.ToString(val);
        }

        /// <summary>
        /// 从行中获取给定列的8位无符号整数
        /// </summary>
        public byte Byte(TKey key)
        {
            return Byte(key, Null.NullByte);
        }

        /// <summary>
        /// 位无符号整数，如果为空，则取给定的默认值
        /// </summary>
        public byte Byte(TKey key, byte defaultValue)
        {
            try
            {
                TValue val = this[key];
                return Null.IsNull(val) ? defaultValue : Convert.ToByte(val);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的16位整数值
        /// </summary>
        public short Short(TKey key)
        {
            return Short(key, Null.NullShort);
        }

        /// <summary>
        /// 从行中获取给定列的16位整数值，如果为空，则取给定的默认值
        /// </summary>
        public short Short(TKey key, short defaultValue)
        {
            try
            {
                TValue val = this[key];
                return Null.IsNull(val) ? defaultValue : Convert.ToInt16(val);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的整数值
        /// </summary>
        public int Int(TKey key)
        {
            return Int(key, Null.NullInteger);
        }

        /// <summary>
        /// 从行中获取给定列的整数值，如果为空，则取给定的默认值
        /// </summary>
        public int Int(TKey key, int defaultValue)
        {
            try
            {
                TValue val = this[key];
                return Null.IsNull(val) ? defaultValue : Convert.ToInt32(val);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的长整数值
        /// </summary>
        public long Long(TKey key)
        {
            return Long(key, Null.NullLong);
        }

        /// <summary>
        /// 从行中获取给定列的长整数值，如果为空，则取给定的默认值
        /// </summary>
        public long Long(TKey key, long defaultValue)
        {
            try
            {
                TValue val = this[key];
                return Null.IsNull(val) ? defaultValue : Convert.ToInt64(val);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的十进制数
        /// </summary>
        public decimal Decimal(TKey key)
        {
            return Decimal(key, Null.NullDecimal);
        }

        /// <summary>
        /// 从行中获取给定列的十进制数，如果为空，则取给定的默认值
        /// </summary>
        public decimal Decimal(TKey key, decimal defaultValue)
        {
            try
            {
                TValue val = this[key];
                return Null.IsNull(val) ? defaultValue : Convert.ToDecimal(val);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的双浮点数
        /// </summary>
        public double Double(TKey key)
        {
            return Double(key, Null.NullDouble);
        }

        /// <summary>
        /// 从行中获取给定列的双浮点数，如果为空，则取给定的默认值
        /// </summary>
        public double Double(TKey key, double defaultValue)
        {
            try
            {
                TValue val = this[key];
                return Null.IsNull(val) ? defaultValue : Convert.ToDouble(val);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的浮点数
        /// </summary>
        public float Float(TKey key)
        {
            return Float(key, Null.NullSingle);
        }

        /// <summary>
        /// 从行中获取给定列的浮点数，如果为空，则取给定的默认值
        /// </summary>
        public float Float(TKey key, float defaultValue)
        {
            try
            {
                TValue val = this[key];
                return Null.IsNull(val) ? defaultValue : Convert.ToSingle(val);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的布尔值
        /// </summary>
        public bool Bool(TKey key)
        {
            return Bool(key, Null.NullBoolean);
        }

        /// <summary>
        /// 从行中获取给定列的布尔值
        /// </summary>
        public bool Bool(TKey key, bool defaultValue)
        {
            try
            {
                TValue val = this[key];
                if (Null.IsNull(key))
                {
                    return defaultValue;
                }
                else
                {
                    string s = String(key);
                    if (!string.IsNullOrEmpty(s))
                    {
                        s = s.ToLower();
                        if (s == "true" || s == "t" || s == "1" || s == "success")
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return defaultValue;
                    }
                }
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 从行中获取给定列的布尔值
        /// </summary>
        public DateTime DateTime(TKey key)
        {
            return DateTime(key, Null.NullDate);
        }

        /// <summary>
        /// 从行中获取给定列的布尔值
        /// </summary>
        public DateTime DateTime(TKey key, DateTime defaultValue)
        {
            try
            {
                TValue val = this[key];
                return Null.IsNull(val) ? defaultValue : Convert.ToDateTime(val);
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
