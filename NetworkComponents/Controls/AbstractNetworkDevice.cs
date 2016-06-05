﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace NetworkComponents.Controls
{
	/// <summary>
	/// Абстрактный класс сетевого устройства
	/// </summary>
	public  partial class AbstractNetworkDevice : UserControl
	{
		public event Action<Point, AbstractNetworkDevice> DeviceMove;


		/// <summary>
		/// Колисчество интерфейсов
		/// </summary>
		public virtual int InterfacesCount { get; }

		//Список подключенных устройств
		//Ключ - номер порта, к которому подключено устройство
		protected Dictionary<int,AbstractNetworkDevice> ConnectedDevices { get; set; }

		/// <summary>
		/// Список подключений (только для чтения)
		/// </summary>
		public ReadOnlyDictionary<int,AbstractNetworkDevice> Connections { get { return new ReadOnlyDictionary<int, AbstractNetworkDevice>(ConnectedDevices); } }

		//IP адерес этого устройств
		public IPAddressWithMask[] InterfaceAdresses { get; private set; }

		/// <summary>
		/// Название (для визуализации)
		/// </summary>
		public new string Name
		{
			get { return lblName.Text; }
			set { lblName.Text = value; }
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////
		/// НАСТРОЙКА
		/////////////////////////////////////////////////////////////////////////////////////////////////

		public AbstractNetworkDevice()
		{
			InitializeComponent();
			ConnectedDevices = new Dictionary<int, AbstractNetworkDevice>();
			
			//Пишем номера портов на форме
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < InterfacesCount; i++)
				builder.Append(i).Append(" ");

			lblPorts.Text = builder.ToString();


			this.MouseDown += AbstractNetworkDevice_MouseDown;
			this.MouseUp += AbstractNetworkDevice_MouseUp;
			this.MouseMove += AbstractNetworkDevice_MouseMove;
			this.MouseLeave += AbstractNetworkDevice_MouseLeave;
		}

		
		/// <summary>
		/// Устанавливает адреса интерфейсов
		/// </summary>
		/// <param name="ips">IP-адрес</param>
		public  void SetIP(params IPAddressWithMask[] ips)
		{
			if (ips.Length != InterfacesCount)
				throw new ArgumentException("Invalid number of IP adresses");

			InterfaceAdresses = ips;
		}

		public virtual void SetIP(params String[] ips)
		{
			if (ips.Length != InterfacesCount)
				throw new ArgumentException("Invalid number of IP adresses");

			InterfaceAdresses = new IPAddressWithMask[InterfacesCount];
			for (int i = 0; i < ips.Length; i++)
				InterfaceAdresses[i] = new IPAddressWithMask(ips[i]);
		}

		/// <summary>
		/// Подключает другое устройство к этому устройству
		/// </summary>
		/// <param name="port_number">Номер порта, к которому подключается</param>
		/// <param name="device">Подключаемое устройство</param>
		/// <returns></returns>
		public void Connect(int port_number, AbstractNetworkDevice device)
		{
			if ((port_number < 0) || (port_number >= InterfacesCount))
				throw new NoSuchInterfaceException(port_number);

			if (ConnectedDevices.ContainsKey(port_number))
				throw new InterfaceAlreadyConnectedException();

			ConnectedDevices.Add(port_number, device);
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////
		/// СЕТЕВОЕ ПОВЕДЕНИЕ
		/////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Подключает другое устройство к этому устройству
		/// </summary>
		/// <param name="port_number">Номер порта, к которму подключается</param>
		/// <param name="other_port_number">Номер порта на другом устройстве, которым оно полкючается</param>
		/// <param name="device">Подключаемое устройство</param>
		public void DuplexConnect(int port_number, int other_port_number, AbstractNetworkDevice device)
		{
			Connect(port_number, device);
			device.Connect(other_port_number, this);
		}

		/// <summary>
		/// "Событие" получения пакета
		/// </summary>
		/// <param name="package">Входящий пакет</param>
		public void ReceivePacakge(Package package, AbstractNetworkDevice sender)
		{
			ProcessPackage(package, sender);
		}

		/// <summary>
		/// Обработка пакета
		/// </summary>
		/// <param name="package"></param>
		public virtual void ProcessPackage(Package package, AbstractNetworkDevice sender) { }

	
		//----------------------------------------------

		//Отправляет пакет
		protected void send(int port, Package package)
		{
			if (!ConnectedDevices.ContainsKey(port))
				throw new ArgumentException("Device to this port not connected");

			PackageManager.AddPackage(package);

			String stage;
			if (InterfaceAdresses!=null)
				stage = TraceToString(Name, InterfaceAdresses[port], port, ConnectedDevices[port].Name, package);
			else
				stage = TraceToString(Name, null, port, ConnectedDevices[port].Name, package);

			Logger.WriteLine(stage);
			package.AddStage(stage);

			//Проверяем, находится ли получатель в той же подсети
			//if (InterfaceAdresses!=null && !InterfaceAdresses[port].IsInSameSubnet(package.EndIP.Peek()))
			//	return; 

			ConnectedDevices[port].ReceivePacakge(package, this);
		}

		//Переводит состояние трассировки в строку
		public static string TraceToString(string sender_name, IPAddressWithMask sender_ip, int sender_port, string receiver_name, Package package)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(sender_name).Append(" (");

			if (sender_ip != null)
				sb.Append(sender_ip);

			sb.Append(":").Append(sender_port).Append(") ")
				.Append("sent packet ")
				.Append(package)
				.Append(" to ")
				.Append(receiver_name);

			return sb.ToString();
		}


		/////////////////////////////////////////////////////////////////////////////////////////////////
		/// ГРАФОН
		/////////////////////////////////////////////////////////////////////////////////////////////////

		//Перетаскивание
		private Point previous_point;
		private bool is_drag = false;

		private void AbstractNetworkDevice_MouseLeave(object sender, EventArgs e)
		{
			is_drag = false;
		}

		private void AbstractNetworkDevice_MouseMove(object sender, MouseEventArgs e)
		{
			if (is_drag)
			{
				int delta_x = e.X - previous_point.X;
				int delta_y = e.Y - previous_point.Y;

				Left += delta_x;
				Top += delta_y;

				if(DeviceMove!=null)
					DeviceMove(new Point(Left,Top), this);
			}
		}

		private void AbstractNetworkDevice_MouseUp(object sender, MouseEventArgs e)
		{
			is_drag = false;
		}

		private void AbstractNetworkDevice_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				previous_point = e.Location;
				is_drag = true;
			}
		}


	}
}
