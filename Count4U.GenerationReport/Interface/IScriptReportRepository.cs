using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using System.Text;
namespace Count4U.GenerationReport
{
  
	public interface IScriptReportRepository
	{
		void RunReportScriptFromFile(bool isMain, bool isClear, bool isClearTag, bool toSetupDB, string path, Encoding encoding);
		void SaveReportScriptToFile(bool isMain, string path, Encoding encoding);

		void SaveTagReportToDb(/*bool isMain*/);
	}
}