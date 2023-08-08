// <copyright file="Encryption.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace LicenseMaker
{
    using System;
    using Org.BouncyCastle.Asn1;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Encodings;
    using Org.BouncyCastle.Crypto.Engines;
    using Org.BouncyCastle.Security;

    /// <summary>
    /// FileProcess.
    /// </summary>
    public class RSACryptoService
    {
        /// <summary>
        /// EncryptByPrivateKey.
        /// </summary>
        /// <param name="content">content.</param>
        /// <param name="key">key.</param>
        /// <returns>encryped content.</returns>
        public byte[] EncryptByPrivateKey(string content, string key)
        {
            IAsymmetricBlockCipher engine = new Pkcs1Encoding(new RsaEngine());
            try
            {
                engine.Init(true, this.GetPrivateKeyParameter(key));
                byte[] byteData = System.Text.Encoding.UTF8.GetBytes(content);
                var resultData = engine.ProcessBlock(byteData, 0, byteData.Length);
                return resultData; //// Convert.ToBase64String(resultData);
                ////Console.WriteLine("密文（base64编码）:" + Convert.ToBase64String(testData) + Environment.NewLine);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public byte[] EncryptByPrivateKey(byte[] byteData, string key)
        {
            IAsymmetricBlockCipher engine = new Pkcs1Encoding(new RsaEngine());
            try
            {
                engine.Init(true, GetPrivateKeyParameter(key));
                //byte[] byteData = System.Text.Encoding.UTF8.GetBytes(s);
                var ResultData = engine.ProcessBlock(byteData, 0, byteData.Length);
                return ResultData;
                //return Convert.ToBase64String(ResultData);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// DecryptByPublicKey.
        /// </summary>
        /// <param name="byteData">byteData.</param>
        /// <param name="key">key.</param>
        /// <returns>decrypted data.</returns>
        public string DecryptByPublicKey(byte[] byteData, string key)
        {
            IAsymmetricBlockCipher engine = new Pkcs1Encoding(new RsaEngine());
            try
            {
                engine.Init(false, this.GetPublicKeyParameter(key));
                ////byte[] byteData = Convert.FromBase64String(s);
                var resultData = engine.ProcessBlock(byteData, 0, byteData.Length);
                return System.Text.Encoding.UTF8.GetString(resultData);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private AsymmetricKeyParameter GetPublicKeyParameter(string s)
        {
            byte[] publicInfoByte = Convert.FromBase64String(s);
            Asn1Object pubKeyObj = Asn1Object.FromByteArray(publicInfoByte); ////这里也可以从流中读取，从本地导入
            AsymmetricKeyParameter pubKey = PublicKeyFactory.CreateKey(publicInfoByte);
            return pubKey;
        }

        private AsymmetricKeyParameter GetPrivateKeyParameter(string s)
        {
            byte[] privateInfoByte = Convert.FromBase64String(s);
            //// Asn1Object priKeyObj = Asn1Object.FromByteArray(privateInfoByte);//这里也可以从流中读取，从本地导入
            //// PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKey);
            AsymmetricKeyParameter priKey = PrivateKeyFactory.CreateKey(privateInfoByte);
            return priKey;
        }
    }
}
