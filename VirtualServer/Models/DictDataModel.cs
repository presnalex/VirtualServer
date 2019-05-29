using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualServer.Models
{
    public class DictDataModel
    {
        public int VirtualServerId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime RemoveDateTime { get; set; }
        public bool DeleteFlag { get; set; }
    }
}