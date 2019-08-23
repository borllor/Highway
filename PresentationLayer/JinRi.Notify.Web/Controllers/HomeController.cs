using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JinRi.Notify.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
//            MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(@"server=192.168.1.25;user id=price01;password=price01!@#;database=fltpricedb;port=33106;CharSet=gb2312;
//");
//            conn.Open();
//            MySql.Data.MySqlClient.MySqlCommand command = conn.CreateCommand();
//            command.CommandText = @"SELECT CURRENT_TIMESTAMP() ";
//            object obj = command.ExecuteScalar();
//            conn.Close();

            return View();
        }

    }
}
