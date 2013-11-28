using System.Web.Helpers;

namespace WebSite
{
    public class UploadSettingsViewModel
    {
        public UploadSettingsViewModel(string controlId, string callBackUrl, object formData)
        {
            ControlId = controlId;
            CallbackUrl = callBackUrl;
            FormData = Json.Encode(formData);

            // 默认值
            FileSizeLimit = 1.5;
            FileTypeExts = "*.*";
            FileTypeDesc = "所有文件";
            ButtonText = "选择文件";
        }

        public string ControlId { get; set; }
        public string CallbackUrl { get; set; }             // 处理上传的Url
        public string FormData { get; private set; }        // client 向 server 传递的数据

        public double FileSizeLimit { get; set; }           // 单位MB
        public string FileTypeExts { get; set; }            // 允许的文件后缀
        public string FileTypeDesc { get; set; }            // 文件类型描述

        public string ButtonText { get; set; }              // 显示文字
        public string OnUploadSuccess { get; set; }         // 上传成功后的 client callback
        public string OnSelect { get; set; }                // 选择文档后 触发事件
    }
}