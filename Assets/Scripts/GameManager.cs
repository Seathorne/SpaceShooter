using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Represents global game logic and game state.
    /// Authors: Scott Clarke and Daniel Darnell.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public HealthBar healthBarPrefab;

        /// <summary>
        /// Whether the game is currently paused.
        /// </summary>
        public static bool IsPaused { get; private set; } = false;

        public static float Score { get; private set; }

        public static Vector2 HalfScreenSize => new Vector2
        {
            x = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)), Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0))) * 0.5f,
            y = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)), Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height))) * 0.5f,
        };

        public static Ship Player { get; private set; }

        public static BulletGenerator BulletGenerator { get; private set; }

        public static HealthBar HealthBarPrefab { get; private set; }

        private void Awake()
        {
            Player = FindObjectOfType<Player>();
            BulletGenerator = FindObjectOfType<BulletGenerator>();
            HealthBarPrefab = healthBarPrefab;
        }

        private void Start()
        {
            foreach (var ship in FindObjectsOfType<Ship>())
            {
                CreateHealthBar(ship);
            }
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

        public static void UpdateScore(Ship prefab)
        {
            // TODO
            Score += 1;
            print($"Score: {Score}");
        }

        public static void SpawnAnother(Ship prefab)
        {
            var ship = Instantiate(prefab);
            ship.Health = prefab.MaxHealth;

            (float x, float y) halfSize = (0.5f, 0.5f);

            ship.transform.position = new Vector2
            {
                x = Random.Range(-HalfScreenSize.x + halfSize.x, HalfScreenSize.x - halfSize.x),
                y = Random.Range(-HalfScreenSize.y + halfSize.y, HalfScreenSize.y - halfSize.y)
            };

            CreateHealthBar(ship);
        }

        private static void CreateHealthBar(Ship ship)
        {
            var health = Instantiate(HealthBarPrefab);
            ship.Moved += () => health.UpdatePosition(ship);
            ship.HealthChanged += () => health.UpdateSize(ship);
            ship.Died += () => Destroy(health.gameObject);
        }
    }
}
