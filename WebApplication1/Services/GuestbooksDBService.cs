using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI.WebControls.Expressions;
using System.Web.WebPages;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class GuestbooksDBService
    {
        // 建立與資料庫的連線字串
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP.NET MVC"].ConnectionString;
        // 建立與資料庫的連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);


        #region 查詢陣列資料
        //根據分頁及搜尋來取得資料陣列的方法
        public List<Guestbooks> GetDataList(ForPaging Paging, string Search)
        {
            //宣告要接受全部搜尋資料的物件
            List<Guestbooks> DataList = new List<Guestbooks>();
            //Sql語法
            if (!string.IsNullOrWhiteSpace(Search))
            {
                //有搜尋條件時
                SetMaxPaging(Paging, Search);
                DataList = GetAllDataList(Paging, Search);
            }
            else
            {
                //無搜尋條件
                SetMaxPaging(Paging);
                DataList = GetAllDataList(Paging);
            }
            return DataList;
        }
        
        
        //無搜尋值的搜尋資料方法
        //無搜尋值的設定最大頁數方法
        public void SetMaxPaging(ForPaging Paging)
        {
            //計算列數
            int Row = 0;
            //Sql語法
            String sql = $@"SELECT * FROM Guestbooks; ";
            //確保程式不會因為執行錯誤而中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行SQL指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得SQL資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) // 獲得下一筆直到沒有資料
                {
                    Row++;
                }
            }
            catch(Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料連線
                conn.Close();    
            }
            //計算所需的總頁數
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row)/Paging.ItemNum));
            //重新設定正確的頁數，避免有不正確值傳入
            Paging.SetRightPage();
        }

        //有搜尋值的最大頁數方法
        public void SetMaxPaging(ForPaging Paging,string Search)
        {
            //計算列數
            int Row = 0;
            //Sql語法
            String sql = $@"SELECT * FROM Guestbooks
                         WHERE Name LIKE '%{Search}%'
                         OR Content LIKE '%{Search}%'
                         OR Reply LIKE '%{Search}%';";
            //確保程式不會因為執行錯誤而中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行SQL指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得SQL資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) // 獲得下一筆直到沒有資料
                {
                    Row++;
                }
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料連線
                conn.Close();
            }
            //計算所需的總頁數
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.ItemNum));
            //重新設定正確的頁數，避免有不正確值傳入
            Paging.SetRightPage();
        }
        #endregion


        #region 搜尋資料方法
        //無搜尋值的搜尋資料方法
        public List<Guestbooks> GetAllDataList(ForPaging paging)
        {
            //宣告要回傳的搜尋資料為資料庫中的Guestbooks資料表
            List<Guestbooks> DataList = new List<Guestbooks>();
            //Sql語法
            string sql = $@"SELECT * FROM (SELECT row_number() OVER(order by Id) AS sort, * FROM GUESTBOOKS)
                            m WHERE m.sort BETWEEN {(paging.NowPage - 1) * paging.ItemNum + 1} AND {paging.NowPage * paging.ItemNum};";
            //確保程式不會因為執行錯誤而中斷
            try
            {
                //開啟資料連線
                conn.Open();
                //執行SQL指令
                SqlCommand cmd = new SqlCommand(sql,conn);
                //取得SQL資料
                SqlDataReader dr = cmd.ExecuteReader();
                while(dr.Read()) //獲得下一筆資料直到沒有資料
                {
                        Guestbooks Data = new Guestbooks();
                        Data.Id = Convert.ToInt32(dr["Id"]);
                        Data.Name = dr["Name"].ToString();
                        Data.Content = dr["Content"].ToString();
                        Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                        // 確定此則留言是否回覆，且不允許空白
                        // 因 C# 是強型別語言，所以轉換時 Datetime 型態不允許存取 null
                        if (!dr["ReplyTime"].Equals(DBNull.Value))
                        {
                            Data.Reply = dr["Reply"].ToString();
                            Data.ReplyTime = Convert.ToDateTime(dr["ReplyTime"]);
                        }
                        DataList.Add(Data);
                    
                }


            }
            catch(Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //回傳搜尋資料
            return DataList;
        }

        //有搜尋值的搜尋資料方法
        public List<Guestbooks> GetAllDataList(ForPaging paging , string Search)
        {
            //宣告要回傳的搜尋資料為資料庫中的Guestbooks資料表
            List<Guestbooks> DataList = new List<Guestbooks>();
            //Sql語法
            string sql = $@" SELECT * FROM (SELECT row_number() OVER(order by Id) AS sort, 
                            * FROM Guestbooks WHERE Name LIKE '%{Search}%' 
                            OR Content LIKE '%{Search}%'
                            OR Reply LIKE '%{Search}%')
                            m WHERE m.sort BETWEEN {(paging.NowPage - 1) * paging.ItemNum + 1} AND {paging.NowPage * paging.ItemNum};";
            //確保程式不會因為執行錯誤而中斷
            try
            {
                //開啟資料連線
                conn.Open();
                //執行SQL指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得SQL資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) //獲得下一筆資料直到沒有資料
                {
                        Guestbooks Data = new Guestbooks();
                        Data.Id = Convert.ToInt32(dr["Id"]);
                        Data.Name = dr["Name"].ToString();
                        Data.Content = dr["Content"].ToString();
                        Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                        // 確定此則留言是否回覆，且不允許空白
                        // 因 C# 是強型別語言，所以轉換時 Datetime 型態不允許存取 null
                        if (!dr["ReplyTime"].Equals(DBNull.Value))
                        {
                            Data.Reply = dr["Reply"].ToString();
                            Data.ReplyTime = Convert.ToDateTime(dr["ReplyTime"]);
                        }
                        DataList.Add(Data);
                }
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //回傳搜尋資料
            return DataList;
        }
        #endregion



        #region 新增留言
        public void IntertGuestbooks(Guestbooks newData)
        {
            //Sql 新增語法
            //設定新增時間為現在

            string sql = $@"INSERT INTO Guestbooks(Name,Content,CreateTime) 
            VALUES ('{newData.Name}','{newData.Content}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}');";

            //確保程式不會因為執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
        }
        #endregion

        #region 查詢一筆資料
        //藉由編號取得單筆資料的方法
        public Guestbooks GetDataByID(int id)
        {
            Guestbooks Data = new Guestbooks();
            //sql語法
            string sql = $@"SELECT * FROM Guestbooks WHERE Id={id};";
            //確保程式不會因為執行錯誤而中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();

                //讀取獲得的資料
                dr.Read();

                //將資料填充至Data的欄位
                Data.Id = Convert.ToInt32(dr["ID"]);
                Data.Name = dr["Name"].ToString();
                Data.Content = dr["Content"].ToString();
                Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                //確定此則留言是否回復，且不允許空白
                if (!string.IsNullOrWhiteSpace(dr["Reply"].ToString()))
                {
                    Data.Reply = dr["Reply"].ToString();
                    Data.ReplyTime = Convert.ToDateTime(dr["ReplyTime"]);
                }

            } catch (Exception e)
            {
                //查無資料
                Data = null;
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            //回傳根據編號獲得的資料
            return Data;
        }
        #endregion

        #region 修改留言
        public void UpdataGuestbooks(Guestbooks UpdateData)
        {
            //sql語法
            string sql = $@"UPDATE Guestbooks SET NAME ='{UpdateData.Name}',Content='{UpdateData.Content}' WHERE Id = {UpdateData.Id}  ;";

            //確保程式不會因為執行錯誤而中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行SQL指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
        }
        #endregion

        #region 回覆留言
        public void ReplyGuestbooks(Guestbooks ReplyData)
        {
            //SQL語法
            //設定回復時間為現在
            string sql = $@" UPDATE Guestbooks SET Reply = '{ReplyData.Reply}'
                        ,ReplyTime ='{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}' 
                        WHERE Id={ReplyData.Id};";
            //確保程式不會因為執行錯誤而中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
        }
        #endregion

        #region 檢查相關
        public bool CheckUpdate(int Id)
        {
            //根據Id取得要修改的資料
            Guestbooks Data = GetDataByID(Id);
            //判斷並回傳(判斷是否有資料以及是否有回復)
            return (Data != null && Data.ReplyTime == null);
        }
        #endregion

        #region 刪除留言
        public void DeleteGuestbooks(int Id)
        {
            //sql語法
            //根據Id刪除留言
            string sql = $@"DELETE FROM Guestbooks WHERE Id = {Id}";
            //確保程式不會因為執行錯誤而中斷
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e) {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

       
        
        }


}