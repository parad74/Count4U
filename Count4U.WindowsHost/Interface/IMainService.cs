using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Main.Interface
{
	public interface IMainService
	{
		void InitProgramArgs(string[] ProgramArgs);
		string[] GetProgramArgs();
	}
}
