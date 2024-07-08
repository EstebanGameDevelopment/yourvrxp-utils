using System.Collections.Generic;
using UnityEngine;


namespace yourvrexperience.Utils
{

	public interface IHTTPComms
	{
		byte[] RawData { get; }
		string UrlRequest { get; }
		WWWForm FormPost { get; }
		int Method { get; }
		Dictionary<string, string> GetHeaders();

		string Build(params object[] _list);
		void Response(byte[] _response);
		void Response(string _response);
		void CancelResponse();
	}

}