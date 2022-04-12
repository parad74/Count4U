using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using Count4U.Model.Common;

namespace Count4U.Model.Count4U.MappingEF
{
	public static class IturMapper
	{
		/// <summary>
		/// Конвертация в объект предметной области.
		/// 
		/// Converting to domain object.
		/// </summary>
		/// <param name="entity">
		/// Сущность базы данных.
		/// 
		/// Database entity.
		/// </param>
		/// <returns>
		/// Объект предметной области.
		/// 
		/// Domain object.
		/// </returns>
		public static Itur ToDomainObject(this App_Data.Itur entity)
		{
			if (entity == null) return null;
			return new Itur()
			{
				ID = entity.ID,
				Approve = entity.Approve,
				Disabled = entity.Disabled,
				IturCode = entity.IturCode,
				ERPIturCode = entity.ERPIturCode != null ? entity.ERPIturCode.CutLength(249) : "",
				Description = entity.Description,
				InitialQuantityMakatExpected = entity.InitialQuantityMakatExpected,
				LocationCode = entity.LocationCode != null ? entity.LocationCode : DomainUnknownCode.UnknownLocation,
				Name = entity.Name,
				Number = entity.Number,
				ModifyDate = entity.ModifyDate,
				NumberPrefix = entity.NumberPrefix,
				NumberSufix = entity.NumberSufix,
				CreateDate = entity.CreateDate != null ? Convert.ToDateTime(entity.CreateDate) : DateTime.Now,
				Publishe = entity.Publishe,
				//StatusIturCode = entity.StatusIturCode != null ? entity.StatusIturCode : DomainUnknownCode.UnknownStatus,
				StatusIturBit = entity.StatusIturBit,
				StatusDocHeaderBit = entity.StatusDocHeaderBit,
				StatusIturGroupBit = entity.StatusIturGroupBit,
				Restore = entity.Restore,
				RestoreBit = entity.RestoreBit != null ? entity.RestoreBit : false,
				UnitPlanCode = entity.UnitPlanCode,
				TotalItem = Convert.ToDouble(entity.TotalItem),
				SumQuantityEdit  = Convert.ToDouble(entity.SumQuantityEdit), 
				DiffQuantityEdit  = Convert.ToDouble(entity.DiffQuantityEdit),

				Width = entity.Width != null ? Convert.ToInt32(entity.Width) : 0,
				Height = entity.Height != null ? Convert.ToInt32(entity.Height) : 0,
				IncludeInFacing = Convert.ToBoolean(entity.IncludeInFacing),
				ShelfCount = Convert.ToInt32(entity.ShelfCount),
				ShelfInItur = Convert.ToInt32(entity.ShelfInItur),
				PlaceCount = Convert.ToInt32(entity.PlaceCount),
				PlaceInItur = Convert.ToInt32(entity.PlaceInItur),
				Supplier1PlaceCount = Convert.ToInt32(entity.Supplier1PlaceCount),
				Supplier2PlaceCount = Convert.ToInt32(entity.Supplier2PlaceCount),
				Supplier3PlaceCount = Convert.ToInt32(entity.Supplier3PlaceCount),
				Supplier4PlaceCount = Convert.ToInt32(entity.Supplier4PlaceCount),
				Supplier5PlaceCount = Convert.ToInt32(entity.Supplier5PlaceCount),
				SupplierOtherPlaceCount = Convert.ToInt32(entity.SupplierOtherPlaceCount),
				UnitPlaceWidth = entity.UnitPlaceWidth != null ? Convert.ToInt32(entity.UnitPlaceWidth) : 0,
				Area = entity.Area != null ? Convert.ToDouble(entity.Area) : 0.0,
				AreaCount = entity.AreaCount != null ? Convert.ToDouble(entity.AreaCount) : 0.0,
				Level1 = entity.Level1 != null ? entity.Level1 : "",
				Level2 = entity.Level2 != null ? entity.Level2 : "",
				Level3 = entity.Level3 != null ? entity.Level3 : "",
				Level4 = entity.Level4 != null ? entity.Level4 : "",
				Name1 = entity.Name1 != null ? entity.Name1 : "",
				Name2 = entity.Name2 != null ? entity.Name2 : "",
				Name3 = entity.Name3 != null ? entity.Name3 : "",
				Name4 = entity.Name4 != null ? entity.Name4 : "",
				NodeType = Convert.ToInt32(entity.NodeType),
				LevelNum = Convert.ToInt32(entity.LevelNum),
				Total = Convert.ToInt32(entity.Total),
				Tag = entity.Tag != null ? entity.Tag : "",
				InvStatus = Convert.ToInt32(entity.InvStatus),
				ParentIturCode = entity.ParentIturCode != null ? entity.ParentIturCode.CutLength(249) : "",
				TypeCode = entity.TypeCode != null ? entity.TypeCode : "",
				BackgroundColor = entity.BackgroundColor != null ? entity.BackgroundColor : "",

				//Barcode =   this.CreateBarcode(barcodePrefix + itur.IturCode);
			};
		}

		public static Itur ToDomainObject(this App_Data.Itur entity, Dictionary<string, Location> locationDictionary)
		{
			if (entity == null) return null;
			string locationCode = entity.LocationCode != null ? entity.LocationCode : DomainUnknownCode.UnknownLocation;
			string locationName = "";
			if(locationDictionary.ContainsKey(locationCode)  == true)  {
				locationName = locationDictionary[locationCode].Name != null ? locationDictionary[locationCode].Name : "";
			}
  			return new Itur()
			{
				ID = entity.ID,
				Approve = entity.Approve,
				Disabled = entity.Disabled,
				IturCode = entity.IturCode,
				ERPIturCode = entity.ERPIturCode != null ? entity.ERPIturCode.CutLength(249) : "",
				Description = entity.Description,
				InitialQuantityMakatExpected = entity.InitialQuantityMakatExpected,
				LocationCode = locationCode,
				Name = entity.Name,
				Number = entity.Number,
				ModifyDate = entity.ModifyDate,
				NumberPrefix = entity.NumberPrefix,
				NumberSufix = entity.NumberSufix,
				CreateDate = entity.CreateDate != null ? Convert.ToDateTime(entity.CreateDate) : DateTime.Now,
				Publishe = entity.Publishe,
				//StatusIturCode = entity.StatusIturCode != null ? entity.StatusIturCode : DomainUnknownCode.UnknownStatus,
				StatusIturBit = entity.StatusIturBit,
				StatusDocHeaderBit = entity.StatusDocHeaderBit,
				StatusIturGroupBit = entity.StatusIturGroupBit,
				Restore = entity.Restore,
				RestoreBit = entity.RestoreBit != null ? entity.RestoreBit : false,
				UnitPlanCode = entity.UnitPlanCode,
				TotalItem = Convert.ToDouble(entity.TotalItem),
				SumQuantityEdit = Convert.ToDouble(entity.SumQuantityEdit),
				DiffQuantityEdit = Convert.ToDouble(entity.DiffQuantityEdit),
				Width = entity.Width != null ? Convert.ToInt32(entity.Width) : 0,
				Height = entity.Height != null ? Convert.ToInt32(entity.Height) : 0,
				IncludeInFacing = Convert.ToBoolean(entity.IncludeInFacing),
				ShelfCount = Convert.ToInt32(entity.ShelfCount),
				ShelfInItur = Convert.ToInt32(entity.ShelfInItur),
				PlaceCount = Convert.ToInt32(entity.PlaceCount),
				PlaceInItur = Convert.ToInt32(entity.PlaceInItur),
				Supplier1PlaceCount = Convert.ToInt32(entity.Supplier1PlaceCount),
				Supplier2PlaceCount = Convert.ToInt32(entity.Supplier2PlaceCount),
				Supplier3PlaceCount = Convert.ToInt32(entity.Supplier3PlaceCount),
				Supplier4PlaceCount = Convert.ToInt32(entity.Supplier4PlaceCount),
				Supplier5PlaceCount = Convert.ToInt32(entity.Supplier5PlaceCount),
				SupplierOtherPlaceCount = Convert.ToInt32(entity.SupplierOtherPlaceCount),
				UnitPlaceWidth = entity.UnitPlaceWidth != null ? Convert.ToInt32(entity.UnitPlaceWidth) : 0,
				Area = entity.Area != null ? Convert.ToDouble(entity.Area) : 0.0,
				AreaCount = entity.AreaCount != null ? Convert.ToDouble(entity.AreaCount) : 0.0,
				Level1 = entity.Level1 != null ? entity.Level1 : "",
				Level2 = entity.Level2 != null ? entity.Level2 : "",
				Level3 = entity.Level3 != null ? entity.Level3 : "",
				Level4 = entity.Level4 != null ? entity.Level4 : "",
				Name1 = entity.Name1 != null ? entity.Name1 : "",
				Name2 = entity.Name2 != null ? entity.Name2 : "",
				Name3 = entity.Name3 != null ? entity.Name3 : "",
				Name4 = entity.Name4 != null ? entity.Name4 : "",
				NodeType = Convert.ToInt32(entity.NodeType),
				LevelNum = Convert.ToInt32(entity.LevelNum),
				Total = Convert.ToInt32(entity.Total),
				Tag = entity.Tag != null ? entity.Tag : "",
				InvStatus = Convert.ToInt32(entity.InvStatus),
				ParentIturCode = entity.ParentIturCode != null ? entity.ParentIturCode.CutLength(249) : "",
				TypeCode = entity.TypeCode != null ? entity.TypeCode : "",
				BackgroundColor = entity.BackgroundColor != null ? entity.BackgroundColor : "",
			};
		}

		/// <summary>
		/// Конвертация в упрощенный объект предметной области.
		/// 
		/// Converting to simple domain object.
		/// </summary>
		/// <param name="entity">
		/// Сущность базы данных.
		/// 
		/// Database entity.
		/// </param>
		/// <returns>
		/// Упрощенный объект предметной области.
		/// 
		/// Simple domain object.
		/// </returns>
		public static Itur ToSimpleDomainObject(this App_Data.Itur entity)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Конвертация в сущность базы данных.
		/// 
		/// Converting to database entity.
		/// </summary>
		/// <param name="domainObject">
		/// Объект предметной области.
		/// 
		/// Domain object.
		/// </param>
		/// <returns>Database entity.</returns>
		public static App_Data.Itur ToEntity(this Itur domainObject)
		{
			if (domainObject == null) return null;
			return new App_Data.Itur()
			{
				ID = domainObject.ID,
				Disabled = domainObject.Disabled != null ? (bool)domainObject.Disabled : false,
				Approve = domainObject.Approve,
				IturCode = domainObject.IturCode,
				ERPIturCode = domainObject.ERPIturCode != null ? domainObject.ERPIturCode.CutLength(249) : "",
				Description = domainObject.Description,
				InitialQuantityMakatExpected = domainObject.InitialQuantityMakatExpected,
				LocationCode = domainObject.LocationCode != null ? domainObject.LocationCode : DomainUnknownCode.UnknownLocation,
				Name = domainObject.Name,
				ModifyDate = domainObject.ModifyDate,
				NumberPrefix = domainObject.NumberPrefix,
				NumberSufix = domainObject.NumberSufix,
				Number = domainObject.Number,
				CreateDate = domainObject.CreateDate,
				Publishe = domainObject.Publishe,
				//StatusIturCode = domainObject.StatusIturCode != null ? domainObject.StatusIturCode : DomainUnknownCode.UnknownStatus,
				StatusIturBit = domainObject.StatusIturBit,
				StatusDocHeaderBit = domainObject.StatusDocHeaderBit,
				StatusIturGroupBit = domainObject.StatusIturGroupBit, 
				Restore = domainObject.Restore,
				RestoreBit = domainObject.RestoreBit != null ? domainObject.RestoreBit : false,
				UnitPlanCode = domainObject.Restore,
				TotalItem = Convert.ToDouble(domainObject.TotalItem),
				SumQuantityEdit = Convert.ToDouble(domainObject.SumQuantityEdit),
				DiffQuantityEdit = Convert.ToDouble(domainObject.DiffQuantityEdit) ,
				Width = domainObject.Width,
				Height = domainObject.Height, 
				IncludeInFacing = domainObject.IncludeInFacing,
				ShelfCount = domainObject.ShelfCount,
				ShelfInItur = domainObject.ShelfInItur,
				PlaceCount = domainObject.PlaceCount,
				PlaceInItur =domainObject.PlaceInItur,
				Supplier1PlaceCount = domainObject.Supplier1PlaceCount,
				Supplier2PlaceCount =domainObject.Supplier2PlaceCount,
				Supplier3PlaceCount = domainObject.Supplier3PlaceCount,
				Supplier4PlaceCount = domainObject.Supplier4PlaceCount,
				Supplier5PlaceCount =domainObject.Supplier5PlaceCount,
				SupplierOtherPlaceCount =domainObject.SupplierOtherPlaceCount,
				UnitPlaceWidth = domainObject.UnitPlaceWidth,
				Area = Convert.ToDouble(domainObject.Area),
				AreaCount = Convert.ToDouble(domainObject.AreaCount) ,
				Level1 = domainObject.Level1,
				Level2 = domainObject.Level2,
				Level3 = domainObject.Level3,
				Level4 = domainObject.Level4,
				Name1 = domainObject.Name1,
				Name2 = domainObject.Name2,
				Name3 = domainObject.Name3,
				Name4 = domainObject.Name4,
				NodeType = Convert.ToInt32(domainObject.NodeType),
				LevelNum = Convert.ToInt32(domainObject.LevelNum),
				Total = Convert.ToInt32(domainObject.Total),
				Tag = domainObject.Tag,
				InvStatus = Convert.ToInt32(domainObject.InvStatus),
				ParentIturCode = domainObject.ParentIturCode.CutLength(249),
				TypeCode = domainObject.TypeCode,
				BackgroundColor = domainObject.BackgroundColor,
			};
		}

		/// <summary>
		/// Применение изменений к сущности базы данных.
		/// 
		/// Apply changes to database entity.
		/// </summary>
		/// <param name="entity">
		/// Сущность базы данных.
		/// 
		/// Database entity.
		/// </param>
		/// <param name="domainObject">
		/// Объект предметной области.
		/// 
		/// Domain object.
		/// </param>
		public static void ApplyChanges(this App_Data.Itur entity, Itur domainObject)
		{
			if (domainObject == null) return;
			entity.Disabled = Convert.ToBoolean(domainObject.Disabled);
			entity.IturCode = domainObject.IturCode;
			entity.ERPIturCode = domainObject.ERPIturCode != null ? domainObject.ERPIturCode.CutLength(249) : "";
			entity.Approve = domainObject.Approve;
			entity.Description = domainObject.Description;
			entity.InitialQuantityMakatExpected = domainObject.InitialQuantityMakatExpected;
			entity.LocationCode = domainObject.LocationCode != null ? domainObject.LocationCode : DomainUnknownCode.UnknownLocation;
			entity.Name = domainObject.Name;
			entity.ModifyDate = domainObject.ModifyDate;
			entity.NumberPrefix = domainObject.NumberPrefix;
			entity.NumberSufix = domainObject.NumberSufix;
			entity.Number = domainObject.Number;
			entity.CreateDate = domainObject.CreateDate;
			entity.Publishe = domainObject.Publishe;
			//entity.StatusIturCode = domainObject.StatusIturCode != null ? domainObject.StatusIturCode : DomainUnknownCode.UnknownStatus;
			entity.StatusIturBit = domainObject.StatusIturBit;
			entity.StatusDocHeaderBit = domainObject.StatusDocHeaderBit;
			entity.StatusIturGroupBit = domainObject.StatusIturGroupBit;
			entity.Restore = domainObject.Restore;
			entity.RestoreBit = domainObject.RestoreBit != null ? domainObject.RestoreBit : false;
			entity.UnitPlanCode = domainObject.UnitPlanCode;
			entity.TotalItem = Convert.ToDouble(domainObject.TotalItem);
			entity.SumQuantityEdit = Convert.ToDouble(domainObject.SumQuantityEdit);
			entity.DiffQuantityEdit = Convert.ToDouble(domainObject.DiffQuantityEdit);
			entity.Width = domainObject.Width;
			entity.Height = domainObject.Height;
			entity.IncludeInFacing = domainObject.IncludeInFacing;
			entity.ShelfCount = domainObject.ShelfCount;
			entity.ShelfInItur = domainObject.ShelfInItur != 0 ? Convert.ToInt32(domainObject.ShelfInItur) : 1;
			entity.PlaceCount = domainObject.PlaceCount;
			entity.PlaceInItur = domainObject.PlaceInItur;
			entity.Supplier1PlaceCount = domainObject.Supplier1PlaceCount;
			entity.Supplier2PlaceCount = domainObject.Supplier2PlaceCount;
			entity.Supplier3PlaceCount = domainObject.Supplier3PlaceCount;
			entity.Supplier4PlaceCount = domainObject.Supplier4PlaceCount;
			entity.Supplier5PlaceCount = domainObject.Supplier5PlaceCount;
			entity.SupplierOtherPlaceCount = domainObject.SupplierOtherPlaceCount;
			entity.UnitPlaceWidth = domainObject.UnitPlaceWidth != null ? Convert.ToInt32(domainObject.UnitPlaceWidth) : 0;
			entity.Area = domainObject.Area != null ? Convert.ToDouble(domainObject.Area) : 0;
			entity.AreaCount = domainObject.AreaCount != null ? Convert.ToDouble(domainObject.AreaCount) : 0;
			entity.Level1 = domainObject.Level1;
			entity.Level2 = domainObject.Level2;
			entity.Level3 = domainObject.Level3;
			entity.Level4 = domainObject.Level4;
			entity.Name1 = domainObject.Name1;
			entity.Name2 = domainObject.Name2;
			entity.Name3 = domainObject.Name3;
			entity.Name4 = domainObject.Name4;
			entity.NodeType = Convert.ToInt32(domainObject.NodeType);
			entity.LevelNum = Convert.ToInt32(domainObject.LevelNum);
			entity.Total = Convert.ToInt32(domainObject.Total);
			entity.Tag = domainObject.Tag;
			entity.InvStatus = Convert.ToInt32(domainObject.InvStatus);
			entity.ParentIturCode = domainObject.ParentIturCode.CutLength(249);
			entity.TypeCode = domainObject.TypeCode;
			entity.BackgroundColor = domainObject.BackgroundColor;
		}


 //       public byte[] CreateBarcode(String barcode)
 //       {
 ////           Dim bdf As Code128BarcodeDraw = BarcodeDrawFactory.Code128WithChecksum
 ////PictureBox1.Image = bdf.Draw("Hello world!", 20)
 //           //BarcodeMetrics tamccbb = new BarcodeMetrics(2, 90);
 //           System.Drawing.Image imagen;

 //           string  barcodeType = this._userSettingsManager.BarcodeTypeGet();
 //           var barcodeTypeEnum = (BarcodeSymbology)System.Enum.Parse(typeof(BarcodeSymbology), barcodeType.Trim());
 //           imagen = BarcodeDrawFactory.GetSymbology(barcodeTypeEnum).Draw(barcode, 30);
 //           //imagen = BarcodeDrawFactory.GetSymbology(BarcodeSymbology.Code39NC).Draw(barcode, 20);	   
 //           ImageFormat format = ImageFormat.Bmp;

 //           MemoryStream mm = new MemoryStream();
 //           imagen.Save(mm, format);
 //           imagen.Dispose();

 //           byte[] bytearray = mm.ToArray();
 //           mm.Close();
 //           mm.Dispose();

 //           return bytearray;
 //       }
	
	}
}
