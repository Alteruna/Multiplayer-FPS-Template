using System;
using UnityEngine;
using Alteruna;

namespace AlterunaFPS
{
	
	// Class split into multiple files for clarity

	[RequireComponent(typeof(CharacterController), typeof(InputSynchronizable), typeof(Health))]
	public partial class PlayerController : Synchronizable
	{
        private void Awake()
        {
			_controller = GetComponent<CharacterController>();
		}

        private void Start()
		{
			InitializeNetworking();
			InitializeGun();
			InitialiseAnimations();
			InitializeInput();
		}
		
		private new void OnEnable()
		{
			base.OnEnable();
			
			ResetAmmo();
			Commit();

			if (_isOwner && _possesed)
			{
				CinemachineVirtualCameraInstance.Instance.Follow(_cameraTarget);
				CinemachineVirtualCameraInstance.Instance.gameObject.SetActive(true);
			}
		}

		// called when the local player is possessed by a client
		// Called from the PlayerController.Networking
		private void OnPossession()
		{
			InitializeCamera();
			InitializeHealth();
		}

		private void OnDisable()
		{
			Commit();
			Sync();
		}

		private void Update()
		{
			JumpAndGravity();
			GroundedCheck();
			Move();
		}

		private void LateUpdate()
		{
			bool lockInput = LockCameraPosition || MenuInstance.Instance.activeSelf;
			CameraRotation(lockInput);
			GunAction(lockInput);

			Sync();
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Grounded ? 
				new Color(0.0f, 1.0f, 0.0f, 0.35f) : 
				new Color(1.0f, 0.0f, 0.0f, 0.35f);

			Vector3 pos = transform.position;
			
			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(
				new Vector3(pos.x, pos.y - GroundedOffset, pos.z),
				GroundedRadius);
		}
	}
}