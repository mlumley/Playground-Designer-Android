var FacebookPopUp = {
  FacebookLogInCaptureClick: function(_contentURL, _contentTitle, _contentDesc, _contentPhoto, _callbackId) {
	var contentURL = Pointer_stringify(_contentURL);
	var contentTitle = Pointer_stringify(_contentTitle);
	var contentDesc = Pointer_stringify(_contentDesc);
	var contentPhoto = Pointer_stringify(_contentPhoto);
 	var callbackId = Pointer_stringify(_callbackId);
 	console.log("Logging Facebook with permissions " + permissions);
     var OpenFacebookLoginPopup = function() {
 		FBUnity.sharelink(contentURL, contentTitle, contentDesc, contentPhoto, callbackId);
 		document.getElementById('canvas').removeEventListener('click', OpenFacebookLoginPopup);
     };
     document.getElementById('canvas').addEventListener('click', OpenFacebookLoginPopup, false);
   }
};
mergeInto(LibraryManager.library, FacebookPopUp);