using blqw;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demo
{
    static class DbTour_Demo
    {
        public static void Demo()
        {
            using (var db = new DbTour("default"))
            {
                var list = db.Sql("select * from Test_User").ToList<User>();
                foreach (var a in list)
                {
                    Console.WriteLine("{0} | {1} | {2} | {3}", a.ID, a.Name, a.Sex, a.Birthday);
                }
                var b = db.Sql("select top 1 * from Test_User").FirstOrDefault<User>();
                Console.WriteLine("{0} | {1} | {2} | {3}", b.ID, b.Name, b.Sex, b.Birthday);
            }

            using (var db = new DbTour("default"))
            {
                dynamic list = db.Sql("select * from Test_User").ToList();
                foreach (var a in list)
                {
                    Console.WriteLine("{0} | {1} | {2} | {3}", a.ID, a.Name, a.Sex, a.Birthday);
                }
                dynamic b = db.Sql("select top 1 * from Test_User").FirstOrDefault();
                Console.WriteLine("{0} | {1} | {2} | {3}", b.ID, b.Name, b.Sex, b.Birthday);
            }

            using (var db = new DbTour("default"))
            {
                var list = db.Sql("select * from Test_User").ToList(row => new { Id = row["Id"], Name = row["Name"] });
                foreach (var a in list)
                {
                    Console.WriteLine("{0} | {1}", a.Id, a.Name);
                }
                var b = db.Sql("select count(1) from Test_User").ExecuteScalar<int>(-1);
                Console.WriteLine(b);
            }

            using (var db = new DbTour("default"))
            {
                db.Sql("select * from Test_User").ExecuteReader(
                    reader => {
                        while (reader.Read())
                        {
                            Console.WriteLine("{0} | {1}", reader[0], reader[1]);
                        }
                    }
                );
                var table = db.Sql("select * from Test_User").ExecuteDataTable();

                foreach (DataRow row in table.Rows)
                {
                    Console.WriteLine(string.Join(" | ", row.ItemArray));
                }
            }

            using (var db = new DbTour("default"))
            {
                var a = db.Sql("select * from Test_User where ID = {0}", 1).FirstOrDefault<User>();
                Console.WriteLine("{0} | {1} | {2} | {3}", a.ID, a.Name, a.Sex, a.Birthday);
            }

            using (var db = new DbTour("default"))
            {
                var a = db.Sql("select * from Test_User where ID = {0}", 1).FirstOrDefault<User>();
                Console.WriteLine("{0} | {1} | {2} | {3}", a.ID, a.Name, a.Sex, a.Birthday);
            }

            using (var db = new DbTour("default"))
            {
                dynamic a = db.Sql("select * from Test_User where ID > {0:id} and Name like '%' + {0:name} + '%'", new { ID = 1, Name = "王" }).FirstOrDefault();
                Console.WriteLine("{0} | {1} | {2} | {3}", a.ID, a.Name, a.Sex, a.Birthday);
            }

            using (var db = new DbTour("default"))
            {
                var a = new { ID = 1, Name = "" };
                db.Sql("select {0:out name} = Name from Test_User where ID = {0:id}", a).Execute();
                Console.WriteLine(a.Name);
            }

            using (var db = new DbTour("default"))
            {
                var p = new System.Data.SqlClient.SqlParameter {
                    ParameterName = "x",
                    Size = -1,
                    Direction = System.Data.ParameterDirection.Output,
                    SqlDbType = System.Data.SqlDbType.NVarChar
                };
                db.Sql("select {1} = Name from Test_User where ID = {0}", 1, p).Execute();
                Console.WriteLine(p.Value);
            }



            using (var db = new DbTour("default"))
            {
                var p = new System.Data.SqlClient.SqlParameter {
                    ParameterName = "x",
                    Size = -1,
                    Direction = System.Data.ParameterDirection.Output,
                    SqlDbType = System.Data.SqlDbType.NVarChar
                };
                db.Sql("select {1} = Name from Test_User where ID = {0}", 1, p).Execute();
                Console.WriteLine(p.Value);
            }

            SameNameCount(1);

            using (var db = new DbTour("default"))
            {
                db.Begin();
                SetName(1, "张三");
                var count = db.Sql("select count(1) from Test_User where Name = {0}", "张三").ExecuteScalar<int>();
                if (count > 1)
                {
                    db.Rollback();
                }
                else
                {
                    db.Commit();
                }
            }

        }


        static void SetName(int id, string name)
        {
            using (var db = new DbTour("default"))
            {
                const string sql = "UPDATE [Test_User] SET [Name] = {1} WHERE ID = {0}";
                db.Sql(sql, id, name).Execute();
            }
        }

        static int SameNameCount(int id)
        {
            using (var db = new DbTour("default"))
            {
                var name = GetName(1); //内部调用
                return db.Sql("select count(1) from Test_User where Name = {0}", name).ExecuteScalar<int>();
            }
        }

        static string GetName(int id)
        {
            using (var db = new DbTour("default"))
            {
                return db.Sql("select  Name from Test_User where ID = {0}", id).ExecuteScalar<string>();
            }
        }


    }
}
