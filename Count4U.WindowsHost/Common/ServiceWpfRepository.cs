using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using Count4U.Model;
using Microsoft.Practices.Unity;
using Type = System.Type;

namespace Count4U.Model.ServiceContract
{
	
	public static class ServiceWpfRepository 
    {
		//IPHostEntry ipHostInfo = Dns.GetHostEntry(serverName);
		//IPAddress ipAddress = ipHostInfo.AddressList
		//	.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);

		public static Tuple<string, string> StartTcpHost(ServiceWpfInfo serviceWpfInfo, string serverIP, string serverPort, string serverPortMex)
		{
			string uri = "";
			string uriMex = "";
			try
			{
				// Formulate the uri for this host
				uri = string.Format(
					@"net.tcp://{0}:{1}/{2}/",
					serverIP,
					serverPort,
					serviceWpfInfo.InterfaceName
				);
				//string uri = "net.tcp://192.168.1.44:2742/ICustomerReportWPFRepository/";

				uriMex = string.Format(
				@"net.tcp://{0}:{1}/{2}/mex",
				serverIP,
				serverPortMex,
				serviceWpfInfo.InterfaceName
			);
				//string uriMex = "net.tcp://192.168.1.44:2744/ICustomerReportWPFRepository/mex"
				// Create a new host
				//ServiceHost host = new ServiceHost(typeof(CustomerReportRepositoryService), new Uri(uri));
				ServiceHost host = new ServiceHost(serviceWpfInfo.ServiceObjectType, new Uri(uri));

				// Add the endpoint binding
				//host.AddServiceEndpoint(
				//	typeof(ICustomerReportWPFRepository),
				//	new NetTcpBinding(SecurityMode.Transport)
				//	{
				//		TransferMode = TransferMode.Streamed
				//	},
				//	uri
				//);
				host.AddServiceEndpoint(
				serviceWpfInfo.IServiceObjectType,
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

				//smb.HttpGetEnabled = true;
				//smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
				smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
				host.Description.Behaviors.Add(smb);

				host.AddServiceEndpoint(
					ServiceMetadataBehavior.MexContractName,
					MetadataExchangeBindings.CreateMexTcpBinding(),
					uriMex
					//"net.tcp://192.168.1.44:2744/ICustomerReportWPFRepository/mex"

				);

				// Run the host
				host.Open();
			}
			catch (Exception exc)
			{
				string msg = exc.Message;
				return new Tuple<string, string>(uri, uriMex + "ERROR: " + exc.Message);
			}

			return new Tuple<string, string>(uri, uriMex);
		}

		public static List<ServiceWpfInfo> GetServiceWpfInfoList(IUnityContainer container)
		{

			//var containerAdapters = container.ResolveAll<IServiceWpfInfo>().Where(r => r.ImportDomainEnum == importDomainEnum).ToList();
			var result = container.ResolveAll<ServiceWpfInfo>().Where(x=>x.TryHost == true).ToList();
			//List<IImportModuleInfo> result = containerAdapters.Where(r => adapters.Any(z => z.AdapterCode.ToLower() == r.Name.ToLower())).ToList();

			//if (!result.Any(r => r.IsDefault) && containerAdapters.Any(r => r.IsDefault && r.ImportDomainEnum == importDomainEnum))
			//	result.Add(containerAdapters.First(r => r.IsDefault && r.ImportDomainEnum == importDomainEnum));

			//result = result.OrderBy(r => r.Title).ToList();

		
			return result;
		}
    }

    
}