using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw;
using System.Data;

namespace demo
{
    class Program
    {
        static void Main(string[] args)
        {
            //CreateTable();

            //DbTour_Demo.Demo();


            DbTour_Linq_Demo.Demo();



            //DropTable();
        }



        static void CreateTable()
        {
            using (var db = new DbTour("default"))
            {
                db.Sql("IF EXISTS(Select 1 From sysobjects Where Name='Test_User' And Xtype='U') DROP TABLE Test_User").Execute();

                db.Sql(@"CREATE TABLE [dbo].[Test_User] (
	[ID] int NOT NULL IDENTITY(1,1) PRIMARY KEY, 
	[Name] nvarchar(50), 
	[Sex] bit, 
	[Birthday] datetime NOT NULL
) ON [PRIMARY]").Execute();
                db.Sql("INSERT INTO Test_User VALUES ({0},{1},{2})", "blqw", 1, DateTime.Parse("1986-10-29")).Execute();
                db.Sql("INSERT INTO Test_User VALUES ({0},{1},{2})", "张三", null, DateTime.Parse("2010-11-1")).Execute();
                db.Sql("INSERT INTO Test_User VALUES ({0:name},{0:sex},{0:birthday})", new User {
                    Name = "李四",
                    Sex = false,
                    Birthday = DateTime.Parse("1999-9-9")
                }).Execute();

                db.Sql("INSERT INTO Test_User VALUES ({0:name},{0:sex},{0:birthday})", new User {
                    Name = "王五",
                    Sex = true,
                    Birthday = DateTime.Parse("2014-11-12")
                }).Execute();
            }
        }

        static void DropTable()
        {
            using (var db = new DbTour("default"))
            {
                db.Sql("DROP TABLE Test_User").Execute();
            }
        }

    }
}
