using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Interface.Count4U;
using Count4U.Planogram.Lib;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System.Windows.Media;

namespace Count4U.Planogram.ViewModel
{
    public class PlanTextAssignViewModel : CBIContextBaseViewModel, IChildWindowViewModel
    {
        private readonly INavigationRepository _navigationRepository;
        private readonly IEventAggregator _eventAggregator;

        private readonly DelegateCommand _okCommand;
        private readonly DelegateCommand _cancelCommand;

        private string _text;
        private int _fontSize;
        private Color _fontColor;

        public PlanTextAssignViewModel(
            IContextCBIRepository contextCbiRepository,
            INavigationRepository navigationRepository,
            IEventAggregator eventAggregator)
            : base(contextCbiRepository)
        {
            _eventAggregator = eventAggregator;
            _navigationRepository = navigationRepository;

            _okCommand = new DelegateCommand(OkCommandExecuted, OkCommandCanExecute);
            _cancelCommand = new DelegateCommand(CancelCommandExecuted);
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                RaisePropertyChanged(() => Text);

                _okCommand.RaiseCanExecuteChanged();
            }
        }

        public int FontSize
        {
            get { return _fontSize; }
            set { _fontSize = value; }
        }

        public Color FontColor
        {
            get { return _fontColor; }
            set { _fontColor = value; }
        }

        public DelegateCommand OkCommand
        {
            get { return _okCommand; }
        }

        public DelegateCommand CancelCommand
        {
            get { return _cancelCommand; }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            PlanTextInfo textInfo = UtilsConvert.GetObjectFromNavigation(navigationContext, _navigationRepository, null, true) as PlanTextInfo;
            if (textInfo != null)
            {
                Build(textInfo);
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
        }

        private void Build(PlanTextInfo textInfo)
        {
            _text = textInfo.Text;
            _fontSize = textInfo.FontSize == 0 ? 10 : textInfo.FontSize;
            _fontColor = ColorParser.StringToColor(textInfo.FontColor);
        }

        private void CancelCommandExecuted()
        {
            _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        private bool OkCommandCanExecute()
        {
            return String.IsNullOrWhiteSpace(_text) == false;
        }

        private void OkCommandExecuted()
        {
            PlanTextInfo textInfo  = new PlanTextInfo();
            textInfo.Text = this._text;
            textInfo.FontSize = this._fontSize;
            textInfo.FontColor = ColorParser.ColorToString(this._fontColor);

            this.ResultData = textInfo;
            _eventAggregator.GetEvent<ModalWindowCloseEvent>().Publish(this);
        }

        public object ResultData { get; set; }       

        private void ChooseFontCommandExecuted()
        {
            FontDialog fontDialog1 = new FontDialog();
            fontDialog1.AllowScriptChange = false;
            fontDialog1.AllowSimulations = false;
            fontDialog1.AllowVectorFonts = false;
            fontDialog1.AllowVerticalFonts = false;
            fontDialog1.ShowColor = true;            
//            fontDialog1.Font = textBox1.Font;
//            fontDialog1.Color = textBox1.ForeColor;

            if (fontDialog1.ShowDialog() != DialogResult.Cancel)
            {
//                textBox1.Font = fontDialog1.Font;
//                textBox1.ForeColor = fontDialog1.Color;
            }
        }

    }
}