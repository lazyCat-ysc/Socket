using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core
{
    class MySqlUser
    {
        private static MySqlUser Instance;
        private MySqlConnection mySqlConnection;

        public MySqlUser()
        {
            mySqlConnection = null;
            InitMySql();
        }

        ~MySqlUser()
        {
            CloseMysql();
        }

        private bool CheckMySqlConnection()
        {
            if (mySqlConnection != null)
                return true;
            return false;
        }

        private void CloseMysql()
        {
            if (CheckMySqlConnection())
            {
                mySqlConnection.Close();
                mySqlConnection.Dispose();
                mySqlConnection = null;
            }
        }

        public static MySqlUser GetInstance
        {
            get
            {
                if (Instance == null)
                    Instance = new MySqlUser();
                return Instance;
            }
        }

        private bool HasAccount(string account)
        {
            if (!CheckMySqlConnection())
                return false;
            string cmdStr = string.Format("select * from ysc_user where account = '{0}';", account);
            MySqlCommand sqlCom = new MySqlCommand(cmdStr, mySqlConnection);
            try
            {
                MySqlDataReader subCmdReader = sqlCom.ExecuteReader();
                bool hasRegister = subCmdReader.HasRows;
                subCmdReader.Close();
                return !hasRegister;
            }
            catch (Exception e)
            {
                Console.WriteLine("[DataMgr] CanRegister:" + e.Message);
                return false;
            }
        }

        private void InitMySql()
        {
            try
            {
                mySqlConnection = new MySqlConnection("Data source='ysc.rabbitmq.com';Initial Catalog='ysc';User ID='407551879';Password='64450252'");
                mySqlConnection.Open();
                RegisterAccount("407551879", "64450252", "爱睡觉的懒喵喵");
            }
            catch (MySqlException e)
            {
                CloseMysql();
                Console.WriteLine(e.Message);
            }
        }

        public void RegisterAccount(string account, string password, string name)
        {
            if (!HasAccount(account))
                return;
            string cmdStr = "insert into ysc_user(ACCOUNT,PASSWORD,NAME) values (@account, @password, @name)";
            MySqlCommand sqlCom = new MySqlCommand(cmdStr, mySqlConnection);
            MySqlParameter sqAccount = new MySqlParameter("@account", MySqlDbType.VarChar);
            sqAccount.Value = account;
            MySqlParameter sqPassword = new MySqlParameter("@password", MySqlDbType.Text);
            sqPassword.Value = password;
            MySqlParameter sqName = new MySqlParameter("@name", MySqlDbType.Text);
            sqName.Value = name;
            sqlCom.Parameters.Add(sqAccount);
            sqlCom.Parameters.Add(sqPassword);
            sqlCom.Parameters.Add(sqName);
            try
            {
                int result = sqlCom.ExecuteNonQuery();
                if(result == 1)
                    Console.WriteLine("success");
            }
            catch (System.Exception e)
            {
                Console.WriteLine("error 111111111111111" + e.Message);
            }
        }
    }
}
