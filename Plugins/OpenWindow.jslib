var OpenWindowPlugin = {
    OpenInNewTab: function(link)
    {
		window.open(Pointer_stringify(link));
    },
	OpenInSameTab: function (link) {
		window.location = Pointer_stringify(link);
		// or window.location.replace(Pointer_stringify(link));
	}	
};

mergeInto(LibraryManager.library, OpenWindowPlugin);