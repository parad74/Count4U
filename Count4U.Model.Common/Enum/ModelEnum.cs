using System;
using System.Collections.Generic;
using System.Linq;
namespace Count4U.Model.
{
	public enum CBIContext
	{
		CreateInventor = 0,
		History = 1, 
		Main = 2
	}

	public enum FromContext
	{
		StartApplicationWithoutAction = 0,
		MdiInventorLinkWithoutAction = 1, 
		FromBranchWithoutAction = 2,
		FromBranchWithAction = 3 ,
		SelectAdapterWithoutAction = 4,
		FromInventorWithoutAction = 5
	}

	public enum ToContext
	{
		ToBranchWithoutAction = 0
	}
	

	public enum DomainTypeEnum
	{
		Customer = 0,
		Branch = 1,
		Inventor = 2,
		InventorConfig = 3
	}
}
