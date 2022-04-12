using Count4U.Model.Audit;
using Count4U.Model.Count4U;
using Count4U.Model.Main;
using Count4U.Model.SelectionParams;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.IO;

namespace Count4U.Model.Count4U
{
	public class ScriptFieldLinktRepository : IScriptFieldLinkRepository
    {
		private readonly IServiceLocator _serviceLocator;
		public ScriptFieldLinktRepository(
			 IServiceLocator serviceLocator)
		{
			this._serviceLocator = serviceLocator;
		}

		public void RunFieldLinkScriptFromFile(bool isClear, bool toSetupDB, string path, Encoding encoding)
		{
			IFileParser fileParser = this._serviceLocator.GetInstance<IFileParser>();
			string sql = "";
			IAlterADOProvider alterADOProvider = this._serviceLocator.GetInstance<IAlterADOProvider>();
			foreach (String record in fileParser.GetRecords(path, encoding, 0))
			{
				if (record.ToUpper().Contains("DROP") == false
					&& record.ToUpper().Contains("DELETE") == false
					&& record.ToUpper().Contains("UPDATE") == false
					&& record.ToUpper().Contains("ALTER") == false
					&& record.ToUpper().Contains("CREATE") == false
					&& record.ToUpper().Contains("SELECT") == false)
				{
					if (record.ToUpper().Contains("INSERT") == true)
					{
					    string fieldLinkTable = @"[FieldLink]";
						if (record.Contains(fieldLinkTable) == true)
						{
							sql = sql + record + Environment.NewLine;
						}
					}
				}
			}  //foreach record

			alterADOProvider.ImportMainReport(sql, isClear, toSetupDB);
		}

		public void SaveFieldLinkScriptToFile(string path, Encoding encoding)
		{
			ILog log = this._serviceLocator.GetInstance<ILog>();
			IAlterADOProvider alterADOProvider = this._serviceLocator.GetInstance<IAlterADOProvider>();
			IFieldLinkRepository fieldLinkRepository = this._serviceLocator.GetInstance<IFieldLinkRepository>();
			FieldLinks fieldLinks = null;
			string sql = @"INSERT INTO [FieldLink] ";
			string retSql = "";
			if (fieldLinks != null)
			{
				foreach (var fieldLink in fieldLinks)
				{
					if (string.IsNullOrWhiteSpace(fieldLink.DomainType) == true) continue;
					if (string.IsNullOrWhiteSpace(fieldLink.PropertyNameInDomainType) == true) continue;
					string domainType = string.IsNullOrWhiteSpace(fieldLink.DomainType) ? "" : fieldLink.DomainType.Replace("'", "''");
					string propertyNameInDomainType = string.IsNullOrWhiteSpace(fieldLink.PropertyNameInDomainType) ? "" : fieldLink.PropertyNameInDomainType.Replace("'", "''");
					string tableName = string.IsNullOrWhiteSpace(fieldLink.TableName) ? "" : fieldLink.TableName.Replace("'", "''");
					string fieldNameInTable = string.IsNullOrWhiteSpace(fieldLink.FieldNameInTable) ? "" : fieldLink.FieldNameInTable.Replace("'", "''");
					string numStringInRecord = string.IsNullOrWhiteSpace(fieldLink.NumStringInRecord.ToString()) ? "" : fieldLink.NumStringInRecord.ToString();
					string editor = string.IsNullOrWhiteSpace(fieldLink.Editor) ? "" : fieldLink.Editor.Replace("'", "''");
					string validator = string.IsNullOrWhiteSpace(fieldLink.Validator) ? "" : fieldLink.Validator.Replace("'", "''");
					string codeLocalizationEditorLable = fieldLink.CodeLocalizationEditorLable;
					string defaultEditorLable = string.IsNullOrWhiteSpace(fieldLink.DefaultEditorLable) ? "" : fieldLink.DefaultEditorLable.Replace("'", "''");
					string nn = string.IsNullOrWhiteSpace(fieldLink.NN.ToString()) ? "" : fieldLink.NN.ToString();
					string inGrid = fieldLink.InGrid ? "1" : "0";
					string inEdit = fieldLink.InEdit ? "1" : "0";
					string inAdd = fieldLink.InAdd ? "1" : "0";

					//([DomainType],[TableName],[PropertyNameInDomainType],[FieldNameInTable],	[NumStringInRecord]
					//,[Editor],[Validator],[CodeLocalizationEditorLable],[DefaultEditorLable],[NN],[InGrid],[InEdit],[InAdd]) 
					//VALUES (N'InventProduct',N'InventProduct',N'IPValueStr1',N'IPValueStr1',6
					//,N'String',N'String_100',N'',N'IPValueStr1',1,1,0,0);
					try
					{
					    string sql1 = sql +
							@"([DomainType],[TableName],[PropertyNameInDomainType],[FieldNameInTable],[NumStringInRecord]" +
							@",[Editor],[Validator],[CodeLocalizationEditorLable],[DefaultEditorLable],[NN],[InGrid],[InEdit],[InAdd])  " +
							@"VALUES " +
					// 	 	N{0}						N{1}						 N{2}						N{3}	      {4}		N{5}			N{6}		N{7}	  N{8}			 {9}{10}{11}{12}
					//		   (N'InventProduct',N'InventProduct',N'IPValueStr1',N'IPValueStr1',6,N'String',N'String_100',N'',N'IPValueStr1',1,1,0,0);
							String.Format("(N'{0}',N'{1}',N'{2}',N'{3}',{4},N'{5}',N'{6}',N'{7}',N'{8}',{9},{10},{11},{12});" + Environment.NewLine,
							domainType.Trim(), tableName.Trim(), propertyNameInDomainType.Trim(), fieldNameInTable.Trim(), numStringInRecord.Trim(),
							editor.Trim(), validator.Trim(), codeLocalizationEditorLable.Trim(), defaultEditorLable.Trim(), nn.Trim(), inGrid.Trim(), inEdit.Trim(), inAdd.Trim());
						retSql = retSql + sql1;
					}
					catch (Exception exp)
					{
						//this.Log = this.Log + exp.Message + " : " + report.FileName;
						log.Add(Model.MessageTypeEnum.Error, exp.Message + " : " + propertyNameInDomainType);
					}
				}
				File.WriteAllText(path, retSql, encoding);
			}
		}



	}
}