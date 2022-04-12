using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.Objects;
using Count4U.Model.SelectionParams;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model;
using Count4U.Model.Count4U;
using System.IO;
using Count4U.Model.Interface;

namespace Count4U.Model.Count4U
{
	public interface IFieldLinkRepository
	{
		IConnectionDB Connection	 {get; set;}
		FieldLinks GetFieldLinks(string pathDB);
		FieldLinks GetFieldLinks(SelectParams selectParams, string pathDB);
		void Delete(FieldLink fieldLink, string pathDB)	 ;
		void DeleteAll(string pathDB);
		void Insert(FieldLink fieldLink, string pathDB);
		void Update(FieldLink fieldLink, string pathDB);
		FieldLink GetFieldLinkByViewName(string viewName, string pathDB) ;
		Dictionary<string, string> GetEditorTemplateDictionary(string pathDB);

	
	}
}
