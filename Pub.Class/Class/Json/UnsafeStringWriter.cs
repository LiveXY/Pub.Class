using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Pub.Class {
    /// <summary> 以非安全方式访问指针操作字符串直接写入内存,以提高字符串拼接效率
    /// </summary>
    [DebuggerDisplay("长度:{Length} 内容: {DebugInfo}")]
    public unsafe class UnsafeStringWriter : IDisposable {
        #region 字段
        /// <summary> 一级缓冲指针
        /// </summary>
        Char* _current;
        /// <summary> 二级缓冲
        /// </summary>
        readonly string[] _buffer = new string[8];
        /// <summary> 备用二级缓冲索引
        /// </summary>
        int _bufferIndex;
        /// <summary> 总字符数
        /// </summary>
        int _length;
        /// <summary> 结束位,一级缓冲长度减一
        /// </summary>
        int _endPosition;
        /// <summary> 以及缓冲当前位置
        /// </summary>
        int _position;
        #endregion

        /// <summary> 获取当前实例中的字符串总长度
        /// </summary>
        public int Length {
            get {
                return _length + _position;
            }
        }

        #region 私有方法
        /// <summary> 在调试器的变量窗口中的显示的信息
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private string DebugInfo {
            get {
                string str = ToString();
                if (str.Length > 70) {
                    var s = str;
                    str = s.Substring(0, 30) + "  ......  ";
                    str += s.Substring(s.Length - 30);
                }
                return str;
            }
        }
        /// <summary> 尝试在一级缓冲区写入一个字符
        /// <para>如果一级缓冲区已满,将会自动调用Flush方法转移一级缓冲区中的内容</para>
        /// </summary>
        private void TryWrite() {
            if (_position > _endPosition) {
                Flush();
            } else if (_endPosition == int.MaxValue) {
                throw new Exception("指针尚未准备就绪!");
            }
        }
        /// <summary> 尝试在一级缓冲区写入指定数量的字符
        /// </summary>
        /// <para>如果尝试写入的字符数大于一级缓冲区的大小,返回false</para>
        /// <para>如果尝试写入的字符数超出一级缓冲区剩余容量,自动调用Flush方法</para>
        /// <param name="count">尝试写入的字符数</param>
        /// <returns></returns>
        private bool TryWrite(int count) {
            if (count >= _endPosition) {
                return false;
            }
            var pre = _position + count;
            if (pre >= _endPosition) {
                Flush();
            } else if (_endPosition == int.MaxValue) {
                throw new Exception("指针尚未准备就绪!");
            }
            return true;
        }
        #endregion

        #region Append
        /// <summary> 将 Boolean 对象转换为字符串追加到当前实例。
        /// </summary>
        public UnsafeStringWriter Append(Boolean val) {
            if (val) {
                TryWrite(4);
                _current[_position++] = 't';
                _current[_position++] = 'r';
                _current[_position++] = 'u';
                _current[_position++] = 'e';
            } else {
                TryWrite(5);
                _current[_position++] = 'f';
                _current[_position++] = 'a';
                _current[_position++] = 'l';
                _current[_position++] = 's';
                _current[_position++] = 'e';
            }
            return this;
        }
        /// <summary> 将 DateTime 对象转换为字符串追加到当前实例。
        /// </summary>
        public UnsafeStringWriter Append(DateTime val) {
            TryWrite(18);
            int a = val.Year;
            #region 年
            if (a > 999) {
                _current[_position++] = (char)(a / 1000 + '0');
                a = a % 1000;
                _current[_position++] = (char)(a / 100 + '0');
                a = a % 100;
                _current[_position++] = (char)(a / 10 + '0');
                a = a % 10;
            } else if (a > 99) {
                _current[_position++] = '0';
                _current[_position++] = (char)(a / 100 + '0');
                a = a % 100;
                _current[_position++] = (char)(a / 10 + '0');
                a = a % 10;
            } else if (a > 9) {
                _current[_position++] = '0';
                _current[_position++] = '0';
                _current[_position++] = (char)(a / 10 + '0');
                a = a % 10;
            } else {
                _current[_position++] = '0';
                _current[_position++] = '0';
                _current[_position++] = '0';
            }

            _current[_position++] = (char)(a + '0');
            #endregion
            _current[_position++] = '-';
            a = val.Month;
            #region 月
            if (a > 9) {
                _current[_position++] = (char)(a / 10 + '0');
                a = a % 10;
            } else {
                _current[_position++] = '0';
            }
            _current[_position++] = (char)(a + '0');
            #endregion
            a = val.Day;
            _current[_position++] = '-';
            #region 日
            if (a > 9) {
                _current[_position++] = (char)(a / 10 + '0');
                a = a % 10;
            } else {
                _current[_position++] = '0';
            }
            _current[_position++] = (char)(a + '0');
            #endregion
            a = val.Hour;
            _current[_position++] = ' ';
            #region 时
            if (a > 9) {
                _current[_position++] = (char)(a / 10 + '0');
                a = a % 10;
            } else {
                _current[_position++] = '0';
            }
            _current[_position++] = (char)(a + '0');
            #endregion
            a = val.Minute;
            _current[_position++] = ':';
            #region 分
            if (a > 9) {
                _current[_position++] = (char)(a / 10 + '0');
                a = a % 10;
            } else {
                _current[_position++] = '0';
            }
            _current[_position++] = (char)(a + '0');
            #endregion
            a = val.Second;
            _current[_position++] = ':';
            #region 秒
            if (a > 9) {
                _current[_position++] = (char)(a / 10 + '0');
                a = a % 10;
            } else {
                _current[_position++] = '0';
            }
            _current[_position++] = (char)(a + '0');
            #endregion
            return this;
        }
        /// <summary> 将 Guid 对象转换为字符串追加到当前实例。
        /// </summary>
        public UnsafeStringWriter Append(Guid val) {
            Append(val.ToString());
            return this;
        }
        /// <summary> 将 Decimal 对象转换为字符串追加到当前实例。
        /// </summary>
        public UnsafeStringWriter Append(Decimal val) {
            Append(val.ToString(CultureInfo.InvariantCulture));
            return this;
        }
        /// <summary> 将 Double 对象转换为字符串追加到当前实例。
        /// </summary>
        public UnsafeStringWriter Append(Double val) {
            Append(Convert.ToString(val));
            return this;
        }
        /// <summary> 将 Single 对象转换为字符串追加到当前实例。
        /// </summary>
        public UnsafeStringWriter Append(Single val) {
            Append(Convert.ToString(val));
            return this;
        }
        /// <summary> 将 SByte 对象转换为字符串追加到当前实例。
        /// </summary>
        public UnsafeStringWriter Append(SByte val) {
            if (val == 0) {
                TryWrite();
                _current[_position++] = '0';
                return this;
            }
            if (val < 0) {
                TryWrite();
                val *= -1;
                _current[_position++] = '-';
            }
            if (val < 10) {
                TryWrite();
                _current[_position++] = (char)('0' + val);
            } else if (val < 100) {
                if (TryWrite(2)) {
                    Flush();
                }
                _current[_position++] = (char)('0' + val / 10);
                _current[_position++] = (char)('0' + val % 10);
            } else {
                if (TryWrite(3)) {
                    Flush();
                }
                _current[_position++] = (char)('0' + val / 100);
                _current[_position++] = (char)('0' + val / 10 % 10);
                _current[_position++] = (char)('0' + val % 10);
            }
            return this;
        }
        /// <summary> 将 Int16 对象转换为字符串追加到当前实例。
        /// </summary>
        public UnsafeStringWriter Append(Int16 val) {
            if (val == 0) {
                TryWrite();
                _current[_position++] = '0';
                return this;
            }
            if (val < 0) {
                TryWrite();
                val *= -1;
                _current[_position++] = '-';
            }
            if (val < 10) {
                TryWrite();
                _current[_position++] = (char)('0' + val);
            } else if (val < 100) {
                if (TryWrite(2)) {
                    Flush();
                }
                _current[_position++] = (char)('0' + val / 10);
                _current[_position++] = (char)('0' + val % 10);
            } else if (val < 1000) {
                if (TryWrite(3)) {
                    Flush();
                }
                _current[_position++] = (char)('0' + val / 100);
                _current[_position++] = (char)('0' + val / 10 % 10);
                _current[_position++] = (char)('0' + val % 10);
            } else if (val < 10000) {
                if (TryWrite(4)) {
                    Flush();
                }
                _current[_position++] = (char)('0' + val / 1000);
                _current[_position++] = (char)('0' + val / 100 % 10);
                _current[_position++] = (char)('0' + val / 10 % 10);
                _current[_position++] = (char)('0' + val % 10);
            } else {
                if (TryWrite(5)) {
                    Flush();
                }
                _current[_position++] = (char)('0' + val / 10000);
                _current[_position++] = (char)('0' + val / 1000 % 10);
                _current[_position++] = (char)('0' + val / 100 % 10);
                _current[_position++] = (char)('0' + val / 10 % 10);
                _current[_position++] = (char)('0' + val % 10);
            }
            return this;
        }
        /// <summary> 将 Int32 对象转换为字符串追加到当前实例。
        /// </summary>
        public UnsafeStringWriter Append(Int32 val) {
            Append((Int64)val);
            return this;
        }
        /// <summary> 将 Int64 对象转换为字符串追加到当前实例。
        /// </summary>
        public UnsafeStringWriter Append(Int64 val) {
            if (val == 0) {
                TryWrite();
                _current[_position++] = '0';
                return this;
            }

            char[] arr = new char[64];
            fixed (char* a = arr) {
                char* number = a;

                var pos = 63;
                if (val < 0) {
                    _current[_position++] = '-';
                    number[pos] = (char)(~(val % 10) + '1');
                    if (val < -10) {
                        val = val / -10;
                        number[--pos] = (char)(val % 10 + '0');
                    }
                } else {
                    number[pos] = (char)(val % 10 + '0');
                }
                while ((val = val / 10L) != 0L) {
                    number[--pos] = (char)(val % 10L + '0');
                }
                var length = 64 - pos;
                Append(number, pos, length);
            }
            return this;
        }
        /// <summary> 将 Char 对象转换为字符串追加到当前实例。
        /// </summary>
        public UnsafeStringWriter Append(Char val) {
            TryWrite();
            _current[_position++] = val;
            return this;
        }
        /// <summary> 将 Byte 对象转换为字符串追加到当前实例。
        /// </summary>
        public UnsafeStringWriter Append(Byte val) {
            if (val == 0) {
                TryWrite();
                _current[_position++] = '0';
            } else if (val < 10) {
                TryWrite();
                _current[_position++] = (char)('0' + val);
            } else if (val < 100) {
                if (TryWrite(2) == false) {
                    Flush();
                }
                _current[_position++] = (char)('0' + val / 10);
                _current[_position++] = (char)('0' + val % 10);
            } else {
                if (TryWrite(3) == false) {
                    Flush();
                }
                _current[_position++] = (char)('0' + val / 100);
                _current[_position++] = (char)('0' + val / 10 % 10);
                _current[_position++] = (char)('0' + val % 10);
            }
            return this;
        }
        /// <summary> 将 UInt16 对象转换为字符串追加到当前实例。
        /// </summary>
        public UnsafeStringWriter Append(UInt16 val) {
            if (val == 0) {
                TryWrite();
                _current[_position++] = '0';
                return this;
            }
            if (val < 10) {
                TryWrite();
                _current[_position++] = (char)('0' + val);
            } else if (val < 100) {
                if (TryWrite(2)) {
                    Flush();
                }
                _current[_position++] = (char)('0' + val / 10);
                _current[_position++] = (char)('0' + val % 10);
            } else if (val < 1000) {
                if (TryWrite(3)) {
                    Flush();
                }
                _current[_position++] = (char)('0' + val / 100);
                _current[_position++] = (char)('0' + val / 10 % 10);
                _current[_position++] = (char)('0' + val % 10);
            } else if (val < 10000) {
                if (TryWrite(4)) {
                    Flush();
                }
                _current[_position++] = (char)('0' + val / 1000);
                _current[_position++] = (char)('0' + val / 100 % 10);
                _current[_position++] = (char)('0' + val / 10 % 10);
                _current[_position++] = (char)('0' + val % 10);
            } else {
                if (TryWrite(5)) {
                    Flush();
                }
                _current[_position++] = (char)('0' + val / 10000);
                _current[_position++] = (char)('0' + val / 1000 % 10);
                _current[_position++] = (char)('0' + val / 100 % 10);
                _current[_position++] = (char)('0' + val / 10 % 10);
                _current[_position++] = (char)('0' + val % 10);
            }
            return this;
        }
        /// <summary> 将 UInt32 对象转换为字符串追加到当前实例。
        /// </summary>
        public UnsafeStringWriter Append(UInt32 val) {
            Append((UInt64)val);
            return this;
        }
        /// <summary> 将 UInt64 对象转换为字符串追加到当前实例。
        /// </summary>
        public UnsafeStringWriter Append(UInt64 val) {
            if (val == 0) {
                TryWrite();
                _current[_position++] = '0';
                return this;
            }
            var arr = new char[64];
            fixed (char* a = arr) {
                char* number = a;
                var pos = 63;

                number[pos] = (char)(val % 10 + '0');

                while ((val = val / 10L) != 0L) {
                    number[--pos] = (char)(val % 10L + '0');
                }
                var length = 64 - pos;
                Append(number, pos, length);
            }
            return this;
        }
        /// <summary> 将字符串追加到当前实例。
        /// </summary>
        public UnsafeStringWriter Append(String val) {
            if (val == null) {
                return this;
            }
            var length = val.Length;
            if (length == 0) {
                return this;
            }
            if (length <= 3) {
                _current[_position++] = val[0];
                TryWrite(length);
                if (length > 2) {
                    _current[_position++] = val[1];
                    _current[_position++] = val[2];
                } else if (length > 1) {
                    _current[_position++] = val[1];
                }
            } else if (TryWrite(length)) {
                fixed (char* c = val) {
                    int* p2;
                    if ((length & 1) != 0) {
                        _current[_position++] = c[0];
                        p2 = ((int*)(c + 1));
                        length--;
                    } else {
                        p2 = ((int*)c);
                    }
                    int* p1 = (int*)&_current[_position];


                    _position += length;
                    while (length >= 8) {
                        (*p1++) = *(p2++);
                        (*p1++) = *(p2++);
                        (*p1++) = *(p2++);
                        (*p1++) = *(p2++);
                        length -= 8;
                    }
                    if ((length & 4) != 0) {
                        (*p1++) = *(p2++);
                        (*p1++) = *(p2++);
                    }
                    if ((length & 2) != 0) {
                        (*p1) = *(p2);
                    }
                }
            } else {
                Flush();
                _buffer[_bufferIndex++] = val;
                _length += val.Length;
            }
            return this;
        }
        /// <summary> 将内存中的字符串追加到当前实例。
        /// </summary>
        /// <param name="point">内存指针</param>
        /// <param name="offset">指针偏移量</param>
        /// <param name="length">字符长度</param>
        /// <returns></returns>
        internal UnsafeStringWriter Append(char* point, int offset, int length) {
            if (length > 0) {
                if (TryWrite(length)) {
                    char* c = point + offset;
                    if ((length & 1) != 0) {
                        _current[_position++] = c[0];
                        c++;
                        length--;
                    }
                    int* p1 = (int*)&_current[_position];
                    int* p2 = ((int*)c);
                    _position += length;
                    while (length >= 8) {
                        (*p1++) = *(p2++);
                        (*p1++) = *(p2++);
                        (*p1++) = *(p2++);
                        (*p1++) = *(p2++);
                        length -= 8;
                    }
                    if ((length & 4) != 0) {
                        (*p1++) = *(p2++);
                        (*p1++) = *(p2++);
                    }
                    if ((length & 2) != 0) {
                        (*p1) = *(p2);
                    }
                } else {
                    Flush();
                    _buffer[_bufferIndex++] = new string(point, offset, length);
                    _length += length;
                }
            }

            return this;
        }
        /// <summary> 将可格式化对象,按指定的格式转换为字符串追加到当前实例。
        /// </summary>
        public UnsafeStringWriter AppendFormat(IFormattable val, string format) {
            Append(val.ToString(format, null));
            return this;
        }

        /// <summary> 将字符串集合追加到当前实例。
        /// </summary>
        /// <param name="strings">字符串集合</param>
        /// <returns></returns>
        public UnsafeStringWriter Append(IEnumerable<string> strings) {
            foreach (var str in strings) {
                Append(str);
            }
            return this;
        }
        /// <summary> 将字符串集合追加到当前实例并追加回车换行。
        /// </summary>
        /// <param name="str">追加到集合的字符串</param>
        /// <returns></returns>
        public UnsafeStringWriter AppendLine(string str) {
            Append(str);
            Append(Environment.NewLine);
            return this;
        }

        #endregion

        /// <summary> 由于调用对象将内存指针固定后,通知当前实例指针准备就绪
        /// </summary>
        /// <param name="point">内存指针</param>
        /// <param name="length">一级缓冲长度0~65536</param>
        /// <returns></returns>
        public UnsafeStringWriter Ready(Char* point, ushort length) {
            if (point == null) {
                throw new ArgumentNullException("point");
            }
            Close();
            _endPosition = length - 1;
            _current = point;
            return this;
        }

        public char[] FixedPointer(ushort length = 4096) {
            var arr = new char[length];
            fixed (char* p = arr) {
                Ready(p, length);
                return arr;
            }
        }

        /// <summary> 关闭当前实例
        /// <para>
        /// 该行为将清空所有缓冲区中的内容,
        /// 并阻止除Ready,Close以外的方法调用,直到再次调用Ready方法
        /// </para>
        /// </summary>
        public void Close() {
            _buffer[0] = _buffer[1] =
            _buffer[2] = _buffer[3] =
            _buffer[4] = _buffer[5] =
            _buffer[6] = _buffer[7] = null;
            _length = 0;
            _position = 0;
            _endPosition = int.MaxValue;
            _current = null;
        }

        /// <summary> 清理当前实例的一级缓冲区的内容，使所有缓冲数据写入二级缓冲区。
        /// </summary>
        public void Flush() {
            if (_position > 0) {
                _length += _position;
                if (_bufferIndex == 8) {
                    _buffer[0] = string.Concat(_buffer[0], _buffer[1], _buffer[2], _buffer[3]);
                    _buffer[1] = string.Concat(_buffer[4], _buffer[5], _buffer[6], _buffer[7]);
                    _buffer[2] = new string(_current, 0, _position);
                    _buffer[3] =
                    _buffer[4] =
                    _buffer[5] =
                    _buffer[6] =
                    _buffer[7] = null;
                    _bufferIndex = 3;
                } else {
                    _buffer[_bufferIndex++] = new string(_current, 0, _position);
                }
                _position = 0;
            }
        }

        /// <summary> 返回当前实例中的字符串
        /// </summary>
        public override string ToString() {
            if (_bufferIndex == 0) {
                return new string(_current, 0, _position);
            }
            if (_bufferIndex <= 3) {
                return string.Concat(_buffer[0], _buffer[1], _buffer[2], new string(_current, 0, _position));
            }
            return string.Concat(
                                 _buffer[0], _buffer[1], _buffer[2], _buffer[3],
                                 _buffer[4], _buffer[5], _buffer[6], _buffer[7],
                                 new string(_current, 0, _position));
        }

        public void Dispose() {
            Close();
        }

    }
}