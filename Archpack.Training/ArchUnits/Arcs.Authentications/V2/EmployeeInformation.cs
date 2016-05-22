using Archpack.Training.ArchUnits.Configuration.V1;
using Archpack.Training.ArchUnits.Contracts.V1;
using Archpack.Training.ArchUnits.Data.Sql.V1;
using Archpack.Training.ArchUnits.Logging.V1;
using Archpack.Training.ArchUnits.Routing.V1;
using Archpack.Training.Properties;
using Microsoft.Owin;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;

namespace Archpack.Training.ArchUnits.Arcs.Authentications.V2
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
    TUSR004.ARLNM as RoleName,
    TACTS034.AACNTID as CenterID,
    TACTS034.AIMGCOFL as VirtualAgentFlag,
    TACTS017.AACNTNM as CenterName,
    TACTS017.AACNTML as CenterMailAddress
FROM 
    TUSR001 INNER JOIN TUSR010 ON 
    TUSR001.AEMPID = TUSR010.AEMPID
    INNER JOIN TUSR002 ON 
    TUSR010.AGRPID = TUSR002.AGRPID
    INNER JOIN TUSR005 ON 
    TUSR005.AEMPID = TUSR001.AEMPID
    INNER JOIN TUSR004 ON
    TUSR004.ARLID = TUSR005.ARLID
    INNER JOIN TACTS034 ON
    TACTS034.AEMPID = TUSR001.AEMPID AND TACTS034.ADLFL = '0'
    LEFT OUTER JOIN TACTS017 ON
    TACTS034.AACNTID = TACTS017.AACNTID AND TACTS017.ADLFL = '0'
WHERE
    (    {0}
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
            if (!identity.IsAuthenticated)
            {
                throw new InvalidOperationException(Resources.UserNotAuthenticated);
            }
            string userID = GetAuthenticatedUserID(identity);
            if (employeePool.ContainsKey(userID))
            {
                return employeePool[userID];
            }
            //ログインしているユーザーの情報を取得します。
            var employee = GetAuthenticatedUserInfo(userID);
            if (employee == null)
            {
                throw new InvalidOperationException(Resources.UserNotExist);
            }
            employeePool.Add(userID, employee);
            return employee;
        }

        public bool HasUserInfo(IIdentity identity)
        {
            var id = GetAuthenticatedUserID(identity);
            var employee = GetAuthenticatedUserInfo(id);
            return employee != null;
        }

        private string GetAuthenticatedUserID(IIdentity identity)
        {            
            
            string userID = identity.Name;
            if (identity is WindowsIdentity)
            {
                userID = userID.Split('\\').Last();
            }
            
            return userID;
        }

        private Employee GetAuthenticatedUserInfo(string userID)
        {
            var config = ServiceConfigurationLoader.Load();
            var dir = config.AppSettings["employeeInfoQueryDir"];
            var domains = ((JArray)config.AppSettings["employeeInfoTargetDomains"]).ToList();
            //@archwaytest.local

            var whereClases = domains.Select(d =>
            {
                return string.Format("LOWER(TUSR001.AML) = LOWER(CONCAT(:UserID/*VARCHAR2(500)*/, '@{0}'))", d.Value<string>());
            });

            var newQuery = string.Format(sql, string.Join(" OR ", whereClases));

            using (EmployeeInformationEntities context = EmployeeInformationEntities.CreateContext())
            {
                FileSqlDefinitionFactory factory = new FileSqlDefinitionFactory("ServiceUnits/Actos/V1/");
                DataQuery query = new DataQuery(context, factory, logContext).AppendQuery(newQuery);

                query.SetParameter("UserID", userID);

                return query.GetList<Employee>().FirstOrDefault();
            }
        }
    }
}