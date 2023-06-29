using Cinemachine;
using UnityEngine;

namespace AlterunaFPS
{
	public class CinemachineVirtualCameraInstance : MonoBehaviour
	{
		public static CinemachineVirtualCameraInstance Instance;
		
		public CinemachineVirtualCamera FirstPersonCamera;
		public CinemachineVirtualCamera ThirdPersonCamera;
		private bool _firstPerson;

		// stored third person values
		private Vector3 _smoothing;
		private float _cameraDistance;
		
		public CinemachineVirtualCameraInstance()
		{
			Instance = this;
		}
		
		private void Awake()
		{
			SetFirstPerson(_firstPerson);
		}

		public bool FirstPerson
		{
			get => _firstPerson;
			set
			{
				if (_firstPerson == value) return;
				_firstPerson = value;
				SetFirstPerson(value);
			}
		}
		
		private void SetFirstPerson(bool value)
		{
			if (value)
			{
				FirstPersonCamera.gameObject.SetActive(true);
				ThirdPersonCamera.gameObject.SetActive(false);
			}
			else
			{
				FirstPersonCamera.gameObject.SetActive(false);
				ThirdPersonCamera.gameObject.SetActive(true);
			}
		}

		public void Follow(Transform target)
		{
			FirstPersonCamera.Follow = target;
			ThirdPersonCamera.Follow = target;
		}
	}
}