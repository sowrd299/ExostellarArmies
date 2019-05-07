﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
//using System.Diagnostics;
using SFB.Game.Management;
using SFB.Game.Content;
using SFB.Game;
using SFB.Net;
using UnityEngine;

namespace SFB.Net.Client
{
	public class Client : MessageHandler
	{
		private static Client _instance = null;
		public static Client instance => _instance ?? (_instance = new Client());

		public bool initialized => gameManager != null;

		private Driver driver;

		public ClientPhase phase { get; private set; }

		public GameManager gameManager { get; private set; }

		public int sideIndex { get; private set; }

		private SocketManager socketManager;
		private CardLoader cardLoader;

		private Client()
		{
			phase = ClientPhase.INIT;
		}

		public static void SetDriver(Driver d)
		{
			Debug.Log("Driver for Client set.");
			instance.driver = d;
		}

		private void ProcessDeltas(XmlDocument doc, CardLoader loader, bool verbose = false)
		{
			foreach (XmlElement element in doc.GetElementsByTagName("delta"))
			{
				if (verbose)
				{
					Debug.Log($"Processing delta: '{element.OuterXml}'");
				}
				Delta d = Delta.FromXml(element, loader);
				d.Apply();
			}
		}

		public void SendPlanningPhaseActions(PlayerAction[] actions)
		{
			Debug.Log("Sending " + actions.Length + " PlayerActions");
			XmlDocument doc = NewEmptyMessage("gameAction");
			foreach (PlayerAction a in actions)
			{
				XmlElement e = a.ToXml(doc);
				doc.DocumentElement.AppendChild(e);
			}
			Debug.Log("Sending PlayerActions: " + doc.OuterXml);
			socketManager.SendXml(doc);
			socketManager.Send("<file type='lockInTurn'></file>");

			phase = ClientPhase.WAIT_TURN_START;
			Debug.Log("Waiting for turn start...");
		}

		private void Connect()
		{
			Debug.Log("Connecting to Server...");

			// Consts
			string HostName = Resources.Load<TextAsset>("hostaddr").text.Trim();
			const int Port = 4011;

			//setup the connection
			Socket socket = new Socket(AddressFamily.InterNetwork,
					SocketType.Stream,
					ProtocolType.Tcp);

			try {
				socket.Connect(HostName, Port);
				Debug.Log("Connected!");
				socketManager = new SocketManager(socket, "</file>");

				//setup game objects
				cardLoader = new CardLoader();

				// wait for match to be made
				phase = ClientPhase.WAIT_MATCH_START;
				socketManager.Send("<file type='joinMatch'><deck id='carthStarter'/></file>");
				Debug.Log("Sent joinMatch request...");
				Debug.Log("Waiting for match to be made...");
			} catch(SocketException e) {
				Debug.Log("Socket Connection Failed.");
			}
		}

		private void InitializeMatch(XmlDocument document)
		{
			// init the gamestate accordingly
			Debug.Log("Initializing gameManager...");

			Debug.Log(document.OuterXml);
			sideIndex = int.Parse(document.DocumentElement.Attributes["sideIndex"].Value);
			Debug.Log("Side Index: " + sideIndex);

			IEnumerable<XmlElement> playerIdNodes = document.GetElementsByTagName("playerIds").Cast<XmlElement>();
			XmlElement[] playerIds = new XmlElement[2];
			XmlElement localPlayerElement = playerIdNodes.First(element => element.Attributes["side"].Value == "local");
			XmlElement opponentPlayerElement = playerIdNodes.First(element => element.Attributes["side"].Value == "opponent");
			playerIds[sideIndex] = localPlayerElement;
			playerIds[1 - sideIndex] = opponentPlayerElement;

			XmlElement[] laneIds = document.GetElementsByTagName(Lane.TAG_NAME).Cast<XmlElement>().ToArray();
			
			driver.gameManager = gameManager = new GameManager(serializedPlayers: playerIds, serializedLanes: laneIds);

			driver.manager.InitializeUI();

			phase = ClientPhase.WAIT_TURN_START;
			Debug.Log("Waiting for turn start...");
		}

		private void ProcessTurnStart(XmlDocument document)
		{
			String type = document?.DocumentElement?.Attributes["type"]?.Value;
			Debug.Log("Received document type: " + type);
			if (type == "turnStart")
			{
				Debug.Log("Planning Phase Begun");
				
				// draw for opponent
				foreach (Delta d in driver.gameManager.Players[1 - sideIndex].GetDrawDeltas())
				{
					d.Apply();
					Debug.Log("Processing draw delta: " + d.GetType());
				}

				// add mana for opponent
				foreach(Delta d in driver.gameManager.Players[1 - sideIndex].ManaPool.GetAddDeltas(2)) {
					d.Apply();
					Debug.Log("Processing draw delta: " + d.GetType());
				}

				Debug.Log("Received turn start deltas; applying them:");
				driver.ProcessTurnStartDeltas(document, cardLoader);

				driver.manager.AfterDrawPhase();

				driver.updateTowerUI();
				driver.updateCardsOntable();

				phase = ClientPhase.PLANNING;
			}
			else if (type == "actionDeltas")
			{
				ProcessDeltas(document, cardLoader, true);
			}
			else
			{
				Debug.Log("Received unexpected document of type " + type + " in ClientPhase.WAIT_TURN_START");
			}
		}

		public void Update()
		{
			if (phase == ClientPhase.INIT)
			{
				Connect();
			}
			else
			{
				XmlDocument receivedDoc = socketManager.ReceiveXml();

				if (receivedDoc == null) return;

				// check for match end
				String type = receivedDoc?.DocumentElement?.Attributes["type"]?.Value;
				if(type == "matchEnd") {
					// TODO: win/lose
				}

				// depends on game phase
				switch (phase)
				{
					case ClientPhase.WAIT_MATCH_START:
						InitializeMatch(receivedDoc);
						break;
					case ClientPhase.WAIT_TURN_START:
						// wait for turnStart message
						ProcessTurnStart(receivedDoc);
						break;
					case ClientPhase.PLANNING:
						// handled by front end calling the below method:
						// Client.instance.SendPlanningPhaseActions(PlayerAction[] actions)
						ProcessDeltas(receivedDoc, cardLoader, true);
						break;
				}
			}
		}

		protected override void handleSocketDeath(SocketManager sm)
		{
			// TODO: Halp
		}

		public enum ClientPhase
		{
			INIT,
			WAIT_MATCH_START,
			WAIT_TURN_START,
			PLANNING
		}
	}
}