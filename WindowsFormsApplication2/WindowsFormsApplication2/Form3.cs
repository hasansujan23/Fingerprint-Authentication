using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace WindowsFormsApplication2
{
    public partial class Form3 : Form
    {
        public static int[] n = new int[15];
        public Form3()
        {
            InitializeComponent();
            getUserName();
            showData();
            string otpKey = getOtp();
            string encryptedData = encryptedText(textBox1.Text, otpKey);
            textBox1.Text = encryptedData;
            try
            {
                sendEmail(getEmailAddress(),otpKey);
                MessageBox.Show("Your otp send to your email");
            }
            catch 
            {
                MessageBox.Show("Network problem. Please try again later!!");
            }
        }
        public void getUserName()
        {
            string cs = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(cs))
            {
                MySqlCommand cmd = new MySqlCommand("select * from t_user where user_id='" + LoginInfo.userId + "'", con);
                con.Open();
                MySqlDataReader reder = cmd.ExecuteReader();
                while (reder.Read())
                {
                    string firstName = reder["first_name"].ToString();
                    string lastName = reder["last_name"].ToString();
                    string name = firstName + " " + lastName;
                    label3.Text = "Welcome " + name;
                    
                }


            }
        }

        public void showData()
        {
            string cs = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(cs))
            {
                MySqlCommand cmd = new MySqlCommand("select * from t_user_data where user_id='"+LoginInfo.userId+"'", con);
                con.Open();
                MySqlDataReader reder = cmd.ExecuteReader();
                while (reder.Read())
                {
                    string data = reder["data"].ToString();
                    textBox1.AppendText(data+"\n");
                }


            }
        }

        public string getEmailAddress()
        {
            string emailAddress = "";
            string cs = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(cs))
            {
                MySqlCommand cmd = new MySqlCommand("select email from t_user where user_id='" + LoginInfo.userId + "'", con);
                con.Open();
                MySqlDataReader reder = cmd.ExecuteReader();
                while (reder.Read())
                {
                    emailAddress = reder["email"].ToString();
                }
                return emailAddress;
            }
        }

        public void sendEmail(string toUser, string mailBody)
        {
            MailMessage mailMessage = new MailMessage("hasansujan23@gmail.com",toUser);
            mailMessage.Subject = "OTP From Server";
            mailMessage.Body = mailBody;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com",587);
            smtpClient.Credentials = new System.Net.NetworkCredential()
            {
                UserName = "hasansujan23@gmail.com",
                Password="01754704559"
            };
            smtpClient.EnableSsl = true;
            smtpClient.Send(mailMessage);
        }

        public byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        public byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        try
                        {
                            cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                            cs.Close();
                        }
                        catch
                        {
                            MessageBox.Show("Invalid key");
                        }
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

        public string encryptedText(string input, string pwd)
        {
            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(input);
            byte[] pwdBytes = Encoding.UTF8.GetBytes(pwd);
            pwdBytes = SHA256.Create().ComputeHash(pwdBytes);
            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, pwdBytes);
            string result = Convert.ToBase64String(bytesEncrypted);
            return result;
        }

        public string DecryptText(string input, string password)
        {
            byte[] bytesToBeDecrypted = Convert.FromBase64String(input);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            string result = Encoding.UTF8.GetString(bytesDecrypted);

            return result;
        }

        // Generate a random string with a given size
        public static string getOtp()
        {
            string allCharacter = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*(){}[];?<>_+-,.";
            string otp = "";
            for (int ctr = 0; ctr < 14; ctr++)
                n[ctr] = -1;
            Random random = new Random();

            //for (int ctr = 0; ctr <= 14; ctr++)
            //    n[ctr] = random.Next(95);
            int ck = 0;
            int count = 0;
            int temp = -1;
            while (count < 15)
            {
                temp = random.Next(85);
                ck = checkPassword(temp);
                if (ck == 0)
                {
                    n[count] = temp;
                    count = count + 1;
                }
                //count = count + 1;
            }

            for (int i = 0; i < 14; i++)
                otp = otp + allCharacter[n[i]];
            return otp;
        }
        public static int checkPassword(int num)
        {
            int ck = 0;
            for (int i = 0; i < 14; i++)
            {
                if (n[i] == num)
                    ck = 1;
            }
            return ck;
        } 

        private void button2_Click(object sender, EventArgs e)
        {
            LoginInfo.userId = null;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form f4 = new Form4();
            f4.Show();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = DecryptText(textBox1.Text,textBox2.Text);
        }

    }
}
