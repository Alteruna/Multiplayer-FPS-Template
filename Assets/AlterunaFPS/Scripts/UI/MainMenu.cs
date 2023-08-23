using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AlterunaFPS
{
	public class MainMenu : MonoBehaviour
	{
		public void LoadScene(int sceneIndex)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
		}
		
		public void LoadScene(string sceneName)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
		}
		
		public void LoadScene(SceneAsset scene)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(scene.name);
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