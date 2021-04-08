
using UnityEngine;

namespace Assets.Scripts
{
    public static class ShipExt
    {
        public static void Face(this Ship self, Ship target)
        {
            self.transform.up = target.transform.position - self.transform.position;
        }

        public static void Seek(this Ship self, Ship target)
        {
            var rb = self.GetComponent<Rigidbody2D>();

            var dir = target.transform.position - self.transform.position;

            rb.velocity = self.MaxSpeed * dir;

            // Clamp actual velocity
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, self.MaxSpeed);
        }
    }
}
