using System;
using System.IO;
using Count4U.Model;
using Type = System.Type;

namespace Count4U.Model.ServiceContract
{
	public interface IServiceWpfInfo 
    {
		string ClassName { get; set; }
		string InterfaceName { get; set; }
		Type ServiceObjectType { get; set; }
		Type IServiceObjectType { get; set; }
		bool TryHost { get; set; }
		ServiceDBEnum ServiceDBEnum { get; set; }

    }

	public class ServiceWpfInfo : IServiceWpfInfo
    {
		public string ClassName { get; set; }
		public string InterfaceName { get; set; }
		public Type ServiceObjectType { get; set; }
		public ServiceDBEnum ServiceDBEnum { get; set; } 
		public Type IServiceObjectType { get; set; }
		public bool TryHost { get; set; }

		public ServiceWpfInfo() 
		{
			TryHost = false;
		}

	
    }

	public enum ServiceDBEnum
	{
		MainDB,
		AuditDB,
		Count4UDB,
		ProcessDB,
		AnalyticDB
	}
    
}