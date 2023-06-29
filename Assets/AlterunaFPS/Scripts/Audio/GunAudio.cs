using UnityEngine;

namespace AlterunaFPS
{
	public class GunAudio : MonoBehaviour
	{
		public RandomPitch ReloadSfx;
		public RandomPitch FireSfx;
		
		public void PlayReloadSfx() => ReloadSfx.Play();
		public void PlayFireSfx() => FireSfx.Play();
	}
}