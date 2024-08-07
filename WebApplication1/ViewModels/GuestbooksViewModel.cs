using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication1.Models;
using System.ComponentModel;
using WebApplication1.Services;


namespace WebApplication1.ViewModels
{
    public class GuestbooksViewModel
    {

        //搜尋欄位
        [DisplayName("搜尋：")]
        public string Search { get; set; }
        // 顯示資料陣列
        public List<Guestbooks> DataList { get; set; }

        //分頁內容
        public ForPaging Paging { get; set; }

    }
}