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
    }
}