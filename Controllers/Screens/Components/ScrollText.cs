using System;
using System.Collections;
using System.Collections.Generic;
using yourvrexperience.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if ENABLE_OCULUS || ENABLE_OPENXR || ENABLE_ULTIMATEXR || ENABLE_NREAL
using yourvrexperience.VR;
#endif

namespace yourvrexperience.Utils
{
	public class ScrollText : MonoBehaviour
	{
        public const string EventScrollTextStop = "EventScrollTextStop";

        private ScrollRect _scrollRect;
        private TextMeshProUGUI _textMeshPro;
        private float _scrollDuration;
        private float _totalHeight;

        private IEnumerator _thread;

        public static void PlayScroll(ScrollRect scrollRect, TextMeshProUGUI textMeshPro, float delayToStart, float scrollDuration)
        {
            GameObject playScroll = new GameObject();
            playScroll.AddComponent<ScrollText>().Init(scrollRect, textMeshPro, delayToStart, scrollDuration);
        }

        public void Init(ScrollRect scrollRect, TextMeshProUGUI textMeshPro, float delayToStart, float scrollDuration)
        {
            _scrollRect = scrollRect;
            _textMeshPro = textMeshPro;
            _scrollDuration = scrollDuration;

            ResizeTextMeshPro();
            Vector2 startPosition = Vector2.zero - new Vector2(0, _totalHeight/2);
            _scrollRect.content.anchoredPosition = startPosition;
            _thread = WaitBeforeScrollCoroutine(delayToStart);
            StartCoroutine(_thread);
            SystemEventController.Instance.Event += OnSystemEvent;

            GameObject.Destroy(this.gameObject, _scrollDuration);
        }

        private void OnDestroy()
        {
            if (SystemEventController.Instance != null) SystemEventController.Instance.Event -= OnSystemEvent;

            _scrollRect = null;
            _textMeshPro = null;
            if (_thread != null) StopCoroutine(_thread);
            _thread = null;
        }

        private void OnSystemEvent(string nameEvent, object[] parameters)
        {            
            if (nameEvent.Equals(EventScrollTextStop))
            {
                GameObject.Destroy(this.gameObject);
            }
        }

        private void ResizeTextMeshPro()
        {
            Canvas.ForceUpdateCanvases();

            _textMeshPro.ForceMeshUpdate();
            RectTransform textRectTransform = _textMeshPro.GetComponent<RectTransform>();

            Vector2 preferredValues = _textMeshPro.GetPreferredValues(_textMeshPro.text, textRectTransform.rect.width, 0);
            _totalHeight = preferredValues.y;
            
            // Debug.LogError(" //////////////////////// RESIZE["+ textRectTransform.rect.width + "," + _totalHeight + "]");

            textRectTransform.sizeDelta = new Vector2(textRectTransform.sizeDelta.x, _totalHeight);
        }

        private IEnumerator WaitBeforeScrollCoroutine(float delayToStart)
        {
            yield return new WaitForSeconds(delayToStart);

            StartCoroutine(ScrollCoroutine());
        }

        private IEnumerator ScrollCoroutine()
        {
            float elapsedTime = 0f;
            Vector2 startPosition = Vector2.zero - new Vector2(0, _totalHeight / 2);
            Vector2 endPosition = new Vector2(0, _totalHeight / 2);

            while (elapsedTime < _scrollDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / _scrollDuration);
                _scrollRect.content.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t);
                yield return null;
            }

            _scrollRect.content.anchoredPosition = endPosition;
        }
    }
}