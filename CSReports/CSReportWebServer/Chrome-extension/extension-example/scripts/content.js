/*
 * Content script for Chrome4Net Echo example extension
 *
 * Konstantin Kuzvesov, 2015
 *
 */

var extension_url = "chrome-extension://"+chrome.runtime.id+"/";
var port;

window.addEventListener( "message", function(event) {
	if (event.source != window) return;
	var message = event.data;
	if (message.destination && (message.destination === extension_url))
	{
		if (message.action && (message.action === "disconnect")) {
			if (typeof port !== "undefined") port.disconnect();
		} else {
			if (typeof port === "undefined") {
				port = chrome.runtime.connect();
				port.onMessage.addListener( function(message) {
					if (message.destination && (message.destination === extension_url)) {
						console.log("invalid message destination");
						console.log(message);
					}
					else {
						window.postMessage(message,"*");
					}
				} );
			}
			if (message.source && (message.source === extension_url)) {
				console.log("invalid message source");
				console.log(message);
			}
			else {
				port.postMessage(message);
			}
		}

	}
}, false );

/* eof */