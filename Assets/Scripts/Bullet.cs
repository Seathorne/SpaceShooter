using UnityEngine;

public class Bullet : MonoBehaviour
{
    //[SerializeField] private Bullet Prefab;

    public float Speed { get; private set; } = 10f;

    public float Damage { get; private set; } = 1f;

    public Vector2 Facing { get; private set; }

    protected void Start()
    {
        
    }

    protected void Update()
    {
        
    }

    protected void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() is Player)
        {
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }

    public void Shoot(Transform source, Vector2 facing)
    {
        transform.position = source.position;
        transform.rotation = source.rotation;
        transform.localScale = source.localScale;

        Facing = facing;

        var rb = GetComponent<Rigidbody2D>();
        rb.velocity = Facing * Speed;
    }
}
