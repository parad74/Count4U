using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Common;
using Count4U.Model.Interface.Count4Mobile;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Count4U
{
	public class InventProductUpdateDBParser4 : InventProductParserBase, IInventProductSimpleParser
	{
		protected Dictionary<string, DocumentHeader> _iturInFileDictionary; //key IturCode, DocumentHeader 
		private readonly IInventProductRepository _inventProductRepository;
		protected DocumentHeaderString _rowDocumentHeader;

		public InventProductUpdateDBParser4(
			IInventProductRepository inventProductRepository,
			IDocumentHeaderRepository documentHeaderRepository,
			IServiceLocator serviceLocator,
			ILog log) :
			base(documentHeaderRepository, serviceLocator, log)
		{
			if (inventProductRepository == null) throw new ArgumentNullException("iturRepository");

			this._iturInFileDictionary = new Dictionary<string, DocumentHeader>();
			this._rowDocumentHeader = new DocumentHeaderString();
			this._inventProductRepository = inventProductRepository;
		}

		/// <summary>
		/// Получение списка InventProduct  
		/// </summary>
		/// <returns></returns>
		public IEnumerable<InventProduct> GetInventProducts(
		string fromPathFile,
		Encoding encoding, string[] separators,
		int countExcludeFirstString, string sessionCodeIn, //Guid workerGUID,
		Dictionary<string, ProductMakat> productMakatDictionary,
		Dictionary<string, Itur> iturFromDBDictionary,
		List<ImportDomainEnum> importType,
		Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			//IPreviousInventoryRepository _previousInventoryEFRepository = this._serviceLocator.GetInstance<IPreviousInventoryRepository>();
			//Dictionary<string, PreviousInventory> dictionaryPreviousInventory = _previousInventoryEFRepository.GetDictionaryPreviousInventorys(fromPathFile);
			string dbPath = parms.GetStringValueFromParm(ImportProviderParmEnum.DBPath);
			string newSessionCode = parms.GetStringValueFromParm(ImportProviderParmEnum.NewSessionCode);

			//ProductMakat productMakat = productMakatDictionary[];

			this._documentHeaderDictionary.Clear();
			this._iturDictionary.Clear();
			this._errorBitList.Clear();
			this._iturInFileDictionary.Clear();

			IDocumentHeaderRepository documentHeaderRepository = this._serviceLocator.GetInstance<IDocumentHeaderRepository>();
			this._iturInFileDictionary = documentHeaderRepository.GetIturDocumentCodeDictionary(dbPath);

			IIturRepository iturRepository = this._serviceLocator.GetInstance<IIturRepository>();
			Dictionary<string, Itur> dictionaryIturCodeERP = new Dictionary<string, Itur>();
			try
			{
				// count4UDB
				dictionaryIturCodeERP = iturRepository.GetERPIturDictionary(dbPath);
			}
			catch { }

			//IEnumerable<Product> productFromDBSimples = this._productRepository.GetProducts(dbPath);
			////this._productRepository.DeleteAll(dbPath);
			//IImportCatalogADORepository provider = ServiceLocator.GetInstance<IImportCatalogADORepository>();
			//	IImportCatalogADORepository provider = ServiceLocator.GetInstance<IImportCatalogADORepository>();

			int kk = 0;

			ITemporaryInventoryRepository temporaryInventoryRepository = this._serviceLocator.GetInstance<ITemporaryInventoryRepository>();
			//temporaryInventoryRepository.FillKeysNewUidTemporaryInventorys(dbPath, "InventProduct");
			//проверить, что отсортирован по дате по убыванию
			List<TemporaryInventory> temporaryInventorysInventProduct = temporaryInventoryRepository.GetTemporaryInventorysInventProduct(dbPath);

			//InventProducts inventProducts1 = this._inventProductRepository.GetInventProducts(dbPath);
			//var inventProductFromDBDictionry = inventProducts1.Select(e => e).ToDictionary(k => k.Barcode);
			//InventProducts inventProductFromDB = this._inventProductRepository.GetInventProducts(dbPath);
			InventProducts inventProductAddToDB = new InventProducts();
			//InventProducts inventProductToDB = new InventProducts();

			//foreach (var newInventProduct in inventProductFromDB)
			//{
			//	inventProductToDB.Add(newInventProduct);
			//}

			//заполнить словарь из TempararyTable
			//ищем barcode в newUID 
			//Dictionary<string, TemporaryInventory> dictionaryTemporaryDeleteByNewUidInventorys =
			//	temporaryInventoryRepository.GetDictionaryDeletedByNewUidTemporaryInventorys(dbPath, "InventProduct");
			//for (int j = 0; j < 7; j++)
			//{
				//Log.Add(MessageTypeEnum.TraceParser, "Loop :: j = " + j);

				//IImportInventProductRepository importInventProductRepository = this._serviceLocator.GetInstance<IImportInventProductRepository>();
				//importInventProductRepository.ClearInventProducts(dbPath);

				
				int k = 0;
				foreach (var oldTemporaryInventory in temporaryInventorysInventProduct)
				{

					InventProduct newInventProduct = new InventProduct();
					InventProduct recoverInventProduct = Recover(oldTemporaryInventory.OldUid, oldTemporaryInventory.NewUid, new InventProduct(), "");

					//=============================
					if (importType.Contains(ImportDomainEnum.ExistMakat) == true)
					{
						if (productMakatDictionary.ContainsKey(recoverInventProduct.Makat) == true)
						{
							recoverInventProduct.TypeMakat = TypeMakatEnum.M.ToString();
							recoverInventProduct.FromCatalogType = (int)FromCatalogTypeEnum.CatalogMakatOrBarcod;
							recoverInventProduct.ProductName = productMakatDictionary[recoverInventProduct.Makat].Name.CutLength(99);
							recoverInventProduct.SectionCode = productMakatDictionary[recoverInventProduct.Makat].SectionCode.CutLength(49);
							recoverInventProduct.SupplierCode = productMakatDictionary[recoverInventProduct.Makat].SupplierCode.CutLength(49);
							recoverInventProduct.ImputTypeCodeFromPDA = productMakatDictionary[recoverInventProduct.Makat].UnitTypeCode;
						}
						else
						{	// TODO: проверить
							recoverInventProduct.TypeMakat = TypeMakatEnum.W.ToString();
							recoverInventProduct.ProductName = "NotExistInCatalog";
							recoverInventProduct.StatusInventProductBit += (int)ConvertDataErrorCodeEnum.InvalidValue;
							recoverInventProduct.FromCatalogType = (int)FromCatalogTypeEnum.InventProductWithoutMakat;
						}
					}		//ExistMakat
					else   //Not ExistMakat
					{
						recoverInventProduct.ProductName = "NotCheckInCatalog";
					}

					// Nativ+ потеряли при MOVE Q
					if (recoverInventProduct.ImputTypeCodeFromPDA == "Q")	 //2. Check if itemCode type = SN
					{
						Itur currentItur = dictionaryIturCodeERP[recoverInventProduct.ERPIturCode];
						recoverInventProduct.ERPIturCode = currentItur.ERPIturCode;

						string currentIturCode = currentItur.IturCode;
						int currentIturInvStatus = currentItur.InvStatus;
						recoverInventProduct.IturCode = currentIturCode;

						string locationCode = DomainUnknownCode.UnknownLocation;
						locationCode = currentItur.LocationCode;

						//словарь Iturs в текущем файле. Создаем для каждого Itur:File один DocumentHeader
						int IDDocumentHeader = 0;
						string currentDocumentCode = GetDocumentHeaderCodeByIturCode(ref IDDocumentHeader, dbPath, currentIturCode, currentIturInvStatus, newSessionCode);

						recoverInventProduct.DocumentCode = currentDocumentCode;
						recoverInventProduct.DocumentHeaderCode = currentDocumentCode;
						recoverInventProduct.SessionCode = newSessionCode;
						recoverInventProduct.SectionNum = 0;
						recoverInventProduct.DocNum = Convert.ToInt32(IDDocumentHeader);
						DateTime dt = DateTime.Now;
						recoverInventProduct.ModifyDate = dt;
						recoverInventProduct.CreateDate = dt;

						//=========================================================
						inventProductAddToDB.Add(recoverInventProduct);
						k++;
						//first.Tag = "Recover";
						//first.NewProductCode = first.OldUid;

						//TemporaryInventory udated = temporaryInventorysInventProduct.FirstOrDefault(x => x.Description == first.Description);
						//if (udated != null) temporaryInventorysInventProduct.Remove(udated);
						//temporaryInventoryRepository.Update(first, dbPath);
						//InventProduct ip = new InventProduct();
						//inventProductFromDB.Add(new InventProduct());
					}
					else
					{
						string unit = recoverInventProduct.ImputTypeCodeFromPDA;
					}

					//ищем barcode в newUID 
					//if (dictionaryTemporaryDeleteByNewUidInventorys.ContainsKey(newInventProduct.Barcode) == true)
					//{
					//	TemporaryInventory temporaryInventory = dictionaryTemporaryDeleteByNewUidInventorys[newInventProduct.Barcode];
					//	string oldUID = temporaryInventory.OldUid;
					//ВОССТАНАВЛИВАЕМ oldUID (newInventproduct) клонируя текущий и заменяя все что касается
					// oldUID (SN, LocationCode , and s o)
					//TAG заполняет данными о восстановлении чтобы их можно было отобрать
					// IturCode, ERPIturCode, DocumentHeader ?? он единственный в Nariv + 
					//	InventProduct recoverInventProduct = Recover(oldUID);
					//}
					//Log.Add(MessageTypeEnum.TraceParser, "Create New IP " + "[" + k + "]");
					//}
				}
								

				//inventProductToDB.Clear();
				//foreach (var newInventProduct in inventProductAddToDB)
				//{
				//	inventProductAddToDB.Add(newInventProduct);
				//}
			//}

	
			//очистить InventProduct
			IImportInventProductRepository importInventProductRepository1 = this._serviceLocator.GetInstance<IImportInventProductRepository>();
		//	importInventProductRepository1.ClearInventProducts(dbPath);

			foreach (var newInventProduct in inventProductAddToDB)
			{
				kk++;
				Log.Add(MessageTypeEnum.TraceParser, "Recover" + kk + ":" + newInventProduct.IPValueStr5 + "->" + newInventProduct.Barcode);
				yield return newInventProduct;
			}

			Log.Add(MessageTypeEnum.TraceParser, "Update InventProduct [4] :: k = " + kk);
		}


		//ВОССТАНАВЛИВАЕМ oldUID (newInventproduct) клонируя текущий и заменяя все что касается
		// oldUID (SN, LocationCode , and s o)
		//TAG заполняет данными о восстановлении чтобы их можно было отобрать
		// IturCode, ERPIturCode, DocumentHeader ?? он единственный в Nariv + 
		public InventProduct Recover(string oldUID, string newUID, InventProduct newInventProduct, string comment)
		{
			// Nativ +
			//SerialNumberLocal|ItemCode|LocationCode|PropertyStr13
			InventProduct recoverIP = new InventProduct(newInventProduct);
			string[] keys = SplitUID(oldUID);
			recoverIP.SerialNumber = keys[0];
			recoverIP.Makat = keys[1];
			recoverIP.Barcode = oldUID;
			recoverIP.ERPIturCode = keys[2];
			recoverIP.IPValueStr13 = keys[3];
			recoverIP.IPValueStr3 = "Recover" + comment  + " :  "  + newUID;
			recoverIP.IPValueStr10 = "Recover" + comment;
			recoverIP.IPValueStr5 = newUID;
			recoverIP.QuantityEdit = 1;
			recoverIP.QuantityOriginal = 1;
			recoverIP.InputTypeCode = InputTypeCodeEnum.B.ToString();
			recoverIP.Code = oldUID;
			recoverIP.ProductName = "NotExistInCatalog";
			recoverIP.ImputTypeCodeFromPDA = "Q";
			newInventProduct.WorkerID = "Recover" + comment;
			//if (recoverIP.Makat == newInventProduct.Makat) recoverIP.ProductName = newInventProduct.ProductName;

			return recoverIP;
		}

	

		public bool IsRecover(string IPValueStr10, int j)
		{
			if (j == 0) return true;
			if (j > 0)
			{
				string n = IPValueStr10.Replace("Recover", "");
				int step = 0;
				bool ret = Int32.TryParse(n.Trim(), out  step);
				if (ret == true)
				{
					if ((step + 1) == j) return true;
				}
				return false;
			}
			return false;
		}


		private string[] SplitUID(string newUid)
		{
			string[] keys = newUid.Split('|');
			string[] keysEmpty = { "", "", "", "" };
			int count = Math.Min(keys.Length, 4);
			for (var k = 0; k < count; k++)
			{
				keysEmpty[k] = keys[k];
			}
			return keysEmpty;
		}

		private string GetDocumentHeaderCodeByIturCode(
		ref int IDDocumentHeader,
		string dbPath,
		string newIturCode,	 //ожидается IturCode
		int currentIturInvStatus,
		string newSessionCode)
		{
			string retDocumentCode = "";
			//if (iturFromDBDictionary.ContainsKey(newIturCode) == false) return retDocumentCode;
			// есть   DocumentHeader в Itur
			if (this._iturInFileDictionary.ContainsKey(newIturCode) == true) //словарь Iturs в текущем файле. Создаем для каждого Itur:File один DocumentHeader
			{
				DocumentHeader document = this._iturInFileDictionary[newIturCode];
				if (document != null) IDDocumentHeader = Convert.ToInt32(document.ID);
				retDocumentCode = document.DocumentCode;

				if (currentIturInvStatus == 1 && document.Approve != false)			//if (currentIturInvStatus == 1) newDocumentHeader.Approve = false;			
				{
					document.Approve = false;
					base._documentHeaderRepository.Update(document, dbPath);
				}
				else if (currentIturInvStatus == 2 && document.Approve != true)	//else if (currentIturInvStatus == 2) newDocumentHeader.Approve = true;		
				{
					document.Approve = true;
					base._documentHeaderRepository.Update(document, dbPath);
				}
				return retDocumentCode;
			}
			//========================================DocumentHeader==================
			else // create new DocumentHeader
			{
				string newDocumentCode = Guid.NewGuid().ToString(); // предполагается несколько документов в файле

				DocumentHeaderString newDocumentHeaderString = new DocumentHeaderString();
				DocumentHeader newDocumentHeader = new DocumentHeader();
				newDocumentCode = Guid.NewGuid().ToString(); // предполагается несколько документов в файле
				newDocumentHeaderString.DocumentCode = newDocumentCode;
				newDocumentHeaderString.SessionCode = newSessionCode;				//in
				newDocumentHeaderString.CreateDate = this._rowDocumentHeader.CreateDate;
				newDocumentHeaderString.WorkerGUID = "UnknownWorker";
				newDocumentHeaderString.IturCode = newIturCode;
				newDocumentHeaderString.Name = this._rowDocumentHeader.Name;
				newDocumentHeaderString.WorkerGUID = this._rowDocumentHeader.WorkerGUID;

				int retBitDocumentHeader = newDocumentHeader.ValidateError(newDocumentHeaderString, this._dtfi);
				if (retBitDocumentHeader != 0)  //Error
				{
					this._errorBitList.Add(new BitAndRecord { Bit = retBitDocumentHeader, Record = this._rowDocumentHeader.Name, ErrorType = MessageTypeEnum.Error });
				}
				else //	Error  retBitSession == 0 
				{
					retBitDocumentHeader = newDocumentHeader.ValidateWarning(newDocumentHeaderString, this._dtfi); //Warning
					newDocumentHeader.Approve = null;
					if (currentIturInvStatus == 1) newDocumentHeader.Approve = false; //first Document in Itur		  //currentIturInvStatus == 1
					else if (currentIturInvStatus == 2) newDocumentHeader.Approve = true;
					IDDocumentHeader = Convert.ToInt32(base._documentHeaderRepository.Insert(newDocumentHeader, dbPath));
					newDocumentHeader.ID = IDDocumentHeader;
					retDocumentCode = newDocumentCode;
					this._iturInFileDictionary[newIturCode] = newDocumentHeader; //словарь IturCode -> DocumentHeader. Создаем для каждого Itur только один DocumentHeader

					if (retBitDocumentHeader != 0)
					{
						this._errorBitList.Add(new BitAndRecord { Bit = retBitDocumentHeader, Record = this._rowDocumentHeader.Name, ErrorType = MessageTypeEnum.WarningParser });
					}
				}
				return retDocumentCode;
			}

		}


	}
}
