using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supermercado_Chino
{
    public class Usuario
    {
        public decimal Usuario_ID { get; set; }
        public string Nombre { get; set; }
        public int Acceso { get; set; }
        public string Email { get; set; }
        public string Celular { get; set; }
        public string Contraseña { get; set; }
        public Usuario(string contraseña, string nombre)
        {

            Contraseña = contraseña;
            Nombre = nombre;

        }
    }
}
