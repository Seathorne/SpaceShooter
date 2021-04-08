using System;

using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    /// <summary>
    /// Represents global game logic and game state.
    /// Authors: Scott Clarke and Daniel Darnell.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public BulletGenerator bulletGenerator;
        public Player player;
        public HealthBar healthBarPrefab;
        public Asteroid asteroidPrefab;
        public Text scoreText;

        private float lastAsteroidTime;
        [SerializeField, Range(0f, 60f)] private float asteroidSpawnTime;
        [SerializeField] private AnimationCurve asteroidSpawnTimeCurve;
        [SerializeField, Range(0f, 10f)] private float maxAsteroidCount;

        public const string HighScore = "HighScore";
        public const string GameScene = "GameScene";

        /// <summary>
        /// Whether the game is currently paused.
        /// </summary>
        public static bool IsPaused { get; private set; } = false;

        public static int Score { get; private set; }

        public static Vector2 HalfScreenSize => new Vector2
        {
            x = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)), Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0))) * 0.5f,
            y = Vector2.Distance(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)), Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height))) * 0.5f,
        };

        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            Instance = FindObjectOfType<GameManager>();
        }

        private void Start()
        {
            foreach (var ship in FindObjectsOfType<Ship>())
            {
                CreateHealthBar(ship);
            }
            UpdateScore(null);
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

        private void FixedUpdate()
        {
            if (!IsPaused)
            {
                TrySpawnAsteroid();
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
        public static void Restart()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(GameScene);
            Score = 0;
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
            int points = 0;
            if (prefab is Asteroid)
            {
                points = 1;
            }
            else if (prefab is BasicEnemy)
            {
                points = 3;
            }

            Score += points;
            int highScore = PlayerPrefs.GetInt(HighScore);

            if (Score > highScore)
            {
                highScore = Score;
                PlayerPrefs.SetInt(HighScore, highScore);
            }

            Instance.scoreText.text = $"Score: {Score}\nHigh Score: {highScore}";
        }

        public void TrySpawnAsteroid()
        {
            float asteroidTimeRand = asteroidSpawnTime * asteroidSpawnTimeCurve.Evaluate(Random.value);
            if (Asteroid.Asteroids.Length < maxAsteroidCount && Time.time - lastAsteroidTime >= asteroidTimeRand)
            {
                var asteroid = Asteroid.Instantiate();
                CreateHealthBar(asteroid);
            }
        }

        private static (float right, float up) Bounds(Vector2 buffer) => (HalfScreenSize.x - buffer.x, HalfScreenSize.y - buffer.y);

        public static bool IsWithinScreen(Transform transform, Vector2 buffer)
        {
            var pos = transform.position;
            (float right, float up) = Bounds(buffer);

            return pos.x >= -right
                && pos.x <= right
                && pos.y >= -up
                && pos.y <= up;
        }

        public static Vector2 ScreenPosition(Vector2 buffer)
        {
            (float right, float up) = Bounds(buffer);

            return new Vector2
            {
                x = Random.Range(-right, right),
                y = Random.Range(-up, up)
            };
        }

        public static Vector2 ScreenEdgePosition(Vector2 buffer)
        {
            float rand = Random.value;

            (float right, float up) = Bounds(buffer);

            if (rand < 0.25f)
            { // Up
                return new Vector2
                {
                    x = Random.Range(-right, right),
                    y = up
                };
            }
            else if (rand < 0.5f)
            { // Down
                return new Vector2
                {
                    x = Random.Range(-right, right),
                    y = -up
                };
            }
            else if (rand < 0.75f)
            { // Right
                return new Vector2
                {
                    x = right,
                    y = Random.Range(-up, up)
                };
            }

            // Left
            return new Vector2
            {
                x = -right,
                y = Random.Range(-up, up)
            };
        }

        public static void SpawnAnother(Ship prefab)
        {
            var ship = Instantiate(prefab);
            ship.Health = prefab.MaxHealth;

            ship.transform.position = ScreenPosition(new Vector2(0.5f, 0.5f));

            CreateHealthBar(ship);
        }

        private static void CreateHealthBar(Ship ship)
        {
            var health = Instantiate(Instance.healthBarPrefab);
            ship.Moved += () => health.UpdatePosition(ship);
            ship.HealthChanged += () => health.UpdateSize(ship);
            ship.Died += () => Destroy(health.gameObject);
        }
    }
}
