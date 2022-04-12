using Count4U.Model;
using System;

namespace Count4U.Common.Constants.AdapterNames
{
    public class AdapterConnections
    {
        public static string GetRelated(string importAdapterName, ImportDomainEnum related)
        {
            switch (importAdapterName)
            {
                case ImportAdapterName.ImportCatalogAvivMultiBarcodesAdapter:
                    switch (related)
                    {
                        case ImportDomainEnum.ExportCatalogERP:
                            return ExportErpAdapterName.ExportErpAvivMulitBarcodes;
                        case ImportDomainEnum.UpdateCatalog:
                            return UpdateCatalogAdapterName.UpdateCatalogERPQuantityAvivMultiBarcodeAdapter;
                        case ImportDomainEnum.ImportBranch:
                            return String.Empty;
                        case ImportDomainEnum.ImportSupplier:
                            return String.Empty;
                    }
                    break;
                case ImportAdapterName.ImportCatalogDefaultAdapter:
                    switch (related)
                    {
                        case ImportDomainEnum.ExportCatalogERP:
                            return ExportErpAdapterName.ExportErpDefaultAdapter;
                        case ImportDomainEnum.UpdateCatalog:
                            return String.Empty;
                        case ImportDomainEnum.ImportBranch:
                            return String.Empty;
                        case ImportDomainEnum.ImportSupplier:
                            return ImportAdapterName.ImportSupplierDefaultAdapter;
                    }
                    break;
                case ImportAdapterName.ImportCatalogComaxASPAdapter:
                    switch (related)
                    {
                        case ImportDomainEnum.ExportCatalogERP:
                            return ExportErpAdapterName.ExportErpComaxAdapter;
                        case ImportDomainEnum.UpdateCatalog:
                            return String.Empty;
                        case ImportDomainEnum.ImportBranch:
                            return String.Empty;
                        case ImportDomainEnum.ImportSupplier:
                            return ImportAdapterName.ImportSupplierComaxASPAdapter;
                    }
                    break;
				case ImportAdapterName.ImportCatalogComaxAspMultiBarcodeAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpComaxAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return String.Empty;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return ImportAdapterName.ImportSupplierComaxASPAdapter;
					}
					break;
				case ImportAdapterName.ImportCatalogOne1Adapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpOne1Adapter;
						case ImportDomainEnum.UpdateCatalog:
							return UpdateCatalogAdapterName.UpdateCatalogERPQuantityOne1Adapter;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;


				case ImportAdapterName.ImportCatalogOrenMutagimAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpOrenMutagimAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return String.Empty;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;

				case ImportAdapterName.ImportCatalogGazitVerifoneSteimaztzkyAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpGazitVerifoneSteimaztzkyAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return UpdateCatalogAdapterName.UpdateCatalogERPQuantityGazitVerifoneSteimaztzkyAdapter;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSection:
							return ImportAdapterName.ImportSectionGazitVerifoneSteimaztzkyAdapter;
					}
					break;

				case ImportAdapterName.ImportCatalogSapb1ZometsfarimAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpSapb1ZometsfarimAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return UpdateCatalogAdapterName.UpdateCatalogERPQuantitySapb1ZometsfarimAdapter;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return ImportAdapterName.ImportSupplierSapb1ZometsfarimAdapter;
						case ImportDomainEnum.ImportSection:
							return ImportAdapterName.ImportSectionSapb1ZometsfarimAdapter;
					}
					break;

				case ImportAdapterName.ImportCatalogSapb1XslxAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpSapb1XslxAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return UpdateCatalogAdapterName.UpdateCatalogERPQuantitySapb1XslxAdapter;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;


				case ImportAdapterName.ImportCatalogWarehouseXslxAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpWarehouseXslxAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return String.Empty;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;
				case ImportAdapterName.ImportCatalogOtechAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpOtechAdapter;
						case ImportDomainEnum.UpdateCatalog:
							//return UpdateCatalogAdapterName.UpdateCatalogERPQuantityOtechAdapter;
							return String.Empty;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
				break;
				case ImportAdapterName.ImportCatalogBazanCsvAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpBazanCsvAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return String.Empty;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;
					
				case ImportAdapterName.ImportCatalogAS400AprilAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpAS400AprilAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return UpdateCatalogAdapterName.UpdateCatalogERPAS400AprilAdapter;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;

				case ImportAdapterName.ImportCatalogH_MAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpH_MAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return String.Empty;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;

				case ImportAdapterName.ImportCatalogH_M_NewAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpH_M_NewAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return String.Empty;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;

				case ImportAdapterName.ImportCatalogNimrodAvivAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpNimrodAvivAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return String.Empty;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;
				case ImportAdapterName.ImportCatalogNibitAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpNibitAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return UpdateCatalogAdapterName.UpdateCatalogERPQuantityNibitAdapter;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;

				case ImportAdapterName.ImportCatalogMade4NetAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpMade4NetAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return String.Empty;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;

				case ImportAdapterName.ImportCatalogTafnitMatrixAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpTafnitMatrixAdapter;
						case ImportDomainEnum.UpdateCatalog:
                            return UpdateCatalogAdapterName.UpdateCatalogERPQuantityTafnitMatrixAdapter;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;

				case ImportAdapterName.ImportCatalogNikeIntAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpNikeIntAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return UpdateCatalogAdapterName.UpdateCatalogERPQuantityNikeIntAdapter;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
				break;


				case ImportAdapterName.ImportCatalogAS400HoAdapter:
				switch (related)
				{
					case ImportDomainEnum.ExportCatalogERP:
						return ExportErpAdapterName.ExportErpAS400HoAdapter;
					case ImportDomainEnum.UpdateCatalog:
						return String.Empty;
					case ImportDomainEnum.ImportBranch:
						return String.Empty;
					case ImportDomainEnum.ImportSupplier:
						return String.Empty;
				}
				break;

				case ImportAdapterName.ImportCatalogAS400MegaAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpAS400MegaAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return UpdateCatalogAdapterName.UpdateCatalogERPAS400MegaAdapter;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;



				case ImportAdapterName.ImportCatalogGazitLeeCooperAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpGazitLeeCooperAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return String.Empty;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;

				case ImportAdapterName.ImportCatalogNativXslxAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpNativAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return String.Empty;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;

				case ImportAdapterName.ImportCatalogPriorityAldoAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpPriorityAldoAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return String.Empty;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;
				case ImportAdapterName.ImportCatalogMPLAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpMPLAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return UpdateCatalogAdapterName.UpdateCatalogERPQuantityMPLAdapter;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;
				case ImportAdapterName.ImportCatalogAS400HonigmanAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpAS400HonigmanAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return String.Empty;
						case ImportDomainEnum.ImportBranch:
							return ImportAdapterName.ImportBranchAS400HonigmanAdapter;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;

				case Common.Constants.ImportAdapterName.ImportCatalogPriorityCastroAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpPriorityCastroAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return String.Empty;
						case ImportDomainEnum.ImportBranch:
							return ImportAdapterName.ImportBranchPriorityCastroAdapter;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;

				case ImportAdapterName.ImportCatalogFRSVisionMirkamAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpFRSVisionMirkamAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return String.Empty;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;

				case ImportAdapterName.ImportCatalogAS400MangoAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpAS400MangoAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return String.Empty;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;

				case ImportAdapterName.ImportCatalogHashAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpHashAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return String.Empty;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;

				case ImportAdapterName.ImportCatalogPriorityKedsShowRoomAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpPriorityKedsShowRoomAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return String.Empty;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;

				case ImportAdapterName.ImportCatalogOrenOriginalsAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpOrenOriginalsAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return UpdateCatalogAdapterName.UpdateCatalogERPQuantityOrenOriginalsAdapter;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;
					
				case ImportAdapterName.ImportCatalogMaccabiPharmSAPAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpMaccabiPharmSAPAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return UpdateCatalogAdapterName.UpdateCatalogERPQuantityMaccabiPharmSAPAdapter;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;
                case ImportAdapterName.ImportCatalogGazitVerifoneAdapter:
                    switch (related)
                    {
                        case ImportDomainEnum.ExportCatalogERP:
                            return ExportErpAdapterName.ExportErpGazitAdapter;
                        case ImportDomainEnum.UpdateCatalog:
                            return String.Empty;
                        case ImportDomainEnum.ImportBranch:
                            return ImportAdapterName.ImportBranchGazitVerifoneAdapter;
                        case ImportDomainEnum.ImportSupplier:
                            return String.Empty;
                    }
                    break;
                case ImportAdapterName.ImportCatalogUnizagAdapter:
                    switch (related)
                    {
                        case ImportDomainEnum.ExportCatalogERP:
                            return ExportErpAdapterName.ExportErpUnizagAdapter;
                        case ImportDomainEnum.UpdateCatalog:
                            return String.Empty;
                        case ImportDomainEnum.ImportBranch:
                            return ImportAdapterName.ImportBranchGazitVerifoneAdapter;
                        case ImportDomainEnum.ImportSupplier:
                            return String.Empty;
                    }
                    break;
                case ImportAdapterName.ImportCatalogGeneralCSVAdapter:
                    switch (related)
                    {
                        case ImportDomainEnum.ExportCatalogERP:
                            return ExportErpAdapterName.ExportErpGeneralCSVAdapter;
                        case ImportDomainEnum.UpdateCatalog:
                            return UpdateCatalogAdapterName.UpdateCatalogERPQuantityGeneralCSVAdapter;
                        case ImportDomainEnum.ImportBranch:
                            return String.Empty;
                        case ImportDomainEnum.ImportSupplier:
                            return String.Empty;
                    }
                    break;

				case ImportAdapterName.ImportCatalogGeneralXLSXAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpGeneralXslxAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return UpdateCatalogAdapterName.UpdateCatalogERPQuantityGeneralCSVAdapter;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;
				case ImportAdapterName.ImportCatalogXtechMeuhedetXlsxAdapter:
                    switch (related)
                    {
                        case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpXtechMeuhedetXlsxAdapter;
                        case ImportDomainEnum.UpdateCatalog:
                            return UpdateCatalogAdapterName.UpdateCatalogERPQuantityXtechMeuhedetXlsxAdapter;
                        case ImportDomainEnum.ImportBranch:
							return ImportAdapterName.ImportBranchXtechMeuhedetXlsxAdapter;
                        case ImportDomainEnum.ImportSupplier:
                            return String.Empty;
                    }
                    break;
                case ImportAdapterName.ImportCatalogYarpaAdapter:
                    switch (related)
                    {
                        case ImportDomainEnum.ExportCatalogERP:
                            return ExportErpAdapterName.ExportErpYarpaAdapter;
                        case ImportDomainEnum.UpdateCatalog:
                            return UpdateCatalogAdapterName.UpdateCatalogERPQuantityYarpaAdapter;
                        case ImportDomainEnum.ImportBranch:
                            return String.Empty;
                        case ImportDomainEnum.ImportSupplier:
                            return String.Empty;
                    }
                    break;
                case ImportAdapterName.ImportCatalogYarpaWindowsAdapter:
                    switch (related)
                    {
                        case ImportDomainEnum.ExportCatalogERP:
                            return ExportErpAdapterName.ExportErpYarpaAdapter;
                        case ImportDomainEnum.UpdateCatalog:
                            return UpdateCatalogAdapterName.UpdateCatalogERPQuantityYarpaWindowsAdapter;
                        case ImportDomainEnum.ImportBranch:
                            return String.Empty;
                        case ImportDomainEnum.ImportSupplier:
                            return String.Empty;
                    }
                    break;
                case ImportAdapterName.ImportCatalogAvivPOSAdapter:
                    switch (related)
                    {
                        case ImportDomainEnum.ExportCatalogERP:
                            return ExportErpAdapterName.ExportErpAvivPOSAdapter;
                        case ImportDomainEnum.UpdateCatalog:
                            return UpdateCatalogAdapterName.UpdateCatalogERPQuantityAvivPOSAdapter;
                        case ImportDomainEnum.ImportBranch:
                            return String.Empty;
                        case ImportDomainEnum.ImportSupplier:
                            return ImportAdapterName.ImportSupplierAvivPOSAdapter;
                    }
                    break;
                case ImportAdapterName.ImportCatalogPriorityRenuarAdapter:
                    switch (related)
                    {
                        case ImportDomainEnum.ExportCatalogERP:
                            return ExportErpAdapterName.ExportErpPriorityRenuarAdapter;
                        case ImportDomainEnum.UpdateCatalog:
                            return String.Empty;
                        case ImportDomainEnum.ImportBranch:
                            return String.Empty;
                        case ImportDomainEnum.ImportSupplier:
                            return String.Empty;
                    }
                    break;
                case ImportAdapterName.ImportCatalogPosSuperPharmAdapter:
                    switch (related)
                    {
                        case ImportDomainEnum.ExportCatalogERP:
                            return ExportErpAdapterName.ExportErpPosSuperPharmAdapter;
                        case ImportDomainEnum.UpdateCatalog:
                            return UpdateCatalogAdapterName.UpdateCatalogERPQuantityPosSuperPharmAdapter;
                        case ImportDomainEnum.ImportBranch:
                            return String.Empty;
                        case ImportDomainEnum.ImportSupplier:
                            return String.Empty;
                    }
                    break;
                case ImportAdapterName.ImportCatalogMirkamSonolAdapter:
                    switch (related)
                    {
                        case ImportDomainEnum.ExportCatalogERP:
                            return ExportErpAdapterName.ExportErpMirkamSonolAdapter;
                        case ImportDomainEnum.UpdateCatalog:
                            return String.Empty;
                        case ImportDomainEnum.ImportBranch:
                            return ImportAdapterName.ImportBranchMirkamSonolAdapter;
                        case ImportDomainEnum.ImportSupplier:
                            return String.Empty;
                    }
                    break;
                case ImportAdapterName.ImportCatalogPriorityKedsAdapter:
                    switch (related)
                    {
                        case ImportDomainEnum.ExportCatalogERP:
                            return ExportErpAdapterName.ExportErpPriorityKedsRegularAdapter;
                        case ImportDomainEnum.UpdateCatalog:
                            return UpdateCatalogAdapterName.UpdateCatalogERPQuantityPriorityKedsAdapter;
                        case ImportDomainEnum.ImportBranch:
                            return String.Empty;
                        case ImportDomainEnum.ImportSupplier:
                            return String.Empty;
                    }
                    break;
                case ImportAdapterName.ImportCatalogRetalixPOS_HOAdapter:
                    switch (related)
                    {
                        case ImportDomainEnum.ExportCatalogERP:
                            return ExportErpAdapterName.ExportErpRetalixPOS_HOAdapter;
                        case ImportDomainEnum.UpdateCatalog:
                            return UpdateCatalogAdapterName.UpdateCatalogERPQuantityRetalixPOS_HOAdapter;
                        case ImportDomainEnum.ImportBranch:
                            return String.Empty;
                        case ImportDomainEnum.ImportSupplier:
                            return String.Empty;
                    }
                    break;
                case ImportAdapterName.ImportCatalogAS400_LeumitAdapter:
                    switch (related)
                    {
                        case ImportDomainEnum.ExportCatalogERP:
                            return ExportErpAdapterName.ExportErpAS400_LeumitAdapter;
                        case ImportDomainEnum.UpdateCatalog:
                            return UpdateCatalogAdapterName.UpdateCatalogERPQuantityAS400_LeumitAdapter;
                        case ImportDomainEnum.ImportBranch:
                            return ImportAdapterName.ImportBranchAS400_LeumitAdapter;
                        case ImportDomainEnum.ImportSupplier:
                            return String.Empty;
                    }
                    break;
                case ImportAdapterName.ImportCatalogMiniSoftAdapter:
                    switch (related)
                    {
                        case ImportDomainEnum.ExportCatalogERP:
                            return ExportErpAdapterName.ExportErpMiniSoftAdapter;
                        case ImportDomainEnum.UpdateCatalog:
                            return String.Empty;
                        case ImportDomainEnum.ImportBranch:
                            return String.Empty;
                        case ImportDomainEnum.ImportSupplier:
                            return String.Empty;
                    }
                    break;
                case ImportAdapterName.ImportCatalogMirkamSonolSAPAdapter:
                    switch (related)
                    {
                        case ImportDomainEnum.ExportCatalogERP:
                            return ExportErpAdapterName.ExportErpMirkamSonolSAPAdapter;
                        case ImportDomainEnum.UpdateCatalog:
                            return String.Empty;
                        case ImportDomainEnum.ImportBranch:
                            return String.Empty;
                        case ImportDomainEnum.ImportSupplier:
                            return String.Empty;
                    }
                    break;
                case ImportAdapterName.ImportCatalogRetalixNextAdapter:
                    switch (related)
                    {
                        case ImportDomainEnum.ExportCatalogERP:
                            return ExportErpAdapterName.ExportErpRetalixNextAdapter;
                        case ImportDomainEnum.UpdateCatalog:
                            return UpdateCatalogAdapterName.UpdateCatalogERPRetalixNextAdapter;
                        case ImportDomainEnum.ImportBranch:
                            return String.Empty;
                        case ImportDomainEnum.ImportSupplier:
                            return String.Empty;   
                    }
                    break;
				case ImportAdapterName.ImportCatalogMikiKupotAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpMikiKupotAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return String.Empty;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;
				case ImportAdapterName.ImportCatalogAS400AmericanEagleAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpAS400AmericanEagleAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return String.Empty;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;

				case ImportAdapterName.ImportCatalogLadyComfortAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpLadyComfortAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return String.Empty;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;

				case ImportAdapterName.ImportCatalogAS400JaforaAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpAS400JaforaAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return String.Empty;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;

				case ImportAdapterName.ImportCatalogMerkavaXslxAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return String.Empty;
						case ImportDomainEnum.UpdateCatalog:
							return String.Empty;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;

				case ImportAdapterName.ImportCatalogClalitXslxAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return String.Empty;
						case ImportDomainEnum.UpdateCatalog:
							return String.Empty;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;


				case ImportAdapterName.ImportCatalogPrioritySweetGirlXlsxAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpPrioritySweetGirlXlsxAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return String.Empty;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;

			

				case ImportAdapterName.ImportCatalogAS400HamashbirAdapter:
					switch (related)
					{
						case ImportDomainEnum.ExportCatalogERP:
							return ExportErpAdapterName.ExportErpAS400HamashbirAdapter;
						case ImportDomainEnum.UpdateCatalog:
							return UpdateCatalogAdapterName.UpdateCatalogAS400HamashbirAdapter;
						case ImportDomainEnum.ImportBranch:
							return String.Empty;
						case ImportDomainEnum.ImportSupplier:
							return String.Empty;
					}
					break;

                default:
                    return string.Empty;
            }

            return String.Empty;
        }
    }
}