using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Data.Context
{
    public static class DatabaseConfig
    {
        public static string DatabasePath => System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "farmacia.db");

        public static string ConnectionString =>
            $"Data Source={DatabasePath};" +
            "Cache Size=10000;" +        // Cache de 10MB para mejor rendimiento
            "Page Size=4096;" +          // Tamaño de página optimizado
            "Temp Store=Memory;" +       // Archivos temporales en memoria
            "Journal Mode=WAL;" +        // Write-Ahead Logging para mejor concurrencia
            "Synchronous=Normal;" +      // Balance entre rendimiento y seguridad
            "Foreign Keys=True;";        // Habilitar claves foráneas

        public static void EnsureDatabaseDirectory()
        {
            var directory = System.IO.Path.GetDirectoryName(DatabasePath);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
        }
    }
}
