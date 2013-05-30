using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using NLog;
using System.IO;

namespace WindowsService
{
    public partial class TimedTask : ServiceBase
    {
        public TimedTask()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            InitTaskList();
            StartTask();
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            StopTask();
            ClearTaskList();
            base.OnStop();
        }

        protected override void OnContinue()
        {
            InitTaskList();
            StartTask();
            base.OnContinue();
        }

        protected override void OnPause()
        {
            StopTask();
            ClearTaskList();
            base.OnPause();
        }

        protected override void OnShutdown()
        {
            StopTask();
            ClearTaskList();

            base.OnShutdown();
        }

        private List<Task> _taskList;

        private void InitTaskList()
        {
            _taskList = TaskReader.GetTasksFromTaskConfig();
        }
        private void ClearTaskList()
        {
            foreach (Task task in _taskList)
            {
                task.Close();
            }

            _taskList.Clear();
        }
        private void StartTask()
        {
            foreach (Task task in _taskList)
            {
                task.Start();
            }
        }
        private void StopTask()
        {
            foreach (Task task in _taskList)
            {
                task.Stop();
            }
        }
    }
}
