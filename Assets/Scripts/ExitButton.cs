
using UnityEngine;

namespace Assets.Scripts
{
    public class ExitButton : ImageButton
    {
        private void Start()
        {
            Clicked += () => Application.Quit();
        }
    }
}
