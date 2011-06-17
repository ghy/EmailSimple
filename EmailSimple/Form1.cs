using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ActiveUp.Net.Mail;




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
            this.listBox1.Items.Insert(0, sb.ToString());
        }

        private void btnReceive_Click(object sender, EventArgs e)
        {
            // We instantiate the pop3 client.
            Pop3Client pop = new Pop3Client();

            string server = txtServer.Text;
            string username = txtUserName.Text;
            string password = txtPassword.Text;

            try
            {
                this.AddLogEntry(string.Format("Connection to the pop 3 server : {0}", server));

                // Connect to the pop3 client
                pop.Connect(server, username, password);

                int count = pop.MessageCount;

                this.AddLogEntry("Retrieve message list");

                MessageCollection mc = new MessageCollection();

                ActiveUp.Net.Mail.Message newMessage = pop.RetrieveMessageObject(count);
                mc.Add(newMessage);
                this.AddLogEntry(string.Format("Message ({0}) : {1}", count.ToString(), newMessage.Subject));

                //for (int n = 1; n < pop.MessageCount + 1; n++)
                //{
                //    pop.RetrieveMessageObject(n);
                //    mc.Add(newMessage);
                //    this.AddLogEntry(string.Format("Message ({0}) : {1}", n.ToString(), newMessage.Subject));
                //}

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
                if (pop.IsConnected)
                {
                    pop.Disconnect();
                }
            }
        }
    }
}
