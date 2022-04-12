using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Main.Interface;

namespace Main.Service
{
	public class MainSerivce : IMainService
	{
		private string[] _programArgs;
		public string Test(string name)
		{
			return name;
		}

		public void InitProgramArgs(string[] ProgramArgs)
		{
			this._programArgs = Count4U.WindowsHost.Program.ProgramArgs;
		}

		public string[] GetProgramArgs()
		{
			return Count4U.WindowsHost.Program.ProgramArgs;
		}
	}
}
	
