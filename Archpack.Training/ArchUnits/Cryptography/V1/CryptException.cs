using Archpack.Training.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Cryptography.V1
{
    /// <summary>
    /// 暗号化・複合化例外。
    /// </summary>
    public class CryptException : Exception
    {
        /// <summary>
        /// 暗号化・複合化例外のコンストラクター。
        /// </summary>
        public CryptException() : base(Resources.SystemErrorMessage) { }

        /// <summary>
        /// 暗号化・複合化例外のコンストラクター。
        /// </summary>
        /// <param name="innerException"></param>
        public CryptException(Exception innerException) : base(Resources.SystemErrorMessage, innerException) { }
    }
}