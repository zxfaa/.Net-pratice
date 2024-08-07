﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace WebApplication1.Services
{
    public class MailService
    {
        private string gmail_account = "";//gmail帳號
        private string gmail_password = "";//gmail密碼
        private string gmail_mail = "";//gmail信箱

        #region 寄會員驗證信
        //產生驗證碼
        public string GetValidCode()
        {
            // 設定驗證碼字元的陣列
            string[] Code ={ "A", "B", "C", "D", "E", "F", "G", "H", "I",
                             "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "U",
                             "V", "W", "X", "Y", "Z", "1", "2", "3", "4", "5", "6",
                             "7", "8", "9", "a", "b", "c", "d", "e", "f", "g", "h",
                             "i", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t",
                             "u", "v", "w", "x", "y", "z" };
            //宣告初始為空的驗證碼自串
            string ValidateCode  = string.Empty;
            //宣告可產生隨機數值的物件
            Random rd = new Random(); 
            //使用迴圈產生驗證碼
            for(int i = 0; i < 10; i++)
            {
                ValidateCode += Code[rd.Next(Code.Count())];
                //Next方法將返回0~Code.Count()之間的隨機整數
            }
            //回傳驗證碼
            return ValidateCode;

        }
        
    
        //將使用者資料填入驗證信範本中
        public string GetRegisterMailBody(string TempString , string UserName ,string ValidateUrl )
        {
            //將使用者資料填入
            TempString = TempString.Replace("{{Username}}", UserName);
            TempString = TempString.Replace("{{ValidateUrl}}", ValidateUrl);
            //回傳結果
            return TempString;
        }

        //寄送驗證信的方法
        public void SendRegisterMail(string MailBody,string ToEmail)
        {
            //建立寄信用Smtp物件,以Gmail為例
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            //設定使用的Port
            SmtpServer.Port = 587;
            //建立使用者憑據
            SmtpServer.Credentials = new System.Net.NetworkCredential(gmail_account, gmail_password);
            //開啟ssl
            SmtpServer.EnableSsl = true;

            //宣告信件內容物件
            MailMessage mail = new MailMessage();
            //設定來源信箱
            mail.From = new MailAddress(gmail_mail);
            //設定收信者信箱
            mail.To.Add(ToEmail);
            //設定信件主旨
            mail.Subject = "會員註冊確認信";
            //設定信件內容
            mail.Body = MailBody;
            //設定信件內容為HTML格式
            mail.IsBodyHtml = true;
            //送出信件
            SmtpServer.Send(mail);
        }
        #endregion
    }
}