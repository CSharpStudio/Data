using Css.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    [Serializable]
    public abstract class Entity<T> : Entity
    {
        static Entity()
        {
            IdProperty = P<Entity<T>>.Register<T>("Id");
        }

        public T Id
        {
            get { return GetValue<T>(IdProperty); }
            set { SetValue(value, IdProperty); }
        }
    }

    /// <summary>
    /// 以 Guid 作为主键的实体基类。
    /// </summary>
    [Serializable]
    public abstract class GuidEntity : Entity<Guid>
    {
    }

    /// <summary>
    /// 以 int 作为主键的实体基类。
    /// </summary>
    [Serializable]
    public abstract class IntEntity : Entity<int>
    {
    }

    /// <summary>
    /// 以 long 作为主键的实体基类。
    /// </summary>
    [Serializable]
    public abstract class LongEntity : Entity<long>
    {
    }

    /// <summary>
    /// 以 string 作为主键的实体基类。
    /// </summary>
    [Serializable]
    public abstract class StringEntity : Entity<string>
    {
    }

    /// <summary>
    /// 以 double 作为主键的实体基类。
    /// </summary>
    [Serializable]
    public abstract class DoubleEntity : Entity<double>
    {
    }
}
