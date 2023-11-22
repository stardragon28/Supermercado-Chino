using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp.text.pdf.qrcode;
using ZXing;
using ZXing.QrCode;

namespace Supermercado_Chino
{
    public partial class FRM_Menu : Form
    {
        public string ConnectionString { get; set; } = @"Server=localhost\SQLEXPRESS;" +
    "Database=ChinoDB; Trusted_Connection=True;TrustServerCertificate=True";
        public FRM_Menu()
        {
            InitializeComponent();
        }
        private void btn_Salir_Click(object sender, EventArgs e)
        {
            Login M_Login = new ();
            Hide();
            M_Login.Show();
            Close();
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (float.TryParse(txt_Pago.Text,out _) && float.TryParse(txt_Monto.Text,out _))
            {
                float Vuelto = float.Parse(txt_Pago.Text) - float.Parse(txt_Monto.Text);
                txt_Vuelto.Text = Vuelto.ToString();
            }
           
        }

        private void btn_Ventas_Click(object sender, EventArgs e)
        {
            Panel_Ventas.BringToFront();
            List<Articulo> Articulos = new List<Articulo>();
            using SqlConnection connection = new SqlConnection(ConnectionString);
            connection.Open();
            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Articulos", connection);
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            while (sqlDataReader.Read())
            {
                Articulos.Add(new Articulo(decimal.Parse(sqlDataReader.GetValue(0).ToString()),sqlDataReader.GetValue(1).ToString(),sqlDataReader.GetValue(2).ToString(),float.Parse(sqlDataReader.GetValue(3).ToString()),float.Parse(sqlDataReader.GetValue(4).ToString()),int.Parse(sqlDataReader.GetValue(5).ToString()),sqlDataReader.GetValue(6).ToString()));
            }
            T_Articulos.DataSource = Articulos;
        }
        private void T_Articulos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            foreach (DataGridViewRow fila in T_Carrito.Rows)
            {
                if (fila.Cells[0].Value != null)
                {
                    if (fila.Cells[0].Value == T_Articulos.SelectedRows[0].Cells[0].Value)
                    {
                        fila.Cells[4].Value = int.Parse(fila.Cells[4].Value.ToString()) + 1;
                        fila.Cells[5].Value = float.Parse(fila.Cells[3].Value.ToString()) * int.Parse(fila.Cells[4].Value.ToString());
                    }
                    else
                    {
                        T_Carrito.Rows.Add(T_Articulos.SelectedRows[0].Cells[0].Value, T_Articulos.SelectedRows[0].Cells[1].Value, T_Articulos.SelectedRows[0].Cells[2].Value, T_Articulos.SelectedRows[0].Cells[4].Value);
                        fila.Cells[4].Value = 1;
                        fila.Cells[5].Value = float.Parse(fila.Cells[3].Value.ToString()) * int.Parse(fila.Cells[4].Value.ToString());
                    }
                }
                
            }
        }
        private void btn_Ticket_Click(object sender, EventArgs e)
        {
            using SaveFileDialog saveFileDiag = new() { Filter = "PDF file|*.pdf", ValidateNames = true };
            if (saveFileDiag.ShowDialog() == DialogResult.OK)
            {
                List<Int64> articleIDs = new(); List<int> articlesAmount = new();
                for (int i = 0; i < T_Carrito.RowCount; i++)
                {
                        articleIDs.Add(Int64.Parse(T_Carrito["Código2", i].Value.ToString()));
                        articlesAmount.Add(int.Parse(T_Carrito["Cantidad", i].Value.ToString()));
                }
                //UpdateStock(articleIDs, articlesAmount);

                iTextSharp.text.Rectangle rect = new(360, 720);
                Document doc = new(rect, 10, 10, 10, 10);
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(saveFileDiag.FileName, FileMode.Create));
                doc.Open();

                iTextSharp.text.Font font = new(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 11);
                doc.Add(new Paragraph($"SUPERMERCADO:Los dos chinos") { Alignment = Element.ALIGN_CENTER });
                doc.Add(new Paragraph($"CUIT:28-28828228-2") { Alignment = Element.ALIGN_CENTER });
                doc.Add(new Paragraph($"DIRECCIÓN:San martin 2") { Alignment = Element.ALIGN_CENTER });
                doc.Add(new Paragraph("------------------------------------------------------------------------------------")
                { Alignment = Element.ALIGN_CENTER });

                PdfPTable table = new(T_Carrito.ColumnCount - 1) { WidthPercentage = 103 };
                table.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                for (int i = 1; i < T_Carrito.ColumnCount; i++) { table.AddCell(new Paragraph(T_Carrito.Columns[i].HeaderText, font)); }
                float totalPrice = 0;
                for (int i = 0; i < T_Carrito.Rows.Count - 1; i++)
                {
                    table.AddCell(new Paragraph($"{T_Carrito[1, i].Value}", font));
                    table.AddCell(new Paragraph($"{T_Carrito[2, i].Value}", font));
                    table.AddCell(new Paragraph($"{T_Carrito[3, i].Value}", font));
                    table.AddCell(new Paragraph($"{T_Carrito[4, i].Value}", font));
                    table.AddCell(new Paragraph($"{T_Carrito[5, i].Value}", font));
                    totalPrice += float.Parse(T_Carrito["Subtotal", i].Value.ToString().Replace("$", string.Empty));
                }
                table.CompleteRow();
                doc.Add(table);
                doc.Add(new Paragraph($"MONTO TOTAL: ${totalPrice}") { Alignment = Element.ALIGN_CENTER });
                doc.Add(new Paragraph($"FECHA Y HORA: {DateTime.Now:dd/MM/yyyy HH:mm:ss}") { Alignment = Element.ALIGN_CENTER });
                doc.Add(new Paragraph($"TICKET N° 0-1") { Alignment = Element.ALIGN_CENTER });
                doc.Close();
                writer.Close();
            }
        }

        private void btn_Codigo_Click(object sender, EventArgs e)
        {
            if (T_Articulos.SelectedRows.Count == 0) MessageBox.Show($"No se seleccionó ningún artículo",
    "Operación fallida", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (T_Articulos.SelectedRows.Count == 1)
            {
                using SaveFileDialog saveFileDiag = new() { Filter = "PDF file|*.pdf", ValidateNames = true };
                if (saveFileDiag.ShowDialog() == DialogResult.OK)
                {
                    iTextSharp.text.Rectangle rect = new(360, 720);
                    Document doc = new(rect, 10, 10, 10, 10);
                    PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(saveFileDiag.FileName, FileMode.Create));
                    doc.Open();
                    iTextSharp.text.Font font = new(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 11);
                    BarcodeWriter BCWriter = new()
                    {
                        Format = BarcodeFormat.EAN_13,
                        Options = new QrCodeEncodingOptions
                        { Width = 400, Height = 100 }
                    };
                    System.Drawing.Image image = BCWriter.Write($"{T_Articulos.SelectedRows[0].Cells[0].Value}");
                    iTextSharp.text.Image iItextSharpImg = iTextSharp.text.Image.GetInstance(image, System.Drawing.Imaging.ImageFormat.Jpeg);
                    iItextSharpImg.Alignment = Element.ALIGN_CENTER;
                    doc.Add(iItextSharpImg);
                    doc.Add(new Paragraph($"{T_Articulos.SelectedRows[0].Cells[1].Value} - {T_Articulos.SelectedRows[0].Cells[2].Value} - "
                        + $"{T_Articulos.SelectedRows[0].Cells[3].Value}")
                    { Alignment = Element.ALIGN_CENTER });
                    doc.Close();
                    writer.Close();
                }
            }
            else { MessageBox.Show($"Seleccione solamente (1) un artículo", "Operación fallida", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btn_Camaras_Click(object sender, EventArgs e)
        {
            FRM_Camaras camaras = new FRM_Camaras();
            camaras.Show();
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void btn_Articulos_Click(object sender, EventArgs e)
        {
            Tabla.DataSource = null;
            Panel_Articulos.BringToFront();
            List<Articulo> Articulos = new List<Articulo>();
            using SqlConnection connection = new SqlConnection(ConnectionString);
            connection.Open();
            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Articulos", connection);
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            while (sqlDataReader.Read())
            {
                Articulos.Add(new Articulo(decimal.Parse(sqlDataReader.GetValue(0).ToString()), sqlDataReader.GetValue(1).ToString(), sqlDataReader.GetValue(2).ToString(), float.Parse(sqlDataReader.GetValue(3).ToString()), float.Parse(sqlDataReader.GetValue(4).ToString()), int.Parse(sqlDataReader.GetValue(5).ToString()),sqlDataReader.GetValue(6).ToString()));
            }
            Tabla.DataSource = Articulos;
            
            
        }

        private void txt_Ingresar_Click(object sender, EventArgs e)
        {
            List<TextBox> textBoxes = new List<TextBox>() { txt_Codigo1, txt_Detalle1, txt_Presentacion1, txt_Compra1, txt_Venta1, txt_Stock1, txt_Proveedor };
            var txt_vacio = textBoxes.FirstOrDefault(tb => tb.Text == string.Empty);
            if (txt_vacio != null)
            {
                MessageBox.Show("Campos sin llenar");
            }
            else
            {
                using SqlConnection connection = new SqlConnection(ConnectionString);
                connection.Open();
                SqlCommand sqlCommand = new SqlCommand("Insert into Articulos(Articulo_ID, Detalle, Presentacion, Precio_Compra, Precio_Venta, Stock, Proveedor) Values(@Articulo_ID, @Detalle, @Presentacion, @Precio_Compra, @Precio_Venta, @Stock, @Proveedor)", connection);
                sqlCommand.Parameters.AddWithValue("@Articulo_ID", txt_Codigo1.Text);
                sqlCommand.Parameters.AddWithValue("@Detalle", txt_Detalle1.Text);
                sqlCommand.Parameters.AddWithValue("@Presentacion", txt_Presentacion1.Text);
                sqlCommand.Parameters.AddWithValue("@Precio_Compra", txt_Compra1.Text);
                sqlCommand.Parameters.AddWithValue("@Precio_Venta", txt_Venta1.Text);
                sqlCommand.Parameters.AddWithValue("@Stock", txt_Stock1.Text);
                sqlCommand.Parameters.AddWithValue("@Proveedor", txt_Proveedor.Text);
                sqlCommand.ExecuteNonQuery();
                sqlCommand.Dispose();
                connection.Close();
            }
        }

        private void txt_Modificar_Click(object sender, EventArgs e)
        {
            List<TextBox> textBoxes = new List<TextBox>() { txt_Codigo1, txt_Detalle1, txt_Presentacion1, txt_Compra1, txt_Venta1, txt_Stock1, txt_Proveedor };
            var txt_vacio = textBoxes.FirstOrDefault(tb => tb.Text == string.Empty);
            if (txt_vacio != null)
            {
                MessageBox.Show("Campos sin llenar");
            }
            else
            {
                using SqlConnection connection = new SqlConnection(ConnectionString);
                connection.Open();
                SqlCommand sqlCommand = new SqlCommand("update Articulos set Articulo_ID = @Articulo_ID, Detalle = @Detalle, Presentacion = @Presentacion, Precio_Compra = @Precio_Compra, Precio_Venta = @Precio_Venta, Stock = @Stock, Proveedor = @Proveedor where Articulo_Id = @Articulo_ID", connection);
                sqlCommand.Parameters.AddWithValue("@Articulo_ID", txt_Codigo1.Text);
                sqlCommand.Parameters.AddWithValue("@Detalle", txt_Detalle1.Text);
                sqlCommand.Parameters.AddWithValue("@Presentacion", txt_Presentacion1.Text);
                sqlCommand.Parameters.AddWithValue("@Precio_Compra", txt_Compra1.Text);
                sqlCommand.Parameters.AddWithValue("@Precio_Venta", txt_Venta1.Text);
                sqlCommand.Parameters.AddWithValue("@Stock", txt_Stock1.Text);
                sqlCommand.Parameters.AddWithValue("@Proveedor", txt_Proveedor.Text);
                sqlCommand.ExecuteNonQuery();
                sqlCommand.Dispose();
                connection.Close();
            }

        }

        private void txt_Eliminar_Click(object sender, EventArgs e)
        {
            if  (txt_Codigo1.Text == string.Empty)
            {
                MessageBox.Show("Campos sin llenar");
            }
            else
            {
                using SqlConnection connection = new SqlConnection(ConnectionString);
                connection.Open();
                SqlCommand sqlCommand = new SqlCommand("delete from Articulos where Articulo_ID = @Articulo_ID", connection);
                sqlCommand.Parameters.AddWithValue("@Articulo_ID", txt_Codigo1.Text);
            }
        }

        private void Tabla_SelectionChanged(object sender, EventArgs e)
        {
            if (Tabla.SelectedRows.Count == 1)
            {
                txt_Codigo1.Text = Tabla.SelectedRows[0].Cells[0].Value.ToString();
                txt_Detalle1.Text = Tabla.SelectedRows[0].Cells[1].Value.ToString();
                txt_Presentacion1.Text = Tabla.SelectedRows[0].Cells[2].Value.ToString();
                txt_Compra1.Text = Tabla.SelectedRows[0].Cells[3].Value.ToString();
                txt_Venta1.Text = Tabla.SelectedRows[0].Cells[4].Value.ToString();
                txt_Stock1.Text = Tabla.SelectedRows[0].Cells[5].Value.ToString();
                txt_Proveedor.Text = Tabla.SelectedRows[0].Cells[6].Value.ToString();
            }
            else
            {
                txt_Codigo1.Text = string.Empty;
                txt_Detalle1.Text = string.Empty;
                txt_Presentacion1.Text = string.Empty;
                txt_Compra1.Text = string.Empty;
                txt_Venta1.Text = string.Empty;
                txt_Stock1.Text = string.Empty;
                txt_Proveedor.Text = string.Empty;
            }
        }
        private void T_Carrito_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if(T_Carrito.ColumnCount >= 8)
            {
                float MontoTotal = 0;
                foreach (DataGridViewRow fila in T_Carrito.Rows)
                {
                    MontoTotal += float.Parse(fila.Cells[9].Value.ToString());
                }
                txt_Monto.Text = MontoTotal.ToString();
            }
           
        }
    }
}
