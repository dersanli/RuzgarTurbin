using System;

namespace DBTransfer
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");

			new PrintRecords ().GenerateReport (DateTime.Now, DateTime.Now - new TimeSpan (100, 0, 0, 0), "./test.csv");
		}
	}
}
