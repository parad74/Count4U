using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Count4U.Model
{
	public enum LocationLevel
	{
		ErrorLevel = -1,
		One = 1,
		Two = 2,
		Three = 3,
		Four = 4
	}

	public enum LevelTypeEnum
	{
		FinalType = 0,
		NotFinalType = 1,
		BothType = 2,
		NotSet = -1
	}

	//public class DomainUnknownCode
	//{
	//	public const string UnknownSection = "none";
	//}

	//public class DomainUnknownName
	//{
	//}
}
