using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IngameDebugConsole;
using UnityEngine.UI;
#if ENABLE_ULTIMATEXR
using UltimateXR.UI.UnityInputModule;
#endif
#if ENABLE_OPENXR
using UnityEngine.XR.Interaction.Toolkit.UI;
#endif
#if ENABLE_OCULUS || ENABLE_OPENXR || ENABLE_ULTIMATEXR
using yourvrexperience.VR;
#endif

namespace yourvrexperience.Utils
{
    public class ScreenController : MonoBehaviour
    {
		public const string EventScreenControllerStarted = "EventScreenControllerStarted";
        public const string EventScreenControllerRequestCameraData = "EventScreenControllerRequestCameraData";
        public const string EventScreenControllerResponseCameraData = "EventScreenControllerResponseCameraData";
		public const string EventScreenControllerDestroyScreen = "EventScreenControllerDestroyScreen";
		public const string EventScreenControllerDestroyAllScreens = "EventScreenControllerDestroyAllScreens";
		public const string EventScreenControllerToggleInGameDebugConsole = "EventScreenControllerToggleInGameDebugConsole";
		public const string EventScreenControllerCreateScreen = "EventScreenControllerCreateScreen";
		public const string EventScreenControllerCreateInformationScreen = "EventScreenControllerCreateInformationScreen";

        private static ScreenController _instance;
        public static ScreenController Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = GameObject.FindObjectOfType<ScreenController>();
                }
                return _instance;
            }
        }

		[SerializeField] private float distanceScreen = 1.2f;
		[SerializeField] private float sizeVRScreen = 0.002f;
		[SerializeField] private float scaleFactor = 0.5f;
		[SerializeField] private float withVRScreen = 1920;
		[SerializeField] private float heightVRScreen = 3414;

        public GameObject[] Screens;

        private List<GameObject> _screensCreated = new List<GameObject>();

		private bool _isInGameDebugConsole = false;
		private Vector3 _forward = Vector3.zero;
        private Vector3 _position = Vector3.zero;
		private float _scale = -1;
		private float _defaultDistance = -1;
		private bool _hasBeenInitialized = false;

		public float DistanceScreen
		{
			get { return distanceScreen; }
		}
		public float SizeVRScreen
		{
			get { return sizeVRScreen; }
		}

		public void Initialize()
		{
			RunInitialization();
		}

        void Start()
        {
			RunInitialization();
        }

		private void RunInitialization()
		{
			if (!_hasBeenInitialized)
			{
				_hasBeenInitialized = true;
				
				SystemEventController.Instance.Event += OnSystemEvent;
				UIEventController.Instance.Event += OnUIEvent;
				
#if ENABLE_OCULUS || ENABLE_OPENXR || ENABLE_ULTIMATEXR
				VRInputController.Instance.Event += OnVREvent;
#endif			
				SystemEventController.Instance.DispatchSystemEvent(EventScreenControllerStarted);

				_isInGameDebugConsole = DebugLogManager.Instance != null;
				if (_isInGameDebugConsole)
				{
					DebugLogManager.Instance.PopupEnabled = false;
				}
			}
		}

        void OnDestroy()
        {
            Destroy();
        }

		void Destroy()
		{
			if (_instance != null)
			{
				_instance = null;
				if (SystemEventController.Instance != null) SystemEventController.Instance.Event -= OnSystemEvent;
				if (UIEventController.Instance != null) UIEventController.Instance.Event -= OnUIEvent;

#if ENABLE_OCULUS || ENABLE_OPENXR || ENABLE_ULTIMATEXR
				if (VRInputController.Instance != null) VRInputController.Instance.Event -= OnVREvent;
#endif			

				GameObject.Destroy(this.gameObject);
			}			
		}

		public int GetTotalNumberScreens()
		{
			return _screensCreated.Count;
		}

#if ENABLE_OCULUS || ENABLE_OPENXR || ENABLE_ULTIMATEXR
		private void OnVREvent(string nameEvent, object[] parameters)
		{
            if (nameEvent.Equals(EventScreenControllerResponseCameraData))
            {
                GameObject targetScreen = (GameObject)parameters[0];
				Vector3 positionCamera = (Vector3)parameters[1];
				Vector3 forwardCamera = (Vector3)parameters[2];
				targetScreen.transform.position = positionCamera + ((_defaultDistance!=-1)?_defaultDistance:DistanceScreen) * forwardCamera;
				targetScreen.transform.forward = forwardCamera;
				targetScreen.transform.localScale = new Vector3(SizeVRScreen, SizeVRScreen, SizeVRScreen);
				if (_forward != Vector3.zero)
				{
					targetScreen.transform.position = positionCamera + _forward * ((_defaultDistance!=-1)?_defaultDistance:DistanceScreen);
					targetScreen.transform.forward = _forward;
				}
				if (_position != Vector3.zero)
				{
					targetScreen.transform.position = _position;
					if (_forward != Vector3.zero)
					{
						targetScreen.transform.forward = _forward;
					}
					else
					{
						targetScreen.transform.forward = forwardCamera;
					}                        
				}
				if (_scale == -1)
				{
					targetScreen.transform.localScale = new Vector3(SizeVRScreen, SizeVRScreen, SizeVRScreen);
				}
				else
				{
					targetScreen.transform.localScale = new Vector3(_scale, _scale, _scale);
				}
				_forward = Vector3.zero;
				_scale = -1;
				_position = Vector3.zero;
				_defaultDistance = -1;
            }
		}
#endif			

		private void OnSystemEvent(string nameEvent, object[] parameters)
        {
			if (nameEvent.Equals(SystemEventController.EventSystemEventControllerReleaseAllResources))
            {
                Destroy();
            }		
			if (nameEvent.Equals(SystemEventController.EventSystemEventControllerDontDestroyOnLoad))
			{
				if (Instance)
				{
					DontDestroyOnLoad(Instance.gameObject);
				}
			}
        }

		private void OnUIEvent(string nameEvent, object[] parameters)
        {
			if (nameEvent.Equals(EventScreenControllerCreateScreen))
			{
				string nameScreen = (string)parameters[0];
				bool destroyPreviousScreen = (bool)parameters[1];
				bool hidePreviousScreen = (bool)parameters[2]; 
				object[] extraParameters = (object[])parameters[3]; 
				CreateScreen(nameScreen, destroyPreviousScreen, hidePreviousScreen, extraParameters);
			}
			if (nameEvent.Equals(EventScreenControllerCreateInformationScreen))
			{
				string screenName = (string)parameters[0];
				GameObject origin = (GameObject)parameters[1];
				string title = (string)parameters[2];
				string description = (string)parameters[3]; 
				string customEvent = ((parameters.Length>4)?(string)parameters[4]:""); 
				string ok = ((parameters.Length>5)?(string)parameters[5]:""); 
				string cancel = ((parameters.Length>6)?(string)parameters[6]:""); 
				Image infoImage = ((parameters.Length>7)?(Image)parameters[7]:null);

				ScreenInformationView.CreateScreenInformation(screenName, origin, title, description, customEvent, ok, cancel, infoImage);
			}
            if (nameEvent.Equals(EventScreenControllerDestroyScreen))
            {
                GameObject screen = (GameObject)parameters[0];
                if (_screensCreated.Contains(screen))
                {
					if (screen.GetComponent<IScreenView>() != null)  screen.GetComponent<IScreenView>().Destroy();
                    _screensCreated.Remove(screen);
                    GameObject.Destroy(screen);
                    screen = null;
					if (_screensCreated.Count > 0)
					{
						IScreenView screenInterface =  _screensCreated[_screensCreated.Count - 1].GetComponent<IScreenView>();
						if (screenInterface != null)
						{
							screenInterface.Content.gameObject.SetActive(true);
						}
					}
                }
            }
			if (nameEvent.Equals(EventScreenControllerDestroyAllScreens))
			{
				DestroyScreens();
			}
			if (nameEvent.Equals(EventScreenControllerToggleInGameDebugConsole))
			{
				if (DebugLogManager.Instance != null)
				{
					if (!DebugLogManager.Instance.IsLogWindowVisible)
					{							
						DebugLogManager.Instance.ShowLogWindow();
#if ENABLE_OCULUS || ENABLE_OPENXR || ENABLE_ULTIMATEXR
						DebugLogManager.Instance.gameObject.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
						DebugLogManager.Instance.gameObject.transform.position = VRInputController.Instance.VRController.HeadController.transform.position + VRInputController.Instance.VRController.HeadController.transform.forward.normalized + new Vector3(0,-0.5f,0);
						DebugLogManager.Instance.gameObject.transform.forward = VRInputController.Instance.VRController.HeadController.transform.forward;
						DebugLogManager.Instance.gameObject.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);
#if ENABLE_OCULUS
						if (DebugLogManager.Instance.gameObject.GetComponent<OVRRaycaster>() == null) DebugLogManager.Instance.gameObject.AddComponent<OVRRaycaster>();
#elif ENABLE_OPENXR
						if (DebugLogManager.Instance.gameObject.GetComponent<TrackedDeviceGraphicRaycaster>() == null) DebugLogManager.Instance.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
#elif ENABLE_ULTIMATEXR						
						if (DebugLogManager.Instance.gameObject.GetComponent<UxrCanvas>() == null) DebugLogManager.Instance.gameObject.AddComponent<UxrCanvas>();
						if (DebugLogManager.Instance.gameObject.GetComponent<UxrLaserPointerRaycaster>() == null) DebugLogManager.Instance.gameObject.AddComponent<UxrLaserPointerRaycaster>();
						DebugLogManager.Instance.gameObject.GetComponent<UxrCanvas>().CanvasInteractionType = UxrInteractionType.LaserPointers;
						DebugLogManager.Instance.gameObject.GetComponentInChildren<Canvas>().renderMode = RenderMode.WorldSpace;
#endif
#endif
					}
					else
					{
						DebugLogManager.Instance.HideLogWindow();
					}
				}
			}
        }

        public void CreateForwardScreen(string nameScreen, Vector3 forward, bool destroyPreviousScreen, bool hidePreviousScreen, params object[] parameters)
        {
            _forward = forward;
            CreateScreen(nameScreen, destroyPreviousScreen, hidePreviousScreen, parameters);
        }

		public void CreateDistanceScreen(string nameScreen, float distance, bool destroyPreviousScreen, bool hidePreviousScreen, params object[] parameters)
        {
            _defaultDistance = distance;
            CreateScreen(nameScreen, destroyPreviousScreen, hidePreviousScreen, parameters);
        }

        public void CreatePositionScreen(string nameScreen, Vector3 position, float scale, bool destroyPreviousScreen, bool hidePreviousScreen, params object[] parameters)
        {
            _position = position;
            _scale = scale;
            CreateScreen(nameScreen, destroyPreviousScreen, hidePreviousScreen, parameters);
        }

        public void CreatePositionForwardScreen(string nameScreen, Vector3 position, Vector3 forward, float scale, bool destroyPreviousScreen, bool hidePreviousScreen, params object[] parameters)
        {
            _position = position;
            _scale = scale;
			_forward = forward;
            CreateScreen(nameScreen, destroyPreviousScreen, hidePreviousScreen, parameters);
        }

        public void CreateWorldScreen(string nameScreen, Vector3 position, Vector3 forward, float scale, bool destroyPreviousScreen, bool hidePreviousScreen, params object[] parameters)
        {
            GameObject newScreen = CreateSingleScreen(nameScreen, destroyPreviousScreen, hidePreviousScreen, parameters);
			newScreen.transform.position = position;
			newScreen.transform.forward = forward;
			newScreen.transform.localScale = new Vector3(scale, scale, scale);
			newScreen.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
        }
		public void CreateScreen(string nameScreen, bool destroyPreviousScreen, bool hidePreviousScreen, params object[] parameters)
        {
			CreateSingleScreen(nameScreen, destroyPreviousScreen, hidePreviousScreen, parameters);
		}
        private GameObject CreateSingleScreen(string nameScreen, bool destroyPreviousScreen, bool hidePreviousScreen, params object[] parameters)
        {
			GameObject newScreen = null;
            if (destroyPreviousScreen)
            {
                DestroyScreens();
            }
			else
			{
				if (hidePreviousScreen)
				{
					foreach (GameObject screen in _screensCreated)
					{
						IScreenView screenInterface = screen.GetComponent<IScreenView>();
						if (screenInterface != null)
						{
							screenInterface.Content.gameObject.SetActive(false);
						}
					}
				}
			}

			bool screenFound = false;
            for (int i = 0; i < Screens.Length; i++)
            {
				if (Screens[i] != null)
				{
					if (Screens[i].name == nameScreen)
					{
						screenFound = true;
						newScreen = Instantiate(Screens[i]);
						newScreen.transform.SetParent(this.transform);
						_screensCreated.Add(newScreen);
						if (newScreen.GetComponent<IScreenView>() != null)  newScreen.GetComponent<IScreenView>().Initialize(parameters);
						ApplyCanvas(newScreen, _defaultDistance);
					}
				}
            }
			if (!screenFound)
			{
				throw new Exception("Screen with the name["+nameScreen+"] has not been found in ScreenController");
			}
			return newScreen;
        }

		public void ApplyCanvas(GameObject newScreen, float defaultDistance = -1)
		{
			_defaultDistance = defaultDistance;
			newScreen.transform.SetParent(this.transform);
#if ENABLE_OCULUS || ENABLE_OPENXR || ENABLE_ULTIMATEXR
			newScreen.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
			newScreen.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
			newScreen.GetComponent<CanvasScaler>().scaleFactor = scaleFactor;			
			newScreen.GetComponent<RectTransform>().sizeDelta = new Vector2(withVRScreen, heightVRScreen);			
#if ENABLE_OCULUS
			newScreen.AddComponent<OVRRaycaster>();
#elif ENABLE_OPENXR
			newScreen.AddComponent<TrackedDeviceGraphicRaycaster>();
#elif ENABLE_ULTIMATEXR
			if (newScreen.GetComponent<UxrCanvas>() == null) newScreen.AddComponent<UxrCanvas>();
			if (newScreen.GetComponent<UxrLaserPointerRaycaster>() == null) newScreen.AddComponent<UxrLaserPointerRaycaster>();
			newScreen.GetComponent<UxrCanvas>().CanvasInteractionType = UxrInteractionType.LaserPointers;
			newScreen.GetComponentInChildren<Canvas>().renderMode = RenderMode.WorldSpace;									
#endif
			VRInputController.Instance.DispatchVREvent(EventScreenControllerRequestCameraData, newScreen);
#endif
		}

        public void DestroyScreens()
        {
            for (int i = 0; i < _screensCreated.Count; i++)
            {
				if (_screensCreated[i].GetComponent<IScreenView>() != null)  _screensCreated[i].GetComponent<IScreenView>().Destroy();
                GameObject.Destroy(_screensCreated[i]);
            }
            _screensCreated.Clear();
        }

		public void Update()
		{
			if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.H))
			{
				UIEventController.Instance.DispatchUIEvent(EventScreenControllerToggleInGameDebugConsole);
			}			
		}
    }
}