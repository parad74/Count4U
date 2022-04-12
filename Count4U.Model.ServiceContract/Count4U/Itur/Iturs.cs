using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.IO;
using Count4U.Model.Count4U;
using Zen.Barcode;
//using Count4U.Common.UserSettings;

namespace Count4U.Model.Count4U
{
	public class Iturs : ObservableCollection<Itur>
	{
		public static Iturs FromEnumerable(IEnumerable<Itur> list)
		{
			var collection = new Iturs();
			return Fill(collection ,  list);
		}

		public static Iturs Fill(Iturs collection, IEnumerable<Itur> list)
		{
			foreach (Itur item in list)
				collection.Add(item);
			return collection;
		}

		public static Iturs FillBarcode(Iturs iturs)
		{
			//Iturs itursNew = iturs;
			foreach (Itur item in iturs)
			{
				item.BarcodeByteNotDB = CreateBarcode(item.IturCode);
			}
			return iturs;
		}

		public Itur CurrentItem { get; set; }

		public EventHandler CurrentChanged { get; set; }

		public long TotalCount { get;  set; }

		public static byte[] CreateBarcode(String barcode)
		{
			//           Dim bdf As Code128BarcodeDraw = BarcodeDrawFactory.Code128WithChecksum
			//PictureBox1.Image = bdf.Draw("Hello world!", 20)
			//BarcodeMetrics tamccbb = new BarcodeMetrics(2, 90);
			System.Drawing.Image imagen;

			//string barcodeType = BarcodeDrawFactory.Code128WithChecksum.ToString();
			//var barcodeTypeEnum = BarcodeSymbology.Code128;
			imagen = BarcodeDrawFactory.GetSymbology(BarcodeSymbology.Code128).Draw(barcode, 30);
			//imagen = BarcodeDrawFactory.GetSymbology(BarcodeSymbology.Code39NC).Draw(barcode, 20);	   
			ImageFormat format = ImageFormat.Bmp;

			MemoryStream mm = new MemoryStream();
			imagen.Save(mm, format);
			imagen.Dispose();

			byte[] bytearray = mm.ToArray();
			mm.Close();
			mm.Dispose();

			return bytearray;
		}
	}

	public static class ItursStaticFunction
	{
		public static Iturs AddEnumerableIturs(this Iturs collection, IEnumerable<Itur> list)
		{
			foreach (Itur item in list)
				collection.Add(item);
			return collection;
		}
	}
	
}




