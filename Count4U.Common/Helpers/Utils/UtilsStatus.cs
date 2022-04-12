using System;
using System.Collections.Generic;
using System.Linq;
using Count4U.Common.UserSettings;
using Count4U.Model;

namespace Count4U.Common.Helpers
{
    public static class UtilsStatus
    {
        public static string FromIturStatusToString(IturStatusEnum status)
        {
            switch (status)
            {
                case IturStatusEnum.NoOneDoc:
                    return IturStatusTitle.NoOneDoc;
                //case IturStatusEnum.None:
                //    return IturStatusTitle.None;
                case IturStatusEnum.OneDocIsNotApprove:
                    return IturStatusTitle.OneDocIsNotApprove;
                case IturStatusEnum.OneDocIsApprove:
                    return IturStatusTitle.OneDocIsApprove;
                case IturStatusEnum.SomeDocIsNotApprove:
                    return IturStatusTitle.SomeDocIsNotApprove;
                case IturStatusEnum.SomeDocIsApprove:
                    return IturStatusTitle.SomeDocIsApprove;
                case IturStatusEnum.DisableAndSomeDocIsApprove:
                    return IturStatusTitle.DisableAndSomeDocIsApprove;
                case IturStatusEnum.DisableAndSomeDocIsNotApprove:
                    return IturStatusTitle.DisableAndSomeDocIsNotApprove;
                case IturStatusEnum.DisableAndNoOneDoc:
                    return IturStatusTitle.DisableAndNoOneDoc;
                case IturStatusEnum.DisableAndOneDocIsApprove:
                    return IturStatusTitle.DisableAndOneDocIsApprove;
                case IturStatusEnum.DisableAndOneDocIsNotApprove:
                    return IturStatusTitle.DisableAndOneDocIsNotApprove;
                case IturStatusEnum.WarningConvert:
                    return IturStatusTitle.WarningConvert;
                default:
                    throw new ArgumentOutOfRangeException("status");
            }
        }

        public static string FromIturStatusGroupToString(IturStatusGroupEnum statusGroup)
        {
            switch (statusGroup)
            {
                case IturStatusGroupEnum.Empty:
                    return IturStatusGroupTitle.Empty;
                case IturStatusGroupEnum.OneDocIsApprove:
                    return IturStatusGroupTitle.OneDocIsApprove;
                case IturStatusGroupEnum.AllDocsIsApprove:
                    return IturStatusGroupTitle.AllDocsIsApprove;
                case IturStatusGroupEnum.NotApprove:
                    return IturStatusGroupTitle.NotApprove;
                case IturStatusGroupEnum.DisableAndNoOneDoc:
                    return IturStatusGroupTitle.DisableAndNoOneDoc;
                case IturStatusGroupEnum.DisableWithDocs:
                    return IturStatusGroupTitle.DisableWithDocs;
                case IturStatusGroupEnum.Error:
                    return IturStatusGroupTitle.Error;
				case IturStatusGroupEnum.None:
					return IturStatusGroupTitle.None;
                default:
                    throw new ArgumentOutOfRangeException("statusGroup");
            }
        }

        public static string FromStatusBitToColor(Dictionary<int, IturStatusEnum> bitStatus, int iturBit, IUserSettingsManager manager)
        {
            if (bitStatus == null) return String.Empty;
            int n = bitStatus.Keys.FirstOrDefault(r => r == iturBit); //bit
            IturStatusEnum en = IturStatusEnum.NoOneDoc;
            if (bitStatus.Keys.Any(r => r == n)) //enum from bit
                en = bitStatus.Where(r => r.Key == n).First().Value;
            return UserSettingsHelpers.StatusColorGetString(manager, en.ToString());
        }

        public static string FromStatusGroupBitToColor(Dictionary<int, IturStatusGroupEnum> bitStatus, int statusGroupBit, IUserSettingsManager manager)
        {
            if (bitStatus == null) return String.Empty;
            int n = bitStatus.Keys.FirstOrDefault(r => r == statusGroupBit); //bit
            IturStatusGroupEnum en = IturStatusGroupEnum.Empty;
            if (bitStatus.Keys.Any(r => r == n)) //enum from bit
                en = bitStatus.Where(r => r.Key == n).First().Value;
            return UserSettingsHelpers.StatusGroupColorGetString(manager, en.ToString());
        }  
    }
}