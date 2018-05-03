using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Geomagnetic_Sensor_GUI_WAF_
{


	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();



		}

		// Variables --------------

		string[] ports;  // Variable destinada a ser contenedor de la lista de Puertos COM

		string read_command = "$0wn01,71$1";
		string get_x = "$0wnA4rm$1";
		string get_y = "$0wnA7rm$1";
		string get_z = "$0wnAArm$1";

		List<Point> PointDB = new List<Point>();

		int cols, rows; // # de columnas y filas




		//--------------------------


		// Definicion Subclasses Requeridas
		public class Point //  Clase point que guarda los componentes del Campo magnético ( XYZ ) además de la magnitud de dicho punto
		{
			public double x;

			public double y;

			public double z;

			public double mag; // Magnitud

			public Point(double equis = 0.0, double Ye = 0.0, double zeta = 0.0) // Constructor
			{
				x = equis;
				y = Ye;
				z = zeta;
				mag = 0;

			}

			public double Magnitude()  // Obtiene la magnitud del campo magnético usando los componentes XYZ proporcionados
			{
				mag = 0;

				mag = x * x + y * y + z * z;

				mag = Math.Sqrt(mag);

				return mag;
			}

		}


		//Fin de Subclasses requeridas


		// Threads necesarios


		//

		public static void Check_Parameters(SerialPort s)
		{
			while (true)
			{
				if (s.PortName.Length < 0)
				{

				}


			}


		}


		public double Teslas(int numero) // Convierte de nivel a su respectivo valor en MicroTeslas, para nanoTeslas, se multiplica por 1000
		{
			double teslas = 0;

			teslas = (double)numero / 75.0;


			return teslas;
		}

		public int Sin_signo_a_Con_signo(int numero)  // Convierte un numero (Hexadecimal -> decimal) recien unsigned a signed 
		{


			if (numero > 8388607)
			{
				numero = numero - 16777;
			}


			return numero;
		}

		public double Magnitude(double x, double y, double z)  // Obtiene la magnitud del campo magnético usando los componentes XYZ proporcionados
		{
			double mag = 0;

			mag = x * x + y * y + z * z;

			mag = Math.Sqrt(mag);

			return mag;
		}

		public double Lectura(string comando_lectura) // Obtiene lectura de un componente del campo mangnetico
		{
			int componente;
			double final = 0.0;
			string V_no_procesado;


			//System.Windows.Forms.MessageBox.Show("Funcion de lectura");
			Task.Delay(1000).Wait();


			serialPort1.Write(comando_lectura);

			Task.Delay(1000).Wait();

			//consoleTxt_box.AppendText(" " + serialPort1.ReadExisting() + " ");

			V_no_procesado = serialPort1.ReadExisting();

			consoleTxt_box.AppendText(V_no_procesado + " ");

			componente = Convert.ToInt32(V_no_procesado, 16);

			consoleTxt_box.AppendText(componente + " ");

			componente = Sin_signo_a_Con_signo(componente);

			final = Convert.ToDouble(componente);

			Task.Delay(5).Wait();

			return final;
		}

		public void Do_reading() // Da al sensor la orden de obtener una lectura del campo magnetico
		{
			//System.Windows.Forms.MessageBox.Show("El sensor hace una lectura");

			serialPort1.Write(read_command);



		}


		public void GetPoint()
		{
			Do_reading();

			Point punto = new Point(Lectura(get_x), Lectura(get_y), Lectura(get_z));

			PointDB.Add(punto);
		}


		public void Build_PointDB()
		{



			cols = int.Parse(cols_txtbx.Text);
			rows = int.Parse(rows_txtbx.Text);

			int k = cols * rows;

			if (!serialPort1.IsOpen || cols * rows == 0)
			{
				//Should not reach here

				consoleTxt_box.AppendText("No serial port connected or invalid size values");

				return;


			}

			consoleTxt_box.AppendText("\n" + "Note: The model will be build in a west->East from South to North manner. Press Enter when prompted after placing the sensor in its position");


			//System.Windows.Forms.MessageBox.Show("Process is about to begin");

			for (int i = 0; i < k; i++)
			{
				//System.Windows.Forms.MessageBox.Show("Press Enter to get point: " + i.ToString());

				GetPoint();


			}


			System.Windows.Forms.MessageBox.Show("DataBase created Succesfully");



		}




		// Buttons, panels, VS toolbox funtions in overall



		private void GetPortBtn_Click(object sender, EventArgs e) // Obtiene la lista de puertos COM disponibles actualmente en el Ordenador
		{

			ports = SerialPort.GetPortNames(); //Obtiene la lista de Puertos COM conectados actualmente a la computadora

			GetPortTxtB.ResetText();


			foreach (string port in ports)
			{
				GetPortTxtB.AppendText(port + "\n");
			}

		}

		private void real_deal_btn_Click(object sender, EventArgs e)
		{
			real_deal_panel.BringToFront();
		}

		private void config_btn_Click(object sender, EventArgs e)
		{
			config_panel.BringToFront();
		}

		private void about_btn_Click(object sender, EventArgs e)
		{
			about_panel.BringToFront();
		}

		private void real_deal_panel_Paint(object sender, PaintEventArgs e)
		{

		}

		private void Inicializar_sesion_real_deal_Click(object sender, EventArgs e)
		{
			Inicializar_sesion_real_deal.Enabled = false;

			Build_PointDB();
			consoleTxt_box.AppendText(" " + PointDB.Count.ToString());

			Inicializar_sesion_real_deal.Enabled = true;

		}

		private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{





		}

		private void AcceptCOM_Btn_Click(object sender, EventArgs e) //Obtiene el puerto COM de cuadro de texto e intenta abrir el puerto con la variable dada
		{

			serialPort1.PortName = EnterCOMTxtB.Text;


			try
			{

				serialPort1.Open();
				Status_Port.Text = "Port succesfully opened";

				serialPort1.Encoding = Encoding.GetEncoding("Windows-1252");


			}

			catch (Exception)
			{
				Status_Port.Text = "Invalid COM or already opened";
			}
		}




	}





}
