using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.GlobalExceptions
{
    public class InvalidPasswordException : Exception
    {
        public InvalidPasswordException(string message) : base(message) { }
    }
}