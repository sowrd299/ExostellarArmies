using System;
using System.Net;
using System.Net.Sockets;

namespace Client{

    //A dummy client just for testing the server
    public class TestClient{
        

        public static void Main (){

            //find local IP
            IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipEntry.AddressList[0];
            //consts
            string HostName = ipAddr.ToString();
            const int Port = 4011;
            //objs
            TcpClient client;
            NetworkStream stream;

            Console.WriteLine("Connecting to server...");

            try{
                //actual work
                client = new TcpClient(HostName, Port);

                Console.WriteLine("Connected!");

                stream = client.GetStream(); 

                //send a message
                Console.WriteLine("Press Enter to start a message...");
                Console.Read();
                Byte[] data = System.Text.Encoding.UTF8.GetBytes("<file type='HI!!!'>");
                stream.Write(data, 0, data.Length);

                //test if will wait for EOF
                Console.WriteLine("Press Enter to finish the message...");
                Console.Read();
                data = System.Text.Encoding.ASCII.GetBytes("</file>GARBAGE");
                stream.Write(data, 0, data.Length);

                //recieve
                data = new byte[256];
                stream.Read(data, 0, data.Length);
                Console.WriteLine("Response from server: {0}",System.Text.Encoding.UTF8.GetString(data));

                //wait for user then close
                Console.WriteLine("Presse Enter to disconnect...");
                Console.Read();
                stream.Close();
                client.Close();
            }catch(SocketException e){
                Console.WriteLine("SocketEsception: {0}", e);
            }
        }

    }

}