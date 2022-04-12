using System;
using Count4U.Model.Audit;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;
using Count4U.Model.ProcessC4U;

namespace Count4U.Model.Interface.ProcessC4U
{
	public interface IProcessRepository
	{
		Processes GetProcesses();
		Processes GetProcesses(SelectParams selectParams);
		Process GetProcess(long id);
		Process GetProcessByProcessCode(string processCode)	;
		void SetStatusInProcess(string processCode);
		void ResetStatusInProcess();
		Process GetProcess_InProcess();
		string GetProcessCode_InProcess();
		Process GetProcessByCode(string code);
		void Delete(long id);
		void Delete(string processCode);
		void Insert(Process Process);
		void Update(Process Process);
		//void RefreshBaseEFRepositoryConnectionDB(); // from base class	 BaseEFRepository
	}
}
