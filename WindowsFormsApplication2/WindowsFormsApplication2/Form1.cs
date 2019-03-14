using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        SerialPort myPort;
        string in_data;
        string[] ports;
        static string userId;
        static string authenticUserId;
        static string deleteId;
        static string tempId="";
        static string temp="";
        public Form1()
        {
            InitializeComponent();
            closeBtn.Enabled = false;
            enrollBtn.Enabled = false;
            deleteBtn.Enabled = false;
            loginBtnClick.Enabled = false;
            registerBtnClick.Enabled = false;
            saveBtn.Enabled = false;

            getAvailablePorts();
            foreach(string port in ports)
            {
                comboBox1.Items.Add(port);
                if (ports[0] != null)
                { 
                    comboBox1.SelectedItem=ports[0];
                }
            }
        }

        public void getAvailablePorts()
        {
            ports = SerialPort.GetPortNames();
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            myPort = new SerialPort();
            myPort.BaudRate = 9600;
            myPort.PortName = comboBox1.Text;
            myPort.Parity = Parity.None;
            myPort.DataBits = 8;
            myPort.StopBits = StopBits.One;
            myPort.DataReceived += myPort_DataReceived;
            try
            {
                myPort.Open();
                MessageBox.Show("Connected");
                closeBtn.Enabled = true;
                enrollBtn.Enabled = true;
                deleteBtn.Enabled = true;
                loginBtnClick.Enabled = true;
                registerBtnClick.Enabled = true;
                saveBtn.Enabled = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message+" Error!!");
            }

        }

        void myPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            in_data = myPort.ReadLine();
            this.Invoke(new EventHandler(displaydata_event));

            string id = in_data.Substring(0, 2);
            if (id == "ID")
            {
                userId = in_data;
            }

            if (in_data.Length >= 5)
            {
                string data = in_data.Substring(0, 5);
                if (data == "Found")
                {
                    MessageBox.Show(in_data);
                    authenticUserId = in_data.Substring(10,2);
                    authenticUserId = authenticUserId.Replace(" ","");
                    LoginInfo.userId = authenticUserId;
                    
                }
                if (data == "Store")
                {
                    MessageBox.Show("Enroll "+userId+" completed.");
                    authenticUserId = userId.Substring(userId.Length-2,2);
                    authenticUserId = authenticUserId.Replace(" ", "");
                    //textBox1.Text = authenticUserId;
                }
            }
            if (in_data.Length >= 11)
            {
                if (in_data.Substring(0, 11) == "Deleting ID")
                {
                    deleteId = in_data.Substring(in_data.Length - 2, 2);
                    deleteId = deleteId.Replace("#", "");
                    //MessageBox.Show(deleteId + " deleted");
                    deleteUser(deleteId);
                }
            }
            if (in_data.Length > 80)
            {
                temp = in_data;
                //MessageBox.Show(temp);
                //MessageBox.Show(tempId);
                //textBox1.Text = temp;
            }
        }

        public void deleteUser(string id)
        {
            string cs = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
            int row = 0;
            using (MySqlConnection con = new MySqlConnection(cs))
            {
                MySqlCommand cmd = new MySqlCommand("delete from t_user where user_id='"+id+"'", con);
                con.Open();
                row = cmd.ExecuteNonQuery();
                con.Close();
                if (row > 0)
                {
                    MessageBox.Show("Delete id");
                }

            }
        }

        private void displaydata_event(object sender, EventArgs e)
        {
            //listBox1.Items.Add(in_data);
            displayTextBox.AppendText(in_data+"\n");
        }

        private void enrollBtn_Click(object sender, EventArgs e)
        {
            //listBox1.Items.Clear();
            displayTextBox.Clear();
            myPort.Write("1");
        }

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            //listBox1.Items.Clear();
            myPort.Write("2");
        }

        private void sendBtn_Click(object sender, EventArgs e)
        {
            myPort.Write(textBox2.Text);
            tempId = textBox2.Text;
            textBox2.Text = "";
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            //listBox1.Items.Clear();
            //myPort.Close();
            displayTextBox.Clear();
        }

        private void loginBtnClick_Click(object sender, EventArgs e)
        {
            if (LoginInfo.userId != null)
            {
                Form f3 = new Form3();
                f3.Show();
            }
            else
            {
                MessageBox.Show("Please complete fringerprint verification process.");
            }
            
        }

        private void registerBtnClick_Click(object sender, EventArgs e)
        {
            if (authenticUserId!=null)
            {
                Form f2 = new Form2(authenticUserId);
                f2.Show();
                authenticUserId = null;
            }
            else
            {
                MessageBox.Show("Please complete fringerprint verification process.");
            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (temp != "" && tempId != "")
            {
                string cs = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
                int row = 0;
                using (MySqlConnection con = new MySqlConnection(cs))
                {
                    MySqlCommand cmd = new MySqlCommand("insert into t_template values('"+tempId+"','"+temp+"')", con);
                    con.Open();
                    try
                    {
                        row = cmd.ExecuteNonQuery();
                    }
                    catch
                    {
                        MessageBox.Show("Template already exist.");
                    }
                    con.Close();
                    if (row > 0)
                    {
                        MessageBox.Show("Save Template successfully");
                    }

                }
            }
            else
            {
                MessageBox.Show("Don't have any template");
            }
        }
    }
}
