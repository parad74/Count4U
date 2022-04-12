using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common.WCF
{
	public class ContextExtention : IExtension<OperationContext>
	{
		// custom data...
		private string _dateTime;

		public ContextExtention(string dateTime) 
		{
			this._dateTime = dateTime;
		}


		#region IExtension<OperationContext> Members
		public void Attach(OperationContext owner)
		{
			// attach to OperationCompleted to destroy context 
			owner.OperationCompleted += new EventHandler(delegate(object sender, EventArgs args)
			 {
				 this.Detach((OperationContext)sender);
			 });
		}


		public void Detach(OperationContext owner)
		{
			// free context data 
		}
		#endregion
	}
}
