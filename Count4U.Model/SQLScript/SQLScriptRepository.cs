using System;
//using System.Windows.Forms;
using System.Linq;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using System.Collections.Generic;
using Count4U.Model.Interface;
using Count4U.Model.Count4U;
using Microsoft.Reporting.WinForms;
using System.IO;
using System.Text;
using Count4U.Localization;

namespace Count4U.Model
{
	public class SQLScriptRepository : ISQLScriptRepository
	{
		
		public SQLScriptRepository()
		{
		}

	

		//public SQLScriptRepository()
		//{
		//}

		#region ISQLScriptRepository Members

		//public void Clear()
		//{
		//    throw new NotImplementedException();
		//}

		public SQLScripts GetScripts()
		{
			// ver = 2	  12/12/2011	  тест
			SQLScripts scripts = new SQLScripts();
			scripts.Add(new SQLScript
				{
					Ver = 2,
					Tab = 1,
					DBType = DBName.AuditDB,
					Text = @"CREATE TABLE [InventorMask] (
  [ID] bigint NOT NULL  IDENTITY (1,1)
, [Code] nvarchar(100) NULL
, [AdapterCode] nvarchar(100) NULL
, [FileCode] nvarchar(100) NULL
, [BarcodeMask] nvarchar(100) NULL
, [MakadMask] nvarchar(100) NULL
);"
				});
				scripts.Add (new SQLScript{
					Ver = 2,
					Tab = 2,
					DBType = DBName.AuditDB,
					Text = @"ALTER TABLE [InventorMask] ADD CONSTRAINT [PK_InventorMask] PRIMARY KEY ([ID]);"
				});
				scripts.Add (new SQLScript{
					Ver = 2,
					Tab = 3,
					DBType = DBName.AuditDB,
					Text = @"CREATE UNIQUE INDEX [UQ__InventorMask__0000000000000211] ON [InventorMask] ([ID] ASC);"
				});


				scripts.Add(new SQLScript
				{
					Ver = 2,
					Tab = 4,
					DBType = DBName.MainDB,
					Text = @"CREATE TABLE [BranchMask] (
  [ID] bigint NOT NULL  IDENTITY (1,1)
, [Code] nvarchar(100) NULL
, [AdapterCode] nvarchar(100) NULL
, [FileCode] nvarchar(100) NULL
, [BarcodeMask] nvarchar(100) NULL
, [MakadMask] nvarchar(100) NULL
);"
				});
				scripts.Add (new SQLScript{
					Ver = 2,
					Tab = 5,
					DBType = DBName.MainDB,
					Text = @"ALTER TABLE [BranchMask] ADD CONSTRAINT [PK_BranchMask] PRIMARY KEY ([ID]);"
				} );

				scripts.Add(new SQLScript
				{
					Ver = 2,
					Tab = 6,
					DBType = DBName.MainDB,
					Text = @"CREATE UNIQUE INDEX [UQ__BranchMask__0000000000000211] ON [BranchMask] ([ID] ASC);"
				});
			
				scripts.Add(new SQLScript
				{
					Ver = 2,
					Tab = 14,
					DBType = DBName.MainDB,
					Text = @"CREATE TABLE [CustomerMask] (
					  [ID] bigint NOT NULL  IDENTITY (1,1)
					, [Code] nvarchar(100) NULL
					, [AdapterCode] nvarchar(100) NULL
					, [FileCode] nvarchar(100) NULL
					, [BarcodeMask] nvarchar(100) NULL
					, [MakadMask] nvarchar(100) NULL
					); "
				});
				scripts.Add (new SQLScript{
					Ver = 2,
					Tab = 15,
					DBType = DBName.MainDB,
					Text = @"ALTER TABLE [CustomerMask] ADD CONSTRAINT [PK_CustomerMask] PRIMARY KEY ([ID]);"
				} );

				scripts.Add(new SQLScript
				{
					Ver = 2,
					Tab = 16,
					DBType = DBName.MainDB,
					Text = @"CREATE UNIQUE INDEX [UQ__CustomerMask__0000000000000211] ON [CustomerMask] ([ID] ASC);"
				});
			

				scripts.Add(new SQLScript
				{
					Ver = 2,
					Tab = 27,
					DBType = DBName.MainDB,
					Text = @"ALTER TABLE [Branch] add MaskCode nvarchar(100) NULL"
				});
				scripts.Add(new SQLScript
				{
					Ver = 2,
					Tab = 28,
					DBType = DBName.MainDB,
					Text = @"ALTER TABLE [Branch] add ReportPath nvarchar(250) NULL"
				});
				scripts.Add(new SQLScript
				{
					Ver = 2,
					Tab = 29,
					DBType = DBName.MainDB,
					Text = @"ALTER TABLE [Customer] add MaskCode nvarchar(100) NULL"
				});
				scripts.Add(new SQLScript
				{
					Ver = 2,
					Tab = 30,
					DBType = DBName.MainDB,
					Text = @"ALTER TABLE [Customer] add ReportPath nvarchar(250) NULL"
				});

			//ver 3			14/12/2011
				scripts.Add(new SQLScript
				{
					Ver = 3,
					Tab = 1,
					DBType = DBName.Count4UDB,
					Text = @"ALTER TABLE [Product] add MakatOriginal nvarchar(100) NULL"
				});
				scripts.Add(new SQLScript
				{
					Ver = 3,
					Tab = 2,
					DBType = DBName.EmptyCount4UDB,
					Text = @"ALTER TABLE [Product] add MakatOriginal nvarchar(100) NULL"
				});
			//ver 4			22/12/2011
				scripts.Add(new SQLScript
				{
					Ver = 4,
					Tab = 1,
					DBType = DBName.MainDB,
					Text = @"DROP TABLE [Report]"
				});

				scripts.Add(new SQLScript
				{
					Ver = 4,
					Tab = 2,
					DBType = DBName.MainDB,
					Text = @"CREATE TABLE [Report] (
					  [ID] bigint NOT NULL  IDENTITY (1,1)
					, [Code] nvarchar(100) NULL
					, [Description] nvarchar(500) NULL
					, [DomainContext] nvarchar(100) NULL
					, [TypeDS] nvarchar(100) NULL
					, [Path] nvarchar(250) NULL
					, [FileName] nvarchar(100) NULL
					, [DomainType] nvarchar(100) NULL
					); "
				});

	
			scripts.Add (new SQLScript{
					Ver = 4,
					Tab = 4,
					DBType = DBName.MainDB,
					Text = @"ALTER TABLE [Report] ADD CONSTRAINT [PK_Report] PRIMARY KEY ([ID]);"
				} );

				scripts.Add(new SQLScript
				{
					Ver = 4,
					Tab = 5,
					DBType = DBName.MainDB,
					Text = @"CREATE UNIQUE INDEX [UQ__Report__0000000000000308] ON [Report] ([ID] ASC);"
				});
			scripts.Add(new SQLScript
				{
					Ver = 4,
					Tab = 6,
					DBType = DBName.MainDB,
					Text = @"
					CREATE TABLE [ImportAdapter] (
					  [ID] bigint NOT NULL  IDENTITY (1,1)
					, [Code] nvarchar(100) NULL
					, [AdapterCode] nvarchar(100) NULL
					, [DomainType] nvarchar(100) NULL
					, [Description] nvarchar(100) NULL
					);"		});

					scripts.Add(new SQLScript
				{
					Ver = 4,
					Tab = 7,
					DBType = DBName.MainDB,
					Text = @"ALTER TABLE [ImportAdapter] ADD CONSTRAINT [PK_ImportAdapter] PRIMARY KEY ([ID]);"
				});
				scripts.Add(new SQLScript
				{
					Ver = 4,
					Tab = 8,
					DBType = DBName.MainDB,
					Text = @"CREATE UNIQUE INDEX [UQ__ImportAdapter__00000000000002A0] ON [ImportAdapter] ([ID] ASC);"
				});

			//ver 5			3/1/2012
			scripts.Add(new SQLScript
			{
				Ver = 5,
				Tab = 1,
				DBType = DBName.MainDB,
				Text = @"CREATE TABLE [MainDBIni] ([ID] bigint NOT NULL  IDENTITY (1,1), [Ver] nvarchar(10) NULL, [Code] nvarchar(100) NULL);
				ALTER TABLE [MainDBIni] ADD CONSTRAINT [PK_MainDBIni] PRIMARY KEY ([ID]);
				CREATE UNIQUE INDEX [UQ__MainDBIni__0000000000000212] ON [MainDBIni] ([ID] ASC); "
			});

			scripts.Add(new SQLScript
			{
				Ver = 5,
				Tab = 2,
				DBType = DBName.AuditDB,
				Text = @"CREATE TABLE [AuditDBIni] ([ID] bigint NOT NULL  IDENTITY (1,1), [Ver] nvarchar(10) NULL, [Code] nvarchar(100) NULL);
					ALTER TABLE [AuditDBIni] ADD CONSTRAINT [PK_AuditDBIni] PRIMARY KEY ([ID]);
					CREATE UNIQUE INDEX [UQ__AuditDBIni__0000000000000212] ON [AuditDBIni] ([ID] ASC); "
			});

			scripts.Add(new SQLScript
			{
				Ver = 5,
				Tab = 3,
				DBType = DBName.Count4UDB,
				Text = @"CREATE TABLE [Count4UDBIni] ([ID] bigint NOT NULL  IDENTITY (1,1), [Ver] nvarchar(10) NULL, [Code] nvarchar(100) NULL);
					ALTER TABLE [Count4UDBIni] ADD CONSTRAINT [PK_Count4UDBIni] PRIMARY KEY ([ID]);
					CREATE UNIQUE INDEX [UQ__Count4UDBIni__0000000000000212] ON [Count4UDBIni] ([ID] ASC); "
			});

			scripts.Add(new SQLScript
			{
				Ver = 5,
				Tab = 13,
				DBType = DBName.EmptyCount4UDB,
				Text = @"CREATE TABLE [Count4UDBIni] ([ID] bigint NOT NULL  IDENTITY (1,1), [Ver] nvarchar(10) NULL, [Code] nvarchar(100) NULL);
					ALTER TABLE [Count4UDBIni] ADD CONSTRAINT [PK_Count4UDBIni] PRIMARY KEY ([ID]);
					CREATE UNIQUE INDEX [UQ__Count4UDBIni__0000000000000212] ON [Count4UDBIni] ([ID] ASC); "
			});

			scripts.Add(new SQLScript
			{
				Ver = 5,
				Tab = 4,
				DBType = DBName.AuditDB,
				Text = @"CREATE TABLE [AuditReport] (
  [ID] bigint NOT NULL  IDENTITY (1,1)
, [Code] nvarchar(100) NULL
, [Description] nvarchar(500) NULL
, [DomainContext] nvarchar(100) NULL
, [TypeDS] nvarchar(100) NULL
, [Path] nvarchar(250) NULL
, [FileName] nvarchar(100) NULL
, [DomainType] nvarchar(100) NULL
);"
			});

			scripts.Add(new SQLScript
			{
				Ver = 5,
				Tab = 5,
				DBType = DBName.AuditDB,
				Text = @"ALTER TABLE [AuditReport] ADD CONSTRAINT [PK_AuditReport] PRIMARY KEY ([ID]);"
			});

			scripts.Add(new SQLScript
			{
				Ver = 5,
				Tab = 6,
				DBType = DBName.AuditDB,
				Text = @"CREATE UNIQUE INDEX [UQ__AuditReport__0000000000000308] ON [AuditReport] ([ID] ASC);"
			});

			// ver 6 - 5/1/2012
			scripts.Add(new SQLScript
			{
				Ver = 6,
				Tab = 1,
				DBType = DBName.Count4UDB,
				Text = @"INSERT INTO [Count4UDBIni] ([Ver]) VALUES (N'6');"
			});

			scripts.Add(new SQLScript
			{
				Ver = 6,
				Tab = 2,
				DBType = DBName.EmptyCount4UDB,
				Text = @"INSERT INTO [Count4UDBIni] ([Ver]) VALUES (N'6');"
			});

			scripts.Add(new SQLScript
			{
				Ver = 6,
				Tab = 3,
				DBType = DBName.AuditDB,
				Text = @"INSERT INTO [AuditDBIni] ([Ver]) VALUES (N'6');"
			});

			scripts.Add(new SQLScript
			{
				Ver = 6,
				Tab = 4,
				DBType = DBName.MainDB,
				Text = @"INSERT INTO [MainDBIni] ([Ver]) VALUES (N'6');"
			});
			
			scripts.Add(new SQLScript
			{
				Ver = 6,
				Tab = 5,
				DBType = DBName.Count4UDB,
				Text = @"ALTER TABLE [Itur] add Disabled bit NULL;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 6,
				Tab = 6,
				DBType = DBName.EmptyCount4UDB,
				Text = @"ALTER TABLE [Itur] add Disabled bit NULL;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 6,
				Tab = 7,
				DBType = DBName.Count4UDB,
				Text = @"CREATE TABLE [IturAnalyzes] (
  [ID] bigint NOT NULL  IDENTITY (1,1)
, [Itur_Disabled] bit NULL
, [Itur_Publishe] bit NULL
, [Itur_StatusIturBit] int NULL DEFAULT 0
, [Itur_Number] int NOT NULL DEFAULT 0
, [Itur_NumberPrefix] nvarchar(50) NULL
, [Itur_NumberSufix] nvarchar(50) NULL
, [Itur_LocationCode] nvarchar(50) NULL
, [Itur_StatusIturGroupBit] int NULL DEFAULT 0
, [Itur_StatusDocHeaderBit] int NULL DEFAULT 0
, [Doc_Name] nvarchar(50) NULL
, [Doc_Approve] bit NULL
, [Doc_IturCode] nvarchar(50) NULL
, [Doc_WorkerGUID] nvarchar(50) NULL
, [Doc_StatusDocHeaderBit] int NULL DEFAULT 0
, [Doc_StatusInventProductBit] int NULL DEFAULT 0
, [Doc_StatusApproveBit] int NULL DEFAULT 0
, [PDA_Barcode] nvarchar(50) NULL
, [PDA_QuantityDifference] nvarchar(10) NULL
, [PDA_QuantityEdit] float NULL
, [PDA_QuantityOriginal] float NULL
, [PDA_SerialNumber] nvarchar(50) NULL
, [PDA_ShelfCode] nvarchar(10) NULL
, [PDA_ModifyDate] datetime NULL DEFAULT GETDATE ( )
, [PDA_ProductName] nvarchar(100) NULL
, [PDA_StatusInventProductBit] int NULL DEFAULT 0
, [PDA_DocumentHeaderCode] nvarchar(50) NULL
, [PDA_InputTypeCode] nvarchar(50) NULL
);
ALTER TABLE [IturAnalyzes] ADD CONSTRAINT [PK__IturAnalyzes__00000000000002D0] PRIMARY KEY ([ID]);
CREATE UNIQUE INDEX [UQ__IturAnalyzes__0000000000000703] ON [IturAnalyzes] ([ID] ASC);
"
			});

			scripts.Add(new SQLScript
			{
				Ver = 6,
				Tab = 8,
				DBType = DBName.EmptyCount4UDB,
				Text = @"CREATE TABLE [IturAnalyzes] (
  [ID] bigint NOT NULL  IDENTITY (1,1)
, [Itur_Disabled] bit NULL
, [Itur_Publishe] bit NULL
, [Itur_StatusIturBit] int NULL DEFAULT 0
, [Itur_Number] int NOT NULL DEFAULT 0
, [Itur_NumberPrefix] nvarchar(50) NULL
, [Itur_NumberSufix] nvarchar(50) NULL
, [Itur_LocationCode] nvarchar(50) NULL
, [Itur_StatusIturGroupBit] int NULL DEFAULT 0
, [Itur_StatusDocHeaderBit] int NULL DEFAULT 0
, [Doc_Name] nvarchar(50) NULL
, [Doc_Approve] bit NULL
, [Doc_IturCode] nvarchar(50) NULL
, [Doc_WorkerGUID] nvarchar(50) NULL
, [Doc_StatusDocHeaderBit] int NULL DEFAULT 0
, [Doc_StatusInventProductBit] int NULL DEFAULT 0
, [Doc_StatusApproveBit] int NULL DEFAULT 0
, [PDA_Barcode] nvarchar(50) NULL
, [PDA_QuantityDifference] nvarchar(10) NULL
, [PDA_QuantityEdit] float NULL
, [PDA_QuantityOriginal] float NULL
, [PDA_SerialNumber] nvarchar(50) NULL
, [PDA_ShelfCode] nvarchar(10) NULL
, [PDA_ModifyDate] datetime NULL DEFAULT GETDATE ( )
, [PDA_ProductName] nvarchar(100) NULL
, [PDA_StatusInventProductBit] int NULL DEFAULT 0
, [PDA_DocumentHeaderCode] nvarchar(50) NULL
, [PDA_InputTypeCode] nvarchar(50) NULL
);
ALTER TABLE [IturAnalyzes] ADD CONSTRAINT [PK__IturAnalyzes__00000000000002D0] PRIMARY KEY ([ID]);
CREATE UNIQUE INDEX [UQ__IturAnalyzes__0000000000000703] ON [IturAnalyzes] ([ID] ASC);
"
			});


			scripts.Add(new SQLScript
			{
				Ver = 7,
				Tab = 5,
				DBType = DBName.MainDB,
				Text = @"ALTER TABLE [Branch] add ExportCatalogAdapterCode nvarchar(100) NULL;
ALTER TABLE [Branch] add ExportIturAdapterCode nvarchar(100) NULL;
ALTER TABLE [Customer] add ExportCatalogAdapterCode nvarchar(100) NULL;
ALTER TABLE [Customer] add ExportIturAdapterCode nvarchar(100) NULL;"
			});

			// ver 6 - 5/1/2012
			scripts.Add(new SQLScript
			{
				Ver = 8,
				Tab = 1,
				DBType = DBName.Count4UDB,
				Text = @"ALTER TABLE [IturAnalyzes] add Code nvarchar(50) NULL;
ALTER TABLE [IturAnalyzes] add LocationCode nvarchar(50) NULL;
ALTER TABLE [IturAnalyzes] add DocumentHeaderCode nvarchar(50) NULL;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 8,
				Tab = 2,
				DBType = DBName.EmptyCount4UDB,
				Text = @"ALTER TABLE [IturAnalyzes] add Code nvarchar(50) NULL;
ALTER TABLE [IturAnalyzes] add LocationCode nvarchar(50) NULL;
ALTER TABLE [IturAnalyzes] add DocumentHeaderCode nvarchar(50) NULL;"
			});


			scripts.Add(new SQLScript
			{
				Ver = 9,
				Tab = 2,
				DBType = DBName.Count4UDB,
				Text = @"ALTER TABLE [DocumentHeader] add Num int NULL DEFAULT 1;
ALTER TABLE [IturAnalyzes] add BarcodeOriginal nvarchar(100) NULL;
ALTER TABLE [IturAnalyzes] add MakatOriginal nvarchar(100) NULL;
ALTER TABLE [IturAnalyzes] add PriceString nvarchar(50) NULL;
ALTER TABLE [IturAnalyzes] add PriceBuy float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add PriceSale float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add PriceExtra float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] DROP COLUMN PDA_QuantityDifference;
ALTER TABLE [IturAnalyzes] add PDA_QuantityDifferenc float NULL DEFAULT 0;
ALTER TABLE [InventProduct] add Num bigint NULL DEFAULT 1;
ALTER TABLE [Product] DROP COLUMN ParentCode;
ALTER TABLE [Product] DROP COLUMN ParentBarcode;
ALTER TABLE [InventProduct]  DROP COLUMN QuantityDifference;
ALTER TABLE [InventProduct] add QuantityDifference float  NULL DEFAULT 0;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 9,
				Tab = 3,
				DBType = DBName.EmptyCount4UDB,
				Text = @"ALTER TABLE [DocumentHeader] add Num int NULL DEFAULT 1;
ALTER TABLE [IturAnalyzes] add BarcodeOriginal nvarchar(100) NULL;
ALTER TABLE [IturAnalyzes] add MakatOriginal nvarchar(100) NULL;
ALTER TABLE [IturAnalyzes] add PriceString nvarchar(50) NULL;
ALTER TABLE [IturAnalyzes] add PriceBuy float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add PriceSale float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add PriceExtra float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] DROP COLUMN PDA_QuantityDifference;
ALTER TABLE [IturAnalyzes] add PDA_QuantityDifferenc float NULL DEFAULT 0;
ALTER TABLE [InventProduct] add Num bigint NULL DEFAULT 1;
ALTER TABLE [Product] DROP COLUMN ParentCode;
ALTER TABLE [Product] DROP COLUMN ParentBarcode;
ALTER TABLE [InventProduct]  DROP COLUMN QuantityDifference;
ALTER TABLE [InventProduct] add QuantityDifference float  NULL DEFAULT 0;"
			});

			// ver 10 - 7/2/2012
			scripts.Add(new SQLScript
			{
				Ver = 10,
				Tab = 4,
				DBType = DBName.Count4UDB,
				Text = @"ALTER TABLE [IturAnalyzes] add Doc_Num int NULL DEFAULT 1;
ALTER TABLE [IturAnalyzes] add PDA_Num int NULL DEFAULT 1;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 10,
				Tab = 5,
				DBType = DBName.EmptyCount4UDB,
				Text = @"ALTER TABLE [IturAnalyzes] add Doc_Num int NULL DEFAULT 1;
ALTER TABLE [IturAnalyzes] add PDA_Num int NULL DEFAULT 1;"
			});


			scripts.Add(new SQLScript
			{
				Ver = 11,
				Tab = 1,
				DBType = DBName.Count4UDB,
				Text = @"ALTER TABLE [InventProduct] add DocumentCode nvarchar(50) NULL;
ALTER TABLE [InventProduct] add IturCode nvarchar(50) NULL;
ALTER TABLE [IturAnalyzes] add DocumentCode nvarchar(50) NULL;
ALTER TABLE [IturAnalyzes] add IturCode nvarchar(50) NULL; "
			});
			scripts.Add(new SQLScript
			{
				Ver = 11,
				Tab = 2,
				DBType = DBName.EmptyCount4UDB,
				Text = @"ALTER TABLE [InventProduct] add DocumentCode nvarchar(50) NULL;
ALTER TABLE [InventProduct] add IturCode nvarchar(50) NULL;
ALTER TABLE [IturAnalyzes] add DocumentCode nvarchar(50) NULL;
ALTER TABLE [IturAnalyzes] add IturCode nvarchar(50) NULL; "
			});

			scripts.Add(new SQLScript
			{
				Ver = 12,
				Tab = 2,
				DBType = DBName.Count4UDB,
				Text = @"ALTER TABLE [InventProduct] add FromCatalogType int NULL DEFAULT 0;
ALTER TABLE [InventProduct] add SectionNum int NULL DEFAULT 0;
ALTER TABLE [Product] add FromCatalogType int NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add FromCatalogType int NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add SectionNum int NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add TypeCode nvarchar(10) NULL;
ALTER TABLE [InventProduct] DROP COLUMN [QuantityEdit];
ALTER TABLE [InventProduct] add [QuantityEdit] float NULL DEFAULT 0;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 12,
				Tab = 3,
				DBType = DBName.EmptyCount4UDB,
				Text = @"ALTER TABLE [InventProduct] add FromCatalogType int NULL DEFAULT 0;
ALTER TABLE [InventProduct] add SectionNum int NULL DEFAULT 0;
ALTER TABLE [Product] add FromCatalogType int NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add FromCatalogType int NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add SectionNum int NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add TypeCode nvarchar(10) NULL;	
ALTER TABLE [InventProduct] DROP COLUMN [QuantityEdit];
ALTER TABLE [InventProduct] add [QuantityEdit] float NULL DEFAULT 0;"
			});

			//========================ver13
			 scripts.Add(new SQLScript
			{
				Ver = 13,
				Tab = 3,
				DBType = DBName.EmptyCount4UDB,
				Text = @"ALTER TABLE [IturAnalyzes] add SectionCode nvarchar(50) NULL;
ALTER TABLE [IturAnalyzes] add SectionName nvarchar(50) NULL;
 ALTER TABLE [IturAnalyzes] add ERPType int NULL DEFAULT 0;"
			});

			 scripts.Add(new SQLScript
			 {
				 Ver = 13,
				 Tab = 3,
				 DBType = DBName.Count4UDB,
				 Text = @"ALTER TABLE [IturAnalyzes] add SectionCode nvarchar(50) NULL;
ALTER TABLE [IturAnalyzes] add SectionName nvarchar(50) NULL;
ALTER TABLE [IturAnalyzes] add ERPType int NULL DEFAULT 0;"
			 });

			 scripts.Add(new SQLScript
			 {
				 Ver = 14,
				 Tab = 3,
				 DBType = DBName.MainDB,
				 Text = @"ALTER TABLE [Report] add [Tag] nvarchar(250) NULL;
ALTER TABLE [Report] add [Menu] bit NULL DEFAULT 0;
ALTER TABLE [Report] add [MenuCaption] nvarchar(100) NULL;"
			 });

			 scripts.Add(new SQLScript
			 {
				 Ver = 14,
				 Tab = 4,
				 DBType = DBName.AuditDB,
				 Text = @"
ALTER TABLE [AuditReport] add [Tag] nvarchar(250) NULL;
ALTER TABLE [AuditReport] add [Menu] bit NULL DEFAULT 0;
ALTER TABLE [AuditReport] add [MenuCaption] nvarchar(100) NULL;"
			 });
		   // ======= ver 15
			 scripts.Add(new SQLScript
			 {
				 Ver = 15,
				 Tab = 1,
				 DBType = DBName.Count4UDB,
				 Text = @"ALTER TABLE [Itur] add IturCode nvarchar(50) NULL;
ALTER TABLE [DocumentHeader] add DocNum int NULL DEFAULT 1;
ALTER TABLE [InventProduct] add IPNum int NULL DEFAULT 1;
ALTER TABLE [InventProduct] add TypeMakat nvarchar(10) NULL;
DROP TABLE [IturAnalyzes];"
			 });

			 scripts.Add(new SQLScript
			 {
				 Ver = 15,
				 Tab = 2,
				 DBType = DBName.Count4UDB,
				 Text = @"CREATE TABLE [IturAnalyzes] (
  [ID] bigint NOT NULL  IDENTITY (1,1)
, [Itur_Disabled] bit NULL
, [Itur_Publishe] bit NULL
, [Itur_StatusIturBit] int NULL DEFAULT 0
, [Itur_Number] int NOT NULL DEFAULT 0
, [Itur_NumberPrefix] nvarchar(50) NULL
, [Itur_NumberSufix] nvarchar(50) NULL
, [Itur_LocationCode] nvarchar(50) NULL
, [Itur_StatusIturGroupBit] int NULL DEFAULT 0
, [Itur_StatusDocHeaderBit] int NULL DEFAULT 0
, [Doc_Name] nvarchar(50) NULL
, [Doc_Approve] bit NULL
, [Doc_WorkerGUID] nvarchar(50) NULL
, [Doc_StatusDocHeaderBit] int NULL DEFAULT 0
, [Doc_StatusInventProductBit] int NULL DEFAULT 0
, [Doc_StatusApproveBit] int NULL DEFAULT 0
, [PDA_StatusInventProductBit] int NULL DEFAULT 0
, [Code] nvarchar(50) NULL
, [LocationCode] nvarchar(50) NULL
, [DocumentHeaderCode] nvarchar(50) NULL
, [BarcodeOriginal] nvarchar(100) NULL
, [MakatOriginal] nvarchar(100) NULL
, [PriceString] nvarchar(50) NULL
, [PriceBuy] float NULL DEFAULT 0
, [PriceSale] float NULL DEFAULT 0
, [PriceExtra] float NULL DEFAULT 0
, [DocumentCode] nvarchar(50) NULL
, [IturCode] nvarchar(50) NULL
, [SectionCode] nvarchar(50) NULL
, [SectionName] nvarchar(50) NULL
, [ERPType] int NULL DEFAULT 0
, [FromCatalogType] int NULL DEFAULT 0
, [TypeCode] nvarchar(10) NULL
, [SectionNum] int NULL DEFAULT 0
, [ValueBuyDifference] float NULL
, [QuantityDifference] float NULL
, [ValueBuyEdit] float NULL
, [QuantityEdit] float NULL
, [ValueBuyQriginal] float NULL
, [QuantityOriginal] float NULL
, [Barcode] nvarchar(50) NULL
, [Makat] nvarchar(50) NULL
, [ProductName] nvarchar(100) NULL
, [ModifyDate] datetime NULL DEFAULT GETDATE ( )
, [SerialNumber] nvarchar(50) NULL
, [ShelfCode] nvarchar(100) NULL
, [TypeMakat] nvarchar(10) NULL
, [InputTypeCode] nvarchar(100) NULL
);
ALTER TABLE [IturAnalyzes] ADD CONSTRAINT [PK__IturAnalyzes__00000000000002D0] PRIMARY KEY ([ID]);
CREATE UNIQUE INDEX [UQ__IturAnalyzes__0000000000000703] ON [IturAnalyzes] ([ID] ASC);
"
			 });


			 scripts.Add(new SQLScript
			 {
				 Ver = 15,
				 Tab = 3,
				 DBType = DBName.EmptyCount4UDB,
				 Text = @"ALTER TABLE [Itur] add IturCode nvarchar(50) NULL;
ALTER TABLE [DocumentHeader] add DocNum int NULL DEFAULT 1;
ALTER TABLE [InventProduct] add IPNum int NULL DEFAULT 1;
ALTER TABLE [InventProduct] add TypeMakat nvarchar(10) NULL;
DROP TABLE [IturAnalyzes];"
			 });

			 scripts.Add(new SQLScript
			 {
				 Ver = 15,
				 Tab = 4,
				 DBType = DBName.EmptyCount4UDB,
				 Text = @"CREATE TABLE [IturAnalyzes] (
  [ID] bigint NOT NULL  IDENTITY (1,1)
, [Itur_Disabled] bit NULL
, [Itur_Publishe] bit NULL
, [Itur_StatusIturBit] int NULL DEFAULT 0
, [Itur_Number] int NOT NULL DEFAULT 0
, [Itur_NumberPrefix] nvarchar(50) NULL
, [Itur_NumberSufix] nvarchar(50) NULL
, [Itur_LocationCode] nvarchar(50) NULL
, [Itur_StatusIturGroupBit] int NULL DEFAULT 0
, [Itur_StatusDocHeaderBit] int NULL DEFAULT 0
, [Doc_Name] nvarchar(50) NULL
, [Doc_Approve] bit NULL
, [Doc_WorkerGUID] nvarchar(50) NULL
, [Doc_StatusDocHeaderBit] int NULL DEFAULT 0
, [Doc_StatusInventProductBit] int NULL DEFAULT 0
, [Doc_StatusApproveBit] int NULL DEFAULT 0
, [PDA_StatusInventProductBit] int NULL DEFAULT 0
, [Code] nvarchar(50) NULL
, [LocationCode] nvarchar(50) NULL
, [DocumentHeaderCode] nvarchar(50) NULL
, [BarcodeOriginal] nvarchar(100) NULL
, [MakatOriginal] nvarchar(100) NULL
, [PriceString] nvarchar(50) NULL
, [PriceBuy] float NULL DEFAULT 0
, [PriceSale] float NULL DEFAULT 0
, [PriceExtra] float NULL DEFAULT 0
, [DocumentCode] nvarchar(50) NULL
, [IturCode] nvarchar(50) NULL
, [SectionCode] nvarchar(50) NULL
, [SectionName] nvarchar(50) NULL
, [ERPType] int NULL DEFAULT 0
, [FromCatalogType] int NULL DEFAULT 0
, [TypeCode] nvarchar(10) NULL
, [SectionNum] int NULL DEFAULT 0
, [ValueBuyDifference] float NULL
, [QuantityDifference] float NULL
, [ValueBuyEdit] float NULL
, [QuantityEdit] float NULL
, [ValueBuyQriginal] float NULL
, [QuantityOriginal] float NULL
, [Barcode] nvarchar(50) NULL
, [Makat] nvarchar(50) NULL
, [ProductName] nvarchar(100) NULL
, [ModifyDate] datetime NULL DEFAULT GETDATE ( )
, [SerialNumber] nvarchar(50) NULL
, [ShelfCode] nvarchar(100) NULL
, [TypeMakat] nvarchar(10) NULL
, [InputTypeCode] nvarchar(100) NULL
);
ALTER TABLE [IturAnalyzes] ADD CONSTRAINT [PK__IturAnalyzes__00000000000002D0] PRIMARY KEY ([ID]);
CREATE UNIQUE INDEX [UQ__IturAnalyzes__0000000000000703] ON [IturAnalyzes] ([ID] ASC);
"
			 });

			 scripts.Add(new SQLScript
			 {
				 Ver = 16,
				 Tab = 3,
				 DBType = DBName.Count4UDB,
				 Text = @"ALTER TABLE [IturAnalyzes] add IPNum int NULL DEFAULT 1;
ALTER TABLE [IturAnalyzes] add DocNum int NULL DEFAULT 1;"
			 });

			 scripts.Add(new SQLScript
			 {
				 Ver = 16,
				 Tab = 3,
				 DBType = DBName.EmptyCount4UDB,
				 Text = @"ALTER TABLE [IturAnalyzes] add IPNum int NULL DEFAULT 1;
ALTER TABLE [IturAnalyzes] add DocNum int NULL DEFAULT 1;"
			 });

			 scripts.Add(new SQLScript
			 {
				 Ver = 17,
				 Tab = 2,
				 DBType = DBName.MainDB,
				 Text = @"DELETE FROM  [Report];"
			 });
			 scripts.Add(new SQLScript
			 {
				 Ver = 17,
				 Tab = 3,
				 DBType = DBName.MainDB,
				 Text = @"INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption]) VALUES (N'Any',null,null,null,N'Itur\Doc\PDA',N'Comparative_Report.rdlc',null,N'From PDA',1,N'Corparative Report');
INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption]) VALUES (N'Any',N'Document Report',null,null,N'Itur\Doc\PDA',N'DOCUMENT_REPORT.rdlc',null,N'From PDA',1,N'Document Report');
INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption]) VALUES (N'Any',null,null,null,N'Itur\Doc\PDA',N'Detailed_Report_BranchXXX.rdlc',null,N'From PDA',1,N'Detailed Branch');
INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption]) VALUES (N'Any',N'Catalog1',null,null,N'Catalog',N'Catalog1.rdlc',null,N'Catalog',0,N'');
INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption]) VALUES (N'2b588315-4b14-4709-bac9-55f5e303721b',null,null,null,N'Catalog',N'Catalog2.rdlc',null,N'Catalog',0,null);
INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption]) VALUES (N'Any',null,null,null,N'Itur\Doc\PDA',N'Summary_Report_BranchXXX.rdlc',null,N'From PDA',1,N'Summary Report Branch ');
INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption]) VALUES (N'Any',null,null,null,N'Itur\Doc\PDA',N'Summary_Report_BranchXXX_With_Departments-Sections.rdlc',null,N'From PDA',1,N'Summary Report Branch - Section');
"
			 });

			 scripts.Add(new SQLScript
			 {
				 Ver = 17,
				 Tab = 4,
				 DBType = DBName.Count4UDB,
				 Text = @"DELETE FROM  [Itur];"
			 });

			 scripts.Add(new SQLScript
			 {
				 Ver = 17,
				 Tab = 5,
				 DBType = DBName.EmptyCount4UDB,
				 Text = @"DELETE FROM  [Itur];"
			 });

			 scripts.Add(new SQLScript
			 {
				 Ver = 17,
				 Tab = 6,
				 DBType = DBName.Count4UDB,
				 Text = @"DELETE FROM  [InventProduct];"
			 });

			 scripts.Add(new SQLScript
			 {
				 Ver = 17,
				 Tab = 7,
				 DBType = DBName.EmptyCount4UDB,
				 Text = @"DELETE FROM  [InventProduct];"
			 });

				 scripts.Add(new SQLScript
			 {
				 Ver = 17,
				 Tab = 8,
				 DBType = DBName.Count4UDB,
				 Text = @"DELETE FROM  [DocumentHeader];"
			 });

			 scripts.Add(new SQLScript
			 {
				 Ver = 17,
				 Tab = 9,
				 DBType = DBName.EmptyCount4UDB,
				 Text = @"DELETE FROM  [DocumentHeader];"
			 });

			 scripts.Add(new SQLScript
			 {
				 Ver = 18,
				 Tab = 2,
				 DBType = DBName.MainDB,
				 Text = @"DELETE FROM  [Report];"
			 });
			 scripts.Add(new SQLScript
			 {
				 Ver = 18,
				 Tab = 3,
				 DBType = DBName.MainDB,
				 Text = @"INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption]) VALUES (N'Any',N'Corporative Report for Iturs',NULL,NULL,N'Iturs',N'ItursCorporativeReport.rdlc','Iturs','CorporativeReport',1,N'Corporative Report - Iturs');
INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption]) VALUES (N'Any',N'Document Report for Itur',NULL,NULL,N'Iturs\Itur',N'IturDocumentReport.rdlc','ItursItur','DocumentReport',1,N'Document Report - Itur');
INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption]) VALUES (N'Any',N'Document Report for Iturs',NULL,NULL,N'Iturs',N'ItursDocumentReport.rdlc','Iturs','DocumentReport',1,N'Document Report - Iturs');
INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption]) VALUES (N'Any',N'Document Report for Doc',NULL,NULL,N'Iturs\Itur\Doc',N'DocDocumentReport.rdlc','ItursIturDoc','DocumentReport',1,N'Document Report - Doc');
INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption]) VALUES (N'Any',N'Detailed Report Branch for Iturs',NULL,NULL,N'Iturs',N'ItursDetailed_Report_BranchXXX.rdlc','Iturs','DetailedReportBranch',1,N'Detailed Report Branch - Iturs');
INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption]) VALUES (N'Any',N'Summary Report Branch for Iturs',NULL,NULL,N'Iturs',N'ItursSummary_Report_BranchXXX.rdlc','Iturs','SummaryReportBranch',1,N'Summary Report Branch - Iturs');
INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption]) VALUES (N'Any',N'Summary Report Branch with Departments - Sections for Iturs',NULL,NULL,N'Iturs',N'ItursSummary_Report_BranchXXX_With_Departments-Sections.rdlc','Iturs','SummaryReportBranch',1,N'Summary Report Branch with Sections - Iturs');"
			 });
			
			 scripts.Add(new SQLScript
			 {
				 Ver = 19, 
				 Tab = 1,
				 DBType = DBName.Count4UDB,
				 Text = @"ALTER TABLE [IturAnalyzes] add PDA_ID bigint NULL DEFAULT 1;"
			 });

			 scripts.Add(new SQLScript
			 {
				 Ver = 19,
				 Tab = 2,
				 DBType = DBName.EmptyCount4UDB,
				 Text = @"ALTER TABLE [IturAnalyzes] add PDA_ID bigint NULL DEFAULT 1;"
			 });

			scripts.Add(new SQLScript
			 {
				 Ver = 20,
				 Tab = 2,
				 DBType = DBName.MainDB,
				 Text = @"INSERT INTO [ImportAdapter] ([Code],[AdapterCode],[DomainType],[Description]) 
					VALUES (N'Any',N'ImportPriorityRenuarAdapter',N'ImportCatalog',null);"
			 });

			scripts.Add(new SQLScript
			{
				Ver = 21,
				Tab = 2,
				DBType = DBName.MainDB,
				Text = @"INSERT INTO [ImportAdapter] ([Code],[AdapterCode],[DomainType],[Description]) VALUES (N'Any',N'ExportHT630Adapter',N'ExportCatalogPDA',N'');
INSERT INTO [ImportAdapter] ([Code],[AdapterCode],[DomainType],[Description]) VALUES (N'Any',N'ExportErpComaxAdapter',N'ExportCatalogERP',N'');
INSERT INTO [ImportAdapter] ([Code],[AdapterCode],[DomainType],[Description]) VALUES (N'Any',N'ExportErpGazitAdapter',N'ExportCatalogERP',N'');
INSERT INTO [ImportAdapter] ([Code],[AdapterCode],[DomainType],[Description]) VALUES (N'Any',N'ExportErpUnizagAdapter',N'ExportCatalogERP',N'');
INSERT INTO [ImportAdapter] ([Code],[AdapterCode],[DomainType],[Description]) VALUES (N'Any',N'ExportErpPriorityRenuarAdapter',N'ExportCatalogERP',N'');"
			});

			scripts.Add(new SQLScript
			{
				Ver = 22,
				Tab = 1,
				DBType = DBName.Count4UDB,
				Text = @"ALTER TABLE [InventorConfig] add  [InventorCode] nvarchar(100) NULL;
ALTER TABLE [InventorConfig] add  [TypeObject] nvarchar(50) NULL;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 22,
				Tab = 2,
				DBType = DBName.EmptyCount4UDB,
				Text = @"ALTER TABLE [InventorConfig] add  [InventorCode] nvarchar(100) NULL;
ALTER TABLE [InventorConfig] add  [TypeObject] nvarchar(50) NULL;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 23,
				Tab = 2,
				DBType = DBName.MainDB,
				Text = @"ALTER TABLE [Report] add [Print] bit NULL DEFAULT 0;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 23,
				Tab = 3,
				DBType = DBName.MainDB,
				Text = @"ALTER TABLE [Customer] add [Print] bit NULL DEFAULT 0;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 23,
				Tab =4,
				DBType = DBName.AuditDB,
				Text = @"ALTER TABLE [AuditReport] add [Print] bit NULL DEFAULT 0;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 23,
				Tab = 5,
				DBType = DBName.Count4UDB,
				Text = @"ALTER TABLE [Itur] add ERPIturCode nvarchar(50) NULL;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 23,
				Tab = 6,
				DBType = DBName.EmptyCount4UDB,
				Text = @"ALTER TABLE [Itur] add ERPIturCode nvarchar(50) NULL;"
			});

			//================25

			scripts.Add(new SQLScript
			{
				Ver = 25,
				Tab = 1,
				DBType = DBName.MainDB,
				Text = @"ALTER TABLE [Branch] add [ModifyDate] datetime NULL DEFAULT GETDATE ( );
ALTER TABLE [Branch] add [CreateDate] datetime NULL DEFAULT GETDATE ( )	;
ALTER TABLE [Branch] add [ReportContext] nvarchar(100) NULL;
ALTER TABLE [Branch] add [ReportDS] nvarchar(100) NULL;
ALTER TABLE [Branch] add [ReportName] nvarchar(100) NULL;
ALTER TABLE [Customer] add [ModifyDate] datetime NULL DEFAULT GETDATE ( );
ALTER TABLE [Customer] add [CreateDate] datetime NULL DEFAULT GETDATE ( ) ;
ALTER TABLE [Customer] add [ReportContext] nvarchar(100) NULL;
ALTER TABLE [Customer] add [ReportDS] nvarchar(100) NULL;
ALTER TABLE [Customer] add [ReportName] nvarchar(100) NULL;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 25,
				Tab = 2,
				DBType = DBName.Count4UDB,
				Text = @"ALTER TABLE [Supplier] DROP COLUMN Code;
ALTER TABLE [Session] DROP COLUMN Code;
ALTER TABLE [UnitType] DROP COLUMN Code;
ALTER TABLE [Section] DROP COLUMN Code;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 25,
				Tab = 3,
				DBType = DBName.EmptyCount4UDB,
				Text = @"ALTER TABLE [Supplier] DROP COLUMN Code;
ALTER TABLE [Session] DROP COLUMN Code;
ALTER TABLE [UnitType] DROP COLUMN Code;
ALTER TABLE [Section] DROP COLUMN Code;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 25,
				Tab = 4,
				DBType = DBName.Count4UDB,
				Text = @"ALTER TABLE [Supplier] add SupplierCode nvarchar(50) NULL;
ALTER TABLE [Session] add SessionCode nvarchar(50) NULL;
ALTER TABLE [UnitType] add UnitTypeCode nvarchar(50) NULL;
ALTER TABLE [Section] add SectionCode nvarchar(50) NULL;"
			});
		
			scripts.Add(new SQLScript
			{
				Ver = 25,
				Tab = 5,
				DBType = DBName.EmptyCount4UDB,
				Text = @"ALTER TABLE [Supplier] add SupplierCode nvarchar(50) NULL;
ALTER TABLE [Session] add SessionCode nvarchar(50) NULL;
ALTER TABLE [UnitType] add UnitTypeCode nvarchar(50) NULL;
ALTER TABLE [Section] add SectionCode nvarchar(50) NULL;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 26,
				Tab = 3,
				DBType = DBName.MainDB,
				Text = @"ALTER TABLE [Customer] add [ImportSectionAdapterCode] nvarchar(100) NULL;
ALTER TABLE [Customer] add [ExportSectionAdapterCode] nvarchar(100) NULL;
ALTER TABLE [Branch] add [ImportSectionAdapterCode] nvarchar(100) NULL;
ALTER TABLE [Branch] add [ExportSectionAdapterCode] nvarchar(100) NULL;"

			});

			scripts.Add(new SQLScript
			{
				Ver = 26,
				Tab = 4,
				DBType = DBName.AuditDB,
				Text = @"ALTER TABLE [Inventor] add [ImportSectionAdapterCode] nvarchar(100) NULL;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 27,
				Tab = 1,
				DBType = DBName.Count4UDB,
				Text = @"ALTER TABLE [IturAnalyzes] add [Count] int NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [ValueChar] nvarchar(100) NULL;
ALTER TABLE [IturAnalyzes] add [ValueInt] int NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [ValueFloat] float NULL DEFAULT 0;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 27,
				Tab = 1,
				DBType = DBName.EmptyCount4UDB,
				Text = @"ALTER TABLE [IturAnalyzes] add [Count] int NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [ValueChar] nvarchar(100) NULL;
ALTER TABLE [IturAnalyzes] add [ValueInt] int NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [ValueFloat] float NULL DEFAULT 0;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 28,
				Tab = 1,
				DBType = DBName.Count4UDB,
				Text = @"ALTER TABLE [IturAnalyzes] add  [IsResulte] bit NULL;
ALTER TABLE [IturAnalyzes] add  [ResultCode] nvarchar(50) NULL;
ALTER TABLE [IturAnalyzes] add  [ResulteDescription] nvarchar(100) NULL;
ALTER TABLE [IturAnalyzes] add  [IsUpdateERP] bit NULL;
ALTER TABLE [IturAnalyzes] add  [ImputTypeCodeFromPDA] nvarchar(50) NULL;
ALTER TABLE [IturAnalyzes] add  [ResulteValue] nvarchar(50) NULL;
ALTER TABLE [InventProduct] add [ImputTypeCodeFromPDA] nvarchar(50) NULL;
ALTER TABLE [Product] add  [IsUpdateERP] bit NULL;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 28,
				Tab = 1,
				DBType = DBName.EmptyCount4UDB,
				Text = @"ALTER TABLE [IturAnalyzes] add  [IsResulte] bit NULL;
ALTER TABLE [IturAnalyzes] add  [ResultCode] nvarchar(50) NULL;
ALTER TABLE [IturAnalyzes] add  [ResulteDescription] nvarchar(100) NULL;
ALTER TABLE [IturAnalyzes] add  [IsUpdateERP] bit NULL;
ALTER TABLE [IturAnalyzes] add  [ImputTypeCodeFromPDA] nvarchar(50) NULL;
ALTER TABLE [IturAnalyzes] add  [ResulteValue] nvarchar(50) NULL;
ALTER TABLE [InventProduct] add [ImputTypeCodeFromPDA] nvarchar(50) NULL;
ALTER TABLE [Product] add  [IsUpdateERP] bit NULL;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 29,
				Tab = 1,
				DBType = DBName.Count4UDB,
				Text = @"ALTER TABLE [Session] add   [CountItem] int NULL DEFAULT 0;
ALTER TABLE [Session] add   [CountDocument] int NULL DEFAULT 0;
ALTER TABLE [Session] add   [CountItur] int NULL DEFAULT 0;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 29,
				Tab = 2,
				DBType = DBName.EmptyCount4UDB,
				Text = @"ALTER TABLE [Session] add   [CountItem] int NULL DEFAULT 0;
ALTER TABLE [Session] add   [CountDocument] int NULL DEFAULT 0;
ALTER TABLE [Session] add   [CountItur] int NULL DEFAULT 0;"
			});


			scripts.Add(new SQLScript
			{
				Ver = 30,
				Tab = 1,
				DBType = DBName.MainDB,
				Text = @"DELETE FROM  [Report];"
			});

			scripts.Add(new SQLScript
			{
				Ver = 31,
				Tab = 2,
				DBType = DBName.MainDB,
				Text = @"INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption],[Print]) VALUES (N'Any',N'Corporative Report - Sum - Iturs',null,null,N'Iturs\AddIn\Sum',N'Iturs_CorporativeReport.rdlc',N'Iturs',N'Iturs Sum',1,N'Corporative Report - ERP - Iturs',0);
INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption],[Print]) VALUES (N'Any',N'Document Report for Itur',null,null,N'Iturs\Itur',N'IturDocumentReport.rdlc',N'ItursItur',N'DocumentReport',1,N'Document Report - Itur',0);
INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption],[Print]) VALUES (N'Any',N'Document Report for Iturs',null,null,N'Iturs',N'ItursDocumentReport.rdlc',N'Iturs',N'DocumentReport',1,N'Document Report - Iturs',0);
INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption],[Print]) VALUES (N'Any',N'Summary Report Branch for Iturs ',null,null,N'Iturs\AddIn\Sum',N'Iturs_Summary_Report_BranchXXX.rdlc',N'Iturs',N'SummaryReportBranch',1,N'Summary Branch ERP - Iturs',0);
INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption],[Print]) VALUES (N'Any',N'Detailed Report Branch - Sum - Iturs',null,null,N'Iturs\AddIn\Sum',N'Iturs_Detailed_Report_BranchXXX.rdlc',N'Iturs',N'ItursSum',1,N'Detailed Report Branch - ERP - Iturs',0);
INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption],[Print]) VALUES (N'Any',N'Summary_Branch_With_Departments-Sections ERP - Iturs',null,null,N'Iturs\AddIn\Sum',N'Iturs_Summary_Report_BranchXXX_With_Departments-Sections.rdlc',N'Iturs',N'SummaryReportBranch',1,N'Summary Branch Sections ERP - Iturs',0);
INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption],[Print]) VALUES (N'Any',N'',null,null,N'Iturs',N'InventProductResult.rdlc',N'Iturs',N'',1,N'Invent Product Result',0);
INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption],[Print]) VALUES (N'Any',N'Catalog',null,null,N'Catalog',N'Catalog1.rdlc',N'Catalog',N'Catalog',1,N'Catalog',0);
INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption],[Print]) VALUES (N'Any',N'Detailed - InventProduct',null,null,N'Iturs\Itur\Doc\PDA',N'Detailed.rdlc',N'ItursIturDocPDA',N'InventProduct',1,N'Detailed - InventProduct',0);
INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption],[Print]) VALUES (N'Any',N'BranchDetaile',null,null,N'Branch',N'BranchDetaile.rdlc',N'Branch',N'Branch',1,N'BranchDetaile',0);
INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption],[Print]) VALUES (N'Any',N'CustomerDetaile',null,null,N'Customer',N'CustomerDetaile.rdlc',N'Customer',N'Customer',1,N'CustomerDetaile',0);
INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption],[Print]) VALUES (N'Any',N'InventorDetaile',null,null,N'Inventor',N'InventorDetaile.rdlc',N'Inventor',N'Inventor',1,N'InventorDetaile',0);
INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption],[Print]) VALUES (N'Any',N'AuditConfigDetaile',null,null,N'AuditConfig',N'AuditConfigDetaile.rdlc',N'AuditConfig',N'AuditConfig',1,N'AuditConfigDetaile',0);
INSERT INTO [Report] ([Code],[Description],[DomainContext],[TypeDS],[Path],[FileName],[DomainType],[Tag],[Menu],[MenuCaption],[Print]) VALUES (N'Any',N'Document Report',null,null,N'Iturs\Itur\Doc\AddIn\Simple',N'Document_Report_XXX_YYY_ZZZ.rdlc',N'ItursIturDoc',N'DocumentReport',1,N'Document Report',1);"
			});

			scripts.Add(new SQLScript
			{
				Ver = 32,
				Tab = 1,
				DBType = DBName.EmptyCount4UDB,
				Text = @"ALTER TABLE [IturAnalyzes] add [QuantityOriginalERP] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [ValueOriginalERP] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [QuantityDifferenceOriginalERP] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [ValueDifferenceOriginalERP] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [SupplierCode] nvarchar(50) NULL;
ALTER TABLE [IturAnalyzes] add [SupplierName] nvarchar(100) NULL;

ALTER TABLE [InventProduct] add [DocNum] bigint NULL DEFAULT 1;
ALTER TABLE [InventProduct] add [SessionCode] nvarchar(50) NULL;
ALTER TABLE [InventProduct] add [SessionNum] bigint NULL DEFAULT 1;
ALTER TABLE [InventProduct] add [SectionCode] nvarchar(50) NULL;
ALTER TABLE [InventProduct] add [SectionName] nvarchar(100) NULL;
ALTER TABLE [InventProduct] add [PriceBuy] float NULL DEFAULT 0;
ALTER TABLE [InventProduct] add [PriceSale] float NULL DEFAULT 0;"
});

		

			scripts.Add(new SQLScript
			{
				Ver = 32,
				Tab = 3,
				DBType = DBName.MainDB,
				Text = @"ALTER TABLE [Report] add [NN] int NULL DEFAULT 1;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 32,
				Tab = 4,
				DBType = DBName.AuditDB,
				Text = @"ALTER TABLE [AuditReport] add [NN] int NULL DEFAULT 1;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 33,
				Tab = 2,
				DBType = DBName.Count4UDB,
				Text = @"ALTER TABLE [IturAnalyzes] add [QuantityOriginalERP] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [ValueOriginalERP] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [QuantityDifferenceOriginalERP] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [ValueDifferenceOriginalERP] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [SupplierCode] nvarchar(50) NULL;
ALTER TABLE [IturAnalyzes] add [SupplierName] nvarchar(100) NULL;

ALTER TABLE [InventProduct] add [DocNum] bigint NULL DEFAULT 1;
ALTER TABLE [InventProduct] add [SessionCode] nvarchar(50) NULL;
ALTER TABLE [InventProduct] add [SessionNum] bigint NULL DEFAULT 1;
ALTER TABLE [InventProduct] add [SectionCode] nvarchar(50) NULL;
ALTER TABLE [InventProduct] add [SectionName] nvarchar(100) NULL;
ALTER TABLE [InventProduct] add [PriceBuy] float NULL DEFAULT 0;
ALTER TABLE [InventProduct] add [PriceSale] float NULL DEFAULT 0;"
			});


			scripts.Add(new SQLScript
			{
				Ver = 34,
				Tab = 3,
				DBType = DBName.MainDB,
				Text = @"ALTER TABLE [Report] add [MenuCaptionLocalizationCode] nvarchar(100) NULL;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 34,
				Tab = 4,
				DBType = DBName.AuditDB,
				Text = @"ALTER TABLE [AuditReport] add [MenuCaptionLocalizationCode] nvarchar(100) NULL;"
			});
			//  Ver = 35 test only


			scripts.Add(new SQLScript
			{
				Ver = 36,
				Tab = 1,
				DBType = DBName.Count4UDB,
				Text = @"ALTER TABLE [IturAnalyzes] add  [LocationName] nvarchar(100) NULL;
ALTER TABLE [IturAnalyzes] add  [IturName] nvarchar(100) NULL;
ALTER TABLE [IturAnalyzes] add  [SessionCode] nvarchar(50) NULL;
ALTER TABLE [IturAnalyzes] add  [SessionNum] int NULL DEFAULT 1;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 36,
				Tab = 2,
				DBType = DBName.EmptyCount4UDB,
				Text = @"ALTER TABLE [IturAnalyzes] add  [LocationName] nvarchar(100) NULL;
ALTER TABLE [IturAnalyzes] add  [IturName] nvarchar(100) NULL;
ALTER TABLE [IturAnalyzes] add  [SessionCode] nvarchar(50) NULL;
ALTER TABLE [IturAnalyzes] add  [SessionNum] int NULL DEFAULT 1;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 37,
				Tab = 3,
				DBType = DBName.MainDB,
				Text = @"ALTER TABLE [Customer] add [UpdateCatalogAdapterCode] nvarchar(100) NULL;
ALTER TABLE [Branch] add [UpdateCatalogAdapterCode] nvarchar(100) NULL;"
			});

				 scripts.Add(new SQLScript
			{
				Ver = 37,
				Tab = 4,
				DBType = DBName.AuditDB,
				Text = @"ALTER TABLE [Inventor] add [UpdateCatalogAdapterCode] nvarchar(100) NULL;"
			});

				 scripts.Add(new SQLScript
				 {
					 Ver = 38,
					 Tab = 4,
					 DBType = DBName.AuditDB,
					 Text = @"ALTER TABLE [Inventor] add [ImportPDAProviderCode] nvarchar(100) NULL;"
				 });

			//=============	 39 
				 scripts.Add(new SQLScript
				 {
					 Ver = 39,
					 Tab = 1,
					 DBType = DBName.AuditDB,
					 Text = @"ALTER TABLE [Inventor] add [Restore] nvarchar(100) NULL;
ALTER TABLE [Inventor] add [RestoreBit] bit NULL DEFAULT 0;"
				 });

				 scripts.Add(new SQLScript
				 {
					 Ver = 39,
					 Tab = 2,
					 DBType = DBName.MainDB,
					 Text = @"ALTER TABLE [Customer] add [ImportBranchAdapterCode] nvarchar(100) NULL;
ALTER TABLE [Customer] add [ExportBranchAdapterCode] nvarchar(100) NULL;
ALTER TABLE [Customer] add [Restore] nvarchar(100) NULL;
ALTER TABLE [Customer] add [RestoreBit] bit NULL DEFAULT 0;"
				 });

				 scripts.Add(new SQLScript
				 {
					 Ver = 39,
					 Tab = 3,
					 DBType = DBName.MainDB,
					 Text = @"ALTER TABLE [Branch] add [Restore] nvarchar(100) NULL;
ALTER TABLE [Branch] add [RestoreBit] bit NULL DEFAULT 0;"
				 });

			//------
				 scripts.Add(new SQLScript
				 {
					 Ver = 40,
					 Tab = 1,
					 DBType = DBName.Count4UDB,
					 Text = @"ALTER TABLE [Itur] add [Restore] nvarchar(100) NULL;
ALTER TABLE [Itur] add [RestoreBit] bit NULL DEFAULT 0;
ALTER TABLE [Location] add [Restore] nvarchar(100) NULL;
ALTER TABLE [Location] add [RestoreBit] bit NULL DEFAULT 0;
ALTER TABLE [DocumentHeader] add [Restore] nvarchar(100) NULL;
ALTER TABLE [DocumentHeader] add [RestoreBit] bit NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [Currency] nvarchar(10) NULL;"
				 });

				 scripts.Add(new SQLScript
				 {
					 Ver = 40,
					 Tab = 2,
					 DBType = DBName.EmptyCount4UDB,
					 Text = @"ALTER TABLE [Itur] add [Restore] nvarchar(100) NULL;
ALTER TABLE [Itur] add [RestoreBit] bit NULL DEFAULT 0;
ALTER TABLE [Location] add [Restore] nvarchar(100) NULL;
ALTER TABLE [Location] add [RestoreBit] bit NULL DEFAULT 0;
ALTER TABLE [DocumentHeader] add [Restore] nvarchar(100) NULL;
ALTER TABLE [DocumentHeader] add [RestoreBit] bit NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [Currency] nvarchar(10) NULL;"
				 });

					 scripts.Add(new SQLScript
				 {
					 Ver = 40,
					 Tab = 3,
					 DBType = DBName.Count4UDB,
					 Text = @"CREATE TABLE [ResulteValue] (
  [ID] bigint NOT NULL  IDENTITY (1,1)
, [Code] nvarchar(100) NULL
, [ValueTypeCode] nvarchar(100) NULL
, [ColGroupCode] nvarchar(100) NULL
, [RowGroupCode] nvarchar(100) NULL
, [ColIndex] int NULL
, [RowIndex] int NULL
, [ColCode] nvarchar(100) NULL
, [RowCode] nvarchar(100) NULL
, [Value] nvarchar(100) NULL
, [Name] nvarchar(100) NULL
, [ValueInt] int NULL DEFAULT 0
, [ValueStr] nvarchar(100) NULL
, [ValueFloat] float NULL DEFAULT 0
, [ValueBit] bit NULL DEFAULT 0
);
ALTER TABLE [ResulteValue] ADD CONSTRAINT [PK_ResulteValue] PRIMARY KEY ([ID]);
CREATE UNIQUE INDEX [UQ__ResulteValue__0000000000000E0C] ON [ResulteValue] ([ID] ASC);"
   });

					 scripts.Add(new SQLScript
					 {
						 Ver = 40,
						 Tab = 4,
						 DBType = DBName.EmptyCount4UDB,
						 Text = @"CREATE TABLE [ResulteValue] (
  [ID] bigint NOT NULL  IDENTITY (1,1)
, [Code] nvarchar(100) NULL
, [ValueTypeCode] nvarchar(100) NULL
, [ColGroupCode] nvarchar(100) NULL
, [RowGroupCode] nvarchar(100) NULL
, [ColIndex] int NULL
, [RowIndex] int NULL
, [ColCode] nvarchar(100) NULL
, [RowCode] nvarchar(100) NULL
, [Value] nvarchar(100) NULL
, [Name] nvarchar(100) NULL
, [ValueInt] int NULL DEFAULT 0
, [ValueStr] nvarchar(100) NULL
, [ValueFloat] float NULL DEFAULT 0
, [ValueBit] bit NULL DEFAULT 0
);
ALTER TABLE [ResulteValue] ADD CONSTRAINT [PK_ResulteValue] PRIMARY KEY ([ID]);
CREATE UNIQUE INDEX [UQ__ResulteValue__0000000000000E0C] ON [ResulteValue] ([ID] ASC);"
					 });

					 // ver 41 RefillAllCBIInventorConfigs

					 scripts.Add(new SQLScript
					 {
						 Ver = 42,
						 Tab = 1,
						 DBType = DBName.Count4UDB,
						 Text = @"ALTER TABLE [IturAnalyzes] add  [ERPIturCode] nvarchar(50) NULL;"
					 });

					 scripts.Add(new SQLScript
					 {
						 Ver = 42,
						 Tab = 2,
						 DBType = DBName.EmptyCount4UDB,
						 Text = @"ALTER TABLE [IturAnalyzes] add  [ERPIturCode] nvarchar(50) NULL;"
					 });

					 scripts.Add(new SQLScript
					 {
						 Ver = 43,
						 Tab = 1,
						 DBType = DBName.Count4UDB,
						 Text = @"ALTER TABLE [IturAnalyzes] DROP COLUMN [ValueBuyQriginal];
ALTER TABLE [IturAnalyzes] add [ValueBuyQriginal] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] DROP COLUMN [ValueBuyEdit];
ALTER TABLE [IturAnalyzes] add  [ValueBuyEdit] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] DROP COLUMN [ValueBuyDifference];
ALTER TABLE [IturAnalyzes] add [ValueBuyDifference] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] DROP COLUMN [QuantityOriginal];
ALTER TABLE [IturAnalyzes] add [QuantityOriginal] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] DROP COLUMN [QuantityEdit];
ALTER TABLE [IturAnalyzes] add [QuantityEdit] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] DROP COLUMN [QuantityDifference];
ALTER TABLE [IturAnalyzes] add [QuantityDifference] float NULL DEFAULT 0;"
					 });

					 scripts.Add(new SQLScript
					 {
						 Ver = 43,
						 Tab = 2,
						 DBType = DBName.EmptyCount4UDB,
						 Text = @"ALTER TABLE [IturAnalyzes] DROP COLUMN [ValueBuyQriginal];
ALTER TABLE [IturAnalyzes] add [ValueBuyQriginal] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] DROP COLUMN [ValueBuyEdit];
ALTER TABLE [IturAnalyzes] add  [ValueBuyEdit] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] DROP COLUMN [ValueBuyDifference];
ALTER TABLE [IturAnalyzes] add [ValueBuyDifference] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] DROP COLUMN [QuantityOriginal];
ALTER TABLE [IturAnalyzes] add [QuantityOriginal] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] DROP COLUMN [QuantityEdit];
ALTER TABLE [IturAnalyzes] add [QuantityEdit] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] DROP COLUMN [QuantityDifference];
ALTER TABLE [IturAnalyzes] add [QuantityDifference] float NULL DEFAULT 0;"
					 });

					 scripts.Add(new SQLScript
					 {
						 Ver = 44,
						 Tab = 1,
						 DBType = DBName.MainDB,
						 Text = @"ALTER TABLE [Report] add  [IturAdvancedSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [Report] add  [InventProductAdvancedSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [Report] add  [InventProductSumAdvancedSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [Report] add  [CustomerSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [Report] add  [BranchSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [Report] add  [InventorSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [Report] add  [AuditConfigSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [Report] add  [IturSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [Report] add  [InventProductSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [Report] add  [LocationSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [Report] add  [ProductSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [Report] add  [CodeReport] nvarchar(50) NULL;"
					 });

					 scripts.Add(new SQLScript
					 {
						 Ver = 44,
						 Tab = 2,
						 DBType = DBName.AuditDB,
						 Text = @"ALTER TABLE [AuditReport] add  [IturAdvancedSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [AuditReport] add  [InventProductAdvancedSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [AuditReport] add  [InventProductSumAdvancedSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [AuditReport] add  [CustomerSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [AuditReport] add  [BranchSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [AuditReport] add  [InventorSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [AuditReport] add  [AuditConfigSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [AuditReport] add  [IturSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [AuditReport] add  [InventProductSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [AuditReport] add  [LocationSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [AuditReport] add  [ProductSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [AuditReport] add  [CodeReport] nvarchar(50) NULL;"
					 });

				   //============== ver 45
					 scripts.Add(new SQLScript
					 {
						 Ver = 45,
						 Tab = 1,
						 DBType = DBName.AuditDB,
						 Text = @"ALTER TABLE [Inventor] add [CompleteDate] datetime NULL DEFAULT GETDATE ( );
ALTER TABLE [Inventor] add [Manager] nvarchar(100) NULL; "
					 });

					 scripts.Add(new SQLScript
					 {
						 Ver = 45,
						 Tab = 2,
						 DBType = DBName.Count4UDB,
						 Text = @"ALTER TABLE [Product] DROP COLUMN [CountInParentPack]; 
ALTER TABLE [Product] DROP COLUMN [BalanceQuantityPartialERP];" 
//ALTER TABLE [InventProduct] DROP COLUMN [QuantityInPackEdit];"
					 });

					 scripts.Add(new SQLScript
					 {
						 Ver = 45,
						 Tab = 3,
						 DBType = DBName.EmptyCount4UDB,
						 Text = @"ALTER TABLE [Product] DROP COLUMN [CountInParentPack]; 
ALTER TABLE [Product] DROP COLUMN [BalanceQuantityPartialERP];" 
//ALTER TABLE [InventProduct] DROP COLUMN [QuantityInPackEdit];"
					 });


					 scripts.Add(new SQLScript
					 {
						 Ver = 45,
						 Tab = 12,
						 DBType = DBName.Count4UDB,
						 Text = @"ALTER TABLE [Product] add [CountInParentPack] int NULL DEFAULT 1;
ALTER TABLE [Product] add [BalanceQuantityPartialERP] int NULL DEFAULT 0;
ALTER TABLE [InventProduct] add [QuantityInPackEdit] int NULL DEFAULT 0;
ALTER TABLE [InventProduct] add [IPValueStr1] nvarchar(100) NULL;
ALTER TABLE [InventProduct] add [IPValueStr2] nvarchar(100) NULL;
ALTER TABLE [InventProduct] add [IPValueStr3] nvarchar(100) NULL;
ALTER TABLE [InventProduct] add [IPValueStr4] nvarchar(100) NULL;
ALTER TABLE [InventProduct] add [IPValueStr5] nvarchar(100) NULL;
ALTER TABLE [InventProduct] add [IPValueFloat1] float NULL DEFAULT 0;
ALTER TABLE [InventProduct] add [IPValueFloat2] float NULL DEFAULT 0;
ALTER TABLE [InventProduct] add [IPValueFloat3] float NULL DEFAULT 0;
ALTER TABLE [InventProduct] add [IPValueFloat4] float NULL DEFAULT 0;
ALTER TABLE [InventProduct] add [IPValueFloat5] float NULL DEFAULT 0;
ALTER TABLE [InventProduct] add [IPValueInt1] int NULL DEFAULT 0;
ALTER TABLE [InventProduct] add [IPValueInt2] int NULL DEFAULT 0;
ALTER TABLE [InventProduct] add [IPValueInt3] int NULL DEFAULT 0;
ALTER TABLE [InventProduct] add [IPValueBit1] bit NULL DEFAULT 0;
ALTER TABLE [InventProduct] add [IPValueBit2] bit NULL DEFAULT 0; "
					 });

					 scripts.Add(new SQLScript
					 {
						 Ver = 45,
						 Tab = 13,
						 DBType = DBName.EmptyCount4UDB,
						 Text = @"ALTER TABLE [Product] add [CountInParentPack] int NULL DEFAULT 1;
ALTER TABLE [Product] add [BalanceQuantityPartialERP] int NULL DEFAULT 0;
ALTER TABLE [InventProduct] add [QuantityInPackEdit] int NULL DEFAULT 0;
ALTER TABLE [InventProduct] add [IPValueStr1] nvarchar(100) NULL;
ALTER TABLE [InventProduct] add [IPValueStr2] nvarchar(100) NULL;
ALTER TABLE [InventProduct] add [IPValueStr3] nvarchar(100) NULL;
ALTER TABLE [InventProduct] add [IPValueStr4] nvarchar(100) NULL;
ALTER TABLE [InventProduct] add [IPValueStr5] nvarchar(100) NULL;
ALTER TABLE [InventProduct] add [IPValueFloat1] float NULL DEFAULT 0;
ALTER TABLE [InventProduct] add [IPValueFloat2] float NULL DEFAULT 0;
ALTER TABLE [InventProduct] add [IPValueFloat3] float NULL DEFAULT 0;
ALTER TABLE [InventProduct] add [IPValueFloat4] float NULL DEFAULT 0;
ALTER TABLE [InventProduct] add [IPValueFloat5] float NULL DEFAULT 0;
ALTER TABLE [InventProduct] add [IPValueInt1] int NULL DEFAULT 0;
ALTER TABLE [InventProduct] add [IPValueInt2] int NULL DEFAULT 0;
ALTER TABLE [InventProduct] add [IPValueInt3] int NULL DEFAULT 0;
ALTER TABLE [InventProduct] add [IPValueBit1] bit NULL DEFAULT 0;
ALTER TABLE [InventProduct] add [IPValueBit2] bit NULL DEFAULT 0; "
					 });
					  //
					 scripts.Add(new SQLScript
					 {
						 Ver = 45,
						 Tab = 14,
						 DBType = DBName.Count4UDB,
						 Text = @"CREATE TABLE [FieldLink] (
  [ID] bigint NOT NULL  IDENTITY (1,1)
, [DomainType] nvarchar(50) NULL
, [TableName] nvarchar(50) NULL
, [PropertyNameInDomainType] nvarchar(50) NULL
, [FieldNameInTable] nvarchar(50) NULL
, [NumStringInRecord] int NULL DEFAULT 0
, [Editor] nvarchar(50) NULL
, [Validator] nvarchar(100) NULL
, [CodeLocalizationEditorLable] nvarchar(50) NULL
, [DefaultEditorLable] nvarchar(50) NULL
, [NN] int NULL DEFAULT 1
, [InGrid] bit NULL DEFAULT 0
, [InEdit] bit NULL DEFAULT 0
, [InAdd] bit NULL DEFAULT 0
);
ALTER TABLE [FieldLink] ADD CONSTRAINT [PK_FieldLink] PRIMARY KEY ([ID]);
CREATE UNIQUE INDEX [UQ__FieldLink__0000000000000F0A] ON [FieldLink] ([ID] ASC);"
					 });

					 scripts.Add(new SQLScript
					 {
						 Ver = 45,
						 Tab = 15,
						 DBType = DBName.EmptyCount4UDB,
						 Text = @"CREATE TABLE [FieldLink] (
  [ID] bigint NOT NULL  IDENTITY (1,1)
, [DomainType] nvarchar(50) NULL
, [TableName] nvarchar(50) NULL
, [PropertyNameInDomainType] nvarchar(50) NULL
, [FieldNameInTable] nvarchar(50) NULL
, [NumStringInRecord] int NULL DEFAULT 0
, [Editor] nvarchar(50) NULL
, [Validator] nvarchar(100) NULL
, [CodeLocalizationEditorLable] nvarchar(50) NULL
, [DefaultEditorLable] nvarchar(50) NULL
, [NN] int NULL DEFAULT 1
, [InGrid] bit NULL DEFAULT 0
, [InEdit] bit NULL DEFAULT 0
, [InAdd] bit NULL DEFAULT 0
);
ALTER TABLE [FieldLink] ADD CONSTRAINT [PK_FieldLink] PRIMARY KEY ([ID]);
CREATE UNIQUE INDEX [UQ__FieldLink__0000000000000F0A] ON [FieldLink] ([ID] ASC);"
					 });

	
			//=========46
					 scripts.Add(new SQLScript
							 {
								 Ver = 46,
								 Tab = 1,
								 DBType = DBName.AuditDB,
								 Text = @"ALTER TABLE [Inventor] add [ExportERPAdapterCode] nvarchar(100) DEFAULT ''; "
							 });

					 scripts.Add(new SQLScript
					 {
						 Ver = 46,
						 Tab = 2,
						 DBType = DBName.MainDB,
						 Text = @"ALTER TABLE [Customer] add [ExportERPAdapterCode] nvarchar(100) DEFAULT '';
ALTER TABLE [Branch] add [ExportERPAdapterCode] nvarchar(100) DEFAULT '';"
					 });
					  
			scripts.Add(new SQLScript
					 {
						 Ver = 46,
						 Tab = 3,
						 DBType = DBName.Count4UDB,
						 Text = @"ALTER TABLE [InventProduct] DROP COLUMN [IPValueStr1];
ALTER TABLE [InventProduct] DROP COLUMN [IPValueStr2];
ALTER TABLE [InventProduct] DROP COLUMN [IPValueStr3];
ALTER TABLE [InventProduct] DROP COLUMN [IPValueStr4];
ALTER TABLE [InventProduct] DROP COLUMN [IPValueStr5];"
					 });

				scripts.Add(new SQLScript
					 {
						 Ver = 46,
						 Tab = 4,
						 DBType = DBName.EmptyCount4UDB,
						 Text = @"ALTER TABLE [InventProduct] DROP COLUMN [IPValueStr1];
ALTER TABLE [InventProduct] DROP COLUMN [IPValueStr2];
ALTER TABLE [InventProduct] DROP COLUMN [IPValueStr3];
ALTER TABLE [InventProduct] DROP COLUMN [IPValueStr4];
ALTER TABLE [InventProduct] DROP COLUMN [IPValueStr5];"
						   });

				scripts.Add(new SQLScript
					 {
						 Ver = 46,
						 Tab = 5,
						 DBType = DBName.Count4UDB,
						 Text = @"ALTER TABLE [InventProduct] add  [IPValueStr1] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [InventProduct] add  [IPValueStr2] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [InventProduct] add  [IPValueStr3] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [InventProduct] add  [IPValueStr4] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [InventProduct] add  [IPValueStr5] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [InventProduct] add  [IPValueStr6] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [InventProduct] add  [IPValueStr7] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [InventProduct] add  [IPValueStr8] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [InventProduct] add  [IPValueStr9] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [InventProduct] add  [IPValueStr10] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [InventProduct] add  [IPValueInt4] int NULL DEFAULT 0;
ALTER TABLE [InventProduct] add  [IPValueInt5] int NULL DEFAULT 0;
ALTER TABLE [InventProduct] add  [IPValueBit3] bit NULL DEFAULT 0;
ALTER TABLE [InventProduct] add  [IPValueBit4] bit NULL DEFAULT 0;
ALTER TABLE [InventProduct] add  [IPValueBit5] bit NULL DEFAULT 0;"
						   });

				scripts.Add(new SQLScript
					 {
						 Ver = 46,
						 Tab = 6,
						 DBType = DBName.EmptyCount4UDB,
						 Text = @"ALTER TABLE [InventProduct] add  [IPValueStr1] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [InventProduct] add  [IPValueStr2] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [InventProduct] add  [IPValueStr3] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [InventProduct] add  [IPValueStr4] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [InventProduct] add  [IPValueStr5] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [InventProduct] add  [IPValueStr6] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [InventProduct] add  [IPValueStr7] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [InventProduct] add  [IPValueStr8] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [InventProduct] add  [IPValueStr9] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [InventProduct] add  [IPValueStr10] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [InventProduct] add  [IPValueInt4] int NULL DEFAULT 0;
ALTER TABLE [InventProduct] add  [IPValueInt5] int NULL DEFAULT 0;
ALTER TABLE [InventProduct] add  [IPValueBit3] bit NULL DEFAULT 0;
ALTER TABLE [InventProduct] add  [IPValueBit4] bit NULL DEFAULT 0;
ALTER TABLE [InventProduct] add  [IPValueBit5] bit NULL DEFAULT 0;"
						   });


				scripts.Add(new SQLScript
				{
					Ver = 47,
					Tab = 5,
					DBType = DBName.Count4UDB,
					Text = @"ALTER TABLE [IturAnalyzes] add [CountInParentPack] int NULL DEFAULT 1;
ALTER TABLE [IturAnalyzes] add [BalanceQuantityPartialERP] int NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [QuantityInPackEdit] int NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add  [IPValueStr1] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [IturAnalyzes] add  [IPValueStr2] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [IturAnalyzes] add  [IPValueStr3] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [IturAnalyzes] add  [IPValueStr4] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [IturAnalyzes] add  [IPValueStr5] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [IturAnalyzes] add  [IPValueStr6] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [IturAnalyzes] add  [IPValueStr7] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [IturAnalyzes] add  [IPValueStr8] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [IturAnalyzes] add  [IPValueStr9] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [IturAnalyzes] add  [IPValueStr10] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [IturAnalyzes] add [IPValueFloat1] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [IPValueFloat2] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [IPValueFloat3] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [IPValueFloat4] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [IPValueFloat5] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [IPValueInt1] int NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [IPValueInt2] int NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [IPValueInt3] int NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add  [IPValueInt4] int NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add  [IPValueInt5] int NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [IPValueBit1] bit NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [IPValueBit2] bit NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add  [IPValueBit3] bit NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add  [IPValueBit4] bit NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add  [IPValueBit5] bit NULL DEFAULT 0;"
				});

				scripts.Add(new SQLScript
				{
					Ver = 47,
					Tab = 6,
					DBType = DBName.EmptyCount4UDB,
					Text = @"ALTER TABLE [IturAnalyzes] add [CountInParentPack] int NULL DEFAULT 1;
ALTER TABLE [IturAnalyzes] add [BalanceQuantityPartialERP] int NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [QuantityInPackEdit] int NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add  [IPValueStr1] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [IturAnalyzes] add  [IPValueStr2] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [IturAnalyzes] add  [IPValueStr3] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [IturAnalyzes] add  [IPValueStr4] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [IturAnalyzes] add  [IPValueStr5] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [IturAnalyzes] add  [IPValueStr6] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [IturAnalyzes] add  [IPValueStr7] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [IturAnalyzes] add  [IPValueStr8] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [IturAnalyzes] add  [IPValueStr9] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [IturAnalyzes] add  [IPValueStr10] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [IturAnalyzes] add [IPValueFloat1] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [IPValueFloat2] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [IPValueFloat3] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [IPValueFloat4] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [IPValueFloat5] float NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [IPValueInt1] int NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [IPValueInt2] int NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [IPValueInt3] int NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add  [IPValueInt4] int NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add  [IPValueInt5] int NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [IPValueBit1] bit NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [IPValueBit2] bit NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add  [IPValueBit3] bit NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add  [IPValueBit4] bit NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add  [IPValueBit5] bit NULL DEFAULT 0;"
				});
		//============= 48
		scripts.Add(new SQLScript
			{
				Ver = 48,
				Tab = 1,
				DBType = DBName.Count4UDB,
				Text = @"ALTER TABLE [FieldLink] add  [ViewName] nvarchar(100) NULL DEFAULT '' ;
ALTER TABLE [FieldLink] add  [EditorTemplate] nvarchar(100) NULL DEFAULT '' ;"
			});

			scripts.Add(new SQLScript
			{
				Ver = 48,
				Tab = 2,
				DBType = DBName.EmptyCount4UDB,
				Text = @"ALTER TABLE [FieldLink] add  [ViewName] nvarchar(100) NULL DEFAULT '' ;
ALTER TABLE [FieldLink] add  [EditorTemplate] nvarchar(100) NULL DEFAULT '' ;"
			});

			//========49
				 scripts.Add(new SQLScript
				 {
					 Ver = 49,
					 Tab = 1,
					 DBType = DBName.AuditDB,
					 Text = @"ALTER TABLE [AuditReport] add [Landscape] bit NULL DEFAULT 0;"
				 });

			
				 scripts.Add(new SQLScript
				 {
					 Ver = 49,
					 Tab = 3,
					 DBType = DBName.MainDB,
					 Text = @"ALTER TABLE [Report] add [Landscape] bit NULL DEFAULT 0;"
				 });

				//========50
				 scripts.Add(new SQLScript
				 {
					 Ver = 50,
					 Tab = 1,
					 DBType = DBName.AuditDB,
					 Text = @"ALTER TABLE [Inventor] add [ImportSupplierAdapterCode] nvarchar(100) DEFAULT '';"
				 });

			
				 scripts.Add(new SQLScript
				 {
					 Ver = 50,
					 Tab = 2,
					 DBType = DBName.MainDB,
					 Text = @"ALTER TABLE [Customer] add [ImportSupplierAdapterCode] nvarchar(100) DEFAULT '';"
				 });

			 scripts.Add(new SQLScript
				 {
					 Ver = 50,
					 Tab = 3,
					 DBType = DBName.MainDB,
					 Text = @"ALTER TABLE [Branch] add [ImportSupplierAdapterCode] nvarchar(100) DEFAULT '';"
				 });

			  //============ 51
			 scripts.Add(new SQLScript
			 {
				 Ver = 51,
				 Tab = 1,
				 DBType = DBName.AuditDB,
				 Text = @"ALTER TABLE [AuditReport] add [SupplierSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [AuditReport] add [SectionSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [AuditReport] add [ItursPopupMenu] bit NULL DEFAULT 0;
ALTER TABLE [AuditReport] add [IturPopupMenu] bit NULL DEFAULT 0;
ALTER TABLE [AuditReport] add [DocumentHeaderPopupMenu] bit NULL DEFAULT 0;
ALTER TABLE [AuditReport] add [ItursListPopupMenu] bit NULL DEFAULT 0;"
			 });


			 scripts.Add(new SQLScript
			 {
				 Ver = 51,
				 Tab = 2,
				 DBType = DBName.MainDB,
				 Text = @"ALTER TABLE [Report] add [SupplierSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [Report] add [SectionSearchMenu] bit NULL DEFAULT 0;
ALTER TABLE [Report] add [ItursPopupMenu] bit NULL DEFAULT 0;
ALTER TABLE [Report] add [IturPopupMenu] bit NULL DEFAULT 0;
ALTER TABLE [Report] add [DocumentHeaderPopupMenu] bit NULL DEFAULT 0;
ALTER TABLE [Report] add [ItursListPopupMenu] bit NULL DEFAULT 0;"
			 });
		
			//================================ ver 52
			 scripts.Add(new SQLScript
			 {
				 Ver = 52,
				 Tab = 1,
				 DBType = DBName.Count4UDB,
				 Text = @"CREATE TABLE [UnitPlan] (
  [ID] bigint NOT NULL  IDENTITY (1,1)
, [UnitPlanCode] nvarchar(50) NULL DEFAULT ''
, [Name] nvarchar(50) NULL DEFAULT ''
, [Description] nvarchar(50) NULL DEFAULT ''
, [LayerCode] nvarchar(50) NULL DEFAULT ''
, [ObjectCode] nvarchar(50) NULL DEFAULT ''
, [Container] bit NULL DEFAULT 0
, [Visible] bit NULL DEFAULT 1
, [ParentCode] nvarchar(50) NULL DEFAULT ''
, [StartX] float NULL DEFAULT 0
, [StartY] float NULL DEFAULT 0
, [Height] float NULL DEFAULT 0
, [Width] float NULL DEFAULT 0
, [Zoom] int NULL DEFAULT 100
, [Rotate] int NULL DEFAULT 0
, [Tag] nvarchar(50) NULL DEFAULT ''
, [StatusUnitPlanBit] int NULL DEFAULT 0
, [StatusGroupUnitPlanBit] int NULL DEFAULT 0
);
ALTER TABLE [UnitPlan] ADD CONSTRAINT [PK__UnitPlan__0000000000000DDF] PRIMARY KEY ([ID]);
CREATE UNIQUE INDEX [UQ__UnitPlan__0000000000000D15] ON [UnitPlan] ([ID] ASC);"
			 });

			 scripts.Add(new SQLScript
			 {
				 Ver = 52,
				 Tab = 2,
				 DBType = DBName.EmptyCount4UDB,
				 Text = @"CREATE TABLE [UnitPlan] (
  [ID] bigint NOT NULL  IDENTITY (1,1)
, [UnitPlanCode] nvarchar(50) NULL DEFAULT ''
, [Name] nvarchar(50) NULL DEFAULT ''
, [Description] nvarchar(50) NULL DEFAULT ''
, [LayerCode] nvarchar(50) NULL DEFAULT ''
, [ObjectCode] nvarchar(50) NULL DEFAULT ''
, [Container] bit NULL DEFAULT 0
, [Visible] bit NULL DEFAULT 1
, [ParentCode] nvarchar(50) NULL DEFAULT ''
, [StartX] float NULL DEFAULT 0
, [StartY] float NULL DEFAULT 0
, [Height] float NULL DEFAULT 0
, [Width] float NULL DEFAULT 0
, [Zoom] int NULL DEFAULT 100
, [Rotate] int NULL DEFAULT 0
, [Tag] nvarchar(50) NULL DEFAULT ''
, [StatusUnitPlanBit] int NULL DEFAULT 0
, [StatusGroupUnitPlanBit] int NULL DEFAULT 0
);
ALTER TABLE [UnitPlan] ADD CONSTRAINT [PK__UnitPlan__0000000000000DDF] PRIMARY KEY ([ID]);
CREATE UNIQUE INDEX [UQ__UnitPlan__0000000000000D15] ON [UnitPlan] ([ID] ASC);"
			 });

			 scripts.Add(new SQLScript
			 {
				 Ver = 52,
				 Tab = 3,
				 DBType = DBName.Count4UDB,
				 Text = @"CREATE TABLE [UnitPlanValue] (
  [ID] bigint NOT NULL  IDENTITY (1,1)
, [UnitPlanCode] nvarchar(50) NULL DEFAULT ''
, [UnitPlanStatusBit] int NULL DEFAULT 0
, [UnitPalnGroupStatusBit] int NULL DEFAULT 0
, [TotalItur] int NULL DEFAULT 0
, [DoneItur] int NULL DEFAULT 0
, [Done] int NULL DEFAULT 0
, [TotalItem] float NULL DEFAULT 0
, [SumQuantityEdit] float NULL DEFAULT 0
, [DiffQuantityEdit] float NULL
);
ALTER TABLE [UnitPlanValue] ADD CONSTRAINT [PK__UnitPlanValue__0000000000000DE4] PRIMARY KEY ([ID]);
CREATE UNIQUE INDEX [UQ__UnitPlanValue__0000000000000DB2] ON [UnitPlanValue] ([ID] ASC);"
			 });

			 scripts.Add(new SQLScript
			 {
				 Ver = 52,
				 Tab = 4,
				 DBType = DBName.EmptyCount4UDB,
				 Text = @"CREATE TABLE [UnitPlanValue] (
  [ID] bigint NOT NULL  IDENTITY (1,1)
, [UnitPlanCode] nvarchar(50) NULL DEFAULT ''
, [TotalItur] int NULL DEFAULT 0
, [DoneItur] int NULL DEFAULT 0
, [Done] int NULL DEFAULT 0
, [TotalItem] float NULL DEFAULT 0
, [SumQuantityEdit] float NULL DEFAULT 0
, [DiffQuantityEdit] float NULL
);
ALTER TABLE [UnitPlanValue] ADD CONSTRAINT [PK__UnitPlanValue__0000000000000DE4] PRIMARY KEY ([ID]);
CREATE UNIQUE INDEX [UQ__UnitPlanValue__0000000000000DB2] ON [UnitPlanValue] ([ID] ASC);"
			 });

			 scripts.Add(new SQLScript
			 {
				 Ver = 52,
				 Tab = 5,
				 DBType = DBName.Count4UDB,
				 Text = @"ALTER TABLE [Itur] add [UnitPlanCode] nvarchar(50) NULL DEFAULT '';
ALTER TABLE [Itur] add [TotalItem] float NULL DEFAULT 0;
ALTER TABLE [Itur] add [SumQuantityEdit] float NULL DEFAULT 0;
ALTER TABLE [Itur] add [DiffQuantityEdit] float NULL DEFAULT 0;"
			 });

			 scripts.Add(new SQLScript
			 {
				 Ver = 53,
				 Tab = 6,
				 DBType = DBName.EmptyCount4UDB,
				 Text = @"ALTER TABLE [UnitPlanValue] add [StatusUnitPlanBit] int NULL DEFAULT 0;
ALTER TABLE [UnitPlanValue] add [StatusGroupUnitPlanBit] int NULL DEFAULT 0; "
			 });

			 scripts.Add(new SQLScript
			 {
				 Ver = 53,
				 Tab = 5,
				 DBType = DBName.Count4UDB,
				 Text = @"ALTER TABLE [UnitPlanValue] add [StatusUnitPlanBit] int NULL DEFAULT 0;
ALTER TABLE [UnitPlanValue] add [StatusGroupUnitPlanBit] int NULL DEFAULT 0; "

			 });

			 scripts.Add(new SQLScript
			 {
				 Ver = 53,
				 Tab = 6,
				 DBType = DBName.EmptyCount4UDB,
				 Text = @"ALTER TABLE [Itur] add [UnitPlanCode] nvarchar(50) NULL DEFAULT '';
ALTER TABLE [Itur] add [TotalItem] float NULL DEFAULT 0;
ALTER TABLE [Itur] add [SumQuantityEdit] float NULL DEFAULT 0;
ALTER TABLE [Itur] add [DiffQuantityEdit] float NULL DEFAULT 0;"
			 });

			//===============55
			 scripts.Add(new SQLScript
			 {
				 Ver = 55,
				 Tab = 1,
				 DBType = DBName.MainDB,
				 Text = @"ALTER TABLE [Branch] add [PriceCode] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [Customer] add [PriceCode] nvarchar(100) NULL DEFAULT '';"
			 });

			 scripts.Add(new SQLScript
			 {
				 Ver = 55,
				 Tab = 2,
				 DBType = DBName.AuditDB,
				 Text = @"ALTER TABLE [Inventor] add [PriceCode] nvarchar(100) NULL DEFAULT '';"
			 });

			//============================56
			 scripts.Add(new SQLScript
			 {
				 Ver = 56,
				 Tab = 1,
				 DBType = DBName.Count4UDB,
				 Text = @"ALTER TABLE [IturAnalyzes] add [Price] float NULL DEFAULT 0;"

			 });

			 scripts.Add(new SQLScript
			 {
				 Ver = 56,
				 Tab = 2,
				 DBType = DBName.EmptyCount4UDB,
				 Text = @"ALTER TABLE [IturAnalyzes] add [Price] float NULL DEFAULT 0;"
			 });
			
//======= ver 57
			  scripts.Add(new SQLScript
				  {
					  Ver = 57,
					  Tab = 2,
					  DBType = DBName.AuditDB,
					  Text = @"ALTER TABLE [Inventor] add [ReportContext] nvarchar(100) NULL DEFAULT '';
				  ALTER TABLE [Inventor] add [ReportDS] nvarchar(100) NULL DEFAULT '';
				  ALTER TABLE [Inventor] add [ReportName] nvarchar(100) NULL DEFAULT '';
				 ALTER TABLE [Inventor] add [ReportPath] nvarchar(250) NULL DEFAULT '';"
				  });
			  //======= ver 58
			  scripts.Add(new SQLScript
			  {
				  Ver = 58,
				  Tab = 1,
				  DBType = DBName.AuditDB,
				  Text = @"ALTER TABLE [Inventor] add [Print] bit NULL DEFAULT 0;"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 58,
				  Tab = 2,
				  DBType = DBName.MainDB,
				  Text = @"ALTER TABLE [Branch] add [Print] bit NULL DEFAULT 0;"
			  });

			  //======= ver 59
			  scripts.Add(new SQLScript
			  {
				  Ver = 59,
				  Tab = 1,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"ALTER TABLE [Session] add [SumQuantityEdit] float NULL DEFAULT 0;"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 59,
				  Tab = 2,
				  DBType = DBName.Count4UDB,
				  Text = @"ALTER TABLE [Session] add [SumQuantityEdit] float NULL DEFAULT 0;"
			  });

			  //======= ver 60
			  scripts.Add(new SQLScript
			  {
				  Ver = 60,
				  Tab = 1,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"ALTER TABLE [DocumentHeader] add [QuantityEdit] float NULL DEFAULT 0;
ALTER TABLE [DocumentHeader] add [Total] bigint NULL DEFAULT 0;
ALTER TABLE [DocumentHeader] add [FromTime] datetime NULL DEFAULT GETDATE ( );
ALTER TABLE [DocumentHeader] add [ToTime] datetime NULL DEFAULT GETDATE ( );
ALTER TABLE [DocumentHeader] add [TicksTimeSpan] bigint NULL DEFAULT 0;
ALTER TABLE [DocumentHeader] add [PeriodFromTo]  nvarchar(100) NULL DEFAULT '';"
			  });

		  scripts.Add(new SQLScript
			  {
				  Ver = 60,
				  Tab = 2,
				  DBType = DBName.Count4UDB,
				  Text = @"ALTER TABLE [DocumentHeader] add [QuantityEdit] float NULL DEFAULT 0;
ALTER TABLE [DocumentHeader] add [Total] bigint NULL DEFAULT 0;
ALTER TABLE [DocumentHeader] add [FromTime] datetime NULL DEFAULT GETDATE ( );
ALTER TABLE [DocumentHeader] add [ToTime] datetime NULL DEFAULT GETDATE ( );
ALTER TABLE [DocumentHeader] add [TicksTimeSpan] bigint NULL DEFAULT 0;
ALTER TABLE [DocumentHeader] add [PeriodFromTo]  nvarchar(100) NULL DEFAULT '';"
			  });

			  //======= ver 61
			  scripts.Add(new SQLScript
			  {
				  Ver = 61,
				  Tab = 1,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"ALTER TABLE [InventProduct] add [WorkerID] nvarchar(50) NULL DEFAULT '';"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 61,
				  Tab = 2,
				  DBType = DBName.Count4UDB,
				  Text = @"ALTER TABLE [InventProduct] add [WorkerID] nvarchar(50) NULL DEFAULT '';"
			  });

			  //======= ver 62
			  scripts.Add(new SQLScript
			  {
				  Ver = 62,
				  Tab = 1,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"ALTER TABLE [IturAnalyzes] add [WorkerID]  nvarchar(50) NULL DEFAULT '';
ALTER TABLE [IturAnalyzes] add [WorkerName]  nvarchar(100) NULL DEFAULT '';
ALTER TABLE [IturAnalyzes] add [Total] bigint NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [FromTime] datetime NULL DEFAULT GETDATE ( );
ALTER TABLE [IturAnalyzes] add [ToTime] datetime NULL DEFAULT GETDATE ( );
ALTER TABLE [IturAnalyzes] add [TicksTimeSpan] bigint NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [PeriodFromTo]  nvarchar(100) NULL DEFAULT '';"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 62,
				  Tab = 2,
				  DBType = DBName.Count4UDB,
				  Text = @"ALTER TABLE [IturAnalyzes] add [WorkerID]  nvarchar(50) NULL DEFAULT '';
ALTER TABLE [IturAnalyzes] add [WorkerName]  nvarchar(100) NULL DEFAULT '';
ALTER TABLE [IturAnalyzes] add [Total] bigint NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [FromTime] datetime NULL DEFAULT GETDATE ( );
ALTER TABLE [IturAnalyzes] add [ToTime] datetime NULL DEFAULT GETDATE ( );
ALTER TABLE [IturAnalyzes] add [TicksTimeSpan] bigint NULL DEFAULT 0;
ALTER TABLE [IturAnalyzes] add [PeriodFromTo]  nvarchar(100) NULL DEFAULT '';"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 62,
				  Tab = 3,
				  DBType = DBName.AuditDB,
				  Text = @"ALTER TABLE [Inventor] add [PDAType] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [Inventor] add [MaintenanceType] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [Inventor] add [ProgramType] nvarchar(100) NULL DEFAULT '';"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 62,
				  Tab = 4,
				  DBType = DBName.MainDB,
  				  Text = @"ALTER TABLE [Branch] add [PDAType] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [Branch] add [MaintenanceType] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [Branch] add [ProgramType] nvarchar(100) NULL DEFAULT '';"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 62,
				  Tab = 5,
				  DBType = DBName.MainDB,
   				  Text = @"ALTER TABLE [Customer] add [PDAType] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [Customer] add [MaintenanceType] nvarchar(100) NULL DEFAULT '';
ALTER TABLE [Customer] add [ProgramType] nvarchar(100) NULL DEFAULT '';"
			  });

			//===============	 63
			  scripts.Add(new SQLScript
			  {
				  Ver = 63,
				  Tab = 1,
				  DBType = DBName.Count4UDB,
				  Text = @"ALTER TABLE [UnitPlan] add [Lock] bit NULL DEFAULT 0;"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 63,
				  Tab = 2,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"ALTER TABLE [UnitPlan] add [Lock] bit NULL DEFAULT 0;"
			  });
            //==============
              scripts.Add(new SQLScript
              {
                  Ver = 65,
                  Tab = 1,
                  DBType = DBName.EmptyCount4UDB,
                  Text = @"ALTER TABLE [UnitPlan] add [ZIndex] int NULL DEFAULT 0;"
              });

              scripts.Add(new SQLScript
              {
                  Ver = 65,
                  Tab = 2,
                  DBType = DBName.Count4UDB,
                  Text = @"ALTER TABLE [UnitPlan] add [ZIndex] int NULL DEFAULT 0;"
              });
			// ========= 66 ======
			  scripts.Add(new SQLScript
			  {
				  Ver = 66,
				  Tab = 1,
				  DBType = DBName.EmptyCount4UDB,
			      Text = @"ALTER TABLE [UnitPlan] add [FontSize] int NULL DEFAULT 0;
								ALTER TABLE [UnitPlan] add [Picture] nvarchar(100) NULL DEFAULT '';
								ALTER TABLE [UnitPlan] add [LocationCode] nvarchar(100) NULL DEFAULT '';
								ALTER TABLE [UnitPlan] add [Color] nvarchar(100) NULL DEFAULT '';
								ALTER TABLE [UnitPlan] add [Font] nvarchar(100) NULL DEFAULT '';
								ALTER TABLE [UnitPlan] add [Value] nvarchar(100) NULL DEFAULT '';
								ALTER TABLE [UnitPlan] add [Tooltip] nvarchar(100) NULL DEFAULT '';
								ALTER TABLE [UnitPlan] add [Title] nvarchar(100) NULL DEFAULT '';"
              });

              scripts.Add(new SQLScript
              {
                  Ver = 66,
                  Tab = 2,
                  DBType = DBName.Count4UDB,
				  Text = @"ALTER TABLE [UnitPlan] add [FontSize] int NULL DEFAULT 0;
								ALTER TABLE [UnitPlan] add [Picture] nvarchar(100) NULL DEFAULT '';
								ALTER TABLE [UnitPlan] add [LocationCode] nvarchar(100) NULL DEFAULT '';
								ALTER TABLE [UnitPlan] add [Color] nvarchar(100) NULL DEFAULT '';
								ALTER TABLE [UnitPlan] add [Font] nvarchar(100) NULL DEFAULT '';
								ALTER TABLE [UnitPlan] add [Value] nvarchar(100) NULL DEFAULT '';
								ALTER TABLE [UnitPlan] add [Tooltip] nvarchar(100) NULL DEFAULT '';
								ALTER TABLE [UnitPlan] add [Title] nvarchar(100) NULL DEFAULT '';"
              });

			  // ========= 67 ======
			  scripts.Add(new SQLScript
			  {
				  Ver = 67,
				  Tab = 1,
				  DBType = DBName.Count4UDB,
				  Text = @"CREATE TABLE [Family] (
  [ID] bigint NOT NULL IDENTITY (1,1) 
, [FamilyCode] nvarchar(50) NULL DEFAULT '' 
, [Name] nvarchar(50) NULL DEFAULT ''
, [Type] nvarchar(50) NULL DEFAULT ''
, [Size] nvarchar(50) NULL DEFAULT ''
, [Extra1] nvarchar(50) NULL DEFAULT ''
, [Extra2] nvarchar(50) NULL DEFAULT ''
, [Description] nvarchar(100) NULL DEFAULT ''
);
ALTER TABLE [Family] ADD CONSTRAINT [PK_Family] PRIMARY KEY ([ID]);
CREATE UNIQUE INDEX [UQ__Family__0000000000000F64] ON [Family] ([ID] ASC);"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 67,
				  Tab = 2,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"CREATE TABLE [Family] (
  [ID] bigint NOT NULL IDENTITY (1,1) 
, [FamilyCode] nvarchar(50) NULL DEFAULT '' 
, [Name] nvarchar(50) NULL DEFAULT ''
, [Type] nvarchar(50) NULL DEFAULT ''
, [Size] nvarchar(50) NULL DEFAULT ''
, [Extra1] nvarchar(50) NULL DEFAULT ''
, [Extra2] nvarchar(50) NULL DEFAULT ''
, [Description] nvarchar(100) NULL DEFAULT ''
);
ALTER TABLE [Family] ADD CONSTRAINT [PK_Family] PRIMARY KEY ([ID]);
CREATE UNIQUE INDEX [UQ__Family__0000000000000F64] ON [Family] ([ID] ASC);"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 67,
				  Tab = 3,
				  DBType = DBName.Count4UDB,
				  Text = @"ALTER TABLE [IturAnalyzes] add [FamilyCode] nvarchar(50) NULL DEFAULT '';
					ALTER TABLE [IturAnalyzes] add [FamilyName] nvarchar(50) NULL DEFAULT '';
					ALTER TABLE [IturAnalyzes] add [FamilyType] nvarchar(50) NULL DEFAULT '';
					ALTER TABLE [IturAnalyzes] add [FamilySize] nvarchar(50) NULL DEFAULT '';
					ALTER TABLE [IturAnalyzes] add [FamilyExtra1] nvarchar(50) NULL DEFAULT '';
					ALTER TABLE [IturAnalyzes] add [FamilyExtra2] nvarchar(50) NULL DEFAULT '';"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 67,
				  Tab = 4,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"ALTER TABLE [IturAnalyzes] add [FamilyCode] nvarchar(50) NULL DEFAULT '';
					ALTER TABLE [IturAnalyzes] add [FamilyName] nvarchar(50) NULL DEFAULT '';
					ALTER TABLE [IturAnalyzes] add [FamilyType] nvarchar(50) NULL DEFAULT '';
					ALTER TABLE [IturAnalyzes] add [FamilySize] nvarchar(50) NULL DEFAULT '';
					ALTER TABLE [IturAnalyzes] add [FamilyExtra1] nvarchar(50) NULL DEFAULT '';
					ALTER TABLE [IturAnalyzes] add [FamilyExtra2] nvarchar(50) NULL DEFAULT '';"
			  });


			  scripts.Add(new SQLScript
			  {
				  Ver = 67,
				  Tab = 5,
				  DBType = DBName.Count4UDB,
				  Text = @"ALTER TABLE [Product] add [FamilyCode] nvarchar(50) NULL DEFAULT '';"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 67,
				  Tab = 5,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"ALTER TABLE [Product] add [FamilyCode] nvarchar(50) NULL DEFAULT '';"
			  });


			  scripts.Add(new SQLScript
			  {
				  Ver = 72,
				  Tab = 1,
				  DBType = DBName.Count4UDB,
				  Text = @"	ALTER TABLE [IturAnalyzes] add  [UnitTypeCode] nvarchar(50)  NULL DEFAULT '';
							 ALTER TABLE [IturAnalyzes] add  [InventorCode] nvarchar(100) NULL DEFAULT '';
							 ALTER TABLE [IturAnalyzes] add  [InventorName] nvarchar(100) NULL DEFAULT '';
			  				ALTER TABLE [IturAnalyzes] add [BranchCode] nvarchar(100) NULL DEFAULT '';
							ALTER TABLE [IturAnalyzes] add  [BranchName] nvarchar(100) NULL DEFAULT '';
							ALTER TABLE [IturAnalyzes] add  [BranchERPCode] nvarchar(50) NULL DEFAULT '';
							ALTER TABLE [IturAnalyzes] add  [InventorDate] datetime NULL DEFAULT GETDATE ( );"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 72,
				  Tab = 5,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"	ALTER TABLE [IturAnalyzes] add  [UnitTypeCode] nvarchar(50)  NULL DEFAULT '';
						 ALTER TABLE [IturAnalyzes] add  [InventorCode] nvarchar(100) NULL DEFAULT '';
						ALTER TABLE [IturAnalyzes] add  [InventorName] nvarchar(100) NULL DEFAULT '';
			  			ALTER TABLE [IturAnalyzes] add [BranchCode] nvarchar(100) NULL DEFAULT '';
						ALTER TABLE [IturAnalyzes] add  [BranchName] nvarchar(100) NULL DEFAULT '';
						ALTER TABLE [IturAnalyzes] add  [BranchERPCode] nvarchar(50) NULL DEFAULT '';
						ALTER TABLE [IturAnalyzes] add  [InventorDate] datetime NULL DEFAULT GETDATE ( );"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 74,
				  Tab = 1,
				  DBType = DBName.Count4UDB,
				  Text = @"ALTER TABLE [Product] add [IturCodeExpected] nvarchar(50) NULL DEFAULT '';
								ALTER TABLE [IturAnalyzes] add [IturCodeExpected] nvarchar(50) NULL DEFAULT '';
								ALTER TABLE [IturAnalyzes] add  [IturCodeDiffer] bit NULL DEFAULT 0;"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 74,
				  Tab = 5,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"ALTER TABLE [Product] add [IturCodeExpected] nvarchar(50) NULL DEFAULT '';
								ALTER TABLE [IturAnalyzes] add [IturCodeExpected] nvarchar(50) NULL DEFAULT '';
								ALTER TABLE [IturAnalyzes] add  [IturCodeDiffer] bit NULL DEFAULT 0;"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 76,
				  Tab = 1,
				  DBType = DBName.Count4UDB,
				  Text = @"ALTER TABLE [Supplier] add [PlaceCount] int NULL DEFAULT 0;
								ALTER TABLE [Supplier] add [Value] float NULL DEFAULT 0.0;"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 76,
				  Tab = 2,
				  DBType = DBName.EmptyCount4UDB,
				Text = @"ALTER TABLE [Supplier] add [PlaceCount] int NULL DEFAULT 0;
								ALTER TABLE [Supplier] add [Value] float NULL DEFAULT 0.0;"
			  });

			 scripts.Add(new SQLScript
			  {
				  Ver = 77,
				  Tab = 1,
				  DBType = DBName.Count4UDB,
				  Text = @"ALTER TABLE [InventProduct] add [SupplierCode]  nvarchar(50) NULL DEFAULT '';
								ALTER TABLE [InventProduct] add [SupplierName]  nvarchar(50) NULL DEFAULT '';
								ALTER TABLE [Itur] add [Width] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [Height] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [IncludeInFacing] bit DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [ShelfCount] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add  [ShelfInItur] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [PlaceCount] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add  [PlaceInItur] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [Supplier1PlaceCount] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [Supplier2PlaceCount] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [Supplier3PlaceCount] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [Supplier4PlaceCount] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [Supplier5PlaceCount] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add  [SupplierOtherPlaceCount] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [UnitPlaceWidth] int DEFAULT 0 NULL;"
			  });
				
			  scripts.Add(new SQLScript
			  {
				  Ver = 77,
				  Tab = 2,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"ALTER TABLE [InventProduct] add [SupplierCode]  nvarchar(50) NULL DEFAULT '';
								ALTER TABLE [InventProduct] add [SupplierName]  nvarchar(50) NULL DEFAULT '';
								ALTER TABLE [Itur] add [Width] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [Height] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [IncludeInFacing] bit DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [ShelfCount] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add  [ShelfInItur] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [PlaceCount] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add  [PlaceInItur] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [Supplier1PlaceCount] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [Supplier2PlaceCount] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [Supplier3PlaceCount] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [Supplier4PlaceCount] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [Supplier5PlaceCount] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add  [SupplierOtherPlaceCount] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [UnitPlaceWidth] int DEFAULT 0 NULL;"
			  });


			  scripts.Add(new SQLScript
			  {
				  Ver = 77,
				  Tab = 3,
				  DBType = DBName.Count4UDB,
					Text = @"CREATE TABLE [Shelf] (
					  [ID] bigint IDENTITY (1,1) NOT NULL
					, [ShelfPartCode] nvarchar(50) DEFAULT '' NULL
					, [IturCode] nvarchar(50) DEFAULT '' NULL
					, [SupplierCode] nvarchar(50) DEFAULT '' NULL
					, [ShelfCode] nvarchar(50) DEFAULT '' NULL
					, [SupplierName] nvarchar(50) DEFAULT '' NULL
					, [CreateDataTime] datetime DEFAULT GETDATE ( ) NULL
					);
					ALTER TABLE [Shelf] ADD CONSTRAINT [PK_Shelf] PRIMARY KEY ([ID]);
					CREATE UNIQUE INDEX [UQ__Shelf__00000000000012DE] ON [Shelf] ([ID] ASC);"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 77,
				  Tab = 4,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"CREATE TABLE [Shelf] (
							  [ID] bigint IDENTITY (1,1) NOT NULL
							, [ShelfPartCode] nvarchar(50) DEFAULT '' NULL
							, [IturCode] nvarchar(50) DEFAULT '' NULL
							, [SupplierCode] nvarchar(50) DEFAULT '' NULL
							, [ShelfCode] nvarchar(50) DEFAULT '' NULL
							, [SupplierName] nvarchar(50) DEFAULT '' NULL
							, [CreateDataTime] datetime DEFAULT GETDATE ( ) NULL
							);
							ALTER TABLE [Shelf] ADD CONSTRAINT [PK_Shelf] PRIMARY KEY ([ID]);
							CREATE UNIQUE INDEX [UQ__Shelf__00000000000012DE] ON [Shelf] ([ID] ASC);"
			  });


		  scripts.Add(new SQLScript
			  {
				  Ver = 78,
				  Tab = 1,
				  DBType = DBName.Count4UDB,
				  Text = @"ALTER TABLE [Supplier] add [IturCount] int NULL DEFAULT 0;
								ALTER TABLE [Supplier] add [SupplierLevel] int NULL DEFAULT 0;
								ALTER TABLE [Supplier] add [Area] float NULL DEFAULT 0.0;
								ALTER TABLE [Supplier] add [PercentArea] float NULL DEFAULT 0.0;
								ALTER TABLE [Itur] add [Area] float NULL DEFAULT 0.0;
								ALTER TABLE [Itur] add [AreaCount] float NULL DEFAULT 0.0;
								ALTER TABLE [Shelf] add [ShelfNum] int NULL DEFAULT 0;
								ALTER TABLE [Shelf] add [Width] float NULL DEFAULT 0.0;
								ALTER TABLE [Shelf] add [Height] float NULL DEFAULT 0.0;
								ALTER TABLE [Shelf] add [Area] float NULL DEFAULT 0.0;"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 78,
				  Tab = 2,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"ALTER TABLE [Supplier] add [IturCount] int NULL DEFAULT 0;
								ALTER TABLE [Supplier] add [SupplierLevel] int NULL DEFAULT 0;
								ALTER TABLE [Supplier] add [Area] float NULL DEFAULT 0.0;
								ALTER TABLE [Supplier] add [PercentArea] float NULL DEFAULT 0.0;
								ALTER TABLE [Itur] add [Area] float NULL DEFAULT 0.0;
								ALTER TABLE [Itur] add [AreaCount] float NULL DEFAULT 0.0;
								ALTER TABLE [Shelf] add [ShelfNum] int NULL DEFAULT 0;
								ALTER TABLE [Shelf] add [Width] float NULL DEFAULT 0.0;
								ALTER TABLE [Shelf] add [Height] float NULL DEFAULT 0.0;
								ALTER TABLE [Shelf] add [Area] float NULL DEFAULT 0.0;"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 78,
				  Tab = 1,
				  DBType = DBName.Count4UDB,
				  Text = @"ALTER TABLE [Supplier] add [IturCount] int NULL DEFAULT 0;
								ALTER TABLE [Supplier] add [SupplierLevel] int NULL DEFAULT 0;
								ALTER TABLE [Supplier] add [Area] float NULL DEFAULT 0.0;
								ALTER TABLE [Supplier] add [PercentArea] float NULL DEFAULT 0.0;
								ALTER TABLE [Itur] add [Area] float NULL DEFAULT 0.0;
								ALTER TABLE [Itur] add [AreaCount] float NULL DEFAULT 0.0;
								ALTER TABLE [Shelf] add [ShelfNum] int NULL DEFAULT 0;
								ALTER TABLE [Shelf] add [Width] float NULL DEFAULT 0.0;
								ALTER TABLE [Shelf] add [Height] float NULL DEFAULT 0.0;
								ALTER TABLE [Shelf] add [Area] float NULL DEFAULT 0.0;"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 78,
				  Tab = 2,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"ALTER TABLE [Supplier] add [IturCount] int NULL DEFAULT 0;
								ALTER TABLE [Supplier] add [SupplierLevel] int NULL DEFAULT 0;
								ALTER TABLE [Supplier] add [Area] float NULL DEFAULT 0.0;
								ALTER TABLE [Supplier] add [PercentArea] float NULL DEFAULT 0.0;
								ALTER TABLE [Itur] add [Area] float NULL DEFAULT 0.0;
								ALTER TABLE [Itur] add [AreaCount] float NULL DEFAULT 0.0;
								ALTER TABLE [Shelf] add [ShelfNum] int NULL DEFAULT 0;
								ALTER TABLE [Shelf] add [Width] float NULL DEFAULT 0.0;
								ALTER TABLE [Shelf] add [Height] float NULL DEFAULT 0.0;
								ALTER TABLE [Shelf] add [Area] float NULL DEFAULT 0.0;"
			  });

			
								//================ 79 =========

			  scripts.Add(new SQLScript
			  {
				  Ver = 79,
				  Tab = 1,
				  DBType = DBName.Count4UDB,
				  Text = @"CREATE TABLE [Device] (
  [ID] bigint IDENTITY (1,1) NOT NULL
, [DeviceCode] nvarchar(50) DEFAULT '' NULL
, [Name] nvarchar(50) DEFAULT '' NULL
, [DateCreated] datetime DEFAULT GETDATE ( ) NULL
, [LicenseDate] datetime DEFAULT GETDATE ( ) NULL
, [Description] nvarchar(100) DEFAULT '' NULL
);
ALTER TABLE [Device] ADD CONSTRAINT [PK_Device] PRIMARY KEY ([ID]);
ALTER TABLE [Device] ADD CONSTRAINT [UQ__Device__0000000000001266] UNIQUE ([ID]);"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 79,
				  Tab = 4,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"CREATE TABLE [Device] (
  [ID] bigint IDENTITY (1,1) NOT NULL
, [DeviceCode] nvarchar(50) DEFAULT '' NULL
, [Name] nvarchar(50) DEFAULT '' NULL
, [DateCreated] datetime DEFAULT GETDATE ( ) NULL
, [LicenseDate] datetime DEFAULT GETDATE ( ) NULL
, [Description] nvarchar(100) DEFAULT '' NULL
);
ALTER TABLE [Device] ADD CONSTRAINT [PK_Device] PRIMARY KEY ([ID]);
ALTER TABLE [Device] ADD CONSTRAINT [UQ__Device__0000000000001266] UNIQUE ([ID]);"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 79,
				  Tab = 2,
				  DBType = DBName.Count4UDB,
				  Text = @"CREATE TABLE [PropertyStr] (
  [ID] bigint IDENTITY (1,1) NOT NULL
, [TypeCode] nvarchar(50) DEFAULT '' NULL
, [PropertyStrCode] nvarchar(50) DEFAULT '' NULL
, [Name] nvarchar(50) DEFAULT '' NULL
, [DomainObject] nvarchar(50) DEFAULT '' NULL
, [Code] nvarchar(50) DEFAULT '' NULL
);
ALTER TABLE [PropertyStr] ADD CONSTRAINT [PK_PropertyStr] PRIMARY KEY ([ID]);
ALTER TABLE [PropertyStr] ADD CONSTRAINT [UQ__PropertyStr__0000000000001281] UNIQUE ([ID]);	"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 79,
				  Tab = 5,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"CREATE TABLE [PropertyStr] (
  [ID] bigint IDENTITY (1,1) NOT NULL
, [TypeCode] nvarchar(50) DEFAULT '' NULL
, [PropertyStrCode] nvarchar(50) DEFAULT '' NULL
, [Name] nvarchar(50) DEFAULT '' NULL
, [DomainObject] nvarchar(50) DEFAULT '' NULL
, [Code] nvarchar(50) DEFAULT '' NULL
);
ALTER TABLE [PropertyStr] ADD CONSTRAINT [PK_PropertyStr] PRIMARY KEY ([ID]);
ALTER TABLE [PropertyStr] ADD CONSTRAINT [UQ__PropertyStr__0000000000001281] UNIQUE ([ID]);	"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 79,
				  Tab = 3,
				  DBType = DBName.Count4UDB,
				  Text = @"CREATE TABLE [PropertyStrToObject] (
  [ID] bigint IDENTITY (1,1) NOT NULL
, [PropertyStrCode] nvarchar(50) DEFAULT '' NULL
, [ObjectCode] nvarchar(50) DEFAULT '' NULL
, [DomainObject] nvarchar(50) DEFAULT '' NULL
);
ALTER TABLE [PropertyStrToObject] ADD CONSTRAINT [PK_PropertyStrToObject] PRIMARY KEY ([ID]);
ALTER TABLE [PropertyStrToObject] ADD CONSTRAINT [UQ__PropertyStrToObject__00000000000012F7] UNIQUE ([ID]);"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 79,
				  Tab = 6,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"CREATE TABLE [PropertyStrToObject] (
  [ID] bigint IDENTITY (1,1) NOT NULL
, [PropertyStrCode] nvarchar(50) DEFAULT '' NULL
, [ObjectCode] nvarchar(50) DEFAULT '' NULL
, [DomainObject] nvarchar(50) DEFAULT '' NULL
);
ALTER TABLE [PropertyStrToObject] ADD CONSTRAINT [PK_PropertyStrToObject] PRIMARY KEY ([ID]);
ALTER TABLE [PropertyStrToObject] ADD CONSTRAINT [UQ__PropertyStrToObject__00000000000012F7] UNIQUE ([ID]);"
			  });


			  scripts.Add(new SQLScript
			  {
				  Ver = 79,
				  Tab = 7,
				  DBType = DBName.Count4UDB,
				  Text = @"ALTER TABLE [Section] add [ParentSectionCode] nvarchar(50) DEFAULT '' NULL;
								ALTER TABLE [Section] add [Tag]  nvarchar(50) DEFAULT '' NULL;
								ALTER TABLE [Section] add [TypeCode] nvarchar(50) DEFAULT '' NULL;"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 79,
				  Tab = 8,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"ALTER TABLE [Section] add [ParentSectionCode] nvarchar(50) DEFAULT '' NULL;
								ALTER TABLE [Section] add [Tag]  nvarchar(50) DEFAULT '' NULL;
								ALTER TABLE [Section] add [TypeCode] nvarchar(50) DEFAULT '' NULL;"
			  });


			  scripts.Add(new SQLScript
			  {
				  Ver = 79,
				  Tab = 9,
				  DBType = DBName.Count4UDB,
				  Text = @"ALTER TABLE [Location] add [ParentLocationCode] nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Location] add [TypeCode] nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Location] add [Level1] nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Location] add [Level2] nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Location] add [Level3] nvarchar(50) DEFAULT '' NULL ;
ALTER TABLE [Location] add [Level4] nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Location] add [Name1] nvarchar(250) DEFAULT '' NULL;
ALTER TABLE [Location] add [Name2] nvarchar(250) DEFAULT '' NULL;
ALTER TABLE [Location] add [Name3] nvarchar(250) DEFAULT '' NULL;
ALTER TABLE [Location] add [Name4] nvarchar(250) DEFAULT '' NULL;
ALTER TABLE [Location] add [NodeType] int DEFAULT 1 NULL;
ALTER TABLE [Location] add [LevelNum] int DEFAULT 1 NULL;
ALTER TABLE [Location] add [Total] int DEFAULT 0 NULL;
ALTER TABLE [Location] add [DateModified] datetime DEFAULT GETDATE ( ) NULL;
ALTER TABLE [Location] add [Tag] nvarchar(100) DEFAULT '' NULL;"
			  });


			  scripts.Add(new SQLScript
			  {
				  Ver = 79,
				  Tab = 10,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"ALTER TABLE [Location] add [ParentLocationCode] nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Location] add [TypeCode] nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Location] add [Level1] nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Location] add [Level2] nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Location] add [Level3] nvarchar(50) DEFAULT '' NULL ;
ALTER TABLE [Location] add [Level4] nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Location] add [Name1] nvarchar(250) DEFAULT '' NULL;
ALTER TABLE [Location] add [Name2] nvarchar(250) DEFAULT '' NULL;
ALTER TABLE [Location] add [Name3] nvarchar(250) DEFAULT '' NULL;
ALTER TABLE [Location] add [Name4] nvarchar(250) DEFAULT '' NULL;
ALTER TABLE [Location] add [NodeType] int DEFAULT 1 NULL;
ALTER TABLE [Location] add [LevelNum] int DEFAULT 1 NULL;
ALTER TABLE [Location] add [Total] int DEFAULT 0 NULL;
ALTER TABLE [Location] add [DateModified] datetime DEFAULT GETDATE ( ) NULL;
ALTER TABLE [Location] add [Tag] nvarchar(100) DEFAULT '' NULL;"
			  });


			  scripts.Add(new SQLScript
			  {
				  Ver = 80,
				  Tab = 1,
				  DBType = DBName.MainDB,
				  Text = @"UPDATE CustomerConfig  SET  Description = Value;"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 80,
				  Tab = 2,
				  DBType = DBName.MainDB,
				  Text = @"ALTER TABLE [CustomerConfig] DROP COLUMN [Value];"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 80,
				  Tab = 3,
				  DBType = DBName.MainDB,
				  Text = @"ALTER TABLE [CustomerConfig] add [Value] nvarchar(500) NULL DEFAULT '';"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 80,
				  Tab = 4,
				  DBType = DBName.MainDB,
				  Text = @"UPDATE CustomerConfig  SET  Value = Description;"
			  });

	
			 scripts.Add(new SQLScript
			  {
				  Ver = 80,
				  Tab = 7,
				  DBType = DBName.Count4UDB,
				  Text = @"ALTER TABLE [Location] add [InvStatus] int DEFAULT 0 NOT NULL;
								ALTER TABLE [Location] add [Disabled] bit DEFAULT 0 NOT NULL;"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 80,
				  Tab = 8,
				  DBType = DBName.EmptyCount4UDB,
			  Text = @"ALTER TABLE [Location] add [InvStatus] int DEFAULT 0 NOT NULL;
								ALTER TABLE [Location] add [Disabled] bit DEFAULT 0 NOT NULL;"
			  });

			
			 scripts.Add(new SQLScript
			  {
				  Ver = 80,
				  Tab = 9,
				  DBType = DBName.Count4UDB,
				  Text = @"ALTER TABLE [InventProduct] add [ItemStatus] nvarchar(50) DEFAULT '' NULL;
								ALTER TABLE [InventProduct] add [ERPIturCode] nvarchar(50) DEFAULT '' NULL;
  								ALTER TABLE [InventProduct] add [IPValueStr11] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [InventProduct] add [IPValueStr12] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [InventProduct] add [IPValueStr13] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [InventProduct] add [IPValueStr14] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [InventProduct] add [IPValueStr15] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [InventProduct] add [IPValueStr16] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [InventProduct] add [IPValueStr17] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [InventProduct] add [IPValueStr18] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [InventProduct] add [IPValueStr19] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [InventProduct] add [IPValueStr20] nvarchar(100) DEFAULT '' NULL;  "
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 80,
				  Tab = 10,
				  DBType = DBName.EmptyCount4UDB,
					  Text = @"ALTER TABLE [InventProduct] add [ItemStatus] nvarchar(50) DEFAULT '' NULL;
								ALTER TABLE [InventProduct] add [ERPIturCode] nvarchar(50) DEFAULT '' NULL;
  								ALTER TABLE [InventProduct] add [IPValueStr11] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [InventProduct] add [IPValueStr12] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [InventProduct] add [IPValueStr13] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [InventProduct] add [IPValueStr14] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [InventProduct] add [IPValueStr15] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [InventProduct] add [IPValueStr16] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [InventProduct] add [IPValueStr17] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [InventProduct] add [IPValueStr18] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [InventProduct] add [IPValueStr19] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [InventProduct] add [IPValueStr20] nvarchar(100) DEFAULT '' NULL;  "
		  });


			scripts.Add(new SQLScript
			  {
				  Ver = 80,
				  Tab = 11,
				  DBType = DBName.Count4UDB,
				  Text = @"ALTER TABLE [Product] add [SectionName] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [Product] add [SubSectionCode] nvarchar(50) DEFAULT '' NULL;
								ALTER TABLE [Product] add [SubSectionName] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [Product] add [SupplierName] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [Product] add [ItemType] nvarchar(50) DEFAULT '' NULL;"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 80,
				  Tab = 12,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"ALTER TABLE [Product] add [SectionName] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [Product] add [SubSectionCode] nvarchar(50) DEFAULT '' NULL;
								ALTER TABLE [Product] add [SubSectionName] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [Product] add [SupplierName] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [Product] add [ItemType] nvarchar(50) DEFAULT '' NULL;"
			  });


				scripts.Add(new SQLScript
			  {
				  Ver = 80,
				  Tab = 13,
				  DBType = DBName.Count4UDB,
				  Text = @"ALTER TABLE [Itur] add [Level1] nvarchar(50) DEFAULT '' NULL;
								ALTER TABLE [Itur] add [Level2] nvarchar(50) DEFAULT '' NULL;
								ALTER TABLE [Itur] add [Level3] nvarchar(50) DEFAULT '' NULL;
								ALTER TABLE [Itur] add [Level4] nvarchar(50) DEFAULT '' NULL;
								ALTER TABLE [Itur] add [Name1] nvarchar(250) DEFAULT '' NULL;
								ALTER TABLE [Itur] add [Name2] nvarchar(250) DEFAULT '' NULL;
								ALTER TABLE [Itur] add [Name3] nvarchar(250) DEFAULT '' NULL;
								ALTER TABLE [Itur] add [Name4] nvarchar(250) DEFAULT '' NULL;
								ALTER TABLE [Itur] add [NodeType] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [LevelNum] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [Total] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [Tag] nvarchar(500) DEFAULT '' NULL;
								ALTER TABLE [Itur] add [InvStatus] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [ParentIturCode] nvarchar(50) DEFAULT '' NULL;
								ALTER TABLE [Itur] add  [BackgroundColor] nvarchar(50) DEFAULT '' NULL;
								ALTER TABLE [Itur] add [TypeCode] nvarchar(50) DEFAULT '' NULL;"
			  });
	
	
			  scripts.Add(new SQLScript
			  {
				  Ver = 80,
				  Tab = 14,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"ALTER TABLE [Itur] add [Level1] nvarchar(50) DEFAULT '' NULL;
								ALTER TABLE [Itur] add [Level2] nvarchar(50) DEFAULT '' NULL;
								ALTER TABLE [Itur] add [Level3] nvarchar(50) DEFAULT '' NULL;
								ALTER TABLE [Itur] add [Level4] nvarchar(50) DEFAULT '' NULL;
								ALTER TABLE [Itur] add [Name1] nvarchar(250) DEFAULT '' NULL;
								ALTER TABLE [Itur] add [Name2] nvarchar(250) DEFAULT '' NULL;
								ALTER TABLE [Itur] add [Name3] nvarchar(250) DEFAULT '' NULL;
								ALTER TABLE [Itur] add [Name4] nvarchar(250) DEFAULT '' NULL;
								ALTER TABLE [Itur] add [NodeType] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [LevelNum] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [Total] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [Tag] nvarchar(500) DEFAULT '' NULL;
								ALTER TABLE [Itur] add [InvStatus] int DEFAULT 0 NULL;
								ALTER TABLE [Itur] add [ParentIturCode] nvarchar(50) DEFAULT '' NULL;
								ALTER TABLE [Itur] add  [BackgroundColor] nvarchar(50) DEFAULT '' NULL;
								ALTER TABLE [Itur] add [TypeCode] nvarchar(50) DEFAULT '' NULL;"
			  });

	
	
//			  scripts.Add(new SQLScript
//			  {
//				  Ver = 81,
//				  Tab = 3,
//				  DBType = DBName.Count4UDB,
//				  Text = @"ALTER TABLE [Location] add [InvStatus] int DEFAULT 0 NOT NULL;
//								ALTER TABLE [Location] add  [Disabled] bit DEFAULT 0 NOT NULL;	  "
//			  });

//			  scripts.Add(new SQLScript
//			  {
//				  Ver = 81,
//				  Tab = 4,
//				  DBType = DBName.EmptyCount4UDB,
//				  Text = @"ALTER TABLE [Location] add [InvStatus] int DEFAULT 0 NOT NULL;
//								ALTER TABLE [Location] add  [Disabled] bit DEFAULT 0 NOT NULL;	  "
//			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 82,
				  Tab = 1,
				  DBType = DBName.Count4UDB,
				  Text = @"ALTER TABLE [IturAnalyzes] add [IPValueStr11] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [IturAnalyzes] add [IPValueStr12] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [IturAnalyzes] add [IPValueStr13] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [IturAnalyzes] add [IPValueStr14] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [IturAnalyzes] add [IPValueStr15] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [IturAnalyzes] add [IPValueStr16] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [IturAnalyzes] add [IPValueStr17] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [IturAnalyzes] add [IPValueStr18] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [IturAnalyzes] add [IPValueStr19] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [IturAnalyzes] add [IPValueStr20] nvarchar(100) DEFAULT '' NULL; 
								ALTER TABLE [IturAnalyzes] add  [SubSessionCode] nvarchar(50) DEFAULT '' NULL;
								ALTER TABLE [IturAnalyzes] add [SessionName] nvarchar(100) DEFAULT '' NULL;
								 ALTER TABLE [IturAnalyzes] add [SubSessionName] nvarchar(100) DEFAULT '' NULL;"

			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 82,
				  Tab = 2,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"ALTER TABLE [IturAnalyzes] add [IPValueStr11] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [IturAnalyzes] add [IPValueStr12] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [IturAnalyzes] add [IPValueStr13] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [IturAnalyzes] add [IPValueStr14] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [IturAnalyzes] add [IPValueStr15] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [IturAnalyzes] add [IPValueStr16] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [IturAnalyzes] add [IPValueStr17] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [IturAnalyzes] add [IPValueStr18] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [IturAnalyzes] add [IPValueStr19] nvarchar(100) DEFAULT '' NULL;
								ALTER TABLE [IturAnalyzes] add [IPValueStr20] nvarchar(100) DEFAULT '' NULL; 
								ALTER TABLE [IturAnalyzes] add  [SubSessionCode] nvarchar(50) DEFAULT '' NULL;
								ALTER TABLE [IturAnalyzes] add [SessionName] nvarchar(100) DEFAULT '' NULL;
								 ALTER TABLE [IturAnalyzes] add [SubSessionName] nvarchar(100) DEFAULT '' NULL;"
			  });


			  scripts.Add(new SQLScript
			  {
				  Ver = 83,
				  Tab = 4,
				  DBType = DBName.MainDB,
				  Text = @"ALTER TABLE [Customer] add ComplexStaticPath1 nvarchar(255) DEFAULT '' NULL;
ALTER TABLE [Customer] add ComplexStaticPath2 nvarchar(255) DEFAULT '' NULL;
ALTER TABLE [Customer] add ComplexStaticPath3 nvarchar(255) DEFAULT ''  NULL;
ALTER TABLE [Customer] add Host nvarchar(255) DEFAULT '' NULL;
ALTER TABLE [Customer] add Port nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Customer] add ImportCatalogPath nvarchar(255) DEFAULT '' NULL;
ALTER TABLE [Customer] add ImportFromPdaPath nvarchar(255) DEFAULT '' NULL;
ALTER TABLE [Customer] add ExportErpPath nvarchar(255) DEFAULT '' NULL;
ALTER TABLE [Customer] add ExportPdaPath  nvarchar(255) DEFAULT '' NULL;
ALTER TABLE [Customer] add SendToOfficePath nvarchar(255) DEFAULT '' NULL;
ALTER TABLE [Customer] add LastUpdatedCatalog datetime DEFAULT GETDATE ( ) NULL;
ALTER TABLE [Customer] add Tag1 nvarchar(500) DEFAULT '' NULL;
ALTER TABLE [Customer] add Tag2 nvarchar(500) DEFAULT '' NULL;
ALTER TABLE [Customer] add Tag3 nvarchar(500) DEFAULT '' NULL;
ALTER TABLE [Customer] add ComplexAdapterCode nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Customer] add ComplexAdapterParametr nvarchar(500) DEFAULT '' NULL;
ALTER TABLE [Customer] add ComplexAdapterParametr1 nvarchar(500) DEFAULT '' NULL;
ALTER TABLE [Customer] add ComplexAdapterParametr2 nvarchar(500) DEFAULT '' NULL;
ALTER TABLE [Customer] add ComplexAdapterParametr3 nvarchar(500) DEFAULT '' NULL;
ALTER TABLE [Customer] add ComplexAdapterParametrERPCode nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Customer] add ComplexAdapterParametrInventorDateTime nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Customer] add StatusAuditConfig nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Customer] add ImportFamilyAdapterCode nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Customer] add MaxCharacters nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Customer] add MakatOrMakatOriginal nvarchar(50) DEFAULT '' NULL;
"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 83,
				  Tab = 5,
				  DBType = DBName.MainDB,
				  Text = @"ALTER TABLE [Branch] add ComplexStaticPath1 nvarchar(255) DEFAULT '' NULL;
ALTER TABLE [Branch] add ComplexStaticPath2 nvarchar(255) DEFAULT '' NULL;
ALTER TABLE [Branch] add ComplexStaticPath3 nvarchar(255) DEFAULT ''  NULL;
ALTER TABLE [Branch] add Host nvarchar(255) DEFAULT '' NULL;
ALTER TABLE [Branch] add Port nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Branch] add ImportCatalogPath nvarchar(255) DEFAULT '' NULL;
ALTER TABLE [Branch] add ImportFromPdaPath nvarchar(255) DEFAULT '' NULL;
ALTER TABLE [Branch] add ExportErpPath nvarchar(255) DEFAULT '' NULL;
ALTER TABLE [Branch] add ExportPdaPath  nvarchar(255) DEFAULT '' NULL;
ALTER TABLE [Branch] add SendToOfficePath nvarchar(255) DEFAULT '' NULL;
ALTER TABLE [Branch] add LastUpdatedCatalog datetime DEFAULT GETDATE ( ) NULL;
ALTER TABLE [Branch] add Tag nvarchar(500) DEFAULT '' NULL;
ALTER TABLE [Branch] add Tag1 nvarchar(500) DEFAULT '' NULL;
ALTER TABLE [Branch] add Tag2 nvarchar(500) DEFAULT '' NULL;
ALTER TABLE [Branch] add Tag3 nvarchar(500) DEFAULT '' NULL;
ALTER TABLE [Branch] add ComplexAdapterCode nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Branch] add ComplexAdapterParametr nvarchar(500) DEFAULT '' NULL;
ALTER TABLE [Branch] add ComplexAdapterParametr1 nvarchar(500) DEFAULT '' NULL;
ALTER TABLE [Branch] add ComplexAdapterParametr2 nvarchar(500) DEFAULT '' NULL;
ALTER TABLE [Branch] add ComplexAdapterParametr3 nvarchar(500) DEFAULT '' NULL;
ALTER TABLE [Branch] add ComplexAdapterParametrERPCode nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Branch] add ComplexAdapterParametrInventorDateTime nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Branch] add StatusAuditConfig nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Branch] add ImportFamilyAdapterCode nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Branch] add MaxCharacters nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Branch] add MakatOrMakatOriginal nvarchar(50) DEFAULT '' NULL;
"
			  });


			  scripts.Add(new SQLScript
			  {
				  Ver = 83,
				  Tab = 6,
				  DBType = DBName.AuditDB,
				  Text = @"ALTER TABLE [Inventor] add ComplexStaticPath1 nvarchar(255) DEFAULT '' NULL;
ALTER TABLE [Inventor] add ComplexStaticPath2 nvarchar(255) DEFAULT '' NULL;
ALTER TABLE [Inventor] add ComplexStaticPath3 nvarchar(255) DEFAULT ''  NULL;
ALTER TABLE [Inventor] add Host nvarchar(255) DEFAULT '' NULL;
ALTER TABLE [Inventor] add Port nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Inventor] add ImportCatalogPath nvarchar(255) DEFAULT '' NULL;
ALTER TABLE [Inventor] add ImportFromPdaPath nvarchar(255) DEFAULT '' NULL;
ALTER TABLE [Inventor] add ExportErpPath nvarchar(255) DEFAULT '' NULL;
ALTER TABLE [Inventor] add ExportPdaPath  nvarchar(255) DEFAULT '' NULL;
ALTER TABLE [Inventor] add SendToOfficePath nvarchar(255) DEFAULT '' NULL;
ALTER TABLE [Inventor] add LastUpdatedCatalog datetime DEFAULT GETDATE ( ) NULL;
ALTER TABLE [Inventor] add Tag nvarchar(500) DEFAULT '' NULL;
ALTER TABLE [Inventor] add Tag1 nvarchar(500) DEFAULT '' NULL;
ALTER TABLE [Inventor] add Tag2 nvarchar(500) DEFAULT '' NULL;
ALTER TABLE [Inventor] add Tag3 nvarchar(500) DEFAULT '' NULL;
ALTER TABLE [Inventor] add ComplexAdapterCode nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Inventor] add ComplexAdapterParametr nvarchar(500) DEFAULT '' NULL;
ALTER TABLE [Inventor] add ComplexAdapterParametr1 nvarchar(500) DEFAULT '' NULL;
ALTER TABLE [Inventor] add ComplexAdapterParametr2 nvarchar(500) DEFAULT '' NULL;
ALTER TABLE [Inventor] add ComplexAdapterParametr3 nvarchar(500) DEFAULT '' NULL;
ALTER TABLE [Inventor] add ComplexAdapterParametrERPCode nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Inventor] add ComplexAdapterParametrInventorDateTime nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Inventor] add StatusAuditConfig nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Inventor] add ImportFamilyAdapterCode nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Inventor] add MaxCharacters nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Inventor] add MakatOrMakatOriginal nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Inventor] add ExportSectionAdapterCode nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Inventor] add ExportCatalogAdapterCode nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Inventor] add ParentCode nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Inventor] add SourceCode nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Inventor] add RootCode nvarchar(50) DEFAULT '' NULL;
ALTER TABLE [Inventor] add ProcessCode nvarchar(50) DEFAULT '' NULL;
"
			  });

			//========== ver 84 с этой версии появилась ProcessDB==============
			  scripts.Add(new SQLScript
			  {
				  Ver = 84,
				  Tab = 3,
				  DBType = DBName.AuditDB,
				  Text = @"CREATE TABLE [AuditProcessJob] (
  [ID] bigint IDENTITY (1,1)  NOT NULL
, [ProcessJobCode] nvarchar(100) DEFAULT '' NULL
, [ProcessCode] nvarchar(100) DEFAULT '' NULL
, [SyncCode] nvarchar(100) DEFAULT '' NULL
, [PoolCode] nvarchar(100) DEFAULT '' NULL
, [ParentProcessCode] nvarchar(100) DEFAULT '' NULL
,[FirstProcessCode] nvarchar(100) DEFAULT '' NULL
,[NextProcessCode] nvarchar(100) DEFAULT '' NULL
,[PrevProcessCode] nvarchar(100) DEFAULT '' NULL
,[LastProcessCode] nvarchar(100) DEFAULT '' NULL
, [DomainType] nvarchar(50) DEFAULT '' NULL
,[NN] int DEFAULT 0
, [Description] nvarchar(500)  NULL
, [JobTypeCode] nvarchar(50) DEFAULT '' NULL
, [JobServiceCode] nvarchar(50) DEFAULT '' NULL
, [StatusCode] nvarchar(50) DEFAULT '' NULL
, [CreateDate] datetime DEFAULT GETDATE ( ) NULL
, [GetDate] datetime DEFAULT GETDATE ( ) NULL
, [ResentDate] datetime DEFAULT GETDATE ( ) NULL
, [StartDate] datetime DEFAULT GETDATE ( ) NULL
, [FinishDate] datetime DEFAULT GETDATE ( ) NULL
, [ClosedDate] datetime DEFAULT GETDATE ( ) NULL
, [ModifiedDate] datetime DEFAULT GETDATE ( ) NULL
, [Owner] nvarchar(100) DEFAULT '' NULL
, [Device] nvarchar(50) DEFAULT '' NULL
, [DbFileName] nvarchar(250) DEFAULT '' NULL
, [Tag] nvarchar(500) DEFAULT '' NULL
, [Tag1] nvarchar(500) DEFAULT '' NULL
, [Tag2] nvarchar(500) DEFAULT '' NULL
, [Tag3] nvarchar(500) DEFAULT '' NULL
, [TagIP] nvarchar(50) DEFAULT '' NULL
, [TagHost] nvarchar(50) DEFAULT '' NULL
, [Operation] nvarchar(50) DEFAULT '' NULL
, [OperationResult] nvarchar(50) DEFAULT '' NULL
,[ContextCBI] nvarchar(100) DEFAULT '' NULL
,[CurrentAuditConfigCode] nvarchar(100) DEFAULT '' NULL
,[CurrentCBIObjectType] nvarchar(100) DEFAULT '' NULL
,[CurrentCBIObjectCode] nvarchar(100) DEFAULT '' NULL
,[CurrentCustomerCode] nvarchar(100) DEFAULT '' NULL
,[CurrentBranchCode] nvarchar(100) DEFAULT '' NULL
,[CurrentInventorCode] nvarchar(100) DEFAULT '' NULL
);	
ALTER TABLE [AuditProcessJob] ADD CONSTRAINT [PK_AuditProcessJob] PRIMARY KEY ([ID]);
ALTER TABLE [AuditProcessJob] ADD CONSTRAINT [UQ__AuditProcessJob__00000000000002C2] UNIQUE ([ID]);
"
			  });


			  scripts.Add(new SQLScript
			  {
				  Ver = 84,
				  Tab = 5,
				  DBType = DBName.AuditDB,
				  Text = @"CREATE TABLE [TemporaryAuditProcessJob] (
  [ID] bigint IDENTITY (1,1)  NOT NULL
, [ProcessJobCode] nvarchar(100) DEFAULT '' NULL
, [ProcessCode] nvarchar(100) DEFAULT '' NULL
, [SyncCode] nvarchar(100) DEFAULT '' NULL
, [PoolCode] nvarchar(100) DEFAULT '' NULL
, [ParentProcessCode] nvarchar(100) DEFAULT '' NULL
,[FirstProcessCode] nvarchar(100) DEFAULT '' NULL
,[NextProcessCode] nvarchar(100) DEFAULT '' NULL
,[PrevProcessCode] nvarchar(100) DEFAULT '' NULL
,[LastProcessCode] nvarchar(100) DEFAULT '' NULL
, [DomainType] nvarchar(50) DEFAULT '' NULL
,[NN] int DEFAULT 0
, [Description] nvarchar(500)  NULL
, [JobTypeCode] nvarchar(50) DEFAULT '' NULL
, [JobServiceCode] nvarchar(50) DEFAULT '' NULL
, [StatusCode] nvarchar(50) DEFAULT '' NULL
, [CreateDate] datetime DEFAULT GETDATE ( ) NULL
, [GetDate] datetime DEFAULT GETDATE ( ) NULL
, [ResentDate] datetime DEFAULT GETDATE ( ) NULL
, [StartDate] datetime DEFAULT GETDATE ( ) NULL
, [FinishDate] datetime DEFAULT GETDATE ( ) NULL
, [ClosedDate] datetime DEFAULT GETDATE ( ) NULL
, [ModifiedDate] datetime DEFAULT GETDATE ( ) NULL
, [Owner] nvarchar(100) DEFAULT '' NULL
, [Device] nvarchar(50) DEFAULT '' NULL
, [DbFileName] nvarchar(250) DEFAULT '' NULL
, [Tag] nvarchar(500) DEFAULT '' NULL
, [Tag1] nvarchar(500) DEFAULT '' NULL
, [Tag2] nvarchar(500) DEFAULT '' NULL
, [Tag3] nvarchar(500) DEFAULT '' NULL
, [TagIP] nvarchar(50) DEFAULT '' NULL
, [TagHost] nvarchar(50) DEFAULT '' NULL
, [Operation] nvarchar(50) DEFAULT '' NULL
, [OperationResult] nvarchar(50) DEFAULT '' NULL
,[ContextCBI] nvarchar(100) DEFAULT '' NULL
,[CurrentAuditConfigCode] nvarchar(100) DEFAULT '' NULL
,[CurrentCBIObjectType] nvarchar(100) DEFAULT '' NULL
,[CurrentCBIObjectCode] nvarchar(100) DEFAULT '' NULL
,[CurrentCustomerCode] nvarchar(100) DEFAULT '' NULL
,[CurrentBranchCode] nvarchar(100) DEFAULT '' NULL
,[CurrentInventorCode] nvarchar(100) DEFAULT '' NULL
);
ALTER TABLE [TemporaryAuditProcessJob] ADD CONSTRAINT [PK_TemporaryAuditProcessJob] PRIMARY KEY ([ID]);
ALTER TABLE [TemporaryAuditProcessJob] ADD CONSTRAINT [UQ__TemporaryAuditProcessJob__00000000000002C2] UNIQUE ([ID]);
"
			
			  });

			  //============		85

			  scripts.Add(new SQLScript
			  {
				  Ver = 85,
				  Tab = 1,
				  DBType = DBName.Count4UDB,
				  Text = @"DROP INDEX [Product].[CodeInx];
			  DROP INDEX [Product].[BarcodeInx];
			  DROP INDEX [Product].[MakatERPInx];
			  DROP INDEX [Product].[MarkInx];
			  DROP INDEX [Product].[SectionCodeInx];
			  DROP INDEX [Product].[SupplierCodeInx];
			   DROP INDEX [Itur].[Code_Inx];
			  DROP INDEX [InventProduct].[BarcodeInx];
			   DROP INDEX [InventProduct].[CodeInx];
			   DROP INDEX [InventProduct].[MakatInx];
			  DROP INDEX [InventProduct].[StatusInventProductCodeInx];"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 85,
				  Tab = 2,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"DROP INDEX [Product].[CodeInx];
			  DROP INDEX [Product].[BarcodeInx];
			  DROP INDEX [Product].[MakatERPInx];
			  DROP INDEX [Product].[MarkInx];
			  DROP INDEX [Product].[SectionCodeInx];
			  DROP INDEX [Product].[SupplierCodeInx];
			   DROP INDEX [Itur].[Code_Inx];
			  DROP INDEX [InventProduct].[BarcodeInx];
			   DROP INDEX [InventProduct].[CodeInx];
			   DROP INDEX [InventProduct].[MakatInx];
			  DROP INDEX [InventProduct].[StatusInventProductCodeInx];"
			  });

		


			  scripts.Add(new SQLScript
			  {
				  Ver = 87,
				  Tab = 1,
				  DBType = DBName.Count4UDB,
				  Text = @"ALTER TABLE [InventProduct] add UnityCode nvarchar(50) DEFAULT '' NULL;
			ALTER TABLE [InventProduct] add LocationCode nvarchar(50) DEFAULT '' NULL;
			ALTER TABLE [InventProduct] add Tag nvarchar(100) DEFAULT '' NULL;
			ALTER TABLE [InventProduct] add Tag1 nvarchar(100) DEFAULT '' NULL;
			ALTER TABLE [InventProduct] add Tag2 nvarchar(100) DEFAULT '' NULL;
			ALTER TABLE [InventProduct] add Tag3 nvarchar(100) DEFAULT '' NULL;
			ALTER TABLE [InventProduct] add [QuantityWithoutPackEdit] float NULL DEFAULT 0.0;
			ALTER TABLE [InventProduct] add [ValueBuyDifference] float NULL DEFAULT 0.0;
			ALTER TABLE [InventProduct] add [ValueBuyEdit] float NULL DEFAULT 0.0;
			ALTER TABLE [InventProduct] add [ValueBuyQriginal] float NULL DEFAULT 0.0;
			ALTER TABLE [InventProduct] add [ValueBuyWithoutPackEdit] float NULL DEFAULT 0.0;
			ALTER TABLE [InventProduct] add [ValueBuyInPackEdit] float NULL DEFAULT 0.0;
			ALTER TABLE [InventProduct] add [ValueSaleDifference] float NULL DEFAULT 0.0;
			ALTER TABLE [InventProduct] add [ValueSaleEdit] float NULL DEFAULT 0.0;
			ALTER TABLE [InventProduct] add [ValueSaleQriginal] float NULL DEFAULT 0.0;
			ALTER TABLE [InventProduct] add [ValueSaleWithoutPackEdit] float NULL DEFAULT 0.0;
			ALTER TABLE [InventProduct] add [ValueSaleInPackEdit] float NULL DEFAULT 0.0;"
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 87,
				  Tab = 2,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"ALTER TABLE [InventProduct] add UnityCode nvarchar(50) DEFAULT '' NULL;
			ALTER TABLE [InventProduct] add LocationCode nvarchar(50) DEFAULT '' NULL;
			ALTER TABLE [InventProduct] add Tag nvarchar(100) DEFAULT '' NULL;
			ALTER TABLE [InventProduct] add Tag1 nvarchar(100) DEFAULT '' NULL;
			ALTER TABLE [InventProduct] add Tag2 nvarchar(100) DEFAULT '' NULL;
			ALTER TABLE [InventProduct] add Tag3 nvarchar(100) DEFAULT '' NULL;
			ALTER TABLE [InventProduct] add [QuantityWithoutPackEdit] float NULL DEFAULT 0.0;
			ALTER TABLE [InventProduct] add [ValueBuyDifference] float NULL DEFAULT 0.0;
			ALTER TABLE [InventProduct] add [ValueBuyEdit] float NULL DEFAULT 0.0;
			ALTER TABLE [InventProduct] add [ValueBuyQriginal] float NULL DEFAULT 0.0;
			ALTER TABLE [InventProduct] add [ValueBuyWithoutPackEdit] float NULL DEFAULT 0.0;
			ALTER TABLE [InventProduct] add [ValueBuyInPackEdit] float NULL DEFAULT 0.0;
			ALTER TABLE [InventProduct] add [ValueSaleDifference] float NULL DEFAULT 0.0;
			ALTER TABLE [InventProduct] add [ValueSaleEdit] float NULL DEFAULT 0.0;
			ALTER TABLE [InventProduct] add [ValueSaleQriginal] float NULL DEFAULT 0.0;
			ALTER TABLE [InventProduct] add [ValueSaleWithoutPackEdit] float NULL DEFAULT 0.0;
			ALTER TABLE [InventProduct] add [ValueSaleInPackEdit] float NULL DEFAULT 0.0;"
			  });


			  scripts.Add(new SQLScript
			  {
				  Ver = 88,
				  Tab = 2,
				  DBType = DBName.Count4UDB,
				  Text = @"ALTER TABLE [Itur] ALTER COLUMN [ERPIturCode] nvarchar(250);
			ALTER TABLE [InventProduct] ALTER COLUMN [Makat] nvarchar(300) ;
			ALTER TABLE [InventProduct] ALTER COLUMN [Barcode] nvarchar(300) ;
			ALTER TABLE [InventProduct] ALTER COLUMN [Code] nvarchar(300) ;
			ALTER TABLE [InventProduct] ALTER COLUMN [ERPIturCode] nvarchar(250) ;
			ALTER TABLE [Product] ALTER COLUMN [Code] nvarchar(300) ;
			ALTER TABLE [Product] ALTER COLUMN [Makat] nvarchar(300) ;
			ALTER TABLE [Product] ALTER COLUMN [MakatOriginal] nvarchar(300) ;
			ALTER TABLE [Product] ALTER COLUMN [ParentMakat] nvarchar(300) ;
			ALTER TABLE [Product] ALTER COLUMN [Barcode] nvarchar(300) ;
			ALTER TABLE [Product] ALTER COLUMN [Name] nvarchar(100) ;  "
			  });


			  scripts.Add(new SQLScript
			  {
				  Ver = 88,
				  Tab = 3,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"ALTER TABLE [Itur] ALTER COLUMN [ERPIturCode] nvarchar(250) ;
			ALTER TABLE [InventProduct] ALTER COLUMN [Makat] nvarchar(300) ;
			ALTER TABLE [InventProduct] ALTER COLUMN [Barcode] nvarchar(300) ;
			ALTER TABLE [InventProduct] ALTER COLUMN [Code] nvarchar(300) ;
			ALTER TABLE [InventProduct] ALTER COLUMN [ERPIturCode] nvarchar(250) ;
			ALTER TABLE [Product] ALTER COLUMN [Code] nvarchar(300) ;
			ALTER TABLE [Product] ALTER COLUMN [Makat] nvarchar(300) ;
			ALTER TABLE [Product] ALTER COLUMN [MakatOriginal] nvarchar(300) ;
			ALTER TABLE [Product] ALTER COLUMN [ParentMakat] nvarchar(300) ;
			ALTER TABLE [Product] ALTER COLUMN [Barcode] nvarchar(300) ;
			ALTER TABLE [Product] ALTER COLUMN [Name] nvarchar(100) ;  "
			  });

			  scripts.Add(new SQLScript
			  {
				  Ver = 89,
				  Tab = 2,
				  DBType = DBName.Count4UDB,
				  Text = @"ALTER TABLE [Itur] ALTER COLUMN [ParentIturCode] nvarchar(250);"
			  });


			  scripts.Add(new SQLScript
			  {
				  Ver = 89,
				  Tab = 3,
				  DBType = DBName.EmptyCount4UDB,
				  Text = @"ALTER TABLE [Itur] ALTER COLUMN [ParentIturCode] nvarchar(250);"
			  });
			//================================	   ver in BDini
			  //!!! ALTER TABLE [CurrentInventoryAdvanced] ALTER COLUMN [TemporaryDevice] nvarchar(250) 
			
			int v = PropertiesSettings.DBVer;
			string v1 = @" VALUES (N'" + v.ToString() + @"');";
			//string v1 = @" VALUES (N'23');";
			scripts.Add(new SQLScript
				{
					Ver = v,
					Tab = 220,
					DBType = DBName.MainDB,
					Text = @"INSERT INTO [MainDBIni] ([Ver])" + v1
				});

			scripts.Add(new SQLScript
			{
				Ver = v,
				Tab = 221,
				DBType = DBName.AuditDB,
				Text = @"INSERT INTO [AuditDBIni] ([Ver])" + v1
			});

			scripts.Add(new SQLScript
			{
				Ver = v,
				Tab = 222,
				DBType = DBName.ProcessDB,
				Text = @"INSERT INTO [ProcessDBIni] ([Ver])" + v1
			});

			scripts.Add(new SQLScript
			{
				Ver = v,
				Tab = 223,
				DBType = DBName.Count4UDB,
				Text = @"INSERT INTO [Count4UDBIni] ([Ver])" + v1
			});

			scripts.Add(new SQLScript
			{
				Ver = v,
				Tab = 224,
				DBType = DBName.EmptyCount4UDB,
				Text = @"INSERT INTO [Count4UDBIni] ([Ver])" + v1
			});

			scripts.Add(new SQLScript
			{
				Ver = v,
				Tab = 225,
				DBType = DBName.AnalyticDB,
				Text = @"INSERT INTO [AnalyticDBIni] ([Ver])" + v1
			});

			scripts.Add(new SQLScript
			{
				Ver = v,
				Tab = 226,
				DBType = DBName.EmptyAnalyticDB,
				Text = @"INSERT INTO [AnalyticDBIni] ([Ver])" + v1
			});

			////SQLScripts scriptsCount4U = this._sqlScriptRepository.GetScripts(-5, DBName.Count4UDB)
			//============ ALTER	  / DROP + CREATE Table IturAnalyzes
			scripts.Add(new SQLScript
			{
				Ver = -5,
				Tab = 1,
				DBType = DBName.Count4UDB,
				Text = @"DROP TABLE [IturAnalyzes]"
			});

			scripts.Add(new SQLScript
			{
				Ver = -5,
				Tab = 2,
				DBType = DBName.Count4UDB,
				Text = @"CREATE TABLE [IturAnalyzes] (
  [ID] bigint IDENTITY (1,1) NOT NULL
, [Itur_Disabled] bit NULL
, [Itur_Publishe] bit NULL
, [Itur_StatusIturBit] int DEFAULT 0 NULL
, [Itur_Number] int DEFAULT 0 NOT NULL
, [Itur_NumberPrefix] nvarchar(50) NULL
, [Itur_NumberSufix] nvarchar(50) NULL
, [Itur_LocationCode] nvarchar(50) NULL
, [Itur_StatusIturGroupBit] int DEFAULT 0 NULL
, [Itur_StatusDocHeaderBit] int DEFAULT 0 NULL
, [Doc_Name] nvarchar(50) NULL
, [Doc_Approve] bit NULL
, [Doc_WorkerGUID] nvarchar(50) NULL
, [Doc_StatusDocHeaderBit] int DEFAULT 0 NULL
, [Doc_StatusInventProductBit] int DEFAULT 0 NULL
, [Doc_StatusApproveBit] int DEFAULT 0 NULL
, [PDA_StatusInventProductBit] int DEFAULT 0 NULL
, [Code] nvarchar(50) NULL
, [LocationCode] nvarchar(50) NULL
, [DocumentHeaderCode] nvarchar(50) NULL
, [BarcodeOriginal] nvarchar(300) NULL
, [MakatOriginal] nvarchar(300) NULL
, [PriceString] nvarchar(50) NULL
, [PriceBuy] float DEFAULT 0 NULL
, [PriceSale] float DEFAULT 0 NULL
, [PriceExtra] float DEFAULT 0 NULL
, [DocumentCode] nvarchar(50) NULL
, [IturCode] nvarchar(50) NULL
, [SectionCode] nvarchar(50) NULL
, [SectionName] nvarchar(50) NULL
, [ERPType] int DEFAULT 0 NULL
, [FromCatalogType] int DEFAULT 0 NULL
, [TypeCode] nvarchar(10) NULL
, [SectionNum] int DEFAULT 0 NULL
, [ValueBuyDifference] float DEFAULT 0 NULL
, [QuantityDifference] float DEFAULT 0 NULL
, [ValueBuyEdit] float DEFAULT 0 NULL
, [QuantityEdit] float DEFAULT 0 NULL
, [ValueBuyQriginal] float DEFAULT 0 NULL
, [QuantityOriginal] float DEFAULT 0 NULL
, [Barcode] nvarchar(300) NULL
, [Makat] nvarchar(300) NULL
, [ProductName] nvarchar(100) NULL
, [ModifyDate] datetime DEFAULT GETDATE ( ) NULL
, [SerialNumber] nvarchar(50) NULL
, [ShelfCode] nvarchar(100) NULL
, [TypeMakat] nvarchar(10) NULL
, [InputTypeCode] nvarchar(100) NULL
, [IPNum] int DEFAULT 1 NULL
, [DocNum] int DEFAULT 1 NULL
, [PDA_ID] bigint DEFAULT 1 NULL
, [Count] int DEFAULT 0 NULL
, [ValueChar] nvarchar(100) NULL
, [ValueInt] int DEFAULT 0 NULL
, [ValueFloat] float DEFAULT 0 NULL
, [IsResulte] bit NULL
, [ResultCode] nvarchar(50) NULL
, [ResulteDescription] nvarchar(100) NULL
, [IsUpdateERP] bit NULL
, [ImputTypeCodeFromPDA] nvarchar(50) NULL
, [ResulteValue] nvarchar(50) NULL
, [QuantityOriginalERP] float DEFAULT 0 NULL
, [ValueOriginalERP] float DEFAULT 0 NULL
, [QuantityDifferenceOriginalERP] float DEFAULT 0 NULL
, [ValueDifferenceOriginalERP] float DEFAULT 0 NULL
, [SupplierCode] nvarchar(50) NULL
, [SupplierName] nvarchar(100) NULL
, [LocationName] nvarchar(100) NULL
, [IturName] nvarchar(100) NULL
, [SessionCode] nvarchar(50) NULL
, [SessionNum] int DEFAULT 1 NULL
, [Currency] nvarchar(10) NULL
, [ERPIturCode] nvarchar(250) NULL
, [CountInParentPack] int DEFAULT 1 NULL
, [BalanceQuantityPartialERP] int DEFAULT 0 NULL
, [QuantityInPackEdit] int DEFAULT 0 NULL
, [IPValueStr1] nvarchar(100) DEFAULT '' NULL
, [IPValueStr2] nvarchar(100) DEFAULT '' NULL
, [IPValueStr3] nvarchar(100) DEFAULT '' NULL
, [IPValueStr4] nvarchar(100) DEFAULT '' NULL
, [IPValueStr5] nvarchar(100) DEFAULT '' NULL
, [IPValueStr6] nvarchar(100) DEFAULT '' NULL
, [IPValueStr7] nvarchar(100) DEFAULT '' NULL
, [IPValueStr8] nvarchar(100) DEFAULT '' NULL
, [IPValueStr9] nvarchar(100) DEFAULT '' NULL
, [IPValueStr10] nvarchar(100) DEFAULT '' NULL
, [IPValueFloat1] float DEFAULT 0 NULL
, [IPValueFloat2] float DEFAULT 0 NULL
, [IPValueFloat3] float DEFAULT 0 NULL
, [IPValueFloat4] float DEFAULT 0 NULL
, [IPValueFloat5] float DEFAULT 0 NULL
, [IPValueInt1] int DEFAULT 0 NULL
, [IPValueInt2] int DEFAULT 0 NULL
, [IPValueInt3] int DEFAULT 0 NULL
, [IPValueInt4] int DEFAULT 0 NULL
, [IPValueInt5] int DEFAULT 0 NULL
, [IPValueBit1] bit DEFAULT 0 NULL
, [IPValueBit2] bit DEFAULT 0 NULL
, [IPValueBit3] bit DEFAULT 0 NULL
, [IPValueBit4] bit DEFAULT 0 NULL
, [IPValueBit5] bit DEFAULT 0 NULL
, [Price] float DEFAULT 0 NULL
, [WorkerID] nvarchar(50) DEFAULT '' NULL
, [WorkerName] nvarchar(100) DEFAULT '' NULL
, [Total] bigint DEFAULT 0 NULL
, [FromTime] datetime DEFAULT GETDATE ( ) NULL
, [ToTime] datetime DEFAULT GETDATE ( ) NULL
, [TicksTimeSpan] bigint DEFAULT 0 NULL
, [PeriodFromTo] nvarchar(100) DEFAULT '' NULL
, [FamilyCode] nvarchar(50) DEFAULT '' NULL
, [FamilyName] nvarchar(50) DEFAULT '' NULL
, [FamilyType] nvarchar(50) DEFAULT '' NULL
, [FamilySize] nvarchar(50) DEFAULT '' NULL
, [FamilyExtra1] nvarchar(50) DEFAULT '' NULL
, [FamilyExtra2] nvarchar(50) DEFAULT '' NULL
, [UnitTypeCode] nvarchar(50) DEFAULT '' NULL
, [InventorCode] nvarchar(100) DEFAULT '' NULL
, [InventorName] nvarchar(100) DEFAULT '' NULL
, [BranchCode] nvarchar(100) DEFAULT '' NULL
, [BranchName] nvarchar(100) DEFAULT '' NULL
, [BranchERPCode] nvarchar(50) DEFAULT '' NULL
, [InventorDate] datetime DEFAULT GETDATE ( ) NULL
, [IturCodeExpected] nvarchar(50) DEFAULT '' NULL
, [IturCodeDiffer] bit DEFAULT 0 NULL
, [IPValueStr11] nvarchar(100) DEFAULT '' NULL
, [IPValueStr12] nvarchar(100) DEFAULT '' NULL
, [IPValueStr13] nvarchar(100) DEFAULT '' NULL
, [IPValueStr14] nvarchar(100) DEFAULT '' NULL
, [IPValueStr15] nvarchar(100) DEFAULT '' NULL
, [IPValueStr16] nvarchar(100) DEFAULT '' NULL
, [IPValueStr17] nvarchar(100) DEFAULT '' NULL
, [IPValueStr18] nvarchar(100) DEFAULT '' NULL
, [IPValueStr19] nvarchar(100) DEFAULT '' NULL
, [IPValueStr20] nvarchar(100) DEFAULT '' NULL
, [SubSessionCode] nvarchar(50) DEFAULT '' NULL
, [SessionName] nvarchar(100) DEFAULT '' NULL
, [SubSessionName] nvarchar(100) DEFAULT '' NULL
);"
			});
			scripts.Add(new SQLScript
			{
				Ver = -5,		 //SQLScripts scriptsCount4U = this._sqlScriptRepository.GetScripts(-5, DBName.Count4UDB)
				Tab = 4,
				DBType = DBName.Count4UDB,
				Text = @"ALTER TABLE [IturAnalyzes] ADD CONSTRAINT [PK__IturAnalyzes__00000000000002D0] PRIMARY KEY ([ID]);"
			});

			scripts.Add(new SQLScript
			{
				Ver = -5,
				Tab = 5,
				DBType = DBName.Count4UDB,
				Text = @"ALTER TABLE [IturAnalyzes] ADD CONSTRAINT [UQ__IturAnalyzes__0000000000000703] UNIQUE ([ID]);"
			});

			//================================

			return scripts;
		}

		public SQLScripts GetScriptsInsertVerDB(int v = 22)
		{
			SQLScripts scripts = new SQLScripts();
			string v1 = @" VALUES (N'" + v.ToString() + @"');";
			scripts.Add(new SQLScript
				{
					Ver = v,
					Tab = 221,
					DBType = DBName.MainDB,
					Text = @"INSERT INTO [MainDBIni] ([Ver])" + v1
				});

			scripts.Add(new SQLScript
			{
				Ver = v,
				Tab = 222,
				DBType = DBName.AuditDB,
				Text = @"INSERT INTO [AuditDBIni] ([Ver])" + v1
			});

			scripts.Add(new SQLScript
			{
				Ver = v,
				Tab = 223,
				DBType = DBName.Count4UDB,
				Text = @"INSERT INTO [Count4UDBIni] ([Ver])" + v1
			});

			scripts.Add(new SQLScript
			{
				Ver = v,
				Tab = 224,
				DBType = DBName.EmptyCount4UDB,
				Text = @"INSERT INTO [Count4UDBIni] ([Ver])" + v1
			});
			return scripts;
		}

		public SQLScripts GetScripts(int oldVer, int newVer)
		{
			var scr = SQLScripts.FromEnumerable(this.GetScripts().ToList()
				.Where(x => x.Ver > oldVer && x.Ver <= newVer)
				.OrderBy(x => x.Tab).Select(x => x));
			return scr;
		}

		public SQLScripts GetScripts(int ver)
		{
			var scr = SQLScripts.FromEnumerable(this.GetScripts().ToList()
				.Where(x => x.Ver == ver)
				.OrderBy(x => x.Tab).Select(x => x));
			return scr;
		}

		public SQLScripts GetScripts(int ver, string dbName)
		{
	
			var scr = SQLScripts.FromEnumerable(this.GetScripts().ToList()
				.Where(x => x.Ver == ver && x.DBType == dbName)
				.OrderBy(x => x.Tab).Select(x => x));
			return scr;
		}

		public void CreateReport()
		{
			//string deviceInfo = null;
			//string format = "PDF";

			LocalReport lr = new LocalReport();
			//string deviceInfo = "<DeviceInfo>" +
			// "  <OutputFormat>PDF</OutputFormat>" +
			// "  <PageWidth>8.5in</PageWidth>" +
			// "  <PageHeight>11.5in</PageHeight>" +
			// "  <MarginTop>0.6in</MarginTop>" +
			// "  <MarginLeft>0.6in</MarginLeft>" +
			// "  <MarginRight>0.4in</MarginRight>" +
			// "  <MarginBottom>0.4in</MarginBottom>" +
			// "</DeviceInfo>";

			 
			//deviceInfo =
			//  "<DeviceInfo>" +
			//  "<SimplePageHeaders>True</SimplePageHeaders>" +
			//  "</DeviceInfo>";

			lr.ReportPath = @"C:\Count4U\trunk\Count4U\Count4U.Model\Report1.rdlc";

			lr.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("SQLScript", this.GetScripts())); //

			//byte[] bytes = lr.Render(format, deviceInfo);

			//using (FileStream fs = new FileStream(@"C:\Count4U\trunk\Count4U\Count4U.Model\Report1.pdf", FileMode.Create))
			//{
			//    fs.Write(bytes, 0, bytes.Length);
			//}

			//
			//LocalReport report = new LocalReport();
			//report.ReportPath = "Report.rdlc";
			//report.DataSources.Add(new ReportDataSource("Sales", LoadSalesData()));

			Export(lr);

		}

		#endregion
		private Stream CreateStream(string name, string fileNameExtension, 
		   Encoding encoding,string mimeType, bool willSeek)
    {
        Stream stream = new FileStream(name + "." + fileNameExtension, 
          FileMode.Create);
        //m_streams.Add(stream);
        return stream;
    }

		private void Export(Microsoft.Reporting.WinForms.LocalReport report)
		{
			string deviceInfo =
			  "<DeviceInfo>" +
			  "  <OutputFormat>PDF</OutputFormat>" +
			  "  <PageWidth>8.5in</PageWidth>" +
			  "  <PageHeight>11in</PageHeight>" +
			  "  <MarginTop>0.25in</MarginTop>" +
			  "  <MarginLeft>0.25in</MarginLeft>" +
			  "  <MarginRight>0.25in</MarginRight>" +
			  "  <MarginBottom>0.25in</MarginBottom>" +
			  "</DeviceInfo>";
			Microsoft.Reporting.WinForms.Warning[] warnings;
			//m_streams = new List<Stream>();
			report.Render("Image", deviceInfo, CreateStream, out warnings);

			//foreach (Stream stream in m_streams)
			//    stream.Position = 0;
		}


	}
}
