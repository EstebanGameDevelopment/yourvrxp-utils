using UnityEngine;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Linq;
using System.IO;

namespace yourvrexperience.Utils
{
	public static class SHAEncryption
	{
        public static string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = Encoding.UTF8.GetBytes(password + salt);
                var hash = sha256.ComputeHash(saltedPassword);
                return Convert.ToBase64String(hash);
            }
        }

        public static string GenerateSalt()
        {
            
            byte[] saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public static string GenerateSaltWithTimestamp(string salt, long timestamp)
        {
            // Get the current timestamp
            string saltWithTimestamp = $"{salt}{timestamp}";

            // Hash the salt and timestamp together
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltWithTimestamp));
                return Convert.ToBase64String(hashBytes); // Opaque combined value
            }
        }

        public static string HashWithSalt(string input, string combinedSalt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] combinedBytes = Encoding.UTF8.GetBytes(input + combinedSalt);
                byte[] hashBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}