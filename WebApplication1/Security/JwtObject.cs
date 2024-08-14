using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Security
{
    //JWT內容設計
    public class JwtObject
    {

        public string Account { get; set; }
        public string Role { get; set; }

        //到期時間
        public string Expire { get; set; }
    }
}