using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ActiveUp.Net.Mail;
using System.Net.Mail;
using System.IO;




namespace EmailSimple
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {


        }

        public void AddLogEntry(string entry)
        {
            DateTime d = DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.Append(d.Hour.ToString().PadLeft(2, '0'));
            sb.Append(":");
            sb.Append(d.Minute.ToString().PadLeft(2, '0'));
            sb.Append(":");
            sb.Append(d.Second.ToString().PadLeft(2, '0'));
            sb.Append(".");
            sb.Append(d.Millisecond.ToString().PadLeft(3, '0'));
            sb.Append(" | ");
            sb.Append(entry);
            this.listBox1.Items.Add(sb.ToString());
        }

        private void btnReceive_Click(object sender, EventArgs e)
        {
            this.listBox1.Items.Clear();
            // We instantiate the pop3 client.
            Pop3Client pop3Client = new Pop3Client();

            string server = this.txtPopServer.Text;
            string username = txtEmail.Text;
            string password = txtPassword.Text;
            int popPort = Convert.ToInt32(txtPopPort.Text);

            try
            {

                // pop3Client.APOPConnect(server, popPort, username, password);

                pop3Client.Connect(server, popPort, username, password);
                //   pop3Client.Authenticate(username, password, SaslMechanism.CramMd5);


                int count = pop3Client.MessageCount;


                this.AddLogEntry(string.Format("共收到{0}封 ：", count));

                MessageCollection mc = new MessageCollection();

                //ActiveUp.Net.Mail.Message newMessage = pop.RetrieveMessageObject(count);
                //mc.Add(newMessage);
                // this.AddLogEntry(string.Format("Message ({0}) : {1}", count.ToString(), newMessage.Subject));

                //for (int n = count; n >= 1; n--)
                //{
                //    var msg = pop.RetrieveMessageObject(n);
                //    this.AddLogEntry(string.Format("{0}-({1}) : {2}", n.ToString(), msg.Date.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"), msg.Subject));
                //}


                string str = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                DateTime d;
                DateTime.TryParse(str, out d);


                var date = DateTime.Now.AddDays(-6);


                while (count > 0)
                {
                    var msg = pop3Client.RetrieveMessageObject(count);

                    if (msg.Date.ToLocalTime() < date)
                        break;
                    var subject = msg.Subject;

                    if (subject.Contains("问题回复-【") && subject.Contains("【") && subject.Contains("】"))
                    {
                        var index = subject.IndexOf("【") + 1;
                        var issueNo = subject.Substring(index, subject.IndexOf("】") - index);
                    }

                    this.AddLogEntry(string.Format("{0}-({1}) : {2}", count.ToString(), msg.Date.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"), msg.Subject));
                    count--;
                }

            }
            catch (Pop3Exception pexp)
            {
                this.AddLogEntry(string.Format("Pop3 Error: {0}", pexp.Message));
            }
            catch (Exception ex)
            {
                this.AddLogEntry(string.Format("Failed: {0}", ex.Message));
            }

            finally
            {
                if (pop3Client.IsConnected)
                {
                    pop3Client.Disconnect();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sendmail();
        }

        private void AuSendmail()
        {

        }

        private void sendmail()
        {
            string smtpServer = this.txtSmtpServer.Text;
            string emailaddress = txtEmail.Text;
            string password = txtPassword.Text;
            int smtpPort = Convert.ToInt32(txtSmtpPort.Text);
            string to = txtTo.Text;

            var email = new MailMessage();
            email.SubjectEncoding = Encoding.UTF8; //邮件编码 
            email.From = new MailAddress(emailaddress, "Hello");//你的邮

            foreach (var item in to.Trim(';').Split(';'))
            {
                email.To.Add(new MailAddress(item));
            }

            email.Subject = txtSubject.Text;
            //   message.CC.Add(new MailAddress("3400415@qq.com")); //抄送
            email.IsBodyHtml = true; //正文是否是HTML
            email.BodyEncoding = System.Text.Encoding.UTF8;
            email.Body = "b";// "问题提出者:【苹果电脑公司】【USER】问题窗口：【小萍】\n【测试新问题】\n问题链接：【http://st002:1005/Issue/Detail/12?searchType=3】"; //txtBody.Text; //正文
            //FileStream fs = File.OpenRead(@"z:\FCSS\Upload\90979535-9877-4106-926a-e63458528be8.jpg");

            //message.Attachments.Add(new System.Net.Mail.Attachment(fs, "aaa.txt")); //附件
            email.Priority = System.Net.Mail.MailPriority.High; //优先级
            try
            {
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(smtpServer, smtpPort); // 587;//Gmail使用的端口
                client.Credentials = new System.Net.NetworkCredential(emailaddress, password); //这里是申请的邮箱和密码
                client.EnableSsl = false;//必须经过ssl加密
                client.Send(email);
                MessageBox.Show("邮件已经成功发送到" + email.To.ToString());
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            txtPopServer.Text = "pop.qq.com";
            txtSmtpServer.Text = "smtp.qq.com";
            txtEmail.Text = "you-may@qq.com";
            txtPassword.Text = "youmay.cn";
        }


        //private void sendmail()
        //{
        //    string smtpServer = this.txtSmtpServer.Text;
        //    string email = txtEmail.Text;
        //    string password = txtPassword.Text;
        //    int smtpPort = Convert.ToInt32(txtSmtpPort.Text);
        //    string to = txtTo.Text;

        //    MailMessage message = new MailMessage();
        //    message.SubjectEncoding = Encoding.UTF8; //邮件编码 
        //    var from = new MailAddress(email, "Hello");//你的邮
        //    message.From = from;

        //    foreach (var item in to.Trim(';').Split(';'))
        //    {
        //        message.To.Add(new MailAddress(item));
        //    }

        //    message.Subject = txtSubject.Text;
        //    //   message.CC.Add(new MailAddress("3400415@qq.com")); //抄送
        //    message.IsBodyHtml = true; //正文是否是HTML
        //    message.BodyEncoding = System.Text.Encoding.UTF8;
        //    message.Body = txtBody.Text; //正文
        //    //message.Attachments.Add(new Attachment(@"c:\1.txt")); //附件
        //    message.Priority = System.Net.Mail.MailPriority.High; //优先级
        //    System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(smtpServer, smtpPort); // 587;//Gmail使用的端口
        //    client.Credentials = new System.Net.NetworkCredential(email, password); //这里是申请的邮箱和密码
        //    client.EnableSsl = true;//必须经过ssl加密

        //    try
        //    {
        //        client.Send(message);
        //        MessageBox.Show("邮件已经成功发送到" + message.To.ToString());
        //    }
        //    catch (Exception ee)
        //    {
        //        MessageBox.Show(ee.Message);
        //    }
        //}
    }
}
