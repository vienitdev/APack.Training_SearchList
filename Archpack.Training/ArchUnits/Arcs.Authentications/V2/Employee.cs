using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Arcs.Authentications.V2
{
    public class Employee
    {
        /// <summary>
        /// 社員ID
        /// </summary>
        public long EmployeeID { get; set; }

        /// <summary>
        /// 社員NO
        /// </summary>
        public string EmployeeNO { get; set; }

        /// <summary>
        /// 姓
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// 名
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// 姓カナ
        /// </summary>
        public string LastKanaName { get; set; }

        /// <summary>
        /// 名カナ
        /// </summary>
        public string FirstKanaName { get; set; }

        /// <summary>
        /// メールアドレス
        /// </summary>
        public string Mail { get; set; }

        /// <summary>
        /// 携帯TEL
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// FAX
        /// </summary>
        public string FAX { get; set; }

        /// <summary>
        /// 外線番号
        /// </summary>
        public string ExtensionPhone { get; set; }

        /// <summary>
        /// 内線番号
        /// </summary>
        public string InternalPhone { get; set; }

        /// <summary>
        /// ロール ID
        /// </summary>
        public string RoleID { get; set; }

        /// <summary>
        /// ロール名
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// グループID
        /// </summary>
        public int GroupID { get; set; }

        /// <summary>
        /// グループ名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// グループ名略称
        /// </summary>
        public string GroupShortName { get; set; }

        /// <summary>
        /// 所属呼込みセンターID
        /// </summary>
        public long? CenterID { get; set; }

        /// <summary>
        /// 架空担当フラグ
        /// </summary>
        public long? VirtualAgentFlag { get; set; }

        /// <summary>
        /// 所属呼込みセンター名
        /// </summary>
        public string CenterName { get; set; }

        /// <summary>
        /// 所属呼込みセンターメールアドレス
        /// </summary>
        public string CenterMailAddress { get; set; }

    }
}