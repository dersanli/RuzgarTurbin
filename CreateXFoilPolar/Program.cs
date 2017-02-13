using System;
using System.Diagnostics;
using System.IO;
using System.Configuration;

namespace CreateXFoilPolar
{
    class Program
    {
        static string AirfoilPath;
        static string PolarOutputPath;
        static string XFoilPath;

        static int Main(string[] args)
        {

            XFoilPath = ConfigurationManager.AppSettings["XFoilPath"];
            AirfoilPath = ConfigurationManager.AppSettings["AirFoilPath"];

            try
            {
                CreatePolar();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
        }

        static void CreatePolar()
        {
            if(!File.Exists(XFoilPath))
            {
                throw new FileNotFoundException("XFoil executable not found at: " + XFoilPath);
            }

            if (!File.Exists(AirfoilPath))
            {
                throw new FileNotFoundException("Airfoil file not found at: " + AirfoilPath);
            }

            PolarOutputPath = Path.GetDirectoryName(AirfoilPath) + @"\" + Path.GetFileNameWithoutExtension(AirfoilPath) + ".pol";

            if (File.Exists(PolarOutputPath))
            {
                File.Delete(PolarOutputPath);
            }

            Process process;

            process = new Process();
            process.StartInfo.FileName = XFoilPath;

            // Set UseShellExecute to false for redirection.
            process.StartInfo.UseShellExecute = false;

            // Redirect standard input.  This stream
            // is used synchronously.
            process.StartInfo.RedirectStandardInput = true;

            // Start the process.
            process.Start();

            // Use a stream writer to synchronously write the sort input.
            StreamWriter processInputSW = process.StandardInput;


            processInputSW.WriteLine("LOAD " + AirfoilPath + "");
            //processInputSW.WriteLine("" + PolarName + "");
            //processInputSW.WriteLine("" + "fx27w470a" + ""); //TODO: HARDCODE
            processInputSW.WriteLine("OPER");
            processInputSW.WriteLine("VPAR");
            processInputSW.WriteLine("N " + ConfigurationManager.AppSettings["ncrit"]);
            processInputSW.WriteLine("\n");
            processInputSW.WriteLine("OPER");//?
            processInputSW.WriteLine("VISC");
            processInputSW.WriteLine("" + ""+ ConfigurationManager.AppSettings["re"] + "");
            processInputSW.WriteLine("ITER");
            processInputSW.WriteLine("100");
            processInputSW.WriteLine("PACC");
            processInputSW.WriteLine("" + PolarOutputPath + "");
            processInputSW.WriteLine("\r");
            processInputSW.WriteLine("ASEQ " + ConfigurationManager.AppSettings["alphamin"] + " " + ConfigurationManager.AppSettings["alphamax"] + " " + ConfigurationManager.AppSettings["increment"] + "");
            processInputSW.WriteLine("PACC");
            processInputSW.WriteLine("\r\n");
            processInputSW.WriteLine("QUIT");

            // End the input stream to the sort command.
            processInputSW.Close();

            // Wait for the  process to write the output.
            process.WaitForExit();

            process.Close();
        }
    }
}
