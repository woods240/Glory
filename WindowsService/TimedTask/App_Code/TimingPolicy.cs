using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsService
{
    /// <summary>
    /// 定时策略
    /// </summary>
    public abstract class TimingPolicy
    {
        public DateTime BeginDate { get; protected set; }     // 开始时间
        public DateTime EndDate { get; protected set; }       // 结束时间
        public Period Period { get; protected set; }          // 何种周期
        public int[] ExecutePoints { get; protected set; }    // 周期内的执行点(例：1天内的第2小时)

        public const double MinInterval = 10;           // 最小间隔时间（开始时间距离第一次执行时间要超过这个ms数）
        protected DateTime referenceTime { get; set; }  // 参照时间点

        /// <summary>
        /// 找到最近的参照时间点
        /// </summary>
        protected void GetReferenceTime()
        {
            // 找到参照时间点（在最小间隔外，推算时间点）
            DateTime time = BeginDate.CompareTo(DateTime.Now) > 0 ? BeginDate : DateTime.Now;
            referenceTime = time.AddSeconds(MinInterval);
        }

        /// <summary>
        /// 距离下个执行点还有多少ms
        /// </summary>
        public double NextExecuteTimeMillsecondsRemain
        {
            get { return NextExecuteTime.Subtract(DateTime.Now).TotalMilliseconds; }
        }

        /// <summary>
        /// 下个执行点的时间
        /// 注意：如果不指定执行时刻的“时:分:秒”，默认以“结束时间”的时刻为准
        /// </summary>
        public abstract DateTime NextExecuteTime { get; }
    }

    /// <summary>
    /// 周期
    /// </summary>
    public enum Period
    {
        小时 = 0,
        天,
        周
    }

    /// <summary>
    /// 以“小时”为周期的定时策略
    /// </summary>
    public class TimingPolicyByHour : TimingPolicy
    {
        public TimingPolicyByHour(DateTime beginDate, DateTime endDate, int[] executePoints)
        {
            BeginDate = beginDate;
            EndDate = endDate;
            Period = Period.小时;
            ExecutePoints = executePoints;
        }

        public override DateTime NextExecuteTime
        {
            get
            {
                // 1.推算<下一次的执行时间点>
                GetReferenceTime();
                int executeSecond = EndDate.Second;
                int nextMinute = 0;

                IEnumerable<int> nextMinutes = ExecutePoints.Where(p => new DateTime(referenceTime.Year, referenceTime.Month, referenceTime.Day, referenceTime.Hour, p, executeSecond) >= referenceTime);
                if (nextMinutes.Count() > 0)
                {
                    nextMinute = nextMinutes.Min();
                }
                else
                {
                    referenceTime = referenceTime.AddHours(1);
                    nextMinute = ExecutePoints.Min();
                }
                DateTime nextTime = new DateTime(referenceTime.Year, referenceTime.Month, referenceTime.Day, referenceTime.Hour, nextMinute, executeSecond);

                // 2.检查是否超过了结束时间
                if (nextTime > EndDate)
                {
                    return DateTime.MinValue;
                }

                return nextTime;
            }
        }
    }

    /// <summary>
    /// 以“天”为周期的定时策略
    /// </summary>
    public class TimingPolicyByDay : TimingPolicy
    {
        public TimingPolicyByDay(DateTime beginDate, DateTime endDate, int[] executePoints)
        {
            BeginDate = beginDate;
            EndDate = endDate;
            Period = Period.天;
            ExecutePoints = executePoints;
        }

        public override DateTime NextExecuteTime
        {
            get
            {
                // 1.推算<下一次的执行时间点>
                GetReferenceTime();
                int executeMinute = EndDate.Minute;
                int executeSecond = EndDate.Second;
                int nextHour = 0;

                IEnumerable<int> nextHours = ExecutePoints.Where(p => new DateTime(referenceTime.Year, referenceTime.Month, referenceTime.Day, p, executeMinute, executeSecond) >= referenceTime);
                if (nextHours.Count() > 0)
                {
                    nextHour = nextHours.Min();
                }
                else
                {
                    referenceTime = referenceTime.AddDays(1);
                    nextHour = ExecutePoints.Min();
                }
                DateTime nextTime = new DateTime(referenceTime.Year, referenceTime.Month, referenceTime.Day, nextHour, executeMinute, executeSecond);

                // 2.检查是否超过了结束时间
                if (nextTime.CompareTo(EndDate) > 0)
                {
                    return DateTime.MinValue;
                }

                return nextTime;
            }
        }
    }

    /// <summary>
    /// 以“周”为周期的定时策略
    /// 注意：每周从周天（0）开始
    /// </summary>
    public class TimingPolicyByWeek : TimingPolicy
    {
        public TimingPolicyByWeek(DateTime beginDate, DateTime endDate, int[] executePoints)
        {
            BeginDate = beginDate;
            EndDate = endDate;
            Period = Period.周;
            ExecutePoints = executePoints;
        }

        public override DateTime NextExecuteTime
        {
            get
            {
                // 1.推算<下一次的执行时间点>
                GetReferenceTime();
                int executeHour = EndDate.Hour;
                int executeMinute = EndDate.Minute;
                int executeSecond = EndDate.Second;
                int nextDayOfWeek = 0;

                IEnumerable<int> nextDayOfWeeks = ExecutePoints.Where(p =>
                    {
                        DateTime thatDay = referenceTime.AddDays(p - (int)referenceTime.DayOfWeek);
                        return new DateTime(thatDay.Year, thatDay.Month, thatDay.Day, executeHour, executeMinute, executeSecond) >= referenceTime;
                    });

                if (nextDayOfWeeks.Count() > 0)
                {
                    nextDayOfWeek = nextDayOfWeeks.Min();
                    referenceTime = referenceTime.AddDays(nextDayOfWeek - (int)referenceTime.DayOfWeek);
                }
                else
                {
                    nextDayOfWeek = ExecutePoints.Min();
                    referenceTime = referenceTime.AddDays(7 + nextDayOfWeek - (int)referenceTime.DayOfWeek);
                }
                DateTime nextTime = new DateTime(referenceTime.Year, referenceTime.Month, referenceTime.Day, executeHour, executeMinute, executeSecond);

                // 2.检查是否超过了结束时间
                if (nextTime.CompareTo(EndDate) > 0)
                {
                    return DateTime.MinValue;
                }

                return nextTime;
            }
        }
    }
}
