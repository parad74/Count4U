using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Count4U.Validate
{
	public static class Bit2List
	{
		public static List<string> GetStatusList(int bit, DomainStatusEnum domainType)
		{
			List<string> statusList = new List<string>();
			switch (domainType)
			{
				case DomainStatusEnum.Itur:
					return IturValidate.Bit2WarningMessage(bit);
				//case DomainStatusEnum.IturDoc:
				//    return IturDocValidate.ConvertIturStatusBit2Message(bit);
				case DomainStatusEnum.Doc:
					return DocumentHeaderValidate.Bit2WarningMessage(bit);
				case DomainStatusEnum.PDA:
					return InventProductValidate.Bit2WarningMessage(bit);
			}
			return statusList;
		}

		public static List<string> GetApproveList(int bit)
		{
			List<string> list = new List<string>();
			IturStatusEnum ret = IturStatusEnum.NoOneDoc;
			try
			{
				ret = (IturStatusEnum)bit;
				list.Add(ret.ToString());
			}
			catch { }
			return list;
		}

		public static List<string> GetStatusGroupList(int bit)
		{
			List<string> list = new List<string>();
			IturStatusGroupEnum ret = IturStatusGroupEnum.Empty;
			try
			{
				ret = (IturStatusGroupEnum)bit;
				list.Add(ret.ToString());
			}
			catch { }
			return list;
		}
	}
}
