
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace yourvrexperience.Utils
{
    public class CustomToggle : Toggle
    {
		public Action<CustomToggle> PointerEnterButton;
		public Action<CustomToggle> PointerExitButton;

        public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			PointerEnterButton?.Invoke(this);
		}
        public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			PointerExitButton?.Invoke(this);
		}
    }
}