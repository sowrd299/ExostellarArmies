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
			setPhase(ClientPhase.INIT);
		}

		public static void InitializeInstance(Driver d) {
			Instance.driver = d;
		}

		private void ProcessDeltas(XmlDocument doc, CardLoader cl, bool verbose = false) {
			foreach(XmlElement e in doc.GetElementsByTagName("delta")) {
				Delta d = Delta.FromXml(e, cl);
				Debug.Log(d.GetType());
				if(verbose) {
					Debug.Log("Processing delta: '" + e.OuterXml + "'");
				}
				d.Apply();
				Debug.Log("P1 Hand" + gameManager.Players[0].Hand.Count);
				Debug.Log("P2 Hand" + gameManager.Players[1].Hand.Count);
				Debug.Log("P1 Deck" + gameManager.Players[0].Deck.Count);
				Debug.Log("P2 Deck" + gameManager.Players[1].Deck.Count);
				Debug.Log("P2 should be unknown " + gameManager.Players[1].Deck[0].Name);
				Debug.Log("P1 Deck.ID" + gameManager.Players[0].Deck.ID);
				Debug.Log("P2 Deck.ID" + gameManager.Players[1].Deck.ID);
				driver.printField();
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


		//public static void InitializeInstance(Driver d) {
		//	if(instance == null)
		//		instance = new Client();
		//	instance.driver = d;
		//	instance.Start();
		//}

		public void Update() {
			if(phase == ClientPhase.INIT) {
				//find local IP
				IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
				IPAddress ipAddr = ipEntry.AddressList[0];

				//consts
				string HostName = "169.234.71.121";
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
			} else {
				// receive a document
				XmlDocument receivedDoc = socketManager.ReceiveXml();
				/*
				// check for match end
				if(receivedDoc != null && receivedDoc.Attributes["type"] != null && receivedDoc.Attributes["type"].Value == "matchEnd") {
					// TODO win/lose
					//break;
				}*/

				// depends on game phase
				switch(phase) {
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
							gameManager = new GameManager(playerIds: playerIds.ToArray(), laneIds: laneIds.ToArray());
							driver.gameManager = gameManager;
							Debug.Log("P1 Hand" + gameManager.Players[0].Hand.Count);
							Debug.Log("P2 Hand" + gameManager.Players[1].Hand.Count);
							Debug.Log("P1 Deck" + gameManager.Players[0].Deck.Count);
							Debug.Log("P2 Deck" + gameManager.Players[1].Deck.Count);
							driver.printField();

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
						// handled by front end calling the below method:
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
				case ClientPhase.PLANNING:
					Debug.Log("PLANNING");
					Debug.Log("P1 Hand" + gameManager.Players[0].Hand.Count);
					Debug.Log("P2 Hand" + gameManager.Players[1].Hand.Count);
					Debug.Log("P1 Deck" + gameManager.Players[0].Deck.Count);
					Debug.Log("P2 Deck" + gameManager.Players[1].Deck.Count);
					driver.printField();

					driver.resoureCount = gameManager.Players[0].Mana.Count;
					driver.manager.DrawPhase();
					foreach(Delta d in driver.gameManager.Players[1].GetDrawDeltas()) {
						d.Apply();
						Debug.Log("Processing delta: " + d.GetType());
					}
					Debug.Log("P1 Hand" + gameManager.Players[0].Hand.Count);
					Debug.Log("P2 Hand" + gameManager.Players[1].Hand.Count);
					Debug.Log("P1 Deck" + gameManager.Players[0].Deck.Count);
					Debug.Log("P2 Deck" + gameManager.Players[1].Deck.Count);
					break;
				case ClientPhase.WAIT_PLANNING_END:
					Debug.Log("Waiting for enemy to finish planning...");
					break;
			}
			phase = nPhase;
		}

		public void callFrontEndMainBtn() {
			Driver.instance.manager.mainBtn();
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