using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using Count4U.Common.UserSettings.LogType;
using Count4U.Model;
using System.Linq;
using NLog;

namespace Count4U.Common.UserSettings
{
    public static class UserSettingsHelpers
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static string ColorToString(Color color)
        {
            return String.Format("{0:000},{1:000},{2:000}", color.R, color.G, color.B);
        }

        public static Color StringToColor(string color)
        {
            try
            {
                string[] split = color.Split(new char[] { ',' });
                byte r = Byte.Parse(split[0]);
                byte g = Byte.Parse(split[1]);
                byte b = Byte.Parse(split[2]);
                return Color.FromRgb(r, g, b);
            }
            catch
            {
                return Colors.White;
            }
        }

        public static Color StatusDefaultColorGet(IturStatusEnum status)
        {
            switch (status)
            {
                case IturStatusEnum.NoOneDoc:
                    return Color.FromRgb(250, 235, 215);
                //case IturStatusEnum.None:
                //    return Color.FromRgb(176, 196, 222);
                case IturStatusEnum.OneDocIsNotApprove:
                    return Color.FromRgb(212, 249, 212);
                case IturStatusEnum.OneDocIsApprove:
                    return Color.FromRgb(211, 211, 211);
                case IturStatusEnum.SomeDocIsNotApprove:
                    return Color.FromRgb(237, 213, 237);
                case IturStatusEnum.SomeDocIsApprove:
                    return Color.FromRgb(255, 218, 185);
                case IturStatusEnum.DisableAndNoOneDoc:
                    return Colors.LightSeaGreen;
                case IturStatusEnum.DisableAndOneDocIsNotApprove:
                    return Colors.LightSlateGray;
                case IturStatusEnum.DisableAndOneDocIsApprove:
                    return Colors.LightSteelBlue;
                case IturStatusEnum.DisableAndSomeDocIsNotApprove:
                    return Colors.LightYellow;
                case IturStatusEnum.DisableAndSomeDocIsApprove:
                    return Colors.Salmon;
                case IturStatusEnum.WarningConvert:
                    return Colors.Orange;
                default:
                    throw new ArgumentOutOfRangeException("status");
            }
        }

        public static Color StatusColorGet(IUserSettingsManager manager, string status)
        {
            if (manager == null)
            {
                IturStatusEnum en = (IturStatusEnum)Enum.Parse(typeof(IturStatusEnum), status);
                return StatusDefaultColorGet(en);
            }

            string color = manager.StatusColorGet(status);

            if (String.IsNullOrEmpty(color) == true)
            {
                IturStatusEnum en = (IturStatusEnum)Enum.Parse(typeof(IturStatusEnum), status);
                return StatusDefaultColorGet(en);
            }
            return StringToColor(color);
        }

        public static string StatusColorGetString(IUserSettingsManager manager, string status)
        {
            if (manager == null) return "";

            string color = manager.StatusColorGet(status);

            if (String.IsNullOrEmpty(color))
            {
                IturStatusEnum en = (IturStatusEnum)Enum.Parse(typeof(IturStatusEnum), status);
                return ColorToString(StatusDefaultColorGet(en));
            }
            return color;
        }

        public static Color StatusGroupDefaultColorGet(IturStatusGroupEnum statusGroup)
        {
            switch (statusGroup)
            {
                case IturStatusGroupEnum.Empty:
                    return Colors.White;
                case IturStatusGroupEnum.OneDocIsApprove:
                    return Colors.LightGreen;
                case IturStatusGroupEnum.AllDocsIsApprove:
                    return Colors.LightBlue;
                case IturStatusGroupEnum.NotApprove:
                    return Colors.Tomato;
                case IturStatusGroupEnum.DisableAndNoOneDoc:
                    return Colors.Gray;
                case IturStatusGroupEnum.DisableWithDocs:
                    return Colors.Brown;
                case IturStatusGroupEnum.Error:
                    return Colors.Orange;
				case IturStatusGroupEnum.None:
					return Colors.LightGray;
                default:
                    throw new ArgumentOutOfRangeException("statusGroup");
            }
        }

        public static Color StatusGroupColorGet(IUserSettingsManager manager, string statusGroup)
        {
            if (manager == null)
            {
                IturStatusGroupEnum en = (IturStatusGroupEnum)Enum.Parse(typeof(IturStatusGroupEnum), statusGroup);
                return StatusGroupDefaultColorGet(en);
            }

            string color = manager.StatusGroupColorGet(statusGroup);

            if (String.IsNullOrEmpty(color) == true)
            {
                IturStatusGroupEnum en = (IturStatusGroupEnum)Enum.Parse(typeof(IturStatusGroupEnum), statusGroup);
                return StatusGroupDefaultColorGet(en);
            }
            return StringToColor(color);
        }

        public static string StatusGroupColorGetString(IUserSettingsManager manager, string statusGroup)
        {
            if (manager == null) return String.Empty;

            string color = manager.StatusGroupColorGet(statusGroup);

            if (String.IsNullOrEmpty(color))
            {
                IturStatusGroupEnum en = (IturStatusGroupEnum)Enum.Parse(typeof(IturStatusGroupEnum), statusGroup);
                return ColorToString(StatusGroupDefaultColorGet(en));
            }
            return color;
        }

        public static Dictionary<MessageTypeEnum, bool> LogTypeListGet(IUserSettingsManager manager)
        {
            Dictionary<MessageTypeEnum, bool> result = new Dictionary<MessageTypeEnum, bool>();

            foreach (MessageTypeEnum en in Enum.GetValues(typeof(MessageTypeEnum)))
            {
                result.Add(en, false);
            }

            LogTypeElementCollection collection = manager.LogTypeGet();

            foreach (LogTypeElement element in collection)
            {
                MessageTypeEnum en;

                if (Enum.TryParse(element.Name, true, out en))
                {
                    result[en] = element.IsEnabled;
                }
            }

            return result;
        }

        public static Encoding GlobalEncodingGet(IUserSettingsManager manager)
        {
            int codePage = manager.GlobalEncodingGet();

            return EncodingFromCodepage(codePage);
        }

        public static void GlobalEncodingSet(IUserSettingsManager mananger, Encoding encoding)
        {
            mananger.GlobalEncodingSet(encoding.CodePage);
        }

        public static Encoding GlobalEncodingDefaultGet()
        {
            return EncodingFromCodepage(Count4U.Common.UserSettings.CommonElement.DefaultGlobalEncoding);
        }

        private static Encoding EncodingFromCodepage(int codepage)
        {
            try
            {
                return Encoding.GetEncoding(codepage);
            }
            catch (Exception exc)
            {
                _logger.ErrorException("GlobalEncodingGet", exc);
            }

            return null;
        }
    }
}