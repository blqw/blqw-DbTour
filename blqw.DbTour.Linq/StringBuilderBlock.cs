using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    public sealed class StringBuilderBlock
    {
        private StringBuilder _Buffer;
        private int _Start;
        public int Length { get; private set; }
        private Action<int> _SendChangedLength;

        public static StringBuilderBlock[] Array(int count)
        {
            var array = new StringBuilderBlock[count];
            var sb = new StringBuilder();
            Action<int> action = null;
            for (int i = count - 1; i >= 0; i--)
            {
                var sbb = new StringBuilderBlock { _Buffer = sb,_Start = sb.Length };
                sbb._SendChangedLength = action;
                action = sbb.OnChangedStart;
                array[i] = sbb;
            }
            return array;
        }

        public string AllString()
        {
            return _Buffer.ToString();
        }
        private void OnChangedStart(int count)
        {
            _Start += count;
            if (_SendChangedLength != null)
            {
                _SendChangedLength(count);
            }
        }
        private void OnChangedLength(int count)
        {
            Length += count;
            if (_SendChangedLength != null)
            {
                _SendChangedLength(count);
            }
        }

        public override string ToString()
        {
            if (Length == 0)
            {
                return "";
            }
            return _Buffer.ToString(_Start, Length);
        }

        public StringBuilderBlock Append(string value)
        {
            var count = _Buffer.Length;
            _Buffer.Insert(_Start + Length, value);
            OnChangedLength(_Buffer.Length - count);
            return this;
        }
        public StringBuilderBlock Append(char value)
        {
            var count = _Buffer.Length;
            _Buffer.Insert(_Start + Length, value);
            OnChangedLength(_Buffer.Length - count);
            return this;
        }
        public StringBuilderBlock Append(int value)
        {
            var count = _Buffer.Length;
            _Buffer.Insert(_Start + Length, value);
            OnChangedLength(_Buffer.Length - count);
            return this;
        }
        public StringBuilderBlock Append(object value)
        {
            var count = _Buffer.Length;
            _Buffer.Insert(_Start + Length, value);
            OnChangedLength(_Buffer.Length - count);
            return this;
        }
        public StringBuilderBlock AppendFormat(string format, params object[] args)
        {
            var count = _Buffer.Length;
            _Buffer.Insert(_Start + Length, string.Format(format, args));
            OnChangedLength(_Buffer.Length - count);
            return this;
        }
        public StringBuilderBlock AppendLine()
        {
            var count = _Buffer.Length;
            _Buffer.Insert(_Start + Length, Environment.NewLine);
            OnChangedLength(_Buffer.Length - count);
            return this;
        }
        public StringBuilderBlock AppendLine(string value)
        {
            var count = _Buffer.Length;
            _Buffer.Insert(_Start + Length, Environment.NewLine);
            _Buffer.Insert(_Start + Length, value);
            OnChangedLength(_Buffer.Length - count);
            return this;
        }
        public StringBuilderBlock AppendLine(object value)
        {
            var count = _Buffer.Length;
            _Buffer.Insert(_Start + Length, Environment.NewLine);
            _Buffer.Insert(_Start + Length, value);
            OnChangedLength(_Buffer.Length - count);
            return this;
        }
        public StringBuilderBlock Clear()
        {
            var count = _Buffer.Length;
            _Buffer.Remove(_Start, Length);
            OnChangedLength(_Buffer.Length - count);
            return this;
        }
        public StringBuilderBlock Remove(int startIndex, int length)
        {
            if (startIndex < 0 || length < 0 || startIndex + length > Length)
            {
                throw new ArgumentOutOfRangeException("如果 startIndex 或 length 小于零，或者 startIndex+length 大于此实例的长度。");
            }
            var count = _Buffer.Length;
            _Buffer.Remove(_Start + startIndex, length);
            OnChangedLength(_Buffer.Length - count);
            return this;
        }
        public StringBuilderBlock Remove(int length)
        {
            if (length < 0 || length > Length)
            {
                throw new ArgumentOutOfRangeException("如果 length 小于零，或者 length 大于此实例的长度。");
            }
            var count = _Buffer.Length;
            _Buffer.Remove(_Start + Length - length, length);
            OnChangedLength(_Buffer.Length - count);
            return this;
        }

    }
}
