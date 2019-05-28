namespace DB
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Базовый класс провайдера доступа к БД
    /// </summary>
    public abstract class BaseProvider
    {
        internal readonly DbConnector connector;

        private BaseProvider()
        {
            throw new ArgumentException("Default constructor deprecated");
        }

        /// <summary>
        /// Коннектор к БД
        /// </summary>
        public DbConnector Db
        {
            get
            {
                return connector;
            }
        }

        /// <summary>
        /// Конструктор с инициализацией строки подключения
        /// </summary>
        /// <param name="connectionString">Строка подключения к БД</param>
        protected BaseProvider(string connectionString)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(connectionString),
                "Connection string is not presented");
            this.connector = new DbConnector(connectionString);
        }
    }
}
