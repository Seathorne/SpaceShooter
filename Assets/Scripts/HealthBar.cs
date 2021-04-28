using Assets.Scripts;

using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public GameObject foreground;

    private void Update()
    {
        if (GameManager.Instance.player.Health <= 0f)
        {
            gameObject.SetActive(false);
        }
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
