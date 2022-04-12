using Count4U.Model;
using Microsoft.Practices.Prism.ViewModel;
using System.Windows.Media;
using Microsoft.Practices.Prism.Commands;

namespace Count4U.Modules.Audit.ViewModels.ProgressItem
{
    public class ProgressItemViewModel : NotificationObject
    {
        private string _text;
        private Brush _color;
		private DelegateCommand<ProgressItemViewModel> _selectCommand;
		public int Count { get; set; }
		public IturStatusGroupEnum En { get; set; }
		public int StatusBit { get; set; }

		public DelegateCommand<ProgressItemViewModel> SelectCommand
		{
			get { return _selectCommand; }
			set { _selectCommand = value; }
		}
		

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                RaisePropertyChanged(() => Text);
            }
        }

        public Brush Color
        {
            get { return _color; }
            set
            {
                _color = value;
                RaisePropertyChanged(() => Color);
            }
        }

    
    }
}