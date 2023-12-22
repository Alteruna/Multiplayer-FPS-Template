using Alteruna;
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

		private void CallInitHost(Multiplayer arg0, Endpoint arg1)
		{
			arg0.OnConnected.RemoveListener(CallInitHost);
			InitHostConnect.Invoke();
		}

		private void OnJoin(Multiplayer arg0, Room arg1, User arg2)
		{
			arg0.OnRoomJoined.RemoveListener(OnJoin);
            //ScoreBoard.Instance.GetOrAddRow(arg2);
            //ScoreBoard.Instance.AddRow(arg2.Index, arg2.Name);
        }
	}
}