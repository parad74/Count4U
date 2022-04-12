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

namespace Count4U.Modules.Audit.ViewModels
{
	public class SectionTagChangeViewModel : CBIContextBaseViewModel
    {
		private readonly ISectionRepository _sectionRepository;
        private readonly IIturRepository _iturRepository;
        private readonly IEventAggregator _eventAggregator;

        private readonly DelegateCommand _cancelCommand;
        private readonly DelegateCommand _okCommand;
		private Sections _sections;


		private string _textValue;
		//private bool _includeAllIturs;        

		public SectionTagChangeViewModel(IContextCBIRepository repository,
			ISectionRepository sectionRepository,
            IIturRepository iturRepository,
            IEventAggregator eventAggregator)
            : base(repository)
        {
            this._eventAggregator = eventAggregator;
            this._iturRepository = iturRepository;
            this._sectionRepository = sectionRepository;
			//this._includeAllIturs = false;

            this._okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecuted);
            this._cancelCommand = new DelegateCommand(CancelCommandExecuted);
        }

       
		public string TextValue
		{
			get { return this._textValue; }
			set
			{
				this._textValue = value;
				this.RaisePropertyChanged(() => this.TextValue);

				this.RaisePropertyChanged(() => this.TotalSections);
				this.RaisePropertyChanged(() => this.SectionCodes);

				this._okCommand.RaiseCanExecuteChanged();
			}
		}

	
		public string TotalSections
		{
			get
			{
				if (_sections == null) return "0";
				return _sections.Count.ToString();
			}
		}

		public string SectionCodes
		{
			get
			{
				if (_sections == null) return "";

				string[] sectionCodes = _sections.Select(x => x.SectionCode).ToArray();
				return sectionCodes.JoinRecord(",");
			}
		}

		private bool IsTextValid() 
		{
			   string validation = CustomerValidate.CodeValidate(this._textValue);
			   if (String.IsNullOrEmpty(validation) == true) return true;
			   else return false;
		}

		public string this[string propertyName] 
		{
			get
			{
				switch (propertyName)
				{
					case "TextValue":
						{
							if (this.IsTextValid() == false)
							{
								return String.Format(Localization.Resources.ViewModel_SectionTag_Expression, Environment.NewLine, Environment.NewLine);
							}
						}
			
						break;
				}
				return null;
			}
		}

	
		public string Error
		{
			get { return string.Empty; }
		}

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

			string commaSeparatedNumbers = navigationContext.Parameters.First(r => r.Key == NavigationSettings.SectionCodes).Value;
			string[] split = commaSeparatedNumbers.Split(new char[] { ',' }).Where(r => !string.IsNullOrEmpty(r)).ToArray();

		   // string commaSeparatedNumbers = navigationContext.Parameters.First(r => r.Key == NavigationSettings.IturCodes).Value;
		   // string[] split = commaSeparatedNumbers.Split(new char[] { ',' }).Where(r => !string.IsNullOrEmpty(r)).ToArray();

			//List<string> locationcodes = new List<string>();

			this._sections = new Sections();
			foreach (string sectionCode in split)
			{
				Count4U.Model.Count4U.Section section = this._sectionRepository.GetSectionByCode(sectionCode, base.GetDbPath);
				if (section != null)
				{
				//	itursNumbers.Add(location.Number);
					this._sections.Add(section);
				}
			}

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

				this._eventAggregator.GetEvent<SectionTagChangedEvent>().Publish(
					//new IturLocationChangedEventPayload { Iturs = Iturs.FromEnumerable(this._iturs), Location = SelectedLocation, AllChange = this.IncludeAllIturs }
					new SectionTagChangedEventPayload { Sections = this._sections, Tag = this.TextValue }
                    );
            }
        }

        private bool OkCommandCanExecuted()
        {
			bool can = ((string.IsNullOrWhiteSpace(this._textValue) == false)//*
				&& (this.IsTextValid() == true)
				&&  (this._sections != null)
				&& (this._sections.Count > 0)); 
            return can; 
        }
    }
}