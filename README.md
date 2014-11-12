#blqw.DbTour 开始美妙的数据之旅

> 
> 尝试一种新的解决方案    
> 结合手写Sql的灵活 和 ORM的方便    
> 带来全新的爽快感受
>  


##更新说明
#### 挂起项目,重构下思路

#### 2014.11.12 
* 整合 3个数据库操作工具类库 [DBHelper](https://coding.net/u/blqw/p/blqw-DBHelper/git),[FQL](https://coding.net/u/blqw/p/blqw-FQL/git),[Faller](https://coding.net/u/blqw/p/blqw-Faller/git) 打造DbTour
* 初始版本代码完成 Demo完成



## 灵活的手写Sql
#### 1. 参数化Sql,全面采用微软string.Format风格编程
```csharp
using (var db = new DbTour("default"))
{
    var a = db.Sql("select * from Test_User where ID = {0}", 1).FirstOrDefault<User>();
    Console.WriteLine("{0} | {1} | {2} | {3}", a.ID, a.Name, a.Sex, a.Birthday);
}
```

#### 2. 灵活的Format传递模式
`*` 遵循数据库**忽略大小写**的编码规则
```csharp
using (var db = new DbTour("default"))
{
    dynamic a = db.Sql("select * from Test_User where ID > {0:id} and Name like '%' + {0:name} + '%'", new { ID = 1, Name = "王" }).FirstOrDefault();
    Console.WriteLine("{0} | {1} | {2} | {3}", a.ID, a.Name, a.Sex, a.Birthday);
}
```

#### 3. 方便的out返回参数
`*` 格式化参数规则同**C#编程规则**,方便好记
`*` 参数支持匿名类
`*` 支持**输出参数**
```csharp
using (var db = new DbTour("default"))
{
    var a = new { ID = 1, Name = "" };
    db.Sql("select {0:out name} = Name from Test_User where ID = {0:id}", a).Execute();
    Console.WriteLine(a.Name);
}
```

#### 4. 支持DbParameter参数调用
`*` 如果数据库参数类型无法使用C#类型映射,比如Oracle的Cursor,也可以**直接使用DbParameter对象**
```csharp
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
```

## 方便的实体转换

#### 1. 仿Linq的方法名称
```csharp
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
```
#### 2. 完全自动的类型转换
```csharp
using (var db = new DbTour("default"))
{
    var list = db.Sql("select * from Test_User").ToList(row => new { Id = row["Id"], Name = row["Name"] });
    foreach (var a in list)
    {
        Console.WriteLine("{0} | {1}", a.Id, a.Name);
    }
    var b = db.Sql("select count(1) from [User]").ExecuteScalar<int>();
    Console.WriteLine(b);
}
```
#### 3. 支持动态类型(需.NET4.0以上)
```csharp
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
```
#### 4. 拓展原生方法
`*` 由于DataReader需要及时释放,所以和原生的方法稍有不同   
`*` 增加了原生中没有的ExecuteDataSet()方法
```csharp
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
```

## 合理的DbConnection调用
#### 1. 内部调用时使用同一个Connection,避免多次Open(),Close()
```csharp

static void Main(string[] args)
{
    SameNameCount(1);
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

```

#### 2. 支持当前线程跨方法的事务
```csharp
//方法仅作为演示使用
static void Main(string[] args)
{
    using (var db = new DbTour("default"))
    {
        db.Begin();
        SetName(1, "张三");
        var count = db.Sql("select count(1) from Test_User where Name = {0}","张三").ExecuteScalar<int>();
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
        const string sql = "UPDATE [Test_User] SET [Name] = {1} WHERE Where ID = {0}";
        db.Sql(sql, id, name).Execute();
    }
}

```


> 有其他好建议请留言..