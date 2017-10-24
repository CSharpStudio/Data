using Css.Configuration;
using Css.Data.Common;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data
{
    /// <summary>
    /// 数据库配置
    /// </summary>
    public class DbSetting : DbConnectionSchema
    {
        DbSetting() { }

        /// <summary>
        /// 配置名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 查找或者根据约定创建连接字符串
        /// </summary>
        /// <param name="dbSettingName"></param>
        /// <returns></returns>
        public static DbSetting FindOrCreate(string dbSettingName)
        {
            Check.NotNull(dbSettingName, nameof(dbSettingName));

            DbSetting setting = null;

            if (!_generatedSettings.TryGetValue(dbSettingName, out setting))
            {
                lock (_generatedSettings)
                {
                    if (!_generatedSettings.TryGetValue(dbSettingName, out setting))
                    {
                        var cfg = new ConfigurationBuilder();
                        var b =  cfg.Build();
                        b.GetConnectionString("");

                        var config = RT.Config.Get<ConnectionStringSection>(dbSettingName);
                        if (config != null)
                        {
                            setting = new DbSetting
                            {
                                ConnectionString = config.ConnectionString,
                                ProviderName = config.ProviderName,
                            };
                        }
                        else
                        {
                            setting = Create(dbSettingName);
                        }

                        setting.Name = dbSettingName;

                        _generatedSettings.Add(dbSettingName, setting);
                    }
                }
            }

            return setting;
        }

        /// <summary>
        /// 添加一个数据库连接配置。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="connectionString"></param>
        /// <param name="providerName"></param>
        public static DbSetting SetSetting(string name, string connectionString, string providerName)
        {
            Check.NotNullOrEmpty(name, nameof(name));
            Check.NotNullOrEmpty(connectionString, nameof(connectionString));
            Check.NotNullOrEmpty(providerName, nameof(providerName));

            var setting = new DbSetting
            {
                Name = name,
                ConnectionString = connectionString,
                ProviderName = providerName
            };

            lock (_generatedSettings)
            {
                _generatedSettings[name] = setting;
            }

            return setting;
        }

        /// <summary>
        /// 获取当前已经被生成的 DbSetting。
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<DbSetting> GetGeneratedSettings()
        {
            return _generatedSettings.Values;
        }

        static Dictionary<string, DbSetting> _generatedSettings = new Dictionary<string, DbSetting>();

        static DbSetting Create(string dbSettingName)
        {
            //查找连接字符串时，根据用户的 LocalSqlServer 来查找。
            //var local = ConfigurationManager.ConnectionStrings[DbProvider.LocalServer];
            //if (local != null && local.ProviderName == DbProvider.SqlClient)
            //{
            //    var builder = new SqlConnectionStringBuilder(local.ConnectionString);

            //    var newCon = new SqlConnectionStringBuilder();
            //    newCon.DataSource = builder.DataSource;
            //    newCon.InitialCatalog = dbSettingName;
            //    newCon.IntegratedSecurity = builder.IntegratedSecurity;
            //    if (!newCon.IntegratedSecurity)
            //    {
            //        newCon.UserID = builder.UserID;
            //        newCon.Password = builder.Password;
            //    }

            //    return new DbSetting
            //    {
            //        ConnectionString = newCon.ToString(),
            //        ProviderName = local.ProviderName
            //    };
            //}

            return new DbSetting
            {
                ConnectionString = string.Format(@"Data Source={0}.sdf", dbSettingName),
                ProviderName = "System.Data.SqlServerCe"
            };
        }
    }
}