const LineByLineReader = require('line-by-line');
const fs = require('fs');
const path = require('path');

var createFile = function () {

    var lineData = null;
    var lr;
    var allDone = false;
    var client;

    // read operations
    //
    var startReader = function(file) {

        console.log('opening: ' + file);

        allDone = false;

        lr = new LineByLineReader(file);

        lr.on('error', function (err) {
            // 'err' contains error object
            console.log('ERROR: ' + err.toString());
        });

        lr.on('line', function (line) {
            // 'line' contains the current line without the trailing newline character.
            //console.log('read: ', line);
            lineData = line;
            lr.pause();
            readLine();
        });

        lr.on('end', function () {
            allDone = true;
            console.log("end");
            client.onEOF();
        });
    };

    var readLine = function() {
        if(allDone) {
            console.log("end");
            client.onEOF();
            return;
        }
        if(lineData === null) {
            lr.resume();
        }
        else {
            var line = lineData;
            lineData = null;
            client.onReadLine(line);
        }
    };

    var openReader = function(file, fileClient) {
        client = fileClient;
        startReader(file);
    };

    // write operations
    //
    var openWriter = function(file, fileClient) {
        client = fileClient;
        fs.open(file, 'w', function(err, fd) {
            if (err) {
                throw 'error opening file: ' + err;
            }
            client.onFileOpen(fd, file);
        });
    };

    var write = function(fileDescriptor, buffer) {
        fs.write(fileDescriptor, buffer + '\n', null, function(err) {
            if(err) {
                return console.log(err);
            }
            client.onWriteComplete(fileDescriptor);
        });
    };

    // public interface
    //
    var self = {
        write: write,
        read: readLine,
        openReader: openReader,
        openWriter: openWriter
    };

    return self;

};

// folder operations
//
var mkdirSync = function (path) {
    try {
        fs.mkdirSync(path);
    } catch(e) {
        if ( e.code != 'EEXIST' ) throw e;
    }
};

var createFolderIfNotExists = function (dirpath) {
    var prefix = "";
    if(dirpath.substr(0,1) === path.sep) {
        prefix = path.sep;
        dirpath = dirpath.substring(1);
    }
    const parts = dirpath.split(path.sep);
    for( var i = 1; i <= parts.length; i++ ) {
        mkdirSync( prefix + parts.slice(0, i).join(path.sep) );
    }
};

// auxiliary file operations
//
var getFileName = function(dirpath) {
    return dirpath.split(path.sep).pop();
};

var createPath = function() {
    return Array.prototype.slice.call(arguments).join(path.sep);
};

// public interface
//
var self = {
    createFile: createFile,
    getFileName: getFileName,
    createFolderIfNotExists: createFolderIfNotExists,
    createPath: createPath,
    pathSep: path.sep
};

/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
if (typeof module != 'undefined' && module.exports) module.exports = self; // CommonJS, node.js
