using UnityEngine;

namespace AlterunaFPS
{
	public partial class PlayerController
	{
		
		[Header("Cinemachine"), SerializeField]
		private bool _firstPerson = true;
		
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow while in first person mode")]
		public Transform FirstPersonCameraTarget;
		
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow while in third person mode")]
		public Transform ThirdPersonCameraTarget;

		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 70.0f;

		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -30.0f;

		[Tooltip("Additional degrees to override the camera. Useful for fine tuning camera position when locked")]
		public float CameraAngleOverride = 0.0f;

		[Tooltip("For locking the camera position on all axis")]
		public bool LockCameraPosition = false;
		
		[Range(0f, 0.4f)]
		public float FirstPersonCameraRotationSmoothing = 0.3f;
		
		
		private bool _defaultCamera;
		
		private float _cinemachineTargetYaw;
		private float _cinemachineTargetPitch;
		private float _bodyRotate = 0f;
		
		private Transform _cameraTarget;
		
		public bool FirstPerson
		{
			get => _firstPerson;
			set
			{
				if (_firstPerson == value) return;
				SetFirstPerson(value);
			}
		}

		private void InitializeCamera()
		{
			_cameraTarget = _firstPerson ? FirstPersonCameraTarget : ThirdPersonCameraTarget;
			
			if (_isOwner)
			{
				CinemachineVirtualCameraInstance.Instance.Follow(_cameraTarget);
				CinemachineVirtualCameraInstance.Instance.FirstPerson = _firstPerson;
			}
			
			_defaultCamera = _firstPerson;
			_cinemachineTargetYaw = _cameraTarget.rotation.eulerAngles.y;
		}
		
		private void SetFirstPerson(bool value)
		{
			_firstPerson = value;
			if (_isOwner)
				_cameraTarget = value ? FirstPersonCameraTarget : ThirdPersonCameraTarget;
				
			var rot = _cameraTarget.rotation.eulerAngles;
			_cameraTarget.rotation = Quaternion.Euler(rot.x, _cinemachineTargetYaw, rot.z);

			if (_isOwner)
			{
				CinemachineVirtualCameraInstance.Instance.FirstPerson = value;
				CinemachineVirtualCameraInstance.Instance.Follow(_cameraTarget);
			}
		}
		
		private void CameraRotation(bool lockInput = false)
		{
			// smooths POV hands
			if (_isOwner && _firstPerson)
				_animator.Update(0);
			
			bool AllowInput = _isOwner && !lockInput;

			var oldYaw = _cinemachineTargetYaw;
			var oldPitch = _cinemachineTargetPitch;

			FirstPerson = _defaultCamera ? !_camera : _camera;

			if (AllowInput)
				_cinemachineTargetYaw += MouseX;

			if (_firstPerson)
			{
				var euler = NormalizeAnglePos(transform.localRotation.eulerAngles.y);


				const float maxDifference = 90f;
				float difference = GetAngleBetweenAngles(_cinemachineTargetYaw, euler);

				if (difference > maxDifference)
				{
					_bodyRotate += difference - maxDifference;
					_bodyRotate = NormalizeAngle(_bodyRotate);

					// apply rotation
					transform.rotation = Quaternion.Euler(0, _bodyRotate, 0);
				}
				else if (difference < -maxDifference)
				{
					_bodyRotate += difference + maxDifference;
					_bodyRotate = NormalizeAngle(_bodyRotate);

					// apply rotation
					transform.rotation = Quaternion.Euler(0, _bodyRotate, 0);
				}

				_cinemachineTargetYaw = NormalizeAnglePos(_cinemachineTargetYaw);
			}

			if (AllowInput)
			{
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch - MouseY, BottomClamp, TopClamp);

				if (Mathf.Abs(oldYaw - _cinemachineTargetYaw) > 0.01f || Mathf.Abs(oldPitch - _cinemachineTargetPitch) > 0.01f)
				{
					Commit();
				}
			}
			else
			{
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
			}

			// Cinemachine will follow this target
			if (!_firstPerson)
			{
				_cameraTarget.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
			}
			else
			{
				_cameraTarget.rotation = Quaternion.Lerp(
					_cameraTarget.rotation,
					Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0),
					Time.deltaTime * (FirstPersonCameraRotationSmoothing / Time.smoothDeltaTime));
			}

			// update animator if using character
			if (_hasAnimator)
			{
				// Calculate the difference in Euler angles
				Vector3 eulerDifference = new Vector3(_cinemachineTargetPitch, _cinemachineTargetYaw, 0f) - transform.localRotation.eulerAngles;

				// Normalize the difference to the range [-180, 180]
				eulerDifference = NormalizeEulerAnglesXY(eulerDifference);

				_animator.SetFloat(_animIDHeadLookY, Mathf.Clamp(eulerDifference.y, -90f, 90f));
				_animator.SetFloat(_animIDHeadLookX, Mathf.Clamp(eulerDifference.x, -90f, 90f));
			}
		}
	}
}