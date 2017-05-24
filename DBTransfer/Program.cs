using System;

namespace DBTransfer
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");

			//new PrintRecords ().GenerateReport (DateTime.Now, DateTime.Now - new TimeSpan (30, 0, 0, 0), "./test.csv");
			
			//new PrintRecords ().GenerateReport (new DateTime(2017,2,10), new DateTime(2017,2,12), "./test.csv");
			
			//new CopyTable();
		}
	}
}
