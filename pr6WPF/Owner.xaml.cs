using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace pr6WPF
{
    public partial class Owner : Window
    {
        
        private Socket socket;

        private List<Socket> client = new List<Socket>();

        List<SocketModel> clientconeck = new List<SocketModel>();
       
        CancellationTokenSource Token = new CancellationTokenSource();

        int update = 0;

        public string clos;



        public Owner(string Name)
        {
            InitializeComponent(); 
           

            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 8888);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);          
            socket.Bind(ip);
            socket.Listen(1000);

            SocketModel admin = new SocketModel(socket,Name);
            clientconeck.Add(admin);
            Gues.Items.Add(admin.name);

            ConectClient();


        }
        private async Task ConectClient()
        {
            while (true)
            {
                var cliens = await socket.AcceptAsync();
                client.Add(cliens);
                MessengClient(cliens);
                 if (cliens.Connected == true) 
                 {                  
                   Client();                
                 }
                                          
            }
        }
        private async Task MessengClient(Socket cliens)
        {
            while (!Token.IsCancellationRequested)
            {
                byte[] bytes = new byte[100];
                await cliens.ReceiveAsync(bytes, SocketFlags.None);
                string messeng = Encoding.UTF8.GetString(bytes);

                Regex regex = new Regex(@"Зашел(\w*)");
                Regex rege = new Regex(@"(/disconnect\w*)");
                MatchCollection matches = regex.Matches(messeng);
                MatchCollection diconect = rege.Matches(messeng);
                if (matches.Count > 0)
                {
                      string lastWord = messeng.Split(' ').Last();
                      SocketModel nameUser = new SocketModel(cliens,lastWord);
                      clientconeck.Add(nameUser);
                      Gues.Items.Add(nameUser.name);
                      ListMesseng.Items.Add($"{SoketExiceon.Time()}: {messeng}");

                    
                }
                else
                {
                    if (diconect.Count > 0)
                    {
                       
                            foreach (var item in clientconeck)
                            {
                                if (item.soket == cliens)
                                {
                                    Gues.Items.Remove(item.name);
                                    clientconeck.Remove(item);
                                    client.Remove(cliens);
                                    foreach(var element in client)
                                    {
                                        var soketelement = SoketExiceon.IsConnected(element);
                                        if (soketelement == true)
                                        {
                                            Client();
                                        }
                                        
                                    }
                                        
                                }
                            }

                    }
                    else
                    {
                        ListMesseng.Items.Add($"{SoketExiceon.Time()}: {messeng}");
                    }
                }
             
                foreach (var items in client)
                {
                    ConvertMessengClient(items, messeng);
                   
                }
            }
        }
        private async Task ConvertMessengClient(Socket cliens, string messeng)
        {

            byte[] bytes = Encoding.UTF8.GetBytes(messeng);
            await cliens.SendAsync(bytes, SocketFlags.None);

        }
        private async Task Client()
        {
            
            var namecopy = SoketExiceon.Namechar(clientconeck);
                                               
            byte[] bytes = Encoding.UTF8.GetBytes(namecopy);
            foreach(var Klient in client)
            {
                 await Klient.SendAsync(bytes, SocketFlags.None);
            }
        
        }

        private void ButtonMesseng_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in client)
            {
                ConvertMessengClient(item, Messeng.Text);
            }
            ListMesseng.Items.Add($"{SoketExiceon.Time()} : {Messeng.Text}");
        }

        private void ChekName_Click(object sender, RoutedEventArgs e)
        {
           
            update += 1;
            if (update %2 == 0)
            { 
                Gues.Items.Clear();
                foreach(var item in clientconeck)
                {                  
                    Gues.Items.Add(item.name);
                }
            }

            else if (update %2 != 0 )
            {
                foreach (var item in clientconeck)
                {
                    if (item.soket == socket)
                    {
                        Gues.Items.Remove(item.name);
                        Gues.Items.Add($"{item.name} : Admin");
                    }
                    else
                    {
                        Gues.Items.Remove(item.name);
                        Gues.Items.Add(item.soket.RemoteEndPoint.ToString());
                    }
                }    
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in clientconeck)
            {
                if (item.soket == socket)
                { 
                   clientconeck.Remove(item);
                  
                   break;                                     
                }
            }
             Client();
             clos = "Закрыть";
             DialogResult = true;
             socket.Close();

        }
       

        private void Window_Closed(object sender, EventArgs e)
        {
            foreach (var item in clientconeck)
            {
                if (item.soket == socket)
                {
                    clientconeck.Remove(item);

                    break;
                }
            }
            Client();

            socket.Close();
        }
    }
}
//26.16.20.0
