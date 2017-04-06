using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Net.Mail;
using System.Net;

namespace MailDisposeConsoleTest
{
    public static class MailDispose
    {
        //使用网易邮箱发送 , 邮箱名字
        private const string FromAddress = "15871940895@163.com" ;
        //注意：此处使用的是授权密码
        private const string MyPassword = "NEWLIFENEWSKY55";

        /// <summary>
        /// 同步发送验证邮件
        /// <para>返回 true 代表成功发送 ， false 代表发送失败</para>
        /// </summary>
        /// <param name="toAddress">用户邮件地址</param>
        /// <param name="name">用户名</param>
        /// <param name="verifyNumber">验证码</param>
        public static bool SendVerifyMail(string toAddress , string name , int verifyNumber)
        {
            if(SendMail(toAddress , HTMLMailFile.ProduceVerifyHTMLMail(name , verifyNumber) , 
                ConfigurationData.MailSender , ConfigurationData.VerifySubject) == string.Empty )
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 使用网易 163 邮箱发送邮件
        /// <para>返回 ： string.Empty 代表成功发送 ， 否则则代表失败</para>
        /// <para>Test success ! 2017.4.5</para>
        /// </summary>
        /// <param name="toAddress">发送的目的地</param>
        /// <param name="htmlMail">需要发送的html mail</param>
        /// <param name="displayName">显示的发送者的名字</param>
        /// <param name="subject">主题</param>
        private static string SendMail(string toAddress , string htmlMail ,  string displayName , string subject)
        {
            MailMessage msg = new MailMessage();
            SmtpClient client = new SmtpClient();

            //目标地址
            msg.To.Add(new MailAddress(toAddress));
            //抄送者地址
            msg.CC.Add(new MailAddress(FromAddress));
            //UTF8制度编码
            msg.From = new MailAddress(FromAddress, displayName, System.Text.Encoding.UTF8);
            //邮件标题
            msg.Subject = subject;
            //邮件标题编码  
            msg.SubjectEncoding = System.Text.Encoding.UTF8;
            msg.Body = htmlMail;
            //邮件内容编码  
            msg.BodyEncoding = System.Text.Encoding.UTF8;


            //SMTP服务器地址  
            client.Host = "smtp.163.com";
            client.UseDefaultCredentials = true;
            //发件人邮箱账号，密码  
            client.Credentials = new System.Net.NetworkCredential(FromAddress, MyPassword);
            //是HTML邮件 
            msg.IsBodyHtml = true;
            //邮件优先级 
            msg.Priority = MailPriority.High;

            try
            {
                //直接发送数据
                client.Send(msg);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }  
  

        }
    }


    /// <summary>
    /// 限制在本程序集内部，外部无法调用
    /// </summary>
    internal static class HTMLMailFile
    {
        private const string VerifyHTMLNameVariable = "THE@USER@NAME";
        private const string VerifyHTMLVerifyNumberVaribale = "VERIFY@NUMBER";

        /// <summary>
        /// 生成验证邮件字符串 , 并没有控制expection , 需要外部捕获
        /// <para>异常：</para>
        /// <para>System.IO.FileNotFoundException -- 文件已经丢失或者被非法修改，移动了位置</para>
        /// <para>System.ArgumentNullException -- 之前使用方法读取配置信息 ， 并且配置信息出现了错误</para>   
        /// <para>返回 ： 生成的校验邮件字符串</para>
        /// </summary>
        /// <param name="name">验证邮件 名字</param>
        /// <param name="verifyNumber">验证邮件 校验数字</param>
        public static string ProduceVerifyHTMLMail(string name , int verifyNumber)
        {
            StringBuilder verifyBulider = new StringBuilder();

            //参数为空 ， 读取错误
            if (ConfigurationData.VerifyHTMLMailFilePath == string.Empty)
            {
                throw new System.ArgumentNullException();
            }

            using(StreamReader reader = File.OpenText(ConfigurationData.VerifyHTMLMailFilePath))
            {
                verifyBulider.Append(reader.ReadToEnd());
            }

            verifyBulider.Replace(VerifyHTMLNameVariable, name);
            verifyBulider.Replace(VerifyHTMLVerifyNumberVaribale, verifyNumber.ToString());

            return verifyBulider.ToString();
        }
    }

    /// <summary>
    /// 用于配置的文件信息
    /// </summary>
    internal static class ConfigurationData
    {
        private static string _VerifyHTMLMailFilePath = string.Empty;
        private static string _VerifySubject = string.Empty;
        private static string _MailSender = string.Empty;


        private const string ConfigurationDataFileName = "MailLibConfiguration.xml";
        private const string VerifyFilePathNodeName = "VerifyMailPath";
        private const string RootName = "MailLibConfiguration";
        private const string VerifySubjecName = "VerifyMailSubject";
        private const string MailSenderNameNodeName = "MailSenderName";

        /// <summary>
        /// 校验邮件html文件地址 ， 相对地址
        /// <para>如果值为 string.Empty 则代表读取数据出现错误</para>
        /// </summary>
        public static string VerifyHTMLMailFilePath
        {
            get
            {
                if(_VerifyHTMLMailFilePath != string.Empty)
                {
                    return _VerifyHTMLMailFilePath;
                }
                else
                {
                    if(ReadConfigurationData() == string.Empty)
                    {
                        return _VerifyHTMLMailFilePath;
                    }
                    //读取出现错误
                    else
                    {
                        return string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// 校验邮件的主题
        /// <para>如果值为 string.Empty 则代表读取数据出现错误</para>
        /// </summary>
        public static string VerifySubject
        {
            get
            {
                if (_VerifySubject != string.Empty)
                {
                    return _VerifySubject;
                }
                else
                {
                    if (ReadConfigurationData() == string.Empty)
                    {
                        return _VerifySubject;
                    }
                    //读取出现错误
                    else
                    {
                        return string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// 校验邮件的发送者名字
        /// <para>如果值为 string.Empty 则代表读取数据出现错误</para>
        /// </summary>
        public static string MailSender
        {
            get
            {
                if (_MailSender != string.Empty)
                {
                    return _MailSender;
                }
                else
                {
                    if (ReadConfigurationData() == string.Empty)
                    {
                        return _MailSender;
                    }
                    //读取出现错误
                    else
                    {
                        return string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// 读取配置文件数据
        /// <para>返回 ： string.Empty 代表成功读取 ， 否则则返回错误信息</para>
        /// </summary>

        public static string ReadConfigurationData()
        {
            string xmlData = string.Empty;
            XmlDocument xmlDoc = new XmlDocument();

            try
            {
                //读取xml文件
                using (StreamReader reader = File.OpenText(ConfigurationDataFileName))
                {
                    xmlData = reader.ReadToEnd();
                }
            }
            //返回错误信息 , 文件丢失或者被非法修改了文件名
            catch(Exception e)
            {
                return "Mail configuration lost or modified illegally";
            }

            try
            {
                //加载xml文件
                xmlDoc.LoadXml(xmlData);
                //获得根节点的所有子节点
                XmlNodeList rootNode = xmlDoc.SelectSingleNode(RootName).ChildNodes;

                for (int counter = 0; counter < rootNode.Count; counter++)
                {
                    //寻找到了验证邮件html文件的地址
                    if (rootNode[counter].Name == VerifyFilePathNodeName)
                    {
                        _VerifyHTMLMailFilePath = rootNode[counter].InnerText;
                    }
                    //寻找到了验证邮件主题string
                    else if(rootNode[counter].Name == VerifySubjecName)
                    {
                        _VerifySubject = rootNode[counter].InnerText;
                    }
                    //寻找到了邮件发送者名字
                    else if(rootNode[counter].Name == MailSenderNameNodeName)
                    {
                        _MailSender = rootNode[counter].InnerText;
                    }
                }
            }
            //读取出现错误，配置文件已经损坏或者被非法修改了内容
            catch(Exception e)
            {
                return "Mail configuration data damaged or modified illegally";
            }
            
            //正确获得信息，返回空字符串
            if(_VerifyHTMLMailFilePath != string.Empty &&
                _VerifySubject != string.Empty &&
                _MailSender != string.Empty)
            {
                return string.Empty ;
            }
            //没有正确获得配置信息，返回失败信息
            //读取出现错误，配置文件已经损坏或者被非法修改了内容
            else
            {
                return "Mail configuration data damaged or modified illegally";
            }
        }
    }
}
