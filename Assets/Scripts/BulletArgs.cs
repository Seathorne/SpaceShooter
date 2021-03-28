
using UnityEngine;

namespace Assets.Scripts
{
    [System.Serializable]
    public struct BulletArgs
    {
        [field: SerializeField, Range(0f, 100f)] public float Speed { get; set; }

        [field: SerializeField, Range(0f, 100f)] public float Damage { get; set; }
    }

    //public class BulletArgsSerializer : MonoBehaviour, ISerializationCallbackReceiver
    //{
    //    [Serializable]
    //    internal struct SerializableBulletArgs
    //    {
    //        public float speed;

    //        public float damage;

    //        public SerializableBulletArgs(BulletArgs args)
    //        {
    //            speed = args.Speed;
    //            damage = args.Damage;
    //        }
    //    }

    //    public void OnBeforeSerialize()
    //    {
    //        var serialized = new SerializableBulletArgs()
    //    }
    //}
}