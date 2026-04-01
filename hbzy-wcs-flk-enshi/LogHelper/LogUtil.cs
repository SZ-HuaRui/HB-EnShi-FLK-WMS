using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

/// <summary>
/// Author：zpw
/// </summary>
namespace LogUtil
{
    public class LogUtil
    {
        private const long FILE_MAX_LENGTH = 2 * 1024 * 1024;

        public static bool Write(string fileName, string text)
        {
            FileStream fileStream = null;
            StreamWriter streamWriter = null;
            string targetFilePath = string.Empty;
            try
            {
                string filePath = $@"{AppDomain.CurrentDomain.BaseDirectory}\{DateTime.Now.ToString("yyyyMMdd")}\{fileName}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.txt";
                string directoryNameTemp = $@"{AppDomain.CurrentDomain.BaseDirectory}\{DateTime.Now.ToString("yyyyMMdd")}";

                if (IfChangeFile(directoryNameTemp, fileName, ref targetFilePath))
                {
                    fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.ReadWrite);
                    streamWriter = new StreamWriter(fileStream);
                    streamWriter.Write($"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}:\n{text}\n");
                }
                else
                {
                    fileStream = new FileStream(targetFilePath, FileMode.Append, FileAccess.Write);
                    streamWriter = new StreamWriter(fileStream);
                    streamWriter.Write($"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}:\n{text}\n");
                }
            }
            catch (Exception)
            {
                streamWriter?.Close();
                fileStream?.Close();
                return false;
            }
            finally
            {
                streamWriter?.Close();
                fileStream?.Close();

            }
            return true;
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="directoryName">目录名</param>
        /// <param name="fileName">文件名</param>
        /// <param name="text">写入的文本</param>
        /// <returns></returns>
        public static bool Write(string directoryName, string fileName, string text)
        {
            FileStream fileStream = null;
            StreamWriter streamWriter = null;
            string targetFilePath = string.Empty;
            try
            {
                string filePath = $@"{AppDomain.CurrentDomain.BaseDirectory}\{directoryName}\{DateTime.Now.ToString("yyyyMMdd")}\{fileName}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.txt";
                string directoryNameTemp = $@"{AppDomain.CurrentDomain.BaseDirectory}\{directoryName}\{DateTime.Now.ToString("yyyyMMdd")}";

                if (IfChangeFile(directoryNameTemp, fileName, ref targetFilePath))
                {
                    fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.ReadWrite);
                    streamWriter = new StreamWriter(fileStream);
                    streamWriter.Write($"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}:\n{text}\n");
                }
                else
                {
                    fileStream = new FileStream(targetFilePath, FileMode.Append, FileAccess.Write);
                    streamWriter = new StreamWriter(fileStream);
                    streamWriter.Write($"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}:\n{text}\n");
                }
            }
            catch (Exception)
            {
                streamWriter?.Close();
                fileStream?.Close();
                return false;
            }
            finally
            {
                streamWriter?.Close();
                fileStream?.Close();

            }
            return true;
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="firstDirectoryName">第一级目录</param>
        /// <param name="secondDirectoryName">第二级目录</param>
        /// <param name="fileName">文件名</param>
        /// <param name="text">写入的文本</param>
        /// <returns></returns>
        public static bool Write(string firstDirectoryName, string secondDirectoryName, string fileName, string text)
        {
            FileStream fileStream = null;
            StreamWriter streamWriter = null;
            string targetFilePath = string.Empty;
            try
            {
                string filePath = $@"{AppDomain.CurrentDomain.BaseDirectory}\{firstDirectoryName}\{secondDirectoryName}\{DateTime.Now.ToString("yyyyMMdd")}\{fileName}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.txt";
                string directoryNameTemp = $@"{AppDomain.CurrentDomain.BaseDirectory}\{firstDirectoryName}\{secondDirectoryName}\{DateTime.Now.ToString("yyyyMMdd")}";

                if (IfChangeFile(directoryNameTemp, fileName, ref targetFilePath))
                {
                    fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.ReadWrite);
                    streamWriter = new StreamWriter(fileStream);
                    streamWriter.Write($"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}:\n{text}\n");
                }
                else
                {
                    fileStream = new FileStream(targetFilePath, FileMode.Append, FileAccess.Write);
                    streamWriter = new StreamWriter(fileStream);
                    streamWriter.Write($"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}:\n{text}\n");

                }
            }
            catch (Exception)
            {
                streamWriter?.Close();
                fileStream?.Close();
                return false;
            }
            finally
            {
                streamWriter?.Close();
                fileStream?.Close();
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryName">目录名</param>
        /// <param name="fileName">文件名</param>
        /// <param name="returnFilePath"></param>
        /// <returns></returns>
        private static bool IfChangeFile(string directoryName, string fileName, ref string returnFilePath)
        {
            bool existFile = false;
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                FileInfo[] childFiles = directoryInfo.GetFiles();
                if (childFiles == null || childFiles.Length == 0)
                {
                    return true;
                }
                foreach (FileInfo fileInfo in childFiles.OrderBy(x => x.CreationTime))
                {
                    if ((fileInfo.CreationTime.ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"))
                        && fileInfo.Name.Contains(fileName)
                        && fileInfo.Length <= FILE_MAX_LENGTH
                        )
                    {
                        existFile = true;
                        returnFilePath = fileInfo.FullName;
                    }
                }
                return !existFile;
            }
            catch
            {
                return !existFile;
            }
        }
    }
}
