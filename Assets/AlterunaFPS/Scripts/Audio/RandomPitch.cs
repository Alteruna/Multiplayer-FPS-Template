using UnityEngine;

namespace AlterunaFPS
{
    [RequireComponent(typeof(AudioSource))]
    public class RandomPitch : MonoBehaviour
    {
        public float MaxOffset = 0.05f;

        private AudioSource _audioSource;
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Play()
        {
            _audioSource.pitch = Random.Range(1f-MaxOffset, 1f+MaxOffset);
            _audioSource.Play();
        }
    }
}

