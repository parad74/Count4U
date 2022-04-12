using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using Count4U.Model.Count4U;

namespace Count4U.Planogram.Lib
{
    public abstract class PlanSpecialObject : PlanObject
    {
        protected readonly DrawingCanvas _canvas;

        protected PlanSpecialObject(DrawingCanvas canvas)
        {
            _canvas = canvas;
        }

        public abstract void SetBorderColor(SolidColorBrush brush);
        public abstract void SetBackgroundColor(double done, SolidColorBrush brush);
    }
}