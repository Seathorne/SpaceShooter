using UnityEngine;

namespace Assets.Scripts
{
    public class BasicEnemy : Ship
    {
        protected new void Start()
        {
            base.Start();

            SetWeapon(GameManager.BulletGenerator.ShootBasic, GameManager.BulletGenerator.BasicBulletArgs);

            Behaviors += () => ShipExt.Face(this, GameManager.Player);

            Died += (sender, args) => GameManager.UpdateScore(prefab);
            Died += (sender, args) => GameManager.SpawnAnother(prefab);
            Died += (sender, args) => Destroy(gameObject);
        }

        protected new void Update()
        {
            base.Update();
            if (!GameManager.IsPaused)
            {
                TryShoot();
            }
        }

        protected override void UpdateMove()
        {
            StayInBounds();
        }

        protected override void UpdateRotate()
        {
            
        }
    }
}