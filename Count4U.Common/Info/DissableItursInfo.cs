using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel;
using Count4U.GenerationReport;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Transfer;
using NLog;
using Count4U.Model.SelectionParams;
using System.Threading.Tasks;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.Ini;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Common
{
	public class DissableItursInfo
	{
		public List<string> IturCodes { get; set; }
		public bool Dissable { get; set; }

		public DissableItursInfo()
		{
			this.IturCodes = new List<string>();
			this.Dissable = false;

		}


	}

}
