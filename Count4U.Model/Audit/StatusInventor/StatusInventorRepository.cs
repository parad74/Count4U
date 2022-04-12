using System;
using System.Linq;
using Count4U.Model.Interface.Audit;
using Count4U.Model.SelectionParams;

namespace Count4U.Model.Audit
{
	public class StatusInventorRepository : IStatusInventorRepository
	{
		private StatusInventors  _list;

		#region IStatusInventorRepository Members
	   /// <summary>
	   /// Статус аудита TODO: {New, Open, Reopen, Process, Dirty, Complete, Send, Exclude}
	   /// </summary>
	   /// <returns></returns>
		public StatusInventors GetStatuses()
		{
			if (this._list == null)
			{
	          		this._list = new StatusInventors {
                    new StatusInventor() { ID = (long)StatusInventorEnum.New, 
						Code = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.New),
						Name = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.New), 
						Description = "NewDescription" },
					new StatusInventor() { ID = (long)StatusInventorEnum.Open, 
						Code = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Open),
						Name = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Open), 
						Description = "OpenDescription" },
					new StatusInventor() { ID = (long)StatusInventorEnum.Reopen, 
						Code = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Reopen),
						Name = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Reopen), 
						Description = "ReopenDescription" },
                    new StatusInventor() { ID = (long)StatusInventorEnum.Process, 
						Code = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Process),
						Name = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Process), 
						Description = "ProcessDescription" },
                    new StatusInventor() { ID = (long)StatusInventorEnum.Dirty, 
						Code = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Dirty),
						Name = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Dirty), 
						Description = "DirtyDescription" },
	                new StatusInventor() { ID = (long)StatusInventorEnum.Complete, 
						Code = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Complete),
						Name = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Complete), 
						Description = "CompleteDescription" },
                    new StatusInventor() { ID = (long)StatusInventorEnum.Send,
						Code = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Send),
						Name = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Send), 
						Description = "SendDescription" },
			       new StatusInventor() { ID = (long)StatusInventorEnum.Exclude,
						Code = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Exclude),
						Name = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Exclude), 
						Description = "ExcludeDescription" },
					new StatusInventor() { ID = (long)StatusInventorEnum.None,
						Code = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.None),
						Name = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.None), 
						Description = "NoneDescription" }
                };
				
			}
			return this._list;
		}

		public StatusInventors GetStatuses(SelectParams selectParams)
		{
			throw new NotImplementedException();
		}

		public StatusInventor CreateNoneStatusInventor()
		{
			return new StatusInventor()
			{
				ID = (long)StatusInventorEnum.None,
				Code = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.None),
				Name = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.None),
				Description = "NoneDescription"
			};
		}

		public StatusInventor GetStatusByName(string name)
		{
			var stuses = GetStatuses();
			if(stuses == null) return  null;
			var entity = stuses.First(e => e.Name.CompareTo(name) == 0);
			if(entity == null) return  null;
			return entity;
		}

		public StatusInventor GetStatusByCode(string statusCode)
		{
			var stuses = GetStatuses();
			if(stuses == null) return  null;
			var entity = stuses.First(e => e.Code.CompareTo(statusCode) == 0);
			if(entity == null) return  null;
			return entity;
		}

		#endregion

		#region private

		private StatusInventor GetEntityByCode(string code)
		{
			var entity = GetStatuses().First(e => e.Code.CompareTo(code) == 0);
			return entity;
		}

		#endregion
	}
}
