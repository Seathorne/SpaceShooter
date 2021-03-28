
using UnityEngine;

namespace Assets.Scripts
{
    public class Bullet : MonoBehaviour
    {
        [field: SerializeField, Range(0f, 100f)] public float Damage { get; set; } = 1f;

        public Ship Source { get; set; }

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