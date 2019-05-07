using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

		public GameManager gameManager { get; private set; }

		private SocketManager socketManager;

		private Client()
		{ }

		public async Task Connect(int retryInterval = 100)
		{
			Socket socket = new Socket(
				AddressFamily.InterNetwork,
				SocketType.Stream,
				ProtocolType.Tcp
			);

			while (Application.isPlaying)
			{
				string host = Resources.Load<TextAsset>("hostaddr").text.Trim();
				int port = 4011;
				try
				{
					await Task.Run(() => socket.Connect(host, port));
					Debug.Log($"Connected to {host}:{port}");
					break;
				}
				catch (SocketException e)
				{
					Debug.LogWarning($"Failed to connect to {host}:{port}; retrying in {retryInterval / 1000f}s");
					await Task.Delay(TimeSpan.FromMilliseconds(retryInterval));
					retryInterval *= 2;
				}
				catch (Exception exception)
				{
					Debug.LogError($"Unexpected error when trying to connect to {host}:{port}\n{exception}");
					await Task.Delay(TimeSpan.FromMilliseconds(retryInterval));
					retryInterval *= 2;
				}
			}

			socketManager = new SocketManager(socket, "</file>");
		}

		public async Task JoinMatch()
		{
			await Task.Run(() => socketManager.Send("<file type='joinMatch'><deck id='carthStarter'/></file>"));
		}

		public async Task<XmlDocument> ReceiveDocument()
		{
			TaskCompletionSource<XmlDocument> receive = new TaskCompletionSource<XmlDocument>();
			socketManager.AsynchReceiveXml(
				(document, _) => receive.SetResult(document),
				(_) => Debug.LogError("TODO Implement socket death handling!"),
				1024
			);
			await receive.Task;
			Debug.Log($"Received document:\n{PrettyPrintXml(receive.Task.Result)}");
			return receive.Task.Result;
		}

		public void SendPlayerActions(PlayerAction[] actions)
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

			Debug.Log("Waiting for turn start...");
		}

		private string PrettyPrintXml(XmlDocument document)
		{
			StringBuilder xmlBuilder = new StringBuilder();
			XmlWriter writer = XmlWriter.Create(xmlBuilder, new XmlWriterSettings()
			{
				OmitXmlDeclaration = true,
				Indent = true
			});
			document.WriteContentTo(writer);
			writer.Flush();
			return xmlBuilder.ToString();
		}

		protected override void handleSocketDeath(SocketManager sm)
		{
			// TODO: Halp
		}
	}
}