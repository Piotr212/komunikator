﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Klient
{
    public partial class komunikator : Form
    {
        Socket sck;
        EndPoint epLocal, epRemote;
        public komunikator()
        {
            InitializeComponent();
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            textLocalIp.Text = GetIP();
            textFrendsIp.Text = GetIP();
        }
        public static WebClient webclient = new WebClient();
        public static string GetIP()
        {
            string externalIP = "";
            externalIP = webclient.DownloadString("http://checkip.dyndns.org/");
            externalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"))
                                           .Matches(externalIP)[0].ToString();
            return externalIP;
        }

        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {
                int size = sck.EndReceiveFrom(aResult,ref epRemote);
                if (size>0)
                {
                    byte[] receivedData = new byte[1464];
                    receivedData = (byte[])aResult.AsyncState;
                    ASCIIEncoding eEncoding = new ASCIIEncoding();
                    string receivedMessage = eEncoding.GetString(receivedData);
                    listMessage.Items.Add("Znajomy: "+receivedMessage);
                }
                byte[] buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch (Exception exp)
            {

                MessageBox.Show(exp.ToString());
            }
        }
        private string GetLocalIP()
        {
            IPHostEntry host=Dns.GetHostEntry(Dns.GetHostName());
            foreach ( IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily==AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            return "127.0.0.1";
        }

        private void connect_Click(object sender, EventArgs e)
        {
            try
            {
                
                epLocal = new IPEndPoint(IPAddress.Parse(GetLocalIP()), Convert.ToInt32(textLocalPort.Text));
                sck.Bind(epLocal);
                epRemote = new IPEndPoint(IPAddress.Parse(textFrendsIp.Text), Convert.ToInt32(textFrendsPort.Text));
                sck.Connect(epRemote);

                byte[] buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
                connect.Text = "Połączony";
                connect.Enabled = false;
                send.Enabled = true;
                textMessage.Focus();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void send_Click(object sender, EventArgs e)
        {
            try
            {
                ASCIIEncoding enc = new ASCIIEncoding();
                byte[]  msg= new byte[1500];
                msg = enc.GetBytes(textMessage.Text);

                sck.Send(msg);
                listMessage.Items.Add("Ja: " + textMessage.Text);
                textMessage.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
        }
    }
}