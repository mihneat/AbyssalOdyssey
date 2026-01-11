using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Player.VR
{
    public class PlayerOverrideDontDestroyOnLoad : MonoBehaviour
    {
        private void Start()
        {
            SceneManager.activeSceneChanged += HandleOnSceneChanged;
        }

        void HandleOnSceneChanged(Scene oldScene, Scene newScene)
        {
            SceneManager.activeSceneChanged -= HandleOnSceneChanged;
            Destroy(gameObject);
        }
    }
}
