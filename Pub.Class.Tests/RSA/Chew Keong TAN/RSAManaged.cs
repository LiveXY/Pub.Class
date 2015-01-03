//************************************************************************************
// RSAManaged Class Version 1.00
//
// Copyright (c) 2010 KevinShan (txhak@163.com)
// All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, provided that the above
// copyright notice(s) and this permission notice appear in all copies of
// the Software and that both the above copyright notice(s) and this
// permission notice appear in supporting documentation.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT
// OF THIRD PARTY RIGHTS. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// HOLDERS INCLUDED IN THIS NOTICE BE LIABLE FOR ANY CLAIM, OR ANY SPECIAL
// INDIRECT OR CONSEQUENTIAL DAMAGES, OR ANY DAMAGES WHATSOEVER RESULTING
// FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT,
// NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION
// WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
// DO NOT TRUST THIS CLASS FOR ENCRYPTION OF COMMERCIAL, PRIVATE OR ANY KIND OF SECRETS!
//
// Disclaimer
// ----------
// Although reasonable care has been taken to ensure the correctness of this
// implementation, this code should never be used in any application without
// proper verification and testing.  I disclaim all liability and responsibility
// to any person or entity with respect to any loss or damage caused, or alleged
// to be caused, directly or indirectly, by the use of this RSAEncryption class.
// 
//************************************************************************************

using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Xml;
using number;

/// <summary>
///  RSA加密解密算法
/// </summary>
/// <remarks>
/// <para>
///   RSA加密算法是一种非对称加密算法。在公钥加密标准和电子商业中RSA被广泛使用。
/// RSA是1977年由罗纳德·李维斯特（Ron Rivest）、阿迪·萨莫尔（Adi Shamir）和伦
/// 纳德·阿德曼（Leonard Adleman）一起提出的。当时他们三人都在麻省理工学院工作。
/// RSA就是他们三人姓氏开头字母拼在一起组成的。
/// </para>
/// <para>
///   RSA算法的可靠性基于分解极大的整数是很困难的。假如有人找到一种很快的分解因
/// 子的算法的话，那么用RSA加密的信息的可靠性就肯定会极度下降。但找到这样的算法
/// 的可能性是非常小的。今天只有短的RSA钥匙才可能被强力方式解破。到2008年为止，
/// 世界上还没有任何可靠的攻击RSA算法的方式。只要其钥匙的长度足够长，用RSA加密的
/// 信息实际上是不能被解破的。
/// </para>
/// <para>
/// <b>公钥和私钥的产生</b>
/// 　　假设Alice想要通过一个不可靠的媒体接收Bob的一条私人讯息。她可以用以下的方式
/// 来产生一个公钥和一个密钥：
///     随意选择两个大的质数p和q，p不等于q，计算N=pq。 
///     根据欧拉函数,不大于N且与N互质的整数个数为(p-1)(q-1) 
///     选择一个整数e与(p-1)(q-1)互质,并且e小于(p-1)(q-1) 
///     用以下这个公式计算d：d× e ≡ 1 (mod (p-1)(q-1)) 
///     将p和q的记录销毁。 
///     e是公钥，d是私钥。d是秘密的，而N是公众都知道的。Alice将她的公钥传给Bob，而
/// 将她的私钥藏起来。
/// </para>
/// <para>
/// <b>加密消息</b>
/// 　　假设Bob想给Alice送一个消息m，他知道Alice产生的N和e。他使用起先与Alice约好的
/// 格式将m转换为一个小于N的整数n，比如他可以将每一个字转换为这个字的Unicode码，然后
/// 将这些数字连在一起组成一个数字。假如他的信息非常长的话，他可以将这个信息分为几
/// 段，然后将每一段转换为n。用下面这个公式他可以将n加密为c：
///     c = m ^ d (mod n)
///     计算c并不复杂。Bob算出c后就可以将它传递给Alice。
/// </para>
/// <para>
/// <b>解密消息</b>
/// 　　Alice得到Bob的消息c后就可以利用她的密钥d和n来解码。她可以用以下这个公式来将c转换为m：
/// 　　m = c ^ e (mod n)
/// </para>
/// <para>
/// <b>签名消息</b>
/// 　　RSA也可以用来为一个消息署名。假如甲想给乙传递一个署名的消息的话，那么她可以为
/// 她的消息计算一个散列值，然后用她的密钥加密这个散列值并将这个“署名”加在消息的后面。
/// 这个消息只有用她的公钥才能被解密。乙获得这个消息后可以用甲的公钥解密这个散列值，然
/// 后将这个数据与他自己为这个消息计算的散列值相比较。假如两者相符的话，那么他就可以知
/// 道发信人持有甲的密钥，以及这个消息在传播路径上没有被篡改过。
/// </para>
/// </remarks>
public class RSAManaged {
    public static byte[] Encrypt(byte[] data, RSAPublicKey publicKey) {
        if (data == null) {
            throw new ArgumentNullException("data");
        }

        if (publicKey == null) {
            throw new ArgumentNullException("publicKey");
        }

        int blockSize = publicKey.Modulus.Length - 1;
        return Compute(data, publicKey, blockSize);
    }
    public static string Encrypt(string data, string publicKey) {
        return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(data), RSAPublicKey.FromXmlString(publicKey)));
    }
    public static byte[] Decrypt(byte[] data, RSAPrivateKey privateKey) {
        if (data == null) {
            throw new ArgumentNullException("data");
        }

        if (privateKey == null) {
            throw new ArgumentNullException("privateKey");
        }

        int blockSize = privateKey.Modulus.Length;
        return Compute(data, privateKey, blockSize);
    }
    public static string Decrypt(string data, string privateKey) {
        return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(data), RSAPrivateKey.FromXmlString(privateKey)));
    }
    public static byte[] Sign(byte[] data, RSAPublicKey publicKey, HashAlgorithm hash) {
        if (data == null) {
            throw new ArgumentNullException("data");
        }

        if (publicKey == null) {
            throw new ArgumentNullException("publicKey");
        }

        if (hash == null) {
            throw new ArgumentNullException("hash");
        }

        byte[] hashData = hash.ComputeHash(data);
        byte[] signature = Encrypt(hashData, publicKey);
        return signature;
    }
    public static string Sign(string data, string publicKey, HashAlgorithm hash) {
        return Convert.ToBase64String(Sign(Encoding.UTF8.GetBytes(data), RSAPublicKey.FromXmlString(publicKey), hash));
    }
    public static bool Verify(byte[] data, RSAPrivateKey privateKey, HashAlgorithm hash, byte[] signature) {
        if (data == null) {
            throw new ArgumentNullException("data");
        }

        if (privateKey == null) {
            throw new ArgumentNullException("privateKey");
        }

        if (hash == null) {
            throw new ArgumentNullException("hash");
        }

        if (signature == null) {
            throw new ArgumentNullException("signature");
        }

        byte[] hashData = hash.ComputeHash(data);
        byte[] signatureHashData = Decrypt(signature, privateKey);

        if (signatureHashData != null && signatureHashData.Length == hashData.Length) {
            for (int i = 0; i < signatureHashData.Length; i++) {
                if (signatureHashData[i] != hashData[i]) {
                    return false;
                }
            }
            return true;
        }

        return false;
    }
    public static bool Verify(string data, string privateKey, HashAlgorithm hash, string signature) {
        return Verify(Encoding.UTF8.GetBytes(data), RSAPrivateKey.FromXmlString(privateKey), hash, Encoding.UTF8.GetBytes(signature));
    }

    //public static byte[] Encrypt(byte[] data, RSAPrivateKey privateKey) {
    //    if (data == null) {
    //        throw new ArgumentNullException("data");
    //    }

    //    if (privateKey == null) {
    //        throw new ArgumentNullException("privateKey");
    //    }

    //    int blockSize = privateKey.Modulus.Length - 1;
    //    return Compute(data, privateKey, blockSize);
    //}
    //public static string Encrypt(string data, string privateKey) {
    //    return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(data), RSAPrivateKey.FromXmlString(privateKey)));
    //}
    //public static byte[] Decrypt(byte[] data, RSAPublicKey publicKey) {
    //    if (data == null) {
    //        throw new ArgumentNullException("data");
    //    }

    //    if (publicKey == null) {
    //        throw new ArgumentNullException("publicKey");
    //    }

    //    int blockSize = publicKey.Modulus.Length;
    //    return Compute(data, publicKey, blockSize);
    //}
    //public static string Decrypt(string data, string publicKey) {
    //    return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(data), RSAPublicKey.FromXmlString(publicKey)));
    //}
    //public static byte[] Sign(byte[] data, RSAPrivateKey privateKey, HashAlgorithm hash) {
    //    if (data == null) {
    //        throw new ArgumentNullException("data");
    //    }

    //    if (privateKey == null) {
    //        throw new ArgumentNullException("privateKey");
    //    }

    //    if (hash == null) {
    //        throw new ArgumentNullException("hash");
    //    }

    //    byte[] hashData = hash.ComputeHash(data);
    //    byte[] signature = Encrypt(hashData, privateKey);
    //    return signature;
    //}
    //public static string Sign(string data, string privateKey, HashAlgorithm hash) {
    //    return Convert.ToBase64String(Sign(Encoding.UTF8.GetBytes(data), RSAPrivateKey.FromXmlString(privateKey), hash));
    //}
    //public static bool Verify(byte[] data, RSAPublicKey publicKey, HashAlgorithm hash, byte[] signature) {
    //    if (data == null) {
    //        throw new ArgumentNullException("data");
    //    }

    //    if (publicKey == null) {
    //        throw new ArgumentNullException("publicKey");
    //    }

    //    if (hash == null) {
    //        throw new ArgumentNullException("hash");
    //    }

    //    if (signature == null) {
    //        throw new ArgumentNullException("signature");
    //    }

    //    byte[] hashData = hash.ComputeHash(data);

    //    byte[] signatureHashData = Decrypt(signature, publicKey);

    //    if (signatureHashData != null && signatureHashData.Length == hashData.Length) {
    //        for (int i = 0; i < signatureHashData.Length; i++) {
    //            if (signatureHashData[i] != hashData[i]) {
    //                return false;
    //            }
    //        }
    //        return true;
    //    }

    //    return false;
    //}
    //public static bool Verify(string data, string publicKey, HashAlgorithm hash, string signature) {
    //    return Verify(Encoding.UTF8.GetBytes(data), RSAPublicKey.FromXmlString(publicKey), hash, Encoding.UTF8.GetBytes(signature));
    //}

    private static byte[] Compute(byte[] data, RSAPublicKey publicKey, int blockSize) {
        //
        // 公钥加密/解密公式为：ci = mi^e ( mod n )            
        // 
        // 先将 m（二进制表示）分成数据块 m1, m2, ..., mi ，然后进行运算。
        //
        BigInteger e = new BigInteger(publicKey.Exponent);
        BigInteger n = new BigInteger(publicKey.Modulus);

        int blockOffset = 0;
        using (MemoryStream stream = new MemoryStream()) {
            while (blockOffset < data.Length) {
                int blockLen = Math.Min(blockSize, data.Length - blockOffset);
                byte[] blockData = new byte[blockLen];
                Buffer.BlockCopy(data, blockOffset, blockData, 0, blockLen);

                BigInteger mi = new BigInteger(blockData);
                BigInteger ci = mi.modPow(e, n);//ci = mi^e ( mod n )

                byte[] block = ci.getBytes();
                stream.Write(block, 0, block.Length);
                blockOffset += blockLen;
            }

            return stream.ToArray();
        }
    }
    private static byte[] Compute(byte[] data, RSAPrivateKey privateKey, int blockSize) {
        //
        // 私钥加密/解密公式为：mi = ci^d ( mod n )
        // 
        // 先将 c（二进制表示）分成数据块 c1, c2, ..., ci ，然后进行运算。            
        //
        BigInteger d = new BigInteger(privateKey.D);
        BigInteger n = new BigInteger(privateKey.Modulus);

        int blockOffset = 0;

        using (MemoryStream stream = new MemoryStream()) {
            while (blockOffset < data.Length) {
                int blockLen = Math.Min(blockSize, data.Length - blockOffset);
                byte[] blockData = new byte[blockLen];
                Buffer.BlockCopy(data, blockOffset, blockData, 0, blockLen);

                BigInteger ci = new BigInteger(blockData);
                BigInteger mi = ci.modPow(d, n);//mi = ci^d ( mod n )

                byte[] block = mi.getBytes();
                stream.Write(block, 0, block.Length);
                blockOffset += blockLen;
            }

            return stream.ToArray();
        }
    }
}

public class RSAPublicKey {
    public byte[] Modulus;
    public byte[] Exponent;

    public static RSAPublicKey FromXmlString(string xmlString) {
        if (string.IsNullOrEmpty(xmlString)) {
            return null;
        }

        try {
            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString))) {
                if (!reader.ReadToFollowing("RSAKeyValue")) {
                    return null;
                }

                if (reader.LocalName != "Modulus" && !reader.ReadToFollowing("Modulus")) {
                    return null;
                }
                string modulus = reader.ReadElementContentAsString();

                if (reader.LocalName != "Exponent" && !reader.ReadToFollowing("Exponent")) {
                    return null;
                }
                string exponent = reader.ReadElementContentAsString();

                RSAPublicKey publicKey = new RSAPublicKey();
                publicKey.Modulus = Convert.FromBase64String(modulus);
                publicKey.Exponent = Convert.FromBase64String(exponent);

                return publicKey;
            }
        } catch {
            return null;
        }
    }
}

public class RSAPrivateKey {
    public byte[] Modulus;
    public byte[] D;

    public static RSAPrivateKey FromXmlString(string xmlString) {
        if (string.IsNullOrEmpty(xmlString)) {
            return null;
        }

        try {
            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString))) {
                if (!reader.ReadToFollowing("RSAKeyValue")) {
                    return null;
                }

                if (reader.LocalName != "Modulus" && !reader.ReadToFollowing("Modulus")) {
                    return null;
                }
                string modulus = reader.ReadElementContentAsString();

                if (reader.LocalName != "D" && !reader.ReadToFollowing("D")) {
                    return null;
                }
                string d = reader.ReadElementContentAsString();

                RSAPrivateKey privateKey = new RSAPrivateKey();
                privateKey.Modulus = Convert.FromBase64String(modulus);
                privateKey.D = Convert.FromBase64String(d);

                return privateKey;
            }
        } catch {
            return null;
        }
    }
}

