using Css.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Css.Domain
{
    public static class ConfigExtension
    {
        /// <summary>
        /// 使用实体元数据创建器
        /// </summary>
        /// <param name="config"></param>
        /// <param name="creatorType">默认为空，为空时使用<see cref="EntityMetaCreator"/></param>
        /// <returns></returns>
        public static IConfig UseEntityMetaCreator(this IConfig config, Type creatorType = null)
        {
            RT.Service.Register(typeof(IEntityMetaCreator), creatorType ?? typeof(EntityMetaCreator));
            return config;
        }
    }
}
