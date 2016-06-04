chrome.runtime.onConnect.addListener( function(port) {

	// parameter port is connected to content.js
	//

  var nativeAppName = "ar.com.crowsoft.csreportwebserver.echo";

  // port.port is connected to the native app
	//
	port.port = chrome.runtime.connectNative( nativeAppName );

  // this is adding to sender a reference to port
	// in handlers for port.onMessage and port.port.onMessage
	// we receive two parameters. the sender parameter in
	// port.port.onMessage is port.port ( read the line above this comment
	// where por.port is created)
	//
	port.port.port = port;

	// this is to handle message from content.js
	//
	port.onMessage.addListener( function(message, sender) {
		return sender.port.postMessage(message);
	} );

  // this is to handle response from native application
	//
	port.port.onMessage.addListener( function(message, sender) {
		return sender.port.postMessage(message);
	} );

	// this is when content.js disconnect
	//
	port.onDisconnect.addListener( function(sender) {
		sender.port.disconnect();
	} );

} );

/* eof */
