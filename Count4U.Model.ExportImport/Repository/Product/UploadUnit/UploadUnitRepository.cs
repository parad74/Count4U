using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using System.Collections.Generic;
using Count4U.Model.Lib.MultiPoint;
using Microsoft.Practices.ServiceLocation;
using System.Windows.Forms;
using System.IO;
using System.Text;

namespace Count4U.Model.Count4U
{

	/// <summary>
	/// ЭТО НЕ ИСПОЛЬЗУЕТСЯ  - код от исходного кода - для примера
	/// </summary>
	public class UploadUnitRepository : IUploadUnitRepository // : BaseEFRepository, IUploadUnitRepository
	{
		private Dictionary<string, Multi> _multiUnitDictionary;

		private readonly IServiceLocator _serviceLocator;
		private System.Windows.Forms.ListView lvPorts;
		List<Multi> lMulti;
		//List<MultiPoint.itemDir>[] lDirs;
		Timer timer1;
		private System.Windows.Forms.CheckedListBox lbFiles;
		private System.Windows.Forms.CheckedListBox lbDir;//?
		private MPMemory cbMemory;//?
	
		public UploadUnitRepository(//IConnectionDB connection,
			IServiceLocator serviceLocator)
			//: base(connection)
		{
			this._multiUnitDictionary = new Dictionary<string, Multi>();
			UploadUnits _uploadUnitList = new UploadUnits();
			this.lvPorts = new ListView();
			this.lMulti = new List<Multi>();
			this.timer1 = new Timer();
			this.lbFiles = new CheckedListBox();
			//this.lbDir = new CheckedListBox();
			//this.cbMemory = MPMemory.FLASH;
			this._serviceLocator = serviceLocator;
		}

		/// ЭТО НЕ ИСПОЛЬЗУЕТСЯ  - код от исходного кода - для примера
		//public Dictionary<string, Multi> GetMultiUnitDictionary(bool refill = true)
		//{
		//	if (refill == true)
		//	{
		//		List<string> ports = System.IO.Ports.SerialPort.GetPortNames().ToList();
		//		this.Init(ports, MPStateReceiver);
		//		//WakeUp()? где
		//	}
		//	return this._multiUnitDictionary;
		//}

		/// ЭТО НЕ ИСПОЛЬЗУЕТСЯ  - код от исходного кода - для примера
		public List<string> GetPortNames()
		{
			var ports = System.IO.Ports.SerialPort.GetPortNames().ToList();
			return ports;
		}
		/// ЭТО НЕ ИСПОЛЬЗУЕТСЯ  - код от исходного кода - для примера
		private void Init(List<string> ports, MPState EventMPState)
		{
			Multi obj;
			foreach (string port in ports)
			{
				obj = new Multi(port, 57600); //TODO из config file
				obj.EventMPState += EventMPState;
				if (obj.Start() == true)
				{
					this._multiUnitDictionary[port] = obj;
				}
			}
		}

		/// ЭТО НЕ ИСПОЛЬЗУЕТСЯ  - код от исходного кода - для примера
		//GUI  + EventMPState
		private void MPStateReceiver(Multi obj, MPStage stage, int state, string text, int current)
		{
			ListViewItem item = null;

			foreach (ListViewItem it in lvPorts.Items)
				if (it.SubItems[1].Text == obj.Port)
					item = it;

			switch (stage)
			{
				case MPStage.Bell:
					item.SubItems[0].Text = (Convert.ToChar(obj.Address)).ToString();
					item.SubItems[2].Text = "WakeUp " + item.SubItems[0].Text;
					break;
				case MPStage.Download:
					item.SubItems[2].Text = "Down: " + text + " " + state + " / " + current;
					break;
				case MPStage.Upload:
					item.SubItems[2].Text = "Up: " + text + " " + state + " / " + current;
					break;
			}
		}
		/// ЭТО НЕ ИСПОЛЬЗУЕТСЯ  - код от исходного кода - для примера	
		private void GetID()
		{
			foreach (Multi item in lMulti)
			{
				if (item.Address != 0x80)
					foreach (ListViewItem it in lvPorts.Items)
						if (it.SubItems[1].Text == item.Port)
						{
							it.SubItems[2].Text = item.GetTerminalID();
						}
			}
		}
		/// ЭТО НЕ ИСПОЛЬЗУЕТСЯ  - код от исходного кода - для примера
		//GUI + Iint
		private void WakeUp()
		{
			foreach (Multi item in lMulti)
			{
				item.WakeUp();
			}
		}
		/// ЭТО НЕ ИСПОЛЬЗУЕТСЯ  - код от исходного кода - для примера
		//GUI
		private void Upload_Click(string folder)
		{
			List<string> list = new List<string>();

			foreach (string file in lbFiles.CheckedItems)
				list.Add(file);

			foreach (Multi item in lMulti)
			{
				if (item.Address != 0x80)
				{
					foreach (ListViewItem lvItem in lvPorts.Items)
					{
						if (lvItem.SubItems[1].Text == item.Port)
							item.UploadFiles(list, folder);
					}
				}
			}
		}
		/// ЭТО НЕ ИСПОЛЬЗУЕТСЯ  - код от исходного кода - для примера
		// ===========GUI
		private void SelectFolder(string folder)
		{
			//if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
			//{
			//	tbFolder.Text = folderBrowserDialog1.SelectedPath;
			//	lbFiles.Items.Clear();
			FileInfo[] list = (new DirectoryInfo(folder)).GetFiles();

			foreach (FileInfo file in list)
				lbFiles.Items.Add(file.Name);
			//}
		}

		/// ЭТО НЕ ИСПОЛЬЗУЕТСЯ  - код от исходного кода - для примера
		private void FolderLeave(string folder)
		{
			lbFiles.Items.Clear();
			FileInfo[] list = (new DirectoryInfo(folder)).GetFiles();

			foreach (FileInfo file in list)
				lbFiles.Items.Add(file.Name);
		}

		//===============private
		/// ЭТО НЕ ИСПОЛЬЗУЕТСЯ  - код от исходного кода - для примера
		//GUI
		private void GetVersion()
		{
			foreach (Multi item in lMulti)
			{
				if (item.Address != 0x80)
					foreach (ListViewItem it in lvPorts.Items)
						if (it.SubItems[1].Text == item.Port)
						{
							it.SubItems[2].Text = item.GetTerminalVersion();
						}
			}
		}
		/// ЭТО НЕ ИСПОЛЬЗУЕТСЯ  - код от исходного кода - для примера
		private void Abort_Click()
		{
			foreach (Multi item in lMulti)
				if (item.Address != 0x80)
					foreach (ListViewItem lvItem in lvPorts.Items)
						if (lvItem.SubItems[1].Text == item.Port)
							item.AbortUpDownLoading();
		}

		//GUI
		//private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		//{
		//	foreach (Multi item in lMulti)
		//		item.Stop();
		//}
// ============= Timer ==========
		/// ЭТО НЕ ИСПОЛЬЗУЕТСЯ  - код от исходного кода - для примера
		private void SetDateTime()
		{
			foreach (Multi item in lMulti)
			{
				if (item.Address != 0x80)
					foreach (ListViewItem it in lvPorts.Items)
						if (it.SubItems[1].Text == item.Port)
						{
							it.SubItems[2].Text = item.SetDateTime(DateTime.Now).ToString();
						}
			}
		}
		/// ЭТО НЕ ИСПОЛЬЗУЕТСЯ  - код от исходного кода - для примера
		private void Timer_Tick()
		{
			int count = 0;

			foreach (Multi item in lMulti)
			{
				if (item.Address != 0x80)
					foreach (ListViewItem it in lvPorts.Items)
						if (it.SubItems[1].Text == item.Port)
							it.SubItems[0].Text = (Convert.ToChar(item.Address)).ToString();
			}

			if (count == lMulti.Count)
				timer1.Stop();
		}


#region notuse

		//private void lvPorts_ItemActivate()
		//{
		//	foreach (ListViewItem lvItem in lvPorts.SelectedItems)
		//	{
		//		string current = lvItem.SubItems[1].Text + ": " + lvItem.SubItems[0].Text;

		//		lbDir.Items.Clear();

		//		if (lDirs[lvItem.Index] != null)
		//		{
		//			foreach (MultiPoint.itemDir item in lDirs[lvItem.Index])
		//				lbDir.Items.Add(item.name);
		//		}
		//	}
		//}
		//private void GetDirectory()
		//{
		//	foreach (Multi item in lMulti)
		//	{
		//		if (item.GetAddress() != 0x80)
		//			foreach (ListViewItem lvItem in lvPorts.Items)
		//				if (lvItem.SubItems[1].Text == item.GetCOMPort())
		//				{
		//					List<MultiPoint.itemDir> list = item.GetDirectory(false);
		//					lDirs[lvItem.Index] = list;
		//					if (list == null)
		//					{
		//						lvItem.SubItems[2].Text = "null";
		//						continue;
		//					}

		//					lvItem.SubItems[2].Text = list.Count.ToString() + ":";
		//					foreach (MultiPoint.itemDir i in list)
		//						lvItem.SubItems[2].Text += " " + i.name + "=" + i.size.ToString();
		//				}
		//	}
		//}

		//private void Delete_Click()
		//{
		//	List<string> list = new List<string>();

		//	foreach (string file in lbDir.CheckedItems)
		//		list.Add(file);

		//	foreach (Multi item in lMulti)
		//		if (item.GetAddress() == Encoding.ASCII.GetBytes(lvPorts.SelectedItems[0].SubItems[0].Text)[0]
		//			&& item.GetCOMPort() == lvPorts.SelectedItems[0].SubItems[1].Text)
		//			item.DeleteFiles(list);
		//}

		//private void Download_Click(string folder)
		//{
		//	List<string> list = new List<string>();

		//	foreach (string file in lbDir.CheckedItems)
		//		list.Add(file);

		//	foreach (Multi item in lMulti)
		//		if (item.GetAddress() != 0x80)
		//			if (item.GetAddress() == Encoding.ASCII.GetBytes(lvPorts.SelectedItems[0].SubItems[0].Text)[0]
		//				&& item.GetCOMPort() == lvPorts.SelectedItems[0].SubItems[1].Text)
		//				item.DownloadFiles(list, folder);
		//}
		//private void ColdStart_Click()
		//{
		//	foreach (Multi item in lMulti)
		//		if (item.GetAddress() != 0x80)
		//			if (item.GetAddress() == Encoding.ASCII.GetBytes(lvPorts.SelectedItems[0].SubItems[0].Text)[0]
		//				&& item.GetCOMPort() == lvPorts.SelectedItems[0].SubItems[1].Text)
		//				item.ColdStart();
		//}

		//private void WarmStart_Click()
		//{
		//	foreach (Multi item in lMulti)
		//		if (item.GetAddress() != 0x80)
		//			if (item.GetAddress() == Encoding.ASCII.GetBytes(lvPorts.SelectedItems[0].SubItems[0].Text)[0]
		//				&& item.GetCOMPort() == lvPorts.SelectedItems[0].SubItems[1].Text)
		//				item.WarmStart();
		//}

		//private void Run_Click()
		//{
		//	MPMemory mem = MPMemory.NONE;
		//	if (this.cbMemory == MPMemory.RAM) mem = MPMemory.RAM;
		//	if (this.cbMemory == MPMemory.FLASH) mem = MPMemory.FLASH;
		//	if (this.cbMemory == MPMemory.ROM) mem = MPMemory.ROM;

		//	foreach (Multi item in lMulti)
		//		if (item.GetAddress() != 0x80)
		//			foreach (ListViewItem lvItem in lvPorts.Items)
		//				if (lvItem.SubItems[1].Text == item.GetCOMPort())
		//					lvItem.SubItems[2].Text = item.Run(lbDir.CheckedItems[0] as string, mem).ToString();
		//}

		#endregion notuse

	
	
		//-------------

	
	}
}