using Count4U.WPF.Service;
using Count4U.WPF.Service.Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel.Description;

namespace Count4U.WindowsHost
{
    public partial class Form1Old : Form
    { 
        // Ссылка на экзкмпляр ServiceHost.       
        public Form1Old()
        {
            InitializeComponent();
        }

        static void LoadCustomerHost()
        {
            // Ссылка на экзкмпляр ServiceHost.

          //  ServiceHost _customerHost = new ServiceHost(typeof(CustomerRepositoryService));

            //---------------------------------------------------------------------------------------
            // Привязка и ее свойства.
           // BasicHttpBinding binding = new BasicHttpBinding();

            // Время ожидания закрытия соединения.
            //binding.CloseTimeout = TimeSpan.MaxValue;

            //// Способ сравнения имен узлов при разборе URI.
            //binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;

            // Размер пула буферов для транспортного протокола.
			//binding.MaxBufferPoolSize = 524888;

			//// Размер одного входящего сообщения.
			//binding.MaxReceivedMessageSize = 65536;

			//// Имя привязки.
			////binding.Name = "MyBinding";

			//// Время ожидания завершения операции открытия соединения.
			//binding.OpenTimeout = TimeSpan.MaxValue;

			//// Время ожидания завершения операции приема.
			//binding.ReceiveTimeout = TimeSpan.MaxValue;
																//	 ------------

			// Указание, где ожидать входящие сообщения.
			Uri address = new Uri("http://192.168.1.35:4000/ICustomerReportWPFRepository");

			// Указание, как обмениваться сообщениями.
			BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);

			// Создание экзкмпляра ServiceHost.
			ServiceHost svc = new ServiceHost(typeof(CustomerReportRepositoryService));

			// Добавление "Конечной Точки" сервиса.
			svc.AddServiceEndpoint(typeof(ICustomerReportWPFRepository), binding, address);


			//--------------------------------------------------------------------------
			// Создание экземпляра ServiceMetadataBehavior.
			ServiceMetadataBehavior metadata = new ServiceMetadataBehavior();
			//NetTcpBinding _netTcpBinding = new NetTcpBinding();
			//_netTcpBinding.Security.Mode = SecurityMode.None;

			// Добаление metadata в ServiceHost.
			svc.Description.Behaviors.Add(metadata);
			
			// Создание адреса для ожидания трафика WS-MetadataExchange.
			Uri mexAddress = new Uri("http://192.168.1.35:4001/ICustomerReportWPFRepository/Mex");
			//_binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
						
			// Создание компоновки метаданных ТСР.
			//System.ServiceModel.Channels.Binding mexBinding = MetadataExchangeBindings.CreateMexTcpBinding();
			System.ServiceModel.Channels.Binding mexBinding = MetadataExchangeBindings.CreateMexHttpBinding();
			
			// Добавление "Конечной Точки" метаданных.
			svc.AddServiceEndpoint(typeof(ICustomerReportWPFRepository), mexBinding, mexAddress);
			//--------------------------------------------------------------------------
            //---------------------------------------------------------------------------------------
			//try
			//{
			//	_customerHost.AddServiceEndpoint(typeof(ICustomerReportWPFRepository), binding, "http://192.168.1.35/ICustomerReportWPFRepository");
			//	_customerHost.Open();
			//}
			//catch (Exception exc)
			//{
			//	string msg = exc.Message;
			//	_customerHost.Close();
			//}
      
        }


		public static void StartCustomerReportHttpHost()
		{
			try
			{
				// Formulate the uri for this host
				//string uri = string.Format(
				//	"net.tcp://{0}:{1}/ServerTasks",
				//	Dns.GetHostName(),
				//	ServerSettings.Instance.TCPListeningPort
				//);
				//string uri = "net.tcp://192.168.1.44:2742/ICustomerReportWPFRepository/";
				string uri = "http://192.168.1.44:2742/ICustomerReportWPFRepository/";

				// Create a new host
				ServiceHost host = new ServiceHost(typeof(CustomerReportRepositoryService), new Uri(uri));

				// Add the endpoint binding
				host.AddServiceEndpoint(
					typeof(ICustomerReportWPFRepository),
					//new NetTcpBinding(SecurityMode.Transport)
					//{
					//	TransferMode = TransferMode.Streamed
					//},
					new BasicHttpBinding(BasicHttpSecurityMode.None)
					{
						TransferMode = TransferMode.Streamed
					},
					uri
				);

				// Add the meta data publishing
				var smb = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
				if (smb == null)
					smb = new ServiceMetadataBehavior();

				//smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
				smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
				host.Description.Behaviors.Add(smb);

				host.AddServiceEndpoint(
					ServiceMetadataBehavior.MexContractName,
					//MetadataExchangeBindings.CreateMexTcpBinding(),
					//"net.tcp://192.168.1.35:4500/ICustomerReportWPFRepository/mex"
					MetadataExchangeBindings.CreateMexHttpBinding(),
					"http://192.168.1.44:2744/ICustomerReportWPFRepository/mex"
				);

				// Run the host
				host.Open();
			}
			catch (Exception exc)
			{
				string msg = exc.Message;
			}
		}
		

		//public static void StartCustomerHttpHost()
		//{
		//	try
		//	{
		//		// Formulate the uri for this host
		//		//string uri = string.Format(
		//		//	"net.tcp://{0}:{1}/ServerTasks",
		//		//	Dns.GetHostName(),
		//		//	ServerSettings.Instance.TCPListeningPort
		//		//);
		//		//string uri = "net.tcp://192.168.1.35:4000/ICustomerWPFRepository";
		//		string uri = "http://192.168.1.44:2742/ICustomerWPFRepository/";

		//		// Create a new host
		//		ServiceHost host = new ServiceHost(typeof(CustomerRepositoryService), new Uri(uri));

		//		// Add the endpoint binding
		//		host.AddServiceEndpoint(
		//			typeof(ICustomerWPFRepository),
		//			//new NetTcpBinding(SecurityMode.Transport)
		//			//{
		//			//	TransferMode = TransferMode.Streamed
		//			//},
		//			new BasicHttpBinding(BasicHttpSecurityMode.None)
		//			{
		//				TransferMode = TransferMode.Streamed
		//			},
		//			uri
		//		);

		//		// Add the meta data publishing
		//		var smb = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
		//		if (smb == null)
		//			smb = new ServiceMetadataBehavior();

		//		smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
		//		host.Description.Behaviors.Add(smb);

		//		host.AddServiceEndpoint(
		//			ServiceMetadataBehavior.MexContractName,
		//			MetadataExchangeBindings.CreateMexHttpBinding(),
		//			"http://192.168.1.44:2744/ICustomerWPFRepository/mex"
		//		//MetadataExchangeBindings.CreateMexTcpBinding(),
		//		////"net.tcp://192.168.1.35:4500/ICustomerWPFRepository/mex"
		//		//
		//		);

		//		// Run the host
		//		host.Open();
		//	}
		//	catch (Exception exc)
		//	{
		//		string msg = exc.Message;
		//	}
		//}

		//================ TCP
		public static void StartCustomerReportTcpHost()
		{
			try
			{
				// Formulate the uri for this host
				//string uri = string.Format(
				//	"net.tcp://{0}:{1}/ServerTasks",
				//	Dns.GetHostName(),
				//	ServerSettings.Instance.TCPListeningPort
				//);
				string uri = "net.tcp://192.168.1.44:2742/ICustomerReportWPFRepository/";
	
				// Create a new host
				ServiceHost host = new ServiceHost(typeof(CustomerReportRepositoryService), new Uri(uri));

				// Add the endpoint binding
				host.AddServiceEndpoint(
					typeof(ICustomerReportWPFRepository),
					new NetTcpBinding(SecurityMode.Transport)
					{
						TransferMode = TransferMode.Streamed
					},
					uri
				);

				// Add the meta data publishing
				var smb = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
				if (smb == null)
					smb = new ServiceMetadataBehavior();

				//smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
				smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
				host.Description.Behaviors.Add(smb);

				host.AddServiceEndpoint(
					ServiceMetadataBehavior.MexContractName,
					MetadataExchangeBindings.CreateMexTcpBinding(),
					"net.tcp://192.168.1.44:2744/ICustomerReportWPFRepository/mex"

				);

				// Run the host
				host.Open();
			}
			catch (Exception exc)
			{
				string msg = exc.Message;
			}
		}


		//public static void StartCustomerTcpHost()
		//{
		//	try
		//	{
		//		// Formulate the uri for this host
		//		//string uri = string.Format(
		//		//	"net.tcp://{0}:{1}/ServerTasks",
		//		//	Dns.GetHostName(),
		//		//	ServerSettings.Instance.TCPListeningPort
		//		//);
		//		string uri = "net.tcp://192.168.1.44:2742/ICustomerWPFRepository";

		//		// Create a new host
		//		ServiceHost host = new ServiceHost(typeof(CustomerRepositoryService), new Uri(uri));

		//		// Add the endpoint binding
		//		host.AddServiceEndpoint(
		//			typeof(ICustomerWPFRepository),
		//			new NetTcpBinding(SecurityMode.Transport)
		//			{
		//				TransferMode = TransferMode.Streamed
		//			},
		//			uri
		//		);

		//		// Add the meta data publishing
		//		var smb = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
		//		if (smb == null)
		//			smb = new ServiceMetadataBehavior();

		//		smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
		//		host.Description.Behaviors.Add(smb);

		//		host.AddServiceEndpoint(
		//			ServiceMetadataBehavior.MexContractName,
		//			MetadataExchangeBindings.CreateMexTcpBinding(),
		//			//"net.tcp://192.168.1.35:2744/ICustomerWPFRepository/mex"
		//			"net.tcp://192.168.1.44:2744/ICustomerWPFRepository/mex"
		//		);

		//		// Run the host
		//		host.Open();
		//	}
		//	catch (Exception exc)
		//	{
		//		string msg = exc.Message;
		//	}
		//}


        private void Form1_Load(object sender, EventArgs e)
        {
			// TODO: This line of code loads data into the 'mainDBDataSet.CustomerReport' table. You can move, or remove it, as needed.
			//this.customerReportTableAdapter.Fill(this.mainDBDataSet.CustomerReport);
			// TODO: This line of code loads data into the 'mainDBDataSet1.CustomerReport' table. You can move, or remove it, as needed.
			//this.customerReportTableAdapter.Fill(this.mainDBDataSet1.CustomerReport);
			//// TODO: This line of code loads data into the 'mainDBDataSet.CustomerReport' table. You can move, or remove it, as needed.
			//this.customerReportTableAdapter.Fill(this.mainDBDataSet.CustomerReport);
			//// TODO: This line of code loads data into the 'mainDBDataSet.CustomerReport' table. You can move, or remove it, as needed.
			//this.customerReportTableAdapter.Fill(this.mainDBDataSet.CustomerReport);
			// TODO: This line of code loads data into the 'mainDBDataSet.CustomerReport' table. You can move, or remove it, as needed.
			this.customerReportTableAdapter.Fill(this.mainDBDataSet.CustomerReport);


			string protocol = "Tcp";

			if (protocol == "Http")
			{
				//StartCustomerHttpHost();
				StartCustomerReportHttpHost();
			}else 
				if (protocol == "Tcp")
			{
				//StartCustomerTcpHost();
				StartCustomerReportTcpHost();
			}
        }

		private void bindingSource1_CurrentChanged(object sender, EventArgs e)
		{

		}

		private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{

		}

		private void button1_Click(object sender, EventArgs e)
		{
			customerReportTableAdapter.Fill(this.mainDBDataSet.CustomerReport);
			//this.dataGridView1.DataSource = this.mainDBDataSet.Tables["CustomerReport"];

		}

		private void bindingSource1_CurrentChanged_1(object sender, EventArgs e)
		{

		}

		private void bindingSource1_CurrentChanged_2(object sender, EventArgs e)
		{

		}

		private void customerReportBindingSource_CurrentChanged(object sender, EventArgs e)
		{

		}

		private void customerReportBindingSource1_CurrentChanged(object sender, EventArgs e)
		{

		}

		private void bindingSource1_CurrentChanged_3(object sender, EventArgs e)
		{

		}

		private void bindingSource2_CurrentChanged(object sender, EventArgs e)
		{

		}

        // this._logMessageHost = new ServiceHost(typeof(LogMessageService));
    }



}
