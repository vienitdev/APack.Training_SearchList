using Archpack.Training.ArchUnits.Configuration.V1;
using Archpack.Training.ArchUnits.Contracts.V1;
using Archpack.Training.ArchUnits.Data.Sql.V1;
using Archpack.Training.ArchUnits.Logging.V1;
using Archpack.Training.ArchUnits.Routing.V1;
using Archpack.Training.Properties;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;

namespace Archpack.Training.ArchUnits.Arcs.Authentications.V1
{
    /// <summary>
    /// ユーザー情報を取得します。
    /// </summary>
    public class EmployeeInformation
    {
        private static string sql = 
@"SELECT 
    TUSR001.AEMPID as EmployeeID,
    TUSR001.AEMPNO as EmployeeNO,
    TUSR001.AFNM as FirstName,
    TUSR001.ALNM as LastName,
    TUSR001.AFKN as FirstKanaName,
    TUSR001.ALKN as LastKanaName,
    TUSR001.AML as Mail,
    TUSR001.AMOBTEL as MobilePhone,
    TUSR001.AFAX as FAX,
    TUSR001.AGAISEN as ExtensionPhone,
    TUSR001.ANAISEN as InternalPhone,
    TUSR002.AGRPID as GroupID,
    TUSR002.AGRPNO as GroupNo,
    TUSR002.AGRPNM as GroupName,
    TUSR002.ALGRPRYAK as GroupShortName,
    TUSR004.ARLID as RoleID,
    TUSR004.ARLNM as RoleName
FROM 
    TUSR001 INNER JOIN TUSR010 ON 
    TUSR001.AEMPID = TUSR010.AEMPID
    INNER JOIN TUSR002 ON 
    TUSR010.AGRPID = TUSR002.AGRPID
    INNER JOIN TUSR005 ON 
    TUSR005.AEMPID = TUSR001.AEMPID
    INNER JOIN TUSR004 ON
    TUSR004.ARLID = TUSR005.ARLID
WHERE
    (
    LOWER(TUSR001.AML) = LOWER(CONCAT(:UserID/*VARCHAR2(500)*/,'@intelligence.local'))
        OR LOWER(TUSR001.AML) = LOWER(CONCAT(:UserID/*VARCHAR2(500)*/, '@inte.co.jp'))
        OR LOWER(TUSR001.AML) = LOWER(CONCAT(:UserID/*VARCHAR2(500)*/, '@exe.inte.co.jp'))
    )
    AND 
    TUSR010.AMAINGRPFL = '1'
    AND TUSR002.ADLFL = '0'
    AND TUSR010.ADLFL = '0'
    AND TUSR004.ADLFL = '0'
    AND TUSR005.ADLFL = '0'
ORDER BY
    TUSR001.AEMPID";

        private LogContext logContext = null;
        private Dictionary<string, Employee> employeePool = new Dictionary<string, Employee>();

        public EmployeeInformation(LogContext logContext)
        {
            this.logContext = logContext;
        }

        public EmployeeInformation()
        {
        }

        /// <summary>
        /// ログインしているユーザーの情報を取得します。
        /// </summary>
        public Employee GetUserInfo(IIdentity identity)
        {
            //ログインしているユーザーのユーザーIDを取得する。
            string userID = GetAuthenticatedUserID(identity);
            if (employeePool.ContainsKey(userID))
            {
                return employeePool[userID];
            }
            //ログインしているユーザーの情報を取得します。
            var employee = GetAuthenticatedUserInfo(userID);
            employeePool.Add(userID, employee);
            return employee;
        }

        private string GetAuthenticatedUserID(IIdentity identity)
        {            

            if (!identity.IsAuthenticated)
            {
                throw new InvalidOperationException(Resources.UserNotAuthenticated);
            }
            
            string userID = identity.Name;
            if (identity is WindowsIdentity)
            {
                userID = userID.Split('\\').Last();
            }
            
            return userID;
        }

        private Employee GetAuthenticatedUserInfo(string userID)
        {
            using (EmployeeInformationEntities context = EmployeeInformationEntities.CreateContext())
            {
                FileSqlDefinitionFactory factory = new FileSqlDefinitionFactory("ServiceUnits/Actos/V1/");
                DataQuery query = new DataQuery(context, factory, logContext).AppendQuery(sql);

                query.SetParameter("UserID", userID);

                Employee userInfo = query.GetList<Employee>().FirstOrDefault();

                if (userInfo == null)
                {                    
                    throw new InvalidOperationException(Resources.UserNotExist);
                }

                return userInfo;
            }
        }
    }
}