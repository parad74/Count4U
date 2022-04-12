using System;
using Count4U.Model.Audit;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;
using Count4U.Model.ProcessC4U;

namespace Count4U.Model.Interface.ProcessC4U
{
	public interface IMainProcessJobRepository
	{
		ProcessJobs GetMainProcessJobs()	;
		ProcessJobs GetMainProcessJobs(SelectParams selectParams)  ;
		ProcessJob GetMainProcessJob(long id)	;
		void Delete(long id)	 ;
		void Insert(ProcessJob mainProcessJob)  ;
		void Update(ProcessJob mainProcessJob);
    }
}
