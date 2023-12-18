using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AlterunaFPS
{
	public class MenuInstance : MonoBehaviour
	{
		public static GameObject Instance;

		[SerializeField] private GameObject _menuObject;


		private void Awake()
		{
			Instance = _menuObject;
			_menuObject.SetActive(false);
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				ToggleMenu();
			}
		}

		private void ToggleMenu()
		{
			_menuObject.SetActive(!_menuObject.activeSelf);
			Cursor.visible = _menuObject.activeSelf;
			Cursor.lockState = _menuObject.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
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