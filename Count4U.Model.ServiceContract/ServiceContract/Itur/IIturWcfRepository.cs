using Count4U.Model.Main;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Collections.Generic;
using Count4U.Model.Count4U;

namespace Count4U.Model.ServiceContract
{
	[ServiceContract]
	public interface IIturWcfRepository
	{
		[OperationContract]
		List<Itur> GetIturList();

		[OperationContract]
		Iturs GetIturs();

		//[OperationContract]
		//Iturs GetIturs(SelectParams selectParams);

		[OperationContract]
		Itur GetItur(string iturCode);

		[OperationContract]
		Itur GetOrCreateIturByERPIturCode(string erpIturCode);

		[OperationContract]
		bool Insert(Itur itur);

		[OperationContract]
		Itur InsertAndReturn(Itur itur);

		[OperationContract]
		bool Update(Itur itur);

		[OperationContract]
		Itur UpdateAndReturn(Itur itur);

		[OperationContract]
		bool Delete(string iturCode);
	}
}

