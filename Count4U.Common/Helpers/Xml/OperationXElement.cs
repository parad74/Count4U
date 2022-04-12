using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Count4U.Common.Interfaces.Adapters;
using Count4U.Model.Extensions;

namespace Count4U.Common
{
	public class OperationXElement
	{
		//		propAdapter.Add(new XAttribute("returntype", propTypeName));
		//		propAdapter.Add(new XAttribute("name", props[i].Name));
		//		propAdapter.Add(new XAttribute("value", propValue.ToString()))

		public string Code { get; set; }
		public string Name { get; set; }
		public string ValueToSet { get; set; }
		public string ReturnType { get; set; }
		public XElement xElement { get; set; }

		public OperationXElement()
		{
			this.Code = "";
			this.Name = "";
			this.ValueToSet = "";
			this.ReturnType = "";
		}

		public OperationXElement(XElement operationXElement)
		{
			this.xElement = operationXElement;

			if (operationXElement != null)
			{
				this.Code = (string)operationXElement.Attribute("code") ?? "";
				this.Name = (string)operationXElement.Attribute("name") ?? "";
				this.ValueToSet = (string)operationXElement.Attribute("value") ?? "";
				this.ReturnType = (string)operationXElement.Attribute("returntype") ?? "";

			}
			else
			{
				this.Code = "";
				this.Name = "";
				this.ValueToSet = "";
				this.ReturnType = "";
			}
		}

		public bool Equals(OperationXElement other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Name, this.Name);
		}

		public override int GetHashCode()
		{
			return (this.Code != null ? this.Code.GetHashCode() : 0);
		}
	}
}

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

		
	//}
//}
