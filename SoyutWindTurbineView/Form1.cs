using System;
//using System.Drawing;
using S7.Net;
using System.Windows.Forms;

namespace SoyutWindTurbineView
{
    public partial class Form1 : Form
    {
        Plc Vipa;

        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            double Result;
            short Result2;

            Result = ((uint)Vipa.Read("MD396")).ConvertToDouble();
            Console.WriteLine("Ruzgar Hiz: " + Result);

            Result2 = ((ushort)Vipa.Read("MW454")).ConvertToShort();
            Console.WriteLine("Ruzgar Fark: " + (180 - Result2));

            Result2 = ((ushort)Vipa.Read("DB4.DBW0")).ConvertToShort();
            Console.WriteLine("Ruzgar Ust Limit: " + (Result2));

            label1.Text = Result.ToString("0.000");
            progressBar1.Maximum = Result2 * 10;
            progressBar1.Value = Convert.ToInt32(Result * 10);

            bool fren = (bool)Vipa.Read("M17.2"); //FREN_KAPALI

            if (fren)
                panel1.BackColor = System.Drawing.Color.Red;
            else
                panel1.BackColor = System.Drawing.Color.Green;

            Console.WriteLine("Fren: " + fren);

            Result2 = ((ushort)Vipa.Read("DB9.DBW30")).ConvertToShort();
            Console.WriteLine("Kanat Max Aci SET: " + Result2);
           
            ///////////////////////////////-----------------DD32 ISE DBD32 YAZ-------------//////////////////////
            int AltMax = ((uint)Vipa.Read("DB9.DBD32")).ConvertToInt();
            numericUpDown1.Value = Result2;
            Console.WriteLine("Alternator Max Devir SET: " + AltMax);

            double RotorDevir = ((uint)Vipa.Read("MD422")).ConvertToDouble();
            //label7.Text = RotorDevir.ToString();
            Console.WriteLine("Rotor Devir: " + RotorDevir);

            double AlternatorDevir = ((uint)Vipa.Read("MD352")).ConvertToDouble();
            label8.Text = AlternatorDevir.ToString();
            Console.WriteLine("Alternator Devir: " + AlternatorDevir);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ErrorCode ECode;

            try
            {
                Vipa = new Plc(CpuType.S7200, "192.168.0.6", 0, 0);
                ECode = Vipa.Open();

                if (ECode != ErrorCode.NoError)
                {
                    if (ECode == ErrorCode.ConnectionError)
                    {
                        MessageBox.Show("Baglanti Hatasi");
                    }

                    Application.Exit();
                    return;
                }

                toolStripProgressBar1.Value = toolStripProgressBar1.Maximum;
                timer1_Tick(null, null);
                timer2_Tick(null, null);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Vipa.IsConnected)
                Vipa.Close();
            toolStripProgressBar1.Value = toolStripProgressBar1.Minimum;

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0), 2.3f);
            //e.Graphics.DrawLine(pen, 20, 10, 300, 100);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            ushort value = (ushort) numericUpDown1.Value;
            Vipa.Write("DB9.DBW30", value);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            uint value = (uint) numericUpDown2.Value;
            Vipa.Write("DB9.DBD32", value);
        }
    }
}