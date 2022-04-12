using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model
{
	public static class DBName
	{
		public const string AuditDB = "AuditDB.sdf";
		public const string Count4UDB = "Count4UDB.sdf";
		public const string EmptyCount4UDB = "EmptyCount4UDB.sdf";
		public const string MainDB = "MainDB.sdf";
		public const string ProcessDB = "ProcessDB.sdf";
		public const string AnalyticDB = "AnalyticDB.sdf";
		public const string EmptyAnalyticDB = "EmptyAnalyticDB.sdf";
		public const string EmptyAuditDB = "EmptyAuditDB.sdf";
		public const string EmptyMainDB = "EmptyMainDB.sdf";
									   
		
	}

	public enum DBType
	{
		AuditDB = 0,
		MainDB = 1,
		Count4UDB = 2,
		EmptyCount4UDB = 3
	}
}
