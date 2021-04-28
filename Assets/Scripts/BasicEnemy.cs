using UnityEngine;

namespace Assets.Scripts
{
    public class BasicEnemy : Ship
    {
        [SerializeField] private Sprite[] sprites;

        protected new void Start()
        {
            base.Start();

            var generator = GameManager.Instance.bulletGenerator;

            Behaviors += () => ShipExt.Face(this, GameManager.Instance.player);
            Behaviors += () => ShipExt.Seek(this, GameManager.Instance.player);
            Died += () => GameManager.SpawnAnother(prefab);

            float r = Random.value;
            int i;
            if (r < 0.6f)
            {
                i = 0;
                ShootTime = 3f;
                MaxSpeed = 2f;
                MaxHealth = 5f;
                Health = 5f;
                CollisionDamage = 1f;
                SetWeapon(generator.ShootBasic, generator.BasicBulletArgs);
            }
            else if (r < 0.9f)
            {
                i = 1;
                ShootTime = 2.5f;
                MaxSpeed = 2.3f;
                MaxHealth = 8f;
                Health = 8f;
                CollisionDamage = 1.1f;
                SetWeapon(generator.ShootBasic, new BulletArgs { Damage = 1.5f, Speed = 10f });
            }
            else
            {
                i = 2;
                ShootTime = 1.5f;
                MaxSpeed = 2.6f;
                MaxHealth = 12f;
                Health = 12f;
                CollisionDamage = 1.2f;
                SetWeapon(generator.ShootBasic, new BulletArgs { Damage = 2f, Speed = 10f });
            }


            GetComponent<SpriteRenderer>().sprite = sprites[i];
        }

        protected new void FixedUpdate()
        {
            base.FixedUpdate();

            if (!GameManager.IsPaused)
            {
                TryShoot();
            }
        }

        protected override void UpdateMove()
        {
            //ShipExt.Seek(this, GameManager.Instance.player);
            //StayInBounds();
        }

        protected override void UpdateRotate()
        {
            
        }
    }
}