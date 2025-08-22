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
        static async void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Configuraciones para hardware limitado
            System.GC.Collect();
            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.Batch;

            try
            {
                // Inicializar base de datos
                await DatabaseInitializer.InitializeAsync();

                // Mostrar formulario de login
                Application.Run(new LoginForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar la aplicación: {ex.Message}",
                               "Error Critical", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
