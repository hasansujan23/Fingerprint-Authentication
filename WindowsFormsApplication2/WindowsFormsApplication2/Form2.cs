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

namespace WindowsFormsApplication2
{
    public partial class Form2 : Form
    {
        string userId;
        public Form2(string Id)
        {
            InitializeComponent();
            userId = Id;
            userId = userId.Replace(" ","");
            textBox1.Text = userId;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string cs = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
            int row = 0;
            using (MySqlConnection con = new MySqlConnection(cs))
            {
                MySqlCommand cmd = new MySqlCommand("insert into t_user values('"+userId+"','" + textBox1.Text + "','" + textBox2.Text + "','"+textBox3.Text+"')", con);
                con.Open();
                row = cmd.ExecuteNonQuery();
                con.Close();
                if (row > 0)
                {
                    MessageBox.Show("Registration Complete");
                }
                else {
                    MessageBox.Show("Registration failed!! Please try agin..");
                }

            }
        }
    }
}
