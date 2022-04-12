using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Count4U.Common.Helpers;
using Count4U.Common.UserSettings;
using Count4U.Model.Interface;
using Count4U.Planogram.Lib.Enums;
using Count4U.Planogram.View.PlanObjects;
using Microsoft.Practices.Unity;

namespace Count4U.Planogram.Lib
{
    public class Helpers
    {
        public static Cursor DefaultCursor = null;

        public const double MinCanvasWidth = 300;
        public const double MaxCanvasWidth = 42500;
        public const double MinCanvasHeight = 300;
        public const double MaxCanvasHeight = 42500;

        public const double DefaultCanvasWidth = 800;
        public const double DefaultCanvasHeight = 600;
        public const double DefaultCanvasZoom = 100;

        public const double MetersToPixesRatio = 0.0166666;

        public const double MinWidthToObject = 12;
        public const double MinHeightToObject = 12;
        public const double MinWidthToSpecialObject = 12;
        public const double MinHeightToSpecialObject = 12;

        public const double MinWidthToCircleDisappear = 37;
        public const double MinHeightToCircleDisappear = 37;

        public const int ZIndexFront = 20;
        public const int ZIndexBack = 10;
        public const int ZIndexSelectionRectangle = 1000;

        public const int CopiedObjectShiftX = 20;
        public const int CopiedObjectShiftY = 20;

        public static int GetDefaultZIndex(enPlanObjectType type)
        {
            switch (type)
            {
                case enPlanObjectType.Shelf:
                case enPlanObjectType.Wall:
                case enPlanObjectType.Window:
                case enPlanObjectType.Text:
                case enPlanObjectType.Picture:
                    return ZIndexFront;
                case enPlanObjectType.Location:
                    return ZIndexBack;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        public static double MetersToPixes(double meters)
        {
            return meters / Helpers.MetersToPixesRatio;
        }

        public static double PixelsToMeters(double pixels)
        {
            return pixels * Helpers.MetersToPixesRatio;
        }

        public static PlanObject CreatePlanObjectByType(enPlanObjectType type, DrawingCanvas canvas, IUnityContainer container, string code)
        {
            PlanObject result;
            switch (type)
            {
                case enPlanObjectType.Shelf:
                    result = new  PlanShelf(canvas);
                    break;
                case enPlanObjectType.Wall:
                    result = new PlanWall();
                    break;
                case enPlanObjectType.Window:
                    result = new PlanWindow();
                    break;
                case enPlanObjectType.Location:
                    result = new PlanLocation();
                    break;
                case enPlanObjectType.Text:
                    result = new PlanText();
                    break;
                case enPlanObjectType.Picture:
                    result = new PlanPicture(container.Resolve<IDBSettings>());
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }

            result.Code = code;

            return result;
        }

        public static string CodeGenerate()
        {
            return Utils.CodeNewGenerate(5);
        }

        public static Color FromPercentageToColor(double p, IUserSettingsManager userSettingsManager, bool isEmpty)
        {
            if (isEmpty)
            {
                return userSettingsManager.PlanEmptyColorGet();
            }

            if (p < 0 || p > 100) return Colors.Gray;

            //            const int r1 = 160;
            //            const int g1 = 160;
            //            const int b1 = 160;
            //            const int r2 = 38;
            //            const int g2 = 127;
            //            const int b2 = 0;

            Color from = userSettingsManager.PlanZeroColorGet();
            Color to = userSettingsManager.PlanHundredColorGet();

            int r1 = from.R;
            int g1 = from.G;
            int b1 = from.B;
            int r2 = to.R;
            int g2 = to.G;
            int b2 = to.B;

            int r3 = (int)(r1 + (((r2 - r1) * p) / 100));
            int g3 = (int)(g1 + (((g2 - g1) * p) / 100));
            int b3 = (int)(b1 + (((b2 - b1) * p) / 100));

            return Color.FromRgb((byte)r3, (byte)g3, (byte)b3);
        }

        public static List<string> EnumeratePictures(IDBSettings dbSettings)
        {
            List<string> result = new List<string>();

            List<string> validExtensions = new List<string>() { ".png", ".jpg", ".jpeg", ".bmp" };

            string folder = dbSettings.PlanogramPictureFolderPath();
            foreach (string file in Directory.EnumerateFiles(folder))
            {
                FileInfo fi = new FileInfo(file);

                if (validExtensions.Contains(fi.Extension.ToLower()) == false) continue;

               result.Add(fi.FullName);
            }

            return result;
        }
    }
}