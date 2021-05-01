
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

        [field: SerializeField, Range(0f, 100f)] public float CollisionDamage { get; protected set; }

        [SerializeField, Range(0f, 100f)] private float _Health;

        [FMODUnity.EventRef] public string BulletHitEvent;

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

        [FMODUnity.EventRef] public string CollideEvent;
        [FMODUnity.EventRef] public string ExplodeEvent;
        [FMODUnity.EventRef] public string GameEndEvent;

        private bool paused;
        private Vector2 velocity;
        private float angular;

        protected void Start()
        {
            Died += () => GameManager.UpdateScore(prefab);

            lastShootTime = Time.time;
        }

        protected void Update()
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
            float distance = (this is Player) ? 0f : Vector2.Distance(transform.position, GameManager.Instance.player.transform.position);
            float distParam = Mathf.InverseLerp(0f, GameManager.HalfScreenSize.x * 2f, distance);

            var sound = FMODUnity.RuntimeManager.CreateInstance(BulletHitEvent);
            sound.setParameterByName("Distance", distParam);
            sound.start();
            sound.release();

            TakeDamage(bullet.Damage);
        }

        public virtual float HitBy(Ship ship)
        {
            float damage = ship.CollisionDamage * (1f + 0.15f * ship.GetComponent<Rigidbody2D>().velocity.magnitude);
            TakeDamage(damage);
            return damage;
        }

        private void TakeDamage(float preDefenseDamage)
        {
            float damage = preDefenseDamage - Defense;
            if (damage > 0f)
            {
                Health -= damage;

                if (Health <= 0f)
                {
                    Die();
                }
            }
        }

        protected void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.GetComponent<Ship>() is Ship ship)
            {
                float incomingDamage = this.HitBy(ship);
                float outgoingDamage = ship.HitBy(this);

                // Prepare collision sound
                (float damage, Ship faster) = (incomingDamage > outgoingDamage)
                    ? (incomingDamage, ship)
                    : (outgoingDamage, this);

                var rb = faster.GetComponent<Rigidbody2D>();

                float distance = (this is Player) ? 0f : Vector2.Distance(transform.position, GameManager.Instance.player.transform.position);
                float distParam = Mathf.InverseLerp(0f, GameManager.HalfScreenSize.x * 2f, distance);

                float realMaxSpeed = 13f; // TODO max speed currently limited by drag
                float speedParam = Mathf.InverseLerp(0f, realMaxSpeed, rb.velocity.magnitude);

                var sound = FMODUnity.RuntimeManager.CreateInstance(CollideEvent);
                sound.setParameterByName("Distance", distParam);
                sound.setParameterByName("Speed", speedParam);
                sound.start();
                sound.release();
            }
        }

        public void Die()
        {
            float distance = (this is Player) ? 0f : Vector2.Distance(transform.position, GameManager.Instance.player.transform.position);
            float param = Mathf.InverseLerp(0f, GameManager.HalfScreenSize.x * 2f, distance);

            FMOD.Studio.EventInstance sound = FMODUnity.RuntimeManager.CreateInstance(ExplodeEvent);
            sound.setParameterByName("Distance", param);
            sound.start();
            sound.release();

            if (this is Player)
            {
                FMODUnity.RuntimeManager.PlayOneShot(GameEndEvent);
            }

            Health = 0f;
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
