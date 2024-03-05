using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Service.Scanner
{
    public class ScanService
    {
        public event EventHandler<ScanEventArgs> Scan;
        
        public virtual void OnScan(ScanEventArgs e)
        {
            Scan?.Invoke(this, e);
        }
    }
}
