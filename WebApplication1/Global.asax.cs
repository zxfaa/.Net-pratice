using Jose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebApplication1.Security;

namespace WebApplication1
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

        }

        //撰寫權限驗證前執行的動作
        //在此用於設定角色(Role)
        //使用object sender 與 EventArgs e 為標準設計模式所需
        // Application_OnPostAuthenticateRequset為使在身分驗證之後、授權之前的事件方法

        protected void Application_OnPostAuthenticateRequest(object sender, EventArgs e)
        {
            //接收資料
            HttpRequest httpRequest = HttpContext.Current.Request;
            //設定JWT密鑰
            string SecretKey = WebConfigurationManager.AppSettings["SecretKey"].ToString();
            //設定Cookie名稱
            string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();

            //檢查Cookie內是否存放Token
            if (httpRequest.Cookies[cookieName] != null)
            {
                //將TOKEN還原
                //JWT.Decode<之後要轉換的型態>
                //三個參數，分別是需要解碼的JWT字符串、key的字節數、簽名算法
                JwtObject jwtObject = JWT.Decode<JwtObject>(
                    Convert.ToString(httpRequest.Cookies[cookieName].Value),
                    Encoding.UTF8.GetBytes(SecretKey),
                    JwsAlgorithm.HS512);
                //將使用者資料取出，並分割成陣列
                string[] roles = jwtObject.Role.Split(new char[] { ',' });
                //自行建立Identity取代HttpContext.Current.User中的Identity
                //將資料塞進Claim內做設計


                //Claims 是個體資訊的集合，用來描述使用者的屬性和角色。
                Claim[] claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name, jwtObject.Account),
                    new Claim(ClaimTypes.NameIdentifier, jwtObject.Account)
                };

                //ClaimsIdentity 是一個包含多個 Claims 的對象，代表了一個使用者的完整身分。
                //第一個參數是Claim的名稱，第二個則是驗證方法
                var claimIdentity = new ClaimsIdentity(claims, cookieName);
                //加入Identityprovider這個Claim使得反仿冒語彙 @Html.AntiForgeryToken()能通過
                claimIdentity.AddClaim(
                new Claim(@"http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "My Identity", @"http://www.w3.org/2001/XMLSchema#string"));
                //指派腳色到目前這個HttpContext的User物件去
                HttpContext.Current.User = new GenericPrincipal(claimIdentity, roles);
                Thread.CurrentPrincipal = HttpContext.Current.User;

            }
        }
    }
}
