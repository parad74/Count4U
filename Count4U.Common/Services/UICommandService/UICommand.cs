using System;
using Microsoft.Practices.Prism.Commands;

namespace Count4U.Common.Services.UICommandService
{
    public interface IUICommand
    {
        string Title { get; set; }
        string Icon { get; set; }
        string Icon64 { get; set; }
        string Icon32 { get; set; }
        string Icon16 { get; set; }
    }
    public class UICommand : DelegateCommand, IUICommand
    {
        public UICommand(Action executeMethod)
            : base(executeMethod)
        {
        }

        public UICommand(Action executeMethod, Func<bool> canExecuteMethod)
            : base(executeMethod, canExecuteMethod)
        {
        }

        public string Title { get; set; }
        public string Icon { get; set; }
        public string Icon64 { get; set; }
        public string Icon32 { get; set; }
        public string Icon16 { get; set; }
    }

    public class UICommand<T> : DelegateCommand<T>, IUICommand
    {
        public UICommand(Action<T> executeMethod)
            : base(executeMethod)
        {

        }
        public UICommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
            : base(executeMethod, canExecuteMethod)
        {

        }

        public string Title { get; set; }
        public string Icon { get; set; }
        public string Icon64 { get; set; }
        public string Icon32 { get; set; }
        public string Icon16 { get; set; }
    }
}