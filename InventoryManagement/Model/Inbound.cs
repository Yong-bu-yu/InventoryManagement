using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Model
{
    public class Inbound
    {
        [Key]
        public string RNO { get; set; }
        public string WBS { get; set; }
        public string ICD { get; set; }
        public string INA { get; set; }
        public string QUA { get; set; }
        public string FQUA { get; set; }
        public string UT { get; set; }
        public string KWCODE { get; set; }
        public string KWNAME { get; set; }
        public string CKNAME { get; set; }
        public string UBY { get; set; }
        public string ISJ { get; set; }
        public string STA { get; set; }
        public string UPSTATE { get; set; }
        public string UPTIME { get; set; }
        public string BZU { get; set; }
        public string CTI { get; set; }
        public string ETI { get; set; }
        public string EMP { get; set; }
        public string KQNAME { get; set; }
        public string PCD { get; set; }
    }
}
