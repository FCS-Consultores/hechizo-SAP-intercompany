using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intercompany.models
{
    class DetallesOPORT
    {
        public int DocEntry { get; set; }
        public string ItemCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal LineTotal { get; set; }
        public decimal DiscPrcnt { get; set; }
        public string VatGroup { get; set; }
        public string PROYECTO { get; set; }
      public string ItemDescription { get; set; }

    }

}
