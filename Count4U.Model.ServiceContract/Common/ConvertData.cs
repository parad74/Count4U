using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Count4U.Model.ServiceContract.Common
{
	public class ConvertData
	{
		public static long ID = 0;

		public static string GetDateString()
		{
			DateTime dt = DateTime.Now;
			string currentDateTime = dt.ToString("yyyyMMdd") + "    " + dt.ToString("HHmmss");
			return currentDateTime;
		}

		//public static string GetDateShotString()
		//{
		//	DateTime dt = DateTime.Now;
		//	string currentDateTime = dt.ToString("yyyyMMdd");
		//	return currentDateTime;
		//}

		public static string GetDateShotString(string propertyValue)
		{
			DateTime dt = DateTime.Now;
			//DD/MM/YYYY
			bool ret = DateTime.TryParse(propertyValue, out dt);
			if (ret == false) dt = DateTime.Now;
			string currentDateTime = dt.ToString("dd") + @"/" +   dt.ToString("MM")+ @"/" + dt.ToString("yyyy") ;
			return currentDateTime;
		}

		

		public static string GetCodeOperation(string request)
		{
			string codeOperation = "";
			if (string.IsNullOrWhiteSpace(request) != true)
			{
				if (request.Length > 1)
				{
					codeOperation = request.Substring(0, 2);
				}
			}
			return codeOperation;
		}


		public static CodeOperationEnum GetCodeOperationEnum(string request)
		{
			if (string.IsNullOrWhiteSpace(request) == true) return CodeOperationEnum.CodeOperationUnknown;
			string codeOperation = GetCodeOperation(request);
			if (string.IsNullOrWhiteSpace(codeOperation) == true) return CodeOperationEnum.CodeOperationUnknown;
			
			if (codeOperation == "99") return CodeOperationEnum.CodeOperation99;
			else if (codeOperation == "98") return CodeOperationEnum.CodeOperation98;
			else if (codeOperation == "63") return CodeOperationEnum.CodeOperation63;
			else if (codeOperation == "64") return CodeOperationEnum.CodeOperation64;
			else if (codeOperation == "09") return CodeOperationEnum.CodeOperation09;
			else if (codeOperation == "10") return CodeOperationEnum.CodeOperation10;
			else if (codeOperation == "11") return CodeOperationEnum.CodeOperation11;
			else if (codeOperation == "12") return CodeOperationEnum.CodeOperation12;
			else if (codeOperation == "17") return CodeOperationEnum.CodeOperation17;
			else if (codeOperation == "18") return CodeOperationEnum.CodeOperation18;
			else if (codeOperation == "29") return CodeOperationEnum.CodeOperation29;
			else if (codeOperation == "30") return CodeOperationEnum.CodeOperation30;
			else if (codeOperation == "35") return CodeOperationEnum.CodeOperation35;
			else if (codeOperation == "36") return CodeOperationEnum.CodeOperation36;
			else if (codeOperation == "37") return CodeOperationEnum.CodeOperation37;
			else if (codeOperation == "38") return CodeOperationEnum.CodeOperation38;
			else if (codeOperation == "93") return CodeOperationEnum.CodeOperation93;
			else if (codeOperation == "94") return CodeOperationEnum.CodeOperation94;
			else if (codeOperation == "101") return CodeOperationEnum.CodeOperation101;
			else if (codeOperation == "102") return CodeOperationEnum.CodeOperation102;
			else if (codeOperation == "103") return CodeOperationEnum.CodeOperation103;
			else if (codeOperation == "104") return CodeOperationEnum.CodeOperation104;
			else if (codeOperation == "105") return CodeOperationEnum.CodeOperation105;
			else if (codeOperation == "106") return CodeOperationEnum.CodeOperation106;
			else if (codeOperation == "107") return CodeOperationEnum.CodeOperation107;
			else if (codeOperation == "108") return CodeOperationEnum.CodeOperation108;
			else if (codeOperation == "109") return CodeOperationEnum.CodeOperation109;
			else if (codeOperation == "110") return CodeOperationEnum.CodeOperation110;
			else return CodeOperationEnum.CodeOperationUnknown;

		}
		public static string GetCodeOperationString(CodeOperationEnum codeOperationEnum)
		{
			switch (codeOperationEnum)
			{
				case CodeOperationEnum.CodeOperation99:
					return "99";
				case CodeOperationEnum.CodeOperation98:
					return "98";
				case CodeOperationEnum.CodeOperation63:
					return "63";
				case CodeOperationEnum.CodeOperation64:
					return "64";
				case CodeOperationEnum.CodeOperation09:
					return "09";
				case CodeOperationEnum.CodeOperation10:
					return "10";
				case CodeOperationEnum.CodeOperation11:
					return "11";
				case CodeOperationEnum.CodeOperation12:
					return "12";
				case CodeOperationEnum.CodeOperation17:
					return "17";
				case CodeOperationEnum.CodeOperation18:
					return "18";
				case CodeOperationEnum.CodeOperation29:
					return "29";
				case CodeOperationEnum.CodeOperation30:
					return "30";
				case CodeOperationEnum.CodeOperation35:
					return "35";
				case CodeOperationEnum.CodeOperation36:
					return "36";
				case CodeOperationEnum.CodeOperation37:
					return "37";
				case CodeOperationEnum.CodeOperation38:
					return "38";
				case CodeOperationEnum.CodeOperation93:
					return "93";
				case CodeOperationEnum.CodeOperation94:
					return "94";
				case CodeOperationEnum.CodeOperation101:
					return "101";
				case CodeOperationEnum.CodeOperation102:
					return "102";
				case CodeOperationEnum.CodeOperation103:
					return "103";
				case CodeOperationEnum.CodeOperation104:
					return "104";
				case CodeOperationEnum.CodeOperation105:
					return "105";
				case CodeOperationEnum.CodeOperation106:
					return "106";
				case CodeOperationEnum.CodeOperation107:
					return "107";
				case CodeOperationEnum.CodeOperation108:
					return "108";
				case CodeOperationEnum.CodeOperation109:
					return "109";
				case CodeOperationEnum.CodeOperation110:
					return "110";
				default:
					return "";

			}

		}

		public static string GetOperationName(CodeOperationEnum codeOperationEnum)
		{
			switch (codeOperationEnum)
			{
				case CodeOperationEnum.CodeOperation99:
					return "SC Status";
				case CodeOperationEnum.CodeOperation98:
					return "SC Status";
				case CodeOperationEnum.CodeOperation63:
					return "Patron Information";
				case CodeOperationEnum.CodeOperation64:
					return "Patron Information";
				case CodeOperationEnum.CodeOperation09:
					return "Checkin";
				case CodeOperationEnum.CodeOperation10:
					return "Checkin";
				case CodeOperationEnum.CodeOperation11:
					return "Checkout";
				case CodeOperationEnum.CodeOperation12:
					return "Checkout";
				case CodeOperationEnum.CodeOperation17:
					return "Item Information";
				case CodeOperationEnum.CodeOperation18:
					return "Item Information";
				case CodeOperationEnum.CodeOperation29:
					return "Renew";
				case CodeOperationEnum.CodeOperation30:
					return "Renew";
				case CodeOperationEnum.CodeOperation35:
					return "End Patron Session";
				case CodeOperationEnum.CodeOperation36:
					return "End Patron Session";
				case CodeOperationEnum.CodeOperation37:
					return "Fee Paid";
				case CodeOperationEnum.CodeOperation38:
					return "Fee Paid";
				case CodeOperationEnum.CodeOperation93:
					return "SIP2 Login";
				case CodeOperationEnum.CodeOperation94:
					return "SIP2 Login";
				case CodeOperationEnum.CodeOperation101:
					return "connect";
				case CodeOperationEnum.CodeOperation102:
					return "connect";
				case CodeOperationEnum.CodeOperation103:
					return "disconnect";
				case CodeOperationEnum.CodeOperation104:
					return "disconnect";
				case CodeOperationEnum.CodeOperation105:
					return "get_items";
				case CodeOperationEnum.CodeOperation106:
					return "get_items";
				case CodeOperationEnum.CodeOperation107:
					return "turn_security_on";
				case CodeOperationEnum.CodeOperation108:
					return "turn_security_on";
				case CodeOperationEnum.CodeOperation109:
					return "turn_security_off";
				case CodeOperationEnum.CodeOperation110:
					return "turn_security_off";
				default:
					return "";

			}
		}

		public static string GetOperationType(CodeOperationEnum codeOperationEnum)
		{
			switch (codeOperationEnum)
			{
				case CodeOperationEnum.CodeOperation99:
					return "request";
				case CodeOperationEnum.CodeOperation98:
					return "response";
				case CodeOperationEnum.CodeOperation63:
					return "request";
				case CodeOperationEnum.CodeOperation64:
					return "response";
				case CodeOperationEnum.CodeOperation09:
					return "request";
				case CodeOperationEnum.CodeOperation10:
					return "response";
				case CodeOperationEnum.CodeOperation11:
					return "request";
				case CodeOperationEnum.CodeOperation12:
					return "response";
				case CodeOperationEnum.CodeOperation17:
					return "request";
				case CodeOperationEnum.CodeOperation18:
					return "response";
				case CodeOperationEnum.CodeOperation29:
					return "request";
				case CodeOperationEnum.CodeOperation30:
					return "response";
				case CodeOperationEnum.CodeOperation35:
					return "request";
				case CodeOperationEnum.CodeOperation36:
					return "response";
				case CodeOperationEnum.CodeOperation37:
					return "request";
				case CodeOperationEnum.CodeOperation38:
					return "response";
				case CodeOperationEnum.CodeOperation93:
					return "request";
				case CodeOperationEnum.CodeOperation94:
					return "response";
				case CodeOperationEnum.CodeOperation101:
					return "request";
				case CodeOperationEnum.CodeOperation102:
					return "response";
				case CodeOperationEnum.CodeOperation103:
					return "request";
				case CodeOperationEnum.CodeOperation104:
					return "response";
				case CodeOperationEnum.CodeOperation105:
					return "request";
				case CodeOperationEnum.CodeOperation106:
					return "response";
				case CodeOperationEnum.CodeOperation107:
					return "request";
				case CodeOperationEnum.CodeOperation108:
					return "response";
				case CodeOperationEnum.CodeOperation109:
					return "request";
				case CodeOperationEnum.CodeOperation110:
					return "response";
				default:
					return "";

			}
		}

		public static string GetCodeBackOperationString(CodeOperationEnum codeOperationEnum)
		{
			switch (codeOperationEnum)
			{
				case CodeOperationEnum.CodeOperation99:
					return "98";
				case CodeOperationEnum.CodeOperation98:
					return "99";
				case CodeOperationEnum.CodeOperation63:
					return "64";
				case CodeOperationEnum.CodeOperation64:
					return "63";
				case CodeOperationEnum.CodeOperation09:
					return "10";
				case CodeOperationEnum.CodeOperation10:
					return "09";
				case CodeOperationEnum.CodeOperation11:
					return "12";
				case CodeOperationEnum.CodeOperation12:
					return "11";
				case CodeOperationEnum.CodeOperation17:
					return "18";
				case CodeOperationEnum.CodeOperation18:
					return "17";
				case CodeOperationEnum.CodeOperation29:
					return "30";
				case CodeOperationEnum.CodeOperation30:
					return "29";
				case CodeOperationEnum.CodeOperation35:
					return "36";
				case CodeOperationEnum.CodeOperation36:
					return "35";
				case CodeOperationEnum.CodeOperation37:
					return "38";
				case CodeOperationEnum.CodeOperation38:
					return "37";
				case CodeOperationEnum.CodeOperation93:
					return "94";
				case CodeOperationEnum.CodeOperation94:
					return "93";
				case CodeOperationEnum.CodeOperation101:
					return "101";
				case CodeOperationEnum.CodeOperation102:
					return "102";
				case CodeOperationEnum.CodeOperation103:
					return "103";
				case CodeOperationEnum.CodeOperation104:
					return "104";
				case CodeOperationEnum.CodeOperation105:
					return "105";
				case CodeOperationEnum.CodeOperation106:
					return "106";
				case CodeOperationEnum.CodeOperation107:
					return "107";
				case CodeOperationEnum.CodeOperation108:
					return "108";
				case CodeOperationEnum.CodeOperation109:
					return "109";
				case CodeOperationEnum.CodeOperation110:
					return "110";
				default:
					return "";

			}

		}

		public static int GetCodeOperationInt(CodeOperationEnum codeOperationEnum)
		{
			switch (codeOperationEnum)
			{
				case CodeOperationEnum.CodeOperation99:
					return 99;
				case CodeOperationEnum.CodeOperation98:
					return 98;
				case CodeOperationEnum.CodeOperation63:
					return 63;
				case CodeOperationEnum.CodeOperation64:
					return 64;
				case CodeOperationEnum.CodeOperation09:
					return 9;
				case CodeOperationEnum.CodeOperation10:
					return 10;
				case CodeOperationEnum.CodeOperation11:
					return 11;
				case CodeOperationEnum.CodeOperation12:
					return 12;
				case CodeOperationEnum.CodeOperation17:
					return 17;
				case CodeOperationEnum.CodeOperation18:
					return 18;
				case CodeOperationEnum.CodeOperation29:
					return 29;
				case CodeOperationEnum.CodeOperation30:
					return 30;
				case CodeOperationEnum.CodeOperation35:
					return 35;
				case CodeOperationEnum.CodeOperation36:
					return 36;
				case CodeOperationEnum.CodeOperation37:
					return 37;
				case CodeOperationEnum.CodeOperation38:
					return 38;
				case CodeOperationEnum.CodeOperation93:
					return 93;
				case CodeOperationEnum.CodeOperation94:
					return 94;
				case CodeOperationEnum.CodeOperation101:
					return 101;
				case CodeOperationEnum.CodeOperation102:
					return 102;
				case CodeOperationEnum.CodeOperation103:
					return 103;
				case CodeOperationEnum.CodeOperation104:
					return 104;
				case CodeOperationEnum.CodeOperation105:
					return 105;
				case CodeOperationEnum.CodeOperation106:
					return 106;
				case CodeOperationEnum.CodeOperation107:
					return 107;
				case CodeOperationEnum.CodeOperation108:
					return 108;
				case CodeOperationEnum.CodeOperation109:
					return 109;
				case CodeOperationEnum.CodeOperation110:
					return 110;
				default:
					return -1;

			}
		}
	}
}
