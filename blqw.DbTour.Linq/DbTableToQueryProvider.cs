using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    public partial class DbTable<T> : IQueryProvider
    {
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (expression.NodeType != ExpressionType.Call)
            {
                throw new NotSupportedException("无法处理");
            }
            var call = (MethodCallExpression)expression;
            switch (call.Method.Name)
            {
                case "Where":
                    //Expression.Lambda(call.Arguments[1], call.Arguments[0]);
                    var expr = ((UnaryExpression)call.Arguments[1]).Operand as LambdaExpression;

                    if (expr == null)
                    {
                        throw new NotSupportedException("expression不是有效的LambdaExpression对象");
                    }
                    var faller = Faller.Create(expr);
                    var sql = faller.ToWhere(_Saw);
                    Console.WriteLine(sql);
                    return new DbTable<TElement>(_db);
                default:
                    break;
            }
            throw new NotImplementedException();
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return CreateQuery<T>(Expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            throw new NotImplementedException();
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

    }
}
