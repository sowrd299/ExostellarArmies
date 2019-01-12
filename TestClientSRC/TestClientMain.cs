using System;
using System.Net.Sockets;

namespace Client{

    //A dummy client just for testing the server
    public class TestClient{
        

        public static void Main (){

            //consts
            const string HostName = "169.234.56.254"; //TODO: make it so this doesn't need to be manually updated
            const int Port = 4011;
            const string Payload = "HELLO!!!!";
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
                Console.WriteLine("Press Enter to send a message...");
                Console.Read();
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(Payload);
                stream.Write(data, 0, data.Length);

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