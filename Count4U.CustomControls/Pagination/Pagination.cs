using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using NLog;

namespace Count4U.CustomControls.Pagination
{
    [TemplatePart(Name = "PART_TextBlockPortion", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_TextBlockTotal", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_ButtonFirst", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "PART_ButtonNext", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "PART_ButtonPrevious", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "PART_ButtonLast", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "PART_ListBox", Type = typeof(ListBox))]
    public class Pagination : Control
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static DependencyProperty ItemsTotalProperty =
            DependencyProperty.Register("ItemsTotal", typeof(int), typeof(Pagination),
            new PropertyMetadata(0, ItemsTotalPropertyChangedCallback, ItemsTotalPropertyCoerceValueCallback));

        private static object ItemsTotalPropertyCoerceValueCallback(
            DependencyObject dependencyObject,
            object baseValue)
        {
            int bValue = (int)baseValue;
            if (bValue < 0) bValue = 0;
            return bValue;
        }

        private static void ItemsTotalPropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            Pagination pagination = dependencyObject as Pagination;
            if (pagination != null)
            {
                pagination.CoerceValue(PageSizeProperty);
                pagination.CoerceValue(PageCurrentProperty);
                pagination.Validate();
            }
        }

        public static DependencyProperty PageSizeProperty =
            DependencyProperty.Register("PageSize", typeof(int), typeof(Pagination),
            new PropertyMetadata(0, PageSizePropertyChangedCallback, PageSizePropertyCoerceValueCallback));

        private static object PageSizePropertyCoerceValueCallback(
            DependencyObject dependencyObject,
            object baseValue)
        {
            int bValue = (int)baseValue;
            if (bValue <= 0)
                bValue = 1;

            int total = (int)dependencyObject.GetValue(ItemsTotalProperty);
            if (total < bValue)
                bValue = total;

            return bValue;
        }

        private static void PageSizePropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            Pagination pagination = dependencyObject as Pagination;
            if (pagination != null)
            {
                pagination.CoerceValue(PageCurrentProperty);
                pagination.Validate();
            }
        }

        public static DependencyProperty PageCurrentProperty =
            DependencyProperty.Register("PageCurrent", typeof(int), typeof(Pagination),
            new PropertyMetadata(0, PageCurrentPropertyChangedCallback, PageCurrentCoerceValueCallback));

        private static object PageCurrentCoerceValueCallback(
            DependencyObject dependencyObject,
            object baseValue)
        {
            int bValue = (int)baseValue;

            int pageSize = (int)dependencyObject.GetValue(PageSizeProperty);
            if (pageSize == 0)
                return 1;

            int totalItems = (int)dependencyObject.GetValue(ItemsTotalProperty);
            if (totalItems == 0)
                return 1;

            if (bValue <= 0)
                return 1;

            int maxPossibleCurrent = (int)Math.Ceiling(((double)totalItems / (double)pageSize));
            if (bValue > maxPossibleCurrent)
                bValue = maxPossibleCurrent;

            return bValue;
        }

        private static void PageCurrentPropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            Pagination pag = dependencyObject as Pagination;
            if (pag != null)
                pag.Validate();
        }

        public int PageCurrent
        {
            get { return (int)GetValue(PageCurrentProperty); }
            set { SetValue(PageCurrentProperty, value); }
        }

        public int ItemsTotal
        {
            get { return (int)GetValue(ItemsTotalProperty); }
            set { SetValue(ItemsTotalProperty, value); }
        }

        public int PageSize
        {
            get { return (int)GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }

        void Validate()
        {
            if (this._textBlockPortion == null || this._textBlockTotal == null || this._buttonFirst == null || this._buttonLast == null || this._buttonNext == null || this._buttonPrevious == null)
                return;

            int total = ItemsTotal;
            int from = ((PageCurrent - 1) * PageSize) + 1;
            int to = from + PageSize - 1;
            if (to > total)
                to = total;

            if (total == 0 && to == 0)
                from = 0;

            int pagesCount = (int)Math.Ceiling((double)ItemsTotal / (double)PageSize);

            this._textBlockPortion.Text = PageCurrent == pagesCount ? (ItemsTotal - (PageSize * (PageCurrent - 1))).ToString() : PageSize.ToString();
            this._textBlockTotal.Text = String.Format(Localization.Resources.Control_Pagination_Of, ItemsTotal);


            this._buttonFirst.IsEnabled = PageCurrent != 1;
            this._buttonPrevious.IsEnabled = PageCurrent > 1;

            this._buttonNext.IsEnabled = to != total;
            this._buttonLast.IsEnabled = to != total;

            BuildPagesList();
        }

        private void BuildPagesList()
        {
            ObservableCollection<object> items = null;
            if (ItemsTotal != 0)
            {
                int pagesCount = (int)Math.Ceiling((double)ItemsTotal / (double)PageSize);

                if (pagesCount <= 11)
                {
                    items = new ObservableCollection<object>();

                    foreach (var x in Enumerable.Range(1, pagesCount <= 0 ? 1 : pagesCount))
                    {
                        items.Add(new PaginationPage { Data = x });
                    }

                    PaginationPage cur = items.OfType<PaginationPage>().Where(r => r.Data == PageCurrent).FirstOrDefault();
                    if (cur != null)
                    {
                        int index = items.IndexOf(cur);
                        if (index == -1)
                        {
                            _logger.Warn("Pagination.BuildPagesList, index of current == -1");
                            _logger.Warn("Pagination.ItemsTotal : {0}", ItemsTotal);
                            _logger.Warn("Pagination.PageCurrent : {0}", PageCurrent);
                            _logger.Warn("Pagination.PageSize : {0}", PageSize);
                        }
                        else
                        {
                            items[index] = new PaginationCurrentPage { Data = PageCurrent };
                        }
                    }
                }
                else
                {
                    List<int> pattern = new List<int>();
                    int currentN = 0;
                    switch (PageCurrent)
                    {

                        case 1:
                        case 2:
                            pattern = new List<int> { 1, 2, 3, -1, pagesCount - 2, pagesCount - 1, pagesCount };
                            currentN = PageCurrent;
                            break;
                        case 3:
                            pattern = new List<int> { 1, 2, 3, 4, -1, pagesCount - 2, pagesCount - 1, pagesCount };
                            currentN = 3;
                            break;
                        case 4:
                            pattern = new List<int> { 1, 2, 3, 4, 5, -1, pagesCount - 2, pagesCount - 1, pagesCount };
                            currentN = 4;
                            break;
                        case 5:
                            pattern = new List<int> { 1, 2, 3, 4, 5, 6, -1, pagesCount - 2, pagesCount - 1, pagesCount };
                            currentN = 5;
                            break;
                        default:
                            {
                                int c = PageCurrent;
                                if (c == pagesCount)
                                {
                                    pattern = new List<int> { 1, 2, 3, -1, pagesCount - 2, pagesCount - 1, pagesCount };
                                    currentN = 7;
                                }
                                else if (c == pagesCount - 1)
                                {
                                    pattern = new List<int> { 1, 2, 3, -1, pagesCount - 2, pagesCount - 1, pagesCount };
                                    currentN = 6;
                                }
                                else if (c == pagesCount - 2)
                                {
                                    pattern = new List<int> { 1, 2, 3, -1, pagesCount - 3, pagesCount - 2, pagesCount - 1, pagesCount };
                                    currentN = 6;
                                }
                                else if (c == pagesCount - 3)
                                {
                                    pattern = new List<int> { 1, 2, 3, -1, pagesCount - 4, pagesCount - 3, pagesCount - 2, pagesCount - 1, pagesCount };
                                    currentN = 6;
                                }
                                else if (c == pagesCount - 4)
                                {
                                    pattern = new List<int> { 1, 2, 3, -1, pagesCount - 5, pagesCount - 4, pagesCount - 3, pagesCount - 2, pagesCount - 1, pagesCount };
                                    currentN = 6;
                                }
                                else
                                {
                                    pattern = new List<int> { 1, 2, 3, -1, c - 1, c, c + 1, -1, pagesCount - 2, pagesCount - 1, pagesCount };
                                    currentN = 6;
                                }
                                break;
                            }
                    }

                    items = BuildPagesListPattern(pattern, currentN);
                }

                if (items != null)
                {
                    foreach (PaginationPage page in items.OfType<PaginationPage>())
                    {
                        page.PageCommand = new DelegateCommand<object>(PageCommandExecuted);
                    }
                }

            }

            this._pagingListBox.ItemsSource = items;
        }

        private void PageCommandExecuted(object i)
        {
            this.PageCurrent = Int32.Parse(i.ToString());
        }

        private ObservableCollection<object> BuildPagesListPattern(IEnumerable<int> pattern, int currentN)
        {
            if (currentN == 0)
                return null;

            ObservableCollection<object> result = new ObservableCollection<object>();
            int i = 1;
            foreach (int p in pattern)
            {
                if (p == -1)
                    result.Add(new PaginationDots());
                else
                    if (i == currentN)
                        result.Add(new PaginationCurrentPage { Data = p });
                    else
                        result.Add(new PaginationPage { Data = p });
                i++;
            }

            return result;
        }

        static Pagination()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pagination),
                new FrameworkPropertyMetadata(typeof(Pagination)));
        }

        public Pagination()
        {
            this.PreviewKeyDown += Pagination_PreviewKeyDown;
        }

        private ButtonBase _buttonFirst;
        private ButtonBase _buttonNext;
        private ButtonBase _buttonPrevious;
        private ButtonBase _buttonLast;
        private ListBox _pagingListBox;
        private TextBlock _textBlockPortion;
        private TextBlock _textBlockTotal;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._textBlockPortion = GetTemplateChild("PART_TextBlockPortion") as TextBlock;
            this._textBlockTotal = GetTemplateChild("PART_TextBlockTotal") as TextBlock;
            this._buttonFirst = GetTemplateChild("PART_ButtonFirst") as ButtonBase;
            this._buttonNext = GetTemplateChild("PART_ButtonNext") as ButtonBase;
            this._buttonPrevious = GetTemplateChild("PART_ButtonPrevious") as ButtonBase;
            this._buttonLast = GetTemplateChild("PART_ButtonLast") as ButtonBase;

            this._pagingListBox = GetTemplateChild("PART_ListBox") as ListBox;

            this._buttonFirst.Click += this._buttonFirst_Click;
            this._buttonNext.Click += this._buttonNext_Click;
            this._buttonPrevious.Click += this._buttonPrevious_Click;
            this._buttonLast.Click += this._buttonLast_Click;

            Validate();           
        }

        void _buttonLast_Click(object sender, RoutedEventArgs e)
        {
            this.PageCurrent = (int)Math.Ceiling(((double)this.ItemsTotal / (double)this.PageSize));
        }

        void _buttonPrevious_Click(object sender, RoutedEventArgs e)
        {
            this.PageCurrent -= 1;
        }

        void _buttonNext_Click(object sender, RoutedEventArgs e)
        {
            this.PageCurrent += 1;
        }

        void _buttonFirst_Click(object sender, RoutedEventArgs e)
        {
            this.PageCurrent = 1;
        }

        void Pagination_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {            
           
        }

       
    }
}
