using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

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

		public void OnEnable()
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
		
		public void LoadScene(int sceneIndex)
		{
			SceneManager.LoadScene(sceneIndex);
		}
		
		public void LoadScene(string sceneName)
		{
			SceneManager.LoadScene(sceneName);
		}
	}
}