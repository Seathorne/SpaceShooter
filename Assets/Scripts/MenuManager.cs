using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private ImageButton[] buttons;

        private int i = 0;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            int old = -1;
            if (VirtualKey.Down.JustPressed() || VirtualKey.Right.JustPressed())
            {
                old = i;
                i = (i + 1) % buttons.Length;
            }
            else if (VirtualKey.Up.JustPressed() || VirtualKey.Left.JustPressed())
            {
                old = i;
                i--;
                if (i < 0)
                {
                    i = buttons.Length - 1;
                }
            }

            if (old != -1)
            {
                buttons[old].OnMouseExit();
                buttons[i].OnMouseEnter();
            }

            if (VirtualKey.Accept.JustPressed())
            {
                buttons[i].OnMouseUpAsButton();
            }
        }
    }
}
