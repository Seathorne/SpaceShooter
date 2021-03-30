
using System;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Scripts
{
    public abstract class Ship : MonoBehaviour
    {
        [field: Header("Stats")]
        [field: SerializeField, Range(0f, 100f)] public float MaxSpeed { get; protected set; }

        [field: SerializeField, Range(0f, 360f)] public float MaxRotation { get; protected set; }

        [field: SerializeField, Range(0f, 100f)] public float Health { get; protected set; }

        [field: SerializeField, Range(0f, 100f)] public float Defense { get; protected set; }

        public event EventHandler Died;

        public BulletArgs BulletArgs { get; protected set; }

        public BulletFactory BulletFactory { get; protected set; }

        public Vector2 Position => transform.position;

        public Vector2 Facing => transform.up;

        protected virtual void Start()
        {
            Died += (sender, args) => GameManager.UpdateScore(this);
            Died += (sender, args) => GameManager.SpawnAnother(this);
            Died += (sender, args) => Destroy(gameObject);
        }

        protected virtual void FixedUpdate()
        {
            if (!GameManager.IsPaused)
            {
                UpdateMove();
                UpdateRotate();
            }
        }

        protected abstract void UpdateMove();

        protected abstract void UpdateRotate();

        protected virtual IEnumerable<Bullet> Shoot()
        {
            return BulletFactory(this, BulletArgs);
        }

        public virtual void HitBy(Bullet bullet)
        {
            float damage = bullet.Damage - Defense;
            if (damage > 0f)
            {
                Health -= damage;
                if (Health <= 0f)
                {
                    Died?.Invoke(this, null);
                }
            }
        }
    }
}
