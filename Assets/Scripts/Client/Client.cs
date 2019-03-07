using System;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using System.Collections.Generic;
//using System.Diagnostics;
using SFB.Game.Management;
using SFB.Game.Content;
using SFB.Game;
using SFB.Net;
using UnityEngine;

namespace SFB.Net.Client {
	public class Client : MessageHandler {
		private static Client instance = null;
		public static Client Instance {
			get {
				if(instance == null)
					instance = new Client();
				return instance;
			}
		}


		private ClientPhase phase;
		public ClientPhase Phase {
			get { return phase; }
		}

		private GameManager gm;
		public GameManager GameManager {
			get { return gm; }
		}

		private SocketManager socketManager;
		private CardLoader cl;

		private Client() {

		}

		private static void ProcessDeltas(XmlDocument doc, CardLoader cl, bool verbose = false) {
			foreach(XmlElement e in doc.GetElementsByTagName("delta")) {
				Delta d = Delta.FromXml(e, cl);
				if(verbose) {
					Debug.Log("Processing delta: '" + e.OuterXml + "'");
				}
				d.Apply();
			}
		}

		public void SendPlanningPhaseActions(PlayerAction[] actions) {
			XmlDocument doc = NewEmptyMessage("gameAction");
			foreach(PlayerAction a in actions) {
				XmlElement e = a.ToXml(doc);
				doc.AppendChild(e);
			}
			socketManager.SendXml(doc);
			socketManager.Send("<file type='lockInTurn'>");
			setPhase(ClientPhase.WAIT_PLANNING_END);
		}

		public void Start() {
			setPhase(ClientPhase.INIT);

			while(true) {
				// receive a document
				XmlDocument receivedDoc = socketManager.ReceiveXml();

				// check for match end
				if(receivedDoc != null && receivedDoc.Attributes["type"] != null && receivedDoc.Attributes["type"].Value == "matchEnd") {
					// TODO win/lose
					break;
				}

				// depends on game phase
				switch(phase) {
					case ClientPhase.INIT:
						//find local IP
						IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
						IPAddress ipAddr = ipEntry.AddressList[0];

						//consts
						string HostName = "169.234.7.103";
						const int Port = 4011;

						//setup the connection
						Socket socket = new Socket(AddressFamily.InterNetwork,
								SocketType.Stream,
								ProtocolType.Tcp);
						socket.Connect(HostName, Port);
						socketManager = new SocketManager(socket, "</file>");

						//setup game objects
						cl = new CardLoader();

						setPhase(ClientPhase.WAIT_MATCH_START);
						break;
					case ClientPhase.WAIT_MATCH_START:
						if(receivedDoc != null) {
							// init the gamestate accordingly
							Debug.Log("Initializing game state...");
							int localPlayerIndex = 0;
							List<XmlElement> playerIds = new List<XmlElement>();
							foreach(XmlElement e in receivedDoc.GetElementsByTagName("playerIds")) {
								if(e.Attributes["side"].Value == "local") {
									localPlayerIndex = playerIds.Count;
								}
								playerIds.Add(e);
							}
							List<XmlElement> laneIds = new List<XmlElement>();
							foreach(XmlElement e in receivedDoc.GetElementsByTagName("laneIds")) {
								laneIds.Add(e);
							}
							gm = new GameManager(playerIds: playerIds.ToArray(), laneIds: laneIds.ToArray());

							setPhase(ClientPhase.WAIT_TURN_START);
						}

						break;
					case ClientPhase.WAIT_TURN_START:
						// wait for turnStart message
						if(receivedDoc != null) {
							Debug.Log("Applying turn start deltas...");
							ProcessDeltas(receivedDoc, cl, true);
							setPhase(ClientPhase.PLANNING);
						}
						break;
					case ClientPhase.PLANNING:
						// handled by client / front end calling the below method:
						// Client.instance.SendPlanningPhaseActions(PlayerAction[] actions)
						break;
					case ClientPhase.WAIT_PLANNING_END:
						// wait for actionDeltas message
						receivedDoc = socketManager.ReceiveXml();
						if(receivedDoc != null) {
							Debug.Log("Applying action deltas...");
							ProcessDeltas(receivedDoc, cl, true);
							setPhase(ClientPhase.WAIT_TURN_START);
						}
						break;
				}
			}
		}

		private void setPhase(ClientPhase nPhase) {
			switch(nPhase) {
				case ClientPhase.WAIT_MATCH_START:
					//join a game
					socketManager.Send("<file type='joinMatch'><deck id='testing'/></file>");
					Debug.Log("Sent joinMatch request...");
					Debug.Log("Waiting for match start...");

					break;
				case ClientPhase.WAIT_TURN_START:
					Debug.Log("Waiting for turn start...");
					break;
				case ClientPhase.WAIT_PLANNING_END:
					Debug.Log("Waiting for enemy to finish planning...");
					break;
			}
		}

		protected override void handleSocketDeath(SocketManager sm) {
			// TODO
		}

		public enum ClientPhase {
			INIT, WAIT_MATCH_START,
			WAIT_TURN_START, PLANNING, WAIT_PLANNING_END
		}
	}
}