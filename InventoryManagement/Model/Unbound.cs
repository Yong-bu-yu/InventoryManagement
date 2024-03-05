using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Model
{
    public class Unbound
    {
        public string CRNO { get; set; }
        public string RNO { get; set; }
        [Key]
        public string CNO { get; set; }
        public string LCNO { get; set; }
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
        public string LLRNAMEPZ { get; set; }
        public string LLRNAME { get; set; }
        public string LLRDLN { get; set; }
        public string LLRID { get; set; }

    }
}
