using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Represents global game logic and game state.
    /// Authors: Scott Clarke and Daniel Darnell.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// Whether the game is currently paused.
        /// </summary>
        public static bool IsPaused { get; private set; } = false;

        public static float Score { get; private set; }

        /// <summary>
        /// Use this for initialization.
        /// </summary>
        private void Start()
        {

        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        private void Update()
        {
            if (VirtualKey.Pause.JustPressed() && !IsPaused)
            {
                IsPaused = true;
            }
            else if (VirtualKey.Unpause.JustPressed() && IsPaused)
            {
                IsPaused = false;
            }
        }

        /// <summary>
        /// Pauses or unpauses the game.
        /// </summary>
        /// <param name="setPaused"><see langword="true"/> to pause the game; <see langword="false"/> to unpause.</param>
        public static void Pause(bool setPaused = true)
        {
            Time.timeScale = setPaused ? 0f : 1f;
            IsPaused = setPaused;
        }

        /// <summary>
        /// Restarts the game scene.
        /// </summary>
        public void Restart()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }

        /// <summary>
        /// Exits the game.
        /// </summary>
        public void Exit()
        {
            Application.Quit();
        }

        public static void UpdateScore(Ship killed)
        {
            // TODO
            Score += 1;
            print($"Score: {Score}");
        }
    }
}
