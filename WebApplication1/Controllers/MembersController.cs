using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using WebApplication1.Security;
using WebApplication1.Services;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class MembersController : Controller
    {
        // GET: Member
        
        //宣告Members資料表的Service物件
        private readonly MembersDBService membersService = new MembersDBService();
        //宣告寄信用的Service物件
        private readonly MailService mailService = new MailService();

        public ActionResult Index()
        {
            return View();
        }

        #region 註冊
        //註冊一開始顯示頁面
        public ActionResult Register() 
        {
            //判斷使用者是否已經過登入驗證
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index" , "Guestbooks");
            }
            //已登入則重新導向
            //否則進入註冊畫面
            return View();
        }
        //傳入註冊資料的Action
        [HttpPost]
        //設定此Action只接受頁面POST資料傳入
        public ActionResult Register(MembersRegisterViewModel RegisterMember)
        {
            //判斷頁面資料是否都經過驗證
            //ModelState.IsValid驗證是否都符合規範
            if (ModelState.IsValid)
            {
                //頁面資料中的密碼欄位填入
                RegisterMember.newMember.Password = RegisterMember.Password;
                //取得信箱驗證碼
                string AuthCode = mailService.GetValidCode();
                //將信箱驗證碼填入
                RegisterMember.newMember.AuthCode= AuthCode;
                //呼叫Service註冊新會員
                membersService.Register(RegisterMember.newMember);
                //取得寫好的驗證信範本
                string TempMail = System.IO.File.ReadAllText(Server.MapPath("~/Views/Shared/RegisterEmailTemplate.html"));
                
                //宣告Email驗證用的Url
                
                UriBuilder ValidateUrl = new UriBuilder(Request.Url)
                //對象初始器，不會去google
                {
                    Path = Url.Action("EmailValidate", "Members"
                        , new
                        {
                            Account = RegisterMember.newMember.Account,
                            AuthCode = AuthCode

                        })
                };

                //藉由Service將使用者資料填入驗證信範本中
                string MailBody = mailService.GetRegisterMailBody(TempMail, RegisterMember.newMember.Name, ValidateUrl.ToString().Replace("%3F","?"));

                //呼叫Service寄出驗證信
                mailService.SendRegisterMail(MailBody, RegisterMember.newMember.Email);

                //用TempData儲存註冊訊息
                TempData["RegisterState"] = "註冊成功，請去收信以驗證Email";
                //重新導向頁面
                return RedirectToAction("RegisterResult");
            }
            //未驗證清空密碼相關欄位
            RegisterMember.Password = null;
            RegisterMember.PasswordCheck = null;
            //將資料回填至View中
            //若odelState.IsValid為false，則會回到註冊頁面，並且保留除了密碼以外的所有資料
            return View(RegisterMember);

        }
       
        //註冊結果顯示頁面
        public ActionResult RegisterResult()
        {
            return View();
        }

        //判斷註冊帳號是否已被註冊過Action
        public JsonResult AccountCheck(MembersRegisterViewModel RegisterMember)
        {
            //呼叫Service來判斷，並回傳結果
            return Json(membersService.AccountCheck(RegisterMember.newMember.Account), JsonRequestBehavior.AllowGet);
        }

        //接收驗證信連結傳來的Action
        public ActionResult EmailValidate(string Account , string AuthCode)
        {

            // 用 ViewData 儲存，使用 Service 進行信箱驗證後的結果訊息
            ViewData["EmailValidate"] = membersService.EmailValidate(Account,AuthCode);
            return View();
        }
        #endregion

        #region 登入
        //登入一般畫面
        public ActionResult Login()
        {
            //判斷使用者是否已經過登入驗證
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Guestbooks");
            }
            return View();
        }
        //傳入登入的資料
        [HttpPost] // 設定此 Action 只接受頁面 POST 資料傳入
        public ActionResult Login(MembersLoginViewModel LoginMember)
        {
           // 使用 Service 裡的方法來驗證登入的帳號密碼
           string ValidateStr = membersService.LoginCheck(LoginMember.Account,
           LoginMember.Password);
            // 判斷驗證後結果是否有錯誤訊息
            if (String.IsNullOrEmpty(ValidateStr))
            {
                // 無錯誤訊息，則登入
                //先藉由Service取得登入者角色資料
                string RoleData = membersService.GetRole(LoginMember.Account);
                //設定Jwt
                JwtService jwtService = new JwtService();

                //從Web.Config撈出資料
                //Cookie名稱
                string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();
                string Token = jwtService.GenerateToekn(LoginMember.Account, RoleData);
                
                //產生Cookie
                HttpCookie cookie = new HttpCookie(cookieName);
                //設定單值
                cookie.Value = Server.UrlEncode(Token);
                //寫到用戶端
                Response.Cookies.Add(cookie);
                //設定Cookie期限
                Response.Cookies[cookieName].Expires = DateTime.Now.AddMinutes(Convert.ToInt32(WebConfigurationManager.AppSettings["ExpireMinutes"]));

                // 重新導向頁面
                return RedirectToAction("Index", "Guestbooks");
            }
            else
            {
                // 有驗證錯誤訊息，加入頁面模型中
                ModelState.AddModelError("", ValidateStr);
                // 將資料回填至 View 中
                return View(LoginMember);
            }
        }

        #endregion

        #region 登出
        //登出Action
        [Authorize] // 設定此Action需登入
        public ActionResult Logout() 
        {
            //使用者登出
            //Cookie名稱
            string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();
            //清除Cookie
            HttpCookie cookie = new HttpCookie(cookieName);
            cookie.Expires = DateTime.Now.AddDays(-1);
            cookie.Values.Clear();
            Response.Cookies.Set(cookie);
            //重新導向至登入Action
            return RedirectToAction("Login");

        }
        #endregion
    }
}