using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using System.Reflection;
using Count4U.Model.ServiceContract.DataContract;
using Count4U.Model.ServiceContract;
using Count4U.Model.ServiceContract.Common;
using Common.Main;

namespace Count4U.Model.Service
{

	public class LogMessageService : ILogMessage
    {

		private string _error = String.Empty;
		private CodeOperationEnum _codeOperationEnum = CodeOperationEnum.CodeOperationUnknown;
		private string _codeBackOperation = String.Empty;
		private string _codeOperation = String.Empty;


		public Process4UBaseData Sip2LogInfo(Process4UBaseData message)
		{
			UiConnection.UpdateLogText("Request >> [" + message.Request + "]" + Environment.NewLine);
			UiConnection.UpdateLogText("Response << [" + message.Response + "]" + Environment.NewLine);
			if (string.IsNullOrWhiteSpace(message.Error) != true)
			{
				UiConnection.UpdateLogText("Response ERROR << [" + message.Error + "]" + Environment.NewLine);
			}
			else
			{
				if (message.Operation == "94")
				{
					if (string.IsNullOrWhiteSpace(message.Result) == false)
					{
						UiConnection.UpdateLogText("Opened new Session with SessionID = [" + message.Result + "]" + Environment.NewLine);
					}
				}

			}

			//UiConnection.UpdateOutputText("Request >> [" + message.Request + "]" + Environment.NewLine);
			UiConnection.UpdateOutputText("Response" + message.Operation + " << [" + message.Response + "]" + Environment.NewLine);
			return message;
		}

		public string Sip2InfoTitle(string message, string title)
		{
			if (string.IsNullOrWhiteSpace(title) == false)
			{
				UiConnection.UpdateLogText(message + Environment.NewLine);
			}
			else
			{
				UiConnection.UpdateLogText( title + " >> [" + message + "]" + Environment.NewLine);
			}
			return message;
		}


		public string Sip2LogInfoTime(string message, string time)
		{
			UiConnection.UpdateLogRequestTimeMessageText(message, time);
			return message + ":" + time;
		}

		public Process4UBaseData UpdateListView(Process4UBaseData request, Process4UBaseData response)
		{
			this.InitCode(request);
			UiConnection.ClearOutputText();
			if (request != null)
			{
				UiConnection.UpdateOutputText("Request" + request.Operation + " >> [" + request.Request + "]" + Environment.NewLine);
				//UiConnection.UpdateLogRequestMessageText(this._codeOperation + " Request : " + Environment.NewLine + OperationContext.Current.RequestContext.RequestMessage.ToString() + Environment.NewLine);
			//	UiConnection.UpdateLogRequestMessageText(this._codeOperation + " Request : " + Environment.NewLine + request.Result + Environment.NewLine);
				UiConnection.UpdateLogRequestMessageText(this._codeOperation + " Response : " + Environment.NewLine + response.Result + Environment.NewLine);


				Dictionary<string, string> propertyDictionary = this.FillFromRequestData(request, out this._error);
				UiConnection.UpdateListViewRequest(this._codeOperation, ConvertData.GetOperationName(this._codeOperationEnum), propertyDictionary);
				if (string.IsNullOrWhiteSpace(this._error) == false)
				{
					UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine);
					request.Error = request.Error + this._error;
				}
			}
			else
			{
				UiConnection.UpdateLogRequestMessageText(this._codeOperation + " Request object is null");
			}
			if (response != null)
			{
				if (string.IsNullOrWhiteSpace(response.Error) == false)
				{
					UiConnection.UpdateLogText("ERROR : " + response.Error + Environment.NewLine);
				}

				Dictionary<string, string> propertyDictionary1 = this.FillFromResponseData(response, out this._error);

				UiConnection.UpdateListViewResponse(this._codeBackOperation, ConvertData.GetOperationName(this._codeOperationEnum), propertyDictionary1);
				if (string.IsNullOrWhiteSpace(this._error) == false)
				{
					UiConnection.UpdateLogText("ERROR : " + this._error + Environment.NewLine);
					response.Error = response.Error + this._error;
				}
			}
			else
			{
				UiConnection.UpdateLogRequestMessageText(this._codeBackOperation + " Response object is null");
			}
			return response;
		}

		private Dictionary<string, string> FillFromRequestData(Process4UBaseData request, out string error)
		{
			string codeOperation = request.Operation;
			string _error = "";
			Dictionary<string, string> propertyDictionary = new Dictionary<string, string>();
			// обойти словарь из конфига и все данные по правилам сконкотенировать
			//UtilsSip2Protocol sip2Protocol = new UtilsSip2Protocol();
			//try
			//{
			//	XElement xElement = sip2Protocol.GetXDocumentOperation(codeOperation, out _error);
			//	Operation operation = new Operation(xElement);
			//	Fields fields = operation.fields;
			//	//надо перебрать весь  field из описания протокола и по рефлексии найти проперти (у request) с таким же имени
			//	PropertyInfo[] props = request.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
			//	//этому проперти (в словаре) присвоить данные 
			//	//которые распарсить по правилам в объекте 
			//	foreach (Field field in fields) // sip2Protocol
			//	{
			//		for (int i = 0; i < props.Length; i++) //requestData
			//		{
			//			if (props[i].Name.ToLower() == field.name.ToLower())
			//			{
			//				if (field.datacontract == "1")
			//				{
			//					propertyDictionary[props[i].Name.ToLower()] = (string)props[i].GetValue(request, null);
			//				}
			//			}
			//		}
			//	}
			//}
			//catch (Exception exp)
			//{
			//	_error = _error + " : " + exp.Message + "--" + exp.StackTrace;
			//}
			error = _error;
			return propertyDictionary;
		}

		private Dictionary<string, string> FillFromResponseData(Process4UBaseData response, out string error)
		{
			string codeOperation = response.Operation;
			string _error = "";
			Dictionary<string, string> propertyDictionary = new Dictionary<string, string>();
			// обойти словарь из конфига и все данные по правилам сконкотенировать
			//UtilsSip2Protocol sip2Protocol = new UtilsSip2Protocol();
			//try
			//{
			//	XElement xElement = sip2Protocol.GetXDocumentOperation(codeOperation, out _error);
			//	Operation operation = new Operation(xElement);
			//	Fields fields = operation.fields;
			//	//надо перебрать весь  field из описания протокола и по рефлексии найти проперти (у response) с таким же имени
			//	PropertyInfo[] props = response.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
			//	//этому проперти (в словаре) присвоить данные 
			//	//которые распарсить по правилам в объекте 
			//	foreach (Field field in fields) // sip2Protocol
			//	{
			//		for (int i = 0; i < props.Length; i++) //responseData
			//		{
			//			if (props[i].Name.ToLower() == field.name.ToLower())
			//			{
			//				if (field.datacontract == "1")
			//				{
			//					propertyDictionary[props[i].Name.ToLower()] = (string)props[i].GetValue(response, null);
			//				}
			//			}
			//		}
			//	}
			//}
			//catch (Exception exp)
			//{
			//	_error = _error + " : " + exp.Message + "--" + exp.StackTrace;
			//}
			error = _error;
			return propertyDictionary;
		}

		private void InitCode(Process4UBaseData request)
		{
			this._error = "";
			this._codeOperationEnum = CodeOperationEnum.CodeOperationUnknown;
			this._codeBackOperation = "";
			this._codeOperation = "";
			if (request == null) return;

			this._codeOperationEnum = ConvertData.GetCodeOperationEnum(request.Request);
			this._codeOperation = ConvertData.GetCodeOperationString(this._codeOperationEnum);
			this._codeBackOperation = ConvertData.GetCodeBackOperationString(this._codeOperationEnum);
			if (this._codeOperationEnum == CodeOperationEnum.CodeOperationUnknown) this._error = "Code Operation Unknown";
		}
	}
}
