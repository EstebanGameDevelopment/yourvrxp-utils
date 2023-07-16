using System;
using System.Collections;
using System.Collections.Generic;
using yourvrexperience.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if ENABLE_OCULUS || ENABLE_OPENXR || ENABLE_ULTIMATEXR
using yourvrexperience.VR;
#endif

namespace yourvrexperience.Utils
{
	public class ScreenInputDescription : BaseScreenView, IScreenView
	{
		public const string ScreenNameSession = "ScreenInputDescription";

		public const string ScreenInputDescriptionEnteredValue = "ScreenInputDescriptionEnteredValue";

		[SerializeField] private TextMeshProUGUI Title;
		[SerializeField] private CustomInput inputNameObject;
		[SerializeField] private TextMeshProUGUI Description;
		[SerializeField] private CustomInput inputDescriptionObject;
		[SerializeField] private Button buttonConfirm;
		[SerializeField] private Button buttonCancel;

		private GameObject _source;

		public override string NameScreen 
		{ 			
			get { return ScreenNameSession; }
		}

		public override void Initialize(params object[] parameters)
		{
			base.Initialize(parameters);

			_source = (GameObject)parameters[0];			

			Title.text = (string)parameters[1];
			inputNameObject.text = (string)parameters[2];

			Description.text = (string)parameters[3];
			inputDescriptionObject.text = (string)parameters[4];

			inputNameObject.OnFocusEvent += OnFocusEvent;

			buttonConfirm.onClick.AddListener(OnButtonConfirmChange);
			buttonConfirm.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = LanguageController.Instance.GetText("text.confirm");

			buttonCancel.onClick.AddListener(OnButtonCancel);
			buttonCancel.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = LanguageController.Instance.GetText("text.cancel");

			UIEventController.Instance.Event += OnUIEvent;
		}

		private void OnFocusEvent()
		{
#if ENABLE_OCULUS || ENABLE_OPENXR || ENABLE_ULTIMATEXR
			ScreenController.Instance.CreateScreen(ScreenVRKeyboardView.ScreenName, false, true,  inputNameObject.gameObject, inputNameObject, 100);
#endif			
		}

		private void OnButtonConfirmChange()
		{
			UIEventController.Instance.DispatchUIEvent(ScreenInputDescriptionEnteredValue, _source, true, inputNameObject.text, inputDescriptionObject.text);
			UIEventController.Instance.DispatchUIEvent(ScreenController.EventScreenControllerDestroyScreen, this.gameObject);
		}

		private void OnButtonCancel()
		{
			UIEventController.Instance.DispatchUIEvent(ScreenInputDescriptionEnteredValue, _source, false);
			UIEventController.Instance.DispatchUIEvent(ScreenController.EventScreenControllerDestroyScreen, this.gameObject);
		}

		public override void Destroy()
		{
			base.Destroy();

			_source = null;
			if (UIEventController.Instance != null) UIEventController.Instance.Event -= OnUIEvent;
		}

		public void SetTitle(string title)
		{
			inputNameObject.text = title;
		}

		public void SetDescription(string description)
		{
			inputDescriptionObject.text = description;
		}

		private void OnUIEvent(string nameEvent, object[] parameters)
		{
#if ENABLE_OCULUS || ENABLE_OPENXR || ENABLE_ULTIMATEXR			
			if (nameEvent.Equals(ScreenVRKeyboardView.EventScreenVRKeyboardSetNewText))
			{
				if (inputNameObject.gameObject == (GameObject)parameters[0])
				{
					inputNameObject.text = (string)parameters[1];
				}				
			}
#endif			
		}
	}
}