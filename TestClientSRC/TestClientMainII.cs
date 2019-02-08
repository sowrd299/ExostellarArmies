using System;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using System.Collections.Generic;
using System.Diagnostics;
using SFB.Game.Management;
using SFB.Game.Content;
using SFB.Game;
using SFB.Net;

namespace SFB.TestClient{

    //A dummy client just for testing the server
    public class TestClient{
        

        private static void ProcessDeltas(XmlDocument doc, CardLoader cl, bool verbose = false){
            foreach(XmlElement e in doc.GetElementsByTagName("delta")){
                Delta d = Delta.FromXml(e, cl);
                if(verbose){
                    Console.WriteLine("Processing delta: '{0}'", e.OuterXml);
                }
                d.Apply();
            }
        }

        public static void Main (){

            // preliminary tests
            Console.WriteLine("Running preliminary tests...");

            // testing unkown card equality
            Card unknownCard = new UnknownCard();
            Card card = new UnitCard(0, "Test Card", Faction.NONE, "Some text", "Some more text", 0, 0, 0);
            Console.WriteLine("Testing UnknownCard == Card: {0}", unknownCard == card ? "Success" : "Fail");

            //find local IP
            IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipEntry.AddressList[0];
            //consts
            string HostName = ipAddr.ToString(); //by default, is using same local IP addr as server; assuming above process is deterministic
            const int Port = 4011;

            //setup the connection
            Socket socket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);
            socket.Connect(HostName, Port);
            SocketManager socketManager = new SocketManager(socket, "</file>");

            //setup game objects
            CardLoader cl = new CardLoader();
            GameManager gm;
            Player localPlayer;

            //join a game and do all the setup
            Console.WriteLine("Press Enter to join a game...");
            Console.ReadLine();
            socketManager.Send("<file type='joinMatch'><deck id='testing'/></file>");

            // get the matchStart message
            Console.WriteLine("Waiting for match start...");
            XmlDocument matchStartDoc;
            do{
                matchStartDoc = socketManager.ReceiveXml();
            }while(matchStartDoc == null);

            // init the gamestate accordingly
            Console.WriteLine("Initializing game state...");
            int localPlayerIndex = 0;
            List<XmlElement> playerIds = new List<XmlElement>();
            foreach(XmlElement e in matchStartDoc.GetElementsByTagName("playerIds")){
                if(e.Attributes["side"].Value == "local"){
                    localPlayerIndex = playerIds.Count;
                }
                playerIds.Add(e);
            }
            gm = new GameManager(ids: playerIds.ToArray());
            localPlayer = gm.Players[localPlayerIndex];

            // get the turnStart message
            Console.WriteLine("Waiting for turn start...");
            XmlDocument turnStartDoc;
            do{
                turnStartDoc = socketManager.ReceiveXml();
            }while(turnStartDoc == null);
            // get the turnStart message
            Console.WriteLine("Applying turn start deltas...");
            ProcessDeltas(turnStartDoc, cl, true);

            // print the gamestate
            foreach(Player p in gm.Players){
                Console.WriteLine("\n{0} player has hand: {1}\n...deck: {2}\n", p == localPlayer? "Local" : "Enemy", p.Hand, p.Deck);
            }
            
            // end the test
            Console.WriteLine("Press Enter to disconnect...");
            Console.ReadLine();

        }

    }

}