using Jose;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace WebApplication1.Security
{
    public class JwtService
    {
        #region 製作Token
        public string GenerateToekn(string Account, string Role)
        {
            JwtObject jwtObject = new JwtObject()
            {
                Account = Account,
                Role = Role,
                Expire = DateTime.Now.AddMinutes(Convert.ToInt32(WebConfigurationManager.AppSettings["ExpireMinutes"])).ToString()
            };
            //從Web.Config取得密鑰
            string SercretKey = WebConfigurationManager.AppSettings["SercretKey"].ToString();
            //JWT內容
            //提高代碼可讀性，payload與 jwtObject本質上是相同的
            var payload = jwtObject;
            //將資料加密為Token
            var token = JWT.Encode(payload, Encoding.UTF8.GetBytes(SercretKey), JwsAlgorithm.HS512);
            return token;
        }
        #endregion
    }
}