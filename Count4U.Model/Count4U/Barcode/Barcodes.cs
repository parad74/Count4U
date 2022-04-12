using System;
using Count4U.Model.Interface.Count4U;
using System.Collections.ObjectModel;

namespace Count4U.Model.Count4U
{
	public class Barcodes : ObservableCollection<Barcode>
	{

		public static Barcodes FromEnumerable(System.Collections.Generic.IEnumerable<Barcode> List)
		{
			Barcodes barcodes = new Barcodes();
			foreach (Barcode item in List)
			{
				barcodes.Add(item);
			}
			return barcodes;
		}

		public Barcode CurrentItem { get; set; }

		public System.EventHandler CurrentChanged { get; set; }

		public long TotalCount { get; internal set; }
	}
}
