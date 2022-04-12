using System;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;
using Count4U.Model;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model.Count4U
{
	public class StatusIturGroupRepository : IStatusIturGroupRepository
	{
		//private StatusIturs this._list;
		private Dictionary<string, StatusIturGroup> _codeStatusIturGroupDictionary;

		private Dictionary<int, IturStatusGroupEnum> _bitStatusIturGroupEnumDictionary;

		//private readonly IServiceLocator _serviceLocator;

		public StatusIturGroupRepository()
		{
			//this._serviceLocator = serviceLocator;
			this._codeStatusIturGroupDictionary = new Dictionary<string, StatusIturGroup>();
			this._bitStatusIturGroupEnumDictionary = new Dictionary<int, IturStatusGroupEnum>();

			this.FillCodeStatusIturGroupDictionary();
			this.FillBitStatusIturGroupEnumDictionary();
		}

		#region IStatusIturRepository Members
		public Dictionary<string, StatusIturGroup> CodeStatusIturGroupDictionary
		{
			get
			{
				return this._codeStatusIturGroupDictionary;
			}
		}

		
		public Dictionary<string, StatusIturGroup> CodeStatusIturGroupWithNoneDictionary
		{
			get
			{
				Dictionary<string, StatusIturGroup> codeStatusIturGroupWithNoneDictionary = new Dictionary<string, StatusIturGroup>();
				  foreach (KeyValuePair<string, StatusIturGroup> group in this._codeStatusIturGroupDictionary)
                  {
					  codeStatusIturGroupWithNoneDictionary[group.Key] = group.Value;
				  }
				  codeStatusIturGroupWithNoneDictionary.Add(IturStatusGroupEnum.None.ToString(),
					new StatusIturGroup()
					{
						Name = IturStatusGroupTitle.None,
						NameLocalizationCode = "IturStatusGroup_None",
						Description = IturStatusGroupTitle.None,
						Code = IturStatusGroupEnum.None.ToString(),
						BackgroundColor = "128, 128, 128",
						Bit = (int)IturStatusGroupEnum.None

					});
				return codeStatusIturGroupWithNoneDictionary;
			}
		}

		public Dictionary<int, IturStatusGroupEnum> BitStatusIturGroupEnumWithNoneDictionary
		{
			get
			{
				Dictionary<int, IturStatusGroupEnum> bitStatusIturGroupEnumWithNoneDictionary = new Dictionary<int, IturStatusGroupEnum>();
				foreach (KeyValuePair<int, IturStatusGroupEnum> group in this._bitStatusIturGroupEnumDictionary)
				{
					bitStatusIturGroupEnumWithNoneDictionary[group.Key] = group.Value;
				}
				bitStatusIturGroupEnumWithNoneDictionary[(int)IturStatusGroupEnum.None] = IturStatusGroupEnum.None;
			
				return bitStatusIturGroupEnumWithNoneDictionary;
			}
		}

		private void FillCodeStatusIturGroupDictionary()
		{
			if (this._codeStatusIturGroupDictionary == null)
			{
				this._codeStatusIturGroupDictionary = new Dictionary<string, StatusIturGroup>();
			}
			this._codeStatusIturGroupDictionary.Add(IturStatusGroupEnum.Empty.ToString(),
				new StatusIturGroup()
				{
					Name = IturStatusGroupTitle.Empty,
					NameLocalizationCode = "IturStatusGroup_Empty",
					Description = IturStatusGroupTitle.Empty,
					Code = IturStatusGroupEnum.Empty.ToString(),
					BackgroundColor = "255, 255, 224",
					Bit = (int)IturStatusGroupEnum.Empty
				});

			this._codeStatusIturGroupDictionary.Add(IturStatusGroupEnum.OneDocIsApprove.ToString(),
				new StatusIturGroup()
				{
					Name = IturStatusGroupTitle.OneDocIsApprove,
					NameLocalizationCode = "IturStatusGroup_OneDocIsApprove",
					Description = IturStatusGroupTitle.OneDocIsApprove,
					Code = IturStatusGroupEnum.OneDocIsApprove.ToString(),
					BackgroundColor = "173, 216, 230",
					Bit = (int)IturStatusGroupEnum.OneDocIsApprove

				});

			this._codeStatusIturGroupDictionary.Add(IturStatusGroupEnum.NotApprove.ToString(),
				new StatusIturGroup()
				{
					Name = IturStatusGroupTitle.NotApprove,
					NameLocalizationCode = "IturStatusGroup_NotApprove",
					Description = IturStatusGroupTitle.NotApprove,
					Code = IturStatusGroupEnum.NotApprove.ToString(),
					BackgroundColor = "103, 116, 230",
					Bit = (int)IturStatusGroupEnum.NotApprove

				});

			this._codeStatusIturGroupDictionary.Add(IturStatusGroupEnum.AllDocsIsApprove.ToString(),
				new StatusIturGroup()
				{
					Name = IturStatusGroupTitle.AllDocsIsApprove,
					NameLocalizationCode = "IturStatusGroup_AllDocsIsApprove",
					Description = IturStatusGroupTitle.AllDocsIsApprove,
					Code = IturStatusGroupEnum.AllDocsIsApprove.ToString(),
					BackgroundColor = "125, 205, 124",
					Bit = (int)IturStatusGroupEnum.AllDocsIsApprove

				});

			this._codeStatusIturGroupDictionary.Add(IturStatusGroupEnum.DisableAndNoOneDoc.ToString(),
			new StatusIturGroup()
			{
				Name = IturStatusGroupTitle.DisableAndNoOneDoc,
				NameLocalizationCode = "IturStatusGroup_DisableAndNoOneDoc",
				Description = IturStatusGroupTitle.DisableAndNoOneDoc,
				Code = IturStatusGroupEnum.DisableAndNoOneDoc.ToString(),
				BackgroundColor = "155, 250, 122",
				Bit = (int)IturStatusGroupEnum.DisableAndNoOneDoc

			});

			this._codeStatusIturGroupDictionary.Add(IturStatusGroupEnum.DisableWithDocs.ToString(),
			new StatusIturGroup()
			{
				Name = IturStatusGroupTitle.DisableWithDocs,
				NameLocalizationCode = "IturStatusGroup_DisableWithDocs",
				Description = IturStatusGroupTitle.DisableWithDocs,
				Code = IturStatusGroupEnum.DisableWithDocs.ToString(),
				BackgroundColor = "155, 250, 122",
				Bit = (int)IturStatusGroupEnum.DisableWithDocs

			});

			this._codeStatusIturGroupDictionary.Add(IturStatusGroupEnum.Error.ToString(),
			new StatusIturGroup()
			{
				Name = IturStatusGroupTitle.Error,
				NameLocalizationCode = "IturStatusGroup_Error",
				Description = IturStatusGroupTitle.Error,
				Code = IturStatusGroupEnum.Error.ToString(),
				BackgroundColor = "155, 250, 122",
				Bit = (int)IturStatusGroupEnum.Error

			});

			//this._codeStatusIturGroupDictionary.Add(IturStatusGroupEnum.None.ToString(),
			//	new StatusIturGroup()
			//	{
			//		Name = IturStatusGroupTitle.None,
			//		NameLocalizationCode = "IturStatusGroup_None",
			//		Description = IturStatusGroupTitle.None,
			//		Code = IturStatusGroupEnum.None.ToString(),
			//		BackgroundColor = "128, 128, 128",
			//		Bit = (int)IturStatusGroupEnum.None

			//	});

		}


		public Dictionary<int, IturStatusGroupEnum> BitStatusIturGroupEnumDictionary
		{
			get
			{
				return this._bitStatusIturGroupEnumDictionary;
			}
		}

		private void FillBitStatusIturGroupEnumDictionary()
		{
			
				if (this._bitStatusIturGroupEnumDictionary == null)
				{
					this._bitStatusIturGroupEnumDictionary = new Dictionary<int, IturStatusGroupEnum>();
				}

					this._bitStatusIturGroupEnumDictionary.Add((int)IturStatusGroupEnum.Empty,
						IturStatusGroupEnum.Empty);

                    this._bitStatusIturGroupEnumDictionary.Add((int)IturStatusGroupEnum.OneDocIsApprove,
                        IturStatusGroupEnum.OneDocIsApprove);

					this._bitStatusIturGroupEnumDictionary.Add((int)IturStatusGroupEnum.AllDocsIsApprove,
						IturStatusGroupEnum.AllDocsIsApprove);

					this._bitStatusIturGroupEnumDictionary.Add((int)IturStatusGroupEnum.DisableAndNoOneDoc,
						IturStatusGroupEnum.DisableAndNoOneDoc);

					this._bitStatusIturGroupEnumDictionary.Add((int)IturStatusGroupEnum.DisableWithDocs,
						IturStatusGroupEnum.DisableWithDocs);

					this._bitStatusIturGroupEnumDictionary.Add((int)IturStatusGroupEnum.NotApprove,
						IturStatusGroupEnum.NotApprove);

					this._bitStatusIturGroupEnumDictionary.Add((int)IturStatusGroupEnum.Error,
						IturStatusGroupEnum.Error);
		}



		public void SetBackgroundColor(string pathDB, IturStatusGroupEnum iturStatusGroup, string backgroundColor)
		{
			this.CodeStatusIturGroupDictionary[iturStatusGroup.ToString()].BackgroundColor = backgroundColor; 
		}

		public IturStatusGroupEnum GetIturStatusGroup(IturStatusEnum iturStatus)
		{
			switch (iturStatus)
			{
				case IturStatusEnum.NoOneDoc:
					return IturStatusGroupEnum.Empty;
				case IturStatusEnum.OneDocIsApprove:
						return IturStatusGroupEnum.OneDocIsApprove;
				case IturStatusEnum.SomeDocIsApprove:
						return IturStatusGroupEnum.AllDocsIsApprove;
				case IturStatusEnum.OneDocIsNotApprove:
				case IturStatusEnum.SomeDocIsNotApprove:
						return IturStatusGroupEnum.NotApprove;
				case IturStatusEnum.DisableAndNoOneDoc:
					return IturStatusGroupEnum.DisableAndNoOneDoc;
				case IturStatusEnum.DisableAndSomeDocIsApprove:
				case IturStatusEnum.DisableAndSomeDocIsNotApprove:
				case IturStatusEnum.DisableAndOneDocIsApprove:
				case IturStatusEnum.DisableAndOneDocIsNotApprove:
					return IturStatusGroupEnum.DisableWithDocs;
				case IturStatusEnum.WarningConvert:
				//case IturStatusEnum.AllDocIsApprove:
				//case IturStatusEnum.AllDocIsApprove:
				    return IturStatusGroupEnum.Error; 

		  		default:
					{
						return IturStatusGroupEnum.Error;
					}
			}
		}


		//public IturStatusGroupEnum GetIturStatusGroup(int iturStatus)
		//{
		//    switch (iturStatus)
		//    {
		//        case (int)IturStatusEnum.NoOneDoc:
		//            return IturStatusGroupEnum.Empty;
		//        case (int)IturStatusEnum.OneDocIsApprove:
		//            return IturStatusGroupEnum.OneDocIsApprove;
		//        case (int)IturStatusEnum.SomeDocIsApprove:
		//            return IturStatusGroupEnum.AllDocsIsApprove;
		//        case (int)IturStatusEnum.OneDocIsNotApprove:
		//        case (int)IturStatusEnum.SomeDocIsNotApprove:
		//            return IturStatusGroupEnum.NotApprove;
		//        case (int)IturStatusEnum.DisableAndNoOneDoc:
		//            return IturStatusGroupEnum.DisableAndNoOneDoc;
		//        case (int)IturStatusEnum.DisableAndSomeDocIsApprove:
		//        case (int)IturStatusEnum.DisableAndSomeDocIsNotApprove:
		//        case (int)IturStatusEnum.DisableAndOneDocIsApprove:
		//        case (int)IturStatusEnum.DisableAndOneDocIsNotApprove:
		//            return IturStatusGroupEnum.DisableWithDocs;
		//        case (int)IturStatusEnum.WarningConvert:
		//            //case IturStatusEnum.AllDocIsApprove:
		//            //case IturStatusEnum.AllDocIsApprove:
		//            return IturStatusGroupEnum.Error;

		//        default:
		//            {
		//                return IturStatusGroupEnum.Error;
		//            }
		//    }
		//}

		public string FromIturStatusGroupBitToLocalizationCode(int statusGroupBit)
		{
			if (this.BitStatusIturGroupEnumDictionary.ContainsKey(statusGroupBit) == false) return "";
			
			IturStatusGroupEnum statusGroupEnum = this.BitStatusIturGroupEnumDictionary[statusGroupBit];

			if (this.CodeStatusIturGroupDictionary.ContainsKey(statusGroupEnum.ToString()) == false) return "";
			return this.CodeStatusIturGroupDictionary[statusGroupEnum.ToString()].NameLocalizationCode;
		}

		public List<IturStatusEnum> GetIturStatusList(IturStatusGroupEnum iturStatusGroup)
		{
			List<IturStatusEnum> ret = new List<IturStatusEnum>();
			switch (iturStatusGroup)
			{
				case IturStatusGroupEnum.Empty:
					{
						ret.Add(IturStatusEnum.NoOneDoc);
						return ret;
					}
				case IturStatusGroupEnum.OneDocIsApprove:
					{
						ret.Add(IturStatusEnum.OneDocIsApprove);
						return ret;
					}
				case IturStatusGroupEnum.AllDocsIsApprove:
					{
						ret.Add(IturStatusEnum.SomeDocIsApprove);
						return ret;
					}
				case IturStatusGroupEnum.NotApprove:
					{
						ret.Add(IturStatusEnum.OneDocIsNotApprove);
						ret.Add(IturStatusEnum.SomeDocIsNotApprove);
						return ret;
					}
				case IturStatusGroupEnum.DisableAndNoOneDoc:
					{
						ret.Add(IturStatusEnum.DisableAndNoOneDoc);
						return ret;
					}
				case IturStatusGroupEnum.DisableWithDocs:
					{
						ret.Add(IturStatusEnum.DisableAndSomeDocIsApprove);
						ret.Add(IturStatusEnum.DisableAndSomeDocIsNotApprove);
						ret.Add(IturStatusEnum.DisableAndOneDocIsApprove);
						ret.Add(IturStatusEnum.DisableAndOneDocIsNotApprove);
						return ret;
					}
				default:
					{
						return ret;
					}
			}

	
				//case IturStatusEnum.AllDocIsApprove:
				//case IturStatusEnum.AllDocIsApprove:
				//case IturStatusEnum.AllDocIsApprove:
				//    return IturStatusGroupEnum.Error; 

			}
		#endregion

	
	}
}
