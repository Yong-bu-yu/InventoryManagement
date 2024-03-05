using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Model
{
    public class Mbase
    {
        [Key]
        public string UUID { get; set; }
        public string ICD { get; set; }
        public string INA { get; set; }
        public string UT { get; set; }
        public string ISJ { get; set; }
        public string IVER { get; set; }
        public string IPH { get; set; }
        public string IGG { get; set; }
        public string BZU { get; set; }
        public string BZU1 { get; set; }
        public string BZU2 { get; set; }
    }
}
