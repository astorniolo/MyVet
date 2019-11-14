using System;
using System.Collections.Generic;
using System.Text;

namespace MyVet.Common.Models
{
    public class TokenResponse
    {
        public string Token { get; set; }    // devuelve el atributo token ....ver postman

        public DateTime Expiration { get; set; }   // 

        public DateTime ExpirationLocal => Expiration.ToLocalTime();    // propiedad de lectura....


    }
}
