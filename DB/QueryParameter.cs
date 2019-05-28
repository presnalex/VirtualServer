namespace DB
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    /// <summary>
    /// Значение для передачи в SqlParameter
    /// </summary>
    public class QueryParameter : IEquatable<QueryParameter>
    {
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Значение
        /// </summary>
        public object Value
        {
            get;
            private set;
        }

        /// <summary>
        /// SqlParameter.Direction
        /// </summary>
        public ParameterDirection Direction
        {
            get;
            private set;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="key">SQL parameter name</param>
        /// <param name="value">SQL parameter value</param>
        /// <param name="direction">SQL parameter direction</param>
        public QueryParameter(string key, object value, ParameterDirection direction = ParameterDirection.Input)
        {
            Name = key;
            Value = value;
            Direction = direction;
        }

        /// <summary>
        /// SQL параметр
        /// </summary>
        public virtual SqlParameter AsSQL
        {
            get
            {
                var s = new SqlParameter(Name, Value);
                if (Direction != ParameterDirection.Input)
                {
                    if (object.ReferenceEquals(Value, null) || Value is string || Value.GetType() == typeof(object))
                    {
                        s.SqlDbType = SqlDbType.VarChar;
                        s.Size = 256;
                    }

                    s.Direction = Direction;
                }

                return s;
            }
        }

        #region Overriden methods and operators

        /// <summary>
        /// Выводит значение параметра ParamValue
        /// </summary>
        public sealed override string ToString()
        {
            return this.Value.ToString();
        }

        /// <summary>
        /// Equality
        /// </summary>
        /// <param name="obj">The object instance to compare</param>
        /// <returns>True если ключи имеют одно и то же имя</returns>
        public override bool Equals(object obj)
        {
            if (obj is QueryParameter)
            {
                return this.Equals((QueryParameter)obj);
            }

            return false;
        }

        /// <summary>
        /// Хеш ключа
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        /// <summary>
        /// Сравнение двух SQL параметров
        /// </summary>
        /// <param name="other">Другой SQL параметр</param>
        /// <returns>true если поля Name у обоих SQL параметров равны (значения игнорируются)</returns>
        public bool Equals(QueryParameter other)
        {
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return this.Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public static bool operator ==(QueryParameter first, QueryParameter second)
        {
            if (object.ReferenceEquals(first, second))
            {
                return true;
            }

            if (object.ReferenceEquals(first, null))
            {
                return false;
            }

            return first.Equals(second);
        }

        public static bool operator !=(QueryParameter first, QueryParameter second)
        {
            return !(first == second);
        }

        #endregion
    }
}
