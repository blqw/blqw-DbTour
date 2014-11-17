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
                var user = db.Table<User>().Where(it => it.ID == 1).FirstOrDefault();
                Console.WriteLine(string.Join(" | ", user.ID, user.Name, user.Sex, user.Birthday));
            }
        }

    }
}
