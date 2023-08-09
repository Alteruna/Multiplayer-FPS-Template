using System;
using UnityEngine;

namespace AlterunaFPS
{
	public class MenuInstance : MonoBehaviour
	{
		public static GameObject Instance;

		private void Awake()
		{
			Instance = gameObject;
			gameObject.SetActive(false);
		}

		private void OnDisable()
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void OnEnable()
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
		
		public void LoadScene(int sceneIndex)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
		}
	}
}