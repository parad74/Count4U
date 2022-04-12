using System;

namespace Count4U.Common.Services.UICommandService
{
    public class UICommandRepository
    {
        private readonly IUICommandIconRepository _iconRepository;
        private readonly UICommandTitleRepository _titleRepository;

        public UICommandRepository(IUICommandIconRepository iconRepository, UICommandTitleRepository titleRepository)
        {
            _titleRepository = titleRepository;
            _iconRepository = iconRepository;
        }

        public UICommand Build(enUICommand type, Action executeMethod, Func<bool> canExecuteMethod = null)
        {
            string title = _titleRepository.GetTitle(type);

            string icon64 = GetIcon(type, enIconSize.Size64);
            string icon32 = GetIcon(type, enIconSize.Size32);
            string icon16 = GetIcon(type, enIconSize.Size16);

            UICommand result;
            if (canExecuteMethod == null)
                result = new UICommand(executeMethod);
            else
                result = new UICommand(executeMethod, canExecuteMethod);

            result.Title = title;
			result.Icon = icon64;	   //  icon64
            result.Icon16 = icon16;
            result.Icon32 = icon32;
            result.Icon64 = icon64;
            return result;
        }

        private string GetIcon(enUICommand type, enIconSize size)
        {
            return _iconRepository.GetIcon(type, size);
        }
		public string GetTitle(enUICommand type)
		{
			return _titleRepository.GetTitle(type);
		}
    }

    public class UICommandRepository<T>
    {
        private readonly IUICommandIconRepository _iconRepository;
        private readonly UICommandTitleRepository _titleRepository;

        public UICommandRepository(IUICommandIconRepository iconRepository, UICommandTitleRepository titleRepository)
        {
            _titleRepository = titleRepository;
            _iconRepository = iconRepository;
        }

        public UICommand<T> Build(enUICommand type, Action<T> executeMethod, Func<T, bool> canExecuteMethod = null)
        {
            string title = _titleRepository.GetTitle(type);
            string icon64 = GetIcon(type, enIconSize.Size64);
            string icon32 = GetIcon(type, enIconSize.Size32);
            string icon16 = GetIcon(type, enIconSize.Size16);

            UICommand<T> result;
            if (canExecuteMethod == null)
                result = new UICommand<T>(executeMethod);
            else
                result = new UICommand<T>(executeMethod, canExecuteMethod);

            result.Title = title;
            result.Icon = icon64;
            result.Icon16 = icon16;
            result.Icon32 = icon32;
            result.Icon64 = icon64;
            return result;
        }

        private string GetIcon(enUICommand type, enIconSize size)
        {
            return _iconRepository.GetIcon(type, size);
        }

	
		
    }
}