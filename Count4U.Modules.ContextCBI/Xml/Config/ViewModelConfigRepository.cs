using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Count4U.Common.Constants;
using Count4U.Common.Enums;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Common.ViewModel.ExportPda;
using Count4U.Model.Extensions;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Modules.ContextCBI.Xml.Config
{
	public static class ViewModelConfigRepository
	{

		public static XDocument UpdatePath(XDocument doc, string adapterType, DomainObjectType fromDomainObjectType, HowUse pathHowUse,
			string absolutePath = @"c:\temp", bool _isDefault = true)
		{
			if (doc == null) return new XDocument();
			doc = RemoveAllElement(doc, "FROMPATH");
			XElement rootPath = new XElement("FROMPATH");
			rootPath.Add(new XAttribute("domainobject", fromDomainObjectType));


			if (adapterType == "ImportWithModulesBaseViewModel")
			{
				rootPath.Add(new XAttribute("adapteruse", AdapterUse.import.ToString()));
			}
			else if (adapterType == "ExportPdaWithModulesViewModel")
			{
				rootPath.Add(new XAttribute("adapteruse", AdapterUse.exportpda.ToString()));
			}
			else if (adapterType == "ExportErpWithModulesViewModel")
			{
				rootPath.Add(new XAttribute("adapteruse", AdapterUse.exporterp.ToString()));
			}

			if (pathHowUse == HowUse.relative)
			{
				rootPath.Add(new XAttribute("howuse", HowUse.relative.ToString()));
				rootPath.Add(new XAttribute("from", From.indata.ToString()));
				rootPath.Add(new XAttribute("value", ""));
			}
			else if (pathHowUse == HowUse.asis)
			{
				rootPath.Add(new XAttribute("howuse", HowUse.asis.ToString()));
				rootPath.Add(new XAttribute("from", From.absolute.ToString()));
				rootPath.Add(new XAttribute("value", absolutePath));
			}
			else if (pathHowUse == HowUse.ftp)
			{
				rootPath.Add(new XAttribute("from", From.ftp.ToString()));
			}

			string isdefault = "0";
			if (_isDefault == true) isdefault = "1";
			rootPath.Add(new XAttribute("isdefault", isdefault));

			doc.Root.AddFirst(rootPath);
			return doc;
		}

		public static XElement GetXElementExportPdaAdapterProperty(object viewModel,
					string dataContextViewModelName,
					IExportPdaModuleInfo selectedAdapter,
					IServiceLocator serviceLocator,
					 ExportPdaSettingsControlViewModel exportPdaSettingsControlViewModel,
					ExportPdaProgramTypeViewModel exportPdaProgramTypeControlViewModel,
					ExportPdaMerkavaAdapterViewModel exportPdaMerkavaAdapterControlViewModel )
		{
			//IImportModuleInfo selectedAdapter = SelectedAdapter;

				XElement root = new XElement("ROOT");

				if (selectedAdapter == null)
				{
					XElement adapterInfo = new XElement("INFO");
					adapterInfo.Add(new XAttribute("Name", "Adapter not selected"));
					root.Add(adapterInfo);
					return root;
				}

			Type viewUserControl = selectedAdapter.UserControlType;
			//var i = viewUserControl.DataContext as ImportModuleBaseViewModel;
	
			//_configXML = _configXML + "<TEST>" + selectedAdapter.UserControlType.Name + "</TEST>" + Environment.NewLine;
			//ExportPdaModuleBaseViewModel baseViewModel = serviceLocator.GetInstance<ExportPdaModuleBaseViewModel>(dataContextViewModelName);
			ExportPdaModuleBaseViewModel baseViewModel = serviceLocator.GetInstance<ExportPdaModuleBaseViewModel>(selectedAdapter.Name);

			Type TestType = Type.GetType(dataContextViewModelName, false, false);

			//если класс не найден
			if (TestType != null)
			{
				//получаем конструктор
				System.Reflection.ConstructorInfo ci = TestType.GetConstructor(new Type[] { });

				//вызываем конструтор
				object Obj = ci.Invoke(new object[] { });
			}
		
			XElement adapter = new XElement("INFO");
			adapter.Add(new XAttribute("Name", selectedAdapter.Name));
			adapter.Add(new XAttribute("Title", selectedAdapter.Title));
			adapter.Add(new XAttribute("UserControlType", selectedAdapter.UserControlType.Name));
			adapter.Add(new XAttribute("DataContextViewModel", dataContextViewModelName));
			adapter.Add(new XAttribute("ImportDomainEnum", selectedAdapter.ImportDomainEnum.ToString()));
			string description = 	selectedAdapter.Description != null ? selectedAdapter.Description : "";
			adapter.Add(new XAttribute("Description", description));
			root.Add(adapter);

			// =================== viewModel ===========================
			AddPropertyXElementFromViewModel(viewModel, adapter);
			if (selectedAdapter.Name == ExportPdaAdapterName.ExportPdaMISAdapter
				|| selectedAdapter.Name == ExportPdaAdapterName.ExportHT630Adapter)
			{
				AddPropertyXElementFromViewModel(exportPdaProgramTypeControlViewModel, adapter);
				AddPropertyXElementFromViewModel(exportPdaSettingsControlViewModel, adapter);
			}
			if (selectedAdapter.Name == ExportPdaAdapterName.ExportPdaClalitSQLiteAdapter
			|| selectedAdapter.Name == ExportPdaAdapterName.ExportPdaMerkavaSQLiteAdapter
			||	selectedAdapter.Name == ExportPdaAdapterName.ExportPdaNativPlusMISSQLiteAdapter
			|| selectedAdapter.Name == ExportPdaAdapterName.ExportPdaNativPlusSQLiteAdapter
			|| selectedAdapter.Name == ExportPdaAdapterName.ExportPdaNativSqliteAdapter)
			{
				AddPropertyXElementFromViewModel(exportPdaMerkavaAdapterControlViewModel, adapter);
			}
			return root;
		}

		private static void AddPropertyXElementFromViewModel(object viewModel, XElement adapter)
		{
			if (viewModel != null)
			{
				PropertyInfo[] props = viewModel.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

				for (int i = 0; i < props.Length; i++) //responseData
				{
					if (props[i] == null) continue;
					try
					{
						string propTypeName = props[i].PropertyType.Name;
						string propName = props[i].Name;
						if (propName == "EncodingCodePage")
						{
							var propValue = props[i].GetValue(viewModel, null);
							if (propValue != null)
							{
								XElement propAdapter = new XElement("PROPERTY");
								propAdapter.Add(new XAttribute("returntype", "int32"));
								propAdapter.Add(new XAttribute("name", "Encoding"));
								propAdapter.Add(new XAttribute("value", propValue.ToString()));
								adapter.Add(propAdapter);
							}
							continue;
						}
						if (propTypeName.ToLower() == "boolean"
						|| propTypeName.ToLower() == "string"
						|| propTypeName.ToLower() == "int16"
						|| propTypeName.ToLower() == "int32"
						 || propTypeName.ToLower() == "int64")
						{
							if (propName.Contains("Execute") == true) continue;
							if (propName.Contains("Error") == true) continue;
							if (propName.Contains("Command") == true) continue;
							var attr = props[i].GetCustomAttributes(typeof(NotInludeAttribute), true).FirstOrDefault() as NotInludeAttribute;
							if (attr != null) continue;

							var propValue = props[i].GetValue(viewModel, null);
							if (propValue != null)
							{
								XElement propAdapter = new XElement("PROPERTY");
								propAdapter.Add(new XAttribute("returntype", propTypeName));
								propAdapter.Add(new XAttribute("name", props[i].Name));
								propAdapter.Add(new XAttribute("value", propValue.ToString()));
								adapter.Add(propAdapter);
							}
						}
					}
					catch (Exception ex)
					{
					}
				}
			}
		}

		public static XElement GetXElementExportERPAdapterProperty(
			object viewModel,
			string dataContextViewModelName,
			IExportErpModuleInfo selectedAdapter, ExportErpCommandInfo info)
		{
	
			XElement root = new XElement("ROOT");
			if (selectedAdapter == null)
			{
				XElement adapterInfo = new XElement("INFO");
				adapterInfo.Add(new XAttribute("Name", "Adapter not selected"));
				root.Add(adapterInfo);
				return root; 				
			}

			//IImportModuleInfo selectedAdapter = SelectedAdapter;
			Type vm = selectedAdapter.UserControlType;
			//_configXML = _configXML + "<TEST>" + selectedAdapter.UserControlType.Name + "</TEST>" + Environment.NewLine;

			XElement adapter = new XElement("INFO");
			adapter.Add(new XAttribute("Name", selectedAdapter.Name));
			adapter.Add(new XAttribute("Title", selectedAdapter.Title));
			adapter.Add(new XAttribute("UserControlType", selectedAdapter.UserControlType.Name));
			adapter.Add(new XAttribute("DataContextViewModel", dataContextViewModelName));
			adapter.Add(new XAttribute("ImportDomainEnum","ExportErpAdapter"));
			string description = 	selectedAdapter.Description != null ? selectedAdapter.Description : "";
			adapter.Add(new XAttribute("Description", description));
			root.Add(adapter);


			if (viewModel != null)
			{
				PropertyInfo[] props = viewModel.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

				for (int i = 0; i < props.Length; i++) //responseData
				{
					if (props[i] == null) continue;
					//if (props[i].Name.ToLower() 
					//string parceData = sip2Protocol.GetValueFromStringByFormat(entity, keyValuePair.Value.xElementFormat);
					try
					{
						string propTypeName = props[i].PropertyType.Name;
						string propName = props[i].Name;

						if (propTypeName.ToLower() == "boolean"
						|| propTypeName.ToLower() == "string"
						|| propTypeName.ToLower() == "int16"
						|| propTypeName.ToLower() == "int32"
						 || propTypeName.ToLower() == "int64")
						{
							if (propName.Contains("Execute") == true) continue;
							if (propName.Contains("Error") == true) continue;
							if (propName.Contains("Command") == true) continue;
							var attr = props[i].GetCustomAttributes(typeof(NotInludeAttribute), true).FirstOrDefault() as NotInludeAttribute;
							//bool notBulk = props[i].Attributes.OfType<NotInludeAttribute>().Any();
							if (attr != null) continue;

							var propValue = props[i].GetValue(viewModel, null);
							if (propValue != null)
							{
								XElement propAdapter = new XElement("PROPERTY");
								propAdapter.Add(new XAttribute("returntype", propTypeName));
								propAdapter.Add(new XAttribute("name", props[i].Name));
								propAdapter.Add(new XAttribute("value", propValue.ToString()));
								adapter.Add(propAdapter);

								//	string ret = propTypeName + " " + props[i].Name + " = " + propValue.ToString();
								//_configXML = _configXML + ret + Environment.NewLine;
							}

						}
					}
					catch (Exception ex)
					{
						//_configXML = _configXML + " Error : " + ex.Message + Environment.NewLine;
					}
				}
			}

			// =======================  info =======
			if (info != null)
			{
				PropertyInfo[] props = info.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

				for (int i = 0; i < props.Length; i++) //responseData
				{
					if (props[i] == null) continue;
					//if (props[i].Name.ToLower() 
					//string parceData = sip2Protocol.GetValueFromStringByFormat(entity, keyValuePair.Value.xElementFormat);
					try
					{
						string propTypeName = props[i].PropertyType.Name;
						string propName = props[i].Name;

						if (propTypeName.ToLower() == "boolean"
						|| propTypeName.ToLower() == "string"
						|| propTypeName.ToLower() == "int16"
						|| propTypeName.ToLower() == "int32"
						 || propTypeName.ToLower() == "int64")
						{
							if (propName.Contains("Execute") == true) continue;
							if (propName.Contains("Error") == true) continue;
							if (propName.Contains("Command") == true) continue;
							var attr = props[i].GetCustomAttributes(typeof(NotInludeAttribute), true).FirstOrDefault() as NotInludeAttribute;
							//bool notBulk = props[i].Attributes.OfType<NotInludeAttribute>().Any();
							if (attr != null) continue;

							var propValue = props[i].GetValue(info, null);
							if (propValue != null)
							{
								XElement propAdapter = new XElement("PROPERTY");
								propAdapter.Add(new XAttribute("returntype", propTypeName));
								propAdapter.Add(new XAttribute("name", props[i].Name));
								propAdapter.Add(new XAttribute("value", propValue.ToString()));
								adapter.Add(propAdapter);

								//	string ret = propTypeName + " " + props[i].Name + " = " + propValue.ToString();
								//_configXML = _configXML + ret + Environment.NewLine;
							}

						}
					}
					catch (Exception ex)
					{
						//_configXML = _configXML + " Error : " + ex.Message + Environment.NewLine;
					}
				}
			}

			return root;
		}


		public static XDocument VerifyToAdapter(XDocument configXMLdoc, IBaseImportExportModuleInfo selectedAdapter)		
		{
			if (selectedAdapter == null) return configXMLdoc;

			XElement adapter = configXMLdoc.Descendants("INFO").FirstOrDefault();
			//Есть проблема с определением ViewModelName
			UpdateXElementAdapter(ref adapter, selectedAdapter);
			return configXMLdoc;
		}

		//public static XDocument VerifyToAdapter(XDocument configXMLdoc, ImportModuleInfo selectedAdapter)		
		//{
		//	if (selectedAdapter == null) return configXMLdoc;

		//	XElement adapter = configXMLdoc.Descendants( "INFO").FirstOrDefault();

		//	UpdateXElementAdapter(ref adapter, selectedAdapter);
		//	return configXMLdoc;
		//}

		public static XDocument RemoveAllElement(XDocument doc, string tag)
		{
			IEnumerable<XElement> tracks = doc.Descendants(tag);
			if (tracks != null) tracks.Remove();
			return doc;
		}

		public static XElement GetXElementImportAdapterProperty(object viewModel,
			string dataContextViewModelName,
			ImportModuleInfo selectedAdapter)
		{
		
			//IImportModuleInfo selectedAdapter = SelectedAdapter;
		//	Type vm = selectedAdapter.UserControlType;
			//_configXML = _configXML + "<TEST>" + selectedAdapter.UserControlType.Name + "</TEST>" + Environment.NewLine;

			XElement root = new XElement("ROOT");

			if (selectedAdapter == null)
			{
				XElement adapterInfo = new XElement("INFO");
				adapterInfo.Add(new XAttribute("Name", "Adapter not selected"));
				root.Add(adapterInfo);
				return root;
			}

			XElement adapter = CreateXElementAdapter(dataContextViewModelName, selectedAdapter);
			root.Add(adapter);

		//	if (vm != null)
			if (viewModel != null)
			{
				PropertyInfo[] props = viewModel.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

				for (int i = 0; i < props.Length; i++) //responseData
				{
					if (props[i] == null) continue;
					//if (props[i].Name.ToLower() 
					//string parceData = sip2Protocol.GetValueFromStringByFormat(entity, keyValuePair.Value.xElementFormat);
					try
					{
						string propTypeName = props[i].PropertyType.Name;
						string propName = props[i].Name;

						if (propTypeName.ToLower() == "boolean"
						|| propTypeName.ToLower() == "string"
						|| propTypeName.ToLower() == "int16"
						|| propTypeName.ToLower() == "int32"
						 || propTypeName.ToLower() == "int64")
						{
							if (propName.Contains("Execute") == true) continue;
							if (propName.Contains("Error") == true) continue;
							if (propName.Contains("Command") == true) continue;
							var attr = props[i].GetCustomAttributes(typeof(NotInludeAttribute), true).FirstOrDefault() as NotInludeAttribute;
							//bool notBulk = props[i].Attributes.OfType<NotInludeAttribute>().Any();
							if (attr != null) continue;

							var propValue = props[i].GetValue(viewModel, null);
							if (propValue != null)
							{
								XElement propAdapter = new XElement("PROPERTY");
								propAdapter.Add(new XAttribute("returntype", propTypeName));
								propAdapter.Add(new XAttribute("name", props[i].Name));
								propAdapter.Add(new XAttribute("value", propValue.ToString()));
								adapter.Add(propAdapter);

								//	string ret = propTypeName + " " + props[i].Name + " = " + propValue.ToString();
								//_configXML = _configXML + ret + Environment.NewLine;
							}

						}
					}
					catch (Exception ex)
					{
						//_configXML = _configXML + " Error : " + ex.Message + Environment.NewLine;
					}
				}
			}
			return root;
		}

		private static XElement CreateXElementAdapter(string dataContextViewModelName, ImportModuleInfo selectedAdapter)
		{
			XElement adapter = new XElement("INFO");
			adapter.Add(new XAttribute("Name", selectedAdapter.Name));
			adapter.Add(new XAttribute("Title", selectedAdapter.Title));
			adapter.Add(new XAttribute("UserControlType", selectedAdapter.UserControlType.Name));
			adapter.Add(new XAttribute("DataContextViewModel", dataContextViewModelName));
			adapter.Add(new XAttribute("ImportDomainEnum", selectedAdapter.ImportDomainEnum.ToString()));
			string description = selectedAdapter.Description != null ? selectedAdapter.Description : "";
			adapter.Add(new XAttribute("Description", description));
			return adapter;
		}

		//private static void UpdateXElementAdapter(ref XElement adapter, ImportModuleInfo selectedAdapter)
		//{
		//	adapter.Attribute("Name").Value = selectedAdapter.Name;
		//	adapter.Attribute("Title").Value =  selectedAdapter.Title;
		//	adapter.Attribute("UserControlType").Value = selectedAdapter.UserControlType.Name;
		//	adapter.Attribute("ImportDomainEnum").Value = selectedAdapter.ImportDomainEnum.ToString();
		//	string description = selectedAdapter.Description != null ? selectedAdapter.Description : "";
		//	adapter.Attribute("Description").Value = description;
		//}



		private static void UpdateXElementAdapter(ref XElement adapter, IBaseImportExportModuleInfo selectedAdapter)
		{
			adapter.Attribute("Name").Value = selectedAdapter.Name;
			adapter.Attribute("Title").Value =  selectedAdapter.Title;
			string description = selectedAdapter.Description != null ? selectedAdapter.Description : "";
			adapter.Attribute("Description").Value = description;

			if (selectedAdapter is IImportModuleInfo)
			{
				IImportModuleInfo importModuleInfo = selectedAdapter as IImportModuleInfo;
				adapter.Attribute("UserControlType").Value = importModuleInfo.UserControlType.Name;
				adapter.Attribute("ImportDomainEnum").Value = importModuleInfo.ImportDomainEnum.ToString();
			}
			else if (selectedAdapter is IExportErpModuleInfo)
			{
				IExportErpModuleInfo exportErpModuleInfo = selectedAdapter as IExportErpModuleInfo;
				adapter.Attribute("UserControlType").Value = exportErpModuleInfo.UserControlType.Name;
			}
			else if (selectedAdapter is IExportPdaModuleInfo)
			{
				IExportPdaModuleInfo exportPdaModuleInfo = selectedAdapter as IExportPdaModuleInfo;
				adapter.Attribute("UserControlType").Value = exportPdaModuleInfo.UserControlType.Name;
				adapter.Attribute("ImportDomainEnum").Value = exportPdaModuleInfo.ImportDomainEnum.ToString();
			}

		}


	
		
	}
}
