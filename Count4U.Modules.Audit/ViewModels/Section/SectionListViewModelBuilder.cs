using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Count4U.Common.Helpers;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.SelectionParams;
using Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.InventProductControl;

namespace Count4U.Modules.Audit.ViewModels.Section
{
    public class SectionListViewModelBuilder
    {
        private readonly ISectionRepository _sectionRepository;
        private readonly IProductRepository _productRepository;

        public SectionListViewModelBuilder(
            ISectionRepository sectionRepository,
            IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _sectionRepository = sectionRepository;
        }

		//public void Build(ObservableCollection<SectionItem2ViewModel> items, string dbPath, SelectParams selectParams)
  //      {
		//	Sections sections = _sectionRepository.GetSections(selectParams, dbPath);
  //          //List<string> sectionCodeList = _productRepository.GetSectionCodeList(dbPath).Where(r => !String.IsNullOrWhiteSpace(r)).ToList();

		//	//foreach (string sectionCode in sectionCodeList)
		//	//{
		//	//	Count4U.Model.Count4U.Section section = sections.FirstOrDefault(r => r.SectionCode == sectionCode);
		//	//	if (section == null)
		//	//	{
		//	//		section = new Model.Count4U.Section();
		//	//		section.SectionCode = sectionCode;
		//	//		section.Name = sectionCode;
		//	//		sections.Add(section);
		//	//	}
		//	//}

  //          Utils.RunOnUI(() =>
  //          {
  //              List<string> sectionCodeList = items.Select(x => x.Code).Distinct().ToList();
  //              foreach (Count4U.Model.Count4U.Section section in sections)
  //              {
  //                  if (sectionCodeList.Contains(section.SectionCode) == true) continue;
  //                  SectionItem2ViewModel viewModel = new SectionItem2ViewModel(section);
  //                  items.Add(viewModel);
  //              }
  //          });
  //      }


        public void Build1(ObservableCollection<SectionItem2ViewModel> items, List<Count4U.Model.Count4U.Section> sections)
        {

            Utils.RunOnUI(() =>
            {
                List<string> sectionCodeList = items.Select(x => x.Code).Distinct().ToList();
                foreach (Count4U.Model.Count4U.Section section in sections)
                {
                    if (sectionCodeList.Contains(section.SectionCode) == true) continue;
                    SectionItem2ViewModel viewModel = new SectionItem2ViewModel(section);
                    items.Add(viewModel);
                }
            });
        }
    }
}