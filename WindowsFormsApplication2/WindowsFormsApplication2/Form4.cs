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
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string cs = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
            int row = 0;
            using (MySqlConnection con = new MySqlConnection(cs))
            {
                MySqlCommand cmd = new MySqlCommand("insert into t_user_data(user_id,data) values('" + LoginInfo.userId + "','" + textBox1.Text + "')", con);
                con.Open();
                row = cmd.ExecuteNonQuery();
                con.Close();
                if (row > 0)
                {
                    MessageBox.Show("Data insert successfully.");
                }
                else
                {
                    MessageBox.Show("Failed!! Please try agin..");
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form f3 = new Form3();
            f3.Show();
            this.Close();
        }
    }
}
