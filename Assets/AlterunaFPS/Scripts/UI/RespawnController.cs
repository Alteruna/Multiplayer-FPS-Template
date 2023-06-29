using System;
using TMPro;
using UnityEngine;

namespace AlterunaFPS
{
	public class RespawnController : MonoBehaviour
	{
		public static RespawnController Instance { get; private set; }

		public TMP_Text countdown;
		
		public float RespawnTime = 5f;
		
		private float _respawnTime;
		
		[HideInInspector]
		public GameObject Player;

		private void Awake()
		{
			Instance = this;
			gameObject.SetActive(false);
		}

		private void OnEnable()
		{
			_respawnTime = RespawnTime;
			countdown.text = "Respawn in " + Mathf.Ceil(_respawnTime);
		}

		private void Update()
		{
			int oldRespawnTime = Mathf.CeilToInt(_respawnTime);
			_respawnTime -= Time.unscaledDeltaTime;
			int ceilTime = Mathf.CeilToInt(_respawnTime);
			if (oldRespawnTime != ceilTime)
			{
				countdown.text = "Respawn in " + ceilTime;
				if (ceilTime <= 0)
				{
					Player.SetActive(true);
					gameObject.SetActive(false);
				}
			}
		}
		
		public static void Respawn(GameObject player)
		{
			if (Instance == null) return;
			Instance.Player = player;
			Instance.gameObject.SetActive(true);
			Instance.Player.SetActive(false);
		}
	}
}