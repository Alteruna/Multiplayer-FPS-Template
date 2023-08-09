using System;
using UnityEngine;

namespace AlterunaFPS
{
	public class MainMenu : MonoBehaviour
	{
		public void LoadScene(int sceneIndex)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
		}
		
		public void Quit()
		{
			Application.Quit();
		}

		private void Awake()
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
	}
}