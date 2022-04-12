using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using System.Data.Entity.Core.Objects;
using Count4U.Model.SelectionParams;
using Count4U.Model.Audit;
using Count4U.Model.Count4U;

namespace Count4U.Model.Main
{
	public static class MaskTemplateRepository
    {
		private static Dictionary<MaskTemplateEnum, MaskTemplate> _maskTemplateDictionary;

		public static Dictionary<MaskTemplateEnum, MaskTemplate> GetMaskTemplateDictionary()
		{
				if (_maskTemplateDictionary == null)
				{
					_maskTemplateDictionary = new Dictionary<MaskTemplateEnum, MaskTemplate>();
					_maskTemplateDictionary.Add(MaskTemplateEnum.AddPrefixRigth,
					new MaskTemplate
					{
						Code = MaskTemplateCode.AddPrefixRigth,
						MaskTemplateType = MaskTemplateEnum.AddPrefixRigth,
						Name = MaskTemplateCode.AddPrefixRigth,
						FormatString = AddPrefixRigthFunction
					});
					_maskTemplateDictionary.Add(MaskTemplateEnum.AddSuffixLeft,
					new MaskTemplate
					{
						Code = MaskTemplateCode.AddSuffixLeft,
						MaskTemplateType = MaskTemplateEnum.AddSuffixLeft,
						Name = MaskTemplateCode.AddSuffixLeft,
						FormatString = AddSuffixLeftFunction
					});
					_maskTemplateDictionary.Add(MaskTemplateEnum.PadChar0Left,
						new MaskTemplate
					{
						Code = MaskTemplateCode.PadChar0Left,
						MaskTemplateType = MaskTemplateEnum.PadChar0Left,
						Name = MaskTemplateCode.PadChar0Left,
						FormatString = PadChar0LeftFunction
					});
					_maskTemplateDictionary.Add(MaskTemplateEnum.FormatIntToString,
						new MaskTemplate
					{
						Code = MaskTemplateCode.FormatIntToString,
						MaskTemplateType = MaskTemplateEnum.FormatIntToString,
						Name = MaskTemplateCode.FormatIntToString,
						FormatString = FormatIntToStringFunction
					});
					_maskTemplateDictionary.Add(MaskTemplateEnum.FormatStringToString,
				new MaskTemplate
				{
					Code = MaskTemplateCode.FormatStringToString,
					MaskTemplateType = MaskTemplateEnum.FormatStringToString,
					Name = MaskTemplateCode.FormatStringToString,
					FormatString = FormatStringToStringFunction
				});
					_maskTemplateDictionary.Add(MaskTemplateEnum.None,
					new MaskTemplate
					{
						Code = MaskTemplateCode.None,
						MaskTemplateType = MaskTemplateEnum.None,
						Name = MaskTemplateCode.None,
						FormatString = NoneFunction
					});
				}
				return _maskTemplateDictionary;
		}

		public static MaskRecord ToMaskRecord(string inputMask)
		{
			MaskRecord ret = null;
			inputMask = inputMask.Trim();

		if ((string.IsNullOrWhiteSpace(inputMask) == true)
				|| (inputMask == MaskTemplateCode.None))
			{
				return ret;
			}
	
			string val = "";
			string code = "";
			string[] vals = inputMask.Split('{');
			if (vals.Length > 1)
			{
				val = vals[0];
				code = "{" + vals[1].Trim(" {}".ToCharArray()) + "}";
			}

			if (string.IsNullOrWhiteSpace(val) == true)
			{
				return ret;
			}


			foreach (KeyValuePair<MaskTemplateEnum, MaskTemplate> keyValuePair
					in GetMaskTemplateDictionary())
			{
				if (code == keyValuePair.Value.Code)
				{
					ret = new MaskRecord
					{
						MaskTemplateType =  keyValuePair.Value.MaskTemplateType,
						Value = val
					};
					return ret;
				}
  			}
			return ret;
		}

  		private static string AddPrefixRigthFunction(string inputString, string maskValue)
		{
			string outputString = maskValue + inputString;
			return outputString;
		}

		private static string AddSuffixLeftFunction(string inputString, string maskValue)
		{
			string outputString = inputString + maskValue;
			return outputString;
		}

		private static string PadChar0LeftFunction(string inputString, string maskValue)
		{
			int width = 0;
			string outputString = inputString;
			bool ret = Int32.TryParse(maskValue, out width);
			if (ret == true)
			{
				if (width > 0)
				{
					outputString = inputString.PadLeft(width, '0');
				}
				else if (width < 0)
				{
					width = width * (-1);
					outputString = inputString.PadRight(width, '0');
				}
			}
			return outputString;
		}

		private static string FormatIntToStringFunction(string inputString, string maskValue)
		{
			string outputString = inputString;
			long outputLong;
			bool ret = Int64.TryParse(inputString, out outputLong);
			if (ret == true)
			{
				int inputLen = outputLong.ToString().Length;	   
				int maskLen = maskValue.Length;				 //длина маски
				string maskVal1 = maskValue.TrimEnd('0');
				int maskLen1 = maskVal1.Length;				//длина маски	без 0 в конце строки
				int maskLen2 = maskLen - maskLen1;		//колличество нулей в маске 
				if (maskLen2 >= inputLen)
				{
					outputString = outputLong.ToString(maskValue);
				}
			}
			return outputString;
		}

		private static string FormatStringToStringFunction(string inputString, string maskValue)
		{
			string outputString = inputString;
			int inputLen = inputString.Length;
			int maskLen = maskValue.Length;				 //длина маски

			//string maskVal1 = maskValue.TrimEnd('0');
			//int maskLen1 = maskVal1.Length;				//длина маски	без 0 в конце строки
			//int maskLen2 = maskLen - maskLen1;		//колличество нулей в маске 
			if (maskLen > inputLen)
			{
				int deltaLen = maskLen - inputLen;
				string mask1 = maskValue.Substring(0, deltaLen);
				outputString = mask1 + inputString;
			}
			return outputString;
		}

		private static string NoneFunction(string inputString, string maskValue)
		{
			string outputString = inputString;
			return outputString;
		}

		public static string MaskFormat(this string entity, MaskRecord maskRecord)
		{
			if (maskRecord == null) return entity;
			if (string.IsNullOrWhiteSpace(entity) == true) return "";
			return GetMaskTemplateDictionary()[maskRecord.MaskTemplateType].
				FormatString(entity, maskRecord.Value);
		}

	}
}
