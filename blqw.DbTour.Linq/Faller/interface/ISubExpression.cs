
using System.Data.Common;
namespace blqw
{
    public interface ISubExpression
    {
        System.Linq.Expressions.LambdaExpression ParentExpression { get; set; }
        bool IsParsingPattern { get; set; }
        string CommandText { get; }
        DbParameter[] Parameters { get; }
    }
}
