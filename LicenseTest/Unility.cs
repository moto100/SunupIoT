using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace LicenseTest
{
    class Unility
    {
        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="publickey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RSAEncrypt(string publickey, string content)
        {
            publickey = @"<RSAKeyValue><Modulus>5m9m14XH3oqLJ8bNGw9e4rGpXpcktv9MSkHSVFVMjHbfv+SJ5v0ubqQxa5YjLN4vc49z7SVju8s0X4gZ6AzZTn06jzWOgyPRV54Q4I0DCYadWW4Ze3e+BOtwgVU1Og3qHKn8vygoj40J6U85Z/PTJu3hN1m75Zr195ju7g9v4Hk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(publickey);
            cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(content), false);

            return Convert.ToBase64String(cipherbytes);
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="privatekey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RSADecrypt(string privatekey, string content)
        {
            privatekey = @"<RSAKeyValue><Modulus>5m9m14XH3oqLJ8bNGw9e4rGpXpcktv9MSkHSVFVMjHbfv+SJ5v0ubqQxa5YjLN4vc49z7SVju8s0X4gZ6AzZTn06jzWOgyPRV54Q4I0DCYadWW4Ze3e+BOtwgVU1Og3qHKn8vygoj40J6U85Z/PTJu3hN1m75Zr195ju7g9v4Hk=</Modulus><Exponent>AQAB</Exponent><P>/hf2dnK7rNfl3lbqghWcpFdu778hUpIEBixCDL5WiBtpkZdpSw90aERmHJYaW2RGvGRi6zSftLh00KHsPcNUMw==</P><Q>6Cn/jOLrPapDTEp1Fkq+uz++1Do0eeX7HYqi9rY29CqShzCeI7LEYOoSwYuAJ3xA/DuCdQENPSoJ9KFbO4Wsow==</Q><DP>ga1rHIJro8e/yhxjrKYo/nqc5ICQGhrpMNlPkD9n3CjZVPOISkWF7FzUHEzDANeJfkZhcZa21z24aG3rKo5Qnw==</DP><DQ>MNGsCB8rYlMsRZ2ek2pyQwO7h/sZT8y5ilO9wu08Dwnot/7UMiOEQfDWstY3w5XQQHnvC9WFyCfP4h4QBissyw==</DQ><InverseQ>EG02S7SADhH1EVT9DD0Z62Y0uY7gIYvxX/uq+IzKSCwB8M2G7Qv9xgZQaQlLpCaeKbux3Y59hHM+KpamGL19Kg==</InverseQ><D>vmaYHEbPAgOJvaEXQl+t8DQKFT1fudEysTy31LTyXjGu6XiltXXHUuZaa2IPyHgBz0Nd7znwsW/S44iql0Fen1kzKioEL3svANui63O3o5xdDeExVM6zOf1wUUh/oldovPweChyoAdMtUzgvCbJk1sYDJf++Nr0FeNW1RB1XG30=</D></RSAKeyValue>";
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(privatekey);
            cipherbytes = rsa.Decrypt(Convert.FromBase64String(content), false);

            return Encoding.UTF8.GetString(cipherbytes);
        }

        public static AsymmetricAlgorithm ReadFromPfxFile()
        {
            X509Certificate2 x509 = new X509Certificate2(@"E:\Sunup\Shared\SunupCertificate.pfx","wk139161",X509KeyStorageFlags.Exportable);
            //X509Certificate2 x509_2 = new X509Certificate2(@"E:\Sunup\Shared\SunupCertificate.cer", "wk139161");
            var publicKey = x509.PublicKey.Key.ToXmlString(false);// "<RSAKeyValue><Modulus>49ZNvEUULlxO+aj6tDpQnXpjhoShUTG81p/PvBCuYGh5c3q6x7fwcSCgzfbdJ2ssVtfqCGB4MFJjtzAqe7OVmQ9iuBGT8sVXQ0lL/3dJ4f80mCNdbeju9Kql/kcReratwUWNr9S+cinoj+rETyA3JZEOa9MP0g/Z2TRXeY9Rfr/bIFkscNp7kS9Pwcgd9oAwO5gwcYRFQnt3tJ7nbrXfkogZXuF/Qb1eoq+FBjWAqqXAP9fehEiNz3p+8q+4mRs+gY1fepN1f3khlvSNu9gWXNI7/umcCDW22QxieG/CcFbNBPasK+RoirY9JsCk79ibcSke4prDAkf8PdIYppVkiQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";// x509.GetRSAPublicKey().ToXmlString(false);
            var privateKey = x509.PrivateKey.ToXmlString(true);// "<RSAKeyValue><Modulus></Modulus><Exponent></Exponent><P>8re3ypaalhNVsDFel1G+piy0s5wYyHojxIH2pi99y6NWOzcJHQsxoWEvxEtFEd9zOEDzLCymhBkrrC8b4YiILC7lmBvwdZ9aXJYYPCJF4XLxpTRtPbsoloSDjPh1yJ9/poKMgpx9u7YBoOLrbzn8UyHGzS7YU0kAVNaMSa6NiEc=</P><Q>8E4fEPpqdj73mBDWmZAIScSgtqNAvWJxzownPDn2vW/yjJDiT3e8wSSstEMMKDxqxxcaLOQ2oYEIUi1kUVTQ9XnJaegRUJiVQKmP/g5Uv7tnL1vReE40FiV8Tp5vJCSa0vGhd8UN74Jt6055o3igqWTaOqbxjjEruTmR5vba5K8=</Q><DP>oh4wNrLNYjxb2anAIpZJHzlxIR68NvvN9C1Sj8Hc5i2dJUQnZCzb44zEmPolkkCL9yD6y6CvQECHuSRmHQhS3CAe3rCathXQB1OBsHi78FAUM2KHLNpby92K+JfkJDLU0mUUcRERslVpVQr8cnhqnyRKYzZM66QxAnmjjnt2sXU=</DP><DQ>LCHsyq1yx+yghCjzgJhXLVUTPveUeDOQOyK6QfFTQ08/+FjTd0zCOaaGoUR/AxS1d4Lip8Px1I9h+9WiSt2M0shfGTgpleev4YRCkaIIJn5AYQ431iRmpHqV7cpjFvwyAvOld92UxZvqnAB5jnAk8Xc4LPVj/31ATnAoP4auhLk=</DQ><InverseQ>0eNopSkNLiG0w2k8SnLjNBTa/9wm+McX219daQc/t/nz+xch55pu2Pky39NxikJ7eb4tm/4S0035rXFRlBmSWy4dy/gnas8JxYRS8QdT2VV8QC4nVUABQMDmig0DCe1QmJufDcvvX0oCqu9WMuCF5+0RRfatHSS+T0hOgA5dmsg=</InverseQ><D>umTo083v5L3AHqAP+sJ6kVs/cR2wnCyEwVGdMc6z2kuekMsnY50oKGT5KYd9BnK3o5Sg8pl5uo6s5YGSQAF7sezUZqJ613J7IaEle6qr1yhibtT/6ptWYvP5OT4LHsm0/RlaAS+bXurKSHNYhyNj+mfh1HSnzMMLBpe7uMd3dKkWARn78CM9D8T7PXOGYh3satSWMRSn4UVi+pUy9kmt6gxzQ32teEz+FNbUrIEk+EqTlw4of3SvnGZBbJ0UW7dCwmmpLQeiznPCwWjAMHnnFE5eQDuwTf5iRnuJHZvnkzEHbLvEODbBLD6JJzHzQgU0zcrokKOS3zd6dAlrFQskwQ==</D></RSAKeyValue>";//x509.GetRSAPrivateKey().ToXmlString(true);
            

            byte[] data = System.Text.Encoding.UTF8.GetBytes("cheshi罗");
                        //证书公钥加密 
            RSACryptoServiceProvider oRSA1 = new RSACryptoServiceProvider();
            oRSA1.FromXmlString(publicKey);
            byte[] AOutput = oRSA1.Encrypt(data, false);

            RSACryptoServiceProvider rsa2 = new RSACryptoServiceProvider();
            rsa2.FromXmlString(privateKey);
            byte[] plainbytes = rsa2.Decrypt(AOutput, false);

            string reslut = Encoding.UTF8.GetString(plainbytes);
            return x509.PrivateKey;
        }
    }
}
