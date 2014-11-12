#blqw.DbTour ��ʼ���������֮��

> 
> ����һ���µĽ������    
> �����дSql����� �� ORM�ķ���    
> ����ȫ�µ�ˬ�����
>  


##����˵��
#### ������Ŀ,�ع���˼·

#### 2014.11.12 
* ���� 3�����ݿ����������� [DBHelper](https://coding.net/u/blqw/p/blqw-DBHelper/git),[FQL](https://coding.net/u/blqw/p/blqw-FQL/git),[Faller](https://coding.net/u/blqw/p/blqw-Faller/git) ����DbTour
* ��ʼ�汾������� Demo���



## ������дSql
#### 1. ������Sql,ȫ�����΢��string.Format�����
```csharp
using (var db = new DbTour("default"))
{
    var a = db.Sql("select * from Test_User where ID = {0}", 1).FirstOrDefault<User>();
    Console.WriteLine("{0} | {1} | {2} | {3}", a.ID, a.Name, a.Sex, a.Birthday);
}
```

#### 2. ����Format����ģʽ
`*` ��ѭ���ݿ�**���Դ�Сд**�ı������
```csharp
using (var db = new DbTour("default"))
{
    dynamic a = db.Sql("select * from Test_User where ID > {0:id} and Name like '%' + {0:name} + '%'", new { ID = 1, Name = "��" }).FirstOrDefault();
    Console.WriteLine("{0} | {1} | {2} | {3}", a.ID, a.Name, a.Sex, a.Birthday);
}
```

#### 3. �����out���ز���
`*` ��ʽ����������ͬ**C#��̹���**,����ü�
`*` ����֧��������
`*` ֧��**�������**
```csharp
using (var db = new DbTour("default"))
{
    var a = new { ID = 1, Name = "" };
    db.Sql("select {0:out name} = Name from Test_User where ID = {0:id}", a).Execute();
    Console.WriteLine(a.Name);
}
```

#### 4. ֧��DbParameter��������
`*` ������ݿ���������޷�ʹ��C#����ӳ��,����Oracle��Cursor,Ҳ����**ֱ��ʹ��DbParameter����**
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

## �����ʵ��ת��

#### 1. ��Linq�ķ�������
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
#### 2. ��ȫ�Զ�������ת��
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
#### 3. ֧�ֶ�̬����(��.NET4.0����)
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
#### 4. ��չԭ������
`*` ����DataReader��Ҫ��ʱ�ͷ�,���Ժ�ԭ���ķ������в�ͬ   
`*` ������ԭ����û�е�ExecuteDataSet()����
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

## �����DbConnection����
#### 1. �ڲ�����ʱʹ��ͬһ��Connection,������Open(),Close()
```csharp

static void Main(string[] args)
{
    SameNameCount(1);
}
static int SameNameCount(int id)
{
    using (var db = new DbTour("default"))
    {
        var name = GetName(1); //�ڲ�����
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

#### 2. ֧�ֵ�ǰ�߳̿緽��������
```csharp
//��������Ϊ��ʾʹ��
static void Main(string[] args)
{
    using (var db = new DbTour("default"))
    {
        db.Begin();
        SetName(1, "����");
        var count = db.Sql("select count(1) from Test_User where Name = {0}","����").ExecuteScalar<int>();
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


> �������ý���������..