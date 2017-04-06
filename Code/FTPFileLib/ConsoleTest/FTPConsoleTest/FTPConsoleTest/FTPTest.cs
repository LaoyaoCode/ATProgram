using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;


namespace FTPConsoleTest
{
    public static class FTPDispose
    {
        public enum DownLoadOrUplaodState
        {
            Start , 
            Disposing ,
            Finished , 
            Error
        }
        public delegate void DownLoadOrUplaodProgress(double percentage, DownLoadOrUplaodState state);
        private const string UserName = "Laoyao";
        private const string Passwords = "NEWLIFENEWSKY@55";
        private const string IpAddress = "120.25.211.114";
        //缓冲大小20KB
        private const int BuffSize = 2048 * 10;
        /// <summary>
        /// 上传一个文件到FTP服务器上，如果保存的名字在服务器上出现了重复，之前的数据将会被删除
        /// <para>test success , 2017.4.6</para>
        /// <para>返回，string.Empty 成功发送 ， 否则则返回错误信息</para>
        /// </summary>
        /// <param name="filePath">上传的文件地址</param>
        /// <param name="saveFileName">上传到服务器的地址</param>
        /// <param name="progress">处理进度回调</param>
        public static string UploadFile(string filePath , string saveFileName , DownLoadOrUplaodProgress progress = null)
        {
            FileStream file = null;
            FtpWebRequest ftpRequest = null;
            byte[] buff = new byte[BuffSize];
            int sendSize = 0 ;
            long totalSendedSize = 0, totalSendSize = 0;
            string result = string.Empty;

            //文件打开是否成功
            try
            {
                file = File.Open(filePath, FileMode.Open);
            }
            catch(Exception e)
            {
                result =  "File open error!";
            }
            //如果打开文件失败，则就此返回，提示失败
            if(result != string.Empty)
            {
                return result;
            }

            //设置IP和问价位置
            ftpRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + IpAddress + "/" + saveFileName));
            //用户名和密码
            ftpRequest.Credentials = new NetworkCredential(UserName, Passwords);
            //执行完请求后连接关闭
            ftpRequest.KeepAlive = false;
            //上传文件
            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
            // 指定数据传输类型  ， binary   
            ftpRequest.UseBinary = true;
            //设置上传文件长度
            ftpRequest.ContentLength = file.Length;
            //不要求身份验证
            ftpRequest.AuthenticationLevel = System.Net.Security.AuthenticationLevel.None;
            //不使用ssl安全验证
            ftpRequest.EnableSsl = false;
            ftpRequest.UsePassive = true;

            try
            {
                //处理进度代理回调
                if (progress != null)
                {
                    //状态为开始
                    progress.Invoke(0.0, DownLoadOrUplaodState.Start);
                }

                //获得总共需要发送的长度
                totalSendSize = file.Length;

                // 获得上传流
                using (Stream ftpSender = ftpRequest.GetRequestStream())
                {
                    //每次读文件流的2kb    
                    sendSize = file.Read(buff, 0, BuffSize);

                    //文件内容没有结束
                    while (sendSize != 0)
                    {
                        // 把内容从file stream 写入 upload stream    
                        ftpSender.Write(buff, 0, sendSize);
                        //记录已经发送的数据长度
                        totalSendedSize += sendSize;

                        //处理进度代理回调
                        if (progress != null)
                        {
                            //发送进度
                            progress.Invoke((double)totalSendedSize / totalSendSize, DownLoadOrUplaodState.Disposing);
                        }

                        sendSize = file.Read(buff, 0, BuffSize);
                    }

                    //处理进度代理回调
                    if (progress != null)
                    {
                        //发送进度 , 状态，完成
                        progress.Invoke(1.0, DownLoadOrUplaodState.Finished);
                    }
                }
            }
            //发送失误，捕捉信息返回
            catch (Exception ex)
            {
                result = "FTP Send File Failed";
                //处理进度代理回调
                if (progress != null)
                {
                    //发送进度 , 状态，错误
                    progress.Invoke(1.0, DownLoadOrUplaodState.Error);
                }
            }    
            finally
            {
                file.Close();
            }

            return result;
        }

        /// <summary>
        /// success 
        /// </summary>
        /// <param name="saveInPCFilePath"></param>
        /// <param name="downloadFileName"></param>
        /// <returns></returns>
        public static string DownLoadFile(string saveInPCFilePath, string downloadFileName)
        {
            FileStream file = null;
            FtpWebRequest ftpRequest = null;
            byte[] buff = new byte[BuffSize];
            int recieveSize = 0;
            string result = string.Empty;

            //文件创建是否成功
            try
            {
                file = File.Open(saveInPCFilePath, FileMode.Create);
            }
            catch (Exception e)
            {
                result = "File create error!";
            }
            //如果打开文件失败，则就此返回，提示失败
            if (result != string.Empty)
            {
                return result;
            }

            //设置IP和问价位置
            ftpRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + IpAddress + "/" + downloadFileName));
            //用户名和密码
            ftpRequest.Credentials = new NetworkCredential(UserName, Passwords);
            //执行完请求后连接关闭
            ftpRequest.KeepAlive = false;
            //下载文件
            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            // 指定数据传输类型  ， binary   
            ftpRequest.UseBinary = true;
            //设置上传文件长度
            ftpRequest.ContentLength = file.Length;
            //不要求身份验证
            ftpRequest.AuthenticationLevel = System.Net.Security.AuthenticationLevel.None;
            //不使用ssl安全验证
            ftpRequest.EnableSsl = false;
            ftpRequest.UsePassive = true;

            try
            {
                // 获得上传流
                using (Stream ftpSender = ftpRequest.GetResponse().GetResponseStream())
                {
                    //每次读文件流的2kb    
                    recieveSize = ftpSender.Read(buff, 0, BuffSize);

                    //文件内容没有结束
                    while (recieveSize != 0)
                    {
                        // 把内容从download stream  写入  file stream
                        file.Write(buff, 0, recieveSize);

                        recieveSize = ftpSender.Read(buff, 0, BuffSize);
                    }
                }
            }
            //发送失误，捕捉信息返回
            catch (Exception ex)
            {
                result = "FTP Send File Failed";
            }
            finally
            {
                file.Close();
            }

            return result;
        }
    }
}
