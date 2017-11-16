using System;

namespace Css.Domain
{
    [Serializable]
    public class DataEntity : DoubleEntity
    {
        public static readonly Property<double> CreateByProperty = P<DataEntity>.Register(p => p.CreateBy);

        public double CreateBy
        {
            get { return Get(CreateByProperty); }
            set { Set(value, CreateByProperty); }
        }

        public static readonly Property<DateTime> CreateDateProperty = P<DataEntity>.Register(p => p.CreateDate);

        public DateTime CreateDate
        {
            get { return Get(CreateDateProperty); }
            set { Set(value, CreateDateProperty); }
        }

        public static readonly Property<double> UpdateByProperty = P<DataEntity>.Register(p => p.UpdateBy);

        public double UpdateBy
        {
            get { return Get(UpdateByProperty); }
            set { Set(value, UpdateByProperty); }
        }

        public static readonly Property<DateTime> UpdateDateProperty = P<DataEntity>.Register(p => p.UpdateDate);

        public DateTime UpdateDate
        {
            get { return Get(UpdateDateProperty); }
            set { Set(value, UpdateDateProperty); }
        }
    }
}
