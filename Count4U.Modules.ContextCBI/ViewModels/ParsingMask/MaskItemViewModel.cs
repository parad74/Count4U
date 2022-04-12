using Count4U.Model;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.ContextCBI.ViewModels.ParsingMask
{
    public class MaskItemViewModel : NotificationObject
    {
        private readonly Mask _mask;

        public MaskItemViewModel(Mask mask)
        {
            this._mask = mask;

            AdapterCode = Mask.AdapterCode;
            FileCode = Mask.FileCode;
            BarcodeMask = Mask.BarcodeMask;
            MakatMask = Mask.MakatMask;
        }

        public string AdapterCode { get; set; }
        public string FileCode { get; set; }
        public string BarcodeMask { get; set; }
        public string MakatMask { get; set; }

        public Mask Mask
        {
            get { return this._mask; }
        }
    }
}