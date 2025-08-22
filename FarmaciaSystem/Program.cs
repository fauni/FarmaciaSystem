using FarmaciaSystem.Data;
using FarmaciaSystem.Forms.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FarmaciaSystem
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
            // Configuraciones para hardware limitado
            System.GC.Collect();
            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.Batch;

            try
            {
                // Inicializar base de datos de forma síncrona para evitar problemas
                InitializeDatabase();

                // Mostrar formulario de login
                Application.Run(new LoginForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar la aplicación: {ex.Message}",
                               "Error Critical", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void InitializeDatabase()
        {
            try
            {
                // Ejecutar inicialización de la base de datos de forma síncrona
                Task.Run(async () => await DatabaseInitializer.InitializeAsync()).Wait();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar la base de datos: {ex.Message}",
                               "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }
    }
}
