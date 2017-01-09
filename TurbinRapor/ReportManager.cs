using System;
using System.IO;
using System.Data;
using System.Configuration;
using System.Text;

namespace TurbinRapor
{
    class ReportManager : IDisposable
    {
        StreamWriter ReportWriter;

        string CSVDelimiter = ConfigurationManager.AppSettings["CSVDelimiter"];
        private double SQRT3 = Math.Sqrt(3.0);

        public void GenerateReport(TimeSpan ReportPeriod, string OutputFile)
        {
            DateTime ReportEnd = DateTime.Now;
            DateTime ReportStart = ReportEnd - ReportPeriod;

            GenerateReport(ReportStart, ReportEnd, OutputFile);
        }

        public void GenerateReport(DateTime ReportStart, DateTime ReportEnd, string OutputFile)
        {
            CSVDelimiter = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;

            string QueryPart = "READDATETIME AS KAYITTARIHSAAT, WINDSPEED AS RUZGARHIZ, EXTERNALTEMP AS HAVASICAKLIK, INTERNALTEMP AS ICSICAKLIK, GEARBOXTEMP AS DISLIKUTUSUSICAKLIK, LOWSPEEDRPM AS ROTORDEVIR, HIGHSPEEDRPM AS ALTERNATORDEVIR, PACALTERNATORVOLTAGEL1 AS ALTERNATORGERILIML1, PACALTERNATORVOLTAGEL2 AS ALTERNATORGERILIML2, PACALTERNATORVOLTAGEL3 AS ALTERNATORGERILIML3, PACALTERNATORCURRENTL1 AS ALTERNATORAKIML1, PACALTERNATORCURRENTL2 AS ALTERNATORAKIML2, PACALTERNATORCURRENTL3 AS ALTERNATORAKIML3";

            string SelectCommandText = "SELECT TOP (100) PERCENT " + QueryPart + " FROM dbo.WTPLCTAGS WHERE (READDATETIME > @STARTTIME) AND (READDATETIME < @ENDTIME) ORDER BY WTPLCTAGID DESC";

            DataSet ReportDataset;

            try
            {
                using (DBUtilsRapor dbr = new DBUtilsRapor())
                {
                    ReportDataset = dbr.GetDataSet(SelectCommandText, ReportStart, ReportEnd);
                }

                ReportWriter = new StreamWriter(OutputFile);
                WriteReportHeader();
                WriteMaximums(ReportStart, ReportEnd);
                WriteReportTable(ReportDataset);
                ReportWriter.Flush();
                ReportWriter.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void WriteReportHeader()
        {
            StringBuilder ReportHeader = new StringBuilder();

            ReportHeader.AppendLine("S O Y U T W I N D  --  R U Z G A R  T U R B I N  R A P O R U");
            ReportHeader.AppendLine("Corum Il Ozel Idaresi 250kW Ruzgar Turbini - Bogazkale / Corum");
            ReportHeader.AppendLine("Tarih: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            ReportHeader.AppendLine();

            ReportWriter.Write(ReportHeader);
            ReportHeader.Clear();
        }

        private void WriteMaximums( DateTime ReportStart, DateTime ReportEnd)
        {
            StringBuilder Maximums = new StringBuilder();
            DataSet ReportDataset;

            string SelectCommandText = "SELECT TOP (1) READDATETIME, WINDSPEED FROM dbo.WTPLCTAGS WHERE (READDATETIME > @STARTTIME) AND (READDATETIME < @ENDTIME) ORDER BY WINDSPEED DESC";
            string MaxWindSpeed; string MaxWindSpeedDateTime;

            try
            {
                using (DBUtilsRapor dbr = new DBUtilsRapor())
                {
                    ReportDataset = dbr.GetDataSet(SelectCommandText, ReportStart, ReportEnd);
                }

                MaxWindSpeedDateTime = ((DateTime)ReportDataset.Tables[0].Rows[0][0]).ToString("dd/MM/yyyy HH:mm:ss");
                MaxWindSpeed = ((double)ReportDataset.Tables[0].Rows[0][1]).ToString("#.00");
            }
            catch (Exception)
            {
                throw;
            }

            ReportDataset.Clear();

            Maximums.AppendLine("Tablodaki En Yuksek Ruzgar Hizi ve Tarihi: " + MaxWindSpeed + " m/s - " + MaxWindSpeedDateTime);
            Maximums.AppendLine();

            ReportWriter.Write(Maximums);
            Maximums.Clear();
        }   

        private void WriteReportTable(DataSet ReportDataSet)
        {
            DataTable Table = ReportDataSet.Tables[0];
            StringBuilder TableWriter = new StringBuilder();

            string CSVLine = string.Empty;

            foreach (DataColumn dc in Table.Columns)
            {
                CSVLine += dc.ColumnName + CSVDelimiter;
            }

            TableWriter.AppendLine(CSVLine.Substring(0, CSVLine.Length - 1));

            foreach (DataRow dr in Table.Rows)
            {
                CSVLine = string.Empty;
                foreach (object cell in dr.ItemArray)
                {
                    if (cell.GetType() == typeof(DateTime))
                    {
                        CSVLine += ((DateTime)cell).ToString("dd/MM/yyyy HH:mm:ss") + CSVDelimiter;
                    }
                    else if (cell.GetType() == typeof(Double))
                    {
                        CSVLine += ((double)cell).ToString("#.00") + CSVDelimiter;
                    }
                    else
                    {
                        CSVLine += cell.ToString() + CSVDelimiter;
                    }
                }

                TableWriter.AppendLine(CSVLine.Substring(0, CSVLine.Length - 1));
            }

            ReportWriter.Write(TableWriter);
            TableWriter.Clear();
        }

        private double CalculateKW(double VACLL, double Amp, double PowerFactor = 0.99)
        {
            double CalculatedKW = 0.0f;

            CalculatedKW = SQRT3 * PowerFactor * VACLL * Amp;

            return CalculatedKW;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                ReportWriter.Dispose();
            }
            // free native resources if there are any.
        }
    }
}