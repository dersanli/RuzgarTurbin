using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace InsertDataToDB
{
	class MainClass
	{
		static string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SWDBConnectionString"].ConnectionString.Replace("@@PASS@@", "!Soyut8745");
		static SqlConnection conn;
		static string CSVDelimiter = ",";

		static StreamWriter ReportWriter;

		public static void Main (string[] args)
		{

				string QueryPart = "READDATETIME AS KAYITTARIHSAAT, WINDSPEED AS RUZGARHIZ, EXTERNALTEMP AS HAVASICAKLIK, INTERNALTEMP AS ICSICAKLIK, GEARBOXTEMP AS DISLIKUTUSUSICAKLIK, LOWSPEEDRPM AS ROTORDEVIR, HIGHSPEEDRPM AS ALTERNATORDEVIR, PACALTERNATORVOLTAGEL1 AS ALTERNATORGERILIML1, PACALTERNATORVOLTAGEL2 AS ALTERNATORGERILIML2, PACALTERNATORVOLTAGEL3 AS ALTERNATORGERILIML3, PACALTERNATORCURRENTL1 AS ALTERNATORAKIML1, PACALTERNATORCURRENTL2 AS ALTERNATORAKIML2, PACALTERNATORCURRENTL3 AS ALTERNATORAKIML3";

				string SelectCommandText = "SELECT TOP (100) PERCENT " + QueryPart + " FROM dbo.WTPLCTAGS WHERE (READDATETIME > @STARTTIME) AND (READDATETIME < @ENDTIME) ORDER BY WTPLCTAGID DESC";

			DateTime ReportEnd = DateTime.Now;
			DateTime ReportStart = ReportEnd - new TimeSpan(100,0,0,0);



			try
			{
				

				DataSet ReportDataSet = GetDataSet(SelectCommandText, ReportStart, ReportEnd);



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
			catch (Exception)
			{
				throw;
			}



		}


		public static DataSet GetDataSet(string SelectCommandText, DateTime StartTime, DateTime EndTime)
		{
			DataSet ds = new DataSet();

			using (conn = new SqlConnection(ConnectionString))
			{
				conn.Open();

				using (SqlDataAdapter da = new SqlDataAdapter(SelectCommandText, conn))
				{
					da.SelectCommand.Parameters.Add(new SqlParameter("@STARTTIME", StartTime));
					da.SelectCommand.Parameters.Add(new SqlParameter("@ENDTIME", EndTime));
					try
					{
						da.Fill(ds);
					}
					catch (Exception)
					{
						throw;
					}
				}
			}
			return ds;
		}


	}
}
