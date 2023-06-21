using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Logic
{
    public class Encrypt
    {

        private IConfiguration config;

        public Encrypt(IConfiguration config)
        {
            this.config = config;
        }

        public static string HASH_SHA1(string password)
        {
            SHA256 sHA256 = SHA256.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb= new StringBuilder();
            stream=sHA256.ComputeHash(encoding.GetBytes(password));
            for(int i = 0; i < stream.Length; i++)
            {
                sb.AppendFormat("{0:x2}", stream[i]);
                
            }
            
            return sb.ToString();
        }
    
    }
}
