
namespace VirtualServer.DB
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;

    /// <summary>
    /// Статический класс для подключения к слою доступа к данным
    /// </summary>
    public static class DataProvider
    {
        /// <summary>
        /// Леса в домене alfaintra.net — Москва и регионы
        /// </summary>
        internal static StringDictionary adDomains { get; private set; }

        #region Строки подключения

        /// <summary>
        /// Строка подключения к UP-SELL
        /// </summary>
        private static string upsellcs = ConfigurationManager.ConnectionStrings["MsSqlDB"].ConnectionString;

        #endregion

        #region Ленивые экземпляры провайдеров



        /// <summary>
        /// Экземпляр провайдера БД UP-SELL для работы с ЕСПиО
        /// </summary>
        private static Lazy<EspioProvider> espioData = new Lazy<EspioProvider>(() => new EspioProvider(upsellcs), true);



        #endregion

        #region Публикация ленивых экземпляров

        /// <summary>
        /// Провайдер работы с БД UP-SELL при работе с предложениями Еспио
        /// </summary>
        internal static EspioProvider espioProvider
        {
            get
            {
                return espioData.Value;
            }
        }

        #endregion

        /// <summary>
        /// Инициализация слоя доступа к данным
        /// </summary>
        /// <param name="upsellCS">Connection string to UP-SELL db</param>
        /// <param name="vipModCS">Connection string to VipModule</param>
        /// <param name="mamsCS">Connection string to MAMS</param>
        /// <param name="aclubCS">Connection string to AClub</param>
        /// <param name="domains">Словарь контроллеров домена</param>
        public static void Initialize(string upsellCS, StringDictionary domains)
        {
            adDomains = domains;
            upsellcs = upsellCS;
        }
    }
}