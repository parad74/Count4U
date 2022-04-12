using System;
using System.Linq;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;
using Count4U.Model;
using Count4U.Model.Main;
using Count4U.Model.Audit;
using Count4U.Model.Count4U;
using System.Globalization;
using Count4U.Common.Constants;
using Zen.Barcode;
using System.Drawing.Imaging;
using System.IO;
using Count4U.Common.UserSettings;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface.Count4U;

namespace Count4U.GenerationReport
{
	public class ContextReportRepository : IContextReportRepository
	{
		private ContextReport _currentContextReport;
		private DateTimeFormatInfo _dtfi;
		public Dictionary<FilterAndSortEnum, string> _dictionaryFilterAndSort;
		private IUserSettingsManager  _userSettingsManager;
		private IServiceLocator _serviceLocator;

		 public ContextReportRepository(IUserSettingsManager userSettingsManager,
			 IServiceLocator serviceLocator)
		{
			this._userSettingsManager = userSettingsManager;
			this._serviceLocator = serviceLocator;
			this._currentContextReport = new ContextReport();
			this._dtfi = new DateTimeFormatInfo();
			this._dtfi.ShortDatePattern = @"dd/MM/yyyy";
			this._dtfi.ShortTimePattern = @"hh:mm:ss";
			this._dictionaryFilterAndSort = new Dictionary<FilterAndSortEnum, string>();
			this._dictionaryFilterAndSort[FilterAndSortEnum.QuantityDifferenceOriginalERP] = "QuantityDifferenceOriginalERP";
			this._dictionaryFilterAndSort[FilterAndSortEnum.ValueDifferenceOriginalERP] = "ValueDifferenceOriginalERP";
			this._dictionaryFilterAndSort[FilterAndSortEnum.QuantityDifference] = "QuantityDifference";
			this._dictionaryFilterAndSort[FilterAndSortEnum.ValueBuyDifference] = "ValueBuyDifference";
			this._dictionaryFilterAndSort[FilterAndSortEnum.QuantityEdit] = "QuantityEdit";
			this._dictionaryFilterAndSort[FilterAndSortEnum.ValueBuyEdit] = "ValueBuyEdit";
			this._dictionaryFilterAndSort[FilterAndSortEnum.SortByField] = "SortByField";
			this._dictionaryFilterAndSort[FilterAndSortEnum.FullFiterAndSort] = "FullFiterAndSort";
		}

		public DateTimeFormatInfo Dtfi
		{
			get { return _dtfi; }
			set { _dtfi = value; }
		}

		public ContextReport GetNewContextReport()
		{
			this._currentContextReport = new ContextReport();
			return this._currentContextReport;
		}

		public void InitContextReport()
		{
			//char CurrencyGet();
			char charShekel = this._userSettingsManager.CurrencyGet();	//'\u20AA';
			const string LTRMark = "\u200E";
			//Localization.Resources.Bit_MessageTypeCaption_Trace;
			this._currentContextReport.Value1 = charShekel.ToString();
			this._currentContextReport.Value2 = LTRMark;
			this._currentContextReport.CurrentCurrency = charShekel.ToString();
			this._currentContextReport.TradeMark = "COUNT4U - Nextline Ltd";
			string appNameReport = this._userSettingsManager.ReportAppNameGet();
			this._currentContextReport.AppNameReport = appNameReport;


			//this._currentContextReport.IturPrefix = this._userSettingsManager.IturNamePrefixGet();
		}

		public void InitContextReport(Dictionary<FilterAndSortEnum, string> dictionaryFilterAndSort)
		{
			this._currentContextReport.QuantityDifferenceERPLess = -100;
			this._currentContextReport.ValueDifferenceERPLess = -100;

			this._currentContextReport.QuantityDifferenceERPMore = 100;
			this._currentContextReport.ValueDifferenceERPMore = 100;

			if (dictionaryFilterAndSort == null) return;
			//dictionaryFilterAndSort = _dictionaryFilterAndSort; //TODO

			this._currentContextReport.QuantityDifferenceOriginalERP =
				dictionaryFilterAndSort.TryGetFilterOrSortValue(FilterAndSortEnum.QuantityDifferenceOriginalERP);
			this._currentContextReport.ValueDifferenceOriginalERP = 
				dictionaryFilterAndSort.TryGetFilterOrSortValue(FilterAndSortEnum.ValueDifferenceOriginalERP);
			this._currentContextReport.QuantityDifference =
				dictionaryFilterAndSort.TryGetFilterOrSortValue(FilterAndSortEnum.QuantityDifference);
			this._currentContextReport.ValueBuyDifference =
				dictionaryFilterAndSort.TryGetFilterOrSortValue(FilterAndSortEnum.ValueBuyDifference);
			this._currentContextReport.QuantityEdit =
				dictionaryFilterAndSort.TryGetFilterOrSortValue(FilterAndSortEnum.QuantityEdit);
			this._currentContextReport.ValueBuyEdit =
				dictionaryFilterAndSort.TryGetFilterOrSortValue(FilterAndSortEnum.ValueBuyEdit);
			this._currentContextReport.SortByField = 
				dictionaryFilterAndSort.TryGetFilterOrSortValue(FilterAndSortEnum.SortByField);
			this._currentContextReport.FullFiterAndSort = 
				dictionaryFilterAndSort.TryGetFilterOrSortValue(FilterAndSortEnum.FullFiterAndSort);

				}

	
		public void InitContextReport(Customer customer)
		{
			if (customer == null) return;
			this._currentContextReport.CustomerCode = customer.Code;
			this._currentContextReport.CustomerContactPerson = customer.ContactPerson;
			this._currentContextReport.CustomerDescription = customer.Description;

			if (string.IsNullOrWhiteSpace(customer.Description) != true)
			{
				string[] descriptionLines = customer.Description.Split('>');
				string descriptions = descriptionLines.JoinRecord(Environment.NewLine);
				this._currentContextReport.InventorDescription = descriptions;
			}

			this._currentContextReport.CustomerFax = customer.Fax;
			this._currentContextReport.CustomerMail = customer.Mail;
			this._currentContextReport.CustomerName = customer.Name;
			this._currentContextReport.CustomerPhone = customer.Phone;
			this._currentContextReport.CustomerDBPath = customer.DBPath;
			this._currentContextReport.CustomerLogoPath = customer.LogoPath;

			this._currentContextReport.CustomerTag = customer.Tag;
			this._currentContextReport.CustomerTag1 = customer.Tag1;
			this._currentContextReport.CustomerTag2 = customer.Tag2;
			this._currentContextReport.CustomerTag3 = customer.Tag3;

		}


		public void InitContextReport(Branch baranch)
		{
			if (baranch == null) return;
			this._currentContextReport.BranchCode = baranch.Code;
			this._currentContextReport.BranchAddress = baranch.Address;
			this._currentContextReport.BranchContactPerson = baranch.ContactPerson;
			this._currentContextReport.BranchDescription = baranch.Description;

			if (string.IsNullOrWhiteSpace(baranch.Description) != true)
			{
				string[] descriptionLines = baranch.Description.Split('>');
				string descriptions = descriptionLines.JoinRecord(Environment.NewLine);
				this._currentContextReport.InventorDescription = descriptions;
			}

			this._currentContextReport.BranchFax = baranch.Fax;
			this._currentContextReport.BranchLogoFile = baranch.LogoFile;
			this._currentContextReport.BranchMail = baranch.Mail;
			this._currentContextReport.BranchName = baranch.Name;
			this._currentContextReport.BranchPhone = baranch.Phone;
			this._currentContextReport.BranchDBPath = baranch.DBPath;
			this._currentContextReport.BranchCodeLocal = baranch.BranchCodeLocal;
			this._currentContextReport.BranchCodeERP = baranch.BranchCodeERP;
			this._currentContextReport.BranchTag = baranch.Tag;
			this._currentContextReport.BranchTag1 = baranch.Tag1;
			this._currentContextReport.BranchTag2 = baranch.Tag2;
			this._currentContextReport.BranchTag3 = baranch.Tag3;

			try
			{
				string branchCode = this._currentContextReport.BranchCode.Replace(this._currentContextReport.CustomerCode, "");
				this._currentContextReport.Value10 = branchCode.Replace("-", ""); ;
			}
			catch { }

		}

		public void InitContextReport(Inventor inventor)
		{
			if (inventor == null) return;
			this._currentContextReport.InventorCode = inventor.Code;
			this._currentContextReport.InventorName = inventor.Name;

			if (string.IsNullOrWhiteSpace(inventor.Description) != true)
			{
				string[] descriptionLines = inventor.Description.Split('>');
				string descriptions = descriptionLines.JoinRecord(Environment.NewLine);
				this._currentContextReport.InventorDescription = descriptions;
			}

			if (inventor.InventorDate != null)
			{
				this._currentContextReport.InventorDate = inventor.InventorDate.ToString(this._dtfi);
				this._currentContextReport.InventorDDMM = inventor.InventorDate.ToString("ddMM");
				this._currentContextReport.InventorYYYY = inventor.InventorDate.Year.ToString();
			}
			this._currentContextReport.InventorStatus = inventor.Status;
			this._currentContextReport.InventorStatusCode = inventor.StatusCode;
			this._currentContextReport.InventorDBPath = inventor.DBPath;

			this._currentContextReport.InventorTag = inventor.Tag;
			this._currentContextReport.InventorTag1 = inventor.Tag1;
			this._currentContextReport.InventorTag2 = inventor.Tag2;
			this._currentContextReport.InventorTag3 = inventor.Tag3;

		}

		public void InitContextReport(Itur itur)
		{
			//this._currentContextReport.IturBarcode = this.CreateBarcode("12345");	
			if (itur == null) return;
			this._currentContextReport.IturCode = itur.IturCode;
			this._currentContextReport.ERPIturCode = itur.ERPIturCode;
			string iturName = itur.Name;
			string barcodePrefix = this._userSettingsManager.BarcodePrefixGet(); //@"%L%"
			this._currentContextReport.IturBarcode = this.CreateBarcode(barcodePrefix + itur.IturCode);	
			string iturNamePrefix = this._userSettingsManager.IturNamePrefixGet();
			this._currentContextReport.IturPrefix = iturNamePrefix;
			

			if (string.IsNullOrWhiteSpace(iturName) == true)
			{
				iturName = iturNamePrefix + itur.NumberPrefix + "-" + itur.NumberSufix;
			}
			this._currentContextReport.IturName = iturName;
			

			if (string.IsNullOrWhiteSpace(itur.ERPIturCode) == false)
			{
				if (this._userSettingsManager.LanguageGet() == enLanguage.Hebrew)
				{
					this._currentContextReport.IturNameFull = itur.ERPIturCode + " \\ " + iturName;
				}
				else
				{
					this._currentContextReport.IturNameFull = iturName + " \\ " + itur.ERPIturCode;
				}
			}
			else
			{
				this._currentContextReport.IturNameFull = iturName;
			}

			this._currentContextReport.IturNumber = itur.Number.ToString();
			if (itur.StatusIturBit != null)
			{
				this._currentContextReport.IturStatusIturBit = itur.StatusIturBit.ToString();
			}
		
			if (itur.StatusIturGroupBit != null)
			{
				this._currentContextReport.IturStatusIturGroupBit = itur.StatusIturGroupBit.ToString();
			}
			this._currentContextReport.IturNumberPrefix = itur.NumberPrefix;
			this._currentContextReport.IturNumberSufix = itur.NumberSufix;
		}

		public void InitContextReport(Location location)
		{
			if (location == null) return;
			this._currentContextReport.LocationCode = location.Code;
			this._currentContextReport.LocationName = location.Name;
			//string barcodePrefix = this._userSettingsManager.BarcodePrefixGet();
			//this._currentContextReport.LocationBarcode = this.CreateBarcode(barcodePrefix + location.Code);
			this._currentContextReport.LocationBarcode = this.CreateBarcode(location.Code);	
		}

		public void InitContextReport(string pathDB, Device device)
		{
			IDocumentHeaderRepository docRepository = this._serviceLocator.GetInstance<IDocumentHeaderRepository>();
			//if (device == null) return;
			////this._currentContextReport.DeviceCode = device.DeviceCode;
			//this._currentContextReport.StartInventorDateTime = device.StartInventorDateTime.ToShortDateString() + " " + device.StartInventorDateTime.ToShortTimeString() ;
			//this._currentContextReport.EndInventorDateTime = device.EndInventorDateTime.ToShortDateString() + " " + device.EndInventorDateTime.ToShortTimeString();
			DateTime startInventorDateTime = this._userSettingsManager.StartInventorDateTimeGet();
			DateTime endInventorDateTime = this._userSettingsManager.EndInventorDateTimeGet();
			this._currentContextReport.StartInventorDateTime = startInventorDateTime.ToShortDateString() + " " + startInventorDateTime.ToShortTimeString();
			this._currentContextReport.EndInventorDateTime = endInventorDateTime.ToShortDateString() + " " + endInventorDateTime.ToShortTimeString();


			{
				TimeSpan fromStartToEnd = (TimeSpan)(endInventorDateTime - startInventorDateTime);
				long ticksTimeSpan1 = 0;
				string periodFromStartToEnd = "00:00";
				int periodFromStartToEndSumMin = 0;
				int periodFromStartToEndSumSec = 0;
				try
				{
					int periodFromStartToEndDays = 0;
					int periodFromStartToEndH = 0;
					int periodFromStartToEndMin = 0;
					int periodFromStartToEndSec = 0;
					ticksTimeSpan1 = fromStartToEnd.Ticks;
					periodFromStartToEndDays = fromStartToEnd.Days;
					periodFromStartToEndH = fromStartToEnd.Hours;
					periodFromStartToEndMin = fromStartToEnd.Minutes;
					periodFromStartToEndSec = fromStartToEnd.Seconds;
					periodFromStartToEnd = (periodFromStartToEndDays * 24 + periodFromStartToEndH).ToString().PadLeft(2, '0') + ":" +
															periodFromStartToEndMin.ToString().PadLeft(2, '0');
														//	 + ":" + periodFromStartToEnd.ToString().PadLeft(2, '0'); //fromFirstToLast.ToString(@"hh\:mm\:ss");  
					periodFromStartToEndSumMin = (periodFromStartToEndDays * 24 + periodFromStartToEndH) * 60 + periodFromStartToEndMin;

					periodFromStartToEndSumSec = ((periodFromStartToEndDays * 24 + periodFromStartToEndH) * 60 + periodFromStartToEndMin) *60
						+ periodFromStartToEndSec;

				}
				catch { }

				//this._currentContextReport.StartEndWorkDuration = periodFromStartToEnd;
				if (periodFromStartToEndSumMin == 0) periodFromStartToEndSumMin = 1;
				this._currentContextReport.StartEndInventorDurationSumMin = periodFromStartToEndSumMin;

				if (periodFromStartToEndSumSec == 0) periodFromStartToEndSumSec = 1;
				this._currentContextReport.StartEndInventorDurationSumSec = periodFromStartToEndSumSec;
			}
			 //==============
			DocumentHeaders documentHeaders = docRepository.GetDocumentHeaders(pathDB);


			if (documentHeaders != null)
			{
				DateTime endwork = documentHeaders.Max(x => (DateTime)x.ToTime);
				DateTime startwork = documentHeaders.Min(x => (DateTime)x.FromTime);
			
				this._currentContextReport.StartWorkDateTime = startwork.ToString(@"dd/MM/yyyy HH:mm:ss");
				this._currentContextReport.EndWorkDateTime = endwork.ToString(@"dd/MM/yyyy HH:mm:ss");

				TimeSpan fromFirstToLast = (TimeSpan)(endwork - startwork);
				long ticksTimeSpan = 0;
				string periodFromFirstToLast = "00:00:00";
				int periodFromFirstToLastSumMin = 0;
				int periodFromFirstToLastSumSec = 0;
				try
				{
					int periodFromFirstToLastDays = 0;
					int periodFromFirstToLastH = 0;
					int periodFromFirstToLastMin = 0;
					int periodFromFirstToLastSec = 0;
					ticksTimeSpan = fromFirstToLast.Ticks;
					periodFromFirstToLastDays = fromFirstToLast.Days;
					periodFromFirstToLastH = fromFirstToLast.Hours;
					periodFromFirstToLastMin = fromFirstToLast.Minutes;
					periodFromFirstToLastSec = fromFirstToLast.Seconds;
					periodFromFirstToLast = (periodFromFirstToLastDays * 24 + periodFromFirstToLastH).ToString().PadLeft(2, '0') + ":" +
															periodFromFirstToLastMin.ToString().PadLeft(2, '0')
															 + ":" +periodFromFirstToLastSec.ToString().PadLeft(2, '0'); //fromFirstToLast.ToString(@"hh\:mm\:ss");  
					periodFromFirstToLastSumMin = (periodFromFirstToLastDays * 24 + periodFromFirstToLastH) * 60 + periodFromFirstToLastMin;

					periodFromFirstToLastSumSec = ((periodFromFirstToLastDays * 24 + periodFromFirstToLastH) * 60 + periodFromFirstToLastMin) *60
																																														+ periodFromFirstToLastSec;

				}
				catch { }

				this._currentContextReport.StartEndWorkDuration = periodFromFirstToLast;
				if (periodFromFirstToLastSumMin == 0) periodFromFirstToLastSumMin = 1;
				this._currentContextReport.StartEndWorkDurationSumMin = periodFromFirstToLastSumMin;

				if (periodFromFirstToLastSumSec == 0) periodFromFirstToLastSumSec = 1;
				this._currentContextReport.StartEndWorkDurationSumSec = periodFromFirstToLastSumSec;


			}
			
		}


		public void InitContextReport(DocumentHeader doc)
		{
			if (doc == null) return;
			this._currentContextReport.DocumentHeaderCode = doc.DocumentCode;
			this._currentContextReport.DocumentCode = doc.DocumentCode;

			if (doc.DocNum != null)
			{
				this._currentContextReport.DocNum = doc.DocNum.ToString();
			}
			this._currentContextReport.DocumentHeaderName = doc.Name;
			this._currentContextReport.WorkerGUID = doc.WorkerGUID;
			if (doc.StatusDocHeaderBit != null)
			{
				this._currentContextReport.DocStatusDocHeaderBit = doc.StatusDocHeaderBit.ToString();
			}
			if (doc.StatusApproveBit != null)
			{
				this._currentContextReport.DocStatusApproveBit = doc.StatusApproveBit.ToString();
			}
		}

		public void InitContextReport(string pathDB, string param1, string param2, string param3, string reportCode = "")
		{
			if (reportCode == null) reportCode = "";
			this._currentContextReport.Value3 = param1;
			this._currentContextReport.Value4 = param2;
			this._currentContextReport.Value5 = param3;
			if (reportCode == "[Rep-IS1-85L]")
			{
				try
				{
					this._currentContextReport.Value4 = GetLocationNames(param1, pathDB);
				}
				catch { }
			}


	}

		public ContextReports GetContextReports()
		{
			ContextReports contextReports = new ContextReports();
			contextReports.Add(this._currentContextReport);
			return contextReports;
		}

		public string GetLocationNames(string locatonCodes, string pathDB )
		{
			List<string> list = new List<string>();
			string[] locationCodes = locatonCodes.Split(',');
			ILocationRepository locationRepository = this._serviceLocator.GetInstance<ILocationRepository>();

			foreach (string locationCode in locationCodes)
			{
				Location location = locationRepository.GetLocationByCode(locationCode, pathDB);
				if (location != null) list.Add(location.Name);
			}
			return list.JoinRecord(",");
		}

		public ContextReports GetIturListContextReports(string pathDB, ContextReport contextReport = null)
		{
			if (contextReport == null) contextReport = this._currentContextReport;
			ContextReports contextReports = new ContextReports();
			IIturRepository iturRepository =	this._serviceLocator.GetInstance<IIturRepository>();
			Iturs iturs = iturRepository.GetIturs(pathDB);
			string barcodePrefix = this._userSettingsManager.BarcodePrefixGet(); //@"%L%"
			foreach (Itur itur in iturs)
			{
				if (itur != null)
				{
					contextReport.IturCode = itur.IturCode;
					contextReport.LocationCode = itur.LocationCode;
					contextReport.IturBarcode = this.CreateBarcode(barcodePrefix + itur.IturCode);
					//contextReport.LocationBarcode = this.CreateBarcode(barcodePrefix + itur.LocationCode);	
					contextReports.Add(contextReport);
				}
			}
			return contextReports;
		}

		public void Clear()
		{
			this._currentContextReport.Clear();
		}

		public byte[] CreateBarcode(String barcode)
		{
 //           Dim bdf As Code128BarcodeDraw = BarcodeDrawFactory.Code128WithChecksum
 //PictureBox1.Image = bdf.Draw("Hello world!", 20)
			//BarcodeMetrics tamccbb = new BarcodeMetrics(2, 90);
			System.Drawing.Image imagen;

			string  barcodeType = this._userSettingsManager.BarcodeTypeGet();
			var barcodeTypeEnum = (BarcodeSymbology)System.Enum.Parse(typeof(BarcodeSymbology), barcodeType.Trim());
			imagen = BarcodeDrawFactory.GetSymbology(barcodeTypeEnum).Draw(barcode, 30);
			//imagen = BarcodeDrawFactory.GetSymbology(BarcodeSymbology.Code39NC).Draw(barcode, 20);	   
			ImageFormat format = ImageFormat.Bmp;

			MemoryStream mm = new MemoryStream();
			imagen.Save(mm, format);
			imagen.Dispose();

			byte[] bytearray = mm.ToArray();
			mm.Close();
			mm.Dispose();

			return bytearray;
		}
	}

	

	public static class FilterOrSortValue
	{
		public static string TryGetFilterOrSortValue(this Dictionary<FilterAndSortEnum, string> dictionaryFilterAndSort, FilterAndSortEnum filterOrSortKey)
		{
			string filterOrSortValue = "";
			bool ret = dictionaryFilterAndSort.TryGetValue(filterOrSortKey, out filterOrSortValue);
			return filterOrSortValue;
		}
	}
	
}
