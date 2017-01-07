using System;
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
        RSA rsa;
        Dictionary<string, int> klucze;
        
        int klucz_publiczny_e=0;
        int klucz_publiczny_N = 0;
        public komunikator()
        {
            InitializeComponent();
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            textLocalIp.Text = GetLocalIP();
            
            textFrendsIp.Text = GetLocalIP();
            rsa = new RSA();
            klucze = rsa.generowanie_klucza();
            
        }
       
        

        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {
                
               
                
                
                    


                    
                        byte[] receivedData = new byte[1464];
                        ASCIIEncoding eEncoding = new ASCIIEncoding();
                        
                        receivedData = (byte[])aResult.AsyncState;
                        string receivedMessage = eEncoding.GetString(receivedData);
                
                if (receivedMessage.First()!='\0')
                {

                    if (klucz_publiczny_e==0)
                    {
                        
                        string[] klucz = receivedMessage.Split(':');
                        klucz_publiczny_e =Int32.Parse(klucz[0]);
                        klucz_publiczny_N = Int32.Parse(klucz[1]);
                        if (klucz[2].Contains("s"))
                        {
                           
                            ASCIIEncoding enc = new ASCIIEncoding();
                            byte[] msg = new byte[1500];
                            msg = enc.GetBytes( klucze["e"]+ ":" + klucze["N"]+":n");
                            sck.Send(msg);
                        }
                    }
                    else
                    {

                        listMessage.Items.Add("Znajomy: "+rsa.odszyfrowanie(receivedMessage,klucze["d"],klucze["N"]));
                    }
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
                ASCIIEncoding enc = new ASCIIEncoding();
                byte[] msg = new byte[1500];
                msg = enc.GetBytes(klucze["e"] + ":" + klucze["N"] + ":s");
                sck.Send(msg);

                
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
                    byte[] msg = new byte[1500];
                    msg = rsa.szyfrowanie(klucz_publiczny_e, klucz_publiczny_N, textMessage.Text);

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
