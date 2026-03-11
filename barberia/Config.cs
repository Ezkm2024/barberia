using System;
using System.Windows.Forms;
using System.Configuration;

namespace barberia
{
    // Helper to obtain the DB connection string from environment or App.config.
    // Do NOT store secrets directly in source code. Set environment variable BARBERIA_CONNECTION
    // or add an AppSetting key "BarberiaConnection" in App.config (not committed with secrets).
    public static class Config
    {
        public static string GetConnectionString()
        {
            try
            {
                var env = Environment.GetEnvironmentVariable("BARBERIA_CONNECTION");
                if (!string.IsNullOrWhiteSpace(env))
                    return env;

                var cfg = ConfigurationManager.AppSettings["BarberiaConnection"];
                if (!string.IsNullOrWhiteSpace(cfg))
                    return cfg;

                MessageBox.Show("Cadena de conexión no configurada. Defina la variable de entorno BARBERIA_CONNECTION o agregue BarberiaConnection en App.config.", "Configuración faltante", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al leer configuración: " + ex.Message);
                return string.Empty;
            }
        }
    }
}
