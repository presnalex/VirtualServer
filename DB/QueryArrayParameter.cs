namespace DB
{
    using System.Data;
    using System.Data.SqlClient;

    /// <summary>
    /// Обертка для Table-valued parameter
    /// </summary>
    internal sealed class QueryArrayParameter : QueryParameter
    {
        public QueryArrayParameter(string key, object[] val, string fieldName)
            : base(key, val)
        {
            this.FieldName = fieldName;
        }

        /// <summary>
        /// Имя поля в типе TVP
        /// </summary>
        public string FieldName
        {
            get;
            private set;
        }

        /// <summary>
        /// Передача DataTable в качестве параметра
        /// </summary>
        public override SqlParameter AsSQL
        {
            get
            {
                var dt = new DataTable("Items");
                dt.Columns.Add(this.FieldName);
                foreach (var rowVal in (object[])this.Value)
                {
                    dt.Rows.Add(rowVal);
                }
                var p = new SqlParameter(this.Name, dt);
                p.SqlDbType = SqlDbType.Structured;
                return p;
            }
        }
    }
}
