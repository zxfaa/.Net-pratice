using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


//此頁面負責與資料庫溝通

namespace WebApplication1.Models
{
    public class Guestbooks
    {
        // 編號
        [DisplayName("編號")]
        public int Id { get; set; }
       

        //帳號
        public string Account {  get; set; }
        // 名字
        [DisplayName("名字")]
        [Required(ErrorMessage ="請輸入名字")]
        [StringLength(20,ErrorMessage ="名字不可超過20個字元")]
        public string Name { get; set; }
        
        // 留言內容
        [DisplayName("留言內容")]
        [Required(ErrorMessage ="請輸入留言內容")]
        [StringLength(100,ErrorMessage ="留言內容不可超過100字元")]
        public string Content { get; set; }
        
        // 新增時間
        [DisplayName("新增時間")]
        public DateTime CreateTime { get; set; }
        
        // 回覆內容
        [DisplayName("回覆內容")]
        [StringLength(100,ErrorMessage ="回覆內容不可超過100字")]
        public string Reply { get; set; }

        // 回覆時間
        //DateTime? 資料型態，允許 DateTime 有 NULL 產生
        [DisplayName("回覆時間")]
        public DateTime? ReplyTime { get; set; }

    }
}