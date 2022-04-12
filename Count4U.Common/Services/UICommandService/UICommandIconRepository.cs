using System;
using System.Windows.Media.Imaging;

namespace Count4U.Common.Services.UICommandService
{
    public class UICommandIconRepository : IUICommandIconRepository
    {
        public string GetIcon(enUICommand type, enIconSize size)
        {
            string name = GetName(type, size);

            if (String.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException();

            return String.Format(@"/Count4U.Media;component/Icons/{0}.png", name);
        }

        private string GetName(enUICommand type, enIconSize size)
        {
            string def = "additurim";
            switch (type)
            {
                case enUICommand.Add:
                    switch (size)
                    {
                        case enIconSize.Size64:
							return "additurim";

                        case enIconSize.Size32:
							return "additurim";

                        case enIconSize.Size16:
                            return "add_very_small";

                        default:
                            throw new ArgumentOutOfRangeException("size");
                    }
				case enUICommand.MultiAdd:
					switch (size)
					{
						case enIconSize.Size64:
							return "addMulti";

						case enIconSize.Size32:
							return "addMulti32";

						case enIconSize.Size16:
							return "add_very_small";

						default:
							throw new ArgumentOutOfRangeException("size");
					}

				case enUICommand.ShowConfig:
				case enUICommand.ShowIni:
				case enUICommand.RunByConfig:
				case enUICommand.ClearByConfig:
				case enUICommand.ShowLogByConfig:
				case enUICommand.NavigateToGrid:

					
                case enUICommand.Edit:
                    switch (size)
                    {
                        case enIconSize.Size64:
                            return "edit";
                        case enIconSize.Size32:
                            return "edit48";
                        case enIconSize.Size16:
							return "itur_edit";//"edit_small";
                        default:
                            throw new ArgumentOutOfRangeException("size");
                    }
				case enUICommand.ShowImage:
					switch (size)
					{
						case enIconSize.Size64:
							return "photo";
						case enIconSize.Size32:
							return "photo";
						case enIconSize.Size16:
							return "photo";//"edit_small";
						default:
							throw new ArgumentOutOfRangeException("size");
					}
				case enUICommand.Copy:
					switch (size)
					{
						case enIconSize.Size64:
							return "copy";
						case enIconSize.Size32:
							return "copy";
						case enIconSize.Size16:
							return "copy";//"edit_small";
						default:
							throw new ArgumentOutOfRangeException("copy");
					}
					
                case enUICommand.Delete:
                    switch (size)
                    {
                        case enIconSize.Size64:
                            return "deleteiturim";
                        case enIconSize.Size32:
                            return "deleteiturim";
                        case enIconSize.Size16:
                            return "delete_very_small";
                        default:
                            throw new ArgumentOutOfRangeException("size");
                    }

				case enUICommand.DeleteNoneCatalog:
                    switch (size)
                    {
                        case enIconSize.Size64:
                            return "clear64";
                        case enIconSize.Size32:
							return "clear64";
                        case enIconSize.Size16:
							return "clear48";
                        default:
                            throw new ArgumentOutOfRangeException("size");
                    }

				case enUICommand.DeleteNoneCatalogMain:
					switch (size)
					{
						case enIconSize.Size64:
							return "clear_64";
						case enIconSize.Size32:
							return "clear_64";
						case enIconSize.Size16:
							return "clear48";
						default:
							throw new ArgumentOutOfRangeException("size");
					}

				case enUICommand.ClearIturs:
                    switch (size)
                    {
                        case enIconSize.Size64:
							return "deleteallwithoutchild";
                        case enIconSize.Size32:
							return "deleteallwithoutchild";
                        case enIconSize.Size16:
							return "deleteallwithoutchild";
                        default:
                            throw new ArgumentOutOfRangeException("size");
                    }
                case enUICommand.DeleteWithoutChild:
				case enUICommand.DeleteObjectsFromMainDB:
                    return "deletewithoutchild";
					
                case enUICommand.DeleteAll:
                    return "deletewithoutchild"; 
                case enUICommand.DeleteAllWithoutChild:
                    return "deleteallwithoutchild";
				case enUICommand.MultiDelete:
					return "DeleteX";
                    //return "deletewithoutchild";
                case enUICommand.Import:
                    return "import64";//"importiturim";
                case enUICommand.Report:
                    switch (size)
                    {
                        case enIconSize.Size64:
                            return "reports1";
                        case enIconSize.Size32:
                            return "reports1";
                        case enIconSize.Size16:
                            return "report_small";
                        default:
                            throw new ArgumentOutOfRangeException("size");
                    }

                case enUICommand.RepairFromDb:
                    return "repair";
			   case enUICommand.EditInventorOptions:
					//return "repair";
					return "command_inventorEdit";
				case enUICommand.EditCustomerOptions:
					return "command_customerEdit";
                case enUICommand.Search:
                    return "search2";
                case enUICommand.Filter:
                    throw new NotImplementedException();
                case enUICommand.ImportFromPda:
                    return "pda";
                case enUICommand.ImportFromPdaAuto:
                    return "pda_auto";
                case enUICommand.IturAddEdit:
                    return "addedititurim";
                case enUICommand.Refresh:
                    return "refresh";
                case enUICommand.RefreshStatus:
                    return "refresh_small";
				case enUICommand.ChangeIturName:
				switch (size)
					{
						case enIconSize.Size64:
							return "edit";
						case enIconSize.Size32:
							return "edit48";
						case enIconSize.Size16:
							return "itur_edit";//"edit_small"; 
						default:
							throw new ArgumentOutOfRangeException("size");
					}
				case enUICommand.ChangeIturPrefix:
					return "repair";
				case enUICommand.ShowShelf:
					return "downLevel";
					
				case enUICommand.ChangeLocation:
					switch (size)
					{
						case enIconSize.Size64:
							return "changelocationiturim";
						case enIconSize.Size32:
							return "changelocationiturim";
						case enIconSize.Size16:
							return "changelocationiturimsmall";
						default:
							throw new ArgumentOutOfRangeException("size");
					}
				case enUICommand.ChangeLocationTag:
					switch (size)
					{
						case enIconSize.Size64:
							return "changelocationiturim";
						case enIconSize.Size32:
							return "changelocationiturim";
						case enIconSize.Size16:
							return "changelocationiturimsmall";
						default:
							throw new ArgumentOutOfRangeException("size");
					}

				case enUICommand.ChangeIturTag:
					switch (size)
					{
						case enIconSize.Size64:
							return "changelocationiturim";
						case enIconSize.Size32:
							return "changelocationiturim";
						case enIconSize.Size16:
							return "changelocationiturimsmall";
						default:
							throw new ArgumentOutOfRangeException("size");
					}
					
                case enUICommand.ChangeState:
                    return "switch";
                case enUICommand.OpenScript:
                    return "open_script";
                case enUICommand.SaveScript:
                    return "save_script";
                case enUICommand.Restore:
                    return "refresh2";
                case enUICommand.Child:
                    return "children2";
                case enUICommand.Clear:
                    switch (size)
                    {
                        case enIconSize.Size64:
                            return "clear";
                        case enIconSize.Size32:
                            return "clear";
                        case enIconSize.Size16:
                            return "clear2";
                        default:
                            throw new ArgumentOutOfRangeException("size");
                    }
                case enUICommand.ClearTag:
                    return "clear64";
                case enUICommand.Accept:
					switch (size)
					{
						case enIconSize.Size64:
							return "check";
						case enIconSize.Size32:
							return "check";
						case enIconSize.Size16:
							return "check16";
						default:
							throw new ArgumentOutOfRangeException("size");
					}
                case enUICommand.Log:
                    return "log";
				case enUICommand.Config:
					return "generate";
                case enUICommand.Export:
                    return "export64";
                case enUICommand.Generate:
                    return "generate";
                case enUICommand.Up:
                    return "up";
                case enUICommand.Down:
                    return "down";
                case enUICommand.More:
                    return def;
				case enUICommand.Compare:
					return def;
                case enUICommand.Update:
                case enUICommand.UpdateCatalog:
				case enUICommand.ComplexOperation:
                    return def;
                case enUICommand.GetFromPda:
                    return def;
                case enUICommand.MaskEdit:
                    return def;
                case enUICommand.Reset:
                    return "reset_small";
                case enUICommand.ResetBit:
                    return "resetBit48";
                case enUICommand.Undo:
                    return "undo48";
                case enUICommand.Adapters:
                case enUICommand.AddBranch:
                case enUICommand.AddCustomer:
                case enUICommand.AddInventor:
                case enUICommand.Catalog:
                case enUICommand.ChangeCatalog:
                case enUICommand.EditBranch:
                case enUICommand.EditCustomer:
                case enUICommand.EditInventor:
                case enUICommand.ExportERP:
                case enUICommand.ExportPDA:
                case enUICommand.Properties:
                case enUICommand.ReportFavorites:
                case enUICommand.SearchBranch:
                case enUICommand.SearchCustomer:
                case enUICommand.SearchInventor:
              	case enUICommand.Profile:
				case enUICommand.Ftp:
				case enUICommand.AddPack:
				case enUICommand.ClearList:
                case enUICommand.SendZipOffice:
				case enUICommand.GoToPage:
                    return def;
                case enUICommand.FromImportToGrid:
                    return "fromimporttogrid";
                case enUICommand.PrintReport:
                    return "print_report";
				case enUICommand.DissableIturs:
					return "Checkbox-2";
					
				case enUICommand.PrintReportByLocationCode:
                    return "print_report1";
                case enUICommand.AddEditLocationItur:
                     return "locations";
                  


                case enUICommand.PrintReportByTag:
                    return "print_report2";
                case enUICommand.View:
                    return "view";
                case enUICommand.ChildrenForBranch:
                    return "inventor";
                case enUICommand.ChildrenForCustomer:
                    return "branch";

                case enUICommand.CustomerPostAddCustomer:
					return "post_customer_add";
                case enUICommand.CustomerPostAddBranch:
                    return "post_branch_add";
                case enUICommand.CustomerPostEditCustomer:
                    return "post_customer_edit";
                case enUICommand.CustomerPostCustomerDashboard:
                    return "post_customer_dashbord";
                case enUICommand.CustomerPostHomeDashboard:
                    return "post_home_dashboard";

                case enUICommand.BranchPostAddBranch:
                    return "post_branch_add";
                case enUICommand.BranchPostAddInventor:
                    return "post_inventor_add";
                case enUICommand.BranchPostEditBranch:
                    return "post_branch_edit";
                case enUICommand.BranchPostBranchDashboard:
                    return "post_branch_dashboard";
                case enUICommand.BranchPostHomeDashboard:
                    return "post_home_dashboard";

                case enUICommand.InventorPostInventorForm:
                    return "post_inventor_main";
                case enUICommand.InventorPostEditInventor:
                    return "post_inventor_edit";
                case enUICommand.InventorPostInventorDashboard:
                    return "post_inventor_dashboard";
                case enUICommand.InventorPostMainDashboard:
                    return "post_main_dashbord";
                case enUICommand.InventorPostHomeDashboard:
                    return "post_home_dashboard";
			  case enUICommand.CreateAndNavigateToProcessInventor:
 			  case enUICommand.CreateNewInventor:
					return "command_invrntorAdd2";
					
				case enUICommand.ToProcessInventor:
					return "command_invrntorEdit";
					
                case enUICommand.ChangeCurrentInventor:
					return "command_inventor_select";
                case enUICommand.Clone:
                    return "clone";
                case enUICommand.Planogram:
                    return "planogram";
                case enUICommand.ExportERPMain:
                    return "exporterpmain";
				case enUICommand.UploadToPDA:
                    return "upload32";
				case enUICommand.DownloadFromPDA:
					return "download32";
				case enUICommand.FromFtp:
                    return "upload32";
				case enUICommand.ToFtp:
					return "download32";
					
				case enUICommand.GetFromFtp:
					return "download32";
				case enUICommand.DevicePDAStatistic:
					return "statistic64";
                case enUICommand.DeviceWorkerPDAStatistic:
                    return "statistic64";
                case enUICommand.Pack:
					return "command_pack64";
				case enUICommand.Unpack:
					return "command_unpack64";
				case enUICommand.AddNewInventor:
					return "command_invrntorAdd2";
					
                case enUICommand.ExportPDAMain:
                    return "exportpdamain";
                case enUICommand.UpdateMain:
                    return "updatemain";
				case enUICommand.ChangeStatus:
					return "ChangeStatus";
                case enUICommand.AlignLeft:
                    return "plan/align_left";
                case enUICommand.AlignRight:
                    return "plan/align_right";
                case enUICommand.AlignTop:
                    return "plan/align_top";
                case enUICommand.AlignBottom:
                    return "plan/align_bottom";
                case enUICommand.AlignSameWidth:
                    return "plan/align_same_width";
                case enUICommand.AlignSameHeight:
                    return "plan/align_same_height";
                case enUICommand.BringForward:
                    return "plan/bring_forward";
                case enUICommand.SendBackward:
                    return "plan/send_backward";
                default:
                    return def;
            }
        }

        private static BitmapImage Build(string name)
        {
            return new BitmapImage(new Uri(String.Format("pack://application:,,,/Count4U.Media;component/Icons/{0}.png", name)));
	    }

        public static BitmapImage IconSearch
        {
            get { return Build(@"search2"); }
        }

        public static BitmapImage FilterState1
        {
            get { return Build(@"filter3"); }
        }

        public static BitmapImage FilterState2
        {
            get { return Build(@"filter4"); }
        }

        public static BitmapImage OpenFolder
        {
            get { return Build("open_folder_32"); }
        }

        public static BitmapImage Copy
        {
            get { return Build("copy"); }
        }

        public static BitmapImage Report
        {
            get { return Build("reports"); }
        }

		public static BitmapImage Export
		{
			get { return Build("export64"); }
		}

        public static BitmapImage Switch
        {
            get { return Build("switch"); }
        }

        public static BitmapImage Switch2
        {
            get { return Build("switch2"); }
        }

        public static BitmapImage FastMove
        {
            get { return Build("fast_move"); }
        }

        public static BitmapImage ChangeLocationIturimSmall
        {
            get { return Build("changelocationiturim"); }
        }

		  public static BitmapImage ChangeNameIturimSmall
        {
			get { return Build("edit48"); }
        }
	

        public static BitmapImage Print
        {
            get { return Build("print_48"); }
        }

        public static BitmapImage Enable
        {
            get { return Build("enable"); }
        }

        public static BitmapImage Disable
        {
            get { return Build("disable"); }
        }
    }
}