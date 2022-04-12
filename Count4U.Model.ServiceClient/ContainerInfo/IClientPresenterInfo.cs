using System;

namespace Count4U.Model.ServiceClient
{
	//Будет для Ptrocess
	public interface IClientPresenterInfo
    {
        string Name { get; set; }
        string Title { get; set; }
        Type ClientType { get; set; }
        bool IsDefault { get; set; }
        string Description { get; set; }
		string Address { get; set; }
		string Binding { get; set; }
		string Contract { get; set; }
		string Tag { get; set; }
    }
}