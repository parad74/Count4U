using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.Objects;
using System.Data.SqlServerCe;
using System.Data;
using Count4U.Model.Interface;
using System.Xml.Linq;
using Count4U.Model;
using Count4U.Model.Count4U;
using Microsoft.Practices.ServiceLocation;
using System.IO;
using Count4U.Model.Interface.Count4U;
using System.Xml.Serialization;
using System.Xml;
using Count4U.Model.Malam;

namespace Count4U.Model
{
	public class ForwardInventProductMalamFileProvider : BaseProvider, IExportProvider
	{
		public ForwardInventProductMalamFileProvider(
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			//this._importTypes.Add(ImportDomainEnum.ImportInventProduct);
		}

		public void Export()
		{
			string fromFilePath = base.Parms.GetStringValueFromParm(ImportProviderParmEnum.SourcePath);	  
			string toFilePath = base.Parms.GetStringValueFromParm(ImportProviderParmEnum.DestinationPath);

			base.FillInfoLog(fromFilePath, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(fromFilePath) == true) return;
			if (this.IsEmptyPath(toFilePath) == true) return;

			IInventProductToObjectParser inventProductParser =
							this._serviceLocator.GetInstance<IInventProductToObjectParser>(InventProductSimpleParserEnum.InventProductToMalamXMLParser.ToString());
			if (inventProductParser == null)
			{
				//Localization.Resources.Log_Error1001%"In  ProductParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1001, InventProductSimpleParserEnum.InventProductToMalamXMLParser.ToString()));
				return;
			}

			Records records = inventProductParser.GetMyObject(fromFilePath, this.ProviderEncoding, this._separators,
				this._countExcludeFirstString, this._importTypes, this.Parms) as Records;

			Serialise<Records>(records, toFilePath);

			//using (MemoryStream ms = new MemoryStream())
			//{
			//	StreamWriter sw = new StreamWriter(ms,  this.ProviderEncoding);

			//	this._exportRepository.WriteToFile(this.FromPathDB, toPathFile, WriterEnum.ExportInventProductH_MERPFileWriter,
			//this.ProviderEncoding, this._separators, this._importTypes, sw, this.Parms);

			//	sw.Flush();

			//	bool fileXlsx = this.Parms.GetBoolValueFromParm(ImportProviderParmEnum.FileXlsx);
			//	if (fileXlsx == false)
			//	{
			//		File.WriteAllText(toPathFile, this.ProviderEncoding.GetString(ms.ToArray()), this.ProviderEncoding);
			//	}
			//	else
			//	{
			//		WriteAllToExcel(ms, toPathFile, this.ProviderEncoding, this._countExcludeFirstString);
			//	}
			//}
		}

		protected void Serialise<T>(T serialisableObject, string toFilePath)
		{
			var xmlSerializer = new XmlSerializer(serialisableObject.GetType());

			using (var ms = new MemoryStream())
			{
				using (var xw = XmlWriter.Create(ms,
					new XmlWriterSettings()
					{
						Encoding = this.ProviderEncoding,
						Indent = true,
						NewLineOnAttributes = true,
					}))
				{
					xmlSerializer.Serialize(xw, serialisableObject);
					xw.Flush();
					File.WriteAllText(toFilePath, this.ProviderEncoding.GetString(ms.ToArray()), this.ProviderEncoding);

					//return this.ProviderEncoding.GetString(ms.ToArray());
					//return Encoding.UTF8.GetString(ms.ToArray());
				}
			}
		}

		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding("windows-1255");
			this._separators = new string[] { SeparatorField.I };
			this._countExcludeFirstString = 0;
		}

		public void Clear()
		{
			//this._exportRepository.DeleteFile(this.FromPathFile);
		}

		//		public static string Serialize<T>(this T toSerialize)
		//{
		//	XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
		//	StringWriter textWriter = new StringWriter();
		//	xmlSerializer.Serialize(textWriter, toSerialize);
		//	return textWriter.ToString();
		//}

		//		public static T Deserialize<T>(this string toDeserialize)
		//{
		//	XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
		//	StringReader textReader = new StringReader(toDeserialize);
		//	return (T)xmlSerializer.Deserialize(textReader);
		//}


	}
}
