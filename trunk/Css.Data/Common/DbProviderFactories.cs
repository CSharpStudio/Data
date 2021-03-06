﻿using Css.Data.Properties;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Text;

namespace Css.Data.Common
{
    public class DbProviderFactories
    {
        static DbProviderFactories()
        {
            RegisterFactory(DbProvider.SqlClient, SqlClientFactory.Instance);
            RegisterFactory(DbProvider.Oracle, OracleClientFactory.Instance);
        }

        static Dictionary<string, DbProviderFactory> _factories = new Dictionary<string, DbProviderFactory>();

        public static DbProviderFactory GetFactory(string provider)
        {
            DbProviderFactory result;
            if (_factories.TryGetValue(provider, out result))
                return result;
            throw new DataException(Css.Data.Properties.Resources.DbProviderFactoryNotFound.FormatArgs(provider));
        }

        public static void RegisterFactory(string providerInvariantName, DbProviderFactory factory)
        {
            _factories.Add(providerInvariantName, factory);
        }

        public static IEnumerable<string> GetFactoryProviderNames()
        {
            return _factories.Keys;
        }
    }
}
