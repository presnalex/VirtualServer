
namespace VirtualServer.DB
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;


    public static class DataProvider
    {

        internal static StringDictionary adDomains { get; private set; }

        #region Строки подключения

        /// <summary>
        /// Строка подключения
        /// </summary>
        private static string upsellcs = ConfigurationManager.ConnectionStrings["MsSqlDB"].ConnectionString;

        #endregion

        #region Ленивые экземпляры провайдеров




        private static Lazy<EspioProvider> espioData = new Lazy<EspioProvider>(() => new EspioProvider(upsellcs), true);



        #endregion

        #region Публикация ленивых экземпляров


        internal static EspioProvider espioProvider
        {
            get
            {
                return espioData.Value;
            }
        }

        #endregion


        public static void Initialize(string upsellCS, StringDictionary domains)
        {
            adDomains = domains;
            upsellcs = upsellCS;
        }
    }
}