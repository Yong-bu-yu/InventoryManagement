using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Model
{
    public class SystemManagement
    {
        [Key]
        public string UUID {  get; set; }
        public string IpAddress { get; set; }
    }
}
