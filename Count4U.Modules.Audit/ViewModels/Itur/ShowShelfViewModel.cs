using System;
using System.Collections.Generic;
using Count4U.Common;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.Audit.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Count4U.Model.Interface.Count4U;
using System.Linq;
using Count4U.Model.Count4U;
using Count4U.Model.Count4U.Validate;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;
//using Count4U.Common.Extensions;

namespace Count4U.Modules.Audit.ViewModels
{
    public class ShowShelfViewModel : CBIContextBaseViewModel
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IIturRepository _iturRepository;
		private readonly IShelfRepository _shelfRepository;
        private readonly IEventAggregator _eventAggregator;

        private readonly DelegateCommand _cancelCommand;
        private readonly DelegateCommand _okCommand;
		private readonly ObservableCollection<ShelfItemViewModel> _shelves;
		private ShelfItemViewModel _shelfSelectedItem;
		//private List<Itur> _iturs;

		//private string _prefixNew;
		private string _iturCode;
		private string _width;
		private string _height;
		private string _area;
		private string _shelvesInItur;
		private bool _includeInFacing;
		private string _sumArea;
		private string _maxNum;
		private string _sumWidth;

	
		
		//private string _itursNumber;
		//private string _textValue;
		//private bool _includeAllIturs;        

		private readonly DelegateCommand<ShelfItemViewModel> _editShelfCommand;
        private readonly DelegateCommand<ShelfItemViewModel> _cancelShelfCommand;
        private readonly DelegateCommand<ShelfItemViewModel> _commitShelfCommand;

		public ShowShelfViewModel(IContextCBIRepository repository,
            ILocationRepository locationRepository,
            IIturRepository iturRepository,
			IShelfRepository shelfRepository,
            IEventAggregator eventAggregator)
            : base(repository)
        {
            this._eventAggregator = eventAggregator;
            this._iturRepository = iturRepository;
			this._shelfRepository = shelfRepository;
            this._locationRepository = locationRepository;
			//this._includeAllIturs = false;
			this._iturCode = "";
			this._shelves = new ObservableCollection<ShelfItemViewModel>();
			this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecuted);
			this._cancelCommand = new DelegateCommand(CancelCommandExecuted);

			this._editShelfCommand = new DelegateCommand<ShelfItemViewModel>(EditShelfCommandExecuted);
			this._cancelShelfCommand = new DelegateCommand<ShelfItemViewModel>(CancelShelfCommandExecuted);
			this._commitShelfCommand = new DelegateCommand<ShelfItemViewModel>(CommitShelfCommandExecuted);

        }

		private void CancelShelfCommandExecuted(ShelfItemViewModel item)
		{
			this._isEditing = false;
		}

		private void EditShelfCommandExecuted(ShelfItemViewModel item)
		{
			this._isEditing = true;
		}

		private bool _isEditing;
		private void CommitShelfCommandExecuted(ShelfItemViewModel viewModel)
		{
			if (this._isEditing)
			{
				this.SaveShelfViewModel(viewModel);

				viewModel.UpdateUIAfterDbSave();  //RaisePropertyChanged()
				ReBuildIturArea(IturCode);
				this._isEditing = false;
			}
		}


		private void SaveShelfViewModel(ShelfItemViewModel viewModel)
		{
			using (new CursorWait())
			{
				Shelf shelf = viewModel.GetShelf(viewModel); //Area считаем (int)(item.Width * item.Height)
				this._shelfRepository.Update(shelf, base.GetDbPath);

				Utils.RunOnUIAsync(() =>
			{
				//this._isGeneratedByCode = true;
				//this._documentHeaderSelectedItem.DocumentHeaderSet(dbObj, this._itur);
				//this._isGeneratedByCode = false;
			}
				);
			}
		}
	
	

		//public string ItursNumber
		//{
		//	get { return this._itursNumber; }
		//	set
		//	{
		//		this._itursNumber = value;
		//		RaisePropertyChanged(() => ItursNumber);

		//		_okCommand.RaiseCanExecuteChanged();
		//	}
		//}

		public DataGrid ShelfDataGrid { get; set; }

		public ObservableCollection<ShelfItemViewModel> Shelves
		{
			get { return _shelves; }
		}

		public ShelfItemViewModel ShelfSelectedItem
		{
			get { return this._shelfSelectedItem;  }
			set
			{
				this._shelfSelectedItem = value;
				this.RaisePropertyChanged(() => this.ShelfSelectedItem);
				//to do
				//this._editInventProductCommand.RaiseCanExecuteChanged();
				//this._deleteInventProductCommand.RaiseCanExecuteChanged();
				//this._cloneCommand.RaiseCanExecuteChanged();

				//this._resetErrorInventProductSelectedCommand.RaiseCanExecuteChanged();
				//this._resetQuantityInventProductCommand.RaiseCanExecuteChanged();
			}
		}

		private bool EditShelfCommandCanExecute()
		{
			return this._shelfSelectedItem != null;
		}

		private void EditShelfCommandExecuted()
		{
			//this.EditInventProduct(this._shelfSelectedItem.InventProduct);
		}

		//public string PrefixNew
		//{
		//	get { return _prefixNew; }
		//	set
		//	{
		//		this._prefixNew = value;
		//		RaisePropertyChanged(() => PrefixNew);

		//		_okCommand.RaiseCanExecuteChanged();
		//	}
		//}

		//public string TextValue
		//{
		//	get { return this._textValue; }
		//	set
		//	{
		//		this._textValue = value;
		//		this.RaisePropertyChanged(() => this.TextValue);

		//		this.RaisePropertyChanged(() => this.TotalIturs);

		//		this._okCommand.RaiseCanExecuteChanged();
		//	}
		//}

		//public string TotalIturs
		//{
		//	get
		//	{
		//		List<int> res = CommaDashStringParser.Parse(this._textValue);
		//		return res == null ? "0" : res.Count.ToString();
		//	}
		//}

		public string IturCode
		{
			get
			{
				return _iturCode;
			}
		}

		public string Width
		{
			get
			{
				return _width;
			}
				set
			{
				this._width  = "0";
				int w = 0;
				this._area = "0.0";
				bool bit = Int32.TryParse(value, out w);
				if (bit == true)
				{
					this._width = value;
					int h = 0;
					bool bit1 = Int32.TryParse(this._height, out h);
					if (bit1 == true)
					{
						this._area =  ((double)(w *h) / 10000.0).ToString();
					}
				}
				
				this.RaisePropertyChanged(() => Height);
				this.RaisePropertyChanged(() => Area);
				_okCommand.RaiseCanExecuteChanged();
			}
		}
			public string Height
		{
			get
			{
				return _height;
			}
			set
			{
				this._height = "0";
				int h = 0;
				this._area = "0.0";
				bool bit = Int32.TryParse(value, out h);
				if (bit == true)
				{
					this._height = value;
					int w = 0;
					bool bit1 = Int32.TryParse(this._width, out w);
					if (bit1 == true)
					{
						this._area =  ((double)(w *h) / 10000.0).ToString();
					}
				}
				
				this.RaisePropertyChanged(() => Height);
				this.RaisePropertyChanged(() => Area);

				_okCommand.RaiseCanExecuteChanged();
			}
		}

		public string ShelvesInItur
		{
			get
			{
				return _shelvesInItur;
			}
			set
			{
				this._shelvesInItur = "1";
				if (this.IsShelvesInIturValid() == true)
				{
					this._shelvesInItur = value;
					RaisePropertyChanged(() => ShelvesInItur);
				}
				_okCommand.RaiseCanExecuteChanged();
			}
		}

		public string Area
		{
			get
			{
				return _area;
			}
			set
			{
				this._area = value;
				RaisePropertyChanged(() => Area);
			}
		}

		public string SumArea
		{
			get
			{
				return _sumArea;
			}
			set
			{
				this._sumArea = value;
				this.RaisePropertyChanged(() => SumArea);
			}
		}

		public string MaxNum
		{
			get { return _maxNum; }
			set { _maxNum = value;
			this.RaisePropertyChanged(() => MaxNum);
			}
		}


		public string SumWidth
		{
			get { return _sumWidth; }
			set { _sumWidth = value;
			this.RaisePropertyChanged(() => SumWidth);
			}
		}

		public bool IncludeInFacing
		{
			get
			{
				return _includeInFacing;
			}
			set
			{
				this._includeInFacing = value;
				this.RaisePropertyChanged(() => this.IncludeInFacing);

				this._okCommand.RaiseCanExecuteChanged();
			}
		}

			
		public string this[string propertyName]
		{
			get
			{
				switch (propertyName)
				{
					case "Width":
						{
							if (String.IsNullOrWhiteSpace(this._width))
								return "0";
							if (this.IsWidthValid() == false)
							{
								return "0";
							}
						}
						break;
					case "Height":
						{
							if (String.IsNullOrWhiteSpace(this._height))
								return "0";
							if (this.IsHeightValid() == false)
							{
								return "0";
							}
						}
						break;
					case "ShelvesInItur":
						{
							if (String.IsNullOrWhiteSpace(this._shelvesInItur))
								return "1";
							if (this.IsShelvesInIturValid() == false)
							{
								return "1";
							}
						}
						break;
				}
				return null;
			}
		}

		//public bool Disable
		//{
		//	get { return !IncludeAllIturs; }
		//}

		//public bool IncludeAllIturs
		//{
		//	get { return this._includeAllIturs; }
		//	set
		//	{
		//		this._includeAllIturs = value;
		//		if (_includeAllIturs == true)
		//		{
		//			this._textValue = "1-9999";
		//		}
		//		else
		//		{
		//			this._textValue = "1";
		//		}
		//		this.RaisePropertyChanged(() => this.TextValue);

		//		this.RaisePropertyChanged(() => this.TotalIturs);

		//		this.RaisePropertyChanged(() => this.IncludeAllIturs);

		//		this._okCommand.RaiseCanExecuteChanged();
				
		//	}
		//}

		public string Error
		{
			get { return string.Empty; }
		}

		//public Locations Locations { get; set; }

		//public Location SelectedLocation
		//{
		//	get { return _selectedLocation; }
		//	set
		//	{
		//		_selectedLocation = value;
		//		RaisePropertyChanged(() => SelectedLocation);

		//		_okCommand.RaiseCanExecuteChanged();
		//	}
		//}

        public DelegateCommand CancelCommand
        {
            get { return this._cancelCommand; }
        }

        public DelegateCommand OkCommand
        {
            get { return this._okCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            string commaSeparatedNumbers = navigationContext.Parameters.First(r => r.Key == NavigationSettings.IturCodes).Value;
            string[] split = commaSeparatedNumbers.Split(new char[] { ',' }).Where(r => !string.IsNullOrEmpty(r)).ToArray();

			
			if (split.Length > 0)
			{
				Itur itur = this._iturRepository.GetIturByCode(split[0], base.GetDbPath);
				if (itur != null)
				{
					this._iturCode = itur.IturCode;
					this._width = itur.Width.ToString();
					this._height = itur.Height.ToString();
					this._area = itur.Area.ToString();
					this._shelvesInItur = itur.ShelfInItur.ToString();
					this._includeInFacing = itur.IncludeInFacing;
					double sumArea = this.BuildShelves(itur.IturCode);
					//this._sumArea = sumArea.ToString();
				}
				this.ShelfSelectedItem = this._shelves.FirstOrDefault();
			}



			//List<int> itursNumbers = new List<int>();

			//this._iturs = new List<Itur>();
			//foreach (string iturCode in split)
			//{
			//	Itur itur = this._iturRepository.GetIturByCode(iturCode, base.GetDbPath);
			//	if (itur != null)
			//	{
			//		itursNumbers.Add(itur.Number);
			//		this._iturs.Add(itur);
			//	}
			//}

			//this.TextValue = CommaDashStringParser.Reverse(itursNumbers);
			//----------------
			//this.Locations = this._locationRepository.GetLocations(base.GetDbPath);

			//string LocationCode = this._locationRepository.GetFistLocationCodeWithoutIturs(base.GetDbPath);
			//if (string.IsNullOrWhiteSpace(LocationCode) == false)
			//{
			//	this.SelectedLocation = this._locationRepository.GetLocationByCode(LocationCode, base.GetDbPath);
			//}
			//else
			//{
			//	this.SelectedLocation = this.Locations.FirstOrDefault();
			//}
        }

		public override void OnNavigatedFrom(NavigationContext navigationContext)
		{
			base.OnNavigatedFrom(navigationContext);
			this.ShelfSelectedItem = null;
			this.ShelfDataGrid = null;
		}


		private double BuildShelves(string iturCode)
		{
			double sumArea = 0.0;
			int maxNum = 0;
			double sumWidth = 0.0;
			this._shelves.Clear();
			Shelfs shelves = this._shelfRepository.GetShelvesByIturCode(iturCode, this.GetDbPath);
			if (shelves == null)
			{
				this._sumArea = "0";
				this._maxNum = "0";
				this._sumWidth = "0";
				return sumArea;
			}
			foreach (var shelf in shelves)
			{
				ShelfItemViewModel viewModel = new ShelfItemViewModel(shelf);
				double ariam2 = shelf.Area / 10000.0;
				viewModel.Area = ariam2;
				viewModel.PropertyChanged += this.DocumentHeaderViewModel_PropertyChanged;
				sumArea = sumArea + ariam2;
				sumWidth = sumWidth + shelf.Width;
				maxNum = maxNum >= shelf.ShelfNum ? maxNum : shelf.ShelfNum;
				this._shelves.Add(viewModel);
			}
			this._sumArea = sumArea.ToString();
			this._sumWidth = sumWidth.ToString();
			this._maxNum = maxNum.ToString();
			return sumArea;
		}

		private void ReBuildIturArea(string iturCode)
		{
			double sumArea = 0.0;
			int maxNum = 0;
			double sumWidth= 0.0;
			Shelfs shelves = this._shelfRepository.GetShelvesByIturCode(iturCode, this.GetDbPath);
			if (shelves == null)
			{
				this._sumArea = "0";
				this._maxNum = "0";
				this._sumWidth = "0";
				this.RaisePropertyChanged(() => SumArea);
				this.RaisePropertyChanged(() => MaxNum);
				this.RaisePropertyChanged(() => SumWidth);
				return;
			}
			foreach (var shelf in shelves)
			{
				double ariam2 = shelf.Area / 10000.0;
				sumArea = sumArea + ariam2;
				sumWidth = sumWidth + shelf.Width;
				maxNum = maxNum >= shelf.ShelfNum ? maxNum : shelf.ShelfNum;
			}
			this._sumArea = sumArea.ToString();
			this._sumWidth = sumWidth.ToString();
			this._maxNum = maxNum.ToString();
			this.RaisePropertyChanged(() => SumArea);
			this.RaisePropertyChanged(() => MaxNum);
			this.RaisePropertyChanged(() => SumWidth);
		}


		void DocumentHeaderViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
		//		ShelfItemViewModel viewModel = sender as ShelfItemViewModel;
		//		if (viewModel == null) return;
		//		Utils.RunOnUI(() => Mouse.OverrideCursor = Cursors.Wait);
		//		Task.Factory.StartNew(() =>
		//		{

		//			//this._documentHeaderRepository.Update(viewModel.DocumentHeader, base.GetDbPath);
		//			//try
		//			//{
		//			//	this._iturRepository.RefillApproveStatusBitByIturCode(viewModel.DocumentHeader.IturCode, base.GetDbPath);
		//			//}
		//			//catch (Exception exp)
		//			//{
		//			//	_logger.ErrorException("DocumentHeaderViewModel_PropertyChanged: RefillApproveStatusBit :  ", exp);
		//			//}

		//			//this._itur = this._iturRepository.GetIturByCode(this._itur.IturCode, GetDbPath);
		//			//this.DocumentHeadersRefresh();
		//			Utils.RunOnUI(() =>
		//			{
		//				//this.BuildIturInfo();
		//				Mouse.OverrideCursor = null;
		//			});
		//		});//.LogTaskFactoryExceptions("DocumentHeaderViewModel_PropertyChanged");
		//	}
		}

		public DelegateCommand<ShelfItemViewModel> EditShelfCommand
		{
			get { return _editShelfCommand; }
		}

		public DelegateCommand<ShelfItemViewModel> CancelShelfCommand
		{
			get { return _cancelShelfCommand; }
		}

		public DelegateCommand<ShelfItemViewModel> CommitShelfCommand
		{
			get { return _commitShelfCommand; }
		}

	
		private bool IsWidthValid()
		{
			if (string.IsNullOrWhiteSpace(this._width) == true) return true;
			int dummy;
			return Int32.TryParse(this._width, out dummy);
		}

		private bool IsHeightValid()
		{
			if (string.IsNullOrWhiteSpace(this._height) == true) return true;
			int dummy;
			return Int32.TryParse(this._height, out dummy);
		}

		private bool IsShelvesInIturValid()
		{
			if (string.IsNullOrWhiteSpace(this._width) == true) return true;
			int dummy;
			return Int32.TryParse(this._shelvesInItur, out dummy);
		}

        private void CancelCommandExecuted()
        {
            this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private void OkCommandExecuted()
        {
			this._eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
			using (new CursorWait())
			{
				var itur = this._iturRepository.GetIturByCode(IturCode, base.GetDbPath);
				if (itur != null)
				{
					int h = 0;
					Int32.TryParse(this._height, out h);
					itur.Height = h;
					int w = 0;
					Int32.TryParse(this._width, out w);
					itur.Width = w;
					int s = 1;
					Int32.TryParse(this._shelvesInItur, out s);
					itur.ShelfInItur = s;
					if (itur.ShelfInItur == 0) itur.ShelfInItur = 1;
					itur.Area = (double)(itur.Width * itur.Height) / 10000.0;
					itur.UnitPlaceWidth = (int)((double)(itur.Width * itur.Height) / (double)itur.ShelfInItur);
					itur.AreaCount = itur.Area / (double)itur.ShelfInItur;
					this._iturRepository.Update(itur, base.GetDbPath);
				}
			}

			//List<int> itursNumberList = CommaDashStringParser.Parse(this._textValue);
			//List<int> itursNumberListDistinct = itursNumberList.Distinct().ToList();
			//Iturs itursAll = new Iturs();
			
			//using (new CursorWait())
			//{
			//	if (this.IncludeAllIturs == false)
			//	{
			//		foreach (int iturNumber in itursNumberListDistinct)
			//		{
			//			var iturs = this._iturRepository.GetItursByNumber(iturNumber, base.GetDbPath);
			//			itursAll.AddEnumerableIturs(iturs);
			//		}
			//	}

			//  this._eventAggregator.GetEvent<IturPrefixChangedEvent>().Publish(
			//	new IturPrefixChangedEventPayload { Iturs = itursAll, PrefixNew = this.PrefixNew, AllChange = this.IncludeAllIturs}
			//  );
			//}
        }

		private bool OkCommandCanExecuted()
		{
			bool can = (this.IsShelvesInIturValid() == true
				&& (this.IsWidthValid() == true)
				&& (this.IsHeightValid() == true));
			return can;

		}
    }
}