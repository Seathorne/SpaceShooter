
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public delegate void Behavior(Ship self, Ship target);

    public abstract class Ship : MonoBehaviour
    {
        public const float ShootTimeVar = 0.2f;

        protected float lastShootTime;

        [SerializeField] protected Ship prefab;

        [field: Header("Stats")]
        [field: SerializeField, Range(0f, 100f)] public float MaxSpeed { get; protected set; }

        [field: SerializeField, Range(0f, 360f)] public float MaxRotation { get; protected set; }

        [field: SerializeField, Range(0f, 100f)] public float MaxHealth { get; protected set; }

        [SerializeField, Range(0f, 100f)] private float _Health;

        public float Health
        {
            get => _Health;
            set
            {
                _Health = value;
                HealthChanged?.Invoke();
            }
        }

        [field: SerializeField, Range(0f, 100f)] public float Defense { get; protected set; }

        public event Action Behaviors;

        public event Action Moved;

        public event Action HealthChanged;

        public event Action Died;

        public static Bullet[] Bullets => FindObjectsOfType<Bullet>();

        public IEnumerable<Bullet> MyBullets => Bullets.Where(x => x.Source == this);

        public Vector2 Position => transform.position;

        public Vector2 Facing => transform.up;

        public BulletArgs? BulletArgs { get; protected set; } = null;

        public BulletFactory BulletFactory { get; protected set; }

        [field: SerializeField, Range(0, 100)] public int MaxBulletCount { get; protected set; }

        [field: SerializeField, Range(0f, 10f)] public float ShootTime { get; protected set; }

        protected void Start()
        {
            Died += () => GameManager.UpdateScore(prefab);
        }

        protected void Update()
        {

        }

        protected void FixedUpdate()
        {
            if (!GameManager.IsPaused)
            {
                Behaviors?.Invoke();
                UpdateMove();
                UpdateRotate();
                Moved?.Invoke();
            }
        }

        protected abstract void UpdateMove();

        protected abstract void UpdateRotate();

        protected virtual bool CanShoot()
        {
            float shootTimeRand = Random.Range((1f - ShootTimeVar) * ShootTime, (1f + ShootTimeVar) * ShootTime);
            return (Time.time - lastShootTime >= shootTimeRand && Bullets.Length < MaxBulletCount);
        }

        protected virtual IEnumerable<Bullet> TryShoot()
        {
            if (CanShoot())
            {
                lastShootTime = Time.time;
                return Shoot();
            }

            return null;
        }

        protected virtual IEnumerable<Bullet> Shoot()
        {
            return (BulletArgs is BulletArgs args)
                ? BulletFactory(this, args)
                : null;
        }

        public virtual void HitBy(Bullet bullet)
        {
            float damage = bullet.Damage - Defense;
            if (damage > 0f)
            {
                Health -= damage;

                if (Health <= 0f)
                {
                    Die();
                }
            }
        }

        public void Die()
        {
            Died?.Invoke();
            Destroy(gameObject);
        }

        protected void StayInBounds()
        {
            var screenSize = GameManager.HalfScreenSize;
            (float x, float y) halfSize = (0.5f, 0.5f);

            var rb = GetComponent<Rigidbody2D>();
            var newPos = Position;
            var newVel = rb.velocity;
            var force = Vector2.zero;
            const float bounce = 50f;

            bool outside = false;

            if (Position.x + halfSize.x > screenSize.x)
            {
                newPos.x = screenSize.x - halfSize.x;
                newVel.x = 0f;
                force.x = -bounce;
                outside = true;
            }
            else if (Position.x - halfSize.x < -screenSize.x)
            {
                newPos.x = -screenSize.x + halfSize.x;
                newVel.x = 0f;
                force.x = bounce;
                outside = true;
            }

            if (Position.y + halfSize.y > screenSize.y)
            {
                newPos.y = screenSize.y - halfSize.y;
                newVel.y = 0f;
                force.y = -bounce;
                outside = true;
            }
            else if (Position.y - halfSize.y < -screenSize.y)
            {
                newPos.y = -screenSize.y + halfSize.y;
                newVel.y = 0f;
                force.y = bounce;
                outside = true;
            }

            if (outside == false) return;

            rb.MovePosition(newPos);
            rb.velocity = newVel;
            rb.AddForce(force);
        }

        public void SetWeapon(BulletFactory type, BulletArgs? args)
        {
            BulletArgs = args;
            BulletFactory = type;
        }
    }
}
