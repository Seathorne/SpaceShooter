using Assets.Scripts;

using System.Collections.Generic;

using UnityEngine;

public class Asteroid : Ship
{
    [SerializeField] private List<Sprite> sprites;

    [SerializeField, Range(0f, 10f)] public float minSpeed;
    [SerializeField, Range(0f, 10f)] public float minScale;
    [SerializeField, Range(0f, 10f)] public float maxScale;

    public static Asteroid[] Asteroids => FindObjectsOfType<Asteroid>();

    private static Vector2 Outside => new Vector2(-2f, -2f);
    private static Vector2 Inside => new Vector2(0.5f, 0.5f);

    public static Asteroid Instantiate()
    {
        var prefab = GameManager.Instance.asteroidPrefab;
        var sprite = prefab.sprites[(int)Random.Range(0, prefab.sprites.Count - float.Epsilon)];
        float scale = Random.Range(prefab.minScale, prefab.maxScale);

        var pos = GameManager.ScreenEdgePosition(Outside);
        var target = GameManager.ScreenPosition(Inside);
        float speed = Random.Range(prefab.minSpeed, prefab.MaxSpeed);
        float rotation = Random.Range(-prefab.MaxRotation, prefab.MaxRotation);
        float health = Random.Range(1f, prefab.MaxHealth);

        var asteroid = Instantiate(prefab);
        asteroid.GetComponent<SpriteRenderer>().sprite = sprite;
        asteroid.transform.localScale = new Vector2(scale, scale);

        var rb = asteroid.GetComponent<Rigidbody2D>();

        asteroid.transform.position = pos;
        rb.velocity = (target - pos).normalized * speed;
        rb.angularVelocity = rotation;
        asteroid.Health = health;

        return asteroid;
    }

    protected override void UpdateMove()
    {
        if (!GameManager.IsWithinScreen(transform, Outside))
        {
            Die();
        }
    }

    protected override void UpdateRotate()
    {
        
    }
}
