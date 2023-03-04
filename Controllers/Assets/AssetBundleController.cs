
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace yourvrexperience.Utils
{
    public delegate void AssetBundleEventHandler(string nameEvent, params object[] parameters);

    /******************************************
	 * 
	 * AssetbundleController
	 * 
	 * Manager of the asset bundle
	 */
    public class AssetBundleController : MonoBehaviour
    {
        public event AssetBundleEventHandler AssetBundleEvent;

        public const string EventAssetBundleAssetsLoaded     = "EventAssetBundleAssetsLoaded";
        public const string EventAssetBundleAssetsProgress   = "EventAssetBundleAssetsProgress";
        public const string EventAssetBundleAssetsUnknownProgress = "EventAssetBundleAssetsUnknownProgress";
        public const string EventAssetBundleLevelXML         = "EventAssetBundleLevelXML";
        public const string EventAssetBundleOneTimeLoadingAssets = "EventAssetBundleOneTimeLoadingAssets";

        public const string CoockieLoadedAssetBundle = "CoockieLoadedAssetBundle";

        private static AssetBundleController _instance;

        public static AssetBundleController Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = GameObject.FindObjectOfType(typeof(AssetBundleController)) as AssetBundleController;
                }
                return _instance;
            }
        }

		[SerializeField] private string AssetBundleURL = "https://www.yourvrexperience.com/mygames/multiuserapp/multiuserapp";
		[SerializeField] private int AssetBundleVersion = 1;

        private bool _isLoadinAnAssetBundle = false;
        private List<ItemMultiObjectEntry> _loadBundles = new List<ItemMultiObjectEntry>();
        private List<string> _urlsBundle = new List<string>();
        private List<AssetBundle> _assetBundle = new List<AssetBundle>();
        private List<TimedEventData> listEvents = new List<TimedEventData>();

        private Dictionary<string, Object> _loadedObjects = new Dictionary<string, Object>();
		
		void Start()
		{
			SystemEventController.Instance.Event += OnSystemEvent;
		}

        void OnDestroy()
        {
            Destroy();
        }

        public void Destroy()
        {
            if (Instance != null)
            {
				AssetBundleController reference = _instance;
				_instance = null;
                Destroy(reference.gameObject);
                _loadedObjects.Clear();                

				if (SystemEventController.Instance != null) SystemEventController.Instance.Event -= OnSystemEvent;
            }
        }

        public void DispatchAssetBundleEvent(string nameEvent, params object[] parameters)
        {
            if (AssetBundleEvent != null) AssetBundleEvent(nameEvent, parameters);
        }

        public void DelayBasicSystemEvent(string nameEvent, float time, params object[] parameters)
        {
            listEvents.Add(new TimedEventData(nameEvent, time, parameters));
        }

        public bool LoadAssetBundle(string url = "", int version = -1)
        {
			string finalURL = url;
			int finalVersion = version;
			if ((finalURL == null) || (finalURL.Length == 0))
			{
				finalURL = AssetBundleURL;
			}
			if (version == -1)
			{
				finalVersion = AssetBundleVersion;
			}
            if (!_urlsBundle.Contains(finalURL))
            {
                _urlsBundle.Add(finalURL);
                DispatchAssetBundleEvent(EventAssetBundleOneTimeLoadingAssets);
                CachedAssetBundle cacheBundle = new CachedAssetBundle();
                StartCoroutine(WebRequestAssetBundle(UnityWebRequestAssetBundle.GetAssetBundle(finalURL, cacheBundle)));
                return false;
            }
            else
            {
                Invoke("AllAssetsLoaded", 0.01f);
                return true;
            }            
        }

        public bool LoadAssetBundles(string[] urls, int version)
        {
            for (int i = 0; i < urls.Length; i++)
            {
                if (!_urlsBundle.Contains(urls[i]))
                {
                    _loadBundles.Add(new ItemMultiObjectEntry(urls[i], version));
                }
            }

            if (_loadBundles.Count == 0)
            {
                Invoke("AllAssetsLoaded", 0.01f);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckAssetsCached()
        {
            return PlayerPrefs.GetInt(CoockieLoadedAssetBundle, -1) == 1;
        }

        public void AllAssetsLoaded()
        {
            _isLoadinAnAssetBundle = true;
            if (_loadBundles.Count == 0)
            {
                PlayerPrefs.SetInt(CoockieLoadedAssetBundle, 1);
                DispatchAssetBundleEvent(EventAssetBundleAssetsLoaded);
            }
        }

        public IEnumerator DownloadAssetBundle(WWW www)
        {
            while (!www.isDone)
            {
                DispatchAssetBundleEvent(EventAssetBundleAssetsProgress, www.progress);
                yield return new WaitForSeconds(.1f);
            }
            _assetBundle.Add(www.assetBundle);
            Invoke("AllAssetsLoaded", 0.01f);
        }

        public IEnumerator WebRequestAssetBundle(UnityWebRequest www)
        {
            DispatchAssetBundleEvent(EventAssetBundleAssetsUnknownProgress, 0);
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                _assetBundle.Add(DownloadHandlerAssetBundle.GetContent(www));
            }
            Invoke("AllAssetsLoaded", 0.01f);
        }

        public GameObject CreateGameObject(string name)
        {
#if UNITY_EDITOR
            Utilities.DebugLogColor("AssetbundleController::CreateGameObject::_name=" + name, Color.red);
#endif
            if (_assetBundle.Count == 0) return null;

            foreach (AssetBundle item in _assetBundle)
            {
                if (item.Contains(name))
                {
                    if (!_loadedObjects.ContainsKey(name))
                    {
                        _loadedObjects.Add(name, item.LoadAsset(name));
                    }
                    return Instantiate(_loadedObjects[name]) as GameObject;
                }
            }
            return null;
        }

        public Sprite CreateSprite(string name)
        {
#if UNITY_EDITOR
            Utilities.DebugLogColor("AssetbundleController::CreateSprite::_name=" + name, Color.red);
#endif
            if (_assetBundle.Count == 0) return null;

            foreach (AssetBundle item in _assetBundle)
            {
                if (item.Contains(name))
                {
                    if (!_loadedObjects.ContainsKey(name))
                    {
                        _loadedObjects.Add(name, item.LoadAsset(name));
                    }
                    return Instantiate(_loadedObjects[name]) as Sprite;
                }
            }
            return null;
        }

        public Texture2D CreateTexture(string name)
        {
#if UNITY_EDITOR
            Utilities.DebugLogColor("AssetbundleController::CreateTexture::_name=" + name, Color.red);
#endif
            if (_assetBundle.Count == 0) return null;

            foreach (AssetBundle item in _assetBundle)
            {
                if (item.Contains(name))
                {
                    if (!_loadedObjects.ContainsKey(name))
                    {
                        _loadedObjects.Add(name, item.LoadAsset(name));
                    }
                    return Instantiate(_loadedObjects[name]) as Texture2D;
                }
            }

            return null;
        }

        public Material CreateMaterial(string name)
        {
#if UNITY_EDITOR
            Utilities.DebugLogColor("AssetbundleController::CreateMaterial::_name=" + name, Color.red);
#endif
            if (_assetBundle.Count == 0) return null;

            foreach (AssetBundle item in _assetBundle)
            {
                if (item.Contains(name))
                {
                    if (!_loadedObjects.ContainsKey(name))
                    {
                        _loadedObjects.Add(name, item.LoadAsset(name));
                    }
                    return Instantiate(_loadedObjects[name]) as Material;
                }
            }

            return null;
        }

        public AudioClip CreateAudioclip(string name)
        {
            if (_assetBundle.Count == 0) return null;

            foreach (AssetBundle item in _assetBundle)
            {
                if (item.Contains(name))
                {
                    if (!_loadedObjects.ContainsKey(name))
                    {
                        _loadedObjects.Add(name, item.LoadAsset(name));
                    }
                    return Instantiate(_loadedObjects[name]) as AudioClip;
                }
            }

            return null;
        }

        public void ClearAssetBundleEvents(string _nameEvent = "")
        {
            if (_nameEvent.Length == 0)
            {
                for (int i = 0; i < listEvents.Count; i++)
                {
                    listEvents[i].Time = -1000;
                }
            }
            else
            {
                for (int i = 0; i < listEvents.Count; i++)
                {
                    TimedEventData eventData = listEvents[i];
                    if (eventData.NameEvent == _nameEvent)
                    {
                        eventData.Time = -1000;
                    }
                }
            }
        }

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

        void Update()
        {
            if (!_isLoadinAnAssetBundle)
            {
                if (_loadBundles.Count > 0)
                {
                    string assetBundleURL = (string)_loadBundles[0].Objects[0];
                    int assetBundleVersion = (int)_loadBundles[0].Objects[1];
                    _loadBundles.RemoveAt(0);
                    _isLoadinAnAssetBundle = true;
                    if (LoadAssetBundle(assetBundleURL, assetBundleVersion))
                    {
                        _isLoadinAnAssetBundle = false;
                    }
                }
            }

            // DELAYED EVENTS
            for (int i = 0; i < listEvents.Count; i++)
            {
                TimedEventData eventData = listEvents[i];
                if (eventData.Time == -1000)
                {
                    eventData.Destroy();
                    listEvents.RemoveAt(i);
                    break;
                }
                else
                {
                    eventData.Time -= Time.deltaTime;
                    if (eventData.Time <= 0)
                    {
                        if ((eventData != null) && (AssetBundleEvent != null))
                        {
                            AssetBundleEvent(eventData.NameEvent, eventData.Parameters);
                            eventData.Destroy();
                        }
                        listEvents.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }
}