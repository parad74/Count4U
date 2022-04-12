using System;
using System.ComponentModel;
using Count4U.Common.Interfaces;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.Audit.ViewModels
{
	public class ShelfItemViewModel : NotificationObject, IDataErrorInfo,  IRowEdititing
    {
		private string _shelfCode;
		private string _shelfPartCode;
		private string _iturCode;
		private string _supplierCode;
		private string _supplierName;
		private DateTime _createDataTime;
		private int _shelfNum;
		private double _width;
		private double _height;
		private double _area;
		private Shelf _backup;
		

		public ShelfItemViewModel(Shelf shelf)
		{
			this._shelfCode = shelf.ShelfCode;
			this._shelfPartCode = shelf.ShelfPartCode;
			this._iturCode = shelf.IturCode;
			this._supplierCode = shelf.SupplierCode;
			this._supplierName = shelf.SupplierName;
			this._createDataTime = shelf.CreateDataTime;
			this._shelfNum = shelf.ShelfNum;
			this._width = shelf.Width;
			this._height = shelf.Height;
			this._area = shelf.Area;
		}

		public void ShelfSet(Shelf shelf)
		{
			this._shelfCode = shelf.ShelfCode;
			this._shelfPartCode = shelf.ShelfPartCode;
			this._iturCode = shelf.IturCode;
			this._supplierCode = shelf.SupplierCode;
			this._supplierName = shelf.SupplierName;
			this._createDataTime = shelf.CreateDataTime;
			this._shelfNum = shelf.ShelfNum;
			this._width = shelf.Width;
			this._height = shelf.Height;
			this._area = shelf.Area;
		}


		public Shelf GetShelf(ShelfItemViewModel item)
		{
			Shelf shelf = new Shelf();
			shelf.ShelfCode = item.ShelfCode;
			shelf.ShelfPartCode = item.ShelfPartCode;
			shelf.IturCode = item.IturCode;
			shelf.SupplierCode = item.SupplierCode;
			shelf.SupplierName = item.SupplierName;
			shelf.CreateDataTime = item.CreateDataTime;
			shelf.ShelfNum = item.ShelfNum;
			shelf.Width = item.Width;
			shelf.Height = item.Height;
			shelf.Area = (int)(item.Width * item.Height);
			return shelf;
		}
		#region Implementation of IRowEdititing

		public void BeginEditing()
		{
			this.RaisePropertyChanged(() => Width);
			this.RaisePropertyChanged(() => SupplierCode);
			this.RaisePropertyChanged(() => ShelfNum);
			this._backup = new Shelf();
			this._backup.Width = this._width;
			this._backup.SupplierCode = this._supplierCode;
			this._backup.ShelfNum = this._shelfNum;
		}

		public void CancelEditing()
		{
			if (_backup == null) return;
			this._width = this._backup.Width;
			this._supplierCode = this._backup.SupplierCode;
			this._shelfNum = this._backup.ShelfNum;
		}

		public void CommitEditing()
		{
			this._width = this.Width;
			this._supplierCode = this.SupplierCode;
			this._shelfNum = this.ShelfNum;
			this.RaisePropertyChanged(() => Width);
			this.RaisePropertyChanged(() => SupplierCode);
			this.RaisePropertyChanged(() => ShelfNum);
		}

		#endregion

		public string ShelfCode
		{
			get { return this._shelfCode; }
			set
			{
				this._shelfCode = value;
				this.RaisePropertyChanged(() => ShelfCode);
				this.RaisePropertyChanged(() => CreateDataTime);
			}
		}

		public string ShelfPartCode
		{
			get { return this._shelfPartCode; }
			set
			{
				this._shelfPartCode = value;
				this.RaisePropertyChanged(() => ShelfPartCode);
			}
		}

		public string IturCode
		{
			get { return this._iturCode; }
			set
			{
				this._iturCode = value;
				this.RaisePropertyChanged(() => IturCode);
			}
		}

		public string SupplierCode
		{
			get { return this._supplierCode; }
			set
			{
				this._supplierCode = value;
				this.RaisePropertyChanged(() => SupplierCode);
				this.RaisePropertyChanged(() => SupplierName);
				this.RaisePropertyChanged(() => CreateDataTime);
			}
		}

		public string SupplierName
		{
			get { return this._supplierName; }
			set
			{
				this._supplierName = value;
				this.RaisePropertyChanged(() => SupplierName);
			}
		}

		public DateTime CreateDataTime
		{
			get { return this._createDataTime; }
			set
			{
				this._createDataTime = value;
				this.RaisePropertyChanged(() => CreateDataTime);
			}
		}


		public int ShelfNum
		{
			get { return this._shelfNum; }
			set
			{
				this._shelfNum = value;
				this.RaisePropertyChanged(() => ShelfNum);
			}
		}

		public double Width
		{
			get { return this._width; }
			set
			{
				this._width = value;
				this.RaisePropertyChanged(() => Width);
				this.RaisePropertyChanged(() => CreateDataTime);
			}
		}

		public double Height
		{
			get { return this._height; }
			set
			{
				this._height = value;
				this.RaisePropertyChanged(() => Height);
			}
		}

		public double Area
		{
			get { return this._area; }
			set
			{
				this._area = value;
				this.RaisePropertyChanged(() => Area);
			}
		}


		public void UpdateUIAfterDbSave()
		{
			Area = (Height * Width) / 10000.0;

			this.RaisePropertyChanged(() => SupplierCode);
			//this.RaisePropertyChanged(() => SupplierName);
			this.RaisePropertyChanged(() => CreateDataTime);
			//this.RaisePropertyChanged(() => Height);
			this.RaisePropertyChanged(() => Width);
			this.RaisePropertyChanged(() => Area);
			this.RaisePropertyChanged(() => ShelfNum);
		}

		#region Implementation of IDataErrorInfo

		public string this[string propertyName]
		{
			get
			{
				switch (propertyName)
				{
					case "Width":
						{
							//                            string validate = InventProductValidate.QuantityEditValidate(this._quantityEdit.ToString());
							//                            if (!String.IsNullOrWhiteSpace(validate))
							//                                return validate;
							//
							//                            if (_quantityEdit <= 0)
							//                                return Localization.Resources.ViewModel_InventProductItem_QuantityEdit;

							return String.Empty;
						}
					case "SupplierCode":
						{
							return String.Empty;
						}
					case "ShelfNum":
						{
							return String.Empty;
						}
				}
				return String.Empty;
			}
		}
		public string Error
		{
			get { return String.Empty; }
		}
		#endregion

	}
}