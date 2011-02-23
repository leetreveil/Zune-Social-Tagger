using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    public class Locale
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
        private static extern int GetGeoInfoA(long geoId, long geoType, StringBuilder lpGeoData,long cchData, long langId);

        [DllImport("kernel32.dll")]
        private static extern uint GetUserDefaultLCID();

        private const long GEO_ISO2 = 0x4;

        private static string GetCountryCode()
        {
            var countryCode = new StringBuilder();

            int geoId = GetGeoId();
            uint userDefaultLcid = GetUserDefaultLCID();
            var length = GetGeoInfoA(geoId, GEO_ISO2, null, 0, userDefaultLcid);

            if (length > 0)
            {
                countryCode = new StringBuilder(length);
                var result = GetGeoInfoA(geoId, GEO_ISO2, countryCode, length, userDefaultLcid);

                if (result == 0)
                    return String.Empty;
            }

            return countryCode.ToString();
        }

        private static int GetGeoId()
        {
            return GetUserGeoID(GEOCLASS.NATION);
        }
    }
}
