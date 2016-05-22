using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.WebApiExtensions.V1
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// ミリ秒以下の値を切り捨てた <see cref="DateTimeOffset"/> の新しいインスタンスを返します。
        /// </summary>
        /// <param name="self">ミリ秒以下の切り捨て元となる <see cref="DateTimeOffset"/></param>
        /// <returns>ミリ秒以下の値を切り捨てた <see cref="DateTimeOffset"/> の新しいインスタンス</returns>
        public static DateTimeOffset TruncateMilliseconds(this DateTimeOffset self)
        {
            var tick = 10000000L;
            return new DateTimeOffset(self.Ticks / tick * tick, self.Offset);
        }
        /// <summary>
        /// 時間の値を切り捨てた <see cref="DateTimeOffset"/> の新しいインスタンスを返します。
        /// </summary>
        /// <param name="self">時間の切り捨て元となる <see cref="DateTimeOffset"/></param>
        /// <returns>時間の値を切り捨てた <see cref="DateTimeOffset"/> の新しいインスタンス</returns>
        public static DateTimeOffset TruncateTime(this DateTimeOffset self)
        {
            var tick = 10000000L * 60L * 60L * 24L; //(ミリ/ナノ秒 * 秒 * 分 * 時)
            return new DateTimeOffset(self.Ticks / tick * tick, self.Offset);
        }
        /// <summary>
        /// ミリ秒以下の値を切り捨てた <see cref="Nullable<DateTimeOffset>"/> の新しいインスタンスを返します。
        /// </summary>
        /// <param name="self">ミリ秒以下の切り捨て元となる <see cref="Nullable<DateTimeOffset>"/></param>
        /// <returns>ミリ秒以下の値を切り捨てた <see cref="Nullable<DateTimeOffset>"/> の新しいインスタンス</returns>
        public static DateTimeOffset? TruncateMilliseconds(this DateTimeOffset? self)
        {
            return !self.HasValue ? self : self.Value.TruncateMilliseconds();
        }
        /// <summary>
        /// 時間の値を切り捨てた <see cref="Nullable<DateTimeOffset>"/> の新しいインスタンスを返します。
        /// </summary>
        /// <param name="self">時間の切り捨て元となる <see cref="Nullable<DateTimeOffset>"/></param>
        /// <returns>時間の値を切り捨てた <see cref="Nullable<DateTimeOffset>"/> の新しいインスタンス</returns>
        public static DateTimeOffset? TruncateTime(this DateTimeOffset? self)
        {
            return !self.HasValue ? self : self.Value.TruncateTime();
        }
        /// <summary>
        /// ミリ秒以下の値を切り捨てた <see cref="DateTime"/> の新しいインスタンスを返します。
        /// </summary>
        /// <param name="self">ミリ秒以下の切り捨て元となる <see cref="DateTime"/></param>
        /// <returns>ミリ秒以下の値を切り捨てた <see cref="DateTime"/> の新しいインスタンス</returns>
        public static DateTime TruncateMilliseconds(this DateTime self)
        {
            var tick = 10000000;
            return new DateTime(self.Ticks / tick * tick, self.Kind);
        }
        /// <summary>
        /// 時間の値を切り捨てた <see cref="DateTime"/> の新しいインスタンスを返します。
        /// </summary>
        /// <param name="self">時間の切り捨て元となる <see cref="DateTime"/></param>
        /// <returns>時間の値を切り捨てた <see cref="DateTime"/> の新しいインスタンス</returns>
        public static DateTime TruncateTime(this DateTime self)
        {
            var tick = 10000000L * 60L * 60L * 24L; //(ミリ/ナノ秒 * 秒 * 分 * 時)
            return new DateTime(self.Ticks / tick * tick, self.Kind);
        }
        /// <summary>
        /// ミリ秒以下の値を切り捨てた <see cref="Nullable<DateTime>"/> の新しいインスタンスを返します。
        /// </summary>
        /// <param name="self">ミリ秒以下の切り捨て元となる <see cref="Nullable<DateTime>"/></param>
        /// <returns>ミリ秒以下の値を切り捨てた <see cref="Nullable<DateTime>"/> の新しいインスタンス</returns>
        public static DateTime? TruncateMilliseconds(this DateTime? self)
        {
            return !self.HasValue ? self : self.Value.TruncateMilliseconds();
        }
        /// <summary>
        /// 時間の値を切り捨てた <see cref="Nullable<DateTime>"/> の新しいインスタンスを返します。
        /// </summary>
        /// <param name="self">時間の切り捨て元となる <see cref="Nullable<DateTime>"/></param>
        /// <returns>時間の値を切り捨てた <see cref="Nullable<DateTime>"/> の新しいインスタンス</returns>
        public static DateTime? TruncateTime(this DateTime? self)
        {
            return !self.HasValue ? self : self.Value.TruncateTime();
        }

        public static DateTime StartOfWeek(this DateTime self, DayOfWeek startOfWeek = DayOfWeek.Sunday)
        {
            int diff = self.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return self.AddDays(-1 * diff).Date;
        }

        public static DateTime EndOfWeek(this DateTime self, DayOfWeek startOfWeek = DayOfWeek.Sunday)
        {
            var start = self.StartOfWeek(startOfWeek);
            return start.AddDays(6);
        }

        public static DateTime StartOfMonth(this DateTime self)
        {
            return new DateTime(self.Year, self.Month, 1, 0, 0, 0, self.Kind);
        }

        public static DateTime EndOfMonth(this DateTime self)
        {
            return new DateTime(self.Year, self.Month, 1, 0, 0, 0, self.Kind).AddMonths(1).AddDays(-1);
        }

        public static DateTime ToUnspecified(this DateTime self)
        {
            return new DateTime(self.Ticks, DateTimeKind.Unspecified);
        }
    }
}