﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuickGraph;

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

        Graph g = new Graph();

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


			System.Windows.Forms.MessageBox.Show("Funcion de lectura");

			serialPort1.Write(comando_lectura);

			Task.Delay(5).Wait();

			V_no_procesado = serialPort1.ReadLine();

			componente = Convert.ToInt32(V_no_procesado);

			componente = Sin_signo_a_Con_signo(componente);

			final = Convert.ToDouble(componente);

			Task.Delay(5).Wait();

			return final;
		}

		public void Do_reading() // Da al sensor la orden de obtener una lectura del campo magnetico
		{
			System.Windows.Forms.MessageBox.Show("El sensor hace una lectura");

			serialPort1.Write(read_command);

		}


		public Point GetPoint()
		{
			Point punto = new Point(Lectura(get_x), Lectura(get_y), Lectura(get_z));

            return punto;
		}

        public void ToDB(Point punto)
        {
            PointDB.Add(punto);
        }


		public void Build_PointDB()
		{
            char char_counter = 'a';
			try
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


				System.Windows.Forms.MessageBox.Show("Process is about to begin");

				for (int i = 0; i < k; i++)
				{
					System.Windows.Forms.MessageBox.Show("Press Enter to get point: " + i.ToString());

					GetPoint();
                    PointDB[i].ID = (char)(char_counter + i);
					
				}


				System.Windows.Forms.MessageBox.Show("DataBase created Succesfully");

			}
			catch (Exception e)
			{

				consoleTxt_box.AppendText("No serial port connected");

			}
		}


        public char WhereAmI(Point pointo)
        {
            char WeAreHere = 'x';



            foreach (var item in PointDB)
            {
                if (pointo.mag >= item.min || pointo.mag <= item.max)
                {
                    WeAreHere = item.ID;
                }
            }



            return WeAreHere;
        }


        public void Start_Journey()
        {
            char Destination = ' ';
            List<char> shortP;
            char current_pos = WhereAmI(GetPoint());
            while (true)
            {

                if (current_pos == Destination)
                {

                    Arduino.Write("z");
                    break;

                }
                else
                {
                    shortP = g.shortest_path(current_pos, Destination);
                    PathConsole.AppendText(shortP[shortP.Capacity - 1].ToString());
                    Arduino.Write(shortP[shortP.Capacity - 1].ToString());
                }
            }



        }







        



		// Buttons, panels, VS toolbox funtions in overall



		private void GetPortBtn_Click(object sender, EventArgs e) // Obtiene la lista de puertos COM disponibles actualmente en el Ordenador
		{

			ports = SerialPort.GetPortNames(); //Obtiene la lista de Puertos COM conectados actualmente a la computadora

			GetPortTxtB.ResetText();


			foreach (string port in ports)
			{
				GetPortTxtB.AppendText(port + "\n");
                ArduinoCOMBOX.Items.AddRange(ports);
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

		

		private void Inicializar_sesion_real_deal_Click(object sender, EventArgs e)
		{
			Inicializar_sesion_real_deal.Enabled = false;

			Build_PointDB();

			Inicializar_sesion_real_deal.Enabled = true;

		}

		private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{

            
		}

		private void AcceptCOM_Btn_Click(object sender, EventArgs e) //Obtiene el puerto COM de cuadro de texto e intenta abrir el puerto con la variable dada
		{

			serialPort1.PortName = EnterCOMTxtB.Text;

            Arduino.PortName = ArduinoCOMBOX.Text;

			try
			{

				serialPort1.Open();
                Arduino.Open();
				Status_Port.Text = "Ports succesfully opened";




			}

			catch (Exception)
			{
				Status_Port.Text = "Invalid COM or already opened";
			}
		}




	}





}
