using TMPro;
using UnityEngine;

namespace VikingParty
{
    public class TextEffectController : AnimationEffect
    {
        public float borders = 10f;
        public TextMeshProUGUI tmPro;
        public RectTransform rectTransform;
        protected override void Awake()
        {
            base.Awake();
            if (animation == null)
                animation = GetComponent<Animation>();
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();
            if (tmPro == null)
                tmPro = GetComponentInChildren<TextMeshProUGUI>();
        }

        public override void ChangeColor(Color color)
        {
            tmPro.color = color;
            base.ChangeColor(color);
        }
        public void ChangeTextValue(string t)
        {
            tmPro.text = t;
            rectTransform.sizeDelta = new Vector2(tmPro.preferredWidth + borders * 2, rectTransform.sizeDelta.y);
        }
    }
}