using System;
using Count4U.Model.Audit;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;
using Count4U.Model.ProcessC4U;

namespace Count4U.Model.Interface.ProcessC4U
{
    public interface IPortRepository
	{
		Ports GetPorts(SelectParams selectParams);
		Port GetPortByCode(string portCode);
		List<string> GetCodeList();
		void Delete(Port port, bool full = true);
		void Delete(string code);
		void Insert(Ports ports);
		void Update(Port port);
    }
}
