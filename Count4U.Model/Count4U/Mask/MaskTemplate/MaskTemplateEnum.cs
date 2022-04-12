using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model
{
	public enum MaskTemplateEnum
	{
		AddPrefixRigth = 0,
		AddSuffixLeft = 1,
		PadChar0Left = 2,
		FormatIntToString = 3,
		FormatStringToString = 4,
		None
  	}

	public class MaskTemplateCode
	{
		public const  string AddPrefixRigth = "{R}";
		public const string  AddSuffixLeft = "{L}";
		public const string  PadChar0Left = "{N}";
		public const string  FormatIntToString = "{F}";
		public const string FormatStringToString = "{S}";
		public const string None = "{E}";
	}
}


