using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Documentation.Web.Helper
{
    public class SecurityHelper
    {
        public static string EncryptPassword(string password)
        {
            //we use codepage 1252 because that is what sql server uses
            byte[] pwdBytes = Encoding.GetEncoding(1252).GetBytes(password);
            byte[] hashBytes = System.Security.Cryptography.MD5.Create().ComputeHash(pwdBytes);
            return Encoding.GetEncoding(1252).GetString(hashBytes);
        }
    }
}