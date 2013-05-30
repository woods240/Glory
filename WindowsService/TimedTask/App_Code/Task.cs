using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using NLog;
using System.Timers;

namespace WindowsService
{
    /// <summary>
    /// 定时任务
    /// </summary>
    public class Task
    {
        public string Name { get; private set; }
        public string BatchFileName { get; private set; }
        public TimingPolicy TimingPolicy { get; private set; }

        public Task(string name, string batchFileName, TimingPolicy timingPolicy)
        {
            Name = name;
            BatchFileName = batchFileName;
            TimingPolicy = timingPolicy;

            _taskTimer.AutoReset = false;
            _taskTimer.Elapsed += new ElapsedEventHandler(ExecuteTask);
        }

        public void Start()
        {
            double interval = TimingPolicy.NextExecuteTimeMillsecondsRemain;
            if (interval > 0)
            {
                _taskTimer.Interval = interval;
                _taskTimer.Start();
            }
            else
            {
                Logger successLogger = LogManager.GetLogger("TaskEndLogger");
                successLogger.Info(string.Format("\r\n任务：<{1}> 于{0}结束 ",
                    DateTime.Now, Name));
            }
        }
        public void Stop()
        {
            _taskTimer.Stop();
        }
        public void Close()
        {
            _taskTimer.Close();
        }


        private Timer _taskTimer = new Timer();
        private void ExecuteTask(object source, ElapsedEventArgs e)
        {
            // 执行任务
            string taskFile = BatchFileName;
            if (ExecuteBatchFile(taskFile))
            {
                Logger successLogger = LogManager.GetLogger("TaskExecuteSuccessLogger");
                string logContent = string.Format("\r\n{0} 引发任务：<{1}> 成功\r\n执行文件：{2}",
                    e.SignalTime, Name, taskFile);

                successLogger.Info(logContent);
            }
            else
            {
                Logger failedLogger = LogManager.GetLogger("TaskBatchExecuteFailedLogger");
                string logContent = string.Format("\r\n{0} 引发任务：<{1}> 失败\r\n执行文件：{2}",
                    e.SignalTime, Name, taskFile);

                failedLogger.Error(logContent);
            }

            // 向下一次执行时间点进发
            Start();
        }
        private bool ExecuteBatchFile(string taskFile)
        {
            // 执行批处理文件
            using (Process process = new Process())
            {
                process.StartInfo.FileName = taskFile;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;

                return process.Start();
            }
        }

    }

}
