using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace WebSite
{
    /// <summary>
    /// 验证码
    /// </summary>
    public class AuthCode
    {
        private int _length = 4;    // 验证码长度

        /// <summary>
        /// 验证码的最大长度
        /// </summary>
        public int MaxLength
        {
            get { return 10; }
        }
        /// <summary>
        /// 验证码的最小长度
        /// </summary>
        public int MinLength
        {
            get { return 1; }
        }

        public AuthCode(int length)
        {
            _length = length;
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        public string GetRandomCode()
        {
            string[] validateNums = new string[_length];
            string validateNumberStr = "";
            //生成起始序列值
            int seekSeek = unchecked((int)DateTime.Now.Ticks);
            Random seekRand = new Random(seekSeek);
            int beginSeek = (int)seekRand.Next(0, Int32.MaxValue - _length * 10000);
            int[] seeks = new int[_length];
            for (int i = 0; i < _length; i++)
            {
                beginSeek += 10000;
                seeks[i] = beginSeek;
            }
            //抽取随机数字
            for (int i = 0; i < _length; i++)
            {
                string numStr = "0123456789ABCDEFGHJKMNPQRSTUVWXYZ" + Guid.NewGuid().ToString().Replace("-", "").ToUpper();
                int numLength = numStr.Length;
                Random rand = new Random(seeks[i]);
                int numPosition = rand.Next(0, numLength - 1);
                validateNums[i] = numStr.Substring(numPosition, 1);
            }
            //生成验证码
            for (int i = 0; i < _length; i++)
            {
                validateNumberStr += validateNums[i].ToString();
            }

            return validateNumberStr;
        }

        /// <summary>
        /// 创建验证码的图片
        /// </summary>
        /// <param name="containsPage">要输出到的page对象</param>
        /// <param name="validateNum">验证码</param>
        public byte[] CreateJpegImage(string validateCode)
        {
            Bitmap image = new Bitmap((int)Math.Ceiling(validateCode.Length * 16.0), 30);
            Graphics g = Graphics.FromImage(image);
            try
            {
                //生成随机生成器
                Random random = new Random();
                //清空图片背景色
                g.Clear(Color.White);
                //画图片的干扰线
                for (int i = 0; i < 40; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }
                Font font = new Font("Arial", 14, (FontStyle.Regular | FontStyle.Italic));
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height),
                 Color.Blue, Color.DarkRed, 1.4f, true);
                g.DrawString(validateCode, font, brush, 2, 4);
                //画图片的前景干扰点
                for (int i = 0; i < 150; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                //保存图片数据
                MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Jpeg);
                //输出图片流
                return stream.ToArray();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }
    }
}
