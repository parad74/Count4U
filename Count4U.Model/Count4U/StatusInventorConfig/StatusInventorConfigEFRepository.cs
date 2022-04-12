using System;
using System.Linq;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Model.Count4U.MappingEF;
using System.Data.Entity.Core.Objects;
using Count4U.Model.Interface;

namespace Count4U.Model.Count4U
{
	public class StatusInventorConfigEFRepository : BaseEFRepository, IStatusInventorConfigRepository
	{
		public StatusInventorConfigEFRepository(IConnectionDB connection)
			: base(connection)
        {

        }

        #region BaseEFRepository Members

        public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
        {
            return objectSet.AsQueryable();
        }
        #endregion


		private StatusInventorConfigs _list;

		#region IStatusInventorConfigRepository Members
		/// <summary>
	   /// Статус аудита TODO: {New, Process, Dirty, Save, Complet, Exclude}
	   /// </summary>
	   /// <returns></returns>
		public StatusInventorConfigs GetStatuses()
		{
			if (this._list == null)
			{
				this._list = new StatusInventorConfigs {
                    new StatusInventorConfig() { ID = (long)StatusInventorEnum.New, 
						Code = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.New),
						Name = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.New), 
						Description = "NewDescription" },
					new StatusInventorConfig() { ID = (long)StatusInventorEnum.Open, 
						Code = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Open),
						Name = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Open), 
						Description = "OpenDescription" },
					new StatusInventorConfig() { ID = (long)StatusInventorEnum.Reopen, 
						Code = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Reopen),
						Name = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Reopen), 
						Description = "ReopenDescription" },
                    new StatusInventorConfig() { ID = (long)StatusInventorEnum.Process, 
						Code = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Process),
						Name = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Process), 
						Description = "ProcessDescription" },
                    new StatusInventorConfig() { ID = (long)StatusInventorEnum.Dirty, 
						Code = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Dirty),
						Name = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Dirty), 
						Description = "DirtyDescription" },
	                new StatusInventorConfig() { ID = (long)StatusInventorEnum.Complete, 
						Code = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Complete),
						Name = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Complete), 
						Description = "CompleteDescription" },
                    new StatusInventorConfig() { ID = (long)StatusInventorEnum.Send,
						Code = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Send),
						Name = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Send), 
						Description = "SendDescription" },
			       new StatusInventorConfig() { ID = (long)StatusInventorEnum.Exclude,
						Code = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Exclude),
						Name = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.Exclude), 
						Description = "ExcludeDescription" },
					new StatusInventorConfig() { ID = (long)StatusInventorEnum.None,
						Code = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.None),
						Name = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.None), 
						Description = "NoneDescription" }
                };
				
			}
			return this._list;
		}

		public StatusInventorConfigs GetStatuses(SelectParams selectParams)
		{
			throw new NotImplementedException();
		}

		public StatusInventorConfig CreateNoneStatusInventor()
		{
			return new StatusInventorConfig()
			{
				ID = (long)StatusInventorEnum.None,
				Code = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.None),
				Name = Enum.GetName(typeof(StatusInventorEnum), StatusInventorEnum.None),
				Description = "NoneDescription"
			};
		}

		public StatusInventorConfig GetStatusByName(string name)
		{
			var entity = this.GetStatuses().First(e => e.Name.CompareTo(name) == 0);
			return entity;
		}

		public StatusInventorConfig GetStatusByCode(string statusCode)
		{
			var entity = this.GetStatuses().First(e => e.Code.CompareTo(statusCode) == 0);
			return entity;
		}

		#endregion

		#region private

		private App_Data.StatusInventorConfig GetEntityByCode(App_Data.Count4UDB dc, string code)
		{
			var entity = dc.StatusInventorConfigs.FirstOrDefault(e => e.Code.CompareTo(code) == 0);
			return entity;
		}

		#endregion

	}
}
