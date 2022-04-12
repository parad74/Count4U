using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;
using Count4U.Model;
using System.IO;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Interface
{
	public interface IExportCurrentInventoryWriter
	{
		void AddRow(StreamWriter sw, CurrentInventoryAdvanced currentInventory, long countRow,
			string ERPNum = "", string INVDate = "", string separator = ",", Dictionary<ImportProviderParmEnum, object> parms = null, List<PreviousInventory> details = null);
		void AddRowSimple(StreamWriter sw, CurrentInventory currentInventory, long countRow,
			string ERPNum = "", string INVDate = "", string separator = ",", Dictionary<ImportProviderParmEnum, object> parms = null, List<PreviousInventory> details = null);
		void AddHeader(StreamWriter sw,
			string ERPNum = "", string INVDate = "", string separator = ",",  Dictionary<ImportProviderParmEnum, object> parms = null, object argument = null);
		void AddHeaderSum(StreamWriter sw, IEnumerable<CurrentInventory> currentInventoryList = null,
			string ERPNum = "", string INVDate = "", string separator = ",", Dictionary<ImportProviderParmEnum, object> parms = null);
		void AddFooter(StreamWriter sw, long countRow,
			string ERPNum = "", string INVDate = "", string separator = ",", Dictionary<ImportProviderParmEnum, object> parms = null);
		void AddFooterSum(StreamWriter sw, long countRow, IEnumerable<CurrentInventory> currentInventoryList = null,
			string ERPNum = "", string INVDate = "", string separator = ",", Dictionary<ImportProviderParmEnum, object> parms = null);

	}
}
