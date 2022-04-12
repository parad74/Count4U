using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;
using Common.Config;
using Count4U.Model.ProxyClient;
using Count4U.Model.ServiceClient.Common;
using Count4U.Model.ServiceContract;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.UnityExtensions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.Model.ServiceClient
{
	public class ServiceProxyClientModuleInit : IModule
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IUnityContainer _container;
		public static IClientPresenterInfo ClientPresenterInfoCurrent { get; set; }
		public static IRequest2ResponseProxy ProxyClientCurrent { get; set; }
	
		public ServiceProxyClientModuleInit(IUnityContainer container)
		{
			this._container = container;
		}

		public static List<ClientPresenterInfo> InitClientPresenterInfo()
		{
			ConfigCommunication configCommunication = new ConfigCommunication();
			string error = "";
			string proxyTitleDefault = configCommunication.GetProxyIsDefault(out error);

			List<ClientPresenterInfo> list = new List<ClientPresenterInfo>();
			ClientPresenterInfo testClient = new ClientPresenterInfo();
			testClient.Name = ProxyClientName.TestDataProxyClient;
			testClient.Title = ProxyClientTitle.TestDataProxyClient;
			testClient.ClientType = typeof(TestDataBibliothecaProxyClient);
			testClient.Description = "";
			testClient.IsDefault = false;
			testClient.Address = @"localhost";
			testClient.Binding = "";
			testClient.Contract = "FillSampleData";
			list.Add(testClient);
			//if (proxyTitleDefault == BibliothecaProxyClientTitle.TestDataBibliothecaProxyClient)	
			ClientPresenterInfoCurrent = testClient;

	   //	 //---------    Ale Kotarim   --------------------
	   //	 {
	   ////			<endpoint address="http://kfar-saba.libraries.co.il/BuildaGate5library/general/bibliotheca.php"
	   ////binding="basicHttpBinding" bindingConfiguration="selfServiceBorrowReturnBinding"
	   ////contract="AleKotarimBibliothecaClient.selfServiceBorrowReturnPortType"
	   ////name="selfServiceBorrowReturnPort" />

	   //		 string addr = "http://demo2010.libraries.co.il/BuildaGate5library/general/bibliotheca.php";
	   //		 string binding = "BasicHttpBinding";
	   //		 string name = "selfServiceBorrowReturnPort";
	   //		 GetEndpoint(ref addr, ref binding, name);

	   //		 ClientPresenterInfo aleKotarim = new ClientPresenterInfo();
	   //		 aleKotarim.Name = BibliothecaProxyClientName.AleKotarimBibliothecaProxyClient;
	   //		 aleKotarim.Title = BibliothecaProxyClientTitle.AleKotarimBibliothecaProxyClient;
	   //		 aleKotarim.ClientType = typeof(AleKotarimBibliothecaProxyClient);
	   //		 aleKotarim.Description = "";
	   //		 aleKotarim.IsDefault = false;
	   //		 aleKotarim.Address = addr;
	   //		 aleKotarim.Binding = binding;
	   //		 aleKotarim.Contract = "selfServiceBorrowReturnPortType";
	   //		 list.Add(aleKotarim);
	   //		 if (proxyTitleDefault == BibliothecaProxyClientTitle.AleKotarimBibliothecaProxyClient) ClientPresenterInfoCurrent = aleKotarim;
	   //	 }

	   //	 //---------    Idea ------------------------
	   //	 {
	   //		 // <endpoint address="http://192.117.14.205:8180/ws/services/idea_api_service"
	   //		 //binding="basicHttpBinding" bindingConfiguration="idea_apiBinding"
	   //		 //contract="ServiceIdea.idea_api_service" name="idea_api_service" />

	   //		 string addr = "http://192.117.14.205:8180/ws/services/idea_api_service";
	   //		 string binding = "BasicHttpBinding";
	   //	 //	idea_api_serviceClient client = new idea_api_serviceClient();
	   //		 string name = "idea_api_service";
	   //		 GetEndpoint(ref addr, ref binding, name);

	   //		 ClientPresenterInfo idea = new ClientPresenterInfo();
	   //		 idea.Name = BibliothecaProxyClientName.IdeaBibliothecaProxyClient;
	   //		 idea.Title = BibliothecaProxyClientTitle.IdeaBibliothecaProxyClient;
	   //		 idea.ClientType = typeof(IdeaBibliothecaProxyClient);
	   //		 idea.Description = "";
	   //		 idea.IsDefault = true;
	   //		 idea.Address = addr;
	   //		 idea.Binding = binding;
	   //		 idea.Contract = "idea_api_service";
	   //		 list.Add(idea);
	   //		 if (proxyTitleDefault == BibliothecaProxyClientTitle.IdeaBibliothecaProxyClient) ClientPresenterInfoCurrent = idea;

	   //	 }

	   //	 //addr = "http://demo2010.libraries.co.il/BuildaGate5library/general/bibliotheca.php";
	   //	 //binding = "BasicHttpBinding";
	   //	 //name = "selfServiceBorrowReturnPortDemo";
	   //	 //GetEndpoint(ref addr, ref binding, name);

	   //	 //ClientPresenterInfo aleKotarim = new ClientPresenterInfo();
	   //	 //aleKotarim.Name = BibliothecaProxyClientName.AleKotarimBibliothecaProxyClient;
	   //	 //aleKotarim.Title = BibliothecaProxyClientTitle.AleKotarimBibliothecaProxyClient;
	   //	 //aleKotarim.ClientType = typeof(AleKotarimBibliothecaProxyClient);
	   //	 //aleKotarim.Description = "";
	   //	 //aleKotarim.IsDefault = false;
	   //	 //aleKotarim.Address = addr;
	   //	 //aleKotarim.Binding = binding;
	   //	 //aleKotarim.Contract = "selfServiceBorrowReturnPortType";
	   //	 //list.Add(aleKotarim);
	   //	 //if (proxyTitleDefault == BibliothecaProxyClientTitle.AleKotarimBibliothecaProxyClient) ClientPresenterInfoCurrent = aleKotarim;


	   //	 //<endpoint address="net.tcp://localhost:10055/TopsysBibliothecaService"
	   //	 //	binding="netTcpBinding"
	   //	 //	bindingConfiguration="TCPBinding_IDataService"
	   //	 //	contract="TopsysBibliothecaClient.ITopsysBibliotheca"
	   //	 //	name="tcpITopsysBibliotheca"  />

	   //	 // --------- Topsys -------------------------
	   //	 {
	   //		 string addr1 = "net.tcp://localhost:10055/TopsysBibliothecaService";
	   //		 string binding1 = "netTcpBinding";
	   //		 string name1 = "tcpITopsysBibliotheca";
	   //		 GetEndpoint(ref addr1, ref binding1, name1);

	   //		 ClientPresenterInfo topsys = new ClientPresenterInfo();
	   //		 topsys.Name = BibliothecaProxyClientName.TopsysBibliothecaProxyClient;
	   //		 topsys.Title = BibliothecaProxyClientTitle.TopsysBibliothecaProxyClient;
	   //		 topsys.ClientType = typeof(TopsysBibliothecaProxyClient);
	   //		 topsys.Description = "";
	   //		 topsys.IsDefault = false;
	   //		 //topsys.Address = @"net.tcp://localhost:10055/TopsysBibliothecaService";
	   //		 topsys.Address = addr1;
	   //		 topsys.Binding = binding1;
	   //		 topsys.Contract = "TopsysBibliothecaClient.ITopsysBibliotheca";
	   //		 list.Add(topsys);
	   //		 if (proxyTitleDefault == BibliothecaProxyClientTitle.TopsysBibliothecaProxyClient) ClientPresenterInfoCurrent = topsys;
	   //	 }

			return list;

	
		}

		private static void GetEndpoint(ref string addr, ref string binding, string name)
		{
			ClientSection clientSection = (ClientSection)ConfigurationManager.GetSection("system.serviceModel/client");
			for (int i = 0; i < clientSection.Endpoints.Count; i++)
			{
				if (clientSection.Endpoints[i].Name == name)
				{
					addr = clientSection.Endpoints[i].Address.ToString();
					binding = clientSection.Endpoints[i].Binding.ToString();
				}
			}
		}

		

        public void Initialize()
        {
			_logger.Info("Initialize ServiceClientModule");
			try
			{
				ConfigCommunication configCommunication = new ConfigCommunication();
				string error = "";
				string proxyTitleDefault = configCommunication.GetProxyIsDefault(out error);

				IRequest2ResponseProxy testDataBibliothecaProxyClient = new TestDataBibliothecaProxyClient();
				this._container.RegisterInstance(typeof(IRequest2ResponseProxy), ProxyClientName.TestDataProxyClient, testDataBibliothecaProxyClient, new ContainerControlledLifetimeManager());
				ProxyClientCurrent = testDataBibliothecaProxyClient;

				//IRequest2ResponseProxy aleKotarimBibliothecaProxyClient = new AleKotarimBibliothecaProxyClient();
				//this._container.RegisterInstance(typeof(IRequest2ResponseProxy), BibliothecaProxyClientName.AleKotarimBibliothecaProxyClient, aleKotarimBibliothecaProxyClient, new ContainerControlledLifetimeManager());
				//if (proxyTitleDefault == BibliothecaProxyClientTitle.AleKotarimBibliothecaProxyClient) BibliothecaProxyClientCurrent = aleKotarimBibliothecaProxyClient;

				//IRequest2ResponseProxy ideaBibliothecaProxyClient = new IdeaBibliothecaProxyClient();
				//this._container.RegisterInstance(typeof(IRequest2ResponseProxy), BibliothecaProxyClientName.IdeaBibliothecaProxyClient, ideaBibliothecaProxyClient, new ContainerControlledLifetimeManager());
				//if (proxyTitleDefault == BibliothecaProxyClientTitle.IdeaBibliothecaProxyClient) BibliothecaProxyClientCurrent = ideaBibliothecaProxyClient;


				//IRequest2ResponseProxy topsysBibliothecaProxyClient = new TopsysBibliothecaProxyClient();
				//this._container.RegisterInstance(typeof(IRequest2ResponseProxy), BibliothecaProxyClientName.TopsysBibliothecaProxyClient, topsysBibliothecaProxyClient, new ContainerControlledLifetimeManager());
				//if (proxyTitleDefault == BibliothecaProxyClientTitle.TopsysBibliothecaProxyClient) BibliothecaProxyClientCurrent = topsysBibliothecaProxyClient;

				
				this._container.RegisterType<ICommmonServiceClient, CommmonServiceClient>();

				// IISTest не рассматривать пока не появится IIS у заказчика
				//IRequest2ResponseProxy iisTestDataBibliothecaProxyClient = new IISTestDataBibliothecaProxyClient(args);
				//_container.RegisterInstance(typeof(IRequest2ResponseProxy), BibliothecaProxyClientName.IISTestDataBibliothecaProxyClient, iisTestDataBibliothecaProxyClient, new ContainerControlledLifetimeManager());

				List<ClientPresenterInfo> listClientInfo = InitClientPresenterInfo();
				foreach (ClientPresenterInfo clientInfo in listClientInfo)
				{
					this._container.RegisterInstance(typeof(IClientPresenterInfo), clientInfo.Name, clientInfo, new ContainerControlledLifetimeManager());
				}

				//IClientPresenterInfo aleKotarim = new ClientPresenterInfo();
				//aleKotarim.Name = BibliothecaProxyClientName.AleKotarimBibliothecaProxyClient;
				//aleKotarim.Title = BibliothecaProxyClientTitle.AleKotarimBibliothecaProxyClient;
				//aleKotarim.ClientType = typeof(AleKotarimBibliothecaProxyClient);
				//aleKotarim.Description = "";
				//aleKotarim.IsDefault = false;
				//aleKotarim.Address = @"http://demo2010.libraries.co.il/BuildaGate5library/general/bibliotheca.php";
				//aleKotarim.Binding = "BasicHttpBinding";
				//aleKotarim.Contract = "selfServiceBorrowReturnPortType";
				//this._container.RegisterInstance(typeof(IClientPresenterInfo), this._aleKotarim.Name, this._aleKotarim, new ContainerControlledLifetimeManager());


				//	<endpoint address="http://localhost/IdSip2.HostIIS/Request2ResponseService.svc"
				//binding="basicHttpBinding" contract="IdSip2.ServiceContract.IRequest2Response"
				//name="httpIRequest2Object" />

				// IISTest не рассматривать пока не появится IIS у заказчика
				//IClientPresenterInfo iisTestClient = new ClientPresenterInfo();
				//iisTestClient.Name = BibliothecaProxyClientName.IISTestDataBibliothecaProxyClient;
				//iisTestClient.Title = BibliothecaProxyClientTitle.IISTestDataBibliothecaProxyClient;
				//iisTestClient.ClientType = typeof(IISTestDataBibliothecaProxyClient);
				//iisTestClient.Description = "";
				//iisTestClient.IsDefault = false;
				//iisTestClient.Address = @"http://localhost/IdSip2.HostIIS/Request2ResponseService.svc";
				//iisTestClient.Binding = "BasicHttpBinding";
				//iisTestClient.Contract = "IdSip2.ServiceContract.IRequest2Response";
				//_container.RegisterInstance(typeof(IClientPresenterInfo), iisTestClient.Name, iisTestClient, new ContainerControlledLifetimeManager());



				//_container.RegisterInstance(typeof(IClientPresenterInfo), iisTestClient.Name, iisTestClient, new ContainerControlledLifetimeManager());


				_logger.Info("Initialize ServiceClientModule - OK");
			}
			catch (Exception ex)
			{
				_logger.ErrorException("Initialize ServiceClientModule failed", ex);
			}

        }

		public static void InitializeStatic(IUnityContainer _containerStatic)
		{

			//_containerStatic.RegisterType(typeof(ILog), typeof(LogImport), new ContainerControlledLifetimeManager());
			_logger.Info("Initialize ServiceClientModule");

			try
			{
				ConfigCommunication configCommunication = new ConfigCommunication();
				string error = "";
				string proxyTitleDefault = configCommunication.GetProxyIsDefault(out error);
		
				IRequest2ResponseProxy testDataBibliothecaProxyClient = new TestDataBibliothecaProxyClient();
				_containerStatic.RegisterInstance(typeof(IRequest2ResponseProxy), ProxyClientName.TestDataProxyClient, testDataBibliothecaProxyClient, new ContainerControlledLifetimeManager());
				ProxyClientCurrent = testDataBibliothecaProxyClient;

				//IRequest2ResponseProxy aleKotarimBibliothecaProxyClient = new AleKotarimBibliothecaProxyClient();
				//_containerStatic.RegisterInstance(typeof(IRequest2ResponseProxy), BibliothecaProxyClientName.AleKotarimBibliothecaProxyClient, aleKotarimBibliothecaProxyClient, new ContainerControlledLifetimeManager());
				//if (proxyTitleDefault == BibliothecaProxyClientTitle.AleKotarimBibliothecaProxyClient) BibliothecaProxyClientCurrent = aleKotarimBibliothecaProxyClient;

				//IRequest2ResponseProxy ideaBibliothecaProxyClient = new IdeaBibliothecaProxyClient();
				//_containerStatic.RegisterInstance(typeof(IRequest2ResponseProxy), BibliothecaProxyClientName.IdeaBibliothecaProxyClient, ideaBibliothecaProxyClient, new ContainerControlledLifetimeManager());
				//if (proxyTitleDefault == BibliothecaProxyClientTitle.IdeaBibliothecaProxyClient) BibliothecaProxyClientCurrent = ideaBibliothecaProxyClient;

				//IRequest2ResponseProxy topsysBibliothecaProxyClient = new TopsysBibliothecaProxyClient();
				//_containerStatic.RegisterInstance(typeof(IRequest2ResponseProxy), BibliothecaProxyClientName.TopsysBibliothecaProxyClient, topsysBibliothecaProxyClient, new ContainerControlledLifetimeManager());
				//if (proxyTitleDefault == BibliothecaProxyClientTitle.TopsysBibliothecaProxyClient) BibliothecaProxyClientCurrent = topsysBibliothecaProxyClient;



				_containerStatic.RegisterType<ICommmonServiceClient, CommmonServiceClient>();
				// IISTest не рассматривать пока не появится IIS у заказчика
				//IRequest2ResponseProxy iisTestDataBibliothecaProxyClient = new IISTestDataBibliothecaProxyClient(args);
				//_containerStatic.RegisterInstance(typeof(IRequest2ResponseProxy), BibliothecaProxyClientName.IISTestDataBibliothecaProxyClient, iisTestDataBibliothecaProxyClient, new ContainerControlledLifetimeManager());



				//IClientPresenterInfo testClient = new ClientPresenterInfo();
				//testClient.Name = BibliothecaProxyClientName.TestDataBibliothecaProxyClient;
				//testClient.Title = BibliothecaProxyClientTitle.TestDataBibliothecaProxyClient;
				//testClient.ClientType = typeof(TestDataBibliothecaProxyClient);
				//testClient.Description = "";
				//testClient.IsDefault = true;
				//testClient.Address = @"localhost";
				//testClient.Binding = "";
				//testClient.Contract = "FillSampleData";
				//_containerStatic.RegisterInstance(typeof(IClientPresenterInfo), testClient.Name, testClient, new ContainerControlledLifetimeManager());

				List<ClientPresenterInfo> listClientInfo = InitClientPresenterInfo();
				foreach (ClientPresenterInfo clientInfo in listClientInfo)
				{
					_containerStatic.RegisterInstance(typeof(IClientPresenterInfo), clientInfo.Name, clientInfo, new ContainerControlledLifetimeManager());
				}
			

				//IClientPresenterInfo aleKotarim = new ClientPresenterInfo();
				//aleKotarim.Name = BibliothecaProxyClientName.AleKotarimBibliothecaProxyClient;
				//aleKotarim.Title = BibliothecaProxyClientTitle.AleKotarimBibliothecaProxyClient;
				//aleKotarim.ClientType = typeof(AleKotarimBibliothecaProxyClient);
				//aleKotarim.Description = "";
				//aleKotarim.IsDefault = false;
				//aleKotarim.Address = @"http://demo2010.libraries.co.il/BuildaGate5library/general/bibliotheca.php";
				//aleKotarim.Binding = "BasicHttpBinding";
				//aleKotarim.Contract = "selfServiceBorrowReturnPortType";
				//_containerStatic.RegisterInstance(typeof(IClientPresenterInfo), aleKotarim.Name, aleKotarim, new ContainerControlledLifetimeManager());


				//	<endpoint address="http://localhost/IdSip2.HostIIS/Request2ResponseService.svc"
				//binding="basicHttpBinding" contract="IdSip2.ServiceContract.IRequest2Response"
				//name="httpIRequest2Object" />

				// IISTest не рассматривать пока не появится IIS у заказчика
				//IClientPresenterInfo iisTestClient = new ClientPresenterInfo();
				//iisTestClient.Name = BibliothecaProxyClientName.IISTestDataBibliothecaProxyClient;
				//iisTestClient.Title = BibliothecaProxyClientTitle.IISTestDataBibliothecaProxyClient;
				//iisTestClient.ClientType = typeof(IISTestDataBibliothecaProxyClient);
				//iisTestClient.Description = "";
				//iisTestClient.IsDefault = false;
				//iisTestClient.Address = @"http://localhost/IdSip2.HostIIS/Request2ResponseService.svc";
				//iisTestClient.Binding = "BasicHttpBinding";
				//iisTestClient.Contract = "IdSip2.ServiceContract.IRequest2Response";
				//_containerStatic.RegisterInstance(typeof(IClientPresenterInfo), iisTestClient.Name, iisTestClient, new ContainerControlledLifetimeManager());



				//_container.RegisterInstance(typeof(IClientPresenterInfo), iisTestClient.Name, iisTestClient, new ContainerControlledLifetimeManager());


				_logger.Info("Initialize ServiceClientModule - OK");
			}
			catch (Exception ex)
			{
				_logger.ErrorException("Initialize ServiceClientModule failed", ex);
			}

		}
	}
}
