using System;
using System.Windows.Forms;

namespace TurbinRapor
{
    public partial class CreateReport : Form
    {
        public CreateReport()
        {
            InitializeComponent();
            dateTimePicker1.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            dateTimePicker2.CustomFormat = "dd/MM/yyyy HH:mm:ss";

            dateTimePicker1.Value = DateTime.Now.AddDays(-1);
            dateTimePicker2.Value = DateTime.Now;
        }

        private void ReportCreate_Click(object sender, EventArgs e)
        {
            DateTime start = dateTimePicker1.Value;
            DateTime end = dateTimePicker2.Value;

            string ReportFileName = "TurbinRapor_" + start.ToShortDateString().Replace('/', '-') + "_" + end.ToShortDateString().Replace('/', '-');

            SaveFileDialog SaveFile = new SaveFileDialog();
            SaveFile.Title = "SoyutWind Rapor Kayit";
            SaveFile.Filter = "CSV Veri Dosyasi|*.csv";
            SaveFile.FileName = ReportFileName;
            DialogResult Result = SaveFile.ShowDialog(this);

            if ((DialogResult.Cancel == Result) || (string.Empty == SaveFile.FileName))
            {
                return;
            }

            try
            {
                new ReportManager().GenerateReport(start, end, SaveFile.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "HATA!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayValues_Click(object sender, EventArgs e)
        {
            new MainForm().Show(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new PGUtils().TestPGSql();
        }
    }
}