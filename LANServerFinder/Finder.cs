using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LANServerFinder
{
	public delegate void ServerFound(string ip, string message);

	public class Finder : IDisposable
	{
		private UdpClient _sendUdpClient;
		private UdpClient _reciveUdpClient;
		private IPEndPoint _broadcastIpEndPoint;

		private ServerFound _onServerFound;

		private bool _finding;

		public int SendPeriod { get; set; } = 1000;

		public void StartFindingServer(int sendPort, int receivePort, ServerFound onServerFound)
		{
			StopFindingServer();

			_finding = true;

			_onServerFound = onServerFound;

			_reciveUdpClient = new UdpClient(receivePort);

			_sendUdpClient = new UdpClient();
			_sendUdpClient.EnableBroadcast = true;

			_broadcastIpEndPoint = new IPEndPoint(IPAddress.Broadcast, sendPort);

			StartReceivingServerReply();
			StartSendingServerRequest();
		}

		public void StopFindingServer()
		{
			Dispose();
		}

		public void Dispose()
		{
			_finding = false;

			if (_sendUdpClient != null)
				_sendUdpClient.Close();

			if (_reciveUdpClient != null)
				_reciveUdpClient.Close();
		}

		private void StartReceivingServerReply()
		{
			_reciveUdpClient.BeginReceive(ReceiveCallback, null);
		}

		private void StartSendingServerRequest()
		{
			if (_reciveUdpClient.Client == null)
				return;

			int receivePort = ((IPEndPoint) _reciveUdpClient.Client.LocalEndPoint).Port;
			var bytes = BitConverter.GetBytes(receivePort);
			_sendUdpClient.BeginSend(bytes, bytes.Length, _broadcastIpEndPoint, SendCallback, null);
		}

		private void ReceiveCallback(IAsyncResult ar)
		{
			if (_reciveUdpClient.Client == null)
				return;

			var serverIpEndPoint = new IPEndPoint(0, 0);
			var messageBytes = _reciveUdpClient.EndReceive(ar, ref serverIpEndPoint);

			if (_onServerFound != null)
				_onServerFound(serverIpEndPoint.Address.ToString(), Encoding.ASCII.GetString(messageBytes));

			_finding = false;
			_reciveUdpClient.Close();
		}

		private void SendCallback(IAsyncResult ar)
		{
			if (_sendUdpClient.Client == null)
				return;

			_sendUdpClient.EndSend(ar);

			if (_finding)
			{
				Thread.Sleep(SendPeriod);
				StartSendingServerRequest();
			}
			else
				_sendUdpClient.Close();
		}
	}
}
