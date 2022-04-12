using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using Count4U.Common.Interfaces;
using Count4U.Common.UserSettings;
using Count4U.Model.Audit;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.SelectionParams;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;
using NLog;
using Branch = Count4U.Model.Main.Branch;
using Customer = Count4U.Model.Main.Customer;
using Type = System.Type;

namespace Count4U.Common.Helpers
{
    public static class UtilsConvert
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public const string LTRMark = "\u200E";
        public const string RTMark = "\u200F";

        public static bool IsEmailValid(string email)
        {
            const string matchEmailPattern =
                     @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
              + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
              + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
              + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

            if (String.IsNullOrEmpty(email)) return false;
            return Regex.IsMatch(email, matchEmailPattern);
        }

        public static bool IsPhoneFaxValid(string phoneFax)
        {
            const string patern = @"^\+?[0-9]{9,10}$";
            if (String.IsNullOrEmpty(phoneFax))
                return false;
            return Regex.IsMatch(phoneFax, patern);
        }

        public static bool IsOkAsDouble(string value)
        {
            if (String.IsNullOrEmpty(value) == true) //important, do not remove 
                return true; 

            double res;
            bool ret = Double.TryParse(value, out res);
            return ret;
        }

        public static bool IsOkAsInt(string value)
        {
            if (String.IsNullOrEmpty(value))
                return true;

            int res;
            return Int32.TryParse(value, out res);
        }

        public static string Serialize(object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream memStr = new MemoryStream();

            string result = String.Empty;
            try
            {
                bf.Serialize(memStr, obj);
                memStr.Position = 0;

                result = Convert.ToBase64String(memStr.ToArray());
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Serialize", exc);
            }
            finally
            {
                memStr.Close();
            }

            return result;
        }

        public static T Deserialize<T>(string str) where T : class
        {
            BinaryFormatter bf = new BinaryFormatter();
            byte[] b = Convert.FromBase64String(str);
            MemoryStream ms = new MemoryStream(b);
            return bf.Deserialize(ms) as T;
        }

        public static void AddEnumToUriQuery(UriQuery query, Type enumType, object enumValue)
        {
            string key = enumType.Name;
            string value = enumValue.ToString();

            query.Add(key, value);
        }

        public static void AddEnumToDictionary(Dictionary<string, string> dic, Type enumType, object enumValue)
        {
            string key = enumType.Name;
            string value = enumValue.ToString();

            dic.Add(key, value);
        }

        public static T ConvertToEnum<T>(NavigationContext navigationContext) where T : struct
        {
            T result = default(T);

            Type type = typeof(T);
            string key = type.Name;

            if (navigationContext.Parameters.Any(r => r.Key == key))
            {
                string value = navigationContext.Parameters.FirstOrDefault(r => r.Key == key).Value;
                if (Enum.TryParse(value, true, out result))
                    return result;
            }


            return result;
        }

        public static void AddObjectToQuery(UriQuery query, INavigationRepository navigationRepo, Object obj, string queryKey = null)
        {
            var kvp = NavigationObject(navigationRepo, obj, queryKey);

            query.Add(kvp.Key, kvp.Value);
        }

        public static void AddObjectToDictionary(Dictionary<string, string> dic, INavigationRepository navigationRepo, Object obj, string queryKey = null)
        {
            var kvp = NavigationObject(navigationRepo, obj, queryKey);

            dic.Add(kvp.Key, kvp.Value);
        }

        private static KeyValuePair<string, string> NavigationObject(INavigationRepository navigationRepo, Object obj, string queryKey = null)
        {
            string key = String.IsNullOrEmpty(queryKey) ? Common.NavigationObjects.Default : queryKey;

            object clone = CreateClone(obj);

            string value = navigationRepo.Add(clone);

            return new KeyValuePair<string, string>(key, value);
        }

        public static object GetObjectFromNavigation(NavigationContext navigationContext, INavigationRepository navigationRepo, string queryKey = null, bool isRemove = false)
        {
            string key = String.IsNullOrEmpty(queryKey) ? Common.NavigationObjects.Default : queryKey;

            if (!navigationContext.Parameters.Any(r => r.Key == key))
                return null;

            string repoKey = navigationContext.Parameters.First(r => r.Key == key).Value;

            if (String.IsNullOrEmpty(repoKey))
                return null;

            object result = navigationRepo.Get(repoKey);

            if (isRemove)
                navigationRepo.Remove(repoKey);

            return result;
        }

        public static object CreateClone(object source)
        {
            if (source == null)
                return null;

            object destination = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    formatter.Serialize(memoryStream, source);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    destination = formatter.Deserialize(memoryStream);
                }
                catch (SerializationException exc)
                {
                    _logger.ErrorException("CreateClone", exc);
                }
            }

            return destination;
        }

        public static string DateToStringLong(DateTime dateTime)
        {
            System.Globalization.DateTimeFormatInfo format = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat;
            return dateTime.ToString(format.ShortDatePattern + " " + format.LongTimePattern);
        }

        public static string DateFormatLong()
        {
            System.Globalization.DateTimeFormatInfo format = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat;
            return format.ShortDatePattern + " " + format.LongTimePattern;
        }

        public static string DateHebrewConvert(DateTime date, enLanguage language)
        {
            switch (language)
            {
                case enLanguage.English:
                case enLanguage.Italian:
                    return date.ToString();
                case enLanguage.Russian:
                case enLanguage.Hebrew:
                    return date.ToString("HH:mm:ss dd-MM-yyyy");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Encoding StringToEncoding(string str)
        {
            if (String.IsNullOrEmpty(str))
                return null;

            Encoding result = null;

            int n;
            if (Int32.TryParse(str, out n))
            {
                try
                {
                    result = Encoding.GetEncoding(n);
                }
                catch (Exception)
                {
                    result = null;
                }
            }

            if (result == null)
            {
                try
                {
                    result = Encoding.GetEncoding(str);
                }
                catch (Exception)
                {
                    result = null;
                }
            }

            return result;
        }

        public static string DecimalPlaceNoRounding(double d)
        {
            return Convert.ToDecimal(String.Format("{0:0.00}", d)).ToString();
        }

        public static string HebrewDouble(double d)
        {
            string doubleFormatted = DecimalPlaceNoRounding(d);

            return LTRMark + doubleFormatted;

        }

        public static string HebrewInt(int d)
        {
            return LTRMark + d.ToString();
        }

        public static string HebrewText(string text)
        {
            return text + LTRMark;
        }

        public static string CustomerFancyName(Customer customer)
        {
			//if (customer == null) return "";
			string customerName = "Customer Unkown Name";
			try
			{
				customerName = customer.Name;
			}
			catch (Exception exc)
			{
				_logger.ErrorException("CustomerFancyName - Name", exc);
			}

			try
			{
				customerName = customerName +
				  UtilsConvert.LTRMark +
				  " " +
				  "[" +
				  customer.Code +
				  "]" +
				  " " +
				  UtilsConvert.LTRMark;
			}
			catch (Exception exc)
			{
				_logger.ErrorException("CustomerFancyName - Code", exc);
			}
			return customerName; 
        }

        public static string BranchFancyName(Branch branch)
        {
			//if (branch == null) return "";
			string branchName = "Branch Unkown Name";
			try
			{
				branchName = branch.Name;
			}
			catch (Exception exc)
			{
				_logger.ErrorException("BranchFancyName - Name", exc);
			}

			try
			{
				branchName = branchName +
				  UtilsConvert.LTRMark +
				  " " +
				  "[" +
				  branch.BranchCodeLocal +
				  "," +
				  branch.BranchCodeERP +
				  "]" +
				  " " +
				  UtilsConvert.LTRMark;
			}
			catch (Exception exc)
			{
				_logger.ErrorException("CustomerFancyName - Code", exc);
			}
			return branchName; 

			//return UtilsConvert.HebrewText(String.Format("{0}{1} [{2},{3}]{4}",
			//  branch.Name,
			//  UtilsConvert.LTRMark,
			//  branch.BranchCodeLocal,
			//  branch.BranchCodeERP,
			//  UtilsConvert.LTRMark));                     
        }

        public static string InventorFancyName(Inventor inventor, enLanguage language)
        {
            return
              UtilsConvert.LTRMark +
              DateHebrewConvert(inventor.InventorDate, language) +
              UtilsConvert.LTRMark +
              " " +
              "[" +
              inventor.Code +
              "]" +
              " " +
              UtilsConvert.LTRMark;
        }

        public static bool HaveDosHebrewCharInWord(string str)
        {
            return str.HaveDosHebrewCharInWord();
        }
    }
}