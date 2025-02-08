using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;

namespace yourvrexperience.Utils
{
	public static class OpenLinkController
	{
		public static void OpenLink(string url)
		{
			Application.OpenURL(url);
		}

		public static void OpenLinkJSPluginNewTab(string url)
		{
#if !UNITY_EDITOR
		OpenInNewTab(url);
#endif
		}
		public static void OpenLinkJSPluginSameTab(string url)
		{
#if !UNITY_EDITOR
		OpenInSameTab(url);
#endif
		}

		[DllImport("__Internal")]
		public static extern void OpenInNewTab(string url);
		[DllImport("__Internal")]
		public static extern void OpenInSameTab(string url);

	}
}