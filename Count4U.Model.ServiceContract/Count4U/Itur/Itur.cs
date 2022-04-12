using System;
using Count4U.Model.Extensions;
using Count4U.Model.ServiceContract;
//using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	[Serializable]
	public class Itur : IItur
	{
		public long ID { get; set; }
		public bool? Disabled { get; set; }
		public bool? Approve { get; set; }
		public string IturCode { get; set; }
		public string ERPIturCode { get; set; }
		public string Description { get; set; }
		public double? InitialQuantityMakatExpected { get; set; }
		//public string Location { get; set; }
		public string LocationCode { get; set; }
		public string Name { get; set; }
		public int Number { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime? ModifyDate { get; set; }
		public bool? Publishe { get; set; }
		public string StatusIturCode { get; set; }
		public int StatusIturBit { get; set; }
		public int StatusIturGroupBit { get; set; }
		public string NumberPrefix { get; set; }
		public string NumberSufix { get; set; }
		public int StatusDocHeaderBit { get; set; }
		public bool? RestoreBit { get; set; }
		public string Restore { get; set; }
		[PropertyNotBulk]
		public byte[] BarcodeByteNotDB { get; set; }

		public string UnitPlanCode { get; set; }
		public double TotalItem { get; set; }
		public double SumQuantityEdit { get; set; }
		public double DiffQuantityEdit { get; set; }

		public int Width { get; set; }
		public int Height { get; set; }
		public bool IncludeInFacing { get; set; }
		public int ShelfCount { get; set; }
		public int ShelfInItur { get; set; }
		public int PlaceCount { get; set; }
		public int PlaceInItur { get; set; }
		public int Supplier1PlaceCount { get; set; }
		public int Supplier2PlaceCount { get; set; }
		public int Supplier3PlaceCount { get; set; }
		public int Supplier4PlaceCount { get; set; }
		public int Supplier5PlaceCount { get; set; }
		public int SupplierOtherPlaceCount { get; set; }
		public int UnitPlaceWidth { get; set; }
		public double Area { get; set; }
		public double AreaCount { get; set; }

		//TO ADD in COUNT4U

		public string Level1 { get; set; }
		public string Level2 { get; set; }
		public string Level3 { get; set; }
		public string Level4 { get; set; }
		public string Name1 { get; set; }
		public string Name2 { get; set; }
		public string Name3 { get; set; }
		public string Name4 { get; set; }
		public int NodeType { get; set; }
		public int LevelNum { get; set; }
		public int Total { get; set; }
		public string Tag { get; set; }
		public int InvStatus { get; set; }

		public string ParentIturCode { get; set; }
		public string TypeCode { get; set; }
		public string BackgroundColor { get; set; }
		//#1601
		//ERPIturCode: 250


		public  Itur ()
		{
			IturCode	= "";
			ERPIturCode= ""; 
			Description = "";
			//Location = "";
			LocationCode = "";
			Name = "";
			Number = 1;
			CreateDate = DateTime.Now;
			ModifyDate = ModelUtils.GetMinDateTime();
			StatusIturCode = "";
			StatusIturBit  = 0;
			StatusIturGroupBit = 0;
			NumberPrefix = "";
			NumberSufix = "";
			StatusDocHeaderBit = 0;
			Restore = "";
			UnitPlanCode = "";
			TotalItem = 0;
			SumQuantityEdit = 0;
			DiffQuantityEdit = 0;

			Width = 0;
			Height = 0;
			ShelfCount = 0;
			ShelfInItur = 0;
			PlaceCount = 0;
			PlaceInItur= 0;
			Supplier1PlaceCount = 0;
			Supplier2PlaceCount = 0;
			Supplier3PlaceCount = 0;
			Supplier4PlaceCount = 0;
			Supplier5PlaceCount = 0;
			SupplierOtherPlaceCount = 0;
			UnitPlaceWidth = 0;
			Area = 0;
			AreaCount = 0;

			//TO ADD in COUNT4U

			Level1 = "";
			Level2 = "";
			Level3 = "";
			Level4 = "";
			Name1 = "";
			Name2 = "";
			Name3 = "";
			Name4 = "";
			NodeType = 0;
			LevelNum = 0;
			Total = 0;
			Tag = "";
			InvStatus = 0;

			ParentIturCode = "";
			TypeCode = "";
			BackgroundColor = "";
		}
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Itur)) return false;
			return Equals((Itur)obj);
		}

		public bool Equals(Itur other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.IturCode, this.IturCode);
		}

		public override int GetHashCode()
		{
			return (IturCode != null ? IturCode.GetHashCode() : 0);
		}

	}
}
