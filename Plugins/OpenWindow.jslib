var OpenWindowPlugin = {
    OpenInNewTab: function(link)
    {
    	var url = Pointer_stringify(link);
        document.onmouseup = function()
        {
        	window.open(url);
        	document.onmouseup = null;
        }
    },
	OpenInSameTab: function (link) {
		window.location = Pointer_stringify(link);
		// or window.location.replace(Pointer_stringify(link));
	}	
};

mergeInto(LibraryManager.library, OpenWindowPlugin);