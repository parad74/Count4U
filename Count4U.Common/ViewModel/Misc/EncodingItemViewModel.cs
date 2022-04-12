using System;
using System.Text;
using Count4U.Common.Helpers;

namespace Count4U.Common.ViewModel.Misc
{
    public class EncodingItemViewModel
    {
        private readonly Encoding _encoding;

        public EncodingItemViewModel(Encoding encoding)
        {
            _encoding = encoding;

            Text =  UtilsConvert.HebrewText(String.Format("{0} ({1})", _encoding.EncodingName, _encoding.CodePage));
        }

        public string Text { get; set; }

        public Encoding Encoding
        {
			get
			{
				if (_encoding == null) return Encoding.GetEncoding(1255);
				return _encoding; 
			}
        }
    }
}