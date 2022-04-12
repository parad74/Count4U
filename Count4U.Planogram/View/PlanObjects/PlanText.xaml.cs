using System;
using System.Collections.Generic;
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
using Count4U.Planogram.Lib;
using Count4U.Planogram.Lib.Enums;

namespace Count4U.Planogram.View.PlanObjects
{
    /// <summary>
    /// Interaction logic for PlanWall.xaml
    /// </summary>
    public partial class PlanText : PlanObject
    {
        private string _text;
        private int _textFontSize;
        private string _textFontColor;

        public PlanText()
        {
            InitializeComponent();
        }

        public override enPlanObjectType PlanType { get { return enPlanObjectType.Text; } }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;

                tbText.Text = _text;

                border.BorderBrush = String.IsNullOrWhiteSpace(_text) ? Brushes.Gray : Brushes.Transparent;
            }
        }

        public int TextFontSize
        {
            get { return _textFontSize; }
            set
            {
                _textFontSize = value;

                if (_textFontSize >= 8)
                    tbText.FontSize = _textFontSize;
            }
        }

        public string TextFontColor
        {
            get { return _textFontColor; }
            set
            {
                _textFontColor = value;

                tbText.Foreground = new SolidColorBrush(ColorParser.StringToColor(_textFontColor));
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
                return _text;
            }
            set
            {
                this.Text = value;
            }
        }
    }
}
