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
        Chart chart1;
        Series pieSeries;
        int totalCount;

        public viewresult()
        {
            InitializeComponent();

            initChart();

            setChart();

            panel1.Controls.Add(chart1);
            chart1.Dock = DockStyle.Fill;
        }
        public void initChart()
        {
            chart1 = new Chart();
            chart1.Palette = ChartColorPalette.BrightPastel;

            ChartArea pieChartArea = new ChartArea("PieChartArea");
            chart1.ChartAreas.Add(pieChartArea);

            pieSeries = new Series("PieSeries");
            pieSeries.ChartType = SeriesChartType.Pie;
            pieSeries.ChartArea = "PieChartArea";
            chart1.Series.Add(pieSeries);

            //pieSeries.Points.AddXY("fgkjfkgj", 20);

            pieSeries.ToolTip = "value: #VALX: percentage: #VALY%";
        }



        public void setChart(){
            con.Open();
            string query = "SELECT COUNT(vote) AS total FROM vote WHERE eid=@eid";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@eid", textBox1.Text);
            SqlDataReader data = cmd.ExecuteReader();
            while(data.Read()){
                totalCount = Convert.ToInt32(data["total"]);
            }
            data.Close();

            string query2 = "SELECT * FROM candidate WHERE eid=@eid";
            SqlCommand cmd2 = new SqlCommand(query2, con);
            cmd2.Parameters.AddWithValue("@eid", textBox1.Text);
            SqlDataReader data2 = cmd.ExecuteReader();
            while(data2.Read()){

                string query3 = "SELECT COUNT(vote) AS count FROM vote WHERE vote=@cid";
                SqlCommand cmd3 = new SqlCommand(query3, con);
                cmd2.Parameters.AddWithValue("@eid", data2[1].ToString());
                SqlDataReader data3 = cmd.ExecuteReader();
                while(data3.Read()){
                    pieSeries.Points.AddXY(data2[2], Convert.ToInt32(data3["count"]));
                }
            }
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void viewresult_Load(object sender, EventArgs e)
        {

        }
    }
}
