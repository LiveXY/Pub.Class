//------------------------------------------------------------
// All Rights Reserved , Copyright (C) 2006 , LiveXY , Ltd. 
//------------------------------------------------------------

using System;
using System.Collections.Generic;
#if NET20
using Pub.Class.Linq;
#else
using System.Linq;
using System.Web.Script.Serialization;
#endif
using System.Text;
using System.Reflection;
using System.Data;
using System.Data.Common;
using System.Globalization;
using Microsoft.VisualBasic;
using System.Data.SqlTypes;

namespace Pub.Class {
    /// <summary>
    /// 日期扩展
    /// 
    /// 修改纪录
    ///     2009.06.25 版本：1.0 livexy 创建此类
    /// 
    /// </summary>
    public static class DateExtensions {
        private static readonly TimeSpan _OneMinute = new TimeSpan(0, 1, 0);
        private static readonly TimeSpan _TwoMinutes = new TimeSpan(0, 2, 0);
        private static readonly TimeSpan _OneHour = new TimeSpan(1, 0, 0);
        private static readonly TimeSpan _TwoHours = new TimeSpan(2, 0, 0);
        private static readonly TimeSpan _OneDay = new TimeSpan(1, 0, 0, 0);
        private static readonly TimeSpan _TwoDays = new TimeSpan(2, 0, 0, 0);
        private static readonly TimeSpan _OneWeek = new TimeSpan(7, 0, 0, 0);
        private static readonly TimeSpan _TwoWeeks = new TimeSpan(14, 0, 0, 0);
        private static readonly TimeSpan _OneMonth = new TimeSpan(31, 0, 0, 0);
        private static readonly TimeSpan _TwoMonths = new TimeSpan(62, 0, 0, 0);
        private static readonly TimeSpan _OneYear = new TimeSpan(365, 0, 0, 0);
        private static readonly TimeSpan _TwoYears = new TimeSpan(730, 0, 0, 0);
        /// <summary>
        /// 时间差
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>TimeSpan时间差</returns>
        public static TimeSpan GetTimeSpan(this DateTime startTime, DateTime endTime) {
            return endTime - startTime;
        }
        /// <summary>
        /// 日期转字符串yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="time">日期</param>
        /// <returns>转字符串yyyy-MM-dd HH:mm:ss</returns>
        public static string ToDateTime(this DateTime time) {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 日期转字符串yyyy-MM-dd
        /// </summary>
        /// <param name="time">日期</param>
        /// <returns>转字符串yyyy-MM-dd</returns>
        public static string ToDate(this DateTime time) {
            return time.ToString("yyyy-MM-dd");
        }
        /// <summary>
        /// 日期转字符串HH:mm:ss
        /// </summary>
        /// <param name="time">日期</param>
        /// <returns>转字符串HH:mm:ss</returns>
        public static string ToTime(this DateTime time) {
            return time.ToString("HH:mm:ss");
        }
        /// <summary>
        /// 日期转字符串HH:mm
        /// </summary>
        /// <param name="time">日期</param>
        /// <returns>转字符串HH:mm</returns>
        public static string ToHHmm(this DateTime time) {
            return time.ToString("HH:mm");
        }
        /// <summary>
        /// 日期转字符串yyyy-MM-dd HH:mm:ss.fffffff
        /// </summary>
        /// <param name="time">日期</param>
        /// <returns>转字符串yyyy-MM-dd HH:mm:ss.fffffff</returns>
        public static string ToDateTimeF(this DateTime time) {
            return time.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
        }
        /// <summary>
        /// 日期转字符串yyyy-MM-dd HH:mm:ss.fff
        /// </summary>
        /// <param name="time">日期</param>
        /// <returns>转字符串yyyy-MM-dd HH:mm:ss.fff</returns>
        public static string ToDateTimeFFF(this DateTime time) {
            return time.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
        /// <summary>
        /// 取年龄
        /// </summary>
        /// <param name="dateOfBirth">生日日期</param>
        /// <returns>返回年龄</returns>
        public static int ToAge(this DateTime dateOfBirth) {
            return ToAge(dateOfBirth, DateTime.Today);
        }
        /// <summary>
        /// 取年龄
        /// </summary>
        /// <param name="dateOfBirth">生日日期</param>
        /// <param name="referenceDate">参考日期</param>
        /// <returns>返回年龄</returns>
        public static int ToAge(this DateTime dateOfBirth, DateTime referenceDate) {
            int years = referenceDate.Year - dateOfBirth.Year;
            if (referenceDate.Month < dateOfBirth.Month || (referenceDate.Month == dateOfBirth.Month && referenceDate.Day < dateOfBirth.Day)) --years;
            return years;
        }
        /// <summary>
        /// 这月多少天
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns>天数</returns>
        public static int GetCountDaysOfMonth(this DateTime date) {
            var nextMonth = date.AddMonths(1);
            return new DateTime(nextMonth.Year, nextMonth.Month, 1).AddDays(-1).Day;
        }
        /// <summary>
        /// 这月的第一天
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns>这月的第一天DateTime</returns>
        public static DateTime GetFirstDayOfMonth(this DateTime date) {
            return new DateTime(date.Year, date.Month, 1);
        }
        /// <summary>
        /// 这月的第一天
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="dayOfWeek">星期几</param>
        /// <returns>这月的第一天DateTime</returns>
        public static DateTime GetFirstDayOfMonth(this DateTime date, DayOfWeek dayOfWeek) {
            var dt = date.GetFirstDayOfMonth();
            while (dt.DayOfWeek != dayOfWeek) dt = dt.AddDays(1);
            return dt;
        }
        /// <summary>
        /// 这月的最后天
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns>这月的最后天DateTime</returns>
        public static DateTime GetLastDayOfMonth(this DateTime date) {
            return new DateTime(date.Year, date.Month, GetCountDaysOfMonth(date));
        }
        /// <summary>
        /// 这月的最后天
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="dayOfWeek">星期几</param>
        /// <returns>这月的最后天DateTime</returns>
        public static DateTime GetLastDayOfMonth(this DateTime date, DayOfWeek dayOfWeek) {
            var dt = date.GetLastDayOfMonth();
            while (dt.DayOfWeek != dayOfWeek) dt = dt.AddDays(-1);
            return dt;
        }
        /// <summary>
        /// 今天否
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns>true/false</returns>
        public static bool IsToday(this DateTime dt) {
            return (dt.Date == DateTime.Today);
        }
        /// <summary>
        /// 今天否
        /// </summary>
        /// <param name="dto">时间点</param>
        /// <returns>true/false</returns>
        public static bool IsToday(this DateTimeOffset dto) {
            return (dto.Date.IsToday());
        }
        /// <summary>
        /// 重值时间
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="hours">时</param>
        /// <param name="minutes">分</param>
        /// <param name="seconds">秒</param>
        /// <returns>DateTime</returns>
        public static DateTime SetTime(this DateTime date, int hours, int minutes, int seconds) {
            return date.SetTime(new TimeSpan(hours, minutes, seconds));
        }
        /// <summary>
        /// 重值时间
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="time">时间</param>
        /// <returns>DateTime</returns>
        public static DateTime SetTime(this DateTime date, TimeSpan time) {
            return date.Date.Add(time);
        }
#if !(NET20 || MONO40)
        /// <summary>
        /// ToDateTimeOffset 日期转时间点
        /// </summary>
        /// <param name="localDateTime">时间</param>
        /// <returns>DateTimeOffset</returns>
        public static DateTimeOffset ToDateTimeOffset(this DateTime localDateTime) {
            return localDateTime.ToDateTimeOffset(null);
        }
        /// <summary>
        /// ToDateTimeOffset 日期转时间点
        /// </summary>
        /// <param name="localDateTime">时间</param>
        /// <param name="localTimeZone">localTimeZone</param>
        /// <returns>DateTimeOffset</returns>
        public static DateTimeOffset ToDateTimeOffset(this DateTime localDateTime, TimeZoneInfo localTimeZone) {
            localTimeZone = (localTimeZone ?? TimeZoneInfo.Local);
            if (localDateTime.Kind != DateTimeKind.Unspecified) localDateTime = new DateTime(localDateTime.Ticks, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(localDateTime, localTimeZone);
        }
#endif
        /// <summary>
        /// 这周的第一天
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns>这周的第一天DateTime</returns>
        public static DateTime GetFirstDayOfWeek(this DateTime date) {
            return date.GetFirstDayOfWeek(null);
        }
        /// <summary>
        /// 这周的第一天
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="cultureInfo">区域设置</param>
        /// <returns>这周的第一天DateTime</returns>
        public static DateTime GetFirstDayOfWeek(this DateTime date, CultureInfo cultureInfo) {
            cultureInfo = (cultureInfo ?? CultureInfo.CurrentCulture);
            var firstDayOfWeek = cultureInfo.DateTimeFormat.FirstDayOfWeek;
            while (date.DayOfWeek != firstDayOfWeek) date = date.AddDays(-1);
            return date;
        }
        /// <summary>
        /// 这周的最后一天
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns>这周的最后一天DateTime</returns>
        public static DateTime GetLastDayOfWeek(this DateTime date) {
            return date.GetLastDayOfWeek(null);
        }
        /// <summary>
        /// 这周的最后一天
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="cultureInfo">区域设置</param>
        /// <returns>这周的最后一天DateTime</returns>
        public static DateTime GetLastDayOfWeek(this DateTime date, CultureInfo cultureInfo) {
            return date.GetFirstDayOfWeek(cultureInfo).AddDays(6);
        }
        /// <summary>
        /// 周日
        /// </summary>
        /// <param name="date">时间</param>
        /// <param name="weekday">星期几</param>
        /// <returns>周日DateTime</returns>
        public static DateTime GetWeekday(this DateTime date, DayOfWeek weekday) {
            return date.GetWeekday(weekday, null);
        }
        /// <summary>
        /// 周日
        /// </summary>
        /// <param name="date">时间</param>
        /// <param name="weekday">星期几</param>
        /// <param name="cultureInfo">区域设置</param>
        /// <returns>周日DateTime</returns>
        public static DateTime GetWeekday(this DateTime date, DayOfWeek weekday, CultureInfo cultureInfo) {
            var firstDayOfWeek = date.GetFirstDayOfWeek(cultureInfo);
            return firstDayOfWeek.GetNextWeekday(weekday);
        }
        /// <summary>
        /// 下周日
        /// </summary>
        /// <param name="date">时间</param>
        /// <param name="weekday">星期几</param>
        /// <returns>下周日DateTime</returns>
        public static DateTime GetNextWeekday(this DateTime date, DayOfWeek weekday) {
            while (date.DayOfWeek != weekday) date = date.AddDays(1);
            return date;
        }
        /// <summary>
        /// 上周日
        /// </summary>
        /// <param name="date">时间</param>
        /// <param name="weekday">星期几</param>
        /// <returns>上周日DateTime</returns>
        public static DateTime GetPreviousWeekday(this DateTime date, DayOfWeek weekday) {
            while (date.DayOfWeek != weekday) date = date.AddDays(-1);
            return date;
        }
#if !(NET20 || MONO40)
        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="date">时间点</param>
        /// <param name="hours">时</param>
        /// <param name="minutes">分</param>
        /// <param name="seconds">秒</param>
        /// <returns>DateTimeOffset</returns>
        public static DateTimeOffset SetTime(this DateTimeOffset date, int hours, int minutes, int seconds) {
            return date.SetTime(new TimeSpan(hours, minutes, seconds));
        }
        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="date">时间点</param>
        /// <param name="time">时间</param>
        /// <returns>DateTimeOffset</returns>
        public static DateTimeOffset SetTime(this DateTimeOffset date, TimeSpan time) {
            return date.SetTime(time, null);
        }
        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="date">时间点</param>
        /// <param name="time">时间</param>
        /// <param name="localTimeZone">TimeZoneInfo</param>
        /// <returns>DateTimeOffset</returns>
        public static DateTimeOffset SetTime(this DateTimeOffset date, TimeSpan time, TimeZoneInfo localTimeZone) {
            var localDate = date.ToLocalDateTime(localTimeZone);
            localDate.SetTime(time);
            return localDate.ToDateTimeOffset(localTimeZone);
        }
        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="dateTimeUtc">时间点</param>
        /// <returns>DateTime</returns>
        public static DateTime ToLocalDateTime(this DateTimeOffset dateTimeUtc) {
            return dateTimeUtc.ToLocalDateTime(null);
        }
        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="dateTimeUtc">时间点</param>
        /// <param name="localTimeZone">TimeZoneInfo</param>
        /// <returns>DateTime</returns>
        public static DateTime ToLocalDateTime(this DateTimeOffset dateTimeUtc, TimeZoneInfo localTimeZone) {
            localTimeZone = (localTimeZone ?? TimeZoneInfo.Local);
            return TimeZoneInfo.ConvertTime(dateTimeUtc, localTimeZone).DateTime;
        }
        /// <summary>
        /// 把两个时间差，三天内的时间用今天，昨天，前天表示，后跟时间，无日期
        /// </summary>
        /// <param name="time">被比较的时间</param>
        /// <param name="currentDateTime">目标时间</param>
        /// <returns>把两个时间差，三天内的时间用今天，昨天，前天表示，后跟时间，无日期</returns>
        public static string ToAgo(this DateTime time, DateTime currentDateTime) {
            string result = "";
            if (currentDateTime.Year == time.Year && currentDateTime.Month == time.Month) { //如果date和当前时间年份或者月份不一致，则直接返回"yyyy-MM-dd HH:mm"格式日期
                if (DateDiff(DateInterval.Hour, time, currentDateTime) <= 10) { //如果date和当前时间间隔在10小时内(曾经是3小时)
                    if (DateDiff(DateInterval.Hour, time, currentDateTime) > 0) return DateDiff(DateInterval.Hour, time, currentDateTime) + "小时前";
                    if (DateDiff(DateInterval.Minute, time, currentDateTime) > 0) return DateDiff(DateInterval.Minute, time, currentDateTime) + "分钟前";
                    if (DateDiff(DateInterval.Second, time, currentDateTime) >= 0) return DateDiff(DateInterval.Second, time, currentDateTime) + "秒前";
                    else return "刚才";//为了解决postdatetime时间精度不够导致发帖时间问题的兼容
                } else {
                    switch (currentDateTime.Day - time.Day) {
                        case 0: result = "今天 " + time.ToString("HH") + ":" + time.ToString("mm"); break;
                        case 1: result = "昨天 " + time.ToString("HH") + ":" + time.ToString("mm"); break;
                        case 2: result = "前天 " + time.ToString("HH") + ":" + time.ToString("mm"); break;
                        default: result = time.ToString("yyyy-MM-dd HH:mm"); break;
                    }
                }
            } else result = time.ToString("yyyy-MM-dd HH:mm");
            return result;
        }
        /// <summary>
        /// 日期相减
        /// </summary>
        /// <param name="value">开始时间</param>
        /// <param name="date">结束时间</param>
        /// <param name="interval">格式化 DateInterval.Year DateInterval.Month DateInterval.Day DateInterval.Hour DateInterval.Minute DateInterval.Second DateInterval.WeekOfYear DateInterval.Quarter</param>
        /// <returns></returns>
        public static long DateDiff(this DateTime value, DateTime date, DateInterval interval) {
            if (interval == DateInterval.Year) return date.Year - value.Year;
            if (interval == DateInterval.Month) return (date.Month - value.Month) + (12 * (date.Year - value.Year));
            TimeSpan ts = date - value;
            if (interval == DateInterval.Day || interval == DateInterval.DayOfYear) return ts.TotalDays.Round();
            if (interval == DateInterval.Hour) return ts.TotalHours.Round();
            if (interval == DateInterval.Minute) return ts.TotalMinutes.Round();
            if (interval == DateInterval.Second) return ts.TotalSeconds.Round();
            if (interval == DateInterval.Weekday) return (ts.TotalDays / 7.0).Round();
            if (interval == DateInterval.WeekOfYear) {
                while (date.DayOfWeek != DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek) date = date.AddDays(-1);
                while (value.DayOfWeek != DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek) value = value.AddDays(-1);
                ts = date - value;
                return (ts.TotalDays / 7.0).Round();
            }
            if (interval == DateInterval.Quarter) {
                double valueQuarter = GetQuarter(value.Month);
                double dateQuarter = GetQuarter(date.Month);
                double valueDiff = dateQuarter - valueQuarter;
                double dateDiff = 4 * (date.Year - value.Year);
                return (valueDiff + dateDiff).Round();
            }
            return 0;
        }
        /// <summary>
        /// 日期相减
        /// </summary>
        /// <param name="interval">格式化 DateInterval.Year DateInterval.Month DateInterval.Day DateInterval.Hour DateInterval.Minute DateInterval.Second DateInterval.WeekOfYear DateInterval.Quarter</param>
        /// <param name="date1">开始时间</param>
        /// <param name="date2">结束时间</param>
        /// <returns></returns>
        private static long DateDiff(this DateInterval interval, DateTime date1, DateTime date2) {
            if (interval == DateInterval.Year) return date2.Year - date1.Year;
            if (interval == DateInterval.Month) return (date2.Month - date1.Month) + (12 * (date2.Year - date1.Year));
            TimeSpan ts = date2 - date1;
            if (interval == DateInterval.Day || interval == DateInterval.DayOfYear) return ts.TotalDays.Round();
            if (interval == DateInterval.Hour) return ts.TotalHours.Round();
            if (interval == DateInterval.Minute) return ts.TotalMinutes.Round();
            if (interval == DateInterval.Second) return ts.TotalSeconds.Round();
            if (interval == DateInterval.Weekday) return (ts.TotalDays / 7.0).Round();
            if (interval == DateInterval.WeekOfYear) {
                while (date2.DayOfWeek != DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek) date2 = date2.AddDays(-1);
                while (date1.DayOfWeek != DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek) date1 = date1.AddDays(-1);
                ts = date2 - date1;
                return (ts.TotalDays / 7.0).Round();
            }
            if (interval == DateInterval.Quarter) {
                double date1Quarter = GetQuarter(date1.Month);
                double date2Quarter = GetQuarter(date2.Month);
                double date1Diff = date2Quarter - date1Quarter;
                double date2Diff = 4 * (date2.Year - date1.Year);
                return (date1Diff + date2Diff).Round();
            }
            return 0;
        }
#endif
        /// <summary>
        /// ***前 如1分钟前 1小时前
        /// </summary>
        /// <param name="date">时间</param>
        /// <returns>***前 如1分钟前 1小时前</returns>
        public static string ToAgo(this DateTime date) {
            TimeSpan timeSpan = date.GetTimeSpan(DateTime.Now);
            if (timeSpan < TimeSpan.Zero) return "未来";
            if (timeSpan < _OneMinute) return "现在";
            if (timeSpan < _TwoMinutes) return "1 分钟前";
            if (timeSpan < _OneHour) return String.Format("{0} 分钟前", timeSpan.Minutes);
            if (timeSpan < _TwoHours) return "1 小时前";
            if (timeSpan < _OneDay) return String.Format("{0} 小时前", timeSpan.Hours);
            if (timeSpan < _TwoDays) return "昨天";
            if (timeSpan < _OneWeek) return String.Format("{0} 天前", timeSpan.Days);
            if (timeSpan < _TwoWeeks) return "1 周前";
            if (timeSpan < _OneMonth) return String.Format("{0} 周前", timeSpan.Days / 7);
            if (timeSpan < _TwoMonths) return "1 月前";
            if (timeSpan < _OneYear) return String.Format("{0} 月前", timeSpan.Days / 31);
            if (timeSpan < _TwoYears) return "1 年前";
            return String.Format("{0} 年前", timeSpan.Days / 365);
        }
        /// <summary>
        /// 一年多少周
        /// </summary>
        /// <param name="datetime">日期</param>
        /// <returns>一年多少周</returns>
        public static int WeekOfYear(this DateTime datetime) {
            System.Globalization.DateTimeFormatInfo dateinf = new System.Globalization.DateTimeFormatInfo();
            System.Globalization.CalendarWeekRule weekrule = dateinf.CalendarWeekRule;
            DayOfWeek firstDayOfWeek = dateinf.FirstDayOfWeek;
            return WeekOfYear(datetime, weekrule, firstDayOfWeek);
        }
        /// <summary>
        /// 一年多少周
        /// </summary>
        /// <param name="datetime">日期</param>
        /// <param name="weekrule">第一周的规则</param>
        /// <returns>一年多少周</returns>
        public static int WeekOfYear(this DateTime datetime, System.Globalization.CalendarWeekRule weekrule) {
            System.Globalization.DateTimeFormatInfo dateinf = new System.Globalization.DateTimeFormatInfo();
            DayOfWeek firstDayOfWeek = dateinf.FirstDayOfWeek;
            return WeekOfYear(datetime, weekrule, firstDayOfWeek);
        }
        /// <summary>
        /// 一年多少周
        /// </summary>
        /// <param name="datetime">日期</param>
        /// <param name="firstDayOfWeek">星期几</param>
        /// <returns>一年多少周</returns>
        public static int WeekOfYear(this DateTime datetime, DayOfWeek firstDayOfWeek) {
            System.Globalization.DateTimeFormatInfo dateinf = new System.Globalization.DateTimeFormatInfo();
            System.Globalization.CalendarWeekRule weekrule = dateinf.CalendarWeekRule;
            return WeekOfYear(datetime, weekrule, firstDayOfWeek);
        }
        /// <summary>
        /// 一年多少周
        /// </summary>
        /// <param name="datetime">日期</param>
        /// <param name="weekrule">第一周的规则</param>
        /// <param name="firstDayOfWeek">星期几</param>
        /// <returns>一年多少周</returns>
        public static int WeekOfYear(this DateTime datetime, System.Globalization.CalendarWeekRule weekrule, DayOfWeek firstDayOfWeek) {
            System.Globalization.CultureInfo ciCurr = System.Globalization.CultureInfo.CurrentCulture;
            return ciCurr.Calendar.GetWeekOfYear(datetime, weekrule, firstDayOfWeek);
        }
        /// <summary>
        /// 第几季度
        /// </summary>
        /// <param name="month">日期</param>
        /// <returns></returns>
        public static int GetQuarter(this int month) {
            if (month <= 3) return 1;
            if (month <= 6) return 2;
            if (month <= 9) return 3;
            return 4;
        }
        /// <summary>
        /// 周日否
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns>true/false</returns>
        public static bool IsWeekday(this DateTime date) {
            return !date.IsWeekend();
        }
        /// <summary>
        /// 周末否
        /// </summary>
        /// <param name="value">日期</param>
        /// <returns>true/false</returns>
        public static bool IsWeekend(this DateTime value) {
            return value.DayOfWeek == DayOfWeek.Sunday || value.DayOfWeek == DayOfWeek.Saturday;
        }
        /// <summary>
        /// 闰年否
        /// </summary>
        /// <param name="value">日期</param>
        /// <returns>true/false</returns>
        public static bool IsLeapYear(this DateTime value) {
            return System.DateTime.IsLeapYear(value.Year);
        }
        /// <summary>
        /// 一天的开始时间 如2011-1-1 0:0:0
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns>一天的开始时间 如2011-1-1 0:0:0</returns>
        public static DateTime DayBegin(this DateTime date) {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        }
        /// <summary>
        /// 一天的结束时间 如2011-1-1 23:59:59
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns>一天的结束时间 如2011-1-1 23:59:59</returns>
        public static DateTime DayEnd(this DateTime date) {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
        }
        /// <summary>
        /// SQL日期
        /// </summary>
        /// <param name="obj">时间</param>
        /// <returns>SQL日期</returns>
        public static DateTime ToSqlDate(this object obj) {
            DateTime dt = Convert.ToDateTime(obj);
            DateTime dtMin = SqlDateTime.MinValue.Value;
            if (dt < dtMin) return SqlDateTime.MinValue.Value;
            DateTime dtMax = SqlDateTime.MaxValue.Value;
            if (dt > dtMax) return SqlDateTime.MaxValue.Value;
            return dt;
        }
        /// <summary>
        /// IsOnTime 时间val与requiredTime之间的差值是否在maxToleranceInSecs范围之内。
        /// </summary>
        /// <param name="requiredTime">开始时间</param>
        /// <param name="val">结束时间</param>
        /// <param name="maxToleranceInSecs">范围</param>
        /// <returns>IsOnTime 时间val与requiredTime之间的差值是否在maxToleranceInSecs范围之内。 true/false</returns>
        public static bool IsOnTime(this DateTime requiredTime, DateTime val, int maxToleranceInSecs) {
            TimeSpan span = val - requiredTime;
            double spanMilliseconds = span.TotalMilliseconds < 0 ? (-span.TotalMilliseconds) : span.TotalMilliseconds;
            return (spanMilliseconds <= (maxToleranceInSecs * 1000));
        }
        /// <summary>
        /// IsOnTime 对于循环调用，时间val与startTime之间的差值(>0)对cycleSpanInSecs求余数的结果是否在maxToleranceInSecs范围之内。
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="val">结束时间</param>
        /// <param name="cycleSpanInSecs">对cycleSpanInSecs求余数</param>
        /// <param name="maxToleranceInSecs">范围之内</param>
        /// <returns>IsOnTime 对于循环调用，时间val与startTime之间的差值(>0)对cycleSpanInSecs求余数的结果是否在maxToleranceInSecs范围之内。 true/false</returns>
        public static bool IsOnTime(this DateTime startTime, DateTime val, int cycleSpanInSecs, int maxToleranceInSecs) {
            TimeSpan span = val - startTime;
            double spanMilliseconds = span.TotalMilliseconds;
            double residual = spanMilliseconds % (cycleSpanInSecs * 1000);
            return (residual <= (maxToleranceInSecs * 1000));
        }
        /// <summary>
        /// RFC822
        /// </summary>
        /// <param name="date">时间</param>
        /// <returns>RFC822时间字符串</returns>
        public static string ToRFC822Time(this DateTime date) {
            int offset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours;
            string timeZone = "+" + offset.ToString().PadLeft(2, '0');
            if (offset < 0) {
                int i = offset * -1;
                timeZone = "-" + i.ToString().PadLeft(2, '0');
            }
            return date.ToString("ddd, dd MMM yyyy HH:mm:ss " + timeZone.PadRight(5, '0'), System.Globalization.CultureInfo.GetCultureInfo("en-US"));
        }
        /// <summary>
        /// 日期转大写
        /// </summary>
        /// <param name="time">当前时间</param>
        /// <returns>日期转大写</returns>
        public static string ToUpper(this DateTime time) {
            string number = "〇一二三四五六七八九";
            System.Text.StringBuilder date = new System.Text.StringBuilder();
            string[] infos = new string[] { time.Year.ToString(), time.Month.ToString("00"), time.Day.ToString("00") };
            int value;
            for (int i = 0; i < infos[0].Length; i++) {
                value = int.Parse(infos[0].Substring(i, 1));
                date.Append(number.Substring(value, 1));
            }
            date.Append("年");

            for (int i = 0; i < infos[1].Length; i++) {
                value = int.Parse(infos[1].Substring(i, 1));
                if (i == 0) {
                    if (value > 0) date.Append("十");
                } else {
                    if (value > 0) date.Append(number.Substring(value, 1));
                }
            }
            date.Append("月");
            for (int i = 0; i < infos[2].Length; i++) {
                value = int.Parse(infos[2].Substring(i, 1));
                if (i == 0) {
                    if (value > 0) {
                        if (value > 1) date.Append(number.Substring(value, 1));
                        date.Append("十");
                    }
                } else {
                    if (value > 0) date.Append(number.Substring(value, 1));
                }
            }
            date.Append("日");
            return date.ToString();
        }
        /// <summary>
        /// ToJavascriptDate
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static double ToJavascriptDate(this DateTime dt) {
            DateTime d1 = new DateTime(1970, 1, 1);
            DateTime d2 = dt.ToUniversalTime();
            TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);
            return ts.TotalMilliseconds;
        }
        /// <summary>
        /// 下个月
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime NextMonth(this DateTime dt) {
            return dt.AddMonths(1);
        }
        /// <summary>
        /// 上个月
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime PrevMonth(this DateTime dt) {
            return dt.AddMonths(-1);
        }
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        public static int ToUnixTimestamp(DateTime dateTime) {
            TimeSpan diff = dateTime.ToUniversalTime() - _epoch;
            return Convert.ToInt32(Math.Floor(diff.TotalSeconds));
        }
        public static DateTime FromUnixTimestamp(int timestamp) {
            return (_epoch + TimeSpan.FromSeconds(timestamp)).ToLocalTime();
        }
    }
}
