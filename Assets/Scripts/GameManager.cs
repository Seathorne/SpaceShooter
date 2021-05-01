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
        public Ship basicEnemyPrefab;

        private float lastAsteroidTime;
        [SerializeField, Range(0f, 60f)] private float asteroidSpawnTime;
        [SerializeField] private AnimationCurve asteroidSpawnTimeCurve;
        [SerializeField, Range(0f, 10f)] private int maxAsteroidCount;

        [SerializeField, Range(0f, 10f)] private int maxShipCount;

        public const string HighScore = "HighScore";
        public const string GameScene = "GameScene";
        public const string TitleScene = "TitleScene";

        [FMODUnity.EventRef] public string GameStartEvent;
        [FMODUnity.EventRef] public string EnemySpawnEvent;
        [FMODUnity.EventRef] public string UIPauseEvent;
        [FMODUnity.EventRef] public string UIUnpauseEvent;

        [SerializeField] private GameObject[] pausedObjects;
        [SerializeField] private MenuManager menu;

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
            IsPaused = true;
            Score = 0;
            Instance.StartCoroutine(Utility.CoroutineDelay(1f, StartGame));
        }

        private void StartGame()
        {
            IsPaused = false;
            menu.Enable = false;

            foreach (var ship in FindObjectsOfType<Ship>())
            {
                CreateHealthBar(ship);
            }

            UpdateScore(null);

            FMODUnity.RuntimeManager.PlayOneShot(GameStartEvent);
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        private void Update()
        {
            if (VirtualKey.Pause.JustPressed() && !IsPaused)
            {
                IsPaused = true;
                foreach (var obj in Instance.pausedObjects)
                {
                    obj.SetActive(true);
                }
                menu.Deselect();
                menu.Enable = true;
                FMODUnity.RuntimeManager.PlayOneShot(Instance.UIPauseEvent);
            }
            else if (VirtualKey.Unpause.JustPressed() && IsPaused)
            {
                IsPaused = false;
                foreach (var obj in Instance.pausedObjects)
                {
                    obj.SetActive(false);
                }
                menu.Deselect();
                menu.Enable = false;
                FMODUnity.RuntimeManager.PlayOneShot(Instance.UIUnpauseEvent);
            }
        }

        private void FixedUpdate()
        {
            if (!IsPaused)
            {
                TrySpawnAsteroid();
                if (FindObjectsOfType<BasicEnemy>().Length == 0)
                {
                    SpawnAnother(basicEnemyPrefab);
                }
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
            FMODUnity.RuntimeManager.PlayOneShot(setPaused ? Instance.UIPauseEvent : Instance.UIUnpauseEvent);
        }

        /// <summary>
        /// Restarts the game scene.
        /// </summary>
        public static void Restart()
        {
            Instance.StartCoroutine(Utility.CoroutineDelay(5f, () => UnityEngine.SceneManagement.SceneManager.LoadScene(TitleScene)));
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
                points = (int)prefab.MaxHealth;
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
            if (Score < 10)
            {
                Instance.maxAsteroidCount = 3;
                Instance.maxShipCount = 1;
            }
            else if (Score < 20)
            {
                Instance.maxAsteroidCount = 4;
                Instance.maxShipCount = 2;
            }
            else if (Score < 50)
            {
                Instance.maxAsteroidCount = 4;
                Instance.maxShipCount = 3;
            }
            else if (Score < 100)
            {
                Instance.maxAsteroidCount = 4;
                Instance.maxShipCount = 4;
            }
            else
            {
                Instance.maxAsteroidCount = 4 + (Score / 75);
                Instance.maxShipCount = 3 + (Score / 50);
            }

            int enemies = FindObjectsOfType(prefab.GetType()).Length - (prefab.isActiveAndEnabled ? 1 : 0);
            int margin = Instance.maxShipCount - enemies;
            float prop = (margin > 1 || enemies < 1)
                ? 1f
                : margin / Instance.maxShipCount;

            int count = 0;
            for (int i = 0; i < margin; i++)
            {
                if (Random.value < prop)
                {
                    count++;
                }
            }

            for (int i = 0; i < count; i++)
            {
                var ship = Instantiate(prefab);
                ship.Health = prefab.MaxHealth;
                ship.transform.position = ScreenEdgePosition(new Vector2(-1f, -1f));
                ship.enabled = false;

                Instance.StartCoroutine(
                    Utility.CoroutineDelay(Random.Range(0f, Instance.maxShipCount * 2f),
                    () =>
                    {
                        if (ship == null) return;
                        FMODUnity.RuntimeManager.PlayOneShot(Instance.EnemySpawnEvent, ship.transform.position);
                        ship.enabled = true;
                        CreateHealthBar(ship);
                    }));
            }
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
