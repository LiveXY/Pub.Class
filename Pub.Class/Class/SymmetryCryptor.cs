using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;

namespace Pub.Class {
    /// <summary>
    /// 对称加密接口
    /// 
    /// 修改纪录
    ///     2010.01.12 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public interface ISymmetryCryptor {
        /// <summary>
        /// 对称加密编码
        /// </summary>
        Encoding Encoding { get; set; }
        /// <summary>
        /// SymmetricAlgorithmType 采用的加密算法类型。
        /// 如果是DES加密，则要求64位密匙。
        /// 如果是Rijndael加密，则支持 128、192 或 256 位的密钥长度。
        /// 如果是RC2加密，则支持的密钥长度为从 40 位到 128 位，以 8 位递增。
        /// 如果是TripleDES加密，则支持从 128 位到 192 位（以 64 位递增）的密钥长度。
        /// </summary>
        SymmetricAlgorithmType SymmetricAlgorithmType { get; set; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mode">CipherMode</param>
        /// <param name="padding">PaddingMode</param>
        void Initialize(CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7);
        /// <summary>
        /// 加密流
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        byte[] EncryptStream(byte[] source, string key);
        /// <summary>
        /// 加密流
        /// </summary>
        /// <param name="source"></param>
        /// <param name="offset"></param>
        /// <param name="toEncrpyLen"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        byte[] EncryptStream(byte[] source, int offset, int toEncrpyLen, string key);
        /// <summary>
        /// 解密流
        /// </summary>
        /// <param name="bytesEncoded"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        byte[] DecryptStream(byte[] bytesEncoded, string key);
        /// <summary>
        /// 解密流
        /// </summary>
        /// <param name="bytesEncoded"></param>
        /// <param name="offset"></param>
        /// <param name="toDecrpyLen"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        byte[] DecryptStream(byte[] bytesEncoded, int offset, int toDecrpyLen, string key);
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        string EncryptString(string source, string key);
        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="strEncoded"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        string DecryptString(string strEncoded, string key);
    }
    /// <summary>
    /// 支持DES, RC2, Rijndael, TripleDES
    /// </summary>
    public enum SymmetricAlgorithmType {
        /// <summary>
        /// DES
        /// </summary>
        DES,
        /// <summary>
        /// RC2
        /// </summary>
        RC2,
        /// <summary>
        /// Rijndael
        /// </summary>
        Rijndael,
        /// <summary>
        /// TripleDES
        /// </summary>
        TripleDES
    }
    /// <summary>
    /// 对称加密算法
    /// </summary>
    public class SymmetryCryptor : ISymmetryCryptor {
        private SymmetricAlgorithm symmetricAlgorithm;
        private string strInitialVector = "SymmetryCryptor";
        //private byte[] initialVector = {0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF}; 
        private byte[] initialVector;
        /// <summary>
        /// 构造器
        /// </summary>
        public SymmetryCryptor() {
        }
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="iv">iv</param>
        public SymmetryCryptor(string iv) {
            if (!iv.IsNullEmpty()) strInitialVector = iv;
        }
        #region Encoding
        private Encoding encoding = Encoding.Default;
        /// <summary>
        /// 编码
        /// </summary>
        public Encoding Encoding {
            get { return encoding; }
            set { encoding = value; }
        }
        #endregion

        #region SymmetricAlgorithmType
        private SymmetricAlgorithmType symmetricAlgorithmType = SymmetricAlgorithmType.DES;
        /// <summary>
        /// SymmetricAlgorithmType
        /// </summary>
        public SymmetricAlgorithmType SymmetricAlgorithmType {
            get { return symmetricAlgorithmType; }
            set { symmetricAlgorithmType = value; }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mode">CipherMode</param>
        /// <param name="padding">PaddingMode</param>
        public void Initialize(CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7) {
            this.initialVector = this.GetExactBytes(strInitialVector, 16);
            switch (this.symmetricAlgorithmType) {
                case SymmetricAlgorithmType.DES: {
                    this.symmetricAlgorithm = new DESCryptoServiceProvider();
                    break;
                }
                case SymmetricAlgorithmType.Rijndael: {
                    this.symmetricAlgorithm = new RijndaelManaged();
                    break;
                }
                case SymmetricAlgorithmType.RC2: {
                    this.symmetricAlgorithm = new RC2CryptoServiceProvider();
                    break;
                }
                case SymmetricAlgorithmType.TripleDES: {
                    this.symmetricAlgorithm = new TripleDESCryptoServiceProvider();
                    break;
                }
                default: {
                    this.symmetricAlgorithm = new DESCryptoServiceProvider();
                    break;
                }
            }

            this.symmetricAlgorithm.Mode = mode;
            this.symmetricAlgorithm.Padding = padding;
        }
        #endregion

        #region EncryptStream
        /// <summary>
        /// 加密流
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public byte[] EncryptStream(byte[] source, string key) {
            if (source.IsNull()) {
                return null;
            }

            byte[] bytesKey = this.encoding.GetBytes(key);

            MemoryStream memStream = new MemoryStream();
            CryptoStream crytoStream = new CryptoStream(memStream, this.symmetricAlgorithm.CreateEncryptor(bytesKey, this.initialVector), CryptoStreamMode.Write);

            try {
                crytoStream.Write(source, 0, source.Length);//将原始字符串加密后写到memStream
                crytoStream.FlushFinalBlock();
                byte[] bytesEncoded = memStream.ToArray();

                return bytesEncoded;

            } finally {
                memStream.Close();
                crytoStream.Close();
            }
        }
        #endregion

        #region EncryptStream
        /// <summary>
        /// 加密流
        /// </summary>
        /// <param name="source"></param>
        /// <param name="offset"></param>
        /// <param name="toEncrpyLen"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public byte[] EncryptStream(byte[] source, int offset, int toEncrpyLen, string key) {
            if (toEncrpyLen == 0) {
                return source;
            }

            byte[] temp = this.GetPartOfStream(source, offset, toEncrpyLen);

            return this.EncryptStream(temp, key);
        }

        private byte[] GetPartOfStream(byte[] source, int offset, int length) {
            if ((source.IsNull()) || offset >= source.Length) {
                return null;
            }

            if (length + offset > source.Length) {
                length = source.Length - offset;
            }

            byte[] temp = new byte[length];
            for (int i = 0; i < length; i++) {
                temp[i] = source[offset + i];
            }

            return temp;
        }

        #endregion

        #region DecryptStream
        /// <summary>
        /// 解密流
        /// </summary>
        /// <param name="bytesEncoded"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public byte[] DecryptStream(byte[] bytesEncoded, string key) {
            if (bytesEncoded.IsNull()) {
                return null;
            }

            byte[] bytesKey = this.encoding.GetBytes(key);
            MemoryStream memStream = new MemoryStream(bytesEncoded);
            CryptoStream crytoStream = new CryptoStream(memStream, this.symmetricAlgorithm.CreateDecryptor(bytesKey, this.initialVector), CryptoStreamMode.Read);

            try {
                StreamReader streamReader = new StreamReader(crytoStream, this.encoding);
                string ss = streamReader.ReadToEnd();
                return this.encoding.GetBytes(ss);
            } finally {
                memStream.Close();
                crytoStream.Close();
            }
        }
        /// <summary>
        /// 解密流
        /// </summary>
        /// <param name="bytesEncoded"></param>
        /// <param name="offset"></param>
        /// <param name="toDecrpyLen"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public byte[] DecryptStream(byte[] bytesEncoded, int offset, int toDecrpyLen, string key) {
            byte[] temp = this.GetPartOfStream(bytesEncoded, offset, toDecrpyLen);
            return this.DecryptStream(temp, key);
        }
        #endregion

        #region EncryptString ,DecryptString
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string EncryptString(string source, string key) {
            byte[] bytes_source = this.encoding.GetBytes(source);
            byte[] bytesEncoded = this.EncryptStream(bytes_source, key);
            return Convert.ToBase64String(bytesEncoded);
        }
        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="strEncoded"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string DecryptString(string strEncoded, string key) {
            byte[] bytesEncoded = Convert.FromBase64String(strEncoded); //不能使用this.encoding.GetBytes(str_encoded);
            byte[] bytesDecoded = this.DecryptStream(bytesEncoded, key);

            return this.encoding.GetString(bytesDecoded, 0, bytesDecoded.Length);
        }
        #endregion

        #region private
        private byte[] GetExactBytes(string source, int length) {
            byte[] result = new byte[length];
            byte[] buff = this.encoding.GetBytes(source);


            int buff_len = buff.Length;

            if (buff_len >= length) {
                for (int i = 0; i < length; i++) {
                    result[i] = buff[i];
                }
            } else {
                for (int i = 0; i < length; i++) {
                    result[i] = buff[i % buff_len];
                }
            }

            return result;
        }

        #endregion
    }

}
