using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supermercado_Chino
{
    public class Venta
    {
        public decimal Venta_ID { get; set; }
        public float Monto { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Usuario_ID { get; set; }
    }
}
