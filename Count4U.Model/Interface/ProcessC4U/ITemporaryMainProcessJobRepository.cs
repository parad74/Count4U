using System;
using Count4U.Model.Audit;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;
using Count4U.Model.ProcessC4U;

namespace Count4U.Model.Interface.ProcessC4U
{
	public interface ITemporaryMainProcessJobRepository
	{
		ProcessJobs GetTemporaryMainProcessJobs();
		ProcessJobs GetTemporaryMainProcessJobs(SelectParams selectParams);
		ProcessJob GetTemporaryMainProcessJob(long id);
		void Delete(long id)	 ;
		void Insert(ProcessJob temporaryMainProcessJob);
		void Update(ProcessJob temporaryMainProcessJob);
    }
}
