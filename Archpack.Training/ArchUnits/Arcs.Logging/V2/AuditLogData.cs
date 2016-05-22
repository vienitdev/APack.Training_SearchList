using Archpack.Training.ArchUnits.Logging.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Arcs.Logging.V2
{
    public class AuditLogData : LogData
    {
        public static readonly string EmpIDPropertyKey = "EmpID";
        public static readonly string ScreenIDPropertyKey = "ScreenID";
        public static readonly string DivisionPropertyKey = "Division";
        public static readonly string TargetIDListPropertyKey = "TargetIDList";

        public AuditLogData()
        {
            this.Division = AuditDivision.ListReference;
            this.TargetIDList = new long[]{};
        }

        public override string LogName
        {
            get
            {
                return "ArcsAudit";
            }
            set { }
        }

        /// <summary>
        /// 社員IDを取得します。
        /// </summary>
        public long EmpID
        {
            get { return this.Get<int>(EmpIDPropertyKey); }
            set { this.Set(EmpIDPropertyKey, value); }
        }
        /// <summary>
        /// 画面IDを取得します。
        /// </summary>
        public string ScreenID
        {
            get { return this.Get<string>(ScreenIDPropertyKey); }
            set { this.Set(ScreenIDPropertyKey, value); }
        }

        /// <summary>
        /// 取得対象区分を取得します。
        /// </summary>
        public AuditDivision Division
        {
            get { return this.Get<AuditDivision>(DivisionPropertyKey); }
            set
            {
                this.Set(DivisionPropertyKey, value);
            }
        }

        /// <summary>
        /// 取得対象区分を取得します。
        /// </summary>
        public long[] TargetIDList
        {
            get { return this.Get<long[]>(TargetIDListPropertyKey); }
            set { this.Set(TargetIDListPropertyKey, value); }
        }
    }
}
