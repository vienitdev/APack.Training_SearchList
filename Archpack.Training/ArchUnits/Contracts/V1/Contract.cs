using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Archpack.Training.Properties;

namespace Archpack.Training.ArchUnits.Contracts.V1
{
    /// <summary>
    /// 事前条件としての値を判定する機能を提供します。
    /// </summary>
    public static class Contract
    {
        /// <summary>
        /// 指定された値が <see cref="false"/> の場合は、<see cref="ArgumentException"/> を発行します。
        /// </summary>
        /// <param name="condition">判定結果</param>
        /// <param name="parameterName">パラメーター名</param>
        public static void Assert(bool condition, string parameterName)
        {
            if (!condition)
            {
                throw new ArgumentException(string.Format(Resources.ArgumentAssertMessage, parameterName));
            }
        }
        /// <summary>
        /// 指定された値が null の場合は、 <see cref="ArgumentNullException"/> を発行します。
        /// </summary>
        /// <param name="value">判定する値</param>
        /// <param name="parameterName">パラメーター名</param>
        public static void NotNull(Object value, string parameterName)
        {
            if (object.ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(parameterName, string.Format(Resources.ArgumentPropertyNullMessageFormat, parameterName));
            }
        }
        /// <summary>
        /// 指定された値が null の場合は <see cref="ArgumentNullException"/> 、空文字の場合は、 <see cref="ArgumentException"/> を発行します。
        /// </summary>
        /// <param name="value">判定する値</param>
        /// <param name="parameterName">パラメーター名</param>
        public static void NotEmpty(string value, string parameterName)
        {
            Contract.NotNull(value, parameterName);
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(string.Format(Resources.ArgumentIsEmptyMessageFormat, parameterName), parameterName);
            }
        }
    }
}