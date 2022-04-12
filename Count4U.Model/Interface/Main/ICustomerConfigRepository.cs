using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Main;
using Count4U.Model.SelectionParams;

namespace Count4U.Model.Interface.Main
{
	/// <summary>
	/// Интерфейс репозитория для доступа к CustomerConfig объектам
	/// </summary>
	public interface ICustomerConfigRepository
	{
		CustomerConfigs GetCustomerConfigs();
		CustomerConfigs GetCustomerConfigs(SelectParams selectParams, string pathDB);
		CustomerConfigs GetCustomerConfigsByCode(string сustomerСode);
		CustomerConfig GetCustomerConfig(string сustomerСode, string name);
		void Insert(CustomerConfig customerConfig);
		void Delete(string сustomerСode, string name);
		void Delete(string сustomerСode);
		void Update(CustomerConfig customerConfig);

		Dictionary<string, CustomerConfig> GetCustomerConfigIniDictionary(string customerCode);
		void InsertCustomerConfigIniDictionary(string customerCode, Dictionary<string, CustomerConfig> customerConfigIniDictionary);
		void UpdateCustomerConfigIniDictionary(Dictionary<string, CustomerConfig> customerConfigIniDictionary);
		Dictionary<CustomerConfigIniEnum, string> GetCustomerConfigDefaultValueDictionary();

	}
}
