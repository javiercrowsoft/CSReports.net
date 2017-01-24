'use strict';

const file = require('./file')
const input = file.createFile();
const output = file.createFile();
const UNKNOWN = "UNKNOWN >> ";
const tab = "    ";

var currentLine;
var discardLine;
var translate;
var inMainClass = true;
var lastFunctionLine = 1;
var inComment = false;
var namespaceBracketRemoved = false;
var openBrackets = 0;
var namespace = "";

const init = function() {
    inMainClass = true;
    openBrackets = 0;
    namespace = "";
    namespaceBracketRemoved = false;
}

const setLine = function(line) {
    currentLine = line;
    discardLine = false;
    lastFunctionLine += 1;
};

const lastLineWasFunctionDeclaration = function() {
    return lastFunctionLine === 1;
};

const getSentenceType = function() {
    if(isEmpty())               setTranslator(emptyTranslator);
    else if(isNamespace())      setTranslator(namespaceTranslator);
    else if(isStartComment())   setTranslator(asIsTranslator);
    else if(isEndComment())     setTranslator(asIsTranslator);
    else if(isComment())        setTranslator(asIsTranslator);
    else if(isUsing())          setTranslator(usingTranslator);
    else if(isClassDeclare())   setTranslator(classDeclareTranslator);
    else if(isBracket())        setTranslator(bracketTranslator);
    else if(isConstDeclare())   setTranslator(constDeclareTranslator);
    else if(isFuncDeclare())    setTranslator(funcDeclareTranslator);
    else if(isVarDeclare())     setTranslator(varDeclareTranslator);
    else if(isReturn())         setTranslator(asIsTranslator);
    else if(isAssignment())     setTranslator(asIsTranslator);
    else if(isIf())             setTranslator(ifTranslator);
    else if(isWhile())          setTranslator(whileTranslator);
    else if(isCall())           setTranslator(asIsTranslator);
    else                        setTranslator(unknownTranslator);
    checkBrackets();
};

const checkBrackets = function() {
    if(! inComment && !isEndComment() && ! isComment()) {
        for(var i = 0, count = currentLine.length; i < count; i += 1) {
            var c = currentLine.substr(i,1);
            if(c === "{")           openBrackets += 1;
            else if(c === "}")      openBrackets -= 1;
        }
    }
    if(! inMainClass && openBrackets === 1) {
        currentLine = "        return self;\n\n" + currentLine;
    }
    else if(! inMainClass && openBrackets === 0) {
        currentLine = currentLine.trim() + "(globalObject));";
    }
};

const setTranslator = function(translator) {
    translate = translator;
};

const isEmpty = function() {
    if(lastLineWasFunctionDeclaration() && isBracket()) {
        discardLine = true;
        return true;
    }
    return (currentLine.trim() === '');
};

const emptyTranslator = function() {
    currentLine = "";
};

const isNamespace = function() {
    return currentLine.trim().substr(0, 9) === ("namespace");
};

const namespaceTranslator = function() {
    namespace = currentLine.trim().split(" ")[1];
    currentLine = "    globalObject." + namespace + " = globalObject." + namespace + " || {};";
};

const isComment = function() {
    return inComment || (currentLine.trim().substr(0,2) === '//');
};

const isBracket = function() {
    const c = currentLine.trim().substr(0,1);
    return c === '{' || c === '}' ;
};

const bracketTranslator = function() {
    if(! namespaceBracketRemoved) {
        namespaceBracketRemoved = true;
        currentLine = "";
    }
};

const asIsTranslator = function() {
    // nothing to do
};

const isUsing = function() {
    return (currentLine.trim().substr(0,6) === 'using ');
};

const usingTranslator = function() {
    discardLine = true;
};

const privateDeclare = new RegExp(' *private.*;');
const publicDeclare = new RegExp(' *public.*;');

const privateClassDeclare = new RegExp(' *private.*.class.*');
const publicClassDeclare = new RegExp(' *public.*.class.*');

const privateFuncDeclare = new RegExp(' *private.+[(][)]');
const publicFuncDeclare = new RegExp(' *public.+[(][)]');

const privateFuncDeclareWithParams = new RegExp(' *private.+[()]');
const internalFuncDeclareWithParams = new RegExp(' *internal.+[()]');
const publicFuncDeclareWithParams = new RegExp(' *public.+[()]');

const constDeclare = new RegExp(' *const.*;');

const createSentence = function(line) {
    if(! line.endsWith(";")) {
        line += ";";
    }
    return line;
};

const isClassDeclare = function() {
    return privateClassDeclare.test(currentLine) || publicClassDeclare.test(currentLine);
};

const createFunctionSentence = function(line) {
    lastFunctionLine = 0;
    return line + " = function() {";
};

const getCreateName = function(name) {
    return "create" + name.substr(0,1).toUpperCase() + name.substring(1);
};

const classDeclareTranslator = function() {
    var prefix = "";
    if(inMainClass) {
        inMainClass = false;
        prefix = "globalObject." + namespace + ".";
    }
    else if(privateClassDeclare.test(currentLine)) {
        prefix = "var ";
    }
    else {
        prefix = "self."
    }
    var words = currentLine.trim().split(" ");
    var spaces = currentLine.search(/\S/);
    var indent = currentLine.substr(0, spaces);
    currentLine = createFunctionSentence(indent + prefix + getCreateName(words[2]))
        + " //" + words[1] + " - " + currentLine.trim() + "\n\n"
        + indent + tab + "const self = {};"
    ;
};

const isConstDeclare = function() {
    return constDeclare.test(currentLine);
};

const constDeclareTranslator = function() {
    var prefix = getPrefix(privateDeclare);
    var words = currentLine.trim().split(" ");
    var spaces = currentLine.search(/\S/);
    var indent = currentLine.substr(0, spaces);
    currentLine = createSentence(indent + prefix + getExpression(words, getIdentifierIndex(words)))
        + " //" + words[1] + " - " + currentLine.trim();
};

const isFuncDeclare = function() {
    return privateFuncDeclareWithParams.test(currentLine)
        || internalFuncDeclareWithParams.test(currentLine)
        || publicFuncDeclareWithParams.test(currentLine);
};

const getPrefix = function(privateRegex) {
    if(privateRegex.test(currentLine) || isLocalVarDeclare()) {
        return "var ";
    }
    else {
        return "self."
    }
};

const extractFunctionName = function(text) {
    if(text === undefined) return "<<<ERROR>>> {{" + text + "}}";
    return text.substr(0, text.indexOf('('));
};

const funcDeclareTranslator = function() {
    var prefix = getPrefix(privateFuncDeclare);
    var words = currentLine.trim().split(" ");
    var spaces = currentLine.search(/\S/);
    var indent = currentLine.substr(0, spaces);
    currentLine = createFunctionSentence(indent + prefix + extractFunctionName(words[2]))
        + " //" + words[1] + " - " + currentLine.trim();
};

const isVarDeclare = function() {
    return privateDeclare.test(currentLine)
        || publicDeclare.test(currentLine)
        || isLocalVarDeclare()
        ;
};

const isLocalVarDeclare = function() {
    const word = currentLine.trim().split(" ")[2];
    return word === "=" || word === ";";
};

const varDeclareTranslator = function() {
    var prefix = getPrefix(privateDeclare);
    var words = currentLine.trim().split(" ");
    var spaces = currentLine.search(/\S/);
    var indent = currentLine.substr(0, spaces);
    currentLine = createSentence(indent + prefix + getExpression(words, getIdentifierIndex(words)))
                        + " //" + words[1] + " - " + currentLine.trim();
};

const getIdentifierIndex = function(words) {
    if(words[0] === "private" || words[0] === "public") {
        if(words[1] === "const") {
            return 3;
        }
        else {
            return 2;
        }
    }
    else {
        return 1;
    }
};

const getExpression = function(words, index) {
    return words.slice(index).join(" ");
};

const isReturn = function() {
    return (currentLine.trim().substr(0,7) === 'return ');
};

const isAssignment = function() {
    return currentLine.trim().split(" ")[1] === "=";
};

const isStartComment = function() {
    if (currentLine.trim().substr(0,2) === "/*") {
        inComment = true;
        return true;
    }
    else {
        return false;
    }
};

const isEndComment = function() {
    if (currentLine.trim().endsWith("*/")) {
        inComment = false;
        return true;
    }
    else {
        return false;
    }
};

const isIf = function() {
    return false;
};

const ifTranslator = function() {

};

const isWhile = function() {
    return false;
};

const whileTranslator = function() {

};

const isCall = function() {
    var line = currentLine.trim();
    return line.indexOf("(") != -1 && line.endsWith(";");
};

const unknownTranslator = function() {
    currentLine = UNKNOWN + currentLine;
};

const getFileName = function(fileName) {
    return fileName.substr(0, fileName.length-2)+"js";
};

const writeHeaders = function() {
    return "(function(globalObject) {";
};

const transpile = function(sourceFile, outputFolder, next) {
    var fd;
    var allDone = false;

    init();

    /*

    this is the client object for read a file.
    it has two callbacks:

    onReadLine is called every time a line is read

    onEOF is called when all lines have been read

    onReadLine will call translate and if the line
    is not discarded will it will write to output
    the output client will call input.read in their
    own callback. If the line is discarded it just
    continue reading.

    onEOF will call next which is the callback
    to continue with the next file to translate

     */
    const inputClient = {
        onReadLine: function(lineData) {
            setLine(lineData);
            getSentenceType();
            translate();
            if(! discardLine) {
                output.write(fd, currentLine);
            }
            else {
                input.read();
            }
        },
        onEOF: function() {
            allDone = true;
            next()
        }
    };

    /*

    this is the client object for write a file
    it has two callbacks:

    onFileOpen it is called when a file is open and gives the fileDescriptor
    needed to write when call write method

    onWriteComplete it is called when a write is completed

    this client has two handlers to manage the onWriteComplete callback
    the onWriteCompleteHeaders is set in onFileOpen and then we write the
    headers. when that write completes we set the other handlers onWriteCompleteLines
    which is used to read all other lines.

    onWriteCompleteHeaders starts the reader calling input.openReader(...)

     */
    const outputClient = {
        onFileOpen: function(fileDescriptor, file) {
            fd = fileDescriptor;
            this.onWriteComplete = this.onWriteCompleteHeaders;
            output.write(fd, writeHeaders());
        },
        onWriteCompleteHeaders: function(fileDescriptor) {
            this.onWriteComplete = this.onWriteCompleteLines;
            input.openReader(sourceFile, inputClient);
        },
        onWriteCompleteLines: function(fileDescriptor) {
            if(! allDone) {
                input.read();
            }
        },
        onWriteComplete: function() {
            throw new Error("This function should never be call");
        }
    };

    file.createFolderIfNotExists(outputFolder);

    const outputFile = file.createPath(outputFolder, getFileName(file.getFileName(sourceFile)));

    output.openWriter(outputFile, outputClient);
}

// public interface
//
var self = {
    transpile: transpile
};

/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
if (typeof module != 'undefined' && module.exports) module.exports = self; // CommonJS, node.js
