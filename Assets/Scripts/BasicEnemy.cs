namespace Assets.Scripts
{
    public class BasicEnemy : Ship
    {
        protected new void Start()
        {
            base.Start();

            var generator = GameManager.Instance.bulletGenerator;
            SetWeapon(generator.ShootBasic, generator.BasicBulletArgs);

            Behaviors += () => ShipExt.Face(this, GameManager.Instance.player);
            Died += () => GameManager.SpawnAnother(prefab);
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