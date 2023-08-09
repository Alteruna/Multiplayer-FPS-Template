using UnityEngine;

namespace AlterunaFPS
{
    public class GameInitSet : MonoBehaviour
    {
        public static bool Host = false;
        
        public void SetHost(bool host)
        {
            Host = host;
        }

        private void OnEnable()
        {
            Host = false;
        }
    }
}
