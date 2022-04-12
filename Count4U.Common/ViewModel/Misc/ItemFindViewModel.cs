using Count4U.Model;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Common.ViewModel.Misc
{
    public class ItemFindViewModel : NotificationObject
    {
        public string Value { get; set; }
        public string Text { get; set; }
		public string FillColor { get; set; }
		
        public string GetTemolateByText(string text)
		{
			switch (text)
			{
				case "Empty":

					return "";

				case "One Doc Is Approve":

					return "";

				case "All Docs Are Approve":

					return "";

				case "Not Approve":

					return "";

				case "Disable And No One Doc":

					return "";
				case "Disable With Docs":

					return "";
				case "Error":

					return "";
				case "None":

					return "";

				default:
					return "";

			}
		}
	}
}