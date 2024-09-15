using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class GuestbooksController : Controller
    {
        // GET: Guestbooks
        //宣告Guestbook資料表的Service物件
        //此物件為宣告一個名為GuestbookService物件 類別是在Service的GuestbooksDBService物件
        private readonly GuestbooksDBService GuestbookService = new GuestbooksDBService();
        public ActionResult Index()
        {
            return View();
        }

        //取得資料陣列用Action，將Page(頁數)預設為1
        public ActionResult GetDataList(string Search, int Page = 1)
        {
            //宣告一個新頁面模型
            GuestbooksViewModel Data = new GuestbooksViewModel();
            //將傳入值Search(搜尋)放入頁面模型中
            Data.Search = Search;
            //新增頁面模型中的分頁
            Data.Paging = new ForPaging(Page);
            //從Service中取得頁面所需陣列資料
            Data.DataList = GuestbookService.GetDataList(Data.Paging , Data.Search);
            //將頁面資料傳入View中
            return PartialView(Data);
        }
        [HttpPost]
        //設定搜尋為接受頁面POST傳入
        //使用Bind的Include來定義只接受的欄位，用來避免傳入其他不相干值
        public ActionResult GetDataList([Bind(Include = "Search")]GuestbooksViewModel Data)
        {
            //重新導向頁面至開始頁面，並傳入搜尋值
            return RedirectToAction("GetDataLsit" , new {Search = Data.Search});  
        }

        #region 新增留言
        //新增留言一開始載入頁面
        public ActionResult Create()
        {
            //因此頁面用於載入至開始頁面中，故使用部分檢視回傳
            return PartialView();
        }

        //設定留言時傳入資料的Action
        [Authorize] // 設定此 Action 須登入
        [HttpPost]
        //設定此Action只接受頁面POST資料傳入
        //使用Bind的Include來定義只接收的欄位，用來避免傳入其他不相干值
        public ActionResult Create([Bind(Include="Name,Content")]Guestbooks Data) 
        {
            //設定新增留言者為登入者
            Data.Account = User.Identity.Name;
            //使用service來新增一筆資料
            GuestbookService.IntertGuestbooks(Data);
            //重新導向面至開始頁面
            return RedirectToAction("Index");
        }
        #endregion

        #region 修改留言
        //新增留言頁面要根據傳入編號來決定要修改的資料
        [Authorize]//設定此Action需登入
        public ActionResult Edit(int Id)
        {
            //取得Service的資料
            Guestbooks Data = GuestbookService.GetDataByID(Id);

            //將資料傳入view中
            return View(Data);
        }
        //修改留言傳入資料的Action
        [Authorize]//設釘此Action需登入
        [HttpPost]//設定此Action只接受頁面POST資料傳入
        //使用Bind的Include來定義只接受的欄位，用來避免傳入其他不相干值
        public ActionResult Edit(int Id, [Bind(Include = "Name,Content")]Guestbooks UpdateData)
        {
            //修改資料的是否可修改判斷
            if(GuestbookService.CheckUpdate(Id))
            {
                //將編號設定至修改資料中
                UpdateData.Id= Id;
                //設定修改留言的留言者為登入者
                UpdateData.Account= User.Identity.Name; 
                //使用Service來修改資料
                GuestbookService.UpdataGuestbooks(UpdateData);
                //重新導向頁面至開始頁面
                return RedirectToAction("Index");

            }
            else
            {
                //重新導向至開始頁面
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region 回覆留言
        //回覆留言頁面要根據傳入編號來決定回復的資料
        [Authorize(Roles ="Admin")] // 此Action只有Admin角色才可使用
        public ActionResult Reply(int Id)
        {
            //取得頁面所需資料，藉由Service取得
            Guestbooks Data = GuestbookService.GetDataByID(Id);
            //將資料傳入View中;
            return View(Data);
        }
        //修改留言傳入資料時的Action
        [HttpPost]//設定此Action只接受頁面POST的資料傳入
        [Authorize(Roles = "Admin")] // 設定此 Action 只有 Admin 角色才可使用
        //使用Bind的Include來定義只接受的欄位，用來避免傳入其他不相干值
        public ActionResult Reply(int Id, [Bind(Include = "Reply.ReplyTime")]Guestbooks ReplyData)
        {
            //修改資料的是否可修改判斷
            if (GuestbookService.CheckUpdate(Id))
            {
                //將編號設定至回覆留言中
                ReplyData.Id= Id;
                //使用Service來回覆留言
                GuestbookService.ReplyGuestbooks(ReplyData);
                //重新導向頁面至開始頁面
                return RedirectToAction("Index");
            }
            else
            {
                //重新導向頁面至開始頁面
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region 刪除留言
        //刪除頁面要根據傳入編號來刪除資料
        [Authorize(Roles = "Admin")] // 設定此 Action 只有 Admin 角色才可使用
        public ActionResult Delete(int Id)
        {
           GuestbookService.DeleteGuestbooks(Id);
            return RedirectToAction("Index");
        }
        #endregion
    }
}