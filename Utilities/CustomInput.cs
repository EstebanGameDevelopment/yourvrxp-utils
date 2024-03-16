
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace yourvrexperience.Utils
{
    public class CustomInput : TMP_InputField
    {
        public delegate void FocusEvent();
        public event FocusEvent OnFocusEvent;
        public event FocusEvent OnFocusDownEvent;

        public void DispatchFocusEvent()
        {
            if (OnFocusEvent != null) OnFocusEvent();
        }
        public void DispatchFocusDownEvent()
        {
            if (OnFocusDownEvent != null) OnFocusDownEvent();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            DispatchFocusDownEvent();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            DispatchFocusEvent();
        }
    }
}