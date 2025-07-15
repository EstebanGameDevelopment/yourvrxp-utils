using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
#if ENABLE_OCULUS || ENABLE_OPENXR || ENABLE_ULTIMATEXR || ENABLE_NREAL
using yourvrexperience.VR;
#endif

namespace yourvrexperience.Utils
{
	public class BaseScreenView : MonoBehaviour, IScreenView
    {
		public const string EventBaseScreenViewCreated = "EventBaseScreenViewCreated";
		public const string EventBaseScreenViewDestroyed = "EventBaseScreenViewDestroyed";
		public const string EventBaseScreenViewEnableInteraction = "EventBaseScreenViewEnableInteraction";
		public const string EventBaseScreenViewSetCanvasOrder = "EventBaseScreenViewSetCanvasOrder";

		protected Transform _content;
		protected Transform _background;

		protected Canvas _canvas;

		public Transform Content
		{ 
			get { return _content; } 
		}
		public Transform Background
		{ 
			get { return _background; } 
		}

		public virtual string NameScreen 
		{ 
			get { return this.name; }
		}

		public virtual void Initialize(params object[] parameters)
		{
			_content = this.transform.Find("Content");
			_background = this.transform.Find("Background");
			_canvas = this.GetComponent<Canvas>();

			Debug.Assert(_content != null);

			UIEventController.Instance.Event += OnUILocalEvent;
			SystemEventController.Instance.DispatchSystemEvent(EventBaseScreenViewCreated, this.gameObject, NameScreen);
#if ENABLE_OCULUS || ENABLE_OPENXR || ENABLE_ULTIMATEXR || ENABLE_NREAL
			if (VRInputController.Instance != null) VRInputController.Instance.DispatchVREvent(VRInputController.EventVRInputControllerResetAllInputs);
#endif			
		}

		public virtual void ActivateContent(bool value)
		{
			Content.gameObject.SetActive(value);
		}

		public virtual void Destroy()
		{
			if (UIEventController.Instance != null) UIEventController.Instance.Event -= OnUILocalEvent;
			SystemEventController.Instance.DelaySystemEvent(EventBaseScreenViewDestroyed, 0.1f, NameScreen);	
#if ENABLE_OCULUS || ENABLE_OPENXR || ENABLE_ULTIMATEXR || ENABLE_NREAL
			if (VRInputController.Instance != null) VRInputController.Instance.DispatchVREvent(VRInputController.EventVRInputControllerResetAllInputs);
#endif			
		}

        private void OnUILocalEvent(string nameEvent, object[] parameters)
        {
            if (nameEvent.Equals(EventBaseScreenViewEnableInteraction))
			{				
				bool interactivity = (bool)parameters[0];
				yourvrexperience.Utils.Utilities.ApplyEnabledInteraction(this.gameObject.transform, interactivity);
			}
			if (nameEvent.Equals(EventBaseScreenViewSetCanvasOrder))
            {
				string targetName = (string)parameters[0];
				if (NameScreen.IndexOf(targetName) != -1)
                {
					_canvas.sortingOrder = (int)parameters[1];
                }
            }
        }
    }
}