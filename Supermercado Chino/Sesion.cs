using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supermercado_Chino
{
    public class Sesion
    {
        public string Usuario_ID { get; set; }
        public string Sesion_ID { get; set; }
        public DateTime Fecha { get; set; }
        public string Hora_de_inicio { get; set; }
        public string Hora_finalizacion { get; set; }
    }
}
