using System;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using System.Collections.Generic;
using System.Diagnostics;
using SFB.Game.Management;
using SFB.Game.Content;
using SFB.Game;

namespace SFB.TestClient{

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
            NetworkStream stream;

            // used for testing delta code
            List<Deck> decks = new List<Deck>();

            Console.WriteLine("Connecting to server at {0}:{1}...", HostName, Port);

            try{
                //actual work
                client = new TcpClient(HostName, Port);

                Console.WriteLine("Connected!");

                CardLoader cl = new CardLoader();

                stream = client.GetStream(); 

                byte[] data;

                /*
                //send a message
                Console.WriteLine("Press Enter to start a message...");
                Console.Read();
                data = System.Text.Encoding.UTF8.GetBytes("<file type='HI!!!'>");
                stream.Write(data, 0, data.Length);

                //test if will wait for EOF
                Console.WriteLine("Press Enter to finish the message...");
                Console.Read();
                data = System.Text.Encoding.UTF8.GetBytes("</file>GARBAGE");
                stream.Write(data, 0, data.Length);

                //recieve
                data = new byte[256];
                stream.Read(data, 0, data.Length);
                Console.WriteLine("Response from server: {0}",System.Text.Encoding.UTF8.GetString(data));
                // */

                //join a game
                Console.WriteLine("Press Enter to join a game...");
                Console.Read();
                data = System.Text.Encoding.UTF8.GetBytes("<file type='joinMatch'><deck id='carthStarter'/></file>");
                stream.Write(data, 0, data.Length);

                //*
                //get matchstart message
                data = new byte[256];
                stream.Read(data, 0, data.Length);
                string startResp = System.Text.Encoding.UTF8.GetString(data);
                Console.WriteLine("Response from server: {0}",startResp);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(startResp);
                foreach(XmlElement e in doc.GetElementsByTagName("playerIds")){
                    decks.Add(new Deck(e.Attributes["deck"].Value));
                }
                // */

                //send a gameaction message
                Console.WriteLine("Press Enter to make a move...");
                Console.Read();
                data = System.Text.Encoding.UTF8.GetBytes("<file type='gameAction'><action></action></file>");
                stream.Write(data, 0, data.Length);

                // get and parse XML deltas
                data = new byte[256];
                stream.Read(data, 0, data.Length);
                string resp = System.Text.Encoding.UTF8.GetString(data); 
                Console.WriteLine("Response from server: {0}", resp);
                doc = new XmlDocument();
                doc.LoadXml(resp);
                foreach(XmlElement e in doc.GetElementsByTagName("delta")){
                    Delta d = Delta.FromXml(e, cl);
                    Console.WriteLine("Delta of type {0} parsed.", d.GetType());
                }

                //get and parse A LOT of Xml, for analysis purposes
                int tests = 1000;
                long avMillis = 0;
                Stopwatch watch = new Stopwatch();
                Stopwatch totalWatch = new Stopwatch();
                totalWatch.Start();
                for(int i = 0; i < tests; i++){
                    watch.Start();

                    // send request
                    data = System.Text.Encoding.UTF8.GetBytes("<file type='gameAction'><action></action></file>");
                    stream.Write(data, 0, data.Length);

                    // get and parse XML deltas
                    data = new byte[256];
                    stream.Read(data, 0, data.Length);
                    resp = System.Text.Encoding.UTF8.GetString(data); 
                    doc = new XmlDocument();
                    doc.LoadXml(resp);
                    foreach(XmlElement e in doc.GetElementsByTagName("delta")){
                        Delta d = Delta.FromXml(e, cl);
                    }
                    watch.Stop();
                    avMillis += watch.ElapsedMilliseconds/((long)tests);
                    watch.Reset();
                }
                totalWatch.Stop();
                Console.WriteLine("{0} tests parsing Xml run; avergage time was {1} millis; total time was {2} millis.", tests, avMillis, totalWatch.ElapsedMilliseconds);

                //send a end turn message
                Console.WriteLine("Press Enter to end turn...");
                Console.Read();
                data = System.Text.Encoding.UTF8.GetBytes("<file type='lockInTurn'></file>");
                stream.Write(data, 0, data.Length);

                data = new byte[256];
                stream.Read(data, 0, data.Length);
                Console.WriteLine("Response from server: {0}",System.Text.Encoding.UTF8.GetString(data));

                //send a gameaction message
                Console.WriteLine("Press Enter to make a move...");
                Console.Read();
                data = System.Text.Encoding.UTF8.GetBytes("<file type='gameAction'><action></action></file>");
                stream.Write(data, 0, data.Length);

                data = new byte[256];
                stream.Read(data, 0, data.Length);
                Console.WriteLine("Response from server: {0}",System.Text.Encoding.UTF8.GetString(data));

                //send a gameaction message
                Console.WriteLine("Press Enter to make a move...");
                Console.Read();
                data = System.Text.Encoding.UTF8.GetBytes("<file type='gameAction'><action></action></file>");
                stream.Write(data, 0, data.Length);

                data = new byte[256];
                stream.Read(data, 0, data.Length);
                Console.WriteLine("Response from server: {0}",System.Text.Encoding.UTF8.GetString(data));

                //send a end turn message
                Console.WriteLine("Press Enter to end turn...");
                Console.Read();
                data = System.Text.Encoding.UTF8.GetBytes("<file type='lockInTurn'></file>");
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