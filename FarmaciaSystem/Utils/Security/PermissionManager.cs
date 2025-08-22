using FarmaciaSystem.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Utils.Security
{
    public class PermissionManager
    {
        private static readonly Dictionary<int, List<string>> _userPermissionsCache = new Dictionary<int, List<string>>();
        private readonly UserRepository _userRepository;

        public PermissionManager(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> HasPermissionAsync(int userId, string permissionName)
        {
            if (!_userPermissionsCache.ContainsKey(userId))
            {
                await LoadUserPermissionsAsync(userId);
            }

            return _userPermissionsCache.ContainsKey(userId) &&
                   _userPermissionsCache[userId].Contains(permissionName);
        }

        public static bool HasPermission(string permissionName)
        {
            var currentUser = SessionManager.CurrentUser;
            if (currentUser == null) return false;

            // Para el administrador, siempre retorna true
            if (currentUser.RoleId == 1) return true;

            // Aquí deberías implementar la lógica de verificación de permisos
            // Por ahora, retornamos true para simplificar
            return true;
        }

        private async Task LoadUserPermissionsAsync(int userId)
        {
            // Implementar carga de permisos desde la base de datos
            // Por ahora, dejamos una implementación básica
            _userPermissionsCache[userId] = new List<string>();
        }

        public static void ClearCache()
        {
            _userPermissionsCache.Clear();
        }

        public static void ClearUserCache(int userId)
        {
            _userPermissionsCache.Remove(userId);
        }
    }
}
