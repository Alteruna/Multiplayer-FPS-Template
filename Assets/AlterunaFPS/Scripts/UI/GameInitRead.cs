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
	}
}