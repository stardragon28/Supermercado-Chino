using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supermercado_Chino
{
    public class Articulo
    {
        public decimal Articulo_ID { get; set; }
        public string Detalle { get; set; }
        public string Presentacion { get; set; }
        public float Precio_Compra { get; set; }
        public float Precio_Venta { get; set; }
        public int Stock { get; set; }
        public string Proveedor {  get; set; }

        public Articulo(decimal articulo_ID, string detalle, string presentacion, float precio_Compra, float precio_Venta, int stock, string proveedor)
        {
            Articulo_ID = articulo_ID;
            Detalle = detalle;
            Presentacion = presentacion;
            Precio_Compra = precio_Compra;
            Precio_Venta = precio_Venta;
            Stock = stock;
            Proveedor = proveedor;
        }
    }
}
