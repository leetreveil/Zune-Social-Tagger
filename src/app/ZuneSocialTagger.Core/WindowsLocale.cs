using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ZuneSocialTagger.Core
{
    public class WindowsLocale
    {
        /// <summary>
        /// Gets the current language + country e.g. en-FR
        /// </summary>
        /// <returns></returns>
        public static string GetLocale()
        {
            try
            {
                return "en" + "-" + GetCountryCode();
            }
            catch (Exception)
            {
                //TODO: Log to file
                return "";
            }
        }

        private enum GEOCLASS : int
        {
            NATION = 16,
            REGION = 14,
        };

        [DllImport("kernel32.dll", ExactSpelling = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int GetUserGeoID(GEOCLASS GeoClass);

        [DllImport("kernel32.dll", ExactSpelling = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int GetGeoInfoA(int geoId, int geoType, StringBuilder lpGeoData, int cchData, int langId);

        [DllImport("kernel32.dll")]
        private static extern int GetUserDefaultLCID();

        private const int GEO_ISO2 = 0x4;

        private static string GetCountryCode()
        {
            var countryCode = new StringBuilder();

            int geoId = GetUserGeoID(GEOCLASS.NATION);
            int userDefaultLcid = GetUserDefaultLCID();

            countryCode = new StringBuilder(3);
            var result = GetGeoInfoA(geoId, GEO_ISO2, countryCode, 3, userDefaultLcid);

            if (result == 0)
                return String.Empty;

            return countryCode.ToString();
        }

    }
}
