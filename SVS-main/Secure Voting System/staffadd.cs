using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace Secure_Voting_System
{
    public partial class staffadd : Form
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["SQL_CONNECTION"]);
        public staffadd()
        {
            InitializeComponent();

            dateTimePicker1.MaxDate = DateTime.Today.AddYears(-18); // max date so that staff must 18 yrs
        }


        private void button1_Click(object sender, EventArgs e)
        {
            String usr = textBox1.Text;
            String pass = textBox2.Text;
            String name = textBox3.Text;
            String dob = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            String gender = comboBox1.Text;
            String address = textBox5.Text;

            if (usr == "") {
                new FloatingNotification(this, "username is mandatory", "#d33235");
                return;
            }
            if (pass == "")
            {
                new FloatingNotification(this, "password is mandatory", "#d33235");
                return;
            }
            if (name == "")
            {
                new FloatingNotification(this, "name is mandatory", "#d33235");
                return;
            }
            if (dob == "")
            {
                new FloatingNotification(this, "dob is mandatory", "#d33235");
                return;
            }
            if (gender == "")
            {
                new FloatingNotification(this, "gender is mandatory", "#d33235");
                return;
            }
            if (address == "")
            {
                new FloatingNotification(this, "address is mandatory", "#d33235");
                return;
            }


            con.Open();
            String query1 = "SELECT * FROM login_details WHERE username=@usr";
            SqlCommand cmd = new SqlCommand(query1, con);
            cmd.Parameters.AddWithValue("@usr", usr);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read()) {
                new FloatingNotification(this, "Username already exists", "#d33235");
                con.Close();
                return;
            }
            con.Close();

            con.Open();
            SqlTransaction transaction = con.BeginTransaction();

            String query2 = "INSERT INTO login_details VALUES(COALESCE((SELECT MAX(lid)+1 FROM login_details), 1), @usr, @pass, 'staff')";
            SqlCommand cmd2 = new SqlCommand(query2, con, transaction);
            cmd2.Parameters.AddWithValue("@usr", usr);
            cmd2.Parameters.AddWithValue("@pass", pass);
            cmd2.ExecuteNonQuery();

            String query3 = "INSERT INTO staff VALUES(COALESCE((SELECT MAX(sid)+1 FROM staff), 1), @name, @dob, @gender, (SELECT lid FROM login_details where username=@usr), @address)";
            SqlCommand cmd3 = new SqlCommand(query3, con,transaction);
            cmd3.Parameters.AddWithValue("@usr", usr);
            cmd3.Parameters.AddWithValue("@name", name);
            cmd3.Parameters.AddWithValue("@dob", dob);
            cmd3.Parameters.AddWithValue("gender", gender);
            cmd3.Parameters.AddWithValue("@address",address );
            cmd3.ExecuteNonQuery();

            transaction.Commit();
            con.Close();

            new FloatingNotification(this, "staff added", "#41a45d");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox5.Text = "";
            comboBox1.SelectedIndex = -1;
            dateTimePicker1.Value = new DateTime(2000, 1, 1, 0, 0, 0);
        }
    }
}
