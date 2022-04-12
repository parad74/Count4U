using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using Count4U.Common.Extensions;
using Count4U.Common.Helpers.Actions;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel.Filter.Sorting;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Main;
using Count4U.Model.SelectionParams;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using NLog;
//using WPF.MDI;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;
using Type = System.Type;

namespace Count4U.Common.Helpers
{
    public static class UtilsMisc
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private static List<ColorItem> _colors;     

        public static MessageBoxResult ShowMessageBox(string message, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage, IUserSettingsManager settings, Window owner = null)
        {
            if (settings == null)
            {
                throw new InvalidOperationException();                
            }

            enLanguage language = settings.LanguageGet();
            MessageBoxOptions opt = language == enLanguage.Hebrew ? MessageBoxOptions.RtlReading : MessageBoxOptions.None;
            return MessageBox.Show(owner ?? Application.Current.MainWindow,
                message, "Count4U", messageBoxButton, messageBoxImage, MessageBoxResult.OK, opt);
        }


        public static void InitColorPicker(ColorPicker colorPicker)
        {
            if (_colors == null)
            {
                _colors = new List<ColorItem>();
                _colors = colorPicker.AvailableColors.OrderBy(r => r.Color.Value.R).ThenBy(r => r.Color.Value.G).ThenBy(r => r.Color.Value.B).ToList();
            }

            colorPicker.AvailableColors.Clear();

            //foreach (ColorItem colorItem in colors.OrderBy(r => System.Drawing.Color.FromArgb(r.Color.R, r.Color.G, r.Color.B).GetHue()))
            foreach (ColorItem colorItem in _colors)
            {
                colorPicker.AvailableColors.Add(colorItem);
            }
        }        

        public static Customers FilterCustomers(Customers input, string filter)
        {
            if (input == null)
                return null;

            if (String.IsNullOrWhiteSpace(filter))
                return input;

            Customers filtered = new Customers();

            foreach (Customer customer in input)
            {
                bool added = false;

                if (!String.IsNullOrWhiteSpace(customer.Code))
                {
                    if (customer.Code.ToLower().Contains(filter.ToLower()))
                    {
                        filtered.Add(customer);
                        added = true;
                    }
                }

                if (!added)
                {
                    if (!String.IsNullOrWhiteSpace(customer.Name))
                    {
                        if (customer.Name.ToLower().Contains(filter.ToLower()))
                        {
                            filtered.Add(customer);
                            added = true;
                        }
                    }
                }
            }

            return filtered;
        }

        public static Branches FilterBranches(Branches input, string filter)
        {
            if (input == null)
                return null;

            if (String.IsNullOrWhiteSpace(filter))
                return input;

            Branches filtered = new Branches();

            foreach (Branch branch in input)
            {
                bool added = false;

                if (!String.IsNullOrWhiteSpace(branch.Name))
                {
                    if (branch.Name.ToLower().Contains(filter.ToLower()))
                    {
                        filtered.Add(branch);
                        added = true;
                    }
                }

                if (!added)
                {
                    if (!String.IsNullOrWhiteSpace(branch.BranchCodeERP))
                    {
                        if (branch.BranchCodeERP.ToLower().Contains(filter.ToLower()))
                        {
                            filtered.Add(branch);
                            added = true;
                        }
                    }
                }

                if (!added)
                {
                    if (!String.IsNullOrWhiteSpace(branch.BranchCodeLocal))
                    {
                        if (branch.BranchCodeLocal.ToLower().Contains(filter.ToLower()))
                        {
                            filtered.Add(branch);
                            added = true;
                        }
                    }
                }
            }

            return filtered;
        }


		public static Locations FilterLocationsByName(Locations input, string filter)
		{
			if (input == null)
				return null;

			if (String.IsNullOrWhiteSpace(filter))
				return input;

			Locations filtered = new Locations();

			foreach (Count4U.Model.Count4U.Location location in input)
			{
				//bool added = false;

				if (String.IsNullOrWhiteSpace(location.Name) == false)
				{
					if (location.Name.ToLower().Contains(filter.ToLower()))
					{
						filtered.Add(location);
						//added = true;
					}
				}
			}

			return filtered;
		}

        public static MenuItem FindMenuItemByName(ContextMenu contextMenu, string name)
        {
            if (contextMenu.Items == null) return null;

            foreach (object item in contextMenu.Items)
            {
                MenuItem menuItem = item as MenuItem;
                if (menuItem != null && menuItem.Name == name)
                    return menuItem;
            }
            return null;
        }

        public static string LocalizationFromLocalizationKey(string localizationKey)
        {
            string result = String.Empty;
            System.Type t = typeof(Localization.Resources);
            PropertyInfo pi = t.GetProperty(localizationKey);
            if (pi != null)
            {
                result = pi.GetValue(null, null) as String;
            }

            return result;
        }

        public static Tuple<bool, string> OpenFileDialog(OpenFileDialogNotification notification, Window window = null)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = notification.DefaultExt;
            dlg.Filter = notification.Filter;
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;
            dlg.Multiselect = false;
            if (!String.IsNullOrEmpty(notification.InitialDirectory))
                dlg.InitialDirectory = notification.InitialDirectory;

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog(window ?? Application.Current.MainWindow);

            return new Tuple<bool, string>(result == true, dlg.FileName);
        }
    }
}