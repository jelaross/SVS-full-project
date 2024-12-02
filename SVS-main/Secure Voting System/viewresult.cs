using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Secure_Voting_System.Properties;
using System.Data.SqlClient;
using System.Configuration;

namespace Secure_Voting_System
{
    public partial class viewresult : Form
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["SQL_CONNECTION"]);
        string selectedElectionName = null;
        int selectedElectionId = -1;
        Chart chart1;
        Series pieSeries;
        int totalCount;
        List<int> candidateIdList = new List<int>();
        List<string> candidateNameList = new List<string>();
        List<string> partyNameList = new List<string>();

        public viewresult()
        {
            InitializeComponent();

            fillCombo();

            initChart();

            setChart();

            panel1.Controls.Add(chart1);
            chart1.Dock = DockStyle.Fill;
        }

        public void fillCombo() {
            con.Open();
            string query = "SELECT election_name as elecName FROM election ORDER BY election_date DESC";
            SqlCommand cmd = new SqlCommand(query,con);
            SqlDataReader data = cmd.ExecuteReader();
            while(data.Read()){
                comboBox1.Items.Add(data["elecName"].ToString());
            }
            con.Close();
        }

        public void initChart()
        {
            chart1 = new Chart();
            chart1.Palette = ChartColorPalette.BrightPastel;

            ChartArea pieChartArea = new ChartArea("PieChartArea");
            pieChartArea.BackColor = Color.Violet;
            chart1.ChartAreas.Add(pieChartArea);

            pieSeries = new Series("PieSeries");
            pieSeries.ChartType = SeriesChartType.Pie;
            pieSeries.ChartArea = "PieChartArea";
            chart1.Series.Add(pieSeries);

            //pieSeries.ToolTip = "value: #VALX: percentage: #VALY%";
        }



        public void setChart(){
            if (selectedElectionId == -1) {
                return;
            }


            pieSeries.Points.Clear();
            candidateIdList.Clear();
            candidateNameList.Clear();
            partyNameList.Clear();

            con.Open();
            string query = "SELECT COUNT(vote) AS total FROM vote WHERE eid=@eid";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@eid", selectedElectionId);
            SqlDataReader data = cmd.ExecuteReader();
            while(data.Read()){
                totalCount = Convert.ToInt32(data["total"]);
            }
            data.Close();

            if (totalCount == 0) {
                new FloatingNotification(this, "no votes for this election");
                con.Close();
                return;
            }

            string query2 = "SELECT cid as cid, name as name, party as party FROM candidate WHERE eid=@eid";
            SqlCommand cmd2 = new SqlCommand(query2, con);
            cmd2.Parameters.AddWithValue("@eid", selectedElectionId);
            SqlDataReader data2 = cmd2.ExecuteReader();
            while(data2.Read())
            {
                candidateIdList.Add(Convert.ToInt32(data2["cid"]));
                candidateNameList.Add(Convert.ToString(data2["name"]));
                partyNameList.Add(Convert.ToString(data2["party"]));
            }

            int i = 0;
            foreach (int candidateId in candidateIdList) 
            { 
                string query3 = "SELECT COUNT(vote) AS count FROM vote WHERE vote=@cid";
                SqlCommand cmd3 = new SqlCommand(query3, con);
                cmd3.Parameters.AddWithValue("@cid", candidateId);
                SqlDataReader data3 = cmd3.ExecuteReader();

                while (data3.Read())
                {
                    double votesCount = Convert.ToDouble(data3["count"]);
                    double percentage;

                    if (votesCount != 0)
                    {
                        percentage = (votesCount / totalCount) * 100;
                    }
                    else
                    {
                        percentage = 0;
                    }

                    pieSeries.Points.AddXY(candidateId.ToString(), percentage);
                    pieSeries.Points[i].ToolTip = "Votes: " + votesCount + candidateNameList[i] + candidateIdList[i] + partyNameList[i];
                }
                i++;
            }

            con.Close();
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void viewresult_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedElectionName = comboBox1.SelectedItem.ToString();
            selectedElectionId = getElecIdWithName(selectedElectionName);
            setChart();
        }

        private int getElecIdWithName(string elecName) {
            con.Open();
            string query = "SELECT eid as id FROM election WHERE election_name=@election_name";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@election_name", elecName);
            SqlDataReader data = cmd.ExecuteReader();
            while (data.Read())
            {
                int res = Convert.ToInt32(data["id"]);
                con.Close();
                return res;
            }
            con.Close();
            return -1;
        }
    }
}
