// This file is required by the index.html file and will
// be executed in the renderer process for that window.
// All of the Node.js APIs are available in this process.
const ipc = require('electron').ipcRenderer;

const selectDirOutputBtn = document.getElementById('output-select-directory');
const selectDirBtn = document.getElementById('select-directory');
const csharpFiles = document.getElementById('csharp-files');
const transpileBtn = document.getElementById('transpile');

const fs = require('fs');

const transpiler = require('./transpiler');

const file = require('./file');

var outputFolder = "/home/javier/Work/CrowSoft/CSReports.net/JavaScript/CSReports";
var sourceFolder = "/home/javier/Work/CrowSoft/CSReports.net/CSReports";
var sourceFolderLength = 0;
var csfiles = [];
var globals = [];

document.getElementById('output-selected-file').innerHTML = `You selected: ${outputFolder}`;
document.getElementById('selected-file').innerHTML = `You selected: ${sourceFolder}`;

var getOutputFolder = function(path) {
    path = path.split(file.pathSep);
    return file.createPath(outputFolder, path.slice(sourceFolderLength, path.length-1).join(file.pathSep));
};

selectDirOutputBtn.addEventListener('click', function (event) {
    ipc.send('open-output-file-dialog');
});

selectDirBtn.addEventListener('click', function (event) {
    ipc.send('open-file-dialog');
});

transpileBtn.addEventListener('click', function() {

    document.getElementById('work-complete').style.display = "none";
    document.getElementById('load-objects-complete').style.display = "none";

    sourceFolderLength = sourceFolder.split("/").length;

    while (csharpFiles.firstChild) {
        csharpFiles.removeChild(csharpFiles.firstChild);
    }

    if(outputFolder === "") {
        var li = document.createElement('li');
        li.appendChild(document.createTextNode("The output folder can't be blank"));
        csharpFiles.appendChild(li);
    }
    else if(sourceFolder === outputFolder) {
        var li = document.createElement('li');
        li.appendChild(document.createTextNode("The source and destinaton can't be the same"));
        csharpFiles.appendChild(li);
    }
    else {
        csfiles = [];
        readFolder(sourceFolder);
        loadGlobalsFromFiles(csfiles,() => readFiles(csfiles));
    }
});

const readFolder = function(path) {
    if(fs.lstatSync(path).isDirectory()) {
        const files = fs.readdirSync(path + file.pathSep);
        for(var i = 0; i < files.length; i += 1) {
            var fileOrFolder = files[i];
            readFolder(path + file.pathSep + fileOrFolder);
        }
    }
    else {
        if(path.toLowerCase().endsWith(".cs")) {
            csfiles.push(path);
        }
    }
};

const loadGlobalsFromFiles = function(files, next) {
    var index = 0;
    const nextFile = function(objects) {

        if(objects !== undefined) {
            globals = globals.concat(objects);
        }

        if(index < files.length) {
            loadGlobals(files[index], nextFile);
            index += 1;
        }
        else {
            document.getElementById('load-objects-complete').style.display = "block";
            next();
        }
    };
    nextFile();
};

const readFiles = function(files) {
    var index = 0;
    const nextFile = function() {
        if(index < files.length) {
            translateFile(files[index], nextFile);
            index += 1;
        }
        else {
            document.getElementById('work-complete').style.display = "block";
        }
    };
    nextFile();
};

const updateProgress = function(file) {
    // add to interface
    //
    var li = document.createElement('li');
    li.appendChild(document.createTextNode(file));
    csharpFiles.appendChild(li);
    li.parentNode.parentNode.scrollTop = li.offsetTop;
}

const loadGlobals = function(file, next) {

    updateProgress(file);

    // load globals from this file the file
    //
    transpiler.loadGlobals(file, getOutputFolder(file), next);
};

const translateFile = function(file, next) {

    updateProgress(file);

    const printOriginalCode = document.getElementById('print-original-code').checked;

    // transpile the file
    //
    transpiler.transpile(file, getOutputFolder(file), next, printOriginalCode, globals);
};


ipc.on('output-selected-directory', function (event, path) {
    document.getElementById('output-selected-file').innerHTML = `You selected: ${path}`;
    outputFolder = path[0];
});

ipc.on('selected-directory', function (event, path) {
    document.getElementById('selected-file').innerHTML = `You selected: ${path}`;
    sourceFolder = path[0];
});
