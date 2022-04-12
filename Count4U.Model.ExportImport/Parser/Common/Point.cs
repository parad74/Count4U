using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Count4U
{
	public class Point
	{
		public int Start { get; set; }
		public int End { get; set; }
		public int Length { get; set; }

		public Point()
		{
			this.Start = 0;
			this.End = 0;
			this.Length = 0;
		}

		public Point(int start, int end, int length)
		{
			this.Start = start;
			this.End = end;
			this.Length = length;
		}

		public Point(int start, int end)
		{
			this.Start = start;
			this.End = end;
			this.Length = end - start + 1;
		}

	}
}
