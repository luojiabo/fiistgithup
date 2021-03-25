mergeInto(LibraryManager.library, {
	Invoke: function (args) {
		var result = call(Pointer_stringify(args));
		var bufferSize = lengthBytesUTF8(result) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(result, buffer, bufferSize);
		return buffer;
	},

	Callback: function (result) {
		onCallback(Pointer_stringify(result));
	}
});
