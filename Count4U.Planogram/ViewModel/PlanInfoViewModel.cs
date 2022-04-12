using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Common.ViewModel;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Count4U.Planogram.Lib;
using System;

namespace Count4U.Planogram.ViewModel
{
    public class PlanInfoViewModel : CBIContextBaseViewModel
    {
        private readonly INavigationRepository _navigationRepository;
        private readonly IStatusIturGroupRepository _statusIturGroupRepository;
        private readonly IUserSettingsManager _userSettingsManager;

        private Dictionary<string, StatusIturGroup> _allStatusGroups;

        private readonly ObservableCollection<PlanInfoItemViewModel> _items;

        private string _totalIturs;
        private string _totalItems;
        private string _process;
        private SolidColorBrush _processColor;

        public PlanInfoViewModel(
            IContextCBIRepository contextCBIRepository,
            INavigationRepository navigationRepository,
            IStatusIturGroupRepository statusIturGroupRepository,
            IUserSettingsManager userSettingsManager
            )
            : base(contextCBIRepository)
        {
            _userSettingsManager = userSettingsManager;
            _statusIturGroupRepository = statusIturGroupRepository;
            _navigationRepository = navigationRepository;
            _items = new ObservableCollection<PlanInfoItemViewModel>();
        }

        public ObservableCollection<PlanInfoItemViewModel> Items
        {
            get { return _items; }
        }

        public string TotalIturs
        {
            get { return _totalIturs; }
            set
            {
                _totalIturs = value;
                RaisePropertyChanged(() => TotalIturs);
            }
        }

        public string TotalItems
        {
            get { return _totalItems; }
            set
            {
                _totalItems = value;
                RaisePropertyChanged(() => TotalItems);
            }
        }

        public string Process
        {
            get { return _process; }
            set
            {
                _process = value;
                RaisePropertyChanged(() => Process);
            }
        }

        public SolidColorBrush ProcessColor
        {
            get { return _processColor; }
            set
            {
                _processColor = value;
                RaisePropertyChanged(() => ProcessColor);
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            object obj = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, isRemove: true);

            PlanInfo info = obj as PlanInfo;
            Build(info);
        }

        private void Build(PlanInfo info)
        {
            if (info == null) return;

            this._allStatusGroups = this._statusIturGroupRepository.CodeStatusIturGroupDictionary;

            foreach (PlanInfoItem infoItem in info.Info.OrderBy(r => r.Itur.Number))
            {
                StatusIturGroup statusGroup = GetStatusIturGroupByItur(infoItem.Itur);
                string color = UtilsStatus.FromStatusGroupBitToColor(this._statusIturGroupRepository.BitStatusIturGroupEnumDictionary,
                                                                     statusGroup.Bit, this._userSettingsManager);

                string numberOfProducts =
                    infoItem.IturAnalyze == null
                    ?
                    Localization.Resources.ViewModel_PlanInfo_msgCalculating
                    :
                    infoItem.IturAnalyze.QuantityEdit.HasValue
                        ?
                        String.Format("{0:0.##}", infoItem.IturAnalyze.QuantityEdit.Value)
                        :
                        0.ToString();

                PlanInfoItemViewModel item = new PlanInfoItemViewModel() { Color = color, Number = infoItem.Itur.Number, NumberOfProducts = numberOfProducts };
                _items.Add(item);
            }

            _totalIturs = String.Format("{0}: {1}", Localization.Resources.View_PlanInfo_tbTotalIturs, info.TotalIturs);
            _totalItems =
                info.TotalItems == -1
                ?
                String.Format("{0}: {1}", Localization.Resources.View_PlanInfo_tbTotalItems, Localization.Resources.ViewModel_PlanInfo_msgCalculating)
                 :
                String.Format("{0}: {1:0.##}", Localization.Resources.View_PlanInfo_tbTotalItems, info.TotalItems);

            _process = String.Format("{0} {1:0.##}%", Localization.Resources.View_PlanInfo_tbProcess, info.Process);
            _processColor = new SolidColorBrush(Helpers.FromPercentageToColor(info.Process, _userSettingsManager, info.Info.Any() == false));
        }

        StatusIturGroup GetStatusIturGroupByItur(Itur itur)
        {
            return this._allStatusGroups.Values.FirstOrDefault(r => r.Bit == itur.StatusIturGroupBit);
        }
    }
}