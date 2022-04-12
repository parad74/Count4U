using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Count4U.Common.Helpers;
using Count4U.Model.Count4U;
using Count4U.Model.Interface;
using Count4U.Planogram.Lib;
using Count4U.Planogram.Lib.Enums;

namespace Count4U.Planogram.View.PlanObjects
{
    /// <summary>
    /// Interaction logic for PlanWall.xaml
    /// </summary>
    public partial class PlanPicture : PlanObject
    {
        private string _fileName;
        private readonly IDBSettings _dbSettings;

        public PlanPicture(IDBSettings dbSettings)
        {
            _dbSettings = dbSettings;
            InitializeComponent();
        }

        public override enPlanObjectType PlanType { get { return enPlanObjectType.Picture; } }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;

                if (_fileName == null)
                    _fileName = String.Empty;

                string path = System.IO.Path.Combine(_dbSettings.PlanogramPictureFolderPath(), _fileName);

                if (File.Exists(path))
                {
                    img.Source = new BitmapImage(new Uri(path));
                    border.BorderBrush = Brushes.Transparent;                    
                }
                else
                {
                    img.Source = null;
                    border.BorderBrush = Brushes.Gray;                                        
                }
            }
        }

        public void SetBackgroundColor(SolidColorBrush brush)
        {
            SolidColorBrush borderBrush = null;

            if (brush == null)
            {
                brush = new SolidColorBrush() { Color = Colors.Transparent };
                borderBrush = new SolidColorBrush() { Color = Colors.Gray };
            }
            else
            {
                borderBrush = new SolidColorBrush() { Color = Colors.Transparent };
            }
            this.Background = brush;
            this.border.BorderBrush = borderBrush;
        }

        public override string PlanName
        {
            get
            {
                return _fileName;
            }
            set
            {
                this.FileName = value;
            }
        }
    }
}
