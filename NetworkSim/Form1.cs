﻿using NetworkComponents;
using NetworkComponents.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetworkSim
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

		private void Form1_Load(object sender, EventArgs e)
		{
			Logger.TextWrited += Logger_TextWrited;
			PackageManager.NewPackage += PackageManager_NewPackage;
			table_packages.CellClick += Table_packages_CellClick;

			server1.SetIP("192.168.1.1/25", "192.168.1.130/25");
			server1.SetProxy(
				@"192.168.1.2;192.168.1.131;pass
				  192.168.1.131;192.168.1.2;pass
				  any;any;deny");

			server1.SetRoute(
				@"192.168.1.128;255.255.255.128;-;1
				  192.168.1.0;255.255.255.128;-;0");

			pc1.SetIP("192.168.1.2/25");
			pc1.SetGateway("192.168.1.1");

			pc2.SetIP("192.168.1.3/25");
			pc2.SetGateway("192.168.1.1");


			pc3.SetIP("192.168.1.131/25");
			pc3.SetGateway("192.168.1.130");

			pc4.SetIP("192.168.1.132/25");
			pc4.SetGateway("192.168.1.130");

			gate1.SetIP("192.168.132/25");

			pc1.DuplexConnect(0, 0, hub1);
			pc2.DuplexConnect(0, 1, hub1);
			hub1.DuplexConnect(2, 0, server1);

			server1.DuplexConnect(1, 2, hub2);
			hub2.DuplexConnect(0, 0, pc3);
			hub2.DuplexConnect(1, 0, pc4);
			hub2.DuplexConnect(3, 0, gate1);


			netDrawer1.Init();
			netDrawer1.UpdateConnections();

			
			//pc1.SendPackage(new NetworkComponents.Package("192.168.1.131"));
		}

	

		//Новый пакет сгенерирован
		private void PackageManager_NewPackage(Package pckg)
		{
			table_packages.Rows.Clear();

			foreach (var package in PackageManager.Pacakges)
			{
				int id = table_packages.Rows.Add();
				table_packages.Rows[id].Cells[0].Value = package.ToString();
				table_packages.Rows[id].Cells[1].Value = package.PackageState;

				if (package.PackageState == Package.State.SENDING)
					table_packages.Rows[id].DefaultCellStyle.BackColor = Color.Yellow;
				else if (package.PackageState == Package.State.RECEIVED)
					table_packages.Rows[id].DefaultCellStyle.BackColor = Color.Green;
				else
					table_packages.Rows[id].DefaultCellStyle.BackColor = Color.Red;
			}
		}

		//Новое сообщение в лог
		private void Logger_TextWrited(string obj)
		{
			txtLog.Text += obj;
		}

		//Удаление всех пакетов из мониторинга
		private void btnReset_Click(object sender, EventArgs e)
		{
			PackageManager.Reset();
			table_packages.Rows.Clear();
			txtLog.Text = "";
			trace_details.Text = "";
		}

		private void Table_packages_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex == -1) return;

			var package = PackageManager.Pacakges[e.RowIndex];

			trace_details.Text = "";
			int i = 1;
			foreach (var stage in package.Trace)
			{
				trace_details.Text += i + ". " + stage + Environment.NewLine; ;
				i++;
			}

			trace_details.Text += Environment.NewLine + "STATUS: " + package.PackageState;
		}
	}
}
