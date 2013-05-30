using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace WindowsService
{
    public class TaskReader
    {
        public static List<Task> GetTasksFromTaskConfig()
        {
            List<Task> tasks = new List<Task>();

            XmlDocument doc = new XmlDocument();
            string configFile = AppDomain.CurrentDomain.BaseDirectory + @"Task\TaskConfig.xml";
            doc.Load(configFile);

            XmlNodeList taskNodes = doc.SelectNodes("//Task");
            foreach (XmlNode taskNode in taskNodes)
            {
                Task task = GetTaskFromXmlNode(taskNode);
                if (task != null)
                {
                    tasks.Add(task);
                }
            }

            return tasks;
        }

        private static Task GetTaskFromXmlNode(XmlNode taskNode)
        {
            string name = string.Empty;
            string batchFileName = string.Empty;
            TimingPolicy timingPolicy = null;

            try
            {
                foreach (XmlNode propertyNode in taskNode.ChildNodes)
                {
                    switch (propertyNode.Name)
                    {
                        case "Name":
                            name = propertyNode.InnerText.Trim();
                            break;

                        case "BactchFile":
                            batchFileName = AppDomain.CurrentDomain.BaseDirectory + @"Task\" + propertyNode.InnerText.Trim();
                            break;

                        case "TimingPolicy":
                            DateTime beginDate = new DateTime();
                            DateTime endDate = new DateTime();
                            Period period = new Period();
                            int[] executePoints = new int[] { };
                            foreach (XmlNode policyNode in propertyNode.ChildNodes)
                            {
                                switch (policyNode.Name)
                                {
                                    case "BeginDate":
                                        beginDate = Convert.ToDateTime(policyNode.InnerText.Trim());
                                        break;

                                    case "EndDate":
                                        endDate = Convert.ToDateTime(policyNode.InnerText.Trim());
                                        break;

                                    case "Period":
                                        period = (Period)Enum.Parse(typeof(Period), policyNode.InnerText.Trim());
                                        break;

                                    case "ExecutePoints":
                                        executePoints = policyNode.InnerText.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToArray();
                                        break;
                                }
                            }

                            switch (period)
                            {
                                case Period.小时:
                                    timingPolicy = new TimingPolicyByHour(beginDate, endDate, executePoints);
                                    break;

                                case Period.天:
                                    timingPolicy = new TimingPolicyByDay(beginDate, endDate, executePoints);
                                    break;

                                case Period.周:
                                    timingPolicy = new TimingPolicyByWeek(beginDate, endDate, executePoints);
                                    break;
                            }

                            break;
                    }
                }
            }
            catch
            {
                return null;
            }
            if (!File.Exists(batchFileName))
            {
                return null;
            }

            return new Task(name, batchFileName, timingPolicy);
        }

    }
}
