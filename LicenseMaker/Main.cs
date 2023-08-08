using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;
using Sunup.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LicenseMaker
{
    public partial class Main : Form
    {
        private string keyFilePath;
        
        public Main()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openImageDialog = new OpenFileDialog();
            openImageDialog.Filter = "Code File (*.bin)|*.bin;";
            openImageDialog.Multiselect = false;

            if (openImageDialog.ShowDialog() == DialogResult.OK)
            {
                keyFilePath = openImageDialog.FileName;
                lblFilePath.Text = keyFilePath;
                MessageBox.Show("Loaded " + keyFilePath);
            }
            else
            {
            }
        }

        private void btnGenerateLicense_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(keyFilePath))
            {
                return;
            }

             var keyFileNameWithoutExt = Path.GetFileNameWithoutExtension(keyFilePath);
             SaveFileDialog saveImageDialog = new SaveFileDialog();
            saveImageDialog.FileName = keyFileNameWithoutExt + ".lic";
            saveImageDialog.Filter = "License File (*.lic)|**.lic";
            //openImageDialog.Multiselect = false;

            if (saveImageDialog.ShowDialog() == DialogResult.OK)
            {
                var fileProcess = new FileProcess();
                if (fileProcess.ReadFile(keyFilePath, out byte[] data) && data.Length>0)
                {
                    var content = Encoding.UTF8.GetString(data);
                    content = content + ">>>>>" + txtPointNumber.Text + ">>>>>" + txtConNum.Text;
                    var dataWithPara = Encoding.UTF8.GetBytes(content);
                    ////var privateKey = "MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAN1Eg1/gbJEkVbQmGZzaHkZ9yAxfNLCrJadS+gfstfXiZ/sktapmDE4TeM/GOJmAOh7d9a8MuRSraqtewOTJy7HxZWgcfpD9Km22XS6TyeeIrhaZQSeZ2Rofiv+2WCIbuh1sI9t3oYhnBdHhTl6FuUOiHkd9IXoZYsLBI+JO5F3nAgEDAoGAJODAj/q8wttjngZZmiRaYRT2rLqIyByGRo3UVqdzqPsRVIYeRxECDQM+zUu0GZVfBST+R9d0LhyRxzp1e3b3SAOQov40qrfXZmpvGyo9GYGqiqKh7e4VHxFYUYkmuzBFQflUUgrVoQMp08v0Qn53Cj2IH5R/r1AhNPBnQqpjBFsCQQDu+9uFmvnXGMs9gHBZA3IHPW9joaGT2cK46zW1WV5A2Iuj6b1wWzcvYJWTejyI2UKafITC8tuFqIwCsEmYem0xAkEA7QW6oaeW1wU7+kIbPVfcd3FnZdPycYCc/o3gyhZguUIF8EAxxkSLJKpFhhqTBhXDlnEFPzCCsvL5HAgIuBfWlwJBAJ9SklkR++S7MikASuYCTATTn5fBFmKRLHtHeSOQ6YCQXRfxKPWSJMpAY7em0wXmLGb9rddMklkbCAHK27r8SMsCQQCeA9HBGmSPWNKm1rzTj+hPoO+ZN/b2Vb3/CUCGuZXQ1q6gKsvZgwdtxtkEEbdZY9e5oK4qIFch91C9WrB6uo8PAkEAyIq/G9SzluZ5hcvn3+gSHkwlhD4UtUYLIu6Vf3ejGC2Cd7TggReyAblIZAbnsr96nnEPxlQa3YxC6VfS6yVa9w==";
                    //var privateKey = "MIIEuwIBADANBgkqhkiG9w0BAQEFAASCBKUwggShAgEAAoIBAQCQKlM2JME5yIPGTddLbuwg9BKSAX0JUH89U0qTCh2BffHdBRwawRUEvSEvMveA8G4erZHkWk8GNyjY77cIXk5f6hiUD3XFvYYG7jzXoMkn80tcVn/N49N7r7v78nhgtblFd/e5W91735fesjfnaghEITJU+TMqruHNCrncK3L12TNBExLma2QWE+BcW3lwIZz69tkv8Ngju7fSeUVz3h/SfacV/SGfHEtbt2TpJN7twevgR2xpvFgaipbkg8RnQqbmX0iXMRIL3B7chWZCUgFuXR5f3xejdm1hZcS7D+K5ci0fTuZgYYE8AZw8jCj07h9OxZPNo0P/DYKZtVWThEHDAgEDAoIBABgHDd5bdYmha0u3o+Hn0gV+AxhVlNbiv9+N4cMsWkA/qE+A2gR1g4DKMDKIfpV9Z6/HmFC5t9Zehs7SnoFlDQ/8WW4Ck6D066vSX3lFdtv94eS5FUz7Tenyn1SoaWVznuDpU/Q5+j9P7qUds/vnAWCwMw4piIcdJaIsdE9ckyj5SEcNXlhdQJ5HtYb2zwJFeoPw//Q8HvBys3wQXgz39Y4hs1cdWpcVyYM/Ttfx71UJxSkXbVFXqu8amloUTJrX6qNZTFReCIlHzuGwKHwtevOiUiMdFge1uoco1yqt2Fa52h8kJr6vkTyr0tTq0GoRjpawp2lJrDtIGB1hrWySJG8CgYEA0QX6OtB2uJokbmTLmY3aFdRWysjYTc40fBKnAS/fOSb4Un8vR1XF1sWLjBjDJl/YNw8TOXHZnUAMtLqKayd4wH57Eeok+Hmi2mBWkw5PKHsd5NzMrf3663Lac2ZpKEzeH27hD8JpiWG9QzB6hPSGW5/yWM8qeY2aG700IUP+uCcCgYEAsJDIogPFJ8ZBMM3HBdSmqA76Dq2v0LLXBtdwD/YvJVa7IIwdur7Tu4Kv/8C2HI+vFdqnow/UuT/eRA1/jZ66ZpPMbLLX5lqKazIN4Erx9z1xTLBj5Xc5IsOWRlSTqGQ48PWU7iXakG49aA6QwYP+bCavToa+wQ3C1iw3I8QYrwUCgYEAi1lRfIr50GbC9EMyZl6RY+LkhzCQM97NqAxvVh/qJhn64aofhOPZOdkHsrssxD/legoM0PaRE4AIeHxcR2+l1amnYUbDUFEXPEA5t17fcFIT7eiIc/6nR6Hm95mbcDM+v59AtSxGW5Z+LMr8WKMEPRVMOzTG+7O8En4iwNf/JW8CgYB1tdsWrS4ahCt13oSujcRwCfwJyR/gdzoEj6AKpB9uOdIVsr58fzfSVx//1c69tR9j5xpstTh7f+mCs6peadGZt92dzI/u5wbyIV6Vh0v6KPYzIEKY+iYXLQ7ZjbfFmCX1+Q30GTxgSX5FXwsrrVRIGco0WdSAs9c5cs9tLWXKAwKBgDe0pkuYgv3cm6egjBunxNF5AONLC8AAtKl082WPsiOFw8te2vyt7qF0ZiZmlhcg7SkQff6Bl0stYdc7DKoPRB01rQzxKA5sxOzaz6waZGBkLH0a6BxyTD3HjvAc3YyMMheH6aFLPaFFAUqKtZIB1FAu+oaZC78O6/8vAgehsvFF";
                    var privateKey = "MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCr3QpHw/UjCv1G27C+C1YbgwstCAilbVu32vll9+oJEnDcq58wa/K1IvfEer2FP6qHfy4nH1gejIZv6XkfRTtUdADLuLu4mOVb3eFcGn0Hv7Ufw9hrJzhvM9R9inBGyE3XQSsCj26C0MTaLgenqnOmxDM3DOTdMKCjKIioBwcJAmquN+cq8w2k/j3mBba/Dg1Y4onWLmiYrbGYGg/m9RvCZoj13uFP5+At2a5hYHxgRgzs1p4t71AJbjnYVdokKytzodh3OrfjuVJcp1i6Lq/MfcgpghlsQL+sQnec++Ugdt/TBSrGp6h6mJez0xWG5cfZHZgzKMal3P7BR4zB4rUtAgEDAoIBAByk1wv1/jCB1OEknXUB469AgdzWrBuSOfP51DupUawtvXocmogR/cjbKUtpykDf8cE/3QaFOVpswRKm6YU2NI4TVXdJdJ7EJjn6UDoEaiv1SNqgpBHb3r0zThTsaAvMDPk1hysX58B4IM8Hq/FHE0Z2CIks0M+IGsXcFsar1oGAIIe0npNw/XQkMewkwqZpvmO/59bAKgUCElyhmC34pxuFO9B86+Uf+zzUIzOJJgFjSMZyFuBgv+qR3DD7GWN6MvmVUQVInk6OTmRfuBMErL7jat0OpEMJi9M0mKx0rW8sBIKyfhNIRxfPUBe5vnl09d7JYhcqBYzy5eVKKNghZxUCgYEA8MAJamIx+VvqnUF1WjT5bNzG7o7ZtAGxinoRCA0q1h6ug4s2Tt49bKUu2YWhKaeP0Y3sR7bNl6ZoD+RT00O8oRn/Mv0CZlBgGYAtJKV7EJadnH40HYGYHQzCLC+5WmdUr0+xOdVQD4bXvbAJJ3zE+BKVsxxTFFO27Z5KIS0hqqcCgYEAtr/yxVQbI5A6dRuzzKOaKiWcLD7TuHjvuPI/dsX2Wv6Ynoe7CxLqkBuyAaaIbrBihrg9zTTdOOObAM4fbouRWIAiv1qEm7wDYoI746KXkryLqnz2IlhvW6xEt2KGeh4aFXMk/H2l7mTk+XVvdzFjDI3PmIvZkQR0rcNAdoP4oAsCgYEAoIAGRuwhUOfxviujkXimSJMvSbSRIqvLsaa2BV4cjr8fAlzO3z7TncN0kQPAxm+1Nl6dhSSJD8RFX+2NN4J9wLv/d1NW7uBAEQAeGG5SCw8Tval4E6u6vgiBcsp7kZo4dN/Le+OKtQSP08qwxP3YpWG5IhLiDY0knmmGwMjBHG8CgYB51UyDjWdtCtGjZ80zF7wcGRLIKeJ6+0p7TCpPLqQ8qbsUWnyyDJxgEnarxFr0dZcEetPeIz4l7RIAiWpJsmDlqsHU5wMSfVeXAX1CbGUMfbJxqKQW5Z+Scth6Qa78FBFjohioU8P0Q0NQ+PT6IOyzCTUQXTu2Avhz14BPAqXABwKBgQDHiNMOAiRyFq/FYk9BdOg3oT8VvwUZH1NeVSx0OQ8vDvg3MLsYqL0x3Et9XmmeYIoWi1OSpweAnkQQUNDfxDzUwnqChxVrj3UFKCb7yJ62CntTC2jhYC/fPtKNdrFMPehcCORx6YOJ6mLyvtbinoAiYpvdG3uyHwt9MJvGoScR/w=="; 
                    var encryption = new RSACryptoService();
                    var encrpted = encryption.EncryptByPrivateKey(dataWithPara, privateKey);
                    if (encrpted != null)
                    {
                        if (fileProcess.SaveFile(saveImageDialog.FileName, encrpted))
                        {
                            MessageBox.Show("Succeed to save file:" + saveImageDialog.FileName);
                        }
                        else
                        {
                            MessageBox.Show("Fail to save file:" + saveImageDialog.FileName);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Fail to encrypt file:" + saveImageDialog.FileName);
                    }
                }
                else
                {
                    MessageBox.Show("Fail to read file:" + saveImageDialog.FileName);
                }
            }
            else
            {
            }
        }
    }
}
