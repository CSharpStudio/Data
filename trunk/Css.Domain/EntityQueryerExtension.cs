using Css.Data;
using Css.Domain;
using Css.Domain.Query;
using Css.Domain.Query.Impl;
using Css.Domain.Query.Linq;
using Css.Reflection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    public static class EntityQueryerExtension
    {
        /// <summary>
        /// 返回第一个类型为T的实体，找不到时返回null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryer"></param>
        /// <returns></returns>
        public static T FirstOrDefault<T>(this IEntityQueryer<T> queryer) where T : Entity
        {
            var query = queryer.ToQuery();
            return queryer.Repository.FirstOrDefault(query) as T;
        }
        /// <summary>
        /// 返回条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryer"></param>
        /// <returns></returns>
        public static int Count<T>(this IEntityQueryer<T> queryer) where T : Entity
        {
            var query = queryer.ToQuery();
            return queryer.Repository.Count(query);
        }
        /// <summary>
        /// 查询返回EntityList&lt;T&gt;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryer"></param>
        /// <param name="paging"></param>
        /// <param name="eagerLoad"></param>
        /// <returns></returns>
        public static EntityList<T> ToList<T>(this IEntityQueryer<T> queryer) where T : Entity
        {
            var query = queryer.ToQuery();
            return queryer.Repository.ToList(query, queryer.Start, queryer.End) as EntityList<T>;
        }
        /// <summary>
        /// 返回第一个类型为T的对象，找不到时返回default(T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryer"></param>
        /// <returns></returns>
        public static T FirstOrDefault<T>(this IEntityQueryer queryer)
        {
            NewExpression newExpression;
            var query = queryer.ToQuery(out newExpression);
            using (var dba = queryer.Repository.CreateDbAccesser())
            {
                var generator = queryer.Repository.DbTable.CreateSqlGenerator();
                generator.Generate(query as TableQuery);
                using (var reader = dba.ExecuteReader(generator.Sql, dba.Connection.State == ConnectionState.Closed, generator.Sql.Parameters))
                {
                    if (reader.Read())
                    {
                        if (typeof(T) == typeof(object) && newExpression != null) // dynamic
                            return (T)reader.ToObject(newExpression);
                        return reader.ToObject<T>();
                    }
                    return default(T);
                }
            }
        }
        /// <summary>
        /// 查询返回T类型的列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryer"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        public static IList<T> ToList<T>(this IEntityQueryer queryer)
        {
            List<T> result = new List<T>();
            NewExpression newExpression;
            var query = queryer.ToQuery(out newExpression);
            Action<IDataReader> action = null;
            if (typeof(T) == typeof(object) && newExpression != null) // dynamic
                action = r => result.Add((T)r.ToObject(newExpression));
            else
                action = r => result.Add(r.ToObject<T>());
            using (var dba = queryer.Repository.CreateDbAccesser())
            {
                var generator = queryer.Repository.DbTable.CreateSqlGenerator();
                generator.Generate(query as TableQuery, queryer.Start, queryer.End);
                using (var reader = dba.ExecuteReader(generator.Sql, dba.Connection.State == ConnectionState.Closed, generator.Sql.Parameters))
                {
                    while (reader.Read())
                        action(reader);
                    return result;
                }
            }
        }

        public static IConstraint ToSubQuery<T>(this IEntityQueryer<T> queryer)
        {
            var expression = queryer.Expression;
            expression = Evaluator.PartialEval(expression);
            var builder = new EntityQueryerBuilder(queryer.Repository);
            var query = builder.BuildQuery(expression, queryer.Alias);
            return query.Where;
        }
    }

    public static class FunctionExtension
    {
        static NotSupportedException Error()
        {
            return new NotSupportedException("SQL函数不支持直接调用 - 只能用在IEntityQueryer Lambda表达式");
        }
        public static T SQL<T>(this Entity e, FormattedSql sql)
        {
            throw Error();
        }
        public static object SQL(this Entity e, FormattedSql sql)
        {
            throw Error();
        }
        public static int COUNT(this object obj)
        {
            throw Error();
        }
        /// <summary>
        /// SQL函数
        /// </summary>
        public static T NVL<T>(this T property, T value)
        {
            throw Error();
        }
        #region String
        /// <summary>
        /// SQL函数
        /// </summary>
        public static string CONCAT(this string property, string str)
        {
            throw Error();
        }
        /// <summary>
        /// SQL函数
        /// </summary>
        public static string UPPER(this string property)
        {
            throw Error();
        }
        /// <summary>
        /// SQL函数
        /// </summary>
        public static string LOWER(this string property)
        {
            throw Error();
        }
        /// <summary>
        /// SQL函数
        /// </summary>
        public static string SUBSTR(this string property, int start, int length)
        {
            throw Error();
        }
        /// <summary>
        /// SQL函数
        /// </summary>
        public static int LENGTH(this string property)
        {
            throw Error();
        }
        /// <summary>
        /// SQL函数
        /// </summary>
        public static string LTRIM(this string property)
        {
            throw Error();
        }
        /// <summary>
        /// SQL函数
        /// </summary>
        public static string RTRIM(this string property)
        {
            throw Error();
        }
        #endregion
        #region
        /// <summary>
        /// SQL函数
        /// </summary>
        public static decimal SUM(this double property)
        {
            throw Error();
        }
        /// <summary>
        /// SQL函数
        /// </summary>
        public static decimal SUM(this decimal property)
        {
            throw Error();
        }
        /// <summary>
        /// SQL函数
        /// </summary>
        public static decimal SUM(this int property)
        {
            throw Error();
        }
        #endregion
        #region AVG
        /// <summary>
        /// SQL函数
        /// </summary>
        public static decimal AVG(this double property)
        {
            throw Error();
        }
        /// <summary>
        /// SQL函数
        /// </summary>
        public static decimal AVG(this decimal property)
        {
            throw Error();
        }
        /// <summary>
        /// SQL函数
        /// </summary>
        public static decimal AVG(this int property)
        {
            throw Error();
        }
        #endregion
        #region Date
        public static int DAY(this DateTime property)
        {
            throw Error();
        }
        public static int YEAR(this DateTime property)
        {
            throw Error();
        }
        public static int MONTH(this DateTime property)
        {
            throw Error();
        }
        #endregion
        #region MAX
        /// <summary>
        /// SQL函数
        /// </summary>
        public static DateTime MAX(this DateTime property)
        {
            throw Error();
        }
        /// <summary>
        /// SQL函数
        /// </summary>
        public static long MAX(this long property)
        {
            throw Error();
        }
        /// <summary>
        /// SQL函数
        /// </summary>
        public static int MAX(this int property)
        {
            throw Error();
        }
        /// <summary>
        /// SQL函数
        /// </summary>
        public static double MAX(this double property)
        {
            throw Error();
        }
        /// <summary>
        /// SQL函数
        /// </summary>
        public static decimal MAX(this decimal property)
        {
            throw Error();
        }
        /// <summary>
        /// SQL函数
        /// </summary>
        public static string MAX(this string property)
        {
            throw Error();
        }
        #endregion
        #region MIN
        /// <summary>
        /// SQL函数
        /// </summary>
        public static DateTime MIN(this DateTime property)
        {
            throw Error();
        }
        /// <summary>
        /// SQL函数
        /// </summary>
        public static long MIN(this long property)
        {
            throw Error();
        }
        /// <summary>
        /// SQL函数
        /// </summary>
        public static int MIN(this int property)
        {
            throw Error();
        }
        /// <summary>
        /// SQL函数
        /// </summary>
        public static double MIN(this double property)
        {
            throw Error();
        }
        /// <summary>
        /// SQL函数
        /// </summary>
        public static decimal MIN(this decimal property)
        {
            throw Error();
        }
        /// <summary>
        /// SQL函数
        /// </summary>
        public static string MIN(this string property)
        {
            throw Error();
        }
        #endregion
    }
}

