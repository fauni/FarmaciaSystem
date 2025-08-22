using FarmaciaSystem.Data.Repositories;
using FarmaciaSystem.Models;
using FarmaciaSystem.Utils.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Business.Services
{
    public class AuthenticationService
    {
        private readonly UserRepository _userRepository;

        public AuthenticationService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            return await _userRepository.AuthenticateAsync(username, password);
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            if (!PasswordHelper.VerifyPassword(currentPassword, user.PasswordHash))
                return false;

            return await _userRepository.ChangePasswordAsync(userId, newPassword);
        }

        public async Task LogoutAsync(int userId)
        {
            // Aquí puedes agregar lógica adicional para el logout
            // como invalidar sesiones, registrar en auditoría, etc.
            await Task.CompletedTask;
        }
    }
}
