using blqw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demo
{
    class DbTour_Linq_Demo
    {
        public static void Demo()
        {
            using (var db = new DbTour())
            {
                var u = db.Table<User>()//.Where(it => it.ID == 7)
                            .Select(it => new {
                                A = db.Table<User>().Where(x => x.Sex == it.Sex).Select(x => x.Name).FirstOrDefault()
                            }
                            ).FirstOrDefault();
                Console.WriteLine(u);
            }
            //    var a = from u1 in db.Table<User>()
            //            from u2 in db.Table<User>()
            //            from u3 in db.Table<User>()
            //            where u1.ID == u2.ID && u1.Name == "xxx"
            //            select u2;


            //    var user = db.Table<User>().Where(it => it.ID == 1).FirstOrDefault();
            //    Console.WriteLine(string.Join(" | ", user.ID, user.Name, user.Sex, user.Birthday));
            //}
        }

    }
}
