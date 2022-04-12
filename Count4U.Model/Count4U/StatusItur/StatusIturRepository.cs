using System;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;
using Count4U.Model;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model.Count4U
{
	public class StatusIturRepository : IStatusIturRepository
	{
		//private StatusIturs this._list;
		private Dictionary<string, StatusItur> _codeStatusIturDictionary;

		private Dictionary<int, IturStatusEnum> _bitStatusIturEnumDictionary;

		//private readonly IServiceLocator _serviceLocator;

		public StatusIturRepository()
        {
			//this._serviceLocator = serviceLocator;
			this._bitStatusIturEnumDictionary = new Dictionary<int, IturStatusEnum>();
			this._codeStatusIturDictionary = new Dictionary<string, StatusItur>();
			this.FillCodeStatusIturDictionary();
			this.FillBitStatusIturEnumDictionary();
        }

		#region IStatusIturRepository Members
		//"No One Document"	 "NoOneDoc"	"NoOneDoc"	0
		//"None"	"None"	"None"	1
		//"One Document Is Not Approve"	"OneDocIsNotApprove"	"OneDocIsNotApprove"	2
		//"One Document Is Approve"	"OneDocIsApprove"	"OneDocIsApprove" 	3
		//"Any of Documents Is Not Approve"	"AnyDocIsNotApprove"	"AnyDocIsNotApprove"	4
		//"All Documents Is Approve"	"AllDocIsApprove"	"AllDocIsApprove"	5

		public Dictionary<string, StatusItur> CodeStatusIturDictionary
		{
			get
			{
				return this._codeStatusIturDictionary;
			}
		}

		private void FillCodeStatusIturDictionary()
		{
			if (this._codeStatusIturDictionary == null)
			{
				this._codeStatusIturDictionary = new Dictionary<string, StatusItur>();
			}
			this._codeStatusIturDictionary.Add(IturStatusEnum.NoOneDoc.ToString(),
						new StatusItur()
						{
							Name = IturStatusTitle.NoOneDoc,
							Description = IturStatusTitle.NoOneDoc,
							Code = IturStatusEnum.NoOneDoc.ToString(),
							BackgroundColor = "255, 255, 224",
							Bit = (int)IturStatusEnum.NoOneDoc
						});

			//this._codeStatusIturDictionary.Add(IturStatusEnum.None.ToString(),
			//    new StatusItur()
			//    {
			//        Name = IturStatusTitle.None,
			//        Description = IturStatusTitle.None,
			//        Code = IturStatusEnum.None.ToString(),
			//        BackgroundColor = "144, 238, 204",
			//        Bit = (int)IturStatusEnum.None
			//    });

			this._codeStatusIturDictionary.Add(IturStatusEnum.OneDocIsNotApprove.ToString(),
				new StatusItur()
				{
					Name = IturStatusTitle.OneDocIsNotApprove,
					Description = IturStatusTitle.OneDocIsNotApprove,
					Code = IturStatusEnum.OneDocIsNotApprove.ToString(),
					BackgroundColor = "173, 216, 230",
					Bit = (int)IturStatusEnum.OneDocIsNotApprove

				});

			this._codeStatusIturDictionary.Add(IturStatusEnum.OneDocIsApprove.ToString(),
				new StatusItur()
				{
					Name = IturStatusTitle.OneDocIsApprove,
					Description = IturStatusTitle.OneDocIsApprove,
					Code = IturStatusEnum.OneDocIsApprove.ToString(),
					BackgroundColor = "103, 116, 230",
					Bit = (int)IturStatusEnum.OneDocIsApprove

				});

			this._codeStatusIturDictionary.Add(IturStatusEnum.SomeDocIsNotApprove.ToString(),
				new StatusItur()
				{
					Name = IturStatusTitle.SomeDocIsNotApprove,
					Description = IturStatusTitle.SomeDocIsNotApprove,
					Code = IturStatusEnum.SomeDocIsNotApprove.ToString(),
					BackgroundColor = "125, 205, 124",
					Bit = (int)IturStatusEnum.SomeDocIsNotApprove

				});

			this._codeStatusIturDictionary.Add(IturStatusEnum.SomeDocIsApprove.ToString(),
			new StatusItur()
			{
				Name = IturStatusTitle.SomeDocIsApprove,
				Description = IturStatusTitle.SomeDocIsApprove,
				Code = IturStatusEnum.SomeDocIsApprove.ToString(),
				BackgroundColor = "155, 250, 122",
				Bit = (int)IturStatusEnum.SomeDocIsApprove

			});

			//
			this._codeStatusIturDictionary.Add(IturStatusEnum.DisableAndNoOneDoc.ToString(),
			new StatusItur()
			{
				Name = IturStatusTitle.DisableAndNoOneDoc,
				Description = IturStatusTitle.DisableAndNoOneDoc,
				Code = IturStatusEnum.DisableAndNoOneDoc.ToString(),
				BackgroundColor = "155, 250, 122",
				Bit = (int)IturStatusEnum.DisableAndNoOneDoc

			});

			this._codeStatusIturDictionary.Add(IturStatusEnum.DisableAndOneDocIsApprove.ToString(),
			new StatusItur()
			{
				Name = IturStatusTitle.DisableAndOneDocIsApprove,
				Description = IturStatusTitle.DisableAndOneDocIsApprove,
				Code = IturStatusEnum.DisableAndOneDocIsApprove.ToString(),
				BackgroundColor = "155, 250, 122",
				Bit = (int)IturStatusEnum.DisableAndOneDocIsApprove

			});

			this._codeStatusIturDictionary.Add(IturStatusEnum.DisableAndOneDocIsNotApprove.ToString(),
			new StatusItur()
			{
				Name = IturStatusTitle.DisableAndOneDocIsNotApprove,
				Description = IturStatusTitle.DisableAndOneDocIsNotApprove,
				Code = IturStatusEnum.DisableAndOneDocIsNotApprove.ToString(),
				BackgroundColor = "155, 250, 122",
				Bit = (int)IturStatusEnum.DisableAndOneDocIsNotApprove

			});

			this._codeStatusIturDictionary.Add(IturStatusEnum.DisableAndSomeDocIsNotApprove.ToString(),
			new StatusItur()
			{
				Name = IturStatusTitle.DisableAndSomeDocIsNotApprove,
				Description = IturStatusTitle.DisableAndSomeDocIsNotApprove,
				Code = IturStatusEnum.DisableAndSomeDocIsNotApprove.ToString(),
				BackgroundColor = "155, 250, 122",
				Bit = (int)IturStatusEnum.DisableAndSomeDocIsNotApprove

			});

			this._codeStatusIturDictionary.Add(IturStatusEnum.DisableAndSomeDocIsApprove.ToString(),
			new StatusItur()
			{
				Name = IturStatusTitle.DisableAndSomeDocIsApprove,
				Description = IturStatusTitle.DisableAndSomeDocIsApprove,
				Code = IturStatusEnum.DisableAndSomeDocIsApprove.ToString(),
				BackgroundColor = "155, 250, 122",
				Bit = (int)IturStatusEnum.DisableAndSomeDocIsApprove

			});

		}


		public Dictionary<int, IturStatusEnum> BitStatusIturEnumDictionary
		{
			get
			{
				return this._bitStatusIturEnumDictionary;
			}
		}

		private void FillBitStatusIturEnumDictionary()
		{
			if (this._bitStatusIturEnumDictionary == null)
			{
				this._bitStatusIturEnumDictionary = new Dictionary<int, IturStatusEnum>();
			}

			this._bitStatusIturEnumDictionary.Add((int)IturStatusEnum.NoOneDoc,
				IturStatusEnum.NoOneDoc);

			//this._bitStatusIturEnumDictionary.Add((int)IturStatusEnum.None,
			//    IturStatusEnum.None);

			this._bitStatusIturEnumDictionary.Add((int)IturStatusEnum.OneDocIsNotApprove,
				IturStatusEnum.OneDocIsNotApprove);

			this._bitStatusIturEnumDictionary.Add((int)IturStatusEnum.OneDocIsApprove,
				IturStatusEnum.OneDocIsApprove);

			this._bitStatusIturEnumDictionary.Add((int)IturStatusEnum.SomeDocIsNotApprove,
				IturStatusEnum.SomeDocIsNotApprove);

			this._bitStatusIturEnumDictionary.Add((int)IturStatusEnum.SomeDocIsApprove,
				IturStatusEnum.SomeDocIsApprove);

			//
			this._bitStatusIturEnumDictionary.Add((int)IturStatusEnum.DisableAndSomeDocIsApprove,
				IturStatusEnum.DisableAndSomeDocIsApprove);

			this._bitStatusIturEnumDictionary.Add((int)IturStatusEnum.DisableAndSomeDocIsNotApprove,
				IturStatusEnum.DisableAndSomeDocIsNotApprove);

			this._bitStatusIturEnumDictionary.Add((int)IturStatusEnum.DisableAndNoOneDoc,
				IturStatusEnum.DisableAndNoOneDoc);

			this._bitStatusIturEnumDictionary.Add((int)IturStatusEnum.DisableAndOneDocIsApprove,
				IturStatusEnum.DisableAndOneDocIsApprove);

			this._bitStatusIturEnumDictionary.Add((int)IturStatusEnum.DisableAndOneDocIsNotApprove,
				IturStatusEnum.DisableAndOneDocIsNotApprove);

			this._bitStatusIturEnumDictionary.Add((int)IturStatusEnum.WarningConvert,
				IturStatusEnum.WarningConvert);


		}


		public void SetBackgroundColor(string pathDB, IturStatusEnum iturStatus, string backgroundColor)
		{
			this.CodeStatusIturDictionary[iturStatus.ToString()].BackgroundColor = backgroundColor; 
		}
		#endregion
	}
}
