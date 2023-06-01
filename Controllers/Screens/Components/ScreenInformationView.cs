using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if ENABLE_OCULUS || ENABLE_OPENXR || ENABLE_ULTIMATEXR
using yourvrexperience.VR;
#endif

namespace yourvrexperience.Utils
{
	public enum ScreenInformationResponses { None = 0, Confirm, Cancel }

    public class ScreenInformationView : BaseScreenView, IScreenView
    {
		public const string EventScreenInformationResponse = "EventScreenInformationResponse";
		public const string EventScreenInformationDestroy = "EventScreenInformationDestroy";
		public const string EventScreenInformationRequestAllScreensDestroyed = "EventScreenInformationRequestAllScreensDestroyed";

        public const string ScreenInformation = "ScreenInformation";
		public const string ScreenInformationBig = "ScreenInformationBig";
		public const string ScreenConfirmation = "ScreenConfirmation";
		public const string ScreenInformationImage = "ScreenInformation";
		public const string ScreenConfirmationImage = "ScreenConfirmationImage";
		public const string ScreenLoading = "ScreenInformationLoading";
		public const string ScreenInput = "ScreenInformationInput";

		private GameObject _origin;
        private string _customOutputEvent = "";
		private CustomInput _inputValue;

		public static void CreateScreenInformation(string screenName, GameObject origin, string title, string description, string customEvent = "", string ok = "", string cancel = "", Image infoImage = null)
		{
			string okText = ok;
			if (okText.Length == 0)
			{
				okText = LanguageController.Instance.GetText("text.ok");
			}
			string cancelText = cancel;
			if (cancelText.Length == 0)
			{
				cancelText = LanguageController.Instance.GetText("text.cancel");
			}
			bool shouldHidePrevious = true;
#if !(ENABLE_OCULUS || ENABLE_OPENXR || ENABLE_ULTIMATEXR)
			shouldHidePrevious = false;
#endif			
			ScreenController.Instance.CreateScreen(screenName, false, shouldHidePrevious, origin, customEvent, title, description, okText, cancelText);
		}

        public override void Initialize(params object[] parameters)
        {
            base.Initialize(parameters);

			_origin = (GameObject)parameters[0];
            _customOutputEvent = (string)parameters[1];
            if (_content.Find("Title") != null) _content.Find("Title").GetComponent<TextMeshProUGUI>().text = (string)parameters[2];
			if (_content.Find("Description") != null) _content.Find("Description").GetComponent<TextMeshProUGUI>().text = (string)parameters[3];
			string textOk = (string)parameters[4];
			string textCancel = (string)parameters[5];

			_inputValue = _content.GetComponentInChildren<CustomInput>();
			if (_inputValue != null)
			{
				_inputValue.OnFocusEvent += OnFocusInputValue;
				_inputValue.text = "";
			}

			if (_content.Find("Image") != null)
			{
				Image imageScreen = _content.Find("Image").GetComponent<Image>();
				imageScreen.sprite = (Sprite)parameters[6];
			}

            if (_content.Find("ButtonOk") != null)
            {
                _content.Find("ButtonOk").GetComponent<Button>().onClick.AddListener(OnConfirmation);
				if (_content.Find("ButtonOk/Text") != null)
				{
					_content.Find("ButtonOk/Text").GetComponent<TextMeshProUGUI>().text = textOk;
				}
            }
            if (_content.Find("ButtonDeny") != null)
            {
                _content.Find("ButtonDeny").GetComponent<Button>().onClick.AddListener(OnCancel);
				if (_content.Find("ButtonDeny/Text") != null)
				{
					_content.Find("ButtonDeny/Text").GetComponent<TextMeshProUGUI>().text = textCancel;
				}
            }
            if (_content.Find("ButtonCancel") != null)
            {
                _content.Find("ButtonCancel").GetComponent<Button>().onClick.AddListener(OnCancel);
            }

#if ENABLE_OCULUS || ENABLE_OPENXR || ENABLE_ULTIMATEXR
			if (_background != null)
			{
				_background.gameObject.SetActive(false);
			}
#else
			this.GetComponent<Canvas>().sortingOrder = 1;			
#endif

			UIEventController.Instance.Event += OnUIEvent;
        }

		public override void Destroy()
        {
			if (_content != null)
			{
				_content = null;
				_origin = null;
				if (_inputValue != null)
				{
					_inputValue.OnFocusEvent -= OnFocusInputValue;
				}				
				_inputValue = null;
				if (UIEventController.Instance != null) UIEventController.Instance.Event -= OnUIEvent;
				base.Destroy();
			}
        }

        private void OnFocusInputValue()
        {
#if ENABLE_OCULUS || ENABLE_OPENXR || ENABLE_ULTIMATEXR
			_content.gameObject.SetActive(false);
			ScreenController.Instance.CreateScreen(ScreenVRKeyboardView.ScreenName, false, true,  _inputValue.gameObject, _inputValue, 200);
#endif			
        }

        private void OnConfirmation()
        {
			if (_inputValue != null)
			{
				if (_customOutputEvent != null)
				{
					if (_customOutputEvent.Length > 0)
					{
						UIEventController.Instance.DispatchUIEvent(_customOutputEvent, _origin, ScreenInformationResponses.Confirm, _inputValue.text);
					}
				}
				UIEventController.Instance.DispatchUIEvent(EventScreenInformationResponse, _origin, ScreenInformationResponses.Confirm, _inputValue.text);
			}
			else
			{
				if (_customOutputEvent != null)
				{
					if (_customOutputEvent.Length > 0)
					{
						UIEventController.Instance.DispatchUIEvent(_customOutputEvent, _origin, ScreenInformationResponses.Confirm);
					}
				}
				UIEventController.Instance.DispatchUIEvent(EventScreenInformationResponse, _origin, ScreenInformationResponses.Confirm);
			}			
            UIEventController.Instance.DispatchUIEvent(ScreenController.EventScreenControllerDestroyScreen, this.gameObject);
        }

		private void OnCancel()
        {
            if (_customOutputEvent != null)
            {
                if (_customOutputEvent.Length > 0)
                {
                    SystemEventController.Instance.DispatchSystemEvent(_customOutputEvent, _origin, ScreenInformationResponses.Cancel);
                }
            }
			UIEventController.Instance.DispatchUIEvent(EventScreenInformationResponse, _origin, ScreenInformationResponses.Cancel);
            UIEventController.Instance.DispatchUIEvent(ScreenController.EventScreenControllerDestroyScreen, this.gameObject);
        }

		private void OnUIEvent(string nameEvent, object[] parameters)
		{
			if (nameEvent.Equals(EventScreenInformationDestroy))
			{
				bool shouldDestroy = true;
				if (parameters.Length > 0)
				{
					GameObject target = (GameObject)parameters[0];
					shouldDestroy = (target == this.gameObject);
				}
				if (shouldDestroy)
				{
					UIEventController.Instance.DispatchUIEvent(ScreenController.EventScreenControllerDestroyScreen, this.gameObject);
				}
			}
			if (nameEvent.Equals(EventScreenInformationRequestAllScreensDestroyed))
			{
				UIEventController.Instance.DispatchUIEvent(ScreenController.EventScreenControllerDestroyScreen, this.gameObject);
			}
#if ENABLE_OCULUS || ENABLE_OPENXR || ENABLE_ULTIMATEXR
			if (nameEvent.Equals(ScreenVRKeyboardView.EventScreenVRKeyboardSetNewText))
			{
				if (_inputValue.gameObject == (GameObject)parameters[0])
				{
					_inputValue.text = (string)parameters[1];
				}
			}
#endif			
		}
    }
}