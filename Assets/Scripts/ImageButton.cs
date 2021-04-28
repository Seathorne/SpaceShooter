using System;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public abstract class ImageButton : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Sprite defaultSprite;
        [SerializeField] private Sprite hoverSprite;
        [SerializeField] private Sprite clickSprite;

        protected event Action Clicked;

        public void OnMouseEnter()
        {
            image.sprite = hoverSprite;
        }

        public void OnMouseExit()
        {
            image.sprite = defaultSprite;
        }

        public void OnMouseDown()
        {
            image.sprite = clickSprite;
        }

        public void OnMouseUp()
        {
            image.sprite = defaultSprite;
        }

        public void OnMouseUpAsButton()
        {
            Clicked?.Invoke();
        }

    }
}
