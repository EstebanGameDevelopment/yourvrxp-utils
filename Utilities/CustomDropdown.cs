using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace yourvrexperience.Utils
{
    public class CustomDropdown : TMP_Dropdown
    {
        public delegate void OpenedEvent();
        public event OpenedEvent OnOpenedEvent;

        public delegate void ClosedEvent();
        public event ClosedEvent OnClosedEvent;

        public void DispatchOpenedEvent()
        {
            if (OnOpenedEvent != null)
                OnOpenedEvent();
        }

        public void DispatchCloseEvent()
        {
            if (OnClosedEvent != null)
                OnClosedEvent();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            DispatchOpenedEvent();
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
            DispatchCloseEvent();
        }

        public override void OnCancel(BaseEventData eventData)
        {
            base.OnCancel(eventData);
            DispatchCloseEvent();
        }

        protected override void DestroyDropdownList(GameObject dropdownList)
        {
            base.DestroyDropdownList(dropdownList);
            DispatchCloseEvent();
        }
    }
}