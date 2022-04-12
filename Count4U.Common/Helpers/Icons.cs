using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Count4U.Common.Helpers
{
    public static class Icons
    {
        #region Custom exceptions class

        public class IconNotFoundException : Exception
        {
            public IconNotFoundException(string fileName, int index)
                : base(string.Format("Icon with Id = {0} wasn't found in file {1}", index, fileName))
            {
            }
        }

        public class UnableToExtractIconsException : Exception
        {
            public UnableToExtractIconsException(string fileName, int firstIconIndex, int iconCount)
                : base(string.Format("Tryed to extract {2} icons starting from the one with id {1} from the \"{0}\" file but failed", fileName, firstIconIndex, iconCount))
            {
            }
        }

        #endregion

        #region DllImports

        /// <summary>
        /// Contains information about a file object. 
        /// </summary>
        struct SHFILEINFO
        {
            public IntPtr hIcon;

            public IntPtr iIcon;

            public uint dwAttributes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        [Flags]
        enum FileInfoFlags : int
        {

            SHGFI_ICON = 0x000000100,

            SHGFI_USEFILEATTRIBUTES = 0x000000010
        }


        [DllImport("Shell32", CharSet = CharSet.Auto)]
        extern static int ExtractIconEx(
            [MarshalAs(UnmanagedType.LPTStr)] 
            string lpszFile,
            int nIconIndex,
            IntPtr[] phIconLarge,
            IntPtr[] phIconSmall,
            int nIcons);

        [DllImport("Shell32", CharSet = CharSet.Auto)]
        extern static IntPtr SHGetFileInfo(
            string pszPath,
            int dwFileAttributes,
            out SHFILEINFO psfi,
            int cbFileInfo,
            FileInfoFlags uFlags);

        #endregion

        /// <summary>
        /// Two constants extracted from the FileInfoFlags, the only that are
        /// meaningfull for the user of this class.
        /// </summary>
        public enum SystemIconSize : int
        {
            Large = 0x000000000,
            Small = 0x000000001
        }

        /// <summary>
        /// Get the number of icons in the specified file.
        /// </summary>
        /// <param name="fileName">Full path of the file to look for.</param>
        /// <returns></returns>
        static int GetIconsCountInFile(string fileName)
        {
            return ExtractIconEx(fileName, -1, null, null, 0);
        }

        #region ExtractIcon-like functions

        public static void ExtractEx(string fileName, List<Icon> largeIcons,
            List<Icon> smallIcons, int firstIconIndex, int iconCount)
        {
            /*
             * Memory allocations
             */

            IntPtr[] smallIconsPtrs = null;
            IntPtr[] largeIconsPtrs = null;

            if (smallIcons != null)
            {
                smallIconsPtrs = new IntPtr[iconCount];
            }
            if (largeIcons != null)
            {
                largeIconsPtrs = new IntPtr[iconCount];
            }

            /*
             * Call to native Win32 API
             */

            int apiResult = ExtractIconEx(fileName, firstIconIndex, largeIconsPtrs, smallIconsPtrs, iconCount);
            if (apiResult != iconCount)
            {
                throw new UnableToExtractIconsException(fileName, firstIconIndex, iconCount);
            }

            /*
             * Fill lists
             */

            if (smallIcons != null)
            {
                smallIcons.Clear();
                foreach (IntPtr actualIconPtr in smallIconsPtrs)
                {
                    smallIcons.Add(Icon.FromHandle(actualIconPtr));
                }
            }
            if (largeIcons != null)
            {
                largeIcons.Clear();
                foreach (IntPtr actualIconPtr in largeIconsPtrs)
                {
                    largeIcons.Add(Icon.FromHandle(actualIconPtr));
                }
            }
        }

        public static List<Icon> ExtractEx(string fileName, SystemIconSize size,
            int firstIconIndex, int iconCount)
        {
            List<Icon> iconList = new List<Icon>();

            switch (size)
            {
                case SystemIconSize.Large:
                    ExtractEx(fileName, iconList, null, firstIconIndex, iconCount);
                    break;

                case SystemIconSize.Small:
                    ExtractEx(fileName, null, iconList, firstIconIndex, iconCount);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("size");
            }

            return iconList;
        }

        public static void Extract(string fileName, List<Icon> largeIcons, List<Icon> smallIcons)
        {
            int iconCount = GetIconsCountInFile(fileName);
            ExtractEx(fileName, largeIcons, smallIcons, 0, iconCount);
        }

        public static List<Icon> Extract(string fileName, SystemIconSize size)
        {
            int iconCount = GetIconsCountInFile(fileName);
            return ExtractEx(fileName, size, 0, iconCount);
        }

        public static Icon ExtractOne(string fileName, int index, SystemIconSize size)
        {
            try
            {
                List<Icon> iconList = ExtractEx(fileName, size, index, 1);
                return iconList[0];
            }
            catch (UnableToExtractIconsException)
            {
                throw new IconNotFoundException(fileName, index);
            }
        }

        public static void ExtractOne(string fileName, int index,
            out Icon largeIcon, out Icon smallIcon)
        {
            List<Icon> smallIconList = new List<Icon>();
            List<Icon> largeIconList = new List<Icon>();
            try
            {
                ExtractEx(fileName, largeIconList, smallIconList, index, 1);
                largeIcon = largeIconList[0];
                smallIcon = smallIconList[0];
            }
            catch (UnableToExtractIconsException)
            {
                throw new IconNotFoundException(fileName, index);
            }
        }

        #endregion

        public static Icon IconFromExtension(string extension, SystemIconSize size)
        {
            // Add the '.' to the extension if needed
            if (extension[0] != '.') extension = '.' + extension;

            //opens the registry for the wanted key.
            RegistryKey Root = Registry.ClassesRoot;
            RegistryKey ExtensionKey = Root.OpenSubKey(extension);
            ExtensionKey.GetValueNames();
            RegistryKey ApplicationKey =
                Root.OpenSubKey(ExtensionKey.GetValue("").ToString());

            //gets the name of the file that have the icon.
            string IconLocation =
                ApplicationKey.OpenSubKey("DefaultIcon").GetValue("").ToString();
            string[] IconPath = IconLocation.Split(',');

//            if (IconPath[1] == null) IconPath[1] = "0";
//            IntPtr[] Large = new IntPtr[1], Small = new IntPtr[1];
//
//            //extracts the icon from the file.
//            ExtractIconEx(IconPath[0],
//                Convert.ToInt16(IconPath[1]), Large, Small, 1);
//            return size == SystemIconSize.Large ?
//                Icon.FromHandle(Large[0]) : Icon.FromHandle(Small[0]);

            IntPtr[] Large = null;
            IntPtr[] Small = null;
            int iIconPathNumber = 0;

            if (IconPath.Length > 1)
                iIconPathNumber = 1;
            else
                iIconPathNumber = 0;


            if (IconPath[iIconPathNumber] == null) IconPath[iIconPathNumber] = "0";
            Large = new IntPtr[1];
            Small = new IntPtr[1];

            //extracts the icon from the file.
            if (iIconPathNumber > 0)
            {
                ExtractIconEx(IconPath[0],
                    Convert.ToInt16(IconPath[iIconPathNumber]), Large, Small, 1);
            }
            else
            {
                ExtractIconEx(IconPath[0],
                    Convert.ToInt16(0), Large, Small, 1);
            }


            return size == SystemIconSize.Large ?
                Icon.FromHandle(Large[0]) : Icon.FromHandle(Small[0]);

        }


        public static Icon IconFromExtensionShell(string extension, SystemIconSize size)
        {
            //add '.' if nessesry
            if (extension[0] != '.') extension = '.' + extension;

            //temp struct for getting file shell info
            SHFILEINFO fileInfo = new SHFILEINFO();

            SHGetFileInfo(
                extension,
                0,
                out fileInfo,
                Marshal.SizeOf(fileInfo),
                FileInfoFlags.SHGFI_ICON | FileInfoFlags.SHGFI_USEFILEATTRIBUTES | (FileInfoFlags)size);

            return Icon.FromHandle(fileInfo.hIcon);
        }

        public static Icon IconFromResource(string resourceName)
        {
            Assembly assembly = Assembly.GetCallingAssembly();            

            return new Icon(assembly.GetManifestResourceStream(resourceName));
        }
       
        public static void ExtractInformationsFromRegistryString(
            string regString, out string fileName, out int index)
        {
            if (regString == null)
            {
                throw new ArgumentNullException("regString");
            }
            if (regString.Length == 0)
            {
                throw new ArgumentException("The string should not be empty.", "regString");
            }

            index = 0;
            string[] strArr = regString.Replace("\"", "").Split(',');
            fileName = strArr[0].Trim();
            if (strArr.Length > 1)
            {
                int.TryParse(strArr[1].Trim(), out index);
            }
        }

        public static Icon ExtractFromRegistryString(string regString, SystemIconSize size)
        {
            string fileName;
            int index;
            ExtractInformationsFromRegistryString(regString, out fileName, out index);
            return ExtractOne(fileName, index, size);
        }
    }
}