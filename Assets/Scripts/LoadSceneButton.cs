
using UnityEngine;

namespace Assets.Scripts
{
    public class LoadSceneButton : ImageButton
    {
        [SerializeField] private string scene;

        private void Start()
        {
            Clicked += () => UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
        }
    }
}
