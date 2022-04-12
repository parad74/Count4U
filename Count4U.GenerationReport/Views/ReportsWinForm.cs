using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Practices.Unity;
using Count4U.Model.Interface;
using Count4U.Model.Count4U;
using System.IO;
using Microsoft.Reporting.WinForms;
using Count4U.Model;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Model.Main;
using Count4U.Model.Audit;
using Count4U.GenerationReport;

namespace Count4U.Modules.ContextCBI.Views.Report
{
    public partial class ReportsWinForm : Form
    {
        private readonly IUnityContainer _container;
		//private ILocationRepository _locationRepository;
		//private IIturRepository _iturRepository;
		//private  IAlterADOProvider this._alterADOProvider;
		//private ISQLScriptRepository _sqlScriptRepository;
		//private DomainEnum this._domainType;
		//private string _domainContextDataSet;
		//private SelectParams this._selectParam;
		//private string _reportPath;

        public ReportsWinForm(IUnityContainer container)
        {
            this._container = container;
			//this.Initialize();
            InitializeComponent();
			//this.CreateReport();
        }

		public void CreateReport(GenerateReportArgs args, string reportTemplatePath, string reportDisplayName, string codeReport,
			List<ReportDataSource> reportDSList,
            Customer customer, Branch branch, Inventor inventor, Itur itur = null, DocumentHeader documentHeader = null)
		{
			//ReportViewer reportViewer = this.InitReport(this.reportViewer1, reportPath, reportDSList,
			//    customer, branch, inventor);

			// =====================   Fill parm in Report File name =========================
			string iturCode = "";
			string docNum = "";
			string branchCodeERP = "";
			string customerCode = "";
			DateTime inventorDate = DateTime.Now;
			if (itur != null) {	iturCode = itur.IturCode;  }
			if (documentHeader != null) {	 docNum = documentHeader.DocNum.ToString();	}
		    if (branch != null)	{ branchCodeERP = string.IsNullOrWhiteSpace(branch.BranchCodeERP) == false ? branch.BranchCodeERP : string.Empty;	}
			if (customer != null) { customerCode = string.IsNullOrWhiteSpace(customer.Code) == false ? customer.Code : string.Empty; }
			if (inventor != null) { inventorDate = inventor.InventorDate; }
			if (reportDisplayName.Contains("XXX") == true)
			{
				reportDisplayName = reportDisplayName.Replace("XXX", branchCodeERP);
			}
			if (reportDisplayName.Contains("YYY") == true)
			{
				reportDisplayName = reportDisplayName.Replace("YYY", iturCode);
			}
			if (reportDisplayName.Contains("ZZZ") == true)
			{
				reportDisplayName = reportDisplayName.Replace("ZZZ", docNum);
			}
			if (reportDisplayName.Contains("VVV") == true)
			{
				reportDisplayName = reportDisplayName.Replace("VVV", customerCode);
			}
			if (reportDisplayName.Contains("DD") == true)
			{
				reportDisplayName = reportDisplayName.Replace("DD", inventorDate.ToString("dd"));
			}
			if (reportDisplayName.Contains("MM") == true)
			{
				reportDisplayName = reportDisplayName.Replace("MM", inventorDate.ToString("MM"));
			}
			if (reportDisplayName.Contains("YY") == true)
			{
				reportDisplayName = reportDisplayName.Replace("YY", inventorDate.ToString("yyyy"));
			}
			if (reportDisplayName.Contains("NOWDATE") == true)
			{
				DateTime dt = DateTime.Now;
				string dateNow = dt.ToString("dd") + "-" + dt.ToString("MM") + "-" + dt.ToString("yyyy");

				reportDisplayName = reportDisplayName.Replace("NOWDATE", dateNow);
			}

			//=======================	 InitReportTemplate ========================= 
			this.InitReportTemplate(reportTemplatePath, reportDisplayName, reportDSList);
			this.InitReportTemplateParms(args, reportTemplatePath, reportDSList,
				customer, branch, inventor);
			//=======================   Show Report in Form
				if (codeReport == "[Rep-IS1-62]"
					|| codeReport == "[Rep-IS1-62n]"
					|| codeReport == "[Rep-IS1-62np]"
					|| codeReport == "[Rep-IS1-63s]"
					|| codeReport == "[Rep-IS1-63v]"
					|| codeReport == "[Rep-IS1-63q]"				
					|| codeReport == "[Rep-IS1-62b]"
					|| codeReport == "[Rep-IS1-64]")
				{
					
						ReportParameter[] reportParams = new ReportParameter[2];
						reportParams[0] = new ReportParameter("ReportParameterDiffQuantityERP", "0", true);
						reportParams[1] = new ReportParameter("ReportParameterDiffValueERP", "0", true);
					SelectParams selectParams = args.SelectParams;
					if (selectParams != null)
					{
						if (selectParams.Extra.ContainsKey(SelectParamsExtra.ReportQuantityDifferenceERP.ToString()) == true)
						{
							string quantityDifferenceERP = selectParams.Extra[SelectParamsExtra.ReportQuantityDifferenceERP.ToString()].ToString();
							if (string.IsNullOrWhiteSpace(quantityDifferenceERP) == false)
							{
								reportParams[0] = new ReportParameter("ReportParameterDiffQuantityERP", quantityDifferenceERP, true);
							}
						}
						if (selectParams.Extra.ContainsKey(SelectParamsExtra.ReportValueDifferenceERP.ToString()) == true)
						{
							string valueDifferenceERP = selectParams.Extra[SelectParamsExtra.ReportValueDifferenceERP.ToString()].ToString();
							if (string.IsNullOrWhiteSpace(valueDifferenceERP) == false)
							{
								reportParams[1] = new ReportParameter("ReportParameterDiffValueERP", valueDifferenceERP, true);
							}
						}
					}
						this.reportViewer1.LocalReport.SetParameters(reportParams);
				}

			this.reportViewer1.RefreshReport();
		}
 
		//private ReportViewer InitReport(ReportViewer reportViewer, 
		//    string reportTemplatePath, List<ReportDataSource> reportDSList,
		//    Customer customer, Branch branch, Inventor inventor)
		//{
		//    InitReportTemplateParms(reportViewer, reportTemplatePath, reportDSList,
		//        customer, branch, inventor);
		//    return InitReportTemplate(reportViewer, reportTemplatePath, reportDSList);
		//}

		private void InitReportTemplateParms(GenerateReportArgs args,
			string reportTemplatePath, List<ReportDataSource> reportDSList,
			Customer customer, Branch branch, Inventor inventor)
		{
			
			//В текстбоксе прописал следущую строку - Parameters!Zagolovok.Value
			//List<ReportParameter> paramList;
			//paramList = new List<ReportParameter>();
			//paramList.Add(new ReportParameter("Zagolovok", "ООО Рога и копыта"));

			//reportViewer.LocalReport.SetParameters(paramList);
			//----------------------------
		//    ReportParameterInfoCollection parameterInfo =
		//this.reportViewer1.LocalReport.GetParameters();
		//    if (parameterInfo.Count > 0)
		//    {
		//        List<ReportParameter> param = new List<ReportParameter>();
		//        ReportParameter yearParam = new ReportParameter("IturParameter", "1");
		//        param.Add(yearParam);

		//        this.reportViewer1.LocalReport.SetParameters(param);
		//    }
		}

		private void InitReportTemplate(
			string reportTemplatePath, string reportDisplayName,
			List<ReportDataSource> reportDSList)
		{
			this.reportViewer1.LocalReport.ReportPath = reportTemplatePath;
			this.reportViewer1.LocalReport.DataSources.Clear();
  			foreach (var reportDS in reportDSList)
			{
				this.reportViewer1.LocalReport.DataSources.Add(reportDS);
			}
			this.reportViewer1.LocalReport.DisplayName = reportDisplayName;
  		}

		//public void RefreshReport(ReportViewer reportViewer)
		//{
		//    this.reportViewer1.RefreshReport();
		//}

        private void ReportsWinForm_Load(object sender, EventArgs e)
        {
           // this.reportViewer1;
        }
		
		public void CreateReportOld()
		{
			//string deviceInfo = null;
			//string format = "PDF";

			//LocalReport lr = new LocalReport();
			//string deviceInfo = "<DeviceInfo>" +
			// "  <OutputFormat>PDF</OutputFormat>" +
			// "  <PageWidth>8.5in</PageWidth>" +
			// "  <PageHeight>11.5in</PageHeight>" +
			// "  <MarginTop>0.6in</MarginTop>" +
			// "  <MarginLeft>0.6in</MarginLeft>" +
			// "  <MarginRight>0.4in</MarginRight>" +
			// "  <MarginBottom>0.4in</MarginBottom>" +
			// "</DeviceInfo>";

			 
			//deviceInfo =
			//  "<DeviceInfo>" +
			//  "<SimplePageHeaders>True</SimplePageHeaders>" +
			//  "</DeviceInfo>";

				//====
			//////ReportDataSource reportDS = new ReportDataSource("DataSet1", 
			//////this._locationRepository.GetLocations(@"Inventor\2011\8\1\2b588315-4b14-4709-bac9-55f5e303721b"));
			////////this.reportViewer1.LocalReport.ReportPath = @"C:\Count4U\trunk\Count4U\Count4U\bin\Debug\Report4.rdlc";
			//////this.reportViewer1.LocalReport.ReportPath = @"C:\Count4U\trunk\Count4U\Count4U.Modules.ContextCBI\Views\Report\Report4.rdlc";
			//////this.reportViewer1.LocalReport.DataSources.Clear();
			//////this.reportViewer1.LocalReport.DataSources.Add(reportDS);
			//////this.reportViewer1.RefreshReport();
			//lr.Refresh();
			 //========

////////                ReportParameter[] params = new ReportParameter[3];
////////params[0] = new ReportParameter("spName", "ProductShipped", false);
////////params[1] = new ReportParameter("spParam", "@parameter1", false);
////////params[2] = new ReportParameter("spParamValues", "test", false);
			////Me.ReportViewer2.LocalReport.SetParameters(Params)

////////this.reportViewer.LocalReport.SetParameters(params);

////Dim Params(3) As ReportParameter
////Params(0) = New ReportParameter("MainTitle", "New Main Title")
////Params(1) = New ReportParameter("X-Axis", "New X Axis")
////Params(2) = New ReportParameter("Y-Axis", "New Y Axis")
////Me.ReportViewer2.LocalReport.SetParameters(Params)



			//byte[] bytes = lr.Render(format, deviceInfo);

			//using (FileStream fs = new FileStream(@"C:\Count4U\trunk\Count4U\Count4U.Model\Report1.pdf", FileMode.Create))
			//{
			//    fs.Write(bytes, 0, bytes.Length);
			//}

			//
			//LocalReport report = new LocalReport();
			//report.ReportPath = "Report.rdlc";
			//report.DataSources.Add(new ReportDataSource("Sales", LoadSalesData()));

			//Export(lr);

		}

		//public void CreateIturReport(string dbPath, string reportTemplatePath)
		//{
		//    ReportDataSource reportDS = new ReportDataSource("ReportDataSet",
		//        this._iturRepository.GetIturs(dbPath));	//@"Inventor\2011\8\1\2b588315-4b14-4709-bac9-55f5e303721b"));
		//    ReportViewer reportViewer = this.FillLocalReport(this.reportViewer1, reportTemplatePath, reportDS);
		//    this.RefreshReport(reportViewer);
		//}

		//public void InitReportContext(	//string domainContextDataSet,
		//    string reportPath,
		//    SelectParams selectParam = null)
		//{
		//    this._reportPath = reportPath;
		//    //this._domainContextDataSet = domainContextDataSet;
		//}

		//public void CreateLocationReport(string dbPath, string reportTemplatePath)
		//{
		//    ReportDataSource reportDS = new ReportDataSource("ReportDataSet",
		//        this._locationRepository.GetLocations(dbPath));	//@"Inventor\2011\8\1\2b588315-4b14-4709-bac9-55f5e303721b"));
		//    ReportViewer reportViewer = this.FillLocalReport(this.reportViewer1, reportTemplatePath, reportDS);
		//    this.RefreshReport(reportViewer);
		//}


		//public void CreateIturReport(Iturs iturs, string reportTemplatePath)
		//{
		//    ReportDataSource reportDS = new ReportDataSource("ReportDataSet", iturs);
		//    ReportViewer reportViewer = this.FillLocalReport(this.reportViewer1, reportTemplatePath, reportDS);
		//    this.RefreshReport(reportViewer);
		//}
		//public void CreateReport1()
		//{
		//    ReportDataSource reportDS = new ReportDataSource("IturDataSet",
		//        this._iturRepository.GetIturs(@"Inventor\2011\8\1\2b588315-4b14-4709-bac9-55f5e303721b"));
		//    this.reportViewer1.LocalReport.ReportPath = @"C:\Count4U\trunk\Count4U\Count4U.Model\Count4U\Itur\IturReport.rdlc";
		//    this.reportViewer1.LocalReport.DataSources.Clear();
		//    this.reportViewer1.LocalReport.DataSources.Add(reportDS);
		//    this.reportViewer1.RefreshReport();
		//}

		//public void CreateReport2()
		//{
		//    ReportDataSource reportDS = new ReportDataSource("IturDataSet",
		//        this._iturRepository.GetIturs(@"Inventor\2011\8\1\2b588315-4b14-4709-bac9-55f5e303721b"));
		//    this.reportViewer1.LocalReport.ReportPath = @"C:\Count4U\trunk\Count4U\Count4U.Modules.ContextCBI\Views\Report\Report6.rdlc";
		//    this.reportViewer1.LocalReport.DataSources.Clear();
		//    this.reportViewer1.LocalReport.DataSources.Add(reportDS);
		//    this.reportViewer1.RefreshReport();
		//}

		//public void CreateReport3()
		//{
		//    ReportDataSource reportDS = new ReportDataSource("IturDataSet",
		//        this._locationRepository.GetLocations(@"Inventor\2011\8\1\2b588315-4b14-4709-bac9-55f5e303721b"));
		//    this.reportViewer1.LocalReport.ReportPath = @"C:\Count4U\trunk\Count4U\Count4U.Modules.ContextCBI\Views\Report\Report7.rdlc";
		//    this.reportViewer1.LocalReport.DataSources.Clear();
		//    this.reportViewer1.LocalReport.DataSources.Add(reportDS);
		//    this.reportViewer1.RefreshReport();
		//}


	//    private Stream CreateStream(string name, string fileNameExtension, 
	//       Encoding encoding,string mimeType, bool willSeek)
	//{
	//    Stream stream = new FileStream(name + "." + fileNameExtension, 
	//      FileMode.Create);
	//    //m_streams.Add(stream);
	//    return stream;
	//}

		//private void Export(LocalReport report)
		//{
		//    string deviceInfo =
		//      "<DeviceInfo>" +
		//      "  <OutputFormat>PDF</OutputFormat>" +
		//      "  <PageWidth>8.5in</PageWidth>" +
		//      "  <PageHeight>11in</PageHeight>" +
		//      "  <MarginTop>0.25in</MarginTop>" +
		//      "  <MarginLeft>0.25in</MarginLeft>" +
		//      "  <MarginRight>0.25in</MarginRight>" +
		//      "  <MarginBottom>0.25in</MarginBottom>" +
		//      "</DeviceInfo>";
		//    Warning[] warnings;
		//    //m_streams = new List<Stream>();
		//    //report.Render("PDF", deviceInfo, CreateStream, out warnings);

		//    //foreach (Stream stream in m_streams)
		//    //    stream.Position = 0;
		//}

		private void ReportsWinForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			//this.reportViewer1.LocalReport.DataSources.Clear();
			//this.reportViewer1.Dispose();
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}

		private void reportViewer1_VisibleChanged(object sender, EventArgs e)
		{
			GC.Collect();
			GC.Collect();
		}

		private void ReportsWinForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.reportViewer1.LocalReport.DataSources.Clear();
			this.reportViewer1.Reset();
			this.reportViewer1.LocalReport.ReleaseSandboxAppDomain();
		}

		//private void button1_Click(object sender, EventArgs e)
		//{
		//    this.CreateReport();
		//}

		//private void button2_Click(object sender, EventArgs e)
		//{
		//    this.CreateReport1();
		//}

		//private void button3_Click(object sender, EventArgs e)
		//{
		//    this.CreateReport2();
		//}

		//private void button4_Click(object sender, EventArgs e)
		//{
		//    this.CreateReport3();
		//}
		//// 
		//// reportViewer1
		//// 
		//this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
		//reportDataSource1.Name = "DataSet1";
		//reportDataSource1.Value = this.locationBindingSource;
		//this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
		//this.reportViewer1.LocalReport.ReportEmbeddedResource = "Count4U.Modules.ContextCBI.Views.Report.Report4.rdlc";
		//this.reportViewer1.Location = new System.Drawing.Point(0, 0);
		//this.reportViewer1.Name = "reportViewer1";
		//this.reportViewer1.Size = new System.Drawing.Size(648, 452);
		//this.reportViewer1.TabIndex = 0;

		//public void Initialize()
		//{
		//    this._locationRepository = this._container.Resolve<LocationEFRepository>();
		//    this._iturRepository = this._container.Resolve<IturEFRepository>();
		//}

	
    }
}
