using System.Collections.Generic;
using System.Linq;
using System.Windows.Interactivity;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace Count4U.Common.Behaviours
{
    public class ColorPickerSortBehavior : Behavior<ColorPicker>
    {
        private bool _isInitialized;
        private static List<ColorItem> _colors;
        private static List<ColorItem> _standardColors;

        static ColorPickerSortBehavior()
        {
        }

        public ColorPickerSortBehavior()
        {

        }

        protected override void OnAttached()
        {
            base.OnAttached();

            if (base.AssociatedObject != null)
            {
                base.AssociatedObject.Loaded += AssociatedObject_Loaded;             
            }
        }

        void AssociatedObject_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_isInitialized) return;

            Init();

            _isInitialized = true;
        }

        private void Init()
        {
            if (_colors == null)
            {
                _colors = base.AssociatedObject.AvailableColors.OrderBy(r => r.Color.Value.R).ThenBy(r => r.Color.Value.G).ThenBy(r => r.Color.Value.B).ToList();
            }
            if (_standardColors == null)
            {
                _standardColors = base.AssociatedObject.StandardColors.ToList();
                _standardColors.Add(new ColorItem(Color.FromRgb(255, 204, 0), "Theme1"));
                _standardColors.Add(new ColorItem(Color.FromRgb(100, 193, 255), "Theme2"));
                _standardColors.Add(new ColorItem(Color.FromRgb(153, 153, 153), "Theme3"));
                _standardColors.Add(new ColorItem(Color.FromRgb(153, 204, 1), "Theme4"));
            }

            base.AssociatedObject.AvailableColors.Clear();

            //foreach (ColorItem colorItem in colors.OrderBy(r => System.Drawing.Color.FromArgb(r.Color.R, r.Color.G, r.Color.B).GetHue()))
            foreach (ColorItem colorItem in _colors)
            {
                base.AssociatedObject.AvailableColors.Add(colorItem);
            }

            base.AssociatedObject.StandardColors.Clear();
            foreach (ColorItem standardColor in _standardColors)
            {
                base.AssociatedObject.StandardColors.Add(standardColor);
            }
         
        }

        //        class ColorComparer : IComparer<Color>
        //        {
        //            #region Implementation of IComparer<in Color>
        //
        //            public int Compare(Color x, Color y)
        //            {
        //                if (x.B != y.B)
        //                    return x.B.CompareTo(y.B);
        //
        //                if (x.G != y.G)
        //                    return x.G.CompareTo(y.G);
        //
        //                if (x.R != y.R)
        //                    return x.R.CompareTo(y.R);
        //
        //                return 0;
        //            }
        //
        //            #endregion
        //        }
    }
}