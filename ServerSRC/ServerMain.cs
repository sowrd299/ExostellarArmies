using System;

namespace Server{
 
    public class ServerMain 
    {

        static public void Main ()
        {
            Console.WriteLine ("Statring Server...");
            GameServer gs = new GameServer();
            while(true){
                gs.Update();
            }
        }
    }

}