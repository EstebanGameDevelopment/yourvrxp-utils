using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace yourvrexperience.Utils
{
    public class LanguageController : MonoBehaviour
    {
        public const string EventLanguageControllerChangedCodeLanguage = "EventLanguageControllerChangedCodeLanguage";

        public const string CodeLanguageEnglish = "en";
        public const string CodeLanguageSpanish = "es";
        public const string CodeLanguageCatalan = "ca";

        private static LanguageController _instance;
        public static LanguageController Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = GameObject.FindObjectOfType<LanguageController>();
                }
                return _instance;
            }
        }

        public TextAsset GameTexts;
        public string CodeLanguage = "en";

        private Hashtable m_texts = new Hashtable();

		void Start()
		{
			SystemEventController.Instance.Event += OnSystemEvent;
		}

		void OnDestroy()
		{
			if (SystemEventController.Instance != null) SystemEventController.Instance.Event -= OnSystemEvent;
		}

		private void Destroy()
		{
			if (Instance)
			{
				LanguageController reference = _instance;
				_instance = null;
				GameObject.Destroy(reference.gameObject);
			}
		}

        private void LoadGameTexts()
        {
            if (m_texts.Count != 0) return;
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(GameTexts.text);
            XmlNodeList textsList = xmlDoc.GetElementsByTagName("text");
            foreach (XmlNode textEntry in textsList)
            {
                XmlNodeList textNodes = textEntry.ChildNodes;
                string idText = textEntry.Attributes["id"].Value;
                m_texts.Add(idText, new TextEntry(idText, textNodes));
            }
        }

        public string GetText(string id)
        {
            LoadGameTexts();
            if (m_texts[id] != null)
            {
                return ((TextEntry)m_texts[id]).GetText(CodeLanguage);
            }
            else
            {
                return id;
            }
        }

        public string GetText(string _id, params object[] _list)
		{
			LoadGameTexts();
			if (m_texts[_id] != null)
			{
				string buffer = ((TextEntry)m_texts[_id]).GetText(CodeLanguage);
				string result = "";
				for (int i = 0; i < _list.Length; i++)
				{
					string valueThing = (_list[i]).ToString();
					int indexTag = buffer.IndexOf("~");
					if (indexTag != -1)
					{
						result += buffer.Substring(0, indexTag) + valueThing;
						buffer = buffer.Substring(indexTag + 1, buffer.Length - (indexTag + 1));
					}
				}
				result += buffer;
				return result;
			}
			else
			{
				return _id;
			}
		}        

        public void ChangeLanguage(string newCodeLanguage)
        {
            bool reportToSystemLanguageChange = false;
            if (CodeLanguage != newCodeLanguage)
            {
                reportToSystemLanguageChange = true;
            }
            CodeLanguage = newCodeLanguage;
            if (reportToSystemLanguageChange)
            {
                SystemEventController.Instance.DispatchSystemEvent(EventLanguageControllerChangedCodeLanguage);
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
    }
}