using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace yourvrexperience.Utils
{
	public class BaseScreenView : MonoBehaviour, IScreenView
    {
		public const string EventBaseScreenViewCreated = "EventBaseScreenViewCreated";
		public const string EventBaseScreenViewDestroyed = "EventBaseScreenViewDestroyed";

		protected Transform _content;

		public Transform Content
		{ 
			get { return _content; } 
		}

		public virtual string NameScreen 
		{ 
			get { return this.name; }
		}

		public virtual void Initialize(params object[] parameters)
		{
			_content = this.transform.Find("Content");

			Debug.Assert(_content != null);

			SystemEventController.Instance.DispatchSystemEvent(EventBaseScreenViewCreated, this.gameObject);
		}

		public virtual void Destroy()
		{
			SystemEventController.Instance.DelaySystemEvent(EventBaseScreenViewDestroyed, 0.1f);			
		}
	}
}