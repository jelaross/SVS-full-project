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
    public partial class editcandidate : Form
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["SQL_CONNECTION"]);
        public editcandidate()
        {
            InitializeComponent();

            fillcombo();
        }

        public void fillcombo(){
            string query = "select election_name from election";
            con.Open();
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                comboBox1.Items.Add(dr[0].ToString());

            }
            con.Close();
        }

        public void fillgrid(DataGridView js)
        {

            con.Open();
            string str = "select cid, name, party from candidate where eid=(select eid from election where election_name='"+ comboBox1.Text + "')";
            DataSet ds = new DataSet();
            SqlDataAdapter sqlda = new SqlDataAdapter(str, con);
            sqlda.Fill(ds);
            js.DataSource = ds.Tables[0].DefaultView;
            con.Close();
        }

        public void fuck() {
            comboBox2.Items.Clear();
            string str = @"select cid from candidate where eid=(select eid from election where election_name=@election_name)";

            con.Open();
            SqlCommand cmd = new SqlCommand(str, con);
            cmd.Parameters.AddWithValue("@election_name", comboBox1.Text);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                comboBox2.Items.Add(dr[0].ToString());

            }
            con.Close();
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {

        }

        private void editcandidate_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            fillgrid(dataGridView1);
            fillgrid(dataGridView2);
            fuck();


        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox2.Text == "")
            {
                new FloatingNotification(this, "Please select Cid", "#d33235");
                return;
            }

            con.Open();
            SqlTransaction transaction = con.BeginTransaction();

            SqlCommand command2 = new SqlCommand("DELETE FROM candidate where cid=@cid", con, transaction);
            command2.Parameters.AddWithValue("@cid", comboBox2.Text);
            command2.ExecuteNonQuery();

            transaction.Commit();
            con.Close();

            fillgrid(dataGridView1);
            fillgrid(dataGridView2);
            fuck();
            MessageBox.Show("deled");
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
