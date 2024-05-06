using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RepositoryLayer.RegularExpressions
{
    public class RegexValidation
    {
        private static string _emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,100}$";
        private static string _firstNamePattern = @"^[a-zA-ZÀ-ÿ'\s-]{3,100}$";
        private static string _lastNamePattern = @"^[a-zA-ZÀ-ÿ'\s-]{1,100}$";
        private static string _passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,100}$";
        public bool IsValidEmail(string? email)
        {
            if (email == null) { return false; }
            return Regex.IsMatch(email, _emailPattern);
        }
        public bool IsValidFirstName(string? name)
        {
            if (name == null) { return false; }
            return Regex.IsMatch(name, _firstNamePattern);
        }
        public bool IsValidLastName(string? name)
        {
            if (name == null) { return false; }
            return Regex.IsMatch(name, _lastNamePattern);
        }
        public bool IsValidPassword(string? password)
        {
            if (password == null) { return false; }
            return Regex.IsMatch(password,_passwordPattern);
        }
    }
}
