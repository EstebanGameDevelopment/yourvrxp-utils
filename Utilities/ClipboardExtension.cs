using UnityEngine;

namespace yourvrexperience.Utils
{
	public static class ClipboardExtension
	{
		/// <summary>
		/// Puts the string into the Clipboard.
		/// </summary>
		public static void CopyToClipboard(this string str)
		{
			GUIUtility.systemCopyBuffer = str;
		}
	}
}