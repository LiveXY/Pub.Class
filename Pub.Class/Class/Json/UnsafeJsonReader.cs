using System;
using System.Globalization;
using System.Diagnostics;

namespace Pub.Class {
    [DebuggerDisplay("当前字符: {Current}")]
    unsafe class UnsafeJsonReader : IDisposable {
        /// <summary>
        /// <para>包含1: 可以为头的字符</para>
        /// <para>包含2: 可以为单词的字符</para>
        /// <para>包含4: 可以为数字的字符</para>
        /// <para>等于8: 空白字符</para>
        /// <para>包含16:转义字符</para>
        /// <para></para>
        /// </summary>
        private readonly static byte[] _WordChars = new byte[char.MaxValue];
        private readonly static sbyte[] _UnicodeFlags = new sbyte[123];
        private readonly static sbyte[, ,] _DateTimeWords;
        static UnsafeJsonReader() {
            for (int i = 0; i < 123; i++) {
                _UnicodeFlags[i] = -1;
            }

            _WordChars['-'] = 1 | 4;
            _WordChars['+'] = 1 | 4;

            _WordChars['$'] = 1 | 2;
            _WordChars['_'] = 1 | 2;
            for (char c = 'a'; c <= 'z'; c++) {
                _WordChars[c] = 1 | 2;
                _UnicodeFlags[c] = (sbyte)(c - 'a' + 10);
            }
            for (char c = 'A'; c <= 'Z'; c++) {
                _WordChars[c] = 1 | 2;
                _UnicodeFlags[c] = (sbyte)(c - 'A' + 10);
            }

            _WordChars['.'] = 1 | 2 | 4;
            for (char c = '0'; c <= '9'; c++) {
                _WordChars[c] = 4;
                _UnicodeFlags[c] = (sbyte)(c - '0');
            }

            //科学计数法
            _WordChars['e'] |= 4;
            _WordChars['E'] |= 4;

            _WordChars[' '] = 8;
            _WordChars['\t'] = 8;
            _WordChars['\r'] = 8;
            _WordChars['\n'] = 8;


            _WordChars['t'] |= 16;
            _WordChars['r'] |= 16;
            _WordChars['n'] |= 16;
            _WordChars['f'] |= 16;
            _WordChars['0'] |= 16;
            _WordChars['"'] |= 16;
            _WordChars['\''] |= 16;
            _WordChars['\\'] |= 16;
            _WordChars['/'] |= 16;


            string[] a = { "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec" };
            string[] b = { "mon", "tue", "wed", "thu", "fri", "sat", "sun" };
            _DateTimeWords = new sbyte[23, 21, 25];

            for (sbyte i = 0; i < a.Length; i++) {
                var d = a[i];
                _DateTimeWords[d[0] - 97, d[1] - 97, d[2] - 97] = (sbyte)(i + 1);
            }

            for (sbyte i = 0; i < b.Length; i++) {
                var d = b[i];
                _DateTimeWords[d[0] - 97, d[1] - 97, d[2] - 97] = (sbyte)-(i + 1);
            }
            _DateTimeWords['g' - 97, 'm' - 97, 't' - 97] = sbyte.MaxValue;
        }

        Char* _p;
        int _position;
        readonly int _length;
        int _end;
        public UnsafeJsonReader(Char* origin, int length) {
            if (origin == null) {
                throw new ArgumentNullException("origin");
            }
            if (length <= 0) {
                throw new ArgumentOutOfRangeException("length");
            }
            _p = origin;
            _length = length;
            _end = length - 1;
            _position = 0;
            Current = *origin;
        }

        public char Current { get; private set; }

        /// <summary> 当前位置
        /// </summary>
        public int Position {
            get { return _position; }
            set {
                if (_position >= _length) {
                    throw new ArgumentOutOfRangeException();
                }
                if (_isDisposed == false) {
                    _position = value;
                    Current = _p[_position];
                }
            }
        }

        /// <summary> 是否已经到结尾,忽略空白
        /// </summary>
        public bool IsEnd() {
            if (_position > _end) {
                return true;
            }
            if (_WordChars[Current] == 8) {
                while (_position < _end) {
                    _position++;
                    Current = _p[_position];
                    if (_WordChars[Current] != 8) {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary> 移动到下一个字符,如果已经是结尾则抛出异常
        /// </summary>
        public void MoveNext() {
            if (_position < _end) {
                _position++;
                Current = _p[_position];
            } else if (_position == _end) {
                _position++;
                Current = '\0';
            } else {
                ThrowException();
            }
        }

        /// <summary> 跳过一个单词
        /// </summary>
        public void SkipWord() {
            if (IsEnd())//已到结尾
            {
                ThrowException();
            }

            if (_WordChars[Current] != 3)      //只能是3 可以为开头的单词
            {
                ThrowException();
            }
            MoveNext();
            while ((_WordChars[Current] & 2) != 0)//读取下一个字符 可是是单词
            {
                MoveNext();
            }
        }

        /// <summary> 跳过一个指定字符,忽略空白,如果字符串意外结束抛出异常
        /// </summary>
        public bool SkipChar(char c) {
            if (IsEnd()) {
                ThrowException();
            }
            if (Current == c) {
                if (_position > _end) {
                    _position++;
                    Current = '\0';
                } else {
                    _position++;
                    Current = _p[_position];
                }
                return true;
            }
            return false;
        }

        /// <summary> 跳过一个字符串
        /// </summary>
        public void SkipString() {
            if (IsEnd())//已到结尾
            {
                ThrowException();
            }
            Char quot = Current;
            if (quot != '"' && quot != '\'') {
                ThrowException();
            }
            MoveNext();
            while (Current != quot)//是否是结束字符
            {
                MoveNext();
                if (Current == '\\')//是否是转义符
                {
                    if ((_WordChars[Current] & 16) == 0) {
                        ThrowException();
                    }
                    MoveNext();
                }
            }
            MoveNext();
        }
        //袖珍版字符串处理类
        sealed class MiniBuffer : IDisposable {
            char* _p;
            string[] _str;
            int _index;
            int _position;
            public MiniBuffer(char* p) {
                _p = p;
            }

            public void AddString(char* point, int offset, int length) {
                if (length > 0) {
                    if (_position + length > 255) {
                        Flush();
                        if (length > 200) {
                            var s = new string(point, offset, length - 1);
                            if (_index == 3) {
                                _str[0] = string.Concat(_str[0], _str[1], _str[2], s);
                                _index = 1;
                            } else {
                                _str[_index++] = s;
                            }
                            return;
                        }
                    }


                    char* c = point + offset;
                    if ((length & 1) != 0) {
                        _p[_position++] = c[0];
                        c++;
                        length--;
                    }
                    int* p1 = (int*)&_p[_position];
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

                }
            }

            public void AddChar(char c) {
                if (_position == 255) {
                    Flush();
                }
                _p[_position++] = c;
            }

            private void Flush() {
                if (_str == null) {
                    _str = new string[3];
                    _str[_index++] = new string(_p, 0, _position);
                    _index = 1;
                } else if (_index < 3) {
                    _str[_index++] = new string(_p, 0, _position);
                } else {
                    _str[0] = string.Concat(_str[0], _str[1], _str[2], new string(_p, 0, _position));
                    _index = 1;
                }
                _position = 0;
            }

            public override string ToString() {
                if (_str == null) {
                    return new string(_p, 0, _position);
                }

                if (_index == 1) {
                    return string.Concat(_str[0], new string(_p, 0, _position));
                }
                if (_index == 2) {
                    return string.Concat(_str[0], _str[1], new string(_p, 0, _position));
                }
                return string.Concat(_str[0], _str[1], _str[2], new string(_p, 0, _position));
            }

            public void Dispose() {
                _p = null;
                _str = null;
            }
        }

        /// <summary> 读取时间类型的对象
        /// </summary>
        /// <returns></returns>
        public object ReadDateTime() {
            if (IsEnd())//已到结尾
            {
                ThrowException();
            }

            Char quot = Current;
            if (quot != '"' && quot != '\'') {
                ThrowException();
            }
            MoveNext();
            if (Current == quot) {
                MoveNext();
                return null;
            }
            var index = _position;

            //0年,1月,2日,3时,4分,5秒,6毫秒,7星期,8+12,9gmt,10 月
            int[] datetime = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
            int numindex = 0;
            if (IsEnd())//跳过空白
            {
                ThrowException();
            }

            int number = -1;

            do {
                if (Current >= '0' && Current <= '9') {
                    number = ReadPositiveInteger();
                } else if (_WordChars[Current] == 3) {
                    var wk = GetDateTimeWord();
                    if (wk == 0) goto label_parse;
                    if (wk == sbyte.MaxValue) {
                        if (datetime[9] >= 0) goto label_parse;
                        datetime[9] = 8;
                    } else if (wk > 0) {
                        if (datetime[10] >= 0) goto label_parse;
                        datetime[10] = wk;
                    } else {
                        if (datetime[7] >= 0) goto label_parse;
                        datetime[7] = -wk;
                    }
                } else if (Current == quot) {
                    if (number >= 0) {
                        datetime[numindex++] = number;
                    }
                    break;
                } else {
                    switch (Current) {
                        case '-':
                        case '/':
                            if (number < 0 || numindex > 2) goto label_parse;
                            datetime[numindex++] = number;
                            break;
                        case ' ':
                            break;
                        case ':':
                            if (numindex < 3) numindex = 3;
                            datetime[numindex++] = number;
                            break;
                        case '.':
                            if (numindex != 6) goto label_parse;
                            datetime[6] = number;
                            break;
                        case 'a':
                        case 'A':
                            MoveNext();
                            if (Current != 'm' && Current != 'M') goto label_parse;
                            if (datetime[8] >= 0) goto label_parse;
                            datetime[8] = 0;
                            break;
                        case 'p':
                        case 'P':
                            MoveNext();
                            if (Current != 'm' && Current != 'M') goto label_parse;
                            if (datetime[8] >= 0) goto label_parse;
                            datetime[8] = 12;
                            break;
                        case '年':
                            if (datetime[0] >= 0) goto label_parse;
                            datetime[0] = number;
                            break;
                        case '月':
                            if (datetime[1] >= 0) goto label_parse;
                            datetime[1] = number;
                            break;
                        case '日':
                            if (datetime[2] >= 0) goto label_parse;
                            datetime[2] = number;
                            break;
                        case '时':
                            if (datetime[3] >= 0) goto label_parse;
                            datetime[3] = number;
                            break;
                        case '分':
                            if (datetime[4] >= 0) goto label_parse;
                            datetime[4] = number;
                            break;
                        case '秒':
                            if (datetime[5] >= 0) goto label_parse;
                            datetime[5] = number;
                            break;
                        case '上':
                            MoveNext();
                            if (Current != '午') goto label_parse;
                            if (datetime[8] >= 0) goto label_parse;
                            datetime[8] = 0;
                            break;
                        case '下':
                            MoveNext();
                            if (Current != '午') goto label_parse;
                            if (datetime[8] >= 0) goto label_parse;
                            datetime[8] = 12;
                            break;
                        case '星':
                            if (datetime[7] >= 0) goto label_parse;
                            datetime[7] = 1;
                            MoveNext();
                            if (Current != '期') goto label_parse;
                            MoveNext();
                            MoveNext();
                            break;
                        case '周':
                            if (datetime[7] >= 0) goto label_parse;
                            datetime[7] = 1;
                            MoveNext();
                            MoveNext();
                            break;
                        case ',':
                            break;
                        default:
                            goto label_parse;
                    }
                    number = -1;
                    MoveNext();
                }
            } while (Current != quot || number != -1);//是否是结束字符

            if (datetime[2] == -1 && datetime[10] >= 0) {
                datetime[2] = datetime[1];
                datetime[1] = datetime[10];
            }
            if (datetime[2] > 31) {
                datetime[10] = datetime[2];
                datetime[2] = datetime[0];
                datetime[0] = datetime[10];
            }

            MoveNext();
            if (datetime[0] > 9999 || datetime[1] > 12 || datetime[2] > 31 ||
                datetime[3] > 24 || datetime[4] > 59 || datetime[5] > 59 || datetime[6] > 999) {
                goto label_parse;
            }

            if (datetime[0] <= 0) datetime[0] = 1990;
            else if (datetime[0] < 100) datetime[0] += 1900;
            if (datetime[1] <= 0) datetime[1] = 1;
            if (datetime[2] <= 0) datetime[2] = 1;
            if (datetime[3] <= 0) datetime[3] = 0;
            if (datetime[4] <= 0) datetime[4] = 0;
            if (datetime[5] <= 0) datetime[5] = 0;
            if (datetime[6] <= 0) datetime[6] = 0;
            if (datetime[8] > 0) datetime[3] = datetime[3] + datetime[8];
            var td = new DateTime(datetime[0], datetime[1], datetime[2], datetime[3], datetime[4], datetime[5], datetime[6]);
            if (datetime[9] >= 0) {
                td = td.AddHours(datetime[9]);
            }
            return td;
        label_parse:

            while (Current != quot) {
                MoveNext();
            }
            var str = new string(_p, index, _position - index);
            return DateTime.Parse(str);
        }

        /// <summary> 获取时间中的英文字符,返回127 = GMT, 大于0 表示月份, 小于0 表示星期
        /// </summary>
        /// <returns></returns>
        private int GetDateTimeWord() {
            char[] c = new char[3];
            for (int i = 0; i < 3; i++) {
                if (Current >= 'a' && Current <= 'z') {
                    c[i] = (char)(Current - 'a');
                } else if (Current >= 'A' && Current <= 'Z') {
                    c[i] = (char)(Current - 'A');
                } else {
                    return 0;
                }
                MoveNext();
            }
            return _DateTimeWords[c[0], c[1], c[2]];
        }
        /// <summary> 读取正整数,在ReadDateTime函数中使用
        /// </summary>
        /// <returns></returns>
        private int ReadPositiveInteger() {
            if (Current < '0' || Current > '9') {
                return -1;
            }
            int num = 0;
            do {
                num = num * 10 + (Current - '0');
                MoveNext();
            } while (Current >= '0' && Current <= '9');
            return num;
        }

        /// <summary> 读取常量
        /// </summary>
        /// <returns></returns>
        public object ReadConsts() {
            if (IsEnd())//已到结尾
            {
                ThrowException();
            }
            switch (Current) {
                case '\'':
                case '"':
                    return string.Empty;
                case 'f'://false
                    MoveNext();
                    if (Current != 'a') ThrowException();
                    MoveNext();
                    if (Current != 'l') ThrowException();
                    MoveNext();
                    if (Current != 's') ThrowException();
                    MoveNext();
                    if (Current != 'e') ThrowException();
                    MoveNext();
                    return false;
                case 't'://true
                    MoveNext();
                    if (Current != 'r') ThrowException();
                    MoveNext();
                    if (Current != 'u') ThrowException();
                    MoveNext();
                    if (Current != 'e') ThrowException();
                    MoveNext();
                    return true;
                case 'n'://null
                    MoveNext();
                    if (Current != 'u') ThrowException();
                    MoveNext();
                    if (Current != 'l') ThrowException();
                    MoveNext();
                    if (Current != 'l') ThrowException();
                    MoveNext();
                    return null;
                case 'u'://undefined
                    MoveNext();
                    if (Current != 'n') ThrowException();
                    MoveNext();
                    if (Current != 'd') ThrowException();
                    MoveNext();
                    if (Current != 'e') ThrowException();
                    MoveNext();
                    if (Current != 'f') ThrowException();
                    MoveNext();
                    if (Current != 'i') ThrowException();
                    MoveNext();
                    if (Current != 'n') ThrowException();
                    MoveNext();
                    if (Current != 'e') ThrowException();
                    MoveNext();
                    if (Current != 'd') ThrowException();
                    MoveNext();
                    return null;
                case 'N'://NaN
                    MoveNext();
                    if (Current != 'a') ThrowException();
                    MoveNext();
                    if (Current != 'N') ThrowException();
                    MoveNext();
                    return double.NaN;
                case 'I'://Infinity
                    MoveNext();
                    if (Current != 'n') ThrowException();
                    MoveNext();
                    if (Current != 'f') ThrowException();
                    MoveNext();
                    if (Current != 'i') ThrowException();
                    MoveNext();
                    if (Current != 'n') ThrowException();
                    MoveNext();
                    if (Current != 'i') ThrowException();
                    MoveNext();
                    if (Current != 't') ThrowException();
                    MoveNext();
                    if (Current != 'y') ThrowException();
                    MoveNext();
                    return double.PositiveInfinity;
                case '-'://-Infinity
                    MoveNext();
                    if ((_WordChars[Current] & 4) > 0) {
                        _position--;
                        Current = _p[_position];
                        return ReadNumber();
                    }
                    if (Current != 'I') ThrowException();
                    MoveNext();
                    if (Current != 'n') ThrowException();
                    MoveNext();
                    if (Current != 'f') ThrowException();
                    MoveNext();
                    if (Current != 'i') ThrowException();
                    MoveNext();
                    if (Current != 'n') ThrowException();
                    MoveNext();
                    if (Current != 'i') ThrowException();
                    MoveNext();
                    if (Current != 't') ThrowException();
                    MoveNext();
                    if (Current != 'y') ThrowException();
                    MoveNext();
                    return double.NegativeInfinity;
                default:
                    if ((_WordChars[Current] & 4) > 0) {
                        return ReadNumber();
                    }
                    ThrowException();
                    return null;
            }
        }

        /// <summary> 读取单词
        /// </summary>
        /// <returns></returns>
        public string ReadWord() {
            if (IsEnd())//已到结尾
            {
                ThrowException();
            }

            //单词起始字符只能是1+2
            if (_WordChars[Current] != 3) {
                return null;
            }

            var index = _position;
            while ((_WordChars[Current] & 6) != 0)//2或者4都可以
            {
                MoveNext();//读取下一个字符
            }
            return new string(_p, index, _position - index);
        }

        /// <summary> 读取数字
        /// </summary>
        /// <returns></returns>
        private object ReadNumber() {
            if (IsEnd())//已到结尾
            {
                ThrowException();
            }

            int pot = -1;
            bool neg = false;

            switch (Current) {
                case '.':
                    pot = 0;
                    break;
                case '+':
                    MoveNext();//读取下一个字符
                    break;
                case '-':
                    MoveNext();//读取下一个字符
                    neg = true;
                    break;
                default:
                    break;
            }
            int index = _position;


            while (true) {
                switch ((_WordChars[Current] & 6)) {
                    case 0:
                        if (neg) {
                            if (pot >= 0) {
                                return -ReadDecimal(index, _position);
                            }
                            return -ReadInteger(index, _position);
                        }
                        if (pot >= 0) {
                            return ReadDecimal(index, _position);
                        }
                        return ReadInteger(index, _position);
                    case 4:
                        break;
                    case 6:
                        if (pot < 0) {
                            pot = _position;
                        } else if (Current == '.') {
                            ThrowException();
                        }

                        if (Current != '.') {
                            if (neg) {
                                index--;
                            }
                            string str = null;
                            if (Current == 'e' || Current == 'E') {
                                //如果是用科学计数法计的,那么小数点后面最多保存5位
                                //不然有可能会报错
                                if (_position - pot > 6) {
                                    str = new string(_p, index, pot + 6 - index);
                                    index = _position;
                                }
                            }
                            MoveNext();
                            while ((_WordChars[Current] & 4) != 0) {
                                MoveNext();//读取下一个字符
                            }

                            str += new string(_p, index, _position - index);
                            double d;
                            if (double.TryParse(str, out d)) {
                                return d;
                            }
                            ThrowException();
                            return null;
                        }
                        break;
                    default:
                        ThrowException();
                        break;
                }
                MoveNext();//读取下一个字符
            }
        }

        /// <summary> 读取小数
        /// </summary>
        /// <param name="index"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private double ReadDecimal(int index, int end) {
            double d1 = 0d;
            for (; _p[index] != '.'; index++) {
                d1 = d1 * 10 + (_p[index] - (double)'0');
            }
            index++;
            end--;
            double d2 = 0d;
            for (; index <= end; end--) {
                d2 = d2 * 0.1 + (_p[end] - (long)'0');
            }
            return d1 + d2 * 0.1;
        }

        /// <summary> 读取整数
        /// </summary>
        /// <param name="index"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private long ReadInteger(int index, int end) {
            long l = 0L;
            for (; index < end; index++) {
                l = l * 10 + (_p[index] - (long)'0');
            }
            return l;
        }

        /// <summary> 读取字符串
        /// </summary>
        public string ReadString() {
            if (IsEnd())//已到结尾
            {
                ThrowException();
            }

            Char quot = Current;
            if (quot != '"' && quot != '\'') {
                ThrowException();
            }
            MoveNext();
            if (Current == quot) {
                MoveNext();
                return "";
            }

            var index = _position;

            do {
                if (Current == '\\')//是否是转义符
                {
                    char[] arr = new char[255];
                    fixed (char* p = arr) {
                        return ReadString(index, quot, new MiniBuffer(p));
                    }
                }
                MoveNext();
            } while (Current != quot);//是否是结束字符
            string str = new string(_p, index, _position - index);
            MoveNext();
            return str;
        }

        private string ReadString(int index, char quot, MiniBuffer buff) {
            do {
                if (Current == '\\')//是否是转义符
                {
                    if ((_WordChars[Current] & 16) == 0) {
                        ThrowException();
                    }
                    buff.AddString(_p, index, _position - index);
                    MoveNext();
                    switch (Current) {
                        case 't':
                            buff.AddChar('\t');
                            index = _position + 1;
                            break;
                        case 'n':
                            buff.AddChar('\n');
                            index = _position + 1;
                            break;
                        case 'r':
                            buff.AddChar('\r');
                            index = _position + 1;
                            break;
                        case '0':
                            buff.AddChar('\0');
                            index = _position + 1;
                            break;
                        case 'f':
                            buff.AddChar('\f');
                            index = _position + 1;
                            break;
                        case 'u':
                            index = _position + ReadUnicode(buff);
                            break;
                        default:
                            index = _position;
                            break;
                    }
                }
                MoveNext();
            } while (Current != quot);//是否是结束字符
            string str;
            buff.AddString(_p, index, _position - index);
            str = buff.ToString();
            buff.Dispose();
            MoveNext();
            return str;
        }

        private int ReadUnicode(MiniBuffer buff) {
            MoveNext();
            var c1 = Current;
            var n1 = UnicodeNumber(Current);
            if (n1 == -1) {
                buff.AddChar('\\');
                buff.AddChar('u');
                buff.AddChar(c1);
                return 2;
            }

            MoveNext();
            var c2 = Current;
            var n2 = UnicodeNumber(Current);
            if (n2 == -1) {
                buff.AddChar('\\');
                buff.AddChar('u');
                buff.AddChar(c1);
                buff.AddChar(c2);
                return 3;
            }

            MoveNext();
            var c3 = Current;
            var n3 = UnicodeNumber(Current);
            if (n3 == -1) {
                buff.AddChar('\\');
                buff.AddChar('u');
                buff.AddChar(c1);
                buff.AddChar(c2);
                buff.AddChar(c3);
                return 4;
            }

            MoveNext();
            var c4 = Current;
            var n4 = UnicodeNumber(Current);
            if (n4 == -1) {
                buff.AddChar('\\');
                buff.AddChar('u');
                buff.AddChar(c1);
                buff.AddChar(c2);
                buff.AddChar(c3);
                buff.AddChar(c4);
                return 5;
            }
            buff.AddChar((char)(n1 * 0x1000 + n2 * 0x100 + n3 * 0x10 + n4));
            return 5;
        }

        private static sbyte UnicodeNumber(char c) {
            if (c > 122) {
                return -1;
            }
            return _UnicodeFlags[c];
        }

        bool _isDisposed;

        public void Dispose() {
            _p = null;
            _end = 0;
            _isDisposed = true;
            Current = '\0';
        }

        private void ThrowException() {
            if (_isDisposed) {
                throw new ObjectDisposedException("UnsafeJsonReader", "不能访问已释放的对象!");
            }
            if (IsEnd()) {
                Dispose();
                throw new Exception("遇到意外的字符串结尾,解析失败!");
            }
            int i = Math.Max(_position - 20, 0);
            int j = Math.Min(_position + 20, _length);
            string pos = _position.ToString(CultureInfo.InvariantCulture);
            string ch = Current.ToString(CultureInfo.InvariantCulture);
            string view = new string(_p, i, j - i);
            Dispose();
            throw new Exception(string.Format(ERRMESSAGE, pos, ch, view));
        }

        const string ERRMESSAGE = "位置{0}遇到意外的字符{1},解析失败!\n截取: {2}";
    }


}
