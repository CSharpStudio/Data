using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Query.Linq
{
    class SimplePropertyFinder : ExpressionVisitor
    {
        IRepository _repo;
        IProperty _property;

        public SimplePropertyFinder(IRepository repo)
        {
            _repo = repo;
        }

        public IProperty Find(Expression expr)
        {
            Visit(expr);
            return _property;
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            var clrProperty = m.Member as PropertyInfo;
            _property = _repo.FindProperty(clrProperty.Name);
            if (_property is IRefEntityProperty)
                throw new NotSupportedException("不支持引用类型托管属性{0}".FormatArgs(_property.Name));
            return m;
        }
    }
}

