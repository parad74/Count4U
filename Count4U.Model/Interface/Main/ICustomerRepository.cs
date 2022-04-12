using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Main;
using Count4U.Model.SelectionParams;
using Count4U.Model.Audit;

namespace Count4U.Model.Interface.Main
{
    /// <summary>
    /// Интерфейс репозитория для доступа к клиентам - Customer объектам
    /// </summary>
    public interface ICustomerRepository
    {
		string BuildRelativeDbPath(Customer customer);

        Customers GetCustomers(CBIContext contextCBI);

		/// <summary>
		/// Получить список клиентов по параметрам выборки
		/// </summary>
		/// <param name="selectParams"></param>
		/// <returns></returns>
		Customers GetCustomers(SelectParams selectParams);

		/// <summary>
        /// Присвоить текущему списку  - список customers
        /// </summary>
        /// <param name="customers"></param>
        void SetCustomers(Customers customers);

        /// <summary>
        /// Получить клиента по коду
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
		Customer GetCustomer(string Code);

        /// <summary>
        /// Получить список клиентов с детальной информацией 
        /// </summary>
        /// <returns></returns>
		Customers GetCustomersDetails();

		/// <summary>
		/// Получить клиента по коду
		/// </summary>
		/// <param name="customerCode"></param>
		/// <returns></returns>
		Customer GetCustomerByCode(string customerCode);

		List<string> GetCodeList();
	
		/// <summary>
		/// Установить текущего клиента
		/// </summary>
		/// <param name="currentBranch"></param>
		void SetCurrent(Customer currentCustomer, AuditConfig auditConfig);

		/// <summary>
		/// Получить текущего клиента
		/// </summary>
		/// <returns></returns>
		Customer GetCurrent(AuditConfig auditConfig);

		/// <summary>
		/// Получить код текущего клиента
		/// </summary>
		/// <returns></returns>
		string GetCurrentCode(AuditConfig auditConfig);

        /// <summary>
        /// Клонировать клиента с публичной информацией
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
		//Customer Clone(Customer customer);

        /// <summary>
        /// Изменить клиента
        /// </summary>
        /// <param name="customer"></param>
		void Update(Customer customer);
		void UpdateDomainСustomerByInventorConfig(Customers customers);

        /// <summary>
        /// Удалить
        /// </summary>
        /// <param name="customer"></param>
		void Delete(Customer customer);

		void Delete(List<string> customerCodeList);
		void DeleteDomainObjectOnly(List<string> customerCodeList);

        /// <summary>
        /// Удалить
        /// </summary>
        /// <param name="customerCode"></param>
		void Delete(string customerCode);


		void InsertDomainСustomerFromInventorConfig(Customers customers);
		void Insert(Customer customer);

		IConnectionDB Connection { get; set; }
		void RefillInventorConfigs(Customer customer);
		void RefillInventorConfigsAllCustomersInMainDB();
		Dictionary<string, Customer> FillCustomerDictionary();
		
	}
}
