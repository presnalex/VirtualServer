using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualServer.Models
{
    public class DictDataModel
    {
        public int VirtualServerId { get; set; }
        public string CreateDateTime { get; set; }
        public string RemoveDateTime { get; set; }
        public bool DeleteFlag { get; set; }
    }
}