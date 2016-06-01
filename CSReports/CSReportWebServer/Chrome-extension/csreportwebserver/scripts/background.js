chrome.runtime.onConnect.addListener( function(port) {

	var manifest = chrome.runtime.getManifest();
	port.port = chrome.runtime.connectNative( "ar.com.crowsoft.csreportwebserver.echo" );
	port.port.port = port;

	port.onMessage.addListener( function(message, sender) {
		return sender.port.postMessage(message);
	} );

	port.port.onMessage.addListener( function(message, sender) {
		return sender.port.postMessage(message);
	} );

	port.onDisconnect.addListener( function(sender) {
		sender.port.disconnect();
	} );

} );

/* eof */
