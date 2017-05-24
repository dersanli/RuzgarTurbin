/*
 * Created by SharpDevelop.
 * User: dersanli
 * Date: 3/6/2017
 * Time: 4:28 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Data;
using System.Data.SqlClient;

namespace DBTransfer
{
	/// <summary>
	/// Description of CopyTable.
	/// </summary>
	public class CopyTable
	{

		string sourceConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SWDBConnectionString"].ConnectionString.Replace("@@PASS@@", "!Soyut8745");
		string targetConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SWDBConnectionString2"].ConnectionString.Replace("@@PASS@@", "!Soyut8745");
		

		public CopyTable()
		{
			try {
				DoCopy();
					
			} catch (Exception) {
				
				throw;
			}
		
		}
		
		
        void DoCopy()
        {

            // Create source connection
			SqlConnection source = new SqlConnection(sourceConnectionString);
			// Create destination connection
			SqlConnection destination = new SqlConnection(targetConnectionString);

			// Clean up destination table. Your destination database must have the
			// table with schema which you are copying data to.
			// Before executing this code, you must create a table BulkDataTable
			// in your database where you are trying to copy data to.
			
			SqlCommand cmd = new SqlCommand("DELETE FROM WTPLCTAGS", destination);
			// Open source and destination connections.
			source.Open();
			destination.Open();
			cmd.ExecuteNonQuery();
			// Select data from Products table
			cmd = new SqlCommand("SELECT * FROM WTPLCTAGS", source);
			// Execute reader
			SqlDataReader reader = cmd.ExecuteReader();
			// Create SqlBulkCopy
			SqlBulkCopy bulkData = new SqlBulkCopy(destination);
			// Set destination table name
			bulkData.DestinationTableName = "WTPLCTAGS";
			// Write data
			bulkData.WriteToServer(reader);
			// Close objects
			bulkData.Close();
			destination.Close();
			source.Close();
        }
	}
}