using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using iPE.Models;
using Xfrog.Net;
using Newtonsoft.Json;
using System.Text;
using System.IO;

namespace iPE.Controllers
{
    public class HomePageController : Controller
    {
        private Users dbUse = new Users();
        private Matches dbMat = new Matches();
        private Buys dbBuy = new Buys();
        private Tickets dbTic = new Tickets();
        private Joins dbJoi = new Joins();
        private Collections dbCol = new Collections();
        private HomePageViewModel viewModel = new HomePageViewModel();

        // GET: HomePage
        public ActionResult Homepage()
        {
            UserLoginModel user = (Session["UserMessage"] as UserLoginModel);
            if(user == null)
            {
                return Content("<script language='javascript'>alert('Please Login First!');top.location='/LoginAndRegister/Index';</script>");
            }
            int curUserId = user.id;
            ViewData["uid"] = curUserId;

            viewModel.myself = (from a in dbUse.TB_User where (a.u_id == curUserId) select a).ToList().FirstOrDefault();
            
            viewModel.releasedMatch = (from a in dbMat.TB_Match where (a.u_id == curUserId) select a).ToList().FirstOrDefault();

            viewModel.boughtTickets = new List<BuyDetail>();
            List<TB_Buy> bought = (from a in dbBuy.TB_Buy where (a.u_id == curUserId) select a).ToList();
            foreach (TB_Buy item in bought)
            {
                BuyDetail detail = new BuyDetail();
                TB_Ticket ticket = (from a in dbTic.TB_Tickets where (a.t_id == item.t_id) select a).ToList().FirstOrDefault();
                detail.t_id = ticket.t_id;
                detail.name = ticket.name;
                detail.number = item.number;
                detail.price = Convert.ToInt32(item.price);
                detail.time = item.time.ToLongDateString();
                detail.ticket = ticket.description;
                viewModel.boughtTickets.Add(detail);
            }

            viewModel.enrollMatches = new List<MatchViewModels>();
            List<TB_Join> joined = (from a in dbJoi.TB_Join where (a.u_id == curUserId) select a).ToList();
            foreach (TB_Join item in joined)
            {
                List<TB_Match> matchList = (from a in dbMat.TB_Match where (a.m_id == item.m_id) select a).ToList();
                MatchViewModels matchView;
                foreach (TB_Match match in matchList)
                {
                    matchView = new MatchViewModels();
                    matchView.id = match.m_id;
                    matchView.name = match.name;
                    matchView.sponsor = match.sponsor;
                    matchView.time = match.s_time.ToShortDateString() + " —— " + match.e_time.ToShortDateString();
                    matchView.location = match.location;
                    if (match.description == null)
                    {
                        matchView.description = "nothing here...";
                    }
                    else
                    {
                        matchView.description = match.description;
                    }
                    viewModel.enrollMatches.Add(matchView);
                }
            }

            viewModel.collection = (from a in dbCol.TB_Collection where (a.u_id == curUserId) select a).ToList();
            viewModel.data = new List<Data5>();
            foreach(TB_Collection collection in viewModel.collection)
            {
                search(collection.teamname, viewModel.data);
                if(viewModel.data.Count >= 10)
                {
                    break;
                }
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Homepage(int? id)
        {
            if(string.IsNullOrEmpty(Request.Form["Apply"]) == false)
            {
                UserLoginModel user = (Session["UserMessage"] as UserLoginModel);
                id = user.id;
                TB_User my = dbUse.TB_User.Find(id);
                if(my != null)
                {
                    if(my.authority == Users.NORMAL_USER)
                    {
                        my.authority = Users.MANAGER_USER;
                        if(ModelState.IsValid)
                        {
                            dbUse.Entry(my).State = EntityState.Modified;
                            dbUse.SaveChanges();
                            return Content("<script language='javascript'>alert('You are match manager now!');history.go(-1);</script>");
                        }
                    }
                    return Content("<script language='javascript'>alert('You already have this privilege!');top.location='/Match/CreateMatch';</script>");
                }
            }
            return View();
        }

        private void search(string team, List<Data5> list)
        {

            UserLoginModel user = (Session["UserMessage"] as UserLoginModel);
            int uid = user.id;
            string appkey1 = "ff18dd43910ba9b60bb06952c51b1541"; //配置您申请的appkey足球
            string appkey2 = "eed4433c18eda1018b26dfdba48c6d26"; //配置您申请的appkey篮球
            string url1 = "http://op.juhe.cn/onebox/football/league";
            var parameters1 = new Dictionary<string, string>();

            parameters1.Add("key", appkey1);//你申请的key
            parameters1.Add("dtype", ""); //返回数据的格式,xml或json，默认json
            parameters1.Add("league", team); //联赛名称

            string result1 = sendPost(url1, parameters1, "get");

            JsonObject newObj1 = new JsonObject(result1);
            string errorCode1 = newObj1["error_code"].Value;
            

            if (errorCode1 == "0")
            {
                string json1 = result1;
                Content game1 = JsonConvert.DeserializeObject<Content>(json1);

                foreach (var item in game1.result.views.saicheng1)
                {
                    Data5 data5 = ToData5(item);
                    data5.status = 0;
                    foreach (TB_Collection collection in dbCol.TB_Collection)
                    {
                        if (collection.u_id == uid)
                        {
                            if (collection.teamname == data5.team1)
                            {
                                if (data5.status != 0)
                                {
                                    data5.status = 3; break;
                                }
                                data5.status = 1;
                            }
                            else if (collection.teamname == data5.team2)
                            {
                                if (data5.status != 0)
                                {
                                    data5.status = 3; break;
                                }
                                data5.status = 2;
                            }
                        }
                    }
                    list.Add(data5);
                }
            }
            else
            {
                //Debug.WriteLine("失败");
                //Console.WriteLine(newObj1["error_code"].Value + ":" + newObj1["reason"].Value);
                //2.足球队名查询
                string url2 = "http://op.juhe.cn/onebox/football/team";

                var parameters2 = new Dictionary<string, string>();

                parameters2.Add("key", appkey1);//你申请的key
                parameters2.Add("dtype", ""); //返回数据的格式,xml或json，默认json
                parameters2.Add("team", team); //球队名称

                string result2 = sendPost(url2, parameters2, "get");

                JsonObject newObj2 = new JsonObject(result2);
                string errorCode2 = newObj2["error_code"].Value;

                if (errorCode2 == "0")
                {
                    string json2 = result2;
                    Content game2 = JsonConvert.DeserializeObject<Content>(json2);

                    foreach (var item in game2.result.list)
                    {
                        Data5 data5 = ToData5(item);
                        data5.status = 0;
                        foreach (TB_Collection collection in dbCol.TB_Collection)
                        {
                            if (collection.u_id == uid)
                            {
                                if (collection.teamname == data5.team1)
                                {
                                    if (data5.status != 0)
                                    {
                                        data5.status = 3; break;
                                    }
                                    data5.status = 1;
                                }
                                else if (collection.teamname == data5.team2)
                                {
                                    if (data5.status != 0)
                                    {
                                        data5.status = 3; break;
                                    }
                                    data5.status = 2;
                                }
                            }
                        }
                        list.Add(data5);
                    }

                }
                else
                {
                    //Debug.WriteLine("失败");
                    //Console.WriteLine(newObj2["error_code"].Value + ":" + newObj2["reason"].Value);
                    ////3.篮球队赛程赛事查询
                    string url3 = "http://op.juhe.cn/onebox/basketball/team";

                    var parameters3 = new Dictionary<string, string>();

                    parameters3.Add("key", appkey2);//你申请的key
                    parameters3.Add("dtype", ""); //返回数据的格式,xml或json，默认json
                    parameters3.Add("team", team); //球队名称

                    string result3 = sendPost(url3, parameters3, "get");

                    JsonObject newObj3 = new JsonObject(result3);
                    string errorCode3 = newObj3["error_code"].Value;

                    if (errorCode3 == "0")
                    {
                        Console.WriteLine("成功");
                        // Console.WriteLine(newObj2);
                        string json3 = result3;
                        Content game3 = JsonConvert.DeserializeObject<Content>(json3);

                        foreach (var item in game3.result.list)
                        {
                            Data5 data5 = ToData52(item);
                            data5.status = 0;
                            foreach (TB_Collection collection in dbCol.TB_Collection)
                            {
                                if (collection.u_id == uid)
                                {
                                    if (collection.teamname == data5.team1)
                                    {
                                        if (data5.status != 0)
                                        {
                                            data5.status = 3; break;
                                        }
                                        data5.status = 1;
                                    }
                                    else if (collection.teamname == data5.team2)
                                    {
                                        if (data5.status != 0)
                                        {
                                            data5.status = 3; break;
                                        }
                                        data5.status = 2;
                                    }
                                }
                            }
                            list.Add(data5);
                        }
                    }
                }
            }
        }


        public static Data5 ToData5(data4 _data4)
        {
            Data5 data5 = new Data5();
            data5.time = _data4.c2 + "  " + _data4.c3;
            data5.team1 = _data4.c4T1;
            data5.team2 = _data4.c4T2;
            data5.state = _data4.c1;
            data5.score = _data4.c4R;
            data5.src = _data4.c53Link;


            return data5;
        }

        public static Data5 ToData52(data4 _data4)
        {
            Data5 data5 = new Data5();
            data5.time = _data4.m_time;
            data5.team1 = _data4.player1;
            data5.team2 = _data4.player2;
            data5.score = _data4.score;
            data5.src = _data4.m_link1url;
            return data5;
        }

        public static Data5 ToData5(data3 _data3)
        {
            Data5 data5 = new Data5();
            //data5.c1 = _data4.c1;
            data5.time = _data3.c2 + _data3.c3;
            data5.team1 = _data3.c4T1;
            data5.team2 = _data3.c4T2;
            data5.state = _data3.c1;
            data5.score = _data3.c4R;
            data5.src = _data3.c52Link;
            return data5;
        }

        static string sendPost(string url, IDictionary<string, string> parameters, string method)
        {
            if (method.ToLower() == "post")
            {
                HttpWebRequest req = null;
                HttpWebResponse rsp = null;
                System.IO.Stream reqStream = null;
                try
                {
                    req = (HttpWebRequest)WebRequest.Create(url);
                    req.Method = method;
                    req.KeepAlive = false;
                    req.ProtocolVersion = HttpVersion.Version10;
                    req.Timeout = 5000;
                    req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
                    byte[] postData = Encoding.UTF8.GetBytes(BuildQuery(parameters, "utf8"));
                    reqStream = req.GetRequestStream();
                    reqStream.Write(postData, 0, postData.Length);
                    rsp = (HttpWebResponse)req.GetResponse();
                    Encoding encoding = Encoding.GetEncoding(rsp.CharacterSet);
                    return GetResponseAsString(rsp, encoding);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                finally
                {
                    if (reqStream != null) reqStream.Close();
                    if (rsp != null) rsp.Close();
                }
            }
            else
            {

                //创建请求
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "?" + BuildQuery(parameters, "utf8"));

                //GET请求
                request.Method = "GET";
                request.ReadWriteTimeout = 5000;
                request.ContentType = "text/html;charset=UTF-8";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));

                //返回内容
                string retString = myStreamReader.ReadToEnd();
                return retString;
            }
        }

        /// <summary>
        /// 组装普通文本请求参数。
        /// </summary>
        /// <param name="parameters">Key-Value形式请求参数字典</param>
        /// <returns>URL编码后的请求数据</returns>
        static string BuildQuery(IDictionary<string, string> parameters, string encode)
        {
            StringBuilder postData = new StringBuilder();
            bool hasParam = false;
            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name))//&& !string.IsNullOrEmpty(value)
                {
                    if (hasParam)
                    {
                        postData.Append("&");
                    }
                    postData.Append(name);
                    postData.Append("=");
                    if (encode == "gb2312")
                    {
                        postData.Append(HttpUtility.UrlEncode(value, Encoding.GetEncoding("gb2312")));
                    }
                    else if (encode == "utf8")
                    {
                        postData.Append(HttpUtility.UrlEncode(value, Encoding.UTF8));
                    }
                    else
                    {
                        postData.Append(value);
                    }
                    hasParam = true;
                }
            }
            return postData.ToString();
        }

        /// <summary>
        /// 把响应流转换为文本。
        /// </summary>
        /// <param name="rsp">响应流对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>响应文本</returns>
        static string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
        {
            System.IO.Stream stream = null;
            StreamReader reader = null;
            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                reader = new StreamReader(stream, encoding);
                return reader.ReadToEnd();
            }
            finally
            {
                // 释放资源
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
                if (rsp != null) rsp.Close();
            }
        }
    

    public ActionResult personalInfo(int? id)
        {
            TB_User user = new TB_User();
            user = dbUse.TB_User.Find(id);
            if(user != null)
            {
                return View(user);
            }
            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult personalInfo(string username)
        {
            TB_User user = dbUse.TB_User.Find((Session["UserMessage"] as UserLoginModel).id);
            user.username = Request.Form["Username"];
            if(Request.Form["Pwd"] != "")
            {
                user.pwd = Request.Form["Pwd"];
            }
            user.name = Request.Form["Name"];
            if(Request.Form["Gender"] == "male")
            {
                user.gender = 1;
            }
            else if(Request.Form["Gender"] == "female")
            {
                user.gender = 2;
            }
            else
            {
                user.gender = 0;
            }
            user.card_id = Request.Form["Cardid"];
            user.phone = Request.Form["Phone"];
            if (Request.Form["Birthday"] != "")
            {
                user.birthday = DateTime.Parse(Request.Form["Birthday"]);
            }
            user.organization = Request.Form["Organization"];
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                try
                {
                    dbUse.Entry(user).State = EntityState.Modified;
                    dbUse.SaveChanges();
                }
                catch(Exception e)
                {
                    return Content("<script language='javascript'>alert('Username has been used!');history.go(-1);</script>");
                }
                return Content("<script language='javascript'>alert('Change successfully!');history.go(-1);</script>");
            }
            return RedirectToAction("homepage");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbUse.Dispose();
                dbMat.Dispose();
                dbBuy.Dispose();
                dbTic.Dispose();
                dbJoi.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
