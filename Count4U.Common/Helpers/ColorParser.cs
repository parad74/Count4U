using System;
using System.Linq;
using System.Windows.Media;
using System.Collections.Generic;

namespace Count4U.Common.Helpers
{
    public static class ColorParser
    {
        public static Color StringToColor(string background)
        {
			if (String.IsNullOrEmpty(background) == true)
			{
				return Colors.Black;
			}

			try
			{
				List<string> colors = background.Split(new[] { ',' }).Select(z => z.Trim()).ToList();
				byte r = Byte.Parse(colors[0]);
				byte g = Byte.Parse(colors[1]);
				byte b = Byte.Parse(colors[2]);
				return Color.FromRgb(r, g, b);
			}
			catch { return Colors.Black; }
       }

        public static string ColorToString(Color color)
        {
            return String.Format("{0}, {1}, {2}", color.R, color.G, color.B);
        }
    }
}