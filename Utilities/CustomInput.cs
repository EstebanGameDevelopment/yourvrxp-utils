
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace yourvrexperience.Utils
{
    public class CustomInput : TMP_InputField
    {
        public delegate void FocusEvent();
        public event FocusEvent OnFocusEvent;

        public void DispatchFocusEvent()
        {
            if (OnFocusEvent != null)
                OnFocusEvent();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            DispatchFocusEvent();
        }
    }
}