
namespace VirtualServer.DB
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using global::DB;
    using VirtualServer.Models;


    /// <summary>
    /// Провайдер доступа к UP-SELL для Еспио
    /// </summary>
    public sealed class EspioProvider : BaseProvider
    {
        //private static List<EspioOfferPrototype> espioPrototypes;

        //private static List<ProjectCampCode> espioCampCode;

        private static Dictionary<string, int> prototypesForProjects = new Dictionary<string, int>();

        /// <summary>
        /// Конструктор  по-умолчанию
        /// </summary>
        /// <param name="cstr">Строка подключения к БД</param>
        public EspioProvider(string cstr)
            : base(cstr)
        {
        }


        public List<DictDataModel> GetDictData()
        {
            var rows = Db.GetValues("select * from dbo.VirtualServer", new QueryParams());
            var list1 = rows.Select(row => new DictDataModel
            {
                VirtualServerId = DbConnector.GetValue<int>(row["VirtualServerId"]),
                CreateDateTime = DbConnector.GetValue<string>(row["CreateDateTime"]),
                RemoveDateTime = DbConnector.GetValue<string>(row["RemoveDateTime"])
            }).ToList();
            return list1;
        }


        public List<DictDataModel> UpdateData(IEnumerable<DictDataModel> data)
        {
            foreach (var row in data)
            {
                if (row.DeleteFlag)
                {
                    DeleteRow(row);
                }
            }

            return GetDictData();
        }

        public void DeleteRow(DictDataModel row)
        {
            var p = new QueryParams("VirtualServerId", row.VirtualServerId);
            p.Add("RemoveDateTime", DateTime.Now);
            Db.Execute("pVirtualServerDel", p);

        }

        public List<DictDataModel> CreateRow()
        {
            Db.Execute("pVirtualServerCreate", new QueryParams("CreateDateTime", DateTime.Now));
            return GetDictData();

        }
    }
}