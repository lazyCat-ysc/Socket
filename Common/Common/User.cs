using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class User
    {
        private static User Instance; 
        private int userId;
        private string password;
        private string account;
        private string lastLoginTime;
        public int UserId { get; set; }
        public string PassWord { get; set; }
        public string LastLoginTime { get; set; }
        public string Account { get; set; }

        public User()
        {

        }

        public User(int userId, string account, string password, string lastLoginTime)
        {
            this.userId = userId;
            this.password = password;
            this.lastLoginTime = lastLoginTime;
            this.account = account;
        }

        public static User GetInstance
        {
            get
            {
                if (Instance == null)
                    Instance = new User();
                return Instance;
            }
        }
    }
}
