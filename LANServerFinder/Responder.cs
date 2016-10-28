using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LANServerFinder
{
	public class Responder : IDisposable
	{
		private UdpClient _reciveUdpClient;

		public string ReplyMessage { get; set; } = "I am here, Client";

		public void StartResponding(int receivePort)
		{
			StopResponding();

			_reciveUdpClient = new UdpClient(receivePort);

			StartReceivingClientRequest();
		}

		public void StopResponding()
		{
			if (_reciveUdpClient != null)
				_reciveUdpClient.Close();
		}

		public void Dispose()
		{
			StopResponding();
		}

		~Responder()
		{
			Dispose();
		}

		private void StartReceivingClientRequest()
		{
			_reciveUdpClient.BeginReceive(ReceiveCallback, null);
		}

		private void ReceiveCallback(IAsyncResult ar)
		{
			if (_reciveUdpClient.Client == null)
				return;

			var clientIpEndPoint = new IPEndPoint(0, 0);
			var messageBytes = _reciveUdpClient.EndReceive(ar, ref clientIpEndPoint);

			clientIpEndPoint.Port = BitConverter.ToInt32(messageBytes, 0);

			SendReplyToClientAsync(clientIpEndPoint);
			StartReceivingClientRequest();
		}

		private void SendReplyToClientAsync(IPEndPoint clientIpEndPoint)
		{
			var bytes = Encoding.ASCII.GetBytes(ReplyMessage);

			var sendUdpClient = new UdpClient();

			sendUdpClient.BeginSend(
				bytes,
				bytes.Length,
				clientIpEndPoint,
				ar => sendUdpClient.Close(),
				null);
		}
	}
}
