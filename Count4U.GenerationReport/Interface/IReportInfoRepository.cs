using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using System.Text;
using System.Collections.Generic;
namespace Count4U.GenerationReport
{
	public interface IReportInfoRepository
	{
		List<ReportInfo> BuildContextMenuReportInfoList(string inventorCode, string reportIniFile, string reportN);
		List<ReportInfo> BuildContextMenuReportInfoList(string inventorCode, string reportIniFile);
	}
}