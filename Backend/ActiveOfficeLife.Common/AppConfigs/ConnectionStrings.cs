using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Common.AppConfigs
{
    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; } = string.Empty;
        public string DefaultConnectionLocal { get; set; } = string.Empty;
        public string RedisConnection { get; set; } = string.Empty;
        public string MongoDBConnection { get; set; } = string.Empty;
        public string MySQLConnection { get; set; } = string.Empty;
        public string PostgreSQLConnection { get; set; } = string.Empty;
        public string OracleConnection { get; set; } = string.Empty;
        public string SqliteConnection { get; set; } = string.Empty;
        public string SqlServerConnection { get; set; } = string.Empty;
    }
}
