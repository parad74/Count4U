using System;

namespace Count4U.Model.ServiceClient
{
	//Будет для Ptrocess
	public class ClientPresenterInfo : IClientPresenterInfo
    {
        public string Name { get; set; }
        public string Title { get; set; }
		public Type ClientPresenterType { get; set; }
        public bool IsDefault { get; set; }
        public string Description { get; set; }
		public Type ClientType { get; set; }
		public string Address { get; set; }
		public string Binding{ get; set; }
		public string Contract { get; set; }
		public string Tag { get; set; }
	}


}