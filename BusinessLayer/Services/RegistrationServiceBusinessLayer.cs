using BusinessLayer.Interface;
using ModelLayer.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Interface;

namespace BusinessLayer.Services
{
    public class RegistrationServiceBusinessLayer : IRegistrationBusinessLayer
    {
        private readonly IRegistrationRepositoryLayer _registration;

        public RegistrationServiceBusinessLayer(IRegistrationRepositoryLayer registration)
        {
            _registration = registration;
        }
        public Task<bool> RegisterUser(UserRegistrationModel userRegistrationModel)
        {

            return _registration.RegisterUser(userRegistrationModel);
        }
    }
}
