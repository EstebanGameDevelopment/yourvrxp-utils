using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using yourvrexperience.Narration;
using yourvrexperience.Utils;

namespace yourvrexperience.Utils
{
    public class GetFileDataHTTP : BaseDataHTTP, IHTTPComms
	{
		public const string EventGetFileDataHTTP = "EventGetFileDataHTTP";

		public string UrlRequest
		{			            
			get { return ""; }
        }

        public string Build(params object[] _list)
		{
			return (string)_list[0];
        }

        public override void Response(string _response)
		{
			if (!ResponseCode(_response))
			{
				SystemEventController.Instance.DispatchSystemEvent(EventGetFileDataHTTP, false);
				return;
			}

			SystemEventController.Instance.DispatchSystemEvent(EventGetFileDataHTTP, true, _response);
		}
	}

}