using System;

namespace SFB.Net.Server{
 
    public class ServerMain 
    {

        static public void Main ()
        {
            Console.WriteLine ("Statring Server...");
            //this DOES start the server durring construction
            GameServer gs = new GameServer();
            gs.StartAsyncAccept();
            //the old synchronous Main Loop
            /*
            while(true){
                //gs.Update(); 
            }
            */
            // handle closing down the server
            Console.WriteLine("Press Enter at any time to kill the server...");
            Console.Read();
        }
    }

}