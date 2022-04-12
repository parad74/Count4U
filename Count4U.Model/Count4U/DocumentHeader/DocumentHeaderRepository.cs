using System;
using System.Linq;
using Count4U.Model.Extensions;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Model.Count4U.Mapping;
using System.Collections.Generic;

namespace Count4U.Model.Count4U
{
    public class DocumentHeaderRepository : IDocumentHeaderRepository
    {
		private Dictionary<string, DocumentHeader> _documentHeaderDictionary;
        private DocumentHeaders _documentHeaderList;

        #region IDocumentHeaderRepository Members

        public DocumentHeaders GetDocumentHeaders( string pathDB)
        {
            if (this._documentHeaderList == null)
			{
                this._documentHeaderList = new DocumentHeaders {
                    new DocumentHeader() { Code = "DocumentHeaderCode1", DocumentCode = "DocumentHeaderDocumentCode1", Name = "DocumentHeader1", IturCode = "Itur1" },
                    new DocumentHeader() { Code = "DocumentHeaderCode2", DocumentCode = "DocumentHeaderDocumentCode2", Name = "DocumentHeader2", IturCode = "Itur1" },
                    new DocumentHeader() { Code = "DocumentHeaderCode3", DocumentCode = "DocumentHeaderDocumentCode3", Name = "DocumentHeader3", IturCode = "Itur2" },
                    new DocumentHeader() { Code = "DocumentHeaderCode4", DocumentCode = "DocumentHeaderDocumentCode4", Name = "DocumentHeader4", IturCode = "Itur2" },
                    new DocumentHeader() { Code = "DocumentHeaderCode5", DocumentCode = "DocumentHeaderDocumentCode5", Name = "DocumentHeader5", IturCode = "Itur3" },
                    new DocumentHeader() { Code = "DocumentHeaderCode6", DocumentCode = "DocumentHeaderDocumentCode6", Name = "DocumentHeader6", IturCode = "Itur3" },
                };
            }
            return this._documentHeaderList;
        }

		public DocumentHeaders GetDocumentHeaders(SelectParams selectParams, string pathDB)
        {
            throw new NotImplementedException();
        }

		public DocumentHeaders GetDocumentHeadersByItur(Itur itur, string pathDB)
        {
			return this.GetDocumentHeadersByIturCode(itur.IturCode, pathDB);
        }

		public DocumentHeaders GetDocumentHeadersByIturCode(string iturCode, string pathDB)
        {
			var domainObjects = this.GetDocumentHeaders(pathDB).Where(e => e.IturCode == iturCode)
                                                    .ToList().Select(e => e.ToDomainObject());
            return DocumentHeaders.FromEnumerable(domainObjects);
        }

		public DocumentHeaders GetDocumentHeadersBySession(Session session, string pathDB)
        {
			return this.GetDocumentHeadersBySessionCode(session.SessionCode, pathDB);
        }

		public DocumentHeaders GetDocumentHeadersBySessionCode(string sessionCode, string pathDB)
        {
			var domainObjects = this.GetDocumentHeaders(pathDB).Where(e => e.SessionCode == sessionCode)
                                                    .ToList().Select(e => e.ToDomainObject());
            return DocumentHeaders.FromEnumerable(domainObjects);
        }

		public DocumentHeaders GetDocumentHeadersByStatusCode(string statusCode, string pathDB)
        {
			var domainObjects = this.GetDocumentHeaders(pathDB).Where(e => e.StatusDocHeaderCode.CompareTo(statusCode) == 0)
                                                    .ToList().Select(e => e.ToDomainObject());
            return DocumentHeaders.FromEnumerable(domainObjects);
        }

		public void Delete(DocumentHeader documentHeader, string pathDB)
        {
			this.Delete(documentHeader.Code, pathDB);
        }

		public void Delete(string documentCode, string pathDB)
        {
			var entity = this.GetEntityByDocumentCode(documentCode, pathDB);
			if (entity == null) return;
			this.GetDocumentHeaders(pathDB).Remove(entity);
        }

		public void DeleteAllByItur(Itur itur, string pathDB)
        {
			this.DeleteAllByIturCode(itur.IturCode, pathDB);
        }

		public void DeleteAllByIturCode(string iturCode, string pathDB)
        {
			this.GetDocumentHeaders(pathDB).RemoveAll(e => e.IturCode == iturCode);
        }

		public void Insert(Itur itur, DocumentHeader documentHeader, string pathDB)
        {
			if (itur == null) return ;
			if (documentHeader == null) return ;
            var entity = documentHeader.ToEntity();
            entity.IturCode = itur.IturCode;
            entity.Itur = itur.Name;
			this.GetDocumentHeaders(pathDB).Add(entity);
        }

		public void Update(DocumentHeader documentHeader, string pathDB)
        {
			if (documentHeader == null) return;
			var entity = this.GetEntityByDocumentCode(documentHeader.DocumentCode, pathDB);
			if (entity == null) return;
            entity.ApplyChanges(documentHeader);
        }

		public DocumentHeader GetDocumentHeaderByCode(string code, string pathDB)
		{
			var entity = this.GetEntityByDocumentCode(code, pathDB);
			if (entity == null) return null;
            return entity.ToDomainObject();
		}

		public void Insert(Itur itur, DocumentHeaders documentHeaders, string pathDB)
		{
			documentHeaders.ToList().ForEach(e => Insert(itur, e, pathDB));
		}



		#endregion

        #region private

		private DocumentHeader GetEntityByCode(string code, string pathDB)
        {
			var entity = this.GetDocumentHeaders(pathDB).First(e => e.Code.CompareTo(code) == 0);
            return entity;
        }

		private DocumentHeader GetEntityByDocumentCode(string documentCode, string pathDB)
		{
			var entity = this.GetDocumentHeaders(pathDB).First(e => e.DocumentCode.CompareTo(documentCode) == 0);
			return entity;
		}

        #endregion

		#region IDocumentHeaderRepository Members


		public System.Collections.BitArray GetResultInventProductStatusBitOrByDocumentCode(string documentCode, string pathDB, bool refill = false)
		{
			throw new NotImplementedException();
		}

		public int GetResultInventProductStatusIntOrByDocumentCode(string documentCode, string pathDB, bool refill = false)
		{
			throw new NotImplementedException();
		}

		public int RefillDocHeaderStatusBitByDocumenCode(string documentCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		public void RefillStatusBit(string pathDB)
		{
			throw new NotImplementedException();
		}

		public void ClearStatusBit(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IDocumentHeaderRepository Members


		public Dictionary<string, DocumentHeader> GetDocumentHeaderDictionary(string pathDB, bool refill = false)
		{
			throw new NotImplementedException();
		}

		public void FillDocumentHeaderDictionary(string pathDB)
		{
			throw new NotImplementedException();
		}

		public void ClearDocumentHeaderDictionary()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IDocumentHeaderRepository Members


		public long GetCountDocumentWithError(List<string> documentCodes, string pathDB)
		{
			throw new NotImplementedException();
		}

		public long GetCountDocumentWithoutError(List<string> documentCodes, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IDocumentHeaderRepository Members


		public List<string> GetIturCodeList(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IDocumentHeaderRepository Members


		public long Insert(DocumentHeader documentHeader, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IDocumentHeaderRepository Members


		public List<string> GetDocumentCodeList(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		

		#region IDocumentHeaderRepository Members


		public void RefillDocumentStatistic(List<string> docCodes, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IDocumentHeaderRepository Members


		public List<string> GetIturCodeList(System.Data.Entity.Core.Objects.ObjectSet<App_Data.DocumentHeader> documentHeaders)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IDocumentHeaderRepository Members


		public List<string> GetDocumentHeaderCodeList(IEnumerable<App_Data.DocumentHeader> documentHeaders)
		{
			throw new NotImplementedException();
		}

		#endregion


		public void Insert(DocumentHeaders documentHeaders, string pathDB)
		{
			throw new NotImplementedException();
		}


		public void RefillDocumentStatisticBySession(List<string> sessionCodeList, string pathDB)
		{
			throw new NotImplementedException();
		}


		public Dictionary<string, DocumentHeader> GetIturDictionaryMaxDateTime(string pathDB)
		{
			throw new NotImplementedException();
		}

		public Dictionary<string, DocumentHeader> GetIturDocumentCodeDictionary(string pathDB)
		{
			throw new NotImplementedException();
		}


		#region IDocumentHeaderRepository Members


		public void DeleteAllDocumentsWithoutAnyInventProduct(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IDocumentHeaderRepository Members


		public void SetNullToApproveDocuments(List<string> documentCodes, string pathDB)
		{
			throw new NotImplementedException();
		}

		public void UpdateWorkerName(string deviceName, string oldWorkerName, string newWorkerName, string pathDB)
		{
			throw new NotImplementedException();
		}

		public void RefillIturStatistic(List<string> iturCodes, List<string> docCodes, string pathDB)
		{
			throw new NotImplementedException();
		}

		public void RefillIturStatistic(List<string> sessionCodeList, List<string> iturCodes, List<string> docCodes, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
