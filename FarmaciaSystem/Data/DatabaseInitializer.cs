using System;
using System.Data.SQLite;
using System.Threading.Tasks;
using FarmaciaSystem.Data.Context;
using FarmaciaSystem.Data.Scripts;

namespace FarmaciaSystem.Data
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync()
        {
            try
            {
                using (var context = new DatabaseContext())
                {
                    await context.OpenAsync();

                    using (var command = new SQLiteCommand(CreateTablesScript.SQL, context.GetConnection()))
                    {
                        await command.ExecuteNonQueryAsync();
                    }

                    await SeedInitialDataAsync(context);

                    Console.WriteLine("Base de datos inicializada correctamente.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al inicializar la base de datos: {ex.Message}");
                throw;
            }
        }

        private static async Task SeedInitialDataAsync(DatabaseContext context)
        {
            // Verificar si ya hay datos
            using (var checkCommand = new SQLiteCommand("SELECT COUNT(*) FROM Roles", context.GetConnection()))
            {
                var count = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());

                if (count > 0) return; // Ya hay datos iniciales

                // Insertar datos iniciales
                var seedScript = InitialDataScript.SQL;
                using (var command = new SQLiteCommand(seedScript, context.GetConnection()))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
