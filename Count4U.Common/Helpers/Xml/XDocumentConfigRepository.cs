using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Count4U.Common.Enums;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Common.ViewModel.Adapters.Export;
using Count4U.Common.ViewModel.Adapters.Import;
using Count4U.Model.Extensions;

namespace Count4U.Common
{
	public static class XDocumentConfigRepository
	{
		public static int SetValueFromConfig(this Dictionary<string, OperationXElement> dictionaryFromInitXDocument, string propertyName, int param)
		{
			int ret = param;
			int outRet = 0;
			if (dictionaryFromInitXDocument.ContainsKey(propertyName) == true)
			{
				OperationXElement element = dictionaryFromInitXDocument[propertyName];
				string ttype = element.ReturnType.ToLower();
				if (ttype == "int16" || ttype == "int32" || ttype == "int64")
				{
					if (Int32.TryParse(element.ValueToSet, out outRet) == true)
					{
						if (outRet == 0) return ret;
						else return outRet;
					}
				}
				else return ret;
			}
			return ret;
		}

		public static string SetValueFromConfig(this Dictionary<string, OperationXElement> dictionaryFromInitXDocument, string propertyName, string param)
		{
			string ret = param;
			if (dictionaryFromInitXDocument.ContainsKey(propertyName) == true)
			{
				OperationXElement element = dictionaryFromInitXDocument[propertyName];
				string ttype = element.ReturnType.ToLower();
				if (ttype == "string")
				{
					ret = element.ValueToSet == string.Empty ? ret : element.ValueToSet;
					return ret;
				}
				else return ret;
			}
			return ret;
		}


		public static bool SetValueFromConfig(this Dictionary<string, OperationXElement> dictionaryFromInitXDocument, string propertyName, bool param)
		{
			bool ret = param;
			bool outRet = param;
			if (dictionaryFromInitXDocument.ContainsKey(propertyName) == true)
			{
				OperationXElement element = dictionaryFromInitXDocument[propertyName];
				string ttype = element.ReturnType.ToLower();
				if (string.IsNullOrWhiteSpace(element.ValueToSet) == true) return ret;
				if (ttype == "boolean")
				{
					if (Boolean.TryParse(element.ValueToSet, out outRet) == true)
					{
						return outRet;
					}
				}
				else return ret;
			}
			return ret;
		}

		//решено пока, что все кофиги храняться у кастомера, в импорт директории,
		// configXDocument заполняем из них - имя файла строится <имя-code адаптера>.config
		//viewModel - viewModel of adapter
		public static void InitXDocumentConfig(object viewModel, XDocument configXDocument)
		{
			//   OperationXElement
			if (configXDocument == null) return;
			var retXElements = configXDocument.Descendants("PROPERTY");
			if (retXElements == null) return;
			Dictionary<string, OperationXElement> dictionaryProperty = new Dictionary<string, OperationXElement>();


			//var responseTestData = xdoc.Descendants("operation").Where(t => t.Attribute("code").Value == codeOperation).Descendants("response").FirstOrDefault().Value;
			foreach (var xelement in retXElements)
			{
				try
				{
					OperationXElement operationXElement = new OperationXElement(xelement);
					string key = operationXElement.Name;
					if (string.IsNullOrWhiteSpace(key) == false)
					{
						dictionaryProperty[key] = operationXElement;
					}
				}
				catch (Exception exp)
				{
					continue;
				}
			}

			PropertyInfo[] props = viewModel.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

			//этому проперти присвоить данные 
			//которые распарсить по правилам в объекте 
			foreach (KeyValuePair<string, OperationXElement> keyValuePair in dictionaryProperty)
			{
				for (int i = 0; i < props.Length; i++)
				{
					var attr = props[i].GetCustomAttributes(typeof(NotInludeAttribute), true).FirstOrDefault() as NotInludeAttribute;
					if (attr != null) continue;

					if (props[i].Name.ToLower() == keyValuePair.Key.ToLower())
					{
						string ttype =  keyValuePair.Value.ReturnType.ToLower();
						string stringValue = keyValuePair.Value.ValueToSet;
						try
						{
							if (ttype == "string")
							{
								try
								{
									props[i].SetValue(viewModel, stringValue, null);
								}
								catch (Exception exc) 
								{ 
									props[i].SetValue(viewModel, "ERROR = " + exc.Message, null); 
								}
							}
							else if (ttype == "boolean")
							{
								bool boolValue = false;
								bool ret = Boolean.TryParse(stringValue, out boolValue);
								if (ret == true)
								{
									props[i].SetValue(viewModel, boolValue, null);
								}
							}
							else if (ttype == "int16" || ttype == "int32" || ttype == "int64")
							{
								int intValue = 0;
								bool ret = Int32.TryParse(stringValue, out intValue);
								if (ret == true)
								{
									if (props[i].Name == "Encoding")
									{
										try
										{
											// не буду парится берем из кода всегда
											//var encoding = viewModel.GetType().GetProperty("Encoding");
											//props[i].= encoding;
										}
										catch { }
									}
									else
									{
										props[i].SetValue(viewModel, intValue, null);
									}
								}
							}
						}
						catch (Exception ecx)
						{
							string error = "ERROR = " + ecx.Message;
						}
					}
				}
			}

		}

		public static Dictionary<string, OperationXElement> GetDictionaryFromInitXDocumentConfig(XDocument configXDocument)
		{
			//   OperationXElement
			Dictionary<string, OperationXElement> dictionaryProperty = new Dictionary<string, OperationXElement>();
			if (configXDocument == null) return dictionaryProperty;
			var retXElements = configXDocument.Descendants("PROPERTY");
			if (retXElements == null) return dictionaryProperty;

			//var responseTestData = xdoc.Descendants("operation").Where(t => t.Attribute("code").Value == codeOperation).Descendants("response").FirstOrDefault().Value;
			foreach (var xelement in retXElements)
			{
				try
				{
					OperationXElement operationXElement = new OperationXElement(xelement);
					string key = operationXElement.Name;
					if (string.IsNullOrWhiteSpace(key) == false)
					{
						dictionaryProperty[key] = operationXElement;
					}
				}
				catch (Exception exp)
				{
					continue;
				}
			}
			return dictionaryProperty;
		}

		//решено пока, что все кофиги храняться у кастомера, в импорт директории, но работая с данными с данными с уровня инветора 
		//<FROMPATH domainobject="inventor" adapteruse="import" howuse="relative" from="indata" value="" isdefault="1" />
		//<FROMPATH domainobject="inventor" adapteruse="import" howuse="asis" from="absolute" value="C:\ErpPath" isdefault="1" />
		public static string GetImportPath(object viewModel, XDocument configXDocument)
		{
			string path = "";
			if (configXDocument == null) return path;
			var retXElements = configXDocument.Descendants("FROMPATH").Where(t => t.Attribute("isdefault").Value == "1").FirstOrDefault();
			if (retXElements == null) return path;

			string howuse = (string)retXElements.Attribute("howuse") ?? "";
			if (howuse.ToLower() == HowUse.asis.ToString())
			{ 
				path = (string)retXElements.Attribute("value") ?? "";
				return path;
			}
			if (howuse.ToLower() == HowUse.relative.ToString())
			{
				string domainobject = (string)retXElements.Attribute("domainobject") ?? "";
				if (viewModel == null) return path;
				ImportModuleBaseViewModel viewModelBase = viewModel as ImportModuleBaseViewModel;
				if (viewModelBase == null) return path;
				return viewModelBase.GetImportFolderPathFromConfig(domainobject); 
			}

			return path;
		}

		public static string GetFixedImportPath(XDocument configXDocument)
		{
			string path = "";
			if (configXDocument == null) return path;
			var retXElements = configXDocument.Descendants("FROMPATH").Where(t => t.Attribute("isdefault").Value == "1").FirstOrDefault();
			if (retXElements == null) return path;

			string howuse = (string)retXElements.Attribute("howuse") ?? "";
			if (howuse.ToLower() == HowUse.asis.ToString())
			{
				path = (string)retXElements.Attribute("value") ?? "";
				return path;
			}
			return path;
		}

		//решено пока, что все кофиги храняться у кастомера, в импорт директории, но работая с данными с данными с уровня инветора 
		public static string GetExportErpPath(object viewModel, XDocument configXDocument)
		{
			string path = "";
			if (configXDocument == null) return path;
			var retXElements = configXDocument.Descendants("FROMPATH").Where(t => t.Attribute("isdefault").Value == "1").FirstOrDefault();
			if (retXElements == null) return path;

			string howuse = (string)retXElements.Attribute("howuse") ?? "";
			if (howuse.ToLower() == HowUse.asis.ToString())
			{
				path = (string)retXElements.Attribute("value") ?? "";
				return path;
			}
			if (howuse.ToLower() == HowUse.relative.ToString())
			{
				string domainobject = (string)retXElements.Attribute("domainobject") ?? "";
				if (viewModel == null) return path;
				ExportErpModuleBaseViewModel viewModelBase = viewModel as ExportErpModuleBaseViewModel;
				if (viewModelBase == null) return path;
				return viewModelBase.BuildPathToExportErpDataFolder();
			}

			return path;
		}

		public static string GetFixedExportErpPath(XDocument configXDocument)
		{
			string path = "";
			if (configXDocument == null) return path;
			var retXElements = configXDocument.Descendants("FROMPATH").Where(t => t.Attribute("isdefault").Value == "1").FirstOrDefault();
			if (retXElements == null) return path;

			string howuse = (string)retXElements.Attribute("howuse") ?? "";
			if (howuse.ToLower() == HowUse.asis.ToString())
			{
				path = (string)retXElements.Attribute("value") ?? "";
				return path;
			}
			return path;
		}

		public static string GetFixedExportErpPath(object viewModel, XDocument configXDocument)
		{
			string path = "";
			if (configXDocument == null) return path;
			var retXElements = configXDocument.Descendants("FROMPATH").Where(t => t.Attribute("isdefault").Value == "1").FirstOrDefault();
			if (retXElements == null) return path;

			string howuse = (string)retXElements.Attribute("howuse") ?? "";
			if (howuse.ToLower() == HowUse.asis.ToString())
			{
				path = (string)retXElements.Attribute("value") ?? "";
				return path;
			}
			return path;
		}

		//решено пока, что все кофиги храняться у кастомера, в импорт директории, но работая с данными с данными с уровня инветора 
		//public static string GetExportPath(object viewModel, XDocument configXDocument)
		//{
		//	string path = "";
		//	ImportModuleBaseViewModel viewModelBase = viewModel as ImportModuleBaseViewModel;
		//	if (configXDocument == null) return path;
		//	var retXElements = configXDocument.Descendants("FROMPATH").Where(t => t.Attribute("isdefault").Value == "1").FirstOrDefault();
		//	if (retXElements == null) return path;

		//	string howuse = (string)retXElements.Attribute("howuse") ?? "";
		//	if (howuse.ToLower() == "relative")
		//	{
		//		string domainobject = (string)retXElements.Attribute("domainobject") ?? "";
		//		return viewModelBase.GetImportFolderPathFromConfig(domainobject);
		//	}

		//	return path;
		//}

		//public static XElement GetXElementExportPdaAdapterProperty(object viewModel, IExportPdaModuleInfo selectedAdapter)
		//{
		//	//IImportModuleInfo selectedAdapter = SelectedAdapter;
		//	Type vm = selectedAdapter.UserControlType;
		//	//_configXML = _configXML + "<TEST>" + selectedAdapter.UserControlType.Name + "</TEST>" + Environment.NewLine;

		//	XElement root = new XElement("ROOT");

		//	XElement adapter = new XElement("INFO");
		//	adapter.Add(new XAttribute("Name", selectedAdapter.Name));
		//	adapter.Add(new XAttribute("Title", selectedAdapter.Title));
		//	adapter.Add(new XAttribute("UserControlType", selectedAdapter.UserControlType.Name));
		//	adapter.Add(new XAttribute("ImportDomainEnum", selectedAdapter.ImportDomainEnum.ToString()));
		//	string description = 	selectedAdapter.Description != null ? selectedAdapter.Description : "";
		//	adapter.Add(new XAttribute("Description", description));
		//	root.Add(adapter);


		//	if (viewModel != null)
		//	{
		//		PropertyInfo[] props = viewModel.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

		//		for (int i = 0; i < props.Length; i++) //responseData
		//		{
		//			if (props[i] == null) continue;
		//			//if (props[i].Name.ToLower() 
		//			//string parceData = sip2Protocol.GetValueFromStringByFormat(entity, keyValuePair.Value.xElementFormat);
		//			try
		//			{
		//				string propTypeName = props[i].PropertyType.Name;
		//				string propName = props[i].Name;

		//				if (propTypeName.ToLower() == "boolean"
		//				|| propTypeName.ToLower() == "string"
		//				|| propTypeName.ToLower() == "int16"
		//				|| propTypeName.ToLower() == "int32"
		//				 || propTypeName.ToLower() == "int64")
		//				{
		//					if (propName.Contains("Execute") == true) continue;
		//					if (propName.Contains("Error") == true) continue;
		//					if (propName.Contains("Command") == true) continue;
		//					var attr = props[i].GetCustomAttributes(typeof(NotInludeAttribute), true).FirstOrDefault() as NotInludeAttribute;
		//					//bool notBulk = props[i].Attributes.OfType<NotInludeAttribute>().Any();
		//					if (attr != null) continue;

		//					var propValue = props[i].GetValue(viewModel, null);
		//					if (propValue != null)
		//					{
		//						XElement propAdapter = new XElement("PROPERTY");
		//						propAdapter.Add(new XAttribute("returntype", propTypeName));
		//						propAdapter.Add(new XAttribute("name", props[i].Name));
		//						propAdapter.Add(new XAttribute("value", propValue.ToString()));
		//						adapter.Add(propAdapter);

		//						//	string ret = propTypeName + " " + props[i].Name + " = " + propValue.ToString();
		//						//_configXML = _configXML + ret + Environment.NewLine;
		//					}

		//				}
		//			}
		//			catch (Exception ex)
		//			{
		//				//_configXML = _configXML + " Error : " + ex.Message + Environment.NewLine;
		//			}
		//		}
		//	}
		//	return root;
		//}

		//public static XElement GetXElementExportERPAdapterProperty(object viewModel, IExportErpModuleInfo selectedAdapter)
		//{
		//	//IImportModuleInfo selectedAdapter = SelectedAdapter;
		//	Type vm = selectedAdapter.UserControlType;
		//	//_configXML = _configXML + "<TEST>" + selectedAdapter.UserControlType.Name + "</TEST>" + Environment.NewLine;

		//	XElement root = new XElement("ROOT");

		//	XElement adapter = new XElement("INFO");
		//	adapter.Add(new XAttribute("Name", selectedAdapter.Name));
		//	adapter.Add(new XAttribute("Title", selectedAdapter.Title));
		//	adapter.Add(new XAttribute("UserControlType", selectedAdapter.UserControlType.Name));
		//	adapter.Add(new XAttribute("ImportDomainEnum","ExportErpAdapter"));
		//	string description = 	selectedAdapter.Description != null ? selectedAdapter.Description : "";
		//	adapter.Add(new XAttribute("Description", description));
		//	root.Add(adapter);


		//	if (viewModel != null)
		//	{
		//		PropertyInfo[] props = viewModel.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

		//		for (int i = 0; i < props.Length; i++) //responseData
		//		{
		//			if (props[i] == null) continue;
		//			//if (props[i].Name.ToLower() 
		//			//string parceData = sip2Protocol.GetValueFromStringByFormat(entity, keyValuePair.Value.xElementFormat);
		//			try
		//			{
		//				string propTypeName = props[i].PropertyType.Name;
		//				string propName = props[i].Name;

		//				if (propTypeName.ToLower() == "boolean"
		//				|| propTypeName.ToLower() == "string"
		//				|| propTypeName.ToLower() == "int16"
		//				|| propTypeName.ToLower() == "int32"
		//				 || propTypeName.ToLower() == "int64")
		//				{
		//					if (propName.Contains("Execute") == true) continue;
		//					if (propName.Contains("Error") == true) continue;
		//					if (propName.Contains("Command") == true) continue;
		//					var attr = props[i].GetCustomAttributes(typeof(NotInludeAttribute), true).FirstOrDefault() as NotInludeAttribute;
		//					//bool notBulk = props[i].Attributes.OfType<NotInludeAttribute>().Any();
		//					if (attr != null) continue;

		//					var propValue = props[i].GetValue(viewModel, null);
		//					if (propValue != null)
		//					{
		//						XElement propAdapter = new XElement("PROPERTY");
		//						propAdapter.Add(new XAttribute("returntype", propTypeName));
		//						propAdapter.Add(new XAttribute("name", props[i].Name));
		//						propAdapter.Add(new XAttribute("value", propValue.ToString()));
		//						adapter.Add(propAdapter);

		//						//	string ret = propTypeName + " " + props[i].Name + " = " + propValue.ToString();
		//						//_configXML = _configXML + ret + Environment.NewLine;
		//					}

		//				}
		//			}
		//			catch (Exception ex)
		//			{
		//				//_configXML = _configXML + " Error : " + ex.Message + Environment.NewLine;
		//			}
		//		}
		//	}
		//	return root;
		//}

		//public static XDocument VerifyToAdapter(XDocument configXMLdoc, ImportModuleInfo selectedAdapter)		
		//{
		//	if (selectedAdapter == null) return configXMLdoc;

		//	XElement adapter = configXMLdoc.Descendants( "INFO").FirstOrDefault();

		//	UpdateXElementAdapter(ref adapter, selectedAdapter);
		//	return configXMLdoc;
		//}

		//public static XDocument RemoveAllElement(XDocument doc, string tag)
		//{
		//	IEnumerable<XElement> tracks = doc.Descendants(tag);
		//	tracks.Remove();
		//	return doc;
		//}

		//public static XElement GetXElementImportAdapterProperty(object viewModel, ImportModuleInfo selectedAdapter)
		//{
		//	if (selectedAdapter == null) return new XElement("ROOT");
		//	//IImportModuleInfo selectedAdapter = SelectedAdapter;
		//	Type vm = selectedAdapter.UserControlType;
		//	//_configXML = _configXML + "<TEST>" + selectedAdapter.UserControlType.Name + "</TEST>" + Environment.NewLine;

		//	XElement root = new XElement("ROOT");

		//	XElement adapter = CreateXElementAdapter(selectedAdapter);
		//	root.Add(adapter);


		//	if (viewModel != null)
		//	{
		//		PropertyInfo[] props = viewModel.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

		//		for (int i = 0; i < props.Length; i++) //responseData
		//		{
		//			if (props[i] == null) continue;
		//			//if (props[i].Name.ToLower() 
		//			//string parceData = sip2Protocol.GetValueFromStringByFormat(entity, keyValuePair.Value.xElementFormat);
		//			try
		//			{
		//				string propTypeName = props[i].PropertyType.Name;
		//				string propName = props[i].Name;

		//				if (propTypeName.ToLower() == "boolean"
		//				|| propTypeName.ToLower() == "string"
		//				|| propTypeName.ToLower() == "int16"
		//				|| propTypeName.ToLower() == "int32"
		//				 || propTypeName.ToLower() == "int64")
		//				{
		//					if (propName.Contains("Execute") == true) continue;
		//					if (propName.Contains("Error") == true) continue;
		//					if (propName.Contains("Command") == true) continue;
		//					var attr = props[i].GetCustomAttributes(typeof(NotInludeAttribute), true).FirstOrDefault() as NotInludeAttribute;
		//					//bool notBulk = props[i].Attributes.OfType<NotInludeAttribute>().Any();
		//					if (attr != null) continue;

		//					var propValue = props[i].GetValue(viewModel, null);
		//					if (propValue != null)
		//					{
		//						XElement propAdapter = new XElement("PROPERTY");
		//						propAdapter.Add(new XAttribute("returntype", propTypeName));
		//						propAdapter.Add(new XAttribute("name", props[i].Name));
		//						propAdapter.Add(new XAttribute("value", propValue.ToString()));
		//						adapter.Add(propAdapter);

		//						//	string ret = propTypeName + " " + props[i].Name + " = " + propValue.ToString();
		//						//_configXML = _configXML + ret + Environment.NewLine;
		//					}

		//				}
		//			}
		//			catch (Exception ex)
		//			{
		//				//_configXML = _configXML + " Error : " + ex.Message + Environment.NewLine;
		//			}
		//		}
		//	}
		//	return root;
		//}

		//private static XElement CreateXElementAdapter(ImportModuleInfo selectedAdapter)
		//{
		//	XElement adapter = new XElement("INFO");
		//	adapter.Add(new XAttribute("Name", selectedAdapter.Name));
		//	adapter.Add(new XAttribute("Title", selectedAdapter.Title));
		//	adapter.Add(new XAttribute("UserControlType", selectedAdapter.UserControlType.Name));
		//	adapter.Add(new XAttribute("ImportDomainEnum", selectedAdapter.ImportDomainEnum.ToString()));
		//	string description = selectedAdapter.Description != null ? selectedAdapter.Description : "";
		//	adapter.Add(new XAttribute("Description", description));
		//	return adapter;
		//}

		//private static void UpdateXElementAdapter(ref XElement adapter, ImportModuleInfo selectedAdapter)
		//{
		//	adapter.Attribute("Name").Value = selectedAdapter.Name;
		//	adapter.Attribute("Title").Value =  selectedAdapter.Title;
		//	adapter.Attribute("UserControlType").Value = selectedAdapter.UserControlType.Name;
		//	adapter.Attribute("ImportDomainEnum").Value = selectedAdapter.ImportDomainEnum.ToString();
		//	string description = selectedAdapter.Description != null ? selectedAdapter.Description : "";
		//	adapter.Attribute("Description").Value = description;
		//}

		
	}
}
