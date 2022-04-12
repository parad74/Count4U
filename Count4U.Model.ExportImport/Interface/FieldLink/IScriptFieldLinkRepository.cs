using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using System.Text;
namespace Count4U.Model.Count4U
{
	public interface IScriptFieldLinkRepository
    {
		void RunFieldLinkScriptFromFile(bool isClear, bool toSetupDB, string path, Encoding encoding);
		void SaveFieldLinkScriptToFile(string path, Encoding encoding);
    }
}