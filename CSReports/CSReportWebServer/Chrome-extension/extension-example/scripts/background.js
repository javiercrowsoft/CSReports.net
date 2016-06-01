/*
 * Background page script for Chrome4Net Echo example extension
 *
 * Konstantin Kuzvesov, 2015
 *
 */

chrome.runtime.onConnect.addListener( function(port) {
	
	var manifest = chrome.runtime.getManifest();
	port.port = chrome.runtime.connectNative( manifest.name.toLowerCase() );
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