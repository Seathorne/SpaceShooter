using Assets.Scripts;

using System.Linq;

using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public const string HealthBarForegroundTag = "HealthBarForeground";

    public GameObject foreground;

    public void Start()
    {
        //print(GetComponents<MonoBehaviour>());
        //foreground = GetComponents<MonoBehaviour>().First(x => x.CompareTag(HealthBarForegroundTag));
    }

    public void UpdateSize(Ship ship)
    {
        // Set red part of bar
        float healthProp = ship.Health / ship.MaxHealth;
        foreground.transform.localPosition = new Vector3((1f - healthProp) / -2f, 0f, -0.1f);
        foreground.transform.localScale = new Vector3(healthProp, 1f, 1f);
    }

    public void UpdatePosition(Ship ship)
    {
        // Position at and above the ship
        transform.position = ship.transform.position;
    }
}
