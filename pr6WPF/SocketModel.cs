using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace pr6WPF
{
    public class SocketModel
    {
        public Socket soket { get; set; }
        public string name { get; set; }
        
       
        public SocketModel(Socket soket,string name)
        {
            this.soket = soket;
            this.name = name;           
        }

       
    }
}
