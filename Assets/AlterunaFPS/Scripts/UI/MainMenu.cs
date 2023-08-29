using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AlterunaFPS
{
	public class MainMenu : MonoBehaviour
	{
		public void LoadScene(int sceneIndex)
		{
			SceneManager.LoadScene(sceneIndex);
		}
		
		public void LoadScene(string sceneName)
		{
			SceneManager.LoadScene(sceneName);
		}
		
		public void Quit()
		{
#if UNITY_EDITOR
			EditorApplication.ExitPlaymode();
#else
			Application.Quit();
#endif
		}

		private void Awake()
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
	}
}