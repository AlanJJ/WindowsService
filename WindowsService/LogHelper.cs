using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WindowsService
{
    /// <summary>
    /// 日志类（多线程版）
    /// </summary>
    public class LogHelper
    {
        private string _fileName;
        private static Dictionary<long, long> lockDic = new Dictionary<long, long>();

        /// <summary>  
        /// 获取或设置文件名称  
        /// </summary>  
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        /// <summary>  
        /// 构造函数  
        /// </summary>
        /// <param name="fileName">文件全路径名</param>  
        public LogHelper(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new Exception("FileName不能为空！");
            }
            Create(fileName);
            _fileName = fileName;
        }

        /// <summary>  
        /// 创建文件路径
        /// </summary>  
        /// <param name="fileName">文件路径</param>  
        public void Create(string fileName)
        {
            var directoryPath = Path.GetDirectoryName(fileName);
            if (string.IsNullOrEmpty(directoryPath))
            {
                throw new Exception("FileName路径错误！");
            }
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        /// <summary>  
        /// 写入文本  
        /// </summary>  
        /// <param name="content">文本内容</param>
        /// <param name="newLine">换行标记</param>  
        private void Write(string content, string newLine)
        {
            using (FileStream fs = new FileStream(_fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 8, FileOptions.Asynchronous))
            {
                Byte[] dataArray = Encoding.UTF8.GetBytes(content + newLine);
                bool flag = true;
                long slen = dataArray.Length;
                long len = 0;
                while (flag)
                {
                    try
                    {
                        if (len >= fs.Length)
                        {
                            fs.Lock(len, slen);
                            lockDic[len] = slen;
                            flag = false;
                        }
                        else
                        {
                            len = fs.Length;
                        }
                    }
                    catch (Exception ex)
                    {
                        while (!lockDic.ContainsKey(len))
                        {
                            len += lockDic[len];
                        }
                    }
                }
                fs.Seek(len, SeekOrigin.Begin);
                fs.Write(dataArray, 0, dataArray.Length);
                fs.Close();
            }
        }

        /// <summary>  
        /// 写入文件内容  
        /// </summary>  
        /// <param name="content">内容</param>  
        public void WriteLine(string content)
        {
            this.Write(content, Environment.NewLine);
        }

        /// <summary>  
        /// 写入文件内容  不换行
        /// </summary>  
        /// <param name="content">内容</param>  
        public void Write(string content)
        {
            this.Write(content, "");
        }
    }
}