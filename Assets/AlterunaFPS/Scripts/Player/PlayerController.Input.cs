using Alteruna;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AlterunaFPS
{
	public partial class PlayerController
	{
		private SyncedAxis _horizontal;
		private SyncedAxis _vertical;
		
		private SyncedKey _jump;
		private SyncedKey _sprint;
		private SyncedKey _reload;
		private SyncedKey _camera;
		
		private InputSynchronizable _input;
		
		private float MouseX => Input.GetAxisRaw("Mouse X");
		private float MouseY => Input.GetAxisRaw("Mouse Y");
		
		private void InitializeInput()
		{
			if (_isOwner)
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}
			
			_input = GetComponent<InputSynchronizable>();
			
			_horizontal = new SyncedAxis(_input, "Horizontal");
			_vertical = new SyncedAxis(_input, "Vertical");
			
			_jump = new SyncedKey(_input, KeyCode.Space, SyncedKey.KeyMode.KeyDown); 
			_sprint = new SyncedKey(_input, KeyCode.LeftShift);
			_reload = new SyncedKey(_input, KeyCode.R, SyncedKey.KeyMode.KeyDown);
			_camera = new SyncedKey(_input, KeyCode.V, SyncedKey.KeyMode.ToggleKeyDown);
		}
	}
}