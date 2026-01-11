using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Helper
{
    public class OverrideDontDestroyOnLoad : MonoBehaviour
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
