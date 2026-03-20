using Alteruna.Multiplayer;
using Alteruna.Multiplayer.EventArgument;
using UnityEngine.Events;

namespace AlterunaFPS
{
	public class GameInitRead : CommunicationBridge
	{
		public UnityEvent InitHost;
		public UnityEvent InitHostConnect;
		public UnityEvent InitClient;

		private void Start()
		{
			Multiplayer.OnRoomJoined.AddListener(OnJoin);

			if (GameInitSet.Host)
			{
				Multiplayer.OnConnected.AddListener(CallInitHost);
				InitHost.Invoke();
			}
			else
			{
				InitClient.Invoke();
			}
		}

		private void CallInitHost(ConnectedEvent @event)
		{
			@event.Controller.OnConnected.RemoveListener(CallInitHost);
			InitHostConnect.Invoke();
		}

		private void OnJoin(RoomJoinedEvent @event)
		{
			@event.Controller.OnRoomJoined.RemoveListener(OnJoin);
            //ScoreBoard.Instance.GetOrAddRow(arg2);
            //ScoreBoard.Instance.AddRow(arg2.Index, arg2.Name);
        }
	}
}