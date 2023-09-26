using System;
using System.Collections.Generic;
using System.Linq;
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

    public partial class User : Window
    {
        private Socket socket;
             
        List<char> charsToRemove = new List<char>() { '@', '/' };

        CancellationTokenSource Token = new CancellationTokenSource();
       
        public string clos;

        string UserDisconect;

        string username;
        public User(IPAddress Ip,string UserName)
        {
            InitializeComponent();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(Ip, 8888);
            username = $"Зашел пользователь {UserName}";
            UserDisconect = UserName;
            RecieveMessage();
           
            LoadMesseng(username);
        }
        private async Task RecieveMessage()
        {
            while (!Token.IsCancellationRequested)
            {
                byte[] bytes = new byte[1000];
                await socket.ReceiveAsync(bytes, SocketFlags.None);
                string messange = Encoding.UTF8.GetString(bytes);


                Regex regex = new Regex(@"/@/(\w*)");
                MatchCollection matches = regex.Matches(messange);
                if (matches.Count > 0)
                {
                    Set.Text = String.Empty;
                    string stroka = SoketExiceon.Filter(messange, charsToRemove); 
                    Set.Text = stroka;

                }
                else
                {
                   
                    ListMesseng.Items.Add($"{SoketExiceon.Time()}: {messange}");
                    
                }
            }
        }
        private async Task SandMesseng(string messeng)
        {
            
           if (messeng == "/disconnect")
           {
                byte[] butes = Encoding.UTF8.GetBytes(messeng);
                await socket.SendAsync(butes, SocketFlags.None);
                clos = "Закрыть";
                DialogResult = true;
                socket.Disconnect(false);

           }
           else
           {
                byte[] butes = Encoding.UTF8.GetBytes(messeng);
                await socket.SendAsync(butes, SocketFlags.None);
           }
            
           
        }
        private async Task LoadMesseng (string username)
        {
            byte[] but = Encoding.UTF8.GetBytes(username);      
            await socket.SendAsync(but, SocketFlags.None);
        }
        private async Task Disconect(string disconect)
        {
            byte[] but = Encoding.UTF8.GetBytes(disconect);
            await socket.SendAsync(but, SocketFlags.None);
        }

        private void ButtonMesseng_Click(object sender, RoutedEventArgs e)
        {
            SandMesseng(Messeng.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            string disconect = $"/disconnect {UserDisconect}";
            Disconect(disconect);
            clos = "Закрыть";
            DialogResult = true;
            socket.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            string disconect = $"/disconnect {UserDisconect}";
            Disconect(disconect);
            //clos = "Закрыть";
            //DialogResult = true;
            socket.Close();

        }
    }
}
