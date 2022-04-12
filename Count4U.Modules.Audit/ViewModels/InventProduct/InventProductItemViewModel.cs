using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Configuration.Dynamic;
using Count4U.Configuration.Interfaces;
using Count4U.Model.Count4U;
using Count4U.Model.Count4U.Validate;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Count4U.Model;
using System.Reflection;
using System.Windows.Media.Imaging;
using Count4U.Common.UserSettings;

namespace Count4U.Modules.Audit.ViewModels
{
    public class InventProductItemViewModel : NotificationObject/*, IEditableObject*/, IDataErrorInfo, IDynamicObject, IRowEdititing
    {
        private static readonly System.Type InventProductType = typeof(Count4U.Model.Count4U.InventProduct);

        private readonly EditorTemplateComboViewModel _editorCombo;
		public string _editorTemplateCurrent;
		public const string _templateCurrentCompare = "Compare";


        private InventProduct _inventProduct;
        private InventProduct _backup;

        private string _barCode;
		private string _toolTipPaint;
		private string _toolTip;
		private string _showToolTip;
		private string _showAccept;
		
        private string _productName;
		private string _serialNumber;
		private string _supplierCode;
		
		private string _createDate;
        private string _inputType;
        private double _quantityOriginal;
        private double _quantityEdit;

        private string _quantityDifference;
        private string _makat;
        private string _status;
        private string _num;
		private string _remark;
		private string _infoMark;
		private string _infoRemarkColor;
	

        private string _iturCode;
		private string _erpIturCode;
        private string _documentCode;

        private string _statusTooltip;

        private string _documentNum;

		private bool _mark;
		private bool _checkSelect;
		private string _markPropertyName;
		private string _photoPropertyName;
		private string _sourseFolder;
		private bool _propertyIsEmpty;
		
		private string _remarkColor;

		private int _ipValueInt1;
		private int _ipValueInt2;
		//private int _ipValueInt3;
		private int _ipValueInt4;

		private string _ipValueStr1;			   //IPValueStr1 - File1
		private string _ipValueStr2;			  //IPValueStr2 - File2
		private string _ipValueStr3;			  //IPValueStr3 - Mark
		private string _ipValueStr4;			  //IPValueStr4 - Description Inv1
		private string _ipValueStr5;			  //IPValueStr5 - Description Inv2
		private string _ipValueStr10;

		//IPValueInt1 - In1
		//IPValueInt2 - In2
		//IPValueFloat3 - Difference
		//IPValueInt4 - Result

		//IPValueStr1 - File1
		//IPValueStr2 - File2
		//IPValueStr3 - Mark

        private readonly ObservableCollection<DynamicProperty> _dynamicList;

		public InventProductItemViewModel(InventProduct inventProduct, EditorTemplateComboViewModel editorCombo,
			string photoPropertyName = "IPValueStr15", string sourseFolder = "",
			bool mark = false, string markPropertyName = "IPValueStr18", bool propertyIsEmpty = false, string remarkColor = "200,200,200")
		{
			this._editorCombo = editorCombo;
			this._editorTemplateCurrent = "NULL";
			if (editorCombo != null)
			{
				this._editorTemplateCurrent = editorCombo.EditorTemplateCurrent.Code;
			}
			this._mark = mark;
			this._markPropertyName = markPropertyName;
			this._photoPropertyName = photoPropertyName;
			this._sourseFolder = sourseFolder;
			this._propertyIsEmpty = propertyIsEmpty;
			this._remarkColor = remarkColor;
			this._checkSelect = false;

			this._dynamicList = new ObservableCollection<DynamicProperty>();
			this.InventProductSet(inventProduct);
			
		}

        public InventProduct InventProduct
        {
			get { return this._inventProduct; }
        }

		public string RemarkColor
		{
			get
			{
				//var color = this._userSettingsManager.InventProductMarkColorGet();
				return this._remarkColor;
			}
		}


	

        public string BarCode
        {
            get { return this._barCode; }
            set
            {
                this._barCode = value;
                RaisePropertyChanged(() => BarCode);
            }
        }

		public int IPValueInt4
		{
			get { return this._ipValueInt4; }
			set
			{
				this._ipValueInt4 = value;
				RaisePropertyChanged(() => IPValueInt4);
			}
		}

		public int IPValueInt1
		{
			get { return this._ipValueInt1; }
			set
			{
				this._ipValueInt1 = value;
				RaisePropertyChanged(() => IPValueInt1);
			}
		}



		public int IPValueInt2
		{
			get { return this._ipValueInt2; }
			set
			{
				this._ipValueInt2 = value;
				RaisePropertyChanged(() => IPValueInt2);
			}
		}


  		public string IPValueStr1
		{
			get { return this._ipValueStr1; }
			set
			{
				this._ipValueStr1 = value;
				RaisePropertyChanged(() => IPValueStr1);
			}
		}



		public string IPValueStr2
		{
			get { return this._ipValueStr2; }
			set
			{
				this._ipValueStr2 = value;
				RaisePropertyChanged(() => IPValueStr2);
			}
		}


		public string IPValueStr4
		{
			get { return this._ipValueStr4; }
			set
			{
				this._ipValueStr4 = value;
				RaisePropertyChanged(() => IPValueStr4);
			}
		}

		public string IPValueStr5
		{
			get { return this._ipValueStr5; }
			set
			{
				this._ipValueStr5 = value;
				RaisePropertyChanged(() => IPValueStr5);
			}
		}

		public string IPValueStr10
		{
			get { return this._ipValueStr10; }
			set
			{
				this._ipValueStr10 = value;
				RaisePropertyChanged(() => IPValueStr10);
			}
		}


			//		string currentCustomerCode = "";
			//if (base.State.CurrentCustomer != null) currentCustomerCode = base.State.CurrentCustomer.Code;
			
			//string importFolderPath = _dbSettings.ImportFolderPath();
			//string path = Path.Combine(importFolderPath, "Customer", currentCustomerCode);
			//string targetFolder = Path.GetFullPath(path);
		public string ToolTipPaint
		{
			get { return this._toolTipPaint; }
			set
			{
				this._toolTipPaint = value;
				RaisePropertyChanged(() => ToolTipPaint);
			}
		}

		public string ToolTip
		{
			get { return this._toolTip; }
			set
			{
				this._toolTip = value;
				RaisePropertyChanged(() => ToolTip);
			}
		}

		public string ShowToolTip
		{
			get { return this._showToolTip; }
			set
			{
				this._showToolTip = value;
				RaisePropertyChanged(() => ShowToolTip);
			}
		}

		public string ShowAccept
		{
			get { return this._showAccept; }
			set
			{
				this._showAccept = value;
				RaisePropertyChanged(() => ShowAccept);
			}
		}

		

		//public string ShowToolTip
		//{
		//	get { return @"pack://application:,,,/Count4U.Media;component/Icons/photo.png"; }
		//}

		private  string BuildBitmapImage(string name)
		{
			return String.Format(@"/Count4U.Media;component/Icons/{0}.png", name);
			//return new BitmapImage(new Uri(String.Format("pack://application:,,,/Count4U.Media;component/Icons/{0}.png", name)));
		}

        public string ProductName
        {
            get { return this._productName; }
            set
            {
                this._productName = value;
                this.RaisePropertyChanged(() => ProductName);
            }
        }

		public string SerialNumber
		{
			get { return this._serialNumber; }
			set
			{
				this._serialNumber = value;
				this.RaisePropertyChanged(() => SerialNumber);
			}
		}


		public string SupplierCode
		{
			get { return this._supplierCode; }
			set
			{
				this._supplierCode = value;
				this.RaisePropertyChanged(() => SupplierCode);
			}
		}

        public string CreateDate
        {
            get { return this._createDate; }
            set
            {
                this._createDate = value;
                this.RaisePropertyChanged(() => CreateDate);
            }
        }

        public string InputType
        {
            get { return this._inputType; }
            set
            {
                this._inputType = value;
                this.RaisePropertyChanged(() => InputType);
            }
        }

        public double QuantityOriginal
        {
            get { return this._quantityOriginal; }
            set
            {
                this._quantityOriginal = value;
                this.RaisePropertyChanged(() => QuantityOriginal);
            }
        }

        public double QuantityEdit
        {
            get { return this._quantityEdit; }
            set
            {
              this._quantityEdit = value;
				this.RaisePropertyChanged(() => QuantityEdit);

                this.RaisePropertyChanged(() => QuantityDifference);
                this.RaisePropertyChanged(() => IsQuantityChanged);
				this.RaisePropertyChanged(() => IsQuantityCompareChanged); 
			
				
            }
        }



		//public double QuantityEditCompare
		//{
		//	get { return this._quantityEdit; }
		//	set
		//	{
		//		this._quantityEdit = value;
		//		this.RaisePropertyChanged(() => QuantityEditCompare);

		//		this.RaisePropertyChanged(() => QuantityDifference);
		//		//this.RaisePropertyChanged(() => IsQuantityChanged);
		//		this.RaisePropertyChanged(() => IsQuantityCompareChanged); 
				
		//	}
		//}

        public double QuantityDifference
        {
            get
            {
                return _quantityEdit - _quantityOriginal;
            }
        }

        public string Makat
        {
            get { return this._makat; }
            set
            {
                this._makat = value;
                RaisePropertyChanged(() => Makat);
            }
        }

		

        public string Status
        {
            get { return this._status; }
            set
            {
                this._status = value;
                RaisePropertyChanged(() => this.Status);
            }
        }

        public string Num
        {
            get { return _num; }
            set
            {
                _num = value;
                RaisePropertyChanged(() => Num);
            }
        }

        public string StatusTooltip
        {
            get { return _statusTooltip; }
            set
            {
                _statusTooltip = value;
                RaisePropertyChanged(() => StatusTooltip);
            }
        }

        public bool IsQuantityChanged
        {
            get
            {
                return _quantityEdit != _quantityOriginal;
            }
        }

		
		public bool IsQuantityCompareChanged
        {
            get
            {
                return _quantityEdit != _quantityOriginal;
            }
        }


		public bool HasMarkColumn
		{
			get
			{
				if (String.IsNullOrWhiteSpace(this._infoMark) == true) return false;
				if (this._infoMark == "4") return false;
				if (this._infoMark == "8") return false;
				if (this._infoMark == "16") return false;
				if (this._infoMark == "32") return false;
				return true;
			}
		}


		public bool HasRemark
		{
			get
			{
				if (this._mark == false) return false;
				bool ret = false;
				if (this._propertyIsEmpty == true)
				{
					ret = (string.IsNullOrWhiteSpace(this._remark) == true);
				}
				else
				{
					ret = (string.IsNullOrWhiteSpace(this._remark) == false);
				}
				return ret;
			}
		}

		public string Remark
		{
			get { return _remark; }
			set
			{
				this._remark = value;
				RaisePropertyChanged(() => Remark);
			}
		}


		public string InfoMark
		{
			get { return _infoMark; }
			set
			{
				this._infoMark = value;
				this._infoRemarkColor = "219,244,160";
				if (this._infoMark == "") { this.InfoRemarkColor = "219,244,160"; }
				if (this._infoMark == "4") { this.InfoRemarkColor = "242,177,65"; }
				if (this._infoMark == "8") { this.InfoRemarkColor = "242,127,65"; }
				if (this._infoMark == "16") { this.InfoRemarkColor = "242,198,65"; }
				if (this._infoMark == "Accept1") { this.InfoRemarkColor = "53,213,181"; }
				if (this._infoMark == "Accept2") { this.InfoRemarkColor = "166,213,53"; }
				if (this._infoMark == "AcceptQE") { this.InfoRemarkColor = "245,125,65"; }// "219,244,160"; }

				RaisePropertyChanged(() => this.InfoMark);
				RaisePropertyChanged(() => this.InfoRemarkColor);
				RaisePropertyChanged(() => this.HasMarkColumn);
				
			}
		}

		//var markColor = this._userSettingsManager.InventProductMarkColorGet();
		//var markColorString = ColorParser.ColorToString(markColor);
		public string InfoRemarkColor
		{
			get
			{
				return this._infoRemarkColor;
	
			}
			set
			{
				this._infoRemarkColor = value;
				RaisePropertyChanged(() => InfoRemarkColor);
			}
		}


        public string IturCode
        {
            get { return _iturCode; }
            set
            {
                _iturCode = value;
                RaisePropertyChanged(() => IturCode);
            }
        }


		public string ERPIturCode
        {
			get { return _erpIturCode; }
            set
            {
				_erpIturCode = value;
				RaisePropertyChanged(() => ERPIturCode);
            }
        }
		

		public bool CheckSelect
        {
			get { return _checkSelect; }
            set
            {
				_checkSelect = value;
				RaisePropertyChanged(() => CheckSelect);
            }
        }
		

        public string DocumentCode
        {
            get { return _documentCode; }
            set
            {
                _documentCode = value;
                RaisePropertyChanged(() => DocumentCode);
            }
        }

        //not part of InventProduct fields!
        public string DocumentNum
        {
            get { return _documentNum; }
            set
            {
                _documentNum = value;
                RaisePropertyChanged(() => DocumentNum);
            }
        }


		//		string currentCustomerCode = "";
		//if (base.State.CurrentCustomer != null) currentCustomerCode = base.State.CurrentCustomer.Code;

		//string importFolderPath = _dbSettings.ImportFolderPath();
		//string path = Path.Combine(importFolderPath, "Customer", currentCustomerCode);
		//string targetFolder = Path.GetFullPath(path);

        public void InventProductSet(InventProduct inventProduct)
        {
            this._inventProduct = inventProduct;

            this.BarCode = this._inventProduct.Barcode;
			//this.ToolTip = "back.png";
			//this.ToolTipPaint = @"pack://application:,,,/Count4U.Media;component/Icons/back.png";//   this._inventProduct.IPValueStr4;
			//this.ShowToolTip = "/Count4U.Media;component/Icons/photo.png"; // @"pack://application:,,,/Count4U.Media;component/Icons/back.png";
			this.ShowToolTip = "";
			string photoPath = this.GetPropertyStringValueByName(this._inventProduct, this._photoPropertyName);
			if (string.IsNullOrWhiteSpace(photoPath) == false)
			{
				this.ShowToolTip = String.Format(@"/Count4U.Media;component/Icons/{0}.png", "photo");

				this.ToolTip = photoPath;
				if (this._sourseFolder != "")
				{
					this.ToolTipPaint = this._sourseFolder + @"\" + photoPath;
				}
			//	this.ToolTipPaint = @"pack://application:,,,/Count4U.Model;component/ImportData/Customer/" + this._customerCode + @"/" + photoPath;//   this._inventProduct.IPValueStr4;
			}
            this.Makat = this._inventProduct.Makat;
            this.ProductName = this._inventProduct.ProductName;
			this.SerialNumber = this._inventProduct.SerialNumber;
			this.SupplierCode = this._inventProduct.SupplierCode;
			
            this.QuantityEdit = this._inventProduct.QuantityEdit;
			//this.QuantityEditCompare = this._inventProduct.QuantityEdit;

            this.CreateDate = UtilsConvert.DateToStringLong(this._inventProduct.CreateDate);
            this.InputType = this._inventProduct.InputTypeCode;
            this.QuantityOriginal = this._inventProduct.QuantityOriginal;
            //this.QuantityDifference = this._inventProduct.QuantityDifference;
            this.Status = BitStatus.ToString(this._inventProduct.StatusInventProductBit);
            this.StatusTooltip = String.Join(Environment.NewLine, Bit2List.GetStatusList(this._inventProduct.StatusInventProductBit, DomainStatusEnum.PDA));
            this.Num = this._inventProduct.IPNum.ToString();

			this.Remark = this.GetPropertyStringValueByName(this._inventProduct, this._markPropertyName);

			this.InfoMark = this._inventProduct.IPValueStr3;	 //"";

            this.IturCode = this._inventProduct.IturCode;
			this.ERPIturCode = this._inventProduct.ERPIturCode;
			
            this.DocumentCode = this._inventProduct.DocumentCode;
			this.DocumentNum = this._inventProduct.DocNum.ToString();

			this.IPValueInt1 = this._inventProduct.IPValueInt1;
			this.IPValueInt2 = this._inventProduct.IPValueInt2;
			this.IPValueInt4 = this._inventProduct.IPValueInt4;

			this.IPValueStr1 = this._inventProduct.IPValueStr1;
			this.IPValueStr2 = this._inventProduct.IPValueStr2;
			this.IPValueStr4 = this._inventProduct.IPValueStr4;
			this.IPValueStr5 = this._inventProduct.IPValueStr5;
			this.IPValueStr10 = this._inventProduct.IPValueStr10;


			this.ShowAccept = "";
			if (this._inventProduct.IPValueInt1 != this._inventProduct.IPValueInt2)
			{
				this.ShowAccept = String.Format(@"/Count4U.Media;component/Icons/{0}.png", "check16");
			}


			RaisePropertyChanged(() => this.InfoMark);
			RaisePropertyChanged(() => InfoRemarkColor);

            RaisePropertyChanged(() => QuantityDifference);
            RaisePropertyChanged(() => IsQuantityChanged);
			RaisePropertyChanged(() => IsQuantityCompareChanged);
			
        }

		private string 	GetPropertyStringValueByName(InventProduct inventProduct, string propertyName)
		{
			string propertyValue= "";
			if (string.IsNullOrWhiteSpace(propertyName) == true) return propertyValue;
			System.Type t = typeof(InventProduct);
			PropertyInfo pi = t.GetProperty(propertyName);
			if (pi == null) return propertyValue;
			propertyValue = pi.GetValue(inventProduct, null).ToString();
			return propertyValue;
		}

	

        public void BeginEditing()
        {
            RaisePropertyChanged(() => QuantityEdit);
            _backup = new InventProduct();
            _backup.QuantityEdit = _quantityEdit;
            if (_editorCombo != null)
            {
                this._editorCombo.DynamicRepository.FillDbPropertiesForGrid(this, _backup);
            }
        }


        public void CancelEditing()
        {
            if (_backup == null) return;

            _quantityEdit = _backup.QuantityEdit;
            _dynamicList.Clear();
            if (_editorCombo != null)
            {
                _editorCombo.DynamicRepository.FillObjectWithDynamicProperties(this, _backup);
                _editorCombo.DynamicRepository.RaisePropertyChanged(this);
            }
        }

        public void CommitEditing()
        {
            this._inventProduct.QuantityEdit = QuantityEdit;
			RaisePropertyChanged(() => QuantityEdit);
			
            if (_editorCombo != null)
            {
				if (_editorTemplateCurrent == _templateCurrentCompare)
				{
					if (QuantityEdit == null) QuantityEdit = 0.0;
					if (QuantityEdit == 0.0)
					{
						this._inventProduct.QuantityOriginal = 0.0;
						this._inventProduct.IPValueInt4 = 0;
						this._inventProduct.IPValueStr3 = "";
						this.InventProductSet(this._inventProduct);
					}
					else
					{
						this._inventProduct.IPValueInt4 = Convert.ToInt32(QuantityEdit);
						this._inventProduct.QuantityOriginal = QuantityEdit;
						this._inventProduct.IPValueStr3 = "AcceptQE";
						this.InventProductSet(this._inventProduct);
					}
					//this._inventProductRepository.Update(inventProduct, GetDbPath);
					//viewModel.InventProductSet(inventProduct);
				}
                this._editorCombo.DynamicRepository.FillDbPropertiesForGrid(this, this._inventProduct);
            }
        }


		

        public void UpdateUIAfterDbSave()
        {
            RaisePropertyChanged(() => QuantityDifference);
            RaisePropertyChanged(() => InputType);
            RaisePropertyChanged(() => IsQuantityChanged);
			RaisePropertyChanged(() => IsQuantityCompareChanged);
			
        }       

        #region Implementation of IDataErrorInfo

        public string this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
                    case "QuantityEdit":
                        {
                            //                            string validate = InventProductValidate.QuantityEditValidate(this._quantityEdit.ToString());
                            //                            if (!String.IsNullOrWhiteSpace(validate))
                            //                                return validate;
                            //
                            //                            if (_quantityEdit <= 0)
                            //                                return Localization.Resources.ViewModel_InventProductItem_QuantityEdit;

                            return String.Empty;
                        }
						// case "QuantityEditCompare":
						//{
						//	return String.Empty;
						//}
						
                }
                return String.Empty;
            }
        }

        public string Error
        {
            get { return String.Empty; }
        }

        public ObservableCollection<DynamicProperty> DynamicList
        {
            get { return _dynamicList; }
        }

        #endregion


    }
}