using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Supermercado_Chino
{
    public partial class Login : Form
    {
        public string ConnectionString { get; set; } = @"Server=localhost\SQLEXPRESS;" +
    "Database=ChinoDB; Trusted_Connection=True;TrustServerCertificate=True";
        public Login()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txt_Usuario.Text == string.Empty && txt_Contraseña.Text == string.Empty)
            {
                
            }
            else
            {
                List<Usuario> Usuarios = new List<Usuario>();
                using SqlConnection connection = new SqlConnection(ConnectionString);
                connection.Open();
                SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Usuario", connection);
                sqlCommand.Parameters.AddWithValue("@Nombre", txt_Usuario.Text);
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    Usuarios.Add(new Usuario(sqlDataReader.GetValue(5).ToString(), sqlDataReader.GetValue(1).ToString()));
                }
                var Identificado = Usuarios.FirstOrDefault(Usa=>Usa.Contraseña == txt_Contraseña.Text && Usa.Nombre == txt_Usuario.Text);
                if (Identificado != null)
                {
                    FRM_Menu Menu = new FRM_Menu();
                    Menu.Show();
                }
                else
                {
                    MessageBox.Show("Error");
                }
                sqlCommand.Dispose();
                sqlDataReader.Dispose();
            }
        }
    }
}
