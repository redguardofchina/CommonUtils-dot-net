using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;

namespace CommonUtils
{
    /// <summary>
    /// 进程相关
    /// </summary>
    public static class ThreadUtil
    {
        /// <summary>
        /// 新建时间片，立即执行，自动开启
        /// </summary>
        public static System.Threading.Timer TimerOnce(Action action, int second)
        {
            var timer = new System.Threading.Timer(delegate
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("error form TimerOnce:");
                    Console.WriteLine(ex);
                }
            }, null, 0, second * 1000);
            return timer;
        }

        /// <summary>
        /// 新建时间片，延时执行，自动开启
        /// </summary>
        public static System.Timers.Timer TimerDelay(Action action, int second)
        {
            var timer = new System.Timers.Timer(second * 1000);
            timer.Elapsed += delegate
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("error form TimerDelay:");
                    Console.WriteLine(ex);
                }
            };
            timer.Start();
            return timer;
        }

        /// <summary>
        /// 新建线程
        /// </summary>
        public static Thread New(ThreadStart action)
        {
            return new Thread(action);
        }

        /// <summary>
        /// 线程执行
        /// </summary>
        public static void Start(ThreadStart action)
        {
            new Thread(action).Start();
        }

        /// <summary>
        /// 判断线程是否完成
        /// </summary>
        public static bool Alive(this List<Thread> threads)
        {
            foreach (var thread in threads)
                if (thread.IsAlive)
                    return true;
            return false;
        }

        /// <summary>
        /// 开启线程
        public static void Start(this List<Thread> threads)
        {
            foreach (var thread in threads)
                thread.Start();
        }

        public static void Sleep(double seconds)
        => Thread.Sleep((int)(seconds * 1000));

        public static void Wait(double seconds)
        => Sleep(seconds);

        #region 循环
        public static System.Threading.Timer LoopOnce(Action action, int second)
        => TimerOnce(action, second);

        public static System.Timers.Timer LoopDelay(Action action, int second)
        => TimerDelay(action, second);
        #endregion

        #region 计划

        /// <summary>
        /// 为线程循环定义循环体
        /// </summary>
        private static System.Timers.Timer mPlanTimer = null;

        /// <summary>
        /// 为线程循环定义委托
        /// </summary>
        private static Action mPlanAction;

        /// <summary>
        /// 为线程循环定义方法
        /// </summary>
        private static void PlanElapsed(object sender, ElapsedEventArgs e)
        {
            if (mPlanTimer != null)
            {
                mPlanTimer.Stop();
                mPlanTimer.Close();
                mPlanTimer.Dispose();
            }
            mPlanAction?.Invoke();
            mPlanAction = null;
        }

        /// <summary>
        /// 计划执行
        /// </summary>
        public static void Plan(Action action, double second)
        {
            mPlanAction = action;
            mPlanTimer = new System.Timers.Timer();
            mPlanTimer.Interval = second * 1000;
            mPlanTimer.Elapsed += PlanElapsed;
            mPlanTimer.Start();
        }

        /// <summary>
        /// 取消执行
        /// </summary>
        public static void CancelPlan()
        {
            if (mPlanTimer != null)
            {
                mPlanTimer.Stop();
                mPlanTimer.Close();
                mPlanTimer.Dispose();
            }
            mPlanAction = null;
        }

        #endregion
    }
}
