using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Count4U.Common.UserSettings;
using Count4U.Model.Audit;
using Count4U.Model.Interface;
using Count4U.Model.Main;
using Newtonsoft.Json;

namespace Count4U.Common.Web.Profile
{
	public static class ProfileRESTRepository 
	{
		//		Эти заголовки обязательны во всех запросах
		//Accept: application/json
		//X-Authorization: TuGr5pE0YFTNQG7NXtDCbpHdxD5gdMme8pLtVFiDRdPdVUVUzXpSRzkQ5wwPNEy7

		//Для запросов POST/PATCH нужно добавлять ещё этот заголовок
		//Content-Type: application/json


		//http://api.inv.wp-funnel.com/v1/c4u/customers
		//Accept: application/json
		//Content-Type: application/json
		//X-Authorization: TuGr5pE0YFTNQG7NXtDCbpHdxD5gdMme8pLtVFiDRdPdVUVUzXpSRzkQ5wwPNEy7
		//Host: api.inv.wp-funnel.com
		//		{
		//"name": "Customer1",
		//"code": "code1",
		//"email": "Customer@mail.ru",
		//"description": "Customer description"
		//}	

		//public static string RemoteCreateProfileObject(object domainObject)
		//{
		//	if (domainObject == null) return "DomainObject is null";
		//	if (domainObject is Customer)
		//	{
		//		  Customer customer = domainObject as Customer;
		//		  RemoteCreateProfileCustomer(customer);
		//	}
		//	else if (domainObject is Branch)
		//	{
		//		return "DomainObject is Branch. And not Create Profile";
		//	}
		//	else if (domainObject is Inventor)
		//	{
		//		Inventor inventor = domainObject as Inventor;
		//		RemoteCreateProfileInventor(inventor);
		//	}
		//	return "DomainObject unknown Type";
		//}

		public static string RemoteCreateProfileCustomer(Customer customer, string webServiceLink, IUserSettingsManager userSettingsManager, ref FtpCommandResult ftpCommandResult)
		{
			//FtpCommandResult ftpCommandResult = new FtpCommandResult();
			//ftpCommandResult.Successful = SuccessfulEnum.Successful;
			//error = false;
			if (customer == null)
			{
				ftpCommandResult.Successful = SuccessfulEnum.NotSuccessful;
				//error = true;
				ftpCommandResult.Error = "Customer is null";
				return "Customer is null";
			}
			Encoding encodingUTF8 = Encoding.UTF8; 
			Encoding encoding1255 = Encoding.GetEncoding("windows-1255");
			//string URL = @"http://api.inv.wp-funnel.com/v1/c4u/customers";		  //OLD

			//string webServiceLink = userSettingsManager.WebServiceLinkGet();
			//DefaultWebServiceLink = @"http://api.prod.minv.dimex.co.il/v1/c4u"
			

			//string URL = @"http://api.prod.minv.dimex.co.il/v1/c4u/customers";
			string URL = webServiceLink.TrimEnd('/') + @"/customers";
			//api.inv.wp - funnel.com  заменить на  api.prod.minv.dimex.co.il

			string customerCode = customer.Code;
			string customerName = customer.Name;
			string customerMail = customer.Mail;
			string customerDescription = customer.Description;

			object newCustomer = new { name = customerName, code = customerCode, email = customerMail, description = customerDescription };

			//var retCustomer = new JavaScriptSerializer().Serialize(newCustomer);
			var retCustomer = JsonConvert.SerializeObject(newCustomer);

			string DATA = retCustomer;
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
			request.Method = @"POST";
			request.Accept = @"application/json";
			request.ContentType = @"application/json; charset=utf-8";
			request.Headers.Add("X-Authorization: TuGr5pE0YFTNQG7NXtDCbpHdxD5gdMme8pLtVFiDRdPdVUVUzXpSRzkQ5wwPNEy7");
			UTF8Encoding encoding = new UTF8Encoding();
			byte[] bytes = encoding.GetBytes(DATA);
			request.ContentLength = bytes.Length;


			//Accept: application/json
			//Content-Type: application/json; charset=utf-8
			//X-Authorization: TuGr5pE0YFTNQG7NXtDCbpHdxD5gdMme8pLtVFiDRdPdVUVUzXpSRzkQ5wwPNEy7
			//Host: api.inv.wp-funnel.com  OLD
			//Host: api.inv.wp-funnel.com
		

			try
			{
				using (Stream writeStream = request.GetRequestStream())
				{
					writeStream.Write(bytes, 0, bytes.Length);
				}

				HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse();
	
				using (StreamReader stream = new StreamReader(
					webResponse.GetResponseStream(), encodingUTF8))
			 	{
					string streamRead = stream.ReadToEnd();
					return streamRead;
				}
			}
			catch (Exception e)
			{
				//error = true;
				ftpCommandResult.Successful = SuccessfulEnum.NotSuccessful;
				ftpCommandResult.Error = e.Message;
				Console.Out.WriteLine("-----------------");
				Console.Out.WriteLine(e.Message);
				return e.Message;
			}
		}


		//"code": "test",
		//"branch_code": "test",
		//"description": "test",
		//"create_date": "DD/MM/YYYY"


		public static string RemoteCreateProfileInventor(Customer currentCustomer, Branch currentBranch, Inventor inventor, string webServiceLink, IUserSettingsManager userSettingsManager, ref FtpCommandResult ftpCommandResult)
		{
			if (inventor == null)
			{
				ftpCommandResult.Successful = SuccessfulEnum.NotSuccessful;
				//error = true;
				ftpCommandResult.Error = "Inventor is null";
				return "Inventor is null";
			}
			
			Encoding encodingUTF8 = Encoding.UTF8;
			Encoding encoding1255 = Encoding.GetEncoding("windows-1255");
			//string URL = @"http://api.inv.wp-funnel.com/v1/c4u/customers/:customer-code/inventories";		//OLD
			//string URL = @"http://api.inv.wp-funnel.com/v1/c4u/customers/" + inventor.CustomerCode + @"/inventories";  //OLD

			//string webServiceLink = userSettingsManager.WebServiceLinkGet();
			//DefaultWebServiceLink = @"http://api.prod.minv.dimex.co.il/v1/c4u"

			//bool useToo = userSettingsManager.UseTooGet();

			//string URL = @"http://api.prod.minv.dimex.co.il/v1/c4u/customers/" + inventor.CustomerCode + @"/inventories";
			string URL = webServiceLink.TrimEnd('/') + @"/customers/" + inventor.CustomerCode + @"/inventories"; 
		
			//api.inv.wp - funnel.com  заменить на  api.prod.minv.dimex.co.il

			string inventorDescription = inventor.Description == null ? "" : inventor.Description;
			string inventorCode = inventor.Code;
			string customerCode = inventor.CustomerCode;
			string branchCode = inventor.BranchCode;
			DateTime dt = inventor.InventorDate;
			//TODO
			 //InventorDate  YYYY/MM/DD – InventorDate format : 2018/1/2
			string inventorDate = dt.ToString("yyyy") + @"/" + dt.Month + @"/" + dt.Day;	   	//Inventory date (format YYYY/M/D)
				
			object newInventor = new {  
										code = inventorCode,
									   branch_code = branchCode,
										branch_name = currentBranch.Name,
										description = inventorDescription,
										inventor_date = inventorDate
			};
	
			//var retInventor = new JavaScriptSerializer().Serialize(newInventor);
			var retInventor = JsonConvert.SerializeObject(newInventor);

			string DATA = retInventor;
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);

			request.Method = @"POST";
			request.Accept = @"application/json";
			request.ContentType = @"application/json; charset=utf-8";
			request.Headers.Add("X-Authorization: TuGr5pE0YFTNQG7NXtDCbpHdxD5gdMme8pLtVFiDRdPdVUVUzXpSRzkQ5wwPNEy7");
			UTF8Encoding encoding = new UTF8Encoding();
			byte[] bytes = encoding.GetBytes(DATA);
			request.ContentLength = bytes.Length;

			//Accept: application/json
			//Content-Type: application/json
			//X-Authorization: TuGr5pE0YFTNQG7NXtDCbpHdxD5gdMme8pLtVFiDRdPdVUVUzXpSRzkQ5wwPNEy7
			//Host: api.inv.wp-funnel.com

//Эти заголовки обязательны во всех запросах
//Accept: application/json
//X-Authorization: TuGr5pE0YFTNQG7NXtDCbpHdxD5gdMme8pLtVFiDRdPdVUVUzXpSRzkQ5wwPNEy7
//Для запросов POST/PATCH нужно добавлять ещё этот заголовок
//Content-Type: application/json

//все параметры которые описаны в документации нужно передавать в формате JSON
//Пример:
//Нужно передавать так:
//{
//"name": "Customer name",
//"code": "Customer code",
//"email": "Customer email",
//"description": "Customer description"
//}

			try
			{
				using (Stream writeStream = request.GetRequestStream())
				{
					writeStream.Write(bytes, 0, bytes.Length);
				}

				HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse();

				using (StreamReader stream = new StreamReader(
					webResponse.GetResponseStream(), encodingUTF8))
				{
					string streamRead = stream.ReadToEnd();
					return streamRead;
				}
			}
			catch (Exception e)
			{
				ftpCommandResult.Successful = SuccessfulEnum.NotSuccessful;
				ftpCommandResult.Error = e.Message;
				Console.Out.WriteLine("-----------------");
				Console.Out.WriteLine(e.Message);
				return "Error RemoteCreateProfileInventor : url Request " + Environment.NewLine + URL + Environment.NewLine + e.Message;
			}
		}
	}
}
