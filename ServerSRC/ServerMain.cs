using System;

namespace Server{
 
    public class ServerMain 
    {

        static public void Main ()
        {
            Console.WriteLine ("Statring Server...");
            //this DOES start the server durring construction
            GameServer gs = new GameServer();
            gs.StartAsynchAccept();
            //the Main Loop
            while(true){
                //gs.Update(); 
                //handle new messages
                gs.SyncReceive();
                //start new games/matches
                gs.MakeMatch();
            }
        }
    }

}