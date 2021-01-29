using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core
{
    class MySqlCore
    {
        private static MySqlCore Instance;

        public static MySqlCore GetInstance
        {
            get
            {
                if (Instance == null)
                    Instance = new MySqlCore();
                return Instance;
            }
        }

        public SqlConnection GetSqlConnection(string catalog)
        {
            try
            {
                string w = "Data source=127.0.0.1;Initial Catalog=test;User ID=407551879;Password=64450252";
                SqlConnection sqlConnection = new SqlConnection(w);
                sqlConnection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            return sqlConnection;
        }
    }
}
