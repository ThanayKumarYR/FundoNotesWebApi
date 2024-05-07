using ModelLayer.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
    public interface IRegistrationBusinessLayer
    {
        public Task<bool> RegisterUser(UserRegistrationModel userRegistrationModel);
        public Task<string> UserLogin(UserLoginModel userLogin);
    }
}
