
using UnityEngine;

namespace Assets.Scripts
{
    public class Bullet : MonoBehaviour
    {
        [field: SerializeField, Range(0f, 100f)] public float Damage { get; set; } = 1f;

        public Ship Source { get; set; }

        private bool paused;
        private Vector2 velocity;
        private float angular;

        private void Update()
        {
            if (!paused && GameManager.IsPaused)
            {
                var rb = GetComponent<Rigidbody2D>();

                paused = true;
                velocity = rb.velocity;
                angular = rb.angularVelocity;

                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;

                return;
            }
            else if (paused && !GameManager.IsPaused)
            {
                var rb = GetComponent<Rigidbody2D>();

                paused = false;
                rb.velocity = velocity;
                rb.angularVelocity = angular;
            }
        }

        protected void OnBecameInvisible()
        {
            Destroy(gameObject);
        }

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.GetComponent<Ship>() is Ship ship)
            {
                // Ignore self-collisions
                if (ship == Source)
                {
                    return;
                }

                Destroy(gameObject);
                ship.HitBy(this);
            }
        }
    }
}