using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WebApplication1.Services
{
    public class ForPaging
    {
        //當前頁數
        public int NowPage { get; set; }

        //最大頁數
        public int MaxPage {  get; set; }

        //分業項目數量，在此設為唯讀
        //以後要修改個數，只需在這裡而不需修改其他地方
        public int ItemNum
        {
            get
            {
                return 5;
            }
        }
        //此類別建構式
        public ForPaging()
        {
            //預設當前頁數1
            this.NowPage = 1;
        }
        //此類別建構式，含傳入頁數
        public ForPaging(int Page)
        {
            //設定當前頁數
            this.NowPage = Page;
        }

        //設定正確頁數的方法，以避免傳入不正確值
        public void SetRightPage()
        {
            //判斷當前頁數是否小於1
            if(this.NowPage < 1) {
                //將頁數設回1
                this.NowPage = 1;
            }
            //判斷當前頁數是否大於總頁數
            else if(this.NowPage > this.MaxPage)
            {
                //設定當前頁數為總頁數
                this.NowPage = this.MaxPage;
            }
            //將無資料時的當前頁數，重新設回1
            if(this.MaxPage.Equals(0)) 
            {
                this.NowPage = 1;
            }
        }
    }
}