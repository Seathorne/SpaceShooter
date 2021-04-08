
using UnityEngine;

namespace Assets.Scripts
{
    public static class ShipExt
    {
        public static void Face(this Ship self, Ship target)
        {
            self.transform.up = target.transform.position - self.transform.position;
        }
    }
}
