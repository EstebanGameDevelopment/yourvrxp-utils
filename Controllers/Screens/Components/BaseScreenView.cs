using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
#if ENABLE_OCULUS || ENABLE_OPENXR || ENABLE_ULTIMATEXR
using yourvrexperience.VR;
#endif

namespace yourvrexperience.Utils
{
	public class BaseScreenView : MonoBehaviour, IScreenView
    {
		public const string EventBaseScreenViewCreated = "EventBaseScreenViewCreated";
		public const string EventBaseScreenViewDestroyed = "EventBaseScreenViewDestroyed";
		public const string EventBaseScreenViewEnableInteraction = "EventBaseScreenViewEnableInteraction";

		protected Transform _content;
		protected Transform _background;

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

			Debug.Assert(_content != null);

			UIEventController.Instance.Event += OnUILocalEvent;
			SystemEventController.Instance.DispatchSystemEvent(EventBaseScreenViewCreated, this.gameObject);
#if ENABLE_OCULUS || ENABLE_OPENXR || ENABLE_ULTIMATEXR
			if (VRInputController.Instance != null) VRInputController.Instance.DispatchVREvent(VRInputController.EventVRInputControllerResetAllInputs);
#endif			
		}

		public virtual void Destroy()
		{
			if (UIEventController.Instance != null) UIEventController.Instance.Event -= OnUILocalEvent;
			SystemEventController.Instance.DelaySystemEvent(EventBaseScreenViewDestroyed, 0.1f, NameScreen);	
#if ENABLE_OCULUS || ENABLE_OPENXR || ENABLE_ULTIMATEXR
			if (VRInputController.Instance != null) VRInputController.Instance.DispatchVREvent(VRInputController.EventVRInputControllerResetAllInputs);
#endif			
		}

        private void OnUILocalEvent(string nameEvent, object[] parameters)
        {
            if (nameEvent.Equals(EventBaseScreenViewEnableInteraction))
			{				
				bool interactivity = (bool)parameters[0];
				Utilities.ApplyEnabledInteraction(this.gameObject.transform, interactivity);
			}
        }
    }
}