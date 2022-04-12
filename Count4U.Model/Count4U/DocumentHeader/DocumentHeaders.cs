using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
    public class DocumentHeaders : ObservableCollection<DocumentHeader>
    {
        public static DocumentHeaders FromEnumerable(IEnumerable<DocumentHeader> list)
        {
            var collection = new DocumentHeaders();
            return Fill(collection, list);
        }

        private static DocumentHeaders Fill(DocumentHeaders collection, IEnumerable<DocumentHeader> list)
        {
            foreach (DocumentHeader item in list)
                collection.Add(item);
            return collection;
        }

        public DocumentHeader CurrentItem { get; set; }

        public EventHandler CurrentChanged { get; set; }

        public long TotalCount { get; internal set; }
    }
}
