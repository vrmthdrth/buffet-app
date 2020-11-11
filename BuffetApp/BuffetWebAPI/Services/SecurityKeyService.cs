using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuffetWebAPI.Services
{
    public class SecurityKeyService
    {
        private string _securityKey;

        public SecurityKeyService(string securityKey)
        {
            this._securityKey = securityKey;
        }

        public string GetSecurityKey()
        {
            return this._securityKey;
        }
    }
}
