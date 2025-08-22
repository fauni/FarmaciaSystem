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
            // Configurar la aplicación
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Configuraciones para hardware limitado
            System.GC.Collect();
            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.Batch;

            // Configurar manejo global de excepciones
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                // Mostrar splash screen mientras se inicializa
                ShowSplashScreen();

                // Inicializar base de datos de forma síncrona para evitar problemas
                InitializeDatabase();

                // Mostrar formulario de login
                Application.Run(new LoginForm());
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error crítico al inicializar la aplicación: {ex.Message}");
            }
        }

        private static void ShowSplashScreen()
        {
            // TODO: Implementar splash screen si es necesario
            // Por ahora, solo agregamos un pequeño delay para simular carga
            System.Threading.Thread.Sleep(500);
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
                ShowErrorMessage($"Error al inicializar la base de datos: {ex.Message}");
                throw;
            }
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception, "Error en la aplicación");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception, "Error no controlado");
        }

        private static void HandleException(Exception ex, string title)
        {
            var message = ex != null ? ex.Message : "Error desconocido";
            ShowErrorMessage($"{title}: {message}");

            // Log del error (aquí podrías implementar logging más sofisticado)
            System.Diagnostics.Debug.WriteLine($"[ERROR] {DateTime.Now}: {ex}");
        }

        private static void ShowErrorMessage(string message)
        {
            MessageBox.Show(
                message,
                "Sistema Farmacia - Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
