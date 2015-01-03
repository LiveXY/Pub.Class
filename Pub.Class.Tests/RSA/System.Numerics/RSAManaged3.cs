using System;
using System.Security.Cryptography;
using System.IO;
using System.Xml;
using System.Numerics;
using System.Diagnostics;
using System.Text;

public class RSAManaged3 {

    public static byte[] Encrypt(byte[] data, RSAPublicKey publicKey) {
        if (data == null) {
            throw new ArgumentNullException("data");
        }

        if (publicKey == null) {
            throw new ArgumentNullException("publicKey");
        }

        BigInteger e = ConvertToBigInteger(publicKey.Exponent);
        BigInteger n = ConvertToBigInteger(publicKey.Modulus);

        int inputBlockMaxSize = publicKey.Modulus.Length - 1;
        int outputBlockSize = publicKey.Modulus.Length + 1;

        using (MemoryStream stream = new MemoryStream()) {
            int inputBlockOffset = 0;
            int outputBlockOffset = 0;

            int lastInputBlockSize = data.Length % inputBlockMaxSize;
            byte[] lastInputBlockSizeData = BitConverter.GetBytes(lastInputBlockSize);
            stream.Write(lastInputBlockSizeData, 0, lastInputBlockSizeData.Length);
            outputBlockOffset += lastInputBlockSizeData.Length;

            while (inputBlockOffset < data.Length) {
                int inputBlockSize = Math.Min(inputBlockMaxSize, data.Length - inputBlockOffset);
                byte[] inputBlockData = new byte[inputBlockSize + 1];
                Buffer.BlockCopy(data, inputBlockOffset, inputBlockData, 0, inputBlockSize);
                inputBlockOffset += inputBlockSize;

                BigInteger mi = new BigInteger(inputBlockData);
                BigInteger ci = BigInteger.ModPow(mi, e, n);//ci = mi^e ( mod n ) 

                byte[] outputBlockData = ci.ToByteArray();
                stream.Write(outputBlockData, 0, outputBlockData.Length);
                outputBlockOffset += outputBlockSize;
                stream.Seek(outputBlockOffset, SeekOrigin.Begin);
            }

            stream.SetLength(outputBlockOffset);
            return stream.ToArray();
        }
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

        BigInteger d = ConvertToBigInteger(privateKey.D);
        BigInteger n = ConvertToBigInteger(privateKey.Modulus);

        int inputBlockSize = privateKey.Modulus.Length + 1;
        int outputBlockMaxSize = privateKey.Modulus.Length - 1;

        if (data.Length % inputBlockSize != sizeof(Int32)) {
            return null;
        }

        using (MemoryStream stream = new MemoryStream()) {
            int inputBlockOffset = 0;
            int outputBlockOffset = 0;

            int lastOutputBlockSize = BitConverter.ToInt32(data, inputBlockOffset);
            inputBlockOffset += sizeof(Int32);

            if (lastOutputBlockSize > outputBlockMaxSize) {
                return null;
            }

            while (inputBlockOffset < data.Length) {
                byte[] inputBlockData = new byte[inputBlockSize];
                Buffer.BlockCopy(data, inputBlockOffset, inputBlockData, 0, inputBlockSize);
                inputBlockOffset += inputBlockSize;

                BigInteger ci = new BigInteger(inputBlockData);
                BigInteger mi = BigInteger.ModPow(ci, d, n);//mi = ci^d ( mod n ) 

                byte[] outputBlockData = mi.ToByteArray();
                stream.Write(outputBlockData, 0, outputBlockData.Length);
                outputBlockOffset += inputBlockOffset >= data.Length ? lastOutputBlockSize : outputBlockMaxSize;
                stream.Seek(outputBlockOffset, SeekOrigin.Begin);
            }

            stream.SetLength(outputBlockOffset);
            return stream.ToArray();
        }
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

    //    BigInteger d = ConvertToBigInteger(privateKey.D);
    //    BigInteger n = ConvertToBigInteger(privateKey.Modulus);

    //    int inputBlockMaxSize = privateKey.Modulus.Length - 1;
    //    int outputBlockSize = privateKey.Modulus.Length + 1;

    //    using (MemoryStream stream = new MemoryStream()) {
    //        int inputBlockOffset = 0;
    //        int outputBlockOffset = 0;

    //        int lastInputBlockSize = data.Length % inputBlockMaxSize;
    //        byte[] lastInputBlockSizeData = BitConverter.GetBytes(lastInputBlockSize);
    //        stream.Write(lastInputBlockSizeData, 0, lastInputBlockSizeData.Length);
    //        outputBlockOffset += lastInputBlockSizeData.Length;

    //        while (inputBlockOffset < data.Length) {
    //            int inputBlockSize = Math.Min(inputBlockMaxSize, data.Length - inputBlockOffset);
    //            byte[] inputBlockData = new byte[inputBlockSize + 1];
    //            Buffer.BlockCopy(data, inputBlockOffset, inputBlockData, 0, inputBlockSize);
    //            inputBlockOffset += inputBlockSize;

    //            BigInteger ci = new BigInteger(inputBlockData);
    //            BigInteger mi = BigInteger.ModPow(ci, d, n);//mi = ci^d ( mod n ) 

    //            byte[] outputBlockData = ci.ToByteArray();
    //            stream.Write(outputBlockData, 0, outputBlockData.Length);
    //            outputBlockOffset += outputBlockSize;
    //            stream.Seek(outputBlockOffset, SeekOrigin.Begin);
    //        }

    //        stream.SetLength(outputBlockOffset);
    //        return stream.ToArray();
    //    }
    //}
    //public static byte[] Decrypt(byte[] data, RSAPublicKey publicKey) {
    //    if (data == null) {
    //        throw new ArgumentNullException("data");
    //    }

    //    if (publicKey == null) {
    //        throw new ArgumentNullException("publicKey");
    //    }

    //    BigInteger e = ConvertToBigInteger(publicKey.Exponent);
    //    BigInteger n = ConvertToBigInteger(publicKey.Modulus);

    //    int inputBlockSize = publicKey.Modulus.Length + 1;
    //    int outputBlockMaxSize = publicKey.Modulus.Length - 1;

    //    if (data.Length % inputBlockSize != sizeof(Int32)) {
    //        return null;
    //    }

    //    using (MemoryStream stream = new MemoryStream()) {
    //        int inputBlockOffset = 0;
    //        int outputBlockOffset = 0;

    //        int lastOutputBlockSize = BitConverter.ToInt32(data, inputBlockOffset);
    //        inputBlockOffset += sizeof(Int32);

    //        if (lastOutputBlockSize > outputBlockMaxSize) {
    //            return null;
    //        }

    //        while (inputBlockOffset < data.Length) {
    //            byte[] inputBlockData = new byte[inputBlockSize];
    //            Buffer.BlockCopy(data, inputBlockOffset, inputBlockData, 0, inputBlockSize);
    //            inputBlockOffset += inputBlockSize;

    //            BigInteger mi = new BigInteger(inputBlockData);
    //            BigInteger ci = BigInteger.ModPow(mi, e, n);//ci = mi^e ( mod n ) 

    //            byte[] outputBlockData = mi.ToByteArray();
    //            stream.Write(outputBlockData, 0, outputBlockData.Length);
    //            outputBlockOffset += inputBlockOffset >= data.Length ? lastOutputBlockSize : outputBlockMaxSize;
    //            stream.Seek(outputBlockOffset, SeekOrigin.Begin);
    //        }

    //        stream.SetLength(outputBlockOffset);
    //        return stream.ToArray();
    //    }
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

    private static BigInteger ConvertToBigInteger(byte[] value) {
        //Debug.Assert(value != null);

        byte[] temp = new byte[value.Length + 1];
        for (int i = 0; i < value.Length; i++) {
            temp[value.Length - 1 - i] = value[i];
        }
        return new BigInteger(temp);
    }
}

