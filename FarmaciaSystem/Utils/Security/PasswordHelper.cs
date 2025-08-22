using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Utils.Security
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("La contraseña no puede estar vacía");

            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var sb = new StringBuilder(hashedBytes.Length * 2);
                foreach (var b in hashedBytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
                return false;

            var hashOfInput = HashPassword(password);
            return string.Equals(hashOfInput, hashedPassword, StringComparison.OrdinalIgnoreCase);
        }

        public static string GenerateRandomPassword(int length = 8)
        {
            if (length < 4) length = 4;
            if (length > 50) length = 50;

            const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string special = "!@#$%&*";

            var random = new Random();
            var password = new StringBuilder();

            // Asegurar al menos un carácter de cada tipo
            password.Append(upperCase[random.Next(upperCase.Length)]);
            password.Append(lowerCase[random.Next(lowerCase.Length)]);
            password.Append(digits[random.Next(digits.Length)]);

            // Completar con caracteres aleatorios
            const string allChars = upperCase + lowerCase + digits + special;
            for (int i = 3; i < length; i++)
            {
                password.Append(allChars[random.Next(allChars.Length)]);
            }

            // Mezclar los caracteres
            return ShuffleString(password.ToString());
        }

        public static bool IsPasswordStrong(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 6)
                return false;

            bool hasUpper = false, hasLower = false, hasDigit = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                if (char.IsLower(c)) hasLower = true;
                if (char.IsDigit(c)) hasDigit = true;
            }

            return hasUpper && hasLower && hasDigit;
        }

        public static PasswordStrength GetPasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password))
                return PasswordStrength.VeryWeak;

            int score = 0;

            // Longitud
            if (password.Length >= 8) score++;
            if (password.Length >= 12) score++;

            // Complejidad
            if (password.Any(char.IsUpper)) score++;
            if (password.Any(char.IsLower)) score++;
            if (password.Any(char.IsDigit)) score++;
            if (password.Any(c => "!@#$%^&*()_+-=[]{}|;:,.<>?".Contains(c))) score++;

            switch (score)
            {
                case 0:
                case 1:
                    return PasswordStrength.VeryWeak;
                case 2:
                case 3:
                    return PasswordStrength.Weak;
                case 4:
                    return PasswordStrength.Medium;
                case 5:
                    return PasswordStrength.Strong;
                default:
                    return PasswordStrength.VeryStrong;
            }
        }

        private static string ShuffleString(string input)
        {
            var array = input.ToCharArray();
            var random = new Random();

            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }

            return new string(array);
        }
    }

    public enum PasswordStrength
    {
        VeryWeak,
        Weak,
        Medium,
        Strong,
        VeryStrong
    }
}
