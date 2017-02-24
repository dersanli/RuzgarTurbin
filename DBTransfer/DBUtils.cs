using System;
using System.Data;
using System.Data.SqlClient;

namespace DBTransfer
{
    class DBUtils : IDisposable
    {
        bool Disposed;

        SqlConnection conn;

        string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SWDBConnectionString"].ConnectionString.Replace("@@PASS@@", "!Soyut8745");


/*
            READDATETIME,
            WINDSPEED,
            WINDDIRECTION,
            TURBINEDIRECTION,
            LOWSPEEDRPM,
            HIGHSPEEDRPM,
            BRAKEOPEN,
            EMERGENCYBUTTON,
            MANUALMODE,
            STATESECTION,
            HUBBATTERYVOLTAGE,
            EXTERNALTEMP,
            INTERNALTEMP,
            GEARBOXTEMP,
            HUBTEMP,
            BEARINGTEMP,
            GEARBOXOILLEVELLOW,
            YAWDRIVEERROR,
            ETHERNETCOMMS,
            ALTSWITCHON,
            ALTSWITCHOFF,
            HIGHSPEEDRPMSETVALUE,
            BLADEPITCHMAXSETVALUE,
            PACALTERNATORVOLTAGEL1,
            PACALTERNATORVOLTAGEL2,
            PACALTERNATORVOLTAGEL3,
            PACALTERNATORCURRENTL1,
            PACALTERNATORCURRENTL2,
            PACALTERNATORCURRENTL3,
            PACALTERNATORFREQUENCY
*/

        public DataSet GetDataSet(string SelectCommandText, DateTime StartTime, DateTime EndTime)
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

        public void ExecuteNonQuery(string ExecuteCommandText)
        {
            using (conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(ExecuteCommandText))
                {
                    try
                    {
                       cmd.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool Disposing)
        {
            if (Disposed)
                return;

            if (Disposing)
            {
             //   conn.Close();
            }

            // release any unmanaged objects
            // set the object references to null

            Disposed = true;
        }
    }
}