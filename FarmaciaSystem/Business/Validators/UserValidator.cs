using FarmaciaSystem.Data.Repositories;
using FarmaciaSystem.Models;
using FarmaciaSystem.Utils.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Business.Validators
{
    public class UserValidator
    {
        private readonly UserRepository _userRepository;

        public UserValidator(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ValidateAsync(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("El nombre de usuario es requerido");

            if (user.Username.Length < 3)
                throw new ArgumentException("El nombre de usuario debe tener al menos 3 caracteres");

            if (user.Username.Length > 50)
                throw new ArgumentException("El nombre de usuario no puede exceder 50 caracteres");

            if (string.IsNullOrWhiteSpace(user.FullName))
                throw new ArgumentException("El nombre completo es requerido");

            if (user.FullName.Length > 200)
                throw new ArgumentException("El nombre completo no puede exceder 200 caracteres");

            if (user.RoleId <= 0)
                throw new ArgumentException("Debe seleccionar un rol válido");

            // Validar email si se proporciona
            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                if (!IsValidEmail(user.Email))
                    throw new ArgumentException("El formato del email no es válido");

                if (user.Email.Length > 100)
                    throw new ArgumentException("El email no puede exceder 100 caracteres");
            }

            // Validar teléfono si se proporciona
            if (!string.IsNullOrWhiteSpace(user.Phone) && user.Phone.Length > 20)
                throw new ArgumentException("El teléfono no puede exceder 20 caracteres");

            // Validar nombre de usuario único
            var existingUser = await _userRepository.GetByUsernameAsync(user.Username);
            if (existingUser != null && existingUser.Id != user.Id)
                throw new ArgumentException("Ya existe un usuario con este nombre de usuario");
        }

        public void ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("La contraseña es requerida");

            if (password.Length < 6)
                throw new ArgumentException("La contraseña debe tener al menos 6 caracteres");

            if (password.Length > 100)
                throw new ArgumentException("La contraseña no puede exceder 100 caracteres");

            if (!PasswordHelper.IsPasswordStrong(password))
                throw new ArgumentException("La contraseña debe contener al menos una mayúscula, una minúscula y un número");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
