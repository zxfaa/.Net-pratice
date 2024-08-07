using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication1.Models;
using System.ComponentModel;

namespace WebApplication1.ViewModels
{
    public class MembersRegisterViewModel
    {
        public Members newMember {  get; set; }

        [DisplayName("密碼")]
        [Required(ErrorMessage ="請輸入密碼")]
        public string Password {  get; set; }

        [DisplayName("確認密碼")]
        //比較PasswordCheck與Password的值是否相同
        [Compare("Password",ErrorMessage ="兩次密碼輸入不一致")]
        [Required(ErrorMessage ="請輸入確認密碼")]
        public string PasswordCheck { get; set; }

    }
}