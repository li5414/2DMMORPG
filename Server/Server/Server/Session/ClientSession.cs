﻿using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace Server
{
    public partial class ClientSession : PacketSession
    {
		public PlayerServerState ServerState { get; private set; } = PlayerServerState.ServerStateLogin;

		public int SessionId { get; set; }
		public Player MyPlayer { get; set; }

		object _lock = new object();
		List<ArraySegment<byte>> _reserveQueue = new List<ArraySegment<byte>>();

		long _pingpongTick = 0;
		public void Ping()
        {
            if (_pingpongTick > 0)
            {
				long delta = (System.Environment.TickCount64 - _pingpongTick);
				if(delta > 30 * 1000)
                {
                    Console.WriteLine("Disconnected by PingCheck!");
					Disconnect();
					return;
                }
            }

			S_Ping pingPacket = new S_Ping();
			Send(pingPacket);

			GameLogic.Instance.PushAfter(5000, Ping);
        }

		public void HandlePong()
        {
			_pingpongTick = System.Environment.TickCount64;
        }

        #region Network
        public void Send(IMessage packet)
		{
			string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
			MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);

			ushort size = (ushort)packet.CalculateSize();
			byte[] sendBuffer = new byte[size + 4];
			Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
			Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
			Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);

            lock (_lock)
            {
				_reserveQueue.Add(sendBuffer);
            }
		}

		public void FlushSend()
        {
			List<ArraySegment<byte>> sendList = null;

            lock (_lock)
            {
				if (_reserveQueue.Count == 0)
					return;

				sendList = _reserveQueue;
				_reserveQueue = new List<ArraySegment<byte>>();
            }

			Send(sendList);
        }

		public override void OnConnected(EndPoint endPoint)
        {
			Console.WriteLine($"OnConnected : {endPoint}");

            {
				S_Connected connectedPacket = new S_Connected();
				Send(connectedPacket);
            }

			GameLogic.Instance.PushAfter(5000, Ping);
        }

		public override void OnRecvPacket(ArraySegment<byte> buffer)
		{
			PacketManager.Instance.OnRecvPacket(this, buffer);
		}

		public override void OnSend(int numOfBytes)
		{
			//Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}

		public override void OnDisconnected(EndPoint endPoint)
        {
			GameLogic.Instance.Push(() =>
			{
				if (MyPlayer == null)
					return;

				GameRoom room = GameLogic.Instance.Find(1);
				room.Push(room.LeaveGame, MyPlayer.Id);
			});

			SessionManager.Instance.Remove(this);

			Console.WriteLine($"OnDisconnected : {endPoint}");
		}
		#endregion
	}
}
