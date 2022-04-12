using System;
using System.ComponentModel;

namespace Count4U.Model
{
	public static class ModelUtils
    {
		private static readonly DateTime MinDataIime = new DateTime(1970, 1, 1, 0, 0, 0,
											DateTimeKind.Utc);

		public static DateTime GetMinDateTime()
		{
			return MinDataIime;
		}

	}
}