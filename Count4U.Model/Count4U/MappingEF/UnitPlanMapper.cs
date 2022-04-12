using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Count4U.Model.Count4U.MappingEF
{
	public static class UnitPlanMapper
	{
	
		public static UnitPlan ToDomainObject(this App_Data.UnitPlan entity)
		{
			if (entity == null) return null;
			return new UnitPlan()
			{
				ID = entity.ID,
				UnitPlanCode = entity.UnitPlanCode,
				Name = entity.Name,
				Description = entity.Description,
				LayerCode = entity.LayerCode,
				ObjectCode = entity.ObjectCode,
				Container = entity.Container != null ? entity.Container : false,
				Visible = entity.Visible != null ? entity.Visible : true,
				Lock = entity.Lock != null ? entity.Lock : false,
				ParentCode = entity.ParentCode,
				StartX = Convert.ToDouble(entity.StartX),
				StartY = Convert.ToDouble(entity.StartY),
				Height = Convert.ToDouble(entity.Height),
				Width = Convert.ToDouble(entity.Width),
				Zoom = Convert.ToInt32(entity.Zoom),
				Rotate = Convert.ToInt32(entity.Rotate),
				ZIndex = Convert.ToInt32(entity.ZIndex),
				Tag = entity.Tag,
				StatusUnitPlanBit = Convert.ToInt32(entity.StatusUnitPlanBit),
				StatusGroupUnitPlanBit = Convert.ToInt32(entity.StatusGroupUnitPlanBit),
				Picture = entity.Picture,
				LocationCode = entity.LocationCode,
				FontSize = Convert.ToInt32(entity.FontSize),
				Color = entity.Color,
				Font = entity.Font,
				Value = entity.Value,
				Tooltip = entity.Tooltip,
				Title = entity.Title
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
		public static UnitPlan ToSimpleDomainObject(this App_Data.UnitPlan entity)
		{
			return new UnitPlan()
			{
				ID = entity.ID,
				UnitPlanCode = entity.UnitPlanCode,
				LayerCode = entity.LayerCode,
				ObjectCode = entity.ObjectCode,
				StartX = Convert.ToDouble(entity.StartX),
				StartY = Convert.ToDouble(entity.StartY),
				Height = Convert.ToDouble(entity.Height),
				Width = Convert.ToDouble(entity.Width),
				Zoom = Convert.ToInt32(entity.Zoom),
				Rotate = Convert.ToInt32(entity.Rotate),
				ZIndex = Convert.ToInt32(entity.ZIndex),
				Picture = entity.Picture,
				LocationCode = entity.LocationCode,
				FontSize = Convert.ToInt32(entity.FontSize),
				Color = entity.Color,
				Font = entity.Font,
				Value = entity.Value,
				Tooltip = entity.Tooltip,
				Title = entity.Title
			};
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
		public static App_Data.UnitPlan ToEntity(this UnitPlan domainObject)
		{
			if (domainObject == null) return null;
			return new App_Data.UnitPlan()
			{
				ID = domainObject.ID,
				UnitPlanCode = domainObject.UnitPlanCode,
				Name = domainObject.Name,
				Description = domainObject.Description,
				LayerCode = domainObject.LayerCode,
				ObjectCode = domainObject.ObjectCode,
				Container = domainObject.Container != null ? domainObject.Container : false,
				Lock = domainObject.Lock != null ? domainObject.Lock : false,
				Visible = domainObject.Visible != null ? domainObject.Visible : true,
				ParentCode = domainObject.ParentCode,
				StartX = Convert.ToDouble(domainObject.StartX),
				StartY = Convert.ToDouble(domainObject.StartY),
				Height = Convert.ToDouble(domainObject.Height),
				Width = Convert.ToDouble(domainObject.Width),
				Zoom = Convert.ToInt32(domainObject.Zoom),
				Rotate = Convert.ToInt32(domainObject.Rotate),
				ZIndex = Convert.ToInt32(domainObject.ZIndex),
				Tag = domainObject.Tag,
				StatusUnitPlanBit = Convert.ToInt32(domainObject.StatusUnitPlanBit),
				StatusGroupUnitPlanBit = Convert.ToInt32(domainObject.StatusGroupUnitPlanBit),
				Picture = domainObject.Picture,
				LocationCode = domainObject.LocationCode,
				FontSize = Convert.ToInt32(domainObject.FontSize),
				Color = domainObject.Color,
				Font = domainObject.Font,
				Value = domainObject.Value,
				Tooltip = domainObject.Tooltip,
				Title = domainObject.Title

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
		public static void ApplyChanges(this App_Data.UnitPlan entity, UnitPlan domainObject)
		{
			if (domainObject == null) return;
			entity.UnitPlanCode = domainObject.UnitPlanCode;
			entity.Name = domainObject.Name;
			entity.Description = domainObject.Description;
			entity.LayerCode = domainObject.LayerCode;
			entity.ObjectCode = domainObject.ObjectCode;
			entity.Container = domainObject.Container != null ? domainObject.Container : false;
			entity.Visible = domainObject.Visible != null ? domainObject.Visible : true;
			entity.Lock = domainObject.Lock != null ? domainObject.Lock : false;
			entity.ParentCode = domainObject.ParentCode;
			entity.StartX = Convert.ToDouble(domainObject.StartX);
			entity.StartY = Convert.ToDouble(domainObject.StartY);
			entity.Height = Convert.ToDouble(domainObject.Height);
			entity.Width = Convert.ToDouble(domainObject.Width);
			entity.Zoom = Convert.ToInt32(domainObject.Zoom);
			entity.Rotate = Convert.ToInt32(domainObject.Rotate);
			entity.ZIndex = Convert.ToInt32(domainObject.ZIndex);
			entity.Tag = domainObject.Tag;
			entity.StatusUnitPlanBit = Convert.ToInt32(domainObject.StatusUnitPlanBit);
			entity.StatusGroupUnitPlanBit = Convert.ToInt32(domainObject.StatusGroupUnitPlanBit);
			entity.Picture = domainObject.Picture;
			entity.LocationCode = domainObject.LocationCode;
			entity.FontSize = Convert.ToInt32(domainObject.FontSize);
			entity.Color = domainObject.Color;
			entity.Font = domainObject.Font;
			entity.Value = domainObject.Value;
			entity.Tooltip = domainObject.Tooltip;
			entity.Title = domainObject.Title;
		}
	}
}
