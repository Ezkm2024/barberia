using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace barberia
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Ensure connection string exists before starting UI
            var conn = Config.GetConnectionString();
            if (string.IsNullOrWhiteSpace(conn))
            {
                MessageBox.Show("No hay cadena de conexión configurada. Defina la variable de entorno BARBERIA_CONNECTION o la clave BarberiaConnection en App.config.", "Configuración faltante", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Application.Run(new Form1());
        }
    }
}
