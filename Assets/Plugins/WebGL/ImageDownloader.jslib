var ImageDownloaderPlugin = {
  ImageDownloaderCaptureClick: function(imageName, b64Data) {

    var binary_string =  window.atob(Pointer_stringify(b64Data));
    var len = binary_string.length;
    var bytes = new Uint8Array( len );
    for (var i = 0; i < len; i++)        {
        bytes[i] = binary_string.charCodeAt(i);
    }

		var blob = new Blob([bytes]);
		var link = document.createElement('a');
		link.href = window.URL.createObjectURL(blob);
		var imageName = Pointer_stringify(imageName) + ".png";
		link.download = imageName;
		link.click();
	}
};
mergeInto(LibraryManager.library, ImageDownloaderPlugin);