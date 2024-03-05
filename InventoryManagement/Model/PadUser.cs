using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Model
{
    public class PadUser
    {
        [Key]
        public string sys_key_uuid { get; set; }
        public string CN_LOGIN { get; set; }
        public string CN_USERID { get; set; }
        public string CN_USERNUMBER { get; set; }
        public string CN_NAME { get; set; }
        public string CN_Download_Password { get; set; }
        public string CN_Alternate_Password { get; set; }
        public string CN_Role { get; set; }
        public string CN_Management { get; set; }
        public string CN_Group_Name { get; set; }
    }
}
