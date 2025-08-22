using FarmaciaSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Utils.Security
{
    public static class SessionManager
    {
        private static User _currentUser;
        private static DateTime _sessionStart;
        private static readonly TimeSpan SessionTimeout = TimeSpan.FromHours(8);

        public static User CurrentUser
        {
            get => IsSessionValid() ? _currentUser : null;
            private set => _currentUser = value;
        }

        public static bool IsLoggedIn => CurrentUser != null;

        public static void StartSession(User user)
        {
            CurrentUser = user ?? throw new ArgumentNullException(nameof(user));
            _sessionStart = DateTime.Now;
        }

        public static void EndSession()
        {
            CurrentUser = null;
            _sessionStart = default;
        }

        public static bool IsSessionValid()
        {
            if (_currentUser == null) return false;
            return DateTime.Now.Subtract(_sessionStart) < SessionTimeout;
        }

        public static TimeSpan GetRemainingTime()
        {
            if (!IsSessionValid()) return TimeSpan.Zero;
            var elapsed = DateTime.Now.Subtract(_sessionStart);
            return SessionTimeout.Subtract(elapsed);
        }

        public static void ExtendSession()
        {
            if (IsSessionValid())
            {
                _sessionStart = DateTime.Now;
            }
        }

        public static string GetSessionInfo()
        {
            if (!IsLoggedIn) return "No hay sesión activa";

            var remaining = GetRemainingTime();
            return $"Usuario: {CurrentUser.FullName}, Tiempo restante: {remaining.Hours:D2}:{remaining.Minutes:D2}";
        }

        public static bool IsSessionExpiringSoon(int minutesThreshold = 15)
        {
            if (!IsSessionValid()) return false;

            var remaining = GetRemainingTime();
            return remaining.TotalMinutes <= minutesThreshold;
        }
    }
}
