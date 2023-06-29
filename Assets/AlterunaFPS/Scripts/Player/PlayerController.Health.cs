using UnityEngine;

namespace AlterunaFPS
{
	public partial class PlayerController
	{
		
		[Header("Health")]
		public float MaxHealth = 20f;
		
		private Health _health;
		private int _lastSpawnIndex;
		
		private void InitializeHealth()
		{
			_health = GetComponent<Health>();
			if (_isOwner)
			{
				_health.OnDeath.AddListener(OnDeath);
				_health.HealthPoints = MaxHealth;
			}
		}

		private void OnDeath()
		{
			if (_possesed)
			{
				CinemachineVirtualCameraInstance.Instance.gameObject.SetActive(false);
				CinemachineVirtualCameraInstance.Instance.Follow(null);
			}
			
			_health.HealthPoints = MaxHealth;

			if (_offline)
			{
				transform.position = Vector3.zero;
			}
			else
			{
				// get random spawn location
				int spawnIndex;
				do
				{
					spawnIndex = Random.Range(0, Multiplayer.AvatarSpawnLocations.Count);
				}
				while (_lastSpawnIndex == spawnIndex);

				Transform spawn = Multiplayer.AvatarSpawnLocations.Count > 0 ? 
					Multiplayer.AvatarSpawnLocations[spawnIndex] : 
					Multiplayer.AvatarSpawnLocation;
				
				_controller.enabled = false;
				transform.position = spawn.position;
				transform.rotation = spawn.rotation;
				_cinemachineTargetYaw = _bodyRotate = spawn.rotation.y;
				_controller.enabled = true;
			}
			RespawnController.Respawn(gameObject);
		}
		
	}
}