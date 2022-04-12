using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model
{
	public class BitAndRecord
	{
		public int Bit { get; set; }
		public string Record { get; set; }
		public MessageTypeEnum ErrorType { get; set; }
	
		public static List<ConvertDataErrorCodeEnum> Bit2ConvertDataErrorCodeEnumList(int bit)
		{
			List<ConvertDataErrorCodeEnum> ret = new List<ConvertDataErrorCodeEnum>();
			List<int> splitBits = BitStatus.SplitBit(bit);
			foreach (int splitBit in splitBits)
			{
				try
				{
					ret.Add((ConvertDataErrorCodeEnum)splitBit);
				}
				catch { }
			}
			return ret;
		}

		public static string ConvertDataErrorCodeEnumToString(ConvertDataErrorCodeEnum status)
		{
			switch (status)
			{
				case ConvertDataErrorCodeEnum.InvalidValue:
					return ConvertDataErrorMessage.InvalidValue;
				case ConvertDataErrorCodeEnum.FKCodeIsEmpty:
					return ConvertDataErrorMessage.FKCodeIsEmpty;
				case ConvertDataErrorCodeEnum.SameCodeExist:
					return ConvertDataErrorMessage.SameCodeExist;
				case ConvertDataErrorCodeEnum.CodeIsEmpty:
					return ConvertDataErrorMessage.CodeIsEmpty;
			}
			return "";
		}
	}		// end class BitAndRecord

	
}
