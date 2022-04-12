using System;
using System.Collections.ObjectModel;
using Count4U.Model.Main;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Main;
using Microsoft.Practices.ServiceLocation;
using System.Collections.Generic;

namespace Count4U.Model.Audit
{
	public class AuditConfigs : ObservableCollection<AuditConfig>
	{

		public static AuditConfigs FromEnumerable(System.Collections.Generic.IEnumerable<AuditConfig> List, 
			IServiceLocator serviceLocator)
		{
			AuditConfigs auditConfigs = new AuditConfigs();

			ICustomerRepository customerRepository = serviceLocator.GetInstance<ICustomerRepository>();
			Dictionary<string, Customer> customerDictionary = customerRepository.FillCustomerDictionary();
			IBranchRepository branchRepository = serviceLocator.GetInstance<IBranchRepository>();
			Dictionary<string, Branch> branchDictionary = branchRepository.FillBranchDictionary();
		
			foreach (AuditConfig item in List)
			{
				item.CustomerName = item.CustomerCode;
				item.BranchName = item.BranchCode;
				if (customerDictionary.ContainsKey(item.CustomerCode) == true)
				{
					item.CustomerName = customerDictionary[item.CustomerCode].Name;
				}
				if (customerDictionary.ContainsKey(item.BranchCode) == true)
				{
					item.BranchName = branchDictionary[item.BranchCode].Name;
				}
				auditConfigs.Add(item);

			}
			return auditConfigs;
		}

		public static Customers CustomersFromEnumerable(System.Collections.Generic.IEnumerable<AuditConfig> List,
			IServiceLocator serviceLocator)
		{
			Customers customers = new Customers();
			ICustomerRepository customerRepository = serviceLocator.GetInstance<ICustomerRepository>();
			Dictionary<string, Customer> customerDictionary = customerRepository.FillCustomerDictionary();

			foreach (AuditConfig item in List)
			{
				string name = item.CustomerName;
				if (customerDictionary.ContainsKey(item.CustomerCode) == true)
				{
				name = customerDictionary[item.CustomerCode].Name;
				}
				Customer customer = new Customer { Code = item.CustomerCode,  Name = name};
				//int index = customers.IndexOf(customer);
				//bool test = customers.Contains(customer);
				if (customers.Contains(customer) == false)
				{
					customers.Add(customer);
				}
			}

			return customers;
		}

		public static Branches BranchesFromEnumerable(System.Collections.Generic.IEnumerable<AuditConfig> List,
			IServiceLocator serviceLocator)
		{
			Branches branches = new Branches();
			IBranchRepository branchRepository = serviceLocator.GetInstance<IBranchRepository>();
			Dictionary<string, Branch> branchDictionary = branchRepository.FillBranchDictionary();

			foreach (AuditConfig item in List)
			{
				string name = item.BranchName;
				if (branchDictionary.ContainsKey(item.BranchCode) == true)
				{
					name = branchDictionary[item.BranchCode].Name;
				}

				Branch branch = new Branch { Code = item.BranchCode, Name = name, CustomerCode = item.CustomerCode };
				if (branches.Contains(branch) == false)
				{
					branches.Add(branch);
				}
			}
			return branches;
		}

		public static Inventors InventorsFromEnumerable(System.Collections.Generic.IEnumerable<AuditConfig> List)
		{
			Inventors inventors = new Inventors();
			foreach (AuditConfig item in List)
			{
				inventors.Add(new Inventor
				{
					Code = item.InventorCode,
					Name = item.InventorName,
					InventorDate = item.InventorDate,
					DBPath = item.DBPath,
					Description = item.Description,
					BranchCode = item.BranchCode,
					CustomerCode = item.CustomerCode,
					CreateDate = item.CreateDate	,
					LastUpdatedCatalog = item.CreateDate
				});
			}
			return inventors;
		}
   
		public static AuditConfigs InventorConfigs2AuditConfigs(System.Collections.Generic.IEnumerable<InventorConfig> List)
		{
			AuditConfigs auditConfigs = new AuditConfigs();
			foreach (InventorConfig item in List)
			{
				auditConfigs.Add(new AuditConfig() {
				//ID = item.ID,
				Code = item.Code,
				InventorDate = item.InventorDate,
				InventorCode = item.Code,
				CreateDate = item.CreateDate,
				Description = item.Description,
				CustomerCode = item.CustomerCode,
				CustomerName = item.CustomerName,
				BranchCode = item.BranchCode,
				BranchName = item.BranchName,
				//IsDirty = item.IsDirty,
				//ModifyDate = item.ModifyDate,
				//StatusInventorConfigID = entity.StatusInventorConfigID,
				//StatusInventorConfigCode = item.StatusInventorConfigCode,
				DBPath = item.DBPath
			});
			}
			return auditConfigs;
		}

		public AuditConfig CurrentItem { get; set; }

		public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}
