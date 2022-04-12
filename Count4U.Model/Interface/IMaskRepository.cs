using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model;
using Count4U.Model.SelectionParams;

namespace Count4U.Model.Interface
{
    
    public interface IMaskRepository
    {

		IConnectionDB Connection { get; set; }

		Masks GetMasks();
		
		Masks GetMasks(SelectParams selectParams);

		void Delete(string code, string adapterCode, string fileCode, string pathDB);

		void Delete(string code, string adapterCode, string pathDB);

		void Delete(string code, string pathDB);

		void Insert(Mask mask, string pathDB);
	
		void Update(Mask mask, string pathDB);
	
	}
}
