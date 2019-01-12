using System;
using System.Net.Sockets;

namespace Client{

    //A dummy client just for testing the server
    public class TestClient{
        

        public static void Main (){

            //consts
            const string HostName = "169.234.56.254";
            const int Port = 4011;
            const string Payload = "HELLO!!!!";
            //objs
            TcpClient client;
            NetworkStream stream;

            //Byte[] data = System.Text.Encoding.ASCII.GetBytes();
            Console.WriteLine("Connecting to server...");

            try{
                //actual work
                client = new TcpClient(HostName, Port);

                Console.WriteLine("Connected!");

                stream = client.GetStream(); 

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