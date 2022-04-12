using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using System.Text;
namespace Count4U.GenerationReport
{
	public interface IReportIniRepository
	{
		string CopyReportTemplateIniFile(string code, string objectType = "Inventor");
		string CopyContextMenuReportTemplateIniFile(string inventorCode);
		string CopyPrintReportTemplateIniFile(string inventorCode);
		string CopyReportTemplateIniFileToCustomer(string configPath);
		
	}
}