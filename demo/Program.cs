using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw;

namespace demo
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new DbTour("default"))
            {
                var list = db.Sql("select * from Category order by sort").ToList<Category>();
                foreach (var a in list)
                {
                    Console.WriteLine("{0} | {1} | {2} | {3} | {4} | {5}",a.ID,a.Name,a.ParnetID,a.Sort,a.Selected,a.FullName);
                }
            }
        }

        class Category
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public int ParnetID { get; set; }
            public int Sort { get; set; }
            public int Selected { get; set; }
            public string FullName { get; set; }
        }

    }
}
