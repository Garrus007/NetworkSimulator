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

namespace NetworkComponents.Controls
{
	/// <summary>
	/// Абстрактный класс сетевого устройства
	/// </summary>
	public abstract partial class AbstractNetworkDevice : UserControl
	{
		/// <summary>
		/// Колисчество интерфейсов
		/// </summary>
		protected abstract int InterfacesCount { get; }

		//Список подключенных устройств
		//Ключ - номер порта, к которому 
		protected Dictionary<int,AbstractNetworkDevice> connected_devices;

		//IP адерес этого устройств
		protected string[] interface_adresses;

		/// <summary>
		/// Название (для визуализации)
		/// </summary>
		public string Name
		{
			get { return lblName.Text; }
			set { lblName.Text = value; }
		}

		public AbstractNetworkDevice()
		{
			InitializeComponent();
			connected_devices = new Dictionary<int, AbstractNetworkDevice>();

			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < InterfacesCount; i++)
				builder.Append(i).Append(" ");

			lblPorts.Text = builder.ToString();
		}

		/// <summary>
		/// Устанавливает адреса интерфейсов
		/// </summary>
		/// <param name="ips"></param>
		public void SetIP(params string[] ips)
		{
			if (ips.Length != InterfacesCount)
				throw new ArgumentException("Invalid number of IP adresses");

			interface_adresses = ips;
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

			if (connected_devices.ContainsKey(port_number))
				throw new InterfaceAlreadyConnectedException();

			connected_devices.Add(port_number, device);
		}

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
		public abstract void ProcessPackage(Package package, AbstractNetworkDevice sender);

	
		//----------------------------------------------
		//Отправляет пакет
		protected void send(int port, Package package)
		{
			if (!connected_devices.ContainsKey(port))
				throw new ArgumentException("Device to this port not connected");

			Debug.WriteLine(Name + " send package "+package+" to " + connected_devices[port].Name);

			package.AddStage(this);
			PackageManager.AddPackage(package);
			connected_devices[port].ReceivePacakge(package, this);
		}
	}
}
