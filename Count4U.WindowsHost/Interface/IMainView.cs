using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.View.Interface;

namespace Common.Main.Interface
{
	public interface IMainView : IView
	{
		string GetInputText();
		void SetOutputText(string text);
		void SetRunOptionsText(string text);
		bool GetIncludeWSDLCheckBox();
		bool GetIncludeTimeTracingCheckBox();
		void SetUrlText(string text)  ;
		void SetUrlMexText(string text);

		//void SetDbPathText(string text);
		//string GetDbPathText();
		//void SetFolderCSVText(string text);
		//string GetFolderCSVText();
		//DataGridView DataGridView { get; }
		//void ChangeDbPathText(string text); 
	}
}
