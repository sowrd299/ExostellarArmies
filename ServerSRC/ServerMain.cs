using System;

namespace Server{
 
    public class ServerMain 
    {

        static public void Main ()
        {
            Console.WriteLine ("Statring Server...");
            //this DOES start the server durring construction
            GameServer gs = new GameServer();
            //the Main Loop
            while(true){
                gs.Update();
            }
        }
    }

}