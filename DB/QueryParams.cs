namespace DB
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// Параметры для передачи в запрос к БД
    /// </summary>
    public sealed class QueryParams
    {
        /// <summary>
        /// Входные параметры
        /// </summary>
        private List<QueryParameter> iparams;

        /// <summary>
        /// Входные параметры
        /// </summary>
        public IEnumerable<QueryParameter> InParams
        {
            get
            {
                return this.iparams;
            }
        }

        /// <summary>
        /// Выходные параметры
        /// </summary>
        public Dictionary<string, object> OutParams
        {
            get;
            private set;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public QueryParams()
        {
            this.iparams = new List<QueryParameter>();
        }

        /// <summary>
        /// Конструктор для одного параметра
        /// </summary>
        /// <param name="name">Имя параметра</param>
        /// <param name="value">Значение</param>
        public QueryParams(string name, object value)
        {
            this.iparams = new List<QueryParameter> { new QueryParameter(name, value) };
        }

        /// <summary>
        /// Добавление значений параметров
        /// </summary>
        /// <param name="paramName">Имя параметра</param>
        /// <param name="paramValue">Значение параметра</param>
        /// <param name="direction">Входной/выходной параметр</param>
        public void Add(string paramName, object paramValue, ParameterDirection direction = ParameterDirection.Input)
        {
            var newParam = new QueryParameter(paramName, paramValue ?? DBNull.Value, direction);
            if (!this.iparams.Contains(newParam))
            {
                this.iparams.Add(newParam);
            }

            if (direction != ParameterDirection.Input && this.OutParams == null)
            {
                this.OutParams = new Dictionary<string, object>();
            }
        }

        /// <summary>
        /// Добавление Table-Valued parameter
        /// </summary>
        /// <param name="paramName">Имя параметра</param>
        /// <param name="paramValueArray">Массив значений</param>
        /// <param name="tvpFieldName">Имя колонки в TVP (поддерживаются только одноколоночные)</param>
        public void AddTVP<T>(string paramName, T[] paramValueArray, string tvpFieldName)
        {
            var p = new QueryArrayParameter(
                paramName,
                paramValueArray.Select(x => (object)x).ToArray(),
                tvpFieldName);
            this.iparams.Add(p);
        }

        /// <summary>
        /// Добавление Table-Valued parameter для произвольного числа колонок
        /// </summary>
        /// <param name="paramName">Имя параметра</param>
        /// <param name="table">Таблица TVP</param>
        public void AddTable(string paramName, DataTable table)
        {
            this.iparams.Add(new QueryTableParameter(paramName, table));
        }
    }
}
