using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Service.Scanner
{
    public class ScanEventArgs : EventArgs
    {
        public string ScanContext { get; private set; }
        public ScanEventArgs(string ScanContext)
        {
            this.ScanContext = ScanContext;
        }
    }
}
