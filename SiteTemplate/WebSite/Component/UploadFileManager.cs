using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebSite.Component
{
    /// <summary>
    /// 上传文件管理器
    /// </summary>
    public class UploadFileManager
    {
        private string _uploadFolder;
        private static volatile UploadFileManager g_instance;
        private static object instanceLock = new object();

        private UploadFileManager(string uploadFolder)
        {
            _uploadFolder = uploadFolder;
        }

        public static UploadFileManager GetInstance(string uploadFolder)
        {
            if (g_instance == null)
            {
                lock (instanceLock)
                {
                    if (g_instance == null)
                    {
                        g_instance = new UploadFileManager(uploadFolder);
                    }
                }
            }

            return g_instance;
        }

        /// <summary>
        /// 保存上传文件
        /// </summary>
        /// <param name="file">上传文件</param>
        /// <returns>文件路径</returns>
        public string AddUploadFile(HttpPostedFileBase file)
        {
            Guid fileId = Guid.NewGuid();
            string fileName = CreateUniqueNameForNewFile(file.FileName, fileId);
            string filePath = string.Format("{0}\\{1}", _uploadFolder, fileName);
            file.SaveAs(filePath);

            return filePath;
        }

        /// <summary>
        /// 清除过期的文件(不会再使用了)
        /// </summary>
        /// <param name="hour">过期时间（默认为24小时）</param>
        public void DeleteExpiredFiles(int hour = 24)
        {
            string[] filePathArray = Directory.GetFiles(_uploadFolder);
            foreach (string filePath in filePathArray)
            {
                if (File.GetLastAccessTime(filePath).AddHours(hour).CompareTo(DateTime.Now) < 0)
                {
                    File.Delete(filePath);
                }
            }
        }

        /// <summary>
        /// 根据编号，获取上传文件的路径
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public string GetFilePath(Guid fileId)
        {
            string fileIdString = fileId.ToString();
            string[] allUploadFiles = GetAllUploadedFilesName();
            string filePath = allUploadFiles.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f).EndsWith(fileIdString));
            if (string.IsNullOrEmpty(filePath))
            {
                throw new KeyNotFoundException(string.Format("没有找到 Id：{0} 的文件", fileId));
            }

            return filePath;
        }

        /// <summary>
        /// 获取所有上传文件路径的集合
        /// </summary>
        /// <returns>上传文件路径的集合</returns>
        public string[] GetAllUploadedFilesName()
        {
            return Directory.GetFiles(_uploadFolder, "*", SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// 获取所有上传文件编号的集合
        /// </summary>
        /// <returns>上传文件编号的集合</returns>
        public Guid[] GetAllUploadedFilesId()
        {
            string[] allUploadedFilesName = GetAllUploadedFilesName();
            return allUploadedFilesName.Select(f =>
            {
                string name = Path.GetFileNameWithoutExtension(f);
                return Guid.Parse(name.Substring(name.Length - 36));
            }).ToArray();
        }

        /// <summary>
        /// 为上传文件创建唯一的文件名
        /// </summary>
        /// <param name="fileName">原来的名称</param>
        /// <param name="guid">文件编号</param>
        /// <returns>新的文件名</returns>
        private string CreateUniqueNameForNewFile(string fileName, Guid guid)
        {
            string name = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);

            return string.Format("{0}_{1}{2}", name, guid, extension);
        }

    }
}