// <copyright file="Hjs39d7Hssj9de.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.IOServer
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using Microsoft.AspNetCore.Cryptography.KeyDerivation;
    using Org.BouncyCastle.Asn1;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Encodings;
    using Org.BouncyCastle.Crypto.Engines;
    using Org.BouncyCastle.Security;
    using Sunup.Contract;
    using Sunup.Diagnostics;
    using Sunup.Utility;

    /// <summary>
    /// FileProcess.
    /// </summary>
    public class Hjs39d7Hssj9de
    {
        /// <summary>
        /// hash string.
        /// </summary>
        /// <param name="content">string.</param>
        /// <param name="saltString">salt.</param>
        /// <returns>hashed string.</returns>
        public static string HashString(string content, string saltString)
        {
            string hashed = string.Empty;
            ////Console.Write("Enter a password: ");
            ////string password = Console.ReadLine();
            try
            {
                byte[] salt = System.Text.Encoding.UTF8.GetBytes(saltString);
                //// generate a 128-bit salt using a cryptographically strong random sequence of nonzero values
                ////byte[] salt = new byte[128 / 8];
                ////using (var rngCsp = new RNGCryptoServiceProvider())
                ////{
                ////    rngCsp.GetNonZeroBytes(salt);
                ////}

                ////Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");

                // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
                hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: content,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));
            }
            catch (Exception ex)
            {
                Logger.LogError("[License] Failed to hash data.", ex);
                Logger.LogTrace("[License] Failed to hash data.", ex);
            }

            ////Console.WriteLine($"Hashed: {hashed}");
            return hashed;
        }

        /// <summary>
        /// Verify.
        /// </summary>
        /// <param name="appId">appId.</param>
        /// <returns>True is ok.</returns>
        public static bool Yhkeh77Uhbeds(string appId)
        {
            appId = string.IsNullOrEmpty(appId) ? string.Empty : appId;
            var fileProcess = new FileProcess();
            var pcInfoName = string.IsNullOrEmpty(appId) ? "license.bin" : appId + ".bin";
            var pcInfo = ComputerInfo.GetComputerInfo();
            var currentPath = System.AppDomain.CurrentDomain.BaseDirectory;
            var licFileName = string.IsNullOrEmpty(appId) ? "license.lic" : appId + ".lic";
            pcInfo += appId + currentPath;
            var hashed = string.Empty;
            if (!string.IsNullOrEmpty(appId))
            {
                hashed = HashString(pcInfo, appId);
            }

            if (string.IsNullOrEmpty(hashed))
            {
                hashed = pcInfo;
            }

            var filePath = Path.Combine(currentPath, licFileName);
            if (File.Exists(filePath))
            {
                if (fileProcess.ReadFile(filePath, out byte[] data) && data.Length > 0)
                {
                    ////var key = "MIGdMA0GCSqGSIb3DQEBAQUAA4GLADCBhwKBgQDdRINf4GyRJFW0Jhmc2h5GfcgMXzSwqyWnUvoH7LX14mf7JLWqZgxOE3jPxjiZgDoe3fWvDLkUq2qrXsDkycux8WVoHH6Q/Spttl0uk8nniK4WmUEnmdkaH4r/tlgiG7odbCPbd6GIZwXR4U5ehblDoh5HfSF6GWLCwSPiTuRd5wIBAw==";
                    var key = "MIIBIDANBgkqhkiG9w0BAQEFAAOCAQ0AMIIBCAKCAQEAq90KR8P1Iwr9RtuwvgtWG4MLLQgIpW1bt9r5ZffqCRJw3KufMGvytSL3xHq9hT+qh38uJx9YHoyGb+l5H0U7VHQAy7i7uJjlW93hXBp9B7+1H8PYayc4bzPUfYpwRshN10ErAo9ugtDE2i4Hp6pzpsQzNwzk3TCgoyiIqAcHCQJqrjfnKvMNpP495gW2vw4NWOKJ1i5omK2xmBoP5vUbwmaI9d7hT+fgLdmuYWB8YEYM7NaeLe9QCW452FXaJCsrc6HYdzq347lSXKdYui6vzH3IKYIZbEC/rEJ3nPvlIHbf0wUqxqeoepiXs9MVhuXH2R2YMyjGpdz+wUeMweK1LQIBAw==";
                    var content = DecryptByPublicKey(data, key);
                    var paras = content.Split(">>>>>");
                    var originKey = paras[0];
                    var dataPointNumberStr = paras[1];
                    var conNumStr = paras[2];
                    if (hashed.Equals(originKey))
                    {
                        int.TryParse(dataPointNumberStr, out int dataPointNumber);
                        int.TryParse(conNumStr, out int conNum);
                        var s = new License(true, dataPointNumber, conNum);
                        Logger.LogInfo("[License] Succeed to verify license and the license is valid.");
                        Logger.LogInfo($"[License] Data point number is {dataPointNumber}, connection number is {conNum}.");
                        return true;
                    }
                    else
                    {
                        Logger.LogError($"[License] Failed to verify license,the license is invalid, file path:{filePath}.");
                        Logger.LogInfo($"[License] By default, data point number is 5, connection number is 1.");
                    }
                }
                else
                {
                    Logger.LogError($"[License] Failed to verify license,file path:{filePath}.");
                    Logger.LogInfo($"[License] By default, data point number is 5, connection number is 1.");
                }
            }
            else
            {
                var contentFilePath = Path.Combine(currentPath, pcInfoName);
                byte[] data = System.Text.Encoding.UTF8.GetBytes(hashed);
                if (fileProcess.SaveFile(contentFilePath, data))
                {
                    Logger.LogError($"[License] Failed to verify license,the license is not found, the license file should be at:{filePath}.");
                    Logger.LogInfo($"[License] By default, data point number is 5, connection number is 1.");
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// DecryptByPublicKey.
        /// </summary>
        /// <param name="byteData">byteData.</param>
        /// <param name="key">key.</param>
        /// <returns>decrypted data.</returns>
        private static string DecryptByPublicKey(byte[] byteData, string key)
        {
            IAsymmetricBlockCipher engine = new Pkcs1Encoding(new RsaEngine());
            try
            {
                engine.Init(false, GetPublicKeyParameter(key));
                ////byte[] byteData = Convert.FromBase64String(s);
                var resultData = engine.ProcessBlock(byteData, 0, byteData.Length);
                return System.Text.Encoding.UTF8.GetString(resultData);
            }
            catch (Exception ex)
            {
                Logger.LogError("[License] Failed to decrypt data.", ex);
                Logger.LogTrace("[License] Failed to decrypt data.", ex);
                return null;
            }
        }

        private static AsymmetricKeyParameter GetPublicKeyParameter(string s)
        {
            byte[] publicInfoByte = Convert.FromBase64String(s);
            Asn1Object pubKeyObj = Asn1Object.FromByteArray(publicInfoByte);
            AsymmetricKeyParameter pubKey = PublicKeyFactory.CreateKey(publicInfoByte);
            return pubKey;
        }
    }
}
