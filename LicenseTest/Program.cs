using System;

namespace LicenseTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //var s = ComputerInfo.GetComputerInfo();
            //Console.WriteLine(s);
            //Console.WriteLine("Hello World!");
            //Unility.ReadFromPfxFile();
            //var ss=  Unility.RSAEncrypt("ss", "AAA");
            //var sss = Unility.RSADecrypt("ss", ss);

            
            var s = ComputerInfo.GetComputerInfo();
            s += ">>>>>50";
            RSAForJava confuser = new RSAForJava();
            var keys =confuser.GetKey();
            //var publicKey = "MIGdMA0GCSqGSIb3DQEBAQUAA4GLADCBhwKBgQDdRINf4GyRJFW0Jhmc2h5GfcgMXzSwqyWnUvoH7LX14mf7JLWqZgxOE3jPxjiZgDoe3fWvDLkUq2qrXsDkycux8WVoHH6Q/Spttl0uk8nniK4WmUEnmdkaH4r/tlgiG7odbCPbd6GIZwXR4U5ehblDoh5HfSF6GWLCwSPiTuRd5wIBAw==";
            //var privateKey = "MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAN1Eg1/gbJEkVbQmGZzaHkZ9yAxfNLCrJadS+gfstfXiZ/sktapmDE4TeM/GOJmAOh7d9a8MuRSraqtewOTJy7HxZWgcfpD9Km22XS6TyeeIrhaZQSeZ2Rofiv+2WCIbuh1sI9t3oYhnBdHhTl6FuUOiHkd9IXoZYsLBI+JO5F3nAgEDAoGAJODAj/q8wttjngZZmiRaYRT2rLqIyByGRo3UVqdzqPsRVIYeRxECDQM+zUu0GZVfBST+R9d0LhyRxzp1e3b3SAOQov40qrfXZmpvGyo9GYGqiqKh7e4VHxFYUYkmuzBFQflUUgrVoQMp08v0Qn53Cj2IH5R/r1AhNPBnQqpjBFsCQQDu+9uFmvnXGMs9gHBZA3IHPW9joaGT2cK46zW1WV5A2Iuj6b1wWzcvYJWTejyI2UKafITC8tuFqIwCsEmYem0xAkEA7QW6oaeW1wU7+kIbPVfcd3FnZdPycYCc/o3gyhZguUIF8EAxxkSLJKpFhhqTBhXDlnEFPzCCsvL5HAgIuBfWlwJBAJ9SklkR++S7MikASuYCTATTn5fBFmKRLHtHeSOQ6YCQXRfxKPWSJMpAY7em0wXmLGb9rddMklkbCAHK27r8SMsCQQCeA9HBGmSPWNKm1rzTj+hPoO+ZN/b2Vb3/CUCGuZXQ1q6gKsvZgwdtxtkEEbdZY9e5oK4qIFch91C9WrB6uo8PAkEAyIq/G9SzluZ5hcvn3+gSHkwlhD4UtUYLIu6Vf3ejGC2Cd7TggReyAblIZAbnsr96nnEPxlQa3YxC6VfS6yVa9w==";
            var publicKey = "MIGdMA0GCSqGSIb3DQEBAQUAA4GLADCBhwKBgQDdRINf4GyRJFW0Jhmc2h5GfcgMXzSwqyWnUvoH7LX14mf7JLWqZgxOE3jPxjiZgDoe3fWvDLkUq2qrXsDkycux8WVoHH6Q/Spttl0uk8nniK4WmUEnmdkaH4r/tlgiG7odbCPbd6GIZwXR4U5ehblDoh5HfSF6GWLCwSPiTuRd5wIBAw==";
            var privateKey = "";// "MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAN1Eg1/gbJEkVbQmGZzaHkZ9yAxfNLCrJadS+gfstfXiZ/sktapmDE4TeM/GOJmAOh7d9a8MuRSraqtewOTJy7HxZWgcfpD9Km22XS6TyeeIrhaZQSeZ2Rofiv+2WCIbuh1sI9t3oYhnBdHhTl6FuUOiHkd9IXoZYsLBI+JO5F3nAgEDAoGAJODAj/q8wttjngZZmiRaYRT2rLqIyByGRo3UVqdzqPsRVIYeRxECDQM+zUu0GZVfBST+R9d0LhyRxzp1e3b3SAOQov40qrfXZmpvGyo9GYGqiqKh7e4VHxFYUYkmuzBFQflUUgrVoQMp08v0Qn53Cj2IH5R/r1AhNPBnQqpjBFsCQQDu+9uFmvnXGMs9gHBZA3IHPW9joaGT2cK46zW1WV5A2Iuj6b1wWzcvYJWTejyI2UKafITC8tuFqIwCsEmYem0xAkEA7QW6oaeW1wU7+kIbPVfcd3FnZdPycYCc/o3gyhZguUIF8EAxxkSLJKpFhhqTBhXDlnEFPzCCsvL5HAgIuBfWlwJBAJ9SklkR++S7MikASuYCTATTn5fBFmKRLHtHeSOQ6YCQXRfxKPWSJMpAY7em0wXmLGb9rddMklkbCAHK27r8SMsCQQCeA9HBGmSPWNKm1rzTj+hPoO+ZN/b2Vb3/CUCGuZXQ1q6gKsvZgwdtxtkEEbdZY9e5oK4qIFch91C9WrB6uo8PAkEAyIq/G9SzluZ5hcvn3+gSHkwlhD4UtUYLIu6Vf3ejGC2Cd7TggReyAblIZAbnsr96nnEPxlQa3YxC6VfS6yVa9w==";

            var encrpted = confuser.EncryptByPrivateKey(s, privateKey);
            var decrpted = confuser.DecryptByPublicKey(encrpted, publicKey);

            Console.ReadLine();
        }
    }
}
