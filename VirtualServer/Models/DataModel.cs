using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualServer.Models
{
    public class DataModel
    {
        public List<DictDataModel> DictData { get; set; }
        public int UsageSecondsFromDB { get; set; }
    }
}