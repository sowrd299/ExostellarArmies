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

		private SocketManager socketManager;
		private object socketLock = new object();

		private List<XmlDocument> messageBacklog;

		private Client()
		{
			messageBacklog = new List<XmlDocument>();
		}

		public async Task<bool> Connect(
			string host, int port,
			int timeout = 100, int retryInterval = 100, int maxAttempts = -1,
			CancellationToken cancelToken = default(CancellationToken)
		)
		{
			for (int attempts = 0; Application.isPlaying && (maxAttempts < 0 || attempts < maxAttempts); attempts++)
			{
				if (cancelToken.IsCancellationRequested)
				{
					return false;
				}

				try
				{
					Socket socket = new Socket(
						AddressFamily.InterNetwork,
						SocketType.Stream,
						ProtocolType.Tcp
					);

					IAsyncResult result = socket.BeginConnect(host, port, null, null);
					bool success = result.AsyncWaitHandle.WaitOne(timeout);

					if (success)
					{
						socket.EndConnect(result);
					}
					else
					{
						socket.Close();
						throw new SocketException();
					}

					Debug.Log($"Connected to {host}:{port}");

					lock (socketLock)
					{
						cancelToken.ThrowIfCancellationRequested();
						socketManager = new SocketManager(socket, "</file>");
						return true;
					}
				}
				catch (SocketException)
				{
					Debug.LogWarning($"Failed to connect to {host}:{port}; retrying in {retryInterval / 1000f}s");
					await Task.Delay(TimeSpan.FromMilliseconds(retryInterval));
					retryInterval *= 2;
				}
				catch (TaskCanceledException)
				{
					return false;
				}
				catch (Exception exception)
				{
					Debug.LogError($"Unexpected error when trying to connect to {host}:{port}\n{exception}");
					await Task.Delay(TimeSpan.FromMilliseconds(retryInterval));
					retryInterval *= 2;
				}
			}

			return false;
		}

		public async Task JoinMatch(string deckId = "testing")
		{
			await Task.Run(() => socketManager.Send("<file type='joinMatch'><deck id='" + deckId + "'/></file>"));
		}

		public async Task<XmlDocument> ReceiveDocument(Predicate<string> shouldAcceptType)
		{
			for (int i = 0; i < messageBacklog.Count; i++)
			{
				if (shouldAcceptType(messageBacklog[i].DocumentElement.GetAttribute("type")))
				{
					XmlDocument backlogDocument = messageBacklog[i];
					messageBacklog.RemoveAt(i);
					return backlogDocument;
				}
			}

			while (true)
			{
				TaskCompletionSource<XmlDocument> receive = new TaskCompletionSource<XmlDocument>();

				socketManager.AsyncReceiveXml(
					(document, _) => receive.SetResult(document),
					(_) => Debug.LogError("TODO Implement socket death handling!"),
					1024
				);
				await receive.Task;
				Debug.Log($"Received document:\n{PrettyPrintXml(receive.Task.Result)}");

				if (shouldAcceptType(receive.Task.Result.DocumentElement.GetAttribute("type")))
				{
					return receive.Task.Result;
				}
				else
				{
					messageBacklog.Add(receive.Task.Result);
				}
			}
		}

		public void SendPlayerActions(PlayerAction[] actions)
		{
			XmlDocument doc = NewEmptyMessage("gameAction");
			foreach (PlayerAction a in actions)
			{
				XmlElement e = a.ToXml(doc);
				doc.DocumentElement.AppendChild(e);
			}
			Debug.Log($"Sending PlayerActions:\n{PrettyPrintXml(doc)}");
			socketManager.SendXml(doc);
		}

		public void SendInputRequestResponse(InputRequest[] requests)
		{
			XmlDocument doc = NewEmptyMessage("inputRequestResponse");
			foreach (InputRequest request in requests)
			{
				XmlElement responseElement = request.ToXml(doc);
				doc.DocumentElement.AppendChild(responseElement);
			}
			Debug.Log($"Sending InputResponse:\n{PrettyPrintXml(doc)}");
			socketManager.SendXml(doc);
		}

		public void LockInTurn()
		{
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

		protected override void HandleSocketDeath(SocketManager sm)
		{
			// TODO: Halp
		}
	}
}