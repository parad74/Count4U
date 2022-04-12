using System.Collections.Generic;
using System.Threading;
using Count4U.Model;
using Count4U.Model.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Audit;
using Count4U.Model.Interface.Audit;

namespace Count4U.Modules.Audit.ViewModels
{
    public class InventProductUtils
    {
		public static void BuildAnalyzeTableSimple(IIturAnalyzesSourceRepository iturAnalyzesSourceRepository,
			IIturAnalyzesRepository iturAnalyzesRepository,
            CancellationTokenSource cts,
			string dbPath,
			Inventor currentInventor)
        {
            SelectParams sp = new SelectParams();

            Dictionary<object, object> dic = new Dictionary<object, object>();
            dic[ImportProviderParmEnum.CancellationToken] = cts.Token;
			dic[ImportProviderParmEnum.PriceCode] = PriceCodeEnum.PriceBuy.ToString();
			if (currentInventor != null)
			{
				if (string.IsNullOrWhiteSpace(currentInventor.PriceCode) == false)
				{
					dic[ImportProviderParmEnum.PriceCode] = currentInventor.PriceCode;
					dic[ImportProviderParmEnum.FromBuildAnalyze] = "BuildAnalyzeTableSimple";
				}
			}
			iturAnalyzesSourceRepository.InsertIturAnalyzes(dbPath, true, true, sp, dic);

			// так можно получить из заполненной таблицы  IturAnalyzes
			//IturAnalyzesCollection totalResult = iturAnalyzesRepository.GetIturAnalyzesTotal(dbPath);
			//long totalCount = totalResult.TotalCount;
			//double sumQuantityEdit = totalResult.SumQuantityEdit;

		
        }

		public static void BuildAnalyzeTableSum(IIturAnalyzesSourceRepository iturAnalyzesSourceRepository,
			IIturAnalyzesRepository iturAnalyzesRepository,
            CancellationTokenSource cts,
            string dbPath,
			Inventor currentInventor)
        {
            SelectParams sp = new SelectParams();
			
            Dictionary<object, object> dic = new Dictionary<object, object>();
            dic[ImportProviderParmEnum.CancellationToken] = cts.Token;
			dic[ImportProviderParmEnum.PriceCode] = PriceCodeEnum.PriceBuy.ToString();
			if (currentInventor != null)
			{
				if (string.IsNullOrWhiteSpace(currentInventor.PriceCode) == false)
				{
					dic[ImportProviderParmEnum.PriceCode] = currentInventor.PriceCode;
					dic[ImportProviderParmEnum.FromBuildAnalyze] = "BuildAnalyzeTableSum";
				}
			}
			bool refillCatalogDictionary = true;
			iturAnalyzesSourceRepository.InsertIturAnalyzesSumSimple(dbPath, true, refillCatalogDictionary, sp, dic);

			// так можно получить из заполненной таблицы  IturAnalyzes
			//IturAnalyzesCollection totalResult = iturAnalyzesRepository.GetIturAnalyzesTotal(dbPath);
			//long totalCount = totalResult.TotalCount;
			//double sumQuantityEdit = totalResult.SumQuantityEdit;
        }
    }
}