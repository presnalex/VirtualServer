namespace DB
{
    using System.Data;
    using System.Data.SqlClient;

    /// <summary>
    /// Table-valued параметр для произвольного количества колонок
    /// (для одноколоночных параметров <see cref="QueryArrayParameter"/>)
    /// </summary>
    class QueryTableParameter : QueryParameter
    {
        public QueryTableParameter(string key, DataTable value)
            : base(key, value)
        {
        }

        public override System.Data.SqlClient.SqlParameter AsSQL
        {
            get
            {
                var p = new SqlParameter(this.Name, this.Value);
                p.SqlDbType = SqlDbType.Structured;
                return p;
            }
        }
    }
}
