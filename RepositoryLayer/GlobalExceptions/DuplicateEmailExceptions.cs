using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.GlobalExceptions
{
    public class DuplicateEmailExceptions : Exception
    {
        public DuplicateEmailExceptions(string message) : base(message) { }
    }
}