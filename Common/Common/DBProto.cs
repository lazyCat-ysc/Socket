using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class DBProto
    {
        public readonly static string DB_SEVER_VERSION = "1.0.0";

        public enum DBCommand
        {
            DB_YSC_SYS,
            DB_YSC_USER
        }

        public enum DBMOpcode
        {
            DB_YSC_SYS_MDM,
            DB_YSC_USER_MDM
        }

        public enum DBSOpcode
        {
            DB_YSC_USER_SUB_REGISTER
        }

        public enum DBSOpcodeReceive
        {
            DB_YSC_USER_SUB_R_REGISTER
        }
    }
}
