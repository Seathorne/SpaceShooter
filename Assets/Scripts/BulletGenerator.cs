using System.Collections.Generic;

using UnityEngine;

namespace Assets.Scripts
{
    public delegate IEnumerable<Bullet> BulletFactory(Ship source, BulletArgs args);

    public class BulletGenerator : MonoBehaviour
    {
        [SerializeField] public Bullet basicBullet;

        public IEnumerable<Bullet> ShootBasic(Ship source, BulletArgs args)
        {
            var bullet = Instantiate(basicBullet);
            bullet.Source = source;
            bullet.Damage = args.Damage;

            // Shoot bullet
            Relocate(source.transform, bullet);
            var rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = source.Facing * args.Speed;

            return new Bullet[] { bullet };
        }

        private static void Relocate(Transform source, params Bullet[] bullets)
        {
            foreach (var bullet in bullets)
            {
                bullet.transform.position = source.position;
                bullet.transform.rotation = source.rotation;
                bullet.transform.localScale = source.localScale;
            }
        }
    }
}