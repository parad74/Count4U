using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using Common.Config;
using Common.Main.Interface;
using Count4U.Model.Interface.Main;
using Count4U.Model.Main;
using Count4U.Model.ServiceClient;
using Count4U.Model.ServiceContract;
using Count4U.WindowsHost;
using Main.Service;
using Microsoft.Practices.ServiceLocation;

namespace Common.Main
{
	public partial class Form1 : Form, IMainView
	{
		private string _MONITOR = "MONITOR";
	public string[] programArgs;

		public readonly IServiceLocator _serviceLocator;
		private IMainPresenter _presenter;
		//IRequest2ObjectServicePresenter _request2ObjectServiceHostPresenter;
		//private IRequest2ResponseServicePresenter _request2ResponseServiceHostPresenter;
		private ILogMessageServicePresenter _logMessageServicePresenter;
		private IMainDBServicePresenter _mainDBServicePresenter;

		private CustomerReports _customerReports;

		public CustomerReports CRs
		{
			get { return _customerReports; }
			set { _customerReports = value; }
		}

		public Form1(
			IMainPresenter presenter,
			IServiceLocator serviceLocator,
			//IRequest2ObjectServicePresenter request2ObjectServiceHostPresenter,
			ILogMessageServicePresenter logMessageServicePresenter,
			IMainDBServicePresenter mainDBServicePresenter)
		{
			InitializeComponent();
			this._presenter = presenter;
			this._serviceLocator = serviceLocator;
			//this._request2ObjectServiceHostPresenter = request2ObjectServiceHostPresenter;
			this._logMessageServicePresenter = logMessageServicePresenter;
			this._mainDBServicePresenter = mainDBServicePresenter;
			//Циклическая зависимость
			this._presenter.View = this;
			//this._request2ObjectServiceHostPresenter.View = this;
			this._logMessageServicePresenter.View = this;
			this._mainDBServicePresenter.View = this;
		
			this.Init();
			programArgs = Program.ProgramArgs;

			//this._request2ResponseServiceHostPresenter = _serviceLocator.GetInstance<IRequest2ResponseServicePresenter>();
			//this._request2ResponseServiceHostPresenter.View = this;
			
			this.SetOptionsRunLikeText();
   			this.UpdateProgArgsLogText();

			//===============   LogMessageService
			if (this._logMessageServicePresenter != null)
			{
				this._logMessageServicePresenter.OpenService();
			}

			if (this._mainDBServicePresenter != null)
			{
				this._mainDBServicePresenter.OpenService();
			}

		}

		private void Init()
		{
			ConfigCommunication configCommunication = new ConfigCommunication();
			string error = "";

			//List<IClientPresenterInfo> containerClientPresenters = this._serviceLocator.GetAllInstances<IClientPresenterInfo>().ToList();
			//List<string> clientsInfoTitles = containerClientPresenters.Select(r => r.Title).ToList();
			//clientNameComboBox.DataSource = clientsInfoTitles;
			////IClientPresenterInfo info = containerClientPresenters.FirstOrDefault(r => r.IsDefault == true);
			//string proxyTitle = configCommunication.GetProxyIsDefault(out error);
			//IClientPresenterInfo info = containerClientPresenters.FirstOrDefault(r => r.Title == proxyTitle);
			//if (info == null)
			//{
			//	if (containerClientPresenters.Count > 0)
			//	{
			//		info = containerClientPresenters[0];
			//	}
			//}
			//if (info != null)
			//{
			//	clientNameComboBox.SelectedIndex = clientNameComboBox.Items.IndexOf(info.Title);
			//	clientAddressTextBox.Text = info.Address;
			//	clientBindingComboBox.Text = info.Binding;
			//	clientContractComboBox.Text = info.Contract;
			//}

			//============================================================
			error = "";
			string logPath = configCommunication.GetLogPath(out error);

			this.folderTextBox.Text = logPath; //UtilsSip2.LogPath; //@"C:\Count4U\log";
			//this.dbFolderTextBox.Text = UtilsSip2.AppDataFolder; //@"C:\Count4U\log";
			this.dbFolderTextBox.Text = UtilsSip2.BuildAppDataFolderPath();
		}

		//========================  IView  ============================
		public string GetInputText()
		{
			return logTextBox.Text;
		}

		public void SetOutputText(string text)
		{
			this.logTextBox.Text += text;
		}

		public void SetRunOptionsText(string text)
		{
			this.runOptionsTextBox.Text += text;
		}

		public bool GetIncludeWSDLCheckBox()
		{
			return this.includeWSDLCheckBox.Checked;
		}

		public bool GetIncludeTimeTracingCheckBox()
		{
			return this.includeTimeTracingCheckBox.Checked;
		}

		public void SetUrlText(string text)
		{
			this.textBoxUrl.Text = text;
		}

		public void SetUrlMexText(string text)
		{
			this.textBoxUrlMex.Text = text;
		}
		

		//=============================================================
		
		private void ClearButton_Click(object sender, EventArgs e)
		{
			//this._presenter.TestClear();
			//this.inputTextBox.Clear();
			//this.outputTextBox.Clear();
			this.logTextBox.Clear();
			this.outputTextBox.Clear();
			this.ClearListView();
			this.UpdateProgArgsLogText();
		}

		private void UpdateProgArgsLogText()
		{
			this.logTextBox.HideSelection = false;
			string progArgs = string.Join(" ", programArgs);
			this.logTextBox.Text += "Program Args : " + progArgs + Environment.NewLine;
			//string serverHosting = "???";
			//if (string.IsNullOrWhiteSpace(UtilsSip2._IIS) == false)
			//{
			//	serverHosting = UtilsSip2._IIS;
			//}
			//this.logTextBox.Text += Environment.NewLine + "Server Hosting : " + serverHosting + Environment.NewLine;
			this.logTextBox.SelectionStart = this.logTextBox.Text.Length;
			this.logTextBox.ScrollToCaret();
		}

		private void SetOptionsRunLikeText()
		{
			this.runOptionsTextBox.Clear();
			string progArgs = string.Join(" ", programArgs);
			this.runOptionsTextBox.Text += "Program Args : " + progArgs + Environment.NewLine;

			ConfigCommunication configCommunication = new ConfigCommunication();
			string backspace = configCommunication.GetProtocolBackspace();
			if (backspace == Environment.NewLine) backspace = @"\r\n";
			else if (backspace == "\r") backspace = @"\r";
			else if (backspace == "\n") backspace = @"\n";

			string filePath = @"APP_DATA\" + configCommunication.GetProtocolFilePath().Trim('\\') + @"\Sip2Liber8Protocol.xml";
																																						 

			this.runOptionsTextBox.Text += "Backspace : " + backspace + " , " + "File Sip2 Protocol : " + filePath + Environment.NewLine; 

			//string serverHosting = "???";
			//if (string.IsNullOrWhiteSpace(UtilsSip2._IIS) == false)
			//{
			//	serverHosting = UtilsSip2._IIS;
			//}
			//this.runOptionsTextBox.Text += Environment.NewLine + "Server Hosting : " + serverHosting + Environment.NewLine;
		}

		private void logFolderButton_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();

			if (fbd.ShowDialog() == DialogResult.OK)
			{
				try
				{
					this.folderTextBox.Text = fbd.SelectedPath;
				}
				catch (Exception ex)
				{
					string massage = ex.Message;
				}
			}
		}

		//START Serveses
		private void openButton_Click(object sender, EventArgs e)
		{

			if (this._logMessageServicePresenter != null)	
			{	
				this._logMessageServicePresenter.OpenService();	
			}
			else	{ this.SetOutputText("_logMessageServicePresenter is null");	}

			if (this._mainDBServicePresenter != null)
			{
				this._mainDBServicePresenter.OpenService();
			}
			else { this.SetOutputText("_mainDBServicePresenter is null"); }

			this.SetOutputText(Environment.NewLine);
			this.SetRunOptionsText(Environment.NewLine);
		}
		
		//STOP Servises
		private void test2Button_Click(object sender, EventArgs e)
		{
			this.CloseAllService();
			//this._request2ObjectServiceHostPresenter.CloseRequest2ObjectServiceHost();
			this.SetOutputText(Environment.NewLine);
        }

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.CloseAllService();
		}

		private void CloseAllService()
		{
			if (this._logMessageServicePresenter != null) 
			{ 
				this._logMessageServicePresenter.CloseService(); 
			}
			if (this._mainDBServicePresenter != null)
			{
				this._mainDBServicePresenter.CloseService();
			}
		}

		public void ClearListView()
		{
			ListView.ListViewItemCollection lvis = listView1.Items;
			foreach (ListViewItem lvi in lvis)
			{
				lvi.Remove();
			}
			if (listView1.Items.Count > 0)
			{
				//buttonOk.Enabled = true;
			}
			else
			{
				//buttonOk.Enabled = false;
			}

			ListView.ListViewItemCollection lvis2 = listView2.Items;
			foreach (ListViewItem lvi in lvis2)
			{
				lvi.Remove();
			}
		}

		private void logFolderButton_Click_1(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("explorer.exe", folderTextBox.Text);
			//FolderBrowserDialog fbd = new FolderBrowserDialog();

			//if (fbd.ShowDialog() == DialogResult.OK)
			//{
			//	try
			//	{
			//		this.folderTextBox.Text = fbd.SelectedPath;
			//	}
			//	catch (Exception ex)
			//	{
			//		string massage = ex.Message;
			//	}
			//}
		}

		private void SaveButton_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();
			saveFileDialog1.Filter = "*.txt|*.txt|*.log|*.log|*.*|*.*";
			saveFileDialog1.Title = "Save Log File";
			saveFileDialog1.FilterIndex = 1;
			saveFileDialog1.RestoreDirectory = true;
			saveFileDialog1.FileName = "log" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "-" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;

			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if(saveFileDialog1.FileName != "")
				{
					this.WriteToLogFile(logTextBox.Text, saveFileDialog1.FileName);
				}
			}


		}


		private void WriteToLogFile(string @string, string toPathFile)
		{
			Encoding providerEncoding = Encoding.GetEncoding("windows-1255");
			using (MemoryStream ms = new MemoryStream())
			{
				StreamWriter sw = new StreamWriter(ms, providerEncoding);
				sw.WriteLine(@string);
				sw.Flush();
				File.WriteAllText(toPathFile, providerEncoding.GetString(ms.ToArray()));
			}
		}

		//private void clientNameComboBox_SelectedIndexChanged(object sender, EventArgs e)
		//{
		//	List<IClientPresenterInfo> containerClientPresenters = this._serviceLocator.GetAllInstances<IClientPresenterInfo>().ToList();
		//	//List<string> clientsInfoTitles = containerClientPresenters.Select(r => r.Title).ToList();
		//	//clientNameComboBox.DataSource = clientsInfoTitles;
		//	//IClientPresenterInfo info = containerClientPresenters.FirstOrDefault(r => r.IsDefault == true);
		//	//if (info == null)
		//	//{
		//	//	if (containerClientPresenters.Count > 0)
		//	//	{
		//	//		info = containerClientPresenters[0];
		//	//	}
		//	//}

		//	string clientTitle = (string)clientNameComboBox.SelectedItem;
		//	ServiceProxyClientModuleInit.ClientPresenterInfoCurrent = containerClientPresenters.FirstOrDefault(r => r.Title == clientTitle);
		//	//IClientPresenterInfo info = containerClientPresenters.FirstOrDefault(r => r.Title == clientTitle);
		//	//if (info != null)
		//	if (ServiceProxyClientModuleInit.ClientPresenterInfoCurrent != null)
		//	{
		//		//clientNameComboBox.SelectedIndex = clientNameComboBox.Items.IndexOf(info.Title);
		//		clientAddressTextBox.Text = ServiceProxyClientModuleInit.ClientPresenterInfoCurrent.Address;
		//		clientBindingComboBox.Text = ServiceProxyClientModuleInit.ClientPresenterInfoCurrent.Binding;
		//		clientContractComboBox.Text = ServiceProxyClientModuleInit.ClientPresenterInfoCurrent.Contract;

		//		ServiceProxyClientModuleInit.ProxyClientCurrent = this._serviceLocator.GetInstance<IRequest2ResponseProxy>(ServiceProxyClientModuleInit.ClientPresenterInfoCurrent.Name);
		//	}
		//}

		private void button1_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("explorer.exe", dbFolderTextBox.Text);
		}

		private void label6_Click(object sender, EventArgs e)
		{

		}

		private void dbFolderTextBox_TextChanged(object sender, EventArgs e)
		{

		}

		private void tabPage1_Click(object sender, EventArgs e)
		{

		}

		private void labelLog_Click(object sender, EventArgs e)
		{

		}

		private void Form1_Load(object sender, EventArgs e)
		{
			
			ICustomerReportRepository  customerReportRepository =  this._serviceLocator.GetInstance<ICustomerReportRepository>	();	
			CRs = customerReportRepository.GetCustomerReports();
			dataGridView1.DataSource = CRs;

			// TODO: This line of code loads data into the 'mainDBDataSet.CustomerReport' table. You can move, or remove it, as needed.
		//	this.customerReportTableAdapter.Fill(this.mainDBDataSet.CustomerReport);

		}

		private void regetButton_Click(object sender, EventArgs e)
		{
			ICustomerReportRepository customerReportRepository = this._serviceLocator.GetInstance<ICustomerReportRepository>();
			CRs = customerReportRepository.GetCustomerReports();
			dataGridView1.DataSource = CRs;
			//customerReportTableAdapter.Fill(this.mainDBDataSet.CustomerReport);
		}

		private void textBox2_TextChanged(object sender, EventArgs e)
		{

		}

	

	

		
	}

	internal static class UiConnection
	{
		//static readonly FormCollection formCollection = Application.OpenForms;
		//static readonly IEnumerable<Form1> forms = Application.OpenForms.Cast<Form1>();
		static readonly Form1 form1 =
			Application.OpenForms.Cast<Form1>().FirstOrDefault(form => form is Form1) as Form1;

				
		internal static void UpdateLogText(string @string, string @time = "")
		{
			if (string.IsNullOrWhiteSpace(@time) == true) @time = DateTime.Now.ToLongTimeString();
			form1.logTextBox.HideSelection = false;
			@string = "[" + @time + "]  " + @string;
			form1.logTextBox.Text += Environment.NewLine + @string;
			form1.logTextBox.SelectionStart = form1.logTextBox.Text.Length;
			form1.logTextBox.ScrollToCaret();
			//WriteToLogFile(form1.logTextBox.Text, form1.folderTextBox.Text + @"\log.txt");
		}

		//internal static void UpdateLogTimeText(string @time, string @string)
		//{
		//	form1.logTextBox.HideSelection = false;
		//	@string = "[" + time + "]  " + @string;
		//	form1.logTextBox.Text += Environment.NewLine + @string;
		//	form1.logTextBox.SelectionStart = form1.logTextBox.Text.Length;
		//	form1.logTextBox.ScrollToCaret();
		//	//WriteToLogFile(form1.logTextBox.Text, form1.folderTextBox.Text + @"\log.txt");
		//}

		internal static void UpdateLogRequestMessageText(string @string)
		{
			if (form1.includeWSDLCheckBox.Checked == true)
			{
				UpdateLogText(@string);
			}
		}

		internal static void UpdateLogRequestTimeMessageText(string @string , string @time = "")
		{
			if (form1.includeTimeTracingCheckBox.Checked == true)
			{
				UpdateLogText(@string + Environment.NewLine, @time);
			}
		}

		internal static bool GetIncludeWSDLCheckBox()
		{
			return form1.includeWSDLCheckBox.Checked;
		}


		internal static bool GetIncludeTimeTracingCheckBox()
		{
			return form1.includeTimeTracingCheckBox.Checked;
		}

		internal static void UpdateOutputText(string @string)
		{
			form1.outputTextBox.HideSelection = false;
			//@string = "[" + DateTime.Now + "]  " + @string;
			form1.outputTextBox.Text += @string;
			form1.outputTextBox.SelectionStart = form1.outputTextBox.Text.Length;
			form1.outputTextBox.ScrollToCaret();
		}

		internal static void ClearOutputText()
		{
			UiConnection.form1.outputTextBox.Text = "";
		}

		internal static void UpdateListViewRequest(string operation, string oparationName, Dictionary<string, string> propDictionary)
		{
			ListView.ListViewItemCollection lvis = form1.listView1.Items;
			foreach (ListViewItem lvi in lvis)
			{
				lvi.Remove();
			}

			foreach (KeyValuePair<string, string> keyValuePair in propDictionary)
			{
				ListViewItem lvi = new ListViewItem(operation + " - " + oparationName);
				lvi.SubItems.Add(keyValuePair.Key);
				lvi.SubItems.Add(keyValuePair.Value);
				lvis.Add(lvi);
			}

		}

		internal static void UpdateListViewResponse(string operation, string oparationName, Dictionary<string, string> propDictionary)
		{
			ListView.ListViewItemCollection lvis = form1.listView2.Items;
			foreach (ListViewItem lvi in lvis)
			{
				lvi.Remove();
			}

			foreach (KeyValuePair<string, string> keyValuePair in propDictionary)
			{
				ListViewItem lvi = new ListViewItem(operation + " - " + oparationName);
				lvi.SubItems.Add(keyValuePair.Key);
				lvi.SubItems.Add(keyValuePair.Value);
				lvis.Add(lvi);
			}
		}

		//internal static string JoinRecord(this String[] records, string separator)
		//{
		//	string ret = "";
		//	foreach (string record in records)
		//	{
		//		ret = ret + record + separator;
		//	}
		//	return ret;
		//}

		// comboBox
		//internal static IRequest2ResponseProxy GetSelectedClientProxy()
		//{
		//	//string clientTitle = (string)form1.clientNameComboBox.SelectedItem;
		//	string clientTitle = ServiceClientModuleInit.ClientPresenterInfoCurrent.Title;
		//	return GetClientProxy(clientTitle);
		//}

		//internal static IRequest2ResponseProxy GetClientProxy(string @stringTitle)
		//{
		//	List<IClientPresenterInfo> containerClientPresenters = form1._serviceLocator.GetAllInstances<IClientPresenterInfo>().ToList();
		//	IClientPresenterInfo info = containerClientPresenters.FirstOrDefault(r => r.Title == @stringTitle);
		//	IRequest2ResponseProxy _bibliothecaProxyClient = form1._serviceLocator.GetInstance<IRequest2ResponseProxy>(info.Name);
		//	return _bibliothecaProxyClient;
		//}

		//internal static IRequest2ResponseProxy GetSelectedClientProxy()
		//{
		//	IClientPresenterInfo info = ServiceClientModuleInit.ClientPresenterInfoCurrent;
		//	IRequest2ResponseProxy _bibliothecaProxyClient = form1._serviceLocator.GetInstance<IRequest2ResponseProxy>(info.Name);
		//	return _bibliothecaProxyClient;
		//}
		

	}
}



