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
            string HostName = ipAddr.ToString(); //by default, is using same local IP addr as server; assuming above process is deterministic
            const int Port = 4011;
            //objs
            TcpClient client;

            Console.WriteLine("Connecting to server at {0}:{1}...", HostName, Port);

            // infinitely connects and disconnects
            // used to ensure server is propperly releasing resources
            while(true){
                try{
                    //actual work
                    client = new TcpClient(HostName, Port);
                    Console.WriteLine("Connected!");
                    client.Close();

                }catch(SocketException e){
                    Console.WriteLine("SocketEsception: {0}", e);
                }
            }
        }

    }

}