using System;
using System.Collections;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace TurbinRapor
{
    public partial class MainForm : Form
    {
        string QueryColumn = string.Empty;
        TimeSpan RaporSure;
        ArrayList QueryColumns;

        public MainForm()
        {
            InitializeComponent();
            QueryColumns = new ArrayList();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            dateTimePicker1.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            dateTimePicker2.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "dd/MM/yyyy HH:mm:ss";
            
            // TODO: Form un ilk selection halini yap
            treeView1.SelectedNode = treeView1.Nodes[0];
            treeView1.Nodes[0].Checked = true;
            toolStripButton1.Select();
            toolStripButton1.PerformClick();
        }

        private void cikisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void DisplayChart()
        {
            QueryColumn = string.Empty;
            chart1.Series.Clear();

            if (QueryColumns.Count == 0)
                return;

            foreach (var Column in QueryColumns)
            {
                QueryColumn += Column.ToString() + ", ";
                Series Serie = chart1.Series.Add(Column.ToString());
                Serie.XValueMember = "READDATETIME";
                Serie.YValueMembers = Column.ToString();
                Serie.ChartType = SeriesChartType.Spline;
            }

            if(QueryColumn.Length > 2)
                QueryColumn = QueryColumn.Substring(0, QueryColumn.Length - 2);

            using (DBUtilsRapor dbu = new DBUtilsRapor())
            {
                try
                {
                    //string SelectCommand = "SELECT TOP (100) PERCENT READDATETIME, " + QueryColumn + " AS DATAVALUES FROM dbo.WTPLCTAGS WHERE (READDATETIME > @STARTTIME) AND(READDATETIME < @ENDTIME) ORDER BY WTPLCTAGID DESC";

                    string SelectCommand = "SELECT TOP (100) PERCENT READDATETIME, " + QueryColumn + " FROM dbo.WTPLCTAGS WHERE (READDATETIME > @STARTTIME) AND(READDATETIME < @ENDTIME) ORDER BY WTPLCTAGID DESC";

                    //string SelectCommand = "SELECT TOP (100) PERCENT READDATETIME, WINDSPEED, INTERNALTEMP FROM dbo.WTPLCTAGS WHERE (READDATETIME > @STARTTIME) AND(READDATETIME < @ENDTIME) ORDER BY WTPLCTAGID DESC";

                    chart1.DataSource = dbu.GetDataSet(SelectCommand, dateTimePicker1.Value, dateTimePicker2.Value);

                    dataGridView1.DataSource = dbu.GetDataSet(SelectCommand, dateTimePicker1.Value, dateTimePicker2.Value).Tables[0];
                    chart1.DataBind();
                    chart1.Update();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            //chart1.SaveImage(@"C:\Users\Devrim Ersanli\testimage.jpg", ChartImageFormat.Jpeg);
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            RaporSure = dateTimePicker2.Value - dateTimePicker1.Value;
            DisplayChart();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            RaporSure = dateTimePicker2.Value - dateTimePicker1.Value;
            DisplayChart();
        }

        private void toolStripButton_Click(object sender, EventArgs e)
        {
            foreach (var item in toolStrip1.Items)
            {
                if ("System.Windows.Forms.ToolStripSeparator" == item.ToString())
                    break;
                if (item.ToString() != sender.ToString())
                {
                    ((ToolStripButton)item).Checked = false;
                }
                else
                {
                    ((ToolStripButton)item).Checked = true;
                    switch (item.ToString())
                    {
                        case "1 Saat":
                            RaporSure = new TimeSpan(1, 0, 0);
                            break;
                        case "1 Gun":
                            RaporSure = new TimeSpan(1, 0, 0, 0);
                            break;
                        case "1 Hafta":
                            RaporSure = new TimeSpan(7, 0, 0, 0);
                            break;
                        case "1 Ay":
                            RaporSure = new TimeSpan(30, 0, 0, 0);
                            break;
                        default:
                            break;
                    }
                }
            }

            dateTimePicker1.Value = DateTime.Now - RaporSure;
            dateTimePicker2.Value = DateTime.Now;

            DisplayChart();
        }

        private void toolStripMenuItem_Click(object sender, EventArgs e)
        {

            //using (DBUtilsRapor dbu = new DBUtilsRapor())
            //{
            //    try
            //    {
            //        string ExecuteCommand = @"BACKUP DATABASE [WINDTURBINE] TO  DISK = N'D:\SQLSERVER\MSSQL12.SQLEXP\MSSQL\Backup\WINDTURBINE.bak' WITH NOFORMAT, NOINIT,  NAME = N'WINDTURBINE-Full Database Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10";

            //        dbu.ExecuteNonQuery(ExecuteCommand);

            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //    }
            //}

            //return;

            string SenderMenuName = ((ToolStripMenuItem)sender).Name;
            TimeSpan ReportPeriod = new TimeSpan(1, 0, 0, 0); //assign a default value

            switch (SenderMenuName)
            {
                case "toolStripMenuItem1":
                    //ReportPeriod = new TimeSpan(1, 0, 0, 0);
                    break;
                case "toolStripMenuItem2":
                    ReportPeriod = new TimeSpan(7, 0, 0, 0);
                    break;
                case "toolStripMenuItem3":
                    ReportPeriod = new TimeSpan(30, 0, 0, 0);
                    break;
                default:
                    break;
            }

            SaveFileDialog SaveFile = new SaveFileDialog();
            SaveFile.Title = "SoyutWind Rapor Kaydetme";
            SaveFile.Filter = "CSV Veri Dosyasi|*.csv";
            SaveFile.FileName = "TurbinRapor_" + DateTime.Now.ToShortDateString().Replace('/','-');
            DialogResult Result = SaveFile.ShowDialog(this);

            if ((DialogResult.Cancel == Result) || (string.Empty == SaveFile.FileName))
            {
                return;
            }

            new ReportManager().GenerateReport(ReportPeriod, SaveFile.FileName);
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            toolStripSplitButton1.ShowDropDown();
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if(e.Node.Name.StartsWith("Top"))
            {
                return;
            }

            if(e.Node.Checked)
            {
                QueryColumns.Add(e.Node.Name);
            }
            DisplayChart();
        }

        private void treeView1_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Name.StartsWith("Top"))
            {
                return;
            }

            if (e.Node.Checked)
            {
                QueryColumns.Remove(e.Node.Name);
            }
        }

        private void haziranToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime start = new DateTime(2016, 9, 25, 0, 0, 0);
            DateTime end = new DateTime(2016, 9, 28, 23, 59, 59);

            TimeSpan ReportPeriod = end - start;

            new ReportManager().GenerateReport(start,end, @"D:\25eylul-28eylul.csv");
        }

        private void kwToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime start = new DateTime(2016, 9, 25, 0, 0, 0);
            DateTime end = new DateTime(2016, 9, 28, 23, 59, 59);

            TimeSpan ReportPeriod = end - start;

            new ReportManager().GenerateReport(start, end, @"D:\25eylul-28eylul.csv");
        }
    }
}