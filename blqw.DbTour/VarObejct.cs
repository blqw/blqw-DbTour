using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

namespace blqw
{
    /// <summary> 数据库中的值,可自动转为各种C#类型
    /// </summary>
    public struct VarObejct : IConvertible
    {

        /// <summary> 数据库值
        /// </summary>
        private object _value;

        /// <summary> 数据库值
        /// </summary>
        public object Value
        {
            get { return _value; }
        }

        /// <summary> 当前值是否为null
        /// </summary>
        private bool _isDBNull;

        /// <summary> 初始化 DBValue
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="converter">自定义转换器,如果是null使用系统转换器</param>
        public VarObejct(object value)
        {
            if (value is DBNull || value == null)
            {
                _value = null;
                _isDBNull = true;
            }
            else
            {
                _value = value;
                _isDBNull = false;
            }
        }

        /// <summary> 当前值是否为null
        /// </summary>
        public bool IsDBNull { get { return _isDBNull; } }

        #region 强转

        public static explicit operator char(VarObejct value)
        {
            return Convert2.ToChar(value._value);
        }

        public static explicit operator int(VarObejct value)
        {
            return Convert2.ToInt32(value._value);
        }

        public static explicit operator long(VarObejct value)
        {
            return Convert2.ToInt64(value._value);
        }

        public static explicit operator bool(VarObejct value)
        {
            return Convert2.ToBoolean(value._value);
        }

        public static explicit operator string(VarObejct value)
        {
            return Convert2.ToString(value._value);
        }

        public static explicit operator DateTime(VarObejct value)
        {
            return Convert2.ToDateTime(value._value);
        }

        public static explicit operator decimal(VarObejct value)
        {
            return Convert2.ToDecimal(value._value);
        }

        public static explicit operator float(VarObejct value)
        {
            return Convert2.ToSingle(value._value);
        }

        public static explicit operator double(VarObejct value)
        {
            return Convert2.ToDouble(value._value);
        }

        public static explicit operator byte(VarObejct value)
        {
            return Convert2.ToByte(value._value);
        }

        public static explicit operator ushort(VarObejct value)
        {
            return Convert2.ToUInt16(value._value);
        }

        public static explicit operator uint(VarObejct value)
        {
            return Convert2.ToUInt32(value._value);
        }

        public static explicit operator Guid(VarObejct value)
        {
            return Convert2.ToGuid(value._value);
        }

        #endregion

        #region ToType
        /// <summary> 将值转为 System.Boolean 类型,如果失败,返回defaultValue
        /// </summary>
        /// <param name="defaultValue">转换失败时返回的值</param>
        public bool ToBoolean(bool defaultValue)
        {

            return Convert2.ToBoolean(_value, defaultValue, false);
        }

        /// <summary> 将值转为 System.Byte 类型,如果失败,返回defaultValue
        /// </summary>
        /// <param name="defaultValue">转换失败时返回的值</param>
        public byte ToByte(byte defaultValue)
        {
            return Convert2.ToByte(_value, defaultValue, false);
        }

        /// <summary> 将值转为 System.Char 类型,如果失败,返回defaultValue
        /// </summary>
        /// <param name="defaultValue">转换失败时返回的值</param>
        public char ToChar(char defaultValue)
        {
            return Convert2.ToChar(_value, defaultValue, false);
        }

        /// <summary> 将值转为 System.DateTime 类型,如果失败,返回defaultValue
        /// </summary>
        /// <param name="defaultValue">转换失败时返回的值</param>
        public DateTime ToDateTime(DateTime defaultValue)
        {
            return Convert2.ToDateTime(_value, defaultValue, false);
        }

        /// <summary> 将值转为 System.Decimal 类型,如果失败,返回defaultValue
        /// </summary>
        /// <param name="defaultValue">转换失败时返回的值</param>
        public decimal ToDecimal(decimal defaultValue)
        {
            return Convert2.ToDecimal(_value, defaultValue, false);
        }

        /// <summary> 将值转为 System.Double 类型,如果失败,返回defaultValue
        /// </summary>
        /// <param name="defaultValue">转换失败时返回的值</param>
        public double ToDouble(double defaultValue)
        {
            return Convert2.ToDouble(_value, defaultValue, false);
        }

        /// <summary> 将值转为 System.Int16 类型,如果失败,返回defaultValue
        /// </summary>
        /// <param name="defaultValue">转换失败时返回的值</param>
        public short ToInt16(short defaultValue)
        {
            return Convert2.ToInt16(_value, defaultValue, false);
        }

        /// <summary> 将值转为 System.Int32 类型,如果失败,返回defaultValue
        /// </summary>
        /// <param name="defaultValue">转换失败时返回的值</param>
        public int ToInt32(int defaultValue)
        {
            return Convert2.ToInt32(_value, defaultValue, false);
        }

        /// <summary> 将值转为 System.Int64 类型,如果失败,返回defaultValue
        /// </summary>
        /// <param name="defaultValue">转换失败时返回的值</param>
        public long ToInt64(long defaultValue)
        {
            return Convert2.ToInt64(_value, defaultValue, false);
        }

        /// <summary> 将值转为 System.SByte 类型,如果失败,返回defaultValue
        /// </summary>
        /// <param name="defaultValue">转换失败时返回的值</param>
        public sbyte ToSByte(sbyte defaultValue)
        {
            return Convert2.ToSByte(_value, defaultValue, false);
        }

        /// <summary> 将值转为 System.Single 类型,如果失败,返回defaultValue
        /// </summary>
        /// <param name="defaultValue">转换失败时返回的值</param>
        public float ToSingle(float defaultValue)
        {
            return Convert2.ToSingle(_value, defaultValue, false);
        }

        /// <summary> 将值转为 System.String 类型,如果值为null,返回defaultValue
        /// </summary>
        /// <param name="defaultValue">转换失败时返回的值</param>
        public string ToString(string defaultValue)
        {
            return Convert2.ToString(_value, defaultValue, false);
        }

        /// <summary> 将值转为 System.UInt16 类型,如果失败,返回defaultValue
        /// </summary>
        /// <param name="defaultValue">转换失败时返回的值</param>
        public ushort ToUInt16(ushort defaultValue)
        {
            return Convert2.ToUInt16(_value, defaultValue, false);
        }

        /// <summary> 将值转为 System.UInt32 类型,如果失败,返回defaultValue
        /// </summary>
        /// <param name="defaultValue">转换失败时返回的值</param>
        public uint ToUInt32(uint defaultValue)
        {
            return Convert2.ToUInt32(_value, defaultValue, false);
        }

        /// <summary> 将值转为 System.UInt64 类型,如果失败,返回defaultValue
        /// </summary>
        /// <param name="defaultValue">转换失败时返回的值</param>
        public ulong ToUInt64(ulong defaultValue)
        {
            return Convert2.ToUInt64(_value, defaultValue, false);
        }

        /// <summary> 将值转为 System.Guid 类型,如果失败,返回defaultValue
        /// </summary>
        /// <param name="defaultValue">转换失败时返回的值</param>
        public Guid ToGuid(Guid defaultValue)
        {
            return Convert2.ToGuid(_value, defaultValue, false);
        }

        /// <summary> 将值转为 System.Byte 数组,如果失败,返回null
        /// </summary>
        public byte[] ToByteArray()
        {
            return Convert2.ToBytes(_value, null, false);
        }

        /// <summary> 将值转为 System.Boolean 数组,如果失败,返回null
        /// </summary>
        public bool[] ToBooleanArray()
        {
            var bits = ToBitArray();
            if (bits != null)
            {
                var arr = new bool[bits.Length];
                bits.CopyTo(arr, 0);
                return arr;
            }
            return null;
        }

        /// <summary> 将值转为 System.Collections.BitArray 类型,如果失败,返回null
        /// </summary>
        public BitArray ToBitArray()
        {
            return new BitArray(Convert2.ToBytes(_value, null, false));
        }


        public override string ToString()
        {
            if (_value == null)
            {
                return null;
            }
            return _value.ToString();
        }
        #endregion

        #region IConvertible

        TypeCode IConvertible.GetTypeCode()
        {
            var conv = _value as IConvertible;
            if (conv == null)
            {
                return TypeCode.Object;
            }
            else
            {
                return conv.GetTypeCode();
            }
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            var conv = _value as IConvertible;
            if (conv == null)
            {
                return (bool)this;
            }
            else
            {
                return conv.ToBoolean(provider);
            }
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            var conv = _value as IConvertible;
            if (conv == null)
            {
                return (byte)this;
            }
            else
            {
                return conv.ToByte(provider);
            }
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            var conv = _value as IConvertible;
            if (conv == null)
            {
                return (char)this;
            }
            else
            {
                return conv.ToChar(provider);
            }
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            var conv = _value as IConvertible;
            if (conv == null)
            {
                return (DateTime)this;
            }
            else
            {
                return conv.ToDateTime(provider);
            }
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            var conv = _value as IConvertible;
            if (conv == null)
            {
                return (decimal)this;
            }
            else
            {
                return conv.ToDecimal(provider);
            }
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            var conv = _value as IConvertible;
            if (conv == null)
            {
                return (double)this;
            }
            else
            {
                return conv.ToDouble(provider);
            }
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            var conv = _value as IConvertible;
            if (conv == null)
            {
                return (short)this;
            }
            else
            {
                return conv.ToInt16(provider);
            }
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            var conv = _value as IConvertible;
            if (conv == null)
            {
                return (int)this;
            }
            else
            {
                return conv.ToInt32(provider);
            }
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            var conv = _value as IConvertible;
            if (conv == null)
            {
                return (long)this;
            }
            else
            {
                return conv.ToInt64(provider);
            }
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            var conv = _value as IConvertible;
            if (conv == null)
            {
                return (sbyte)this;
            }
            else
            {
                return conv.ToSByte(provider);
            }
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            var conv = _value as IConvertible;
            if (conv == null)
            {
                return (float)this;
            }
            else
            {
                return conv.ToSingle(provider);
            }
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            var conv = _value as IConvertible;
            if (conv == null)
            {
                return (string)this;
            }
            else
            {
                return conv.ToString(provider);
            }
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            var conv = _value as IConvertible;
            if (conv == null)
            {
                return Convert.ChangeType(this._value, conversionType, provider);
            }
            else
            {
                return conv.ToType(conversionType, provider);
            }
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            var conv = _value as IConvertible;
            if (conv == null)
            {
                return (ushort)this;
            }
            else
            {
                return conv.ToUInt16(provider);
            }
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            var conv = _value as IConvertible;
            if (conv == null)
            {
                return (uint)this;
            }
            else
            {
                return conv.ToUInt32(provider);
            }
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            var conv = _value as IConvertible;
            if (conv == null)
            {
                return (ulong)(long)this;
            }
            else
            {
                return conv.ToUInt64(provider);
            }
        }

        #endregion

    }
}
