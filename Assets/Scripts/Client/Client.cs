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
		private static Client inst = null;
		public static Client Instance {
			get {
				if(inst == null)
					inst = new Client();
				return inst;
			}
		}

		public bool DoneInitializing {
			get { return gameManager != null; }
		}

		private Driver driver;

		private ClientPhase phase;
		public ClientPhase Phase {
			get { return phase; }
		}

		private GameManager gameManager;
		public GameManager GameManager {
			get { return gameManager; }
		}

		private SocketManager socketManager;
		private CardLoader cl;

		private Client() {
			phase = ClientPhase.INIT;
		}

		public static void SetDriver(Driver d) {
			Debug.Log("Driver for Client set.");
			Instance.driver = d;
		}

		private void ProcessDeltas(XmlDocument doc, CardLoader cl, bool verbose = false) {
			foreach(XmlElement e in doc.GetElementsByTagName("delta")) {
				Delta d = Delta.FromXml(e, cl);
				if(verbose) {
					Debug.Log("    processing delta: '" + e.OuterXml + "'");
				}
				d.Apply();
			}
		}

		public void SendPlanningPhaseActions(PlayerAction[] actions) {
			Debug.Log("Sending " + actions.Length + " PlayerActions");
			XmlDocument doc = NewEmptyMessage("gameAction");
			foreach(PlayerAction a in actions) {
				XmlElement e = a.ToXml(doc);
				doc.DocumentElement.AppendChild(e);
			}
			Debug.Log("Sending PlayerActions: " + doc.OuterXml);
			socketManager.SendXml(doc);
			socketManager.Send("<file type='lockInTurn'></file>");

			phase = ClientPhase.WAIT_TURN_START;
			Debug.Log("Waiting for turn start...");
		}

		public void Update() {
			if(phase == ClientPhase.INIT) {
				Debug.Log("Connecting to Server...");

				//find local IP
				IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
				IPAddress ipAddr = ipEntry.AddressList[0];

				//consts
				string HostName = "169.234.71.114";
				const int Port = 4011;

				//setup the connection
				Socket socket = new Socket(AddressFamily.InterNetwork,
						SocketType.Stream,
						ProtocolType.Tcp);
				socket.Connect(HostName, Port);
				Debug.Log("Connected!");
				socketManager = new SocketManager(socket, "</file>");

				//setup game objects
				cl = new CardLoader();

				// wait for match to be made
				phase = ClientPhase.WAIT_MATCH_START;
				socketManager.Send("<file type='joinMatch'><deck id='carthStarter'/></file>");
				Debug.Log("Sent joinMatch request...");
				Debug.Log("Waiting for match to be made...");

			} else {
				// receive a document
				XmlDocument receivedDoc = socketManager.ReceiveXml();
				
				// check for match end
				/*if(receivedDoc != null && receivedDoc?.Attributes["type"] != null && receivedDoc?.Attributes["type"]?.Value == "matchEnd") {
					// TODO win/lose
				}*/

				// depends on game phase
				switch(phase) {
					case ClientPhase.WAIT_MATCH_START:
						if(receivedDoc != null) {
							// init the gamestate accordingly
							Debug.Log("Initializing gameManager...");
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
							gameManager = new GameManager(playerIds: playerIds.ToArray(), laneIds: laneIds.ToArray());
							driver.gameManager = gameManager;


							phase = ClientPhase.WAIT_TURN_START;
							Debug.Log("Waiting for turn start...");
						}

						break;
					case ClientPhase.WAIT_TURN_START:
						// wait for turnStart message
						if(receivedDoc != null) {
							Debug.Log("Received turn start deltas; applying them:");
							ProcessDeltas(receivedDoc, cl, true);

							driver.updateTowerUI();
							driver.updateCardsOntable();
							driver.manager.StartDrawPhase(gameManager.Players);




							phase = ClientPhase.PLANNING;
							Debug.Log("Planning Phase Begun");
							foreach(Delta d in driver.gameManager.Players[1].GetDrawDeltas()) {
								d.Apply();
								Debug.Log("Processing delta: " + d.GetType());
							}
						}
						break;
					case ClientPhase.PLANNING:
						// handled by front end calling the below method:
						// Client.instance.SendPlanningPhaseActions(PlayerAction[] actions)
						if(receivedDoc != null)
							ProcessDeltas(receivedDoc, cl, true);

						break;
				}
			}
		}
		
		protected override void handleSocketDeath(SocketManager sm) {
			// TODO
		}

		public enum ClientPhase {
			INIT, WAIT_MATCH_START,
			WAIT_TURN_START, PLANNING
		}
	}
}