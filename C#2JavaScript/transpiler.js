'use strict';

const file = require('./file')
const input = file.createFile();
const output = file.createFile();
const UNKNOWN = "UNKNOWN >> ";
const tab = "    ";

var debugString = "while (!pIsEndCallFunction(c, nInner) && nStart < nLenCode);";

var currentLine;
var originalLine;
var discardLine;
var translate;
var inMainClass = true;
var lastFunctionLine = 1;
var inComment = false;
var namespaceBracketRemoved = false;
var openBrackets = 0;
var namespace = "";
var mainClassName = "";
var className = "";
var inFunction = false;
var inForEach = false;
var loopIdentifiers = ["i_", "j_", "t_", "k_"];
var loopBracketIndex = [];
var loopIndex = -1;
var removeNextBracket = 2;
var inMultiLineBlock = false;
var openParenthesis = 0;
var inParamDeclaration = false;
var inMultiLineParameter = false;

const escapeRegExp = function(str) {
    return str.replace(/[.*+?^${}()|[\]\\]/g, "\\$&"); // $& means the whole matched string
};

const replaceAll = function(target, search, replacement) {
    return target.replace(new RegExp(escapeRegExp(search), 'g'), replacement);
};

const init = function() {
    inMainClass = true;
    mainClassName = "";
    openBrackets = 0;
    namespace = "";
    namespaceBracketRemoved = false;
    inMultiLineBlock = false;
    openParenthesis = 0;
};

const setLine = function(line) {
    currentLine = line;
    originalLine = isComment() ? "" : line;
    discardLine = false;
    lastFunctionLine += 1;
    removeNextBracket += 1;
};

const lastLineWasFunctionDeclaration = function() {
    return lastFunctionLine === 1;
};

const getSentenceType = function() {

    if(debugString && currentLine.trim().substr(0, debugString.length) === debugString) {
        debugger;
    }

    if(isEmpty())               setTranslator(emptyTranslator);

    else if(inMultiLineBlock)   setTranslator(inBlockTranslator);

    else if(isNamespace())      setTranslator(namespaceTranslator);
    else if(isStartComment())   setTranslator(asIsTranslator);
    else if(isEndComment())     setTranslator(asIsTranslator);
    else if(isComment())        setTranslator(asIsTranslator);
    else if(isUsing())          setTranslator(usingTranslator);
    else if(isClassDeclare())   setTranslator(classDeclareTranslator);
    else if(isConstructor())    setTranslator(constructorTranslator);
    else if(isBracket())        setTranslator(bracketTranslator);
    else if(isConstDeclare())   setTranslator(constDeclareTranslator);
    else if(isDelegate())       setTranslator(commentLineTranslator);
    else if(isEvent())          setTranslator(commentLineTranslator);
    else if(isFuncDeclare())    setTranslator(funcDeclareTranslator);
    else if(isFor())            setTranslator(forTranslator);
    else if(isVarDeclare())     setTranslator(varDeclareTranslator);
    else if(isReturn())         setTranslator(asIsTranslator);
    else if(isSwitch())         setTranslator(asIsTranslator);
    else if(isCase())           setTranslator(asIsTranslator);
    else if(isBreak())          setTranslator(asIsTranslator);
    else if(isAssignment())     setTranslator(assignmentTranslator);
    else if(isIf())             setTranslator(beginBlockTranslator);
    else if(isElse())           setTranslator(beginBlockTranslator);
    else if(isForEach())        setTranslator(forEachTranslator);
    else if(isWhile())          setTranslator(beginBlockTranslator);
    else if(isDo())             setTranslator(beginBlockTranslator);
    else if(isCall())           setTranslator(asIsTranslator);
    else if(isTry())            setTranslator(beginBlockTranslator);
    else if(isCatch())          setTranslator(catchTranslator);
    else if(isMultilineParam()) setTranslator(multilineParamTranslator);
    else if(isPPorMM())         setTranslator(asIsTranslator);
    else if(isArrayAssign())    setTranslator(asIsTranslator);
    else                        setTranslator(unknownTranslator);
    checkBrackets();
};

const checkBrackets = function() {
    var inString = false;
    var scaped = false;
    if(! inComment && !isEndComment() && ! isComment()) {
        for(var i = 0, count = currentLine.length; i < count; i += 1) {
            var c = currentLine.substr(i,1);
            if(c === "\\" ) {
                scaped = ! scaped;
            }
            if(c === '"' && ! scaped) {
                inString = ! inString;
            }
            if(c !== "\\" ) {
                scaped = false;
            }
            if(! inString) {
                if (c === "{")           openBrackets += 1;
                else if (c === "}")      openBrackets -= 1;
            }
        }
    }
    if(! inMainClass && openBrackets === 1) {
        currentLine = "        return self;\n\n" + currentLine;
    }
    else if(! inMainClass && openBrackets === 0) {
        currentLine = currentLine.trim() + "(globalObject));";
    }
};

const checkParenthesis = function() {
    if(inMultiLineParameter && originalLine.trim().split("//")[0].endsWith(")")) {
        inMultiLineParameter = false;
        inParamDeclaration = false;
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
    if(removeNextBracket === 1 && currentLine.trim() === "{") {
        discardLine = true;
    }
    if(! namespaceBracketRemoved) {
        namespaceBracketRemoved = true;
        currentLine = "";
    }
    else if(inFunction) {
        if(openBrackets === 2 && currentLine.trim().substr(0,1) === "}") {
            currentLine = currentLine.replace("}", "};");
            inFunction = false;
        }
        else if(inForEach) {
            if(openBrackets === loopBracketIndex[loopIndex] && currentLine.trim().substr(0,1) === "}") {
                loopIndex--;
            }
        }
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
const internalClassDeclare = new RegExp(' *internal.*.class.*');
const publicClassDeclare = new RegExp(' *public.*.class.*');

const privateFuncDeclare = new RegExp(' *private.+[(]');
const publicFuncDeclare = new RegExp(' *public.+[(]');

const privateFuncDeclareWithParams = new RegExp(' *private.+[()]');
const protectedFuncDeclareWithParams = new RegExp(' *protected.+[()]');
const internalFuncDeclareWithParams = new RegExp(' *internal.+[()]');
const publicFuncDeclareWithParams = new RegExp(' *public.+[()]');

const validIdentifierCharacter = new RegExp('[a-z_A-Z0-9$]');

const arrayAssignment = new RegExp('\\[*\\]');

const constDeclare = new RegExp(' *const.*;');

const createSentence = function(line) {
    if(! line.endsWith(";")) {
        line += ";";
    }
    return line;
};

const isClassDeclare = function() {
    return privateClassDeclare.test(currentLine)
        || internalClassDeclare.test(currentLine)
        || publicClassDeclare.test(currentLine);
};

const getParams = function(line) {
    var start =-1;
    var end =-1;
    for(var i = 0, count = line.length; i < count; i++) {
        var c = line.substr(i, 1);
        if(c === "(") start = i;
        if(c === ")") end = i;
        if(start !== -1 && end !== -1) break;
    }
    end = end !== -1 ? end : line.length;
    var params = line.substring(start+1, end);
    var words = params.split(",");
    params = [];
    for(var i = 0, count = words.length; i < count; i+=1) {
        var param = words[i].trim().split(" ");
        if(param.length > 2) {
            params.push(param[2]);
        }
        else {
            params.push(param[1]);
        }

    }
    return params.join(", ");
};

const createFunctionSentence = function(line, parameters, isMultiline) {
    parameters = parameters || "";
    lastFunctionLine = 0;
    if(isMultiline)
        return line + " = function(";
    else
        return line + " = function(" + parameters + ") {";
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
    else if(privateClassDeclare.test(currentLine) || internalClassDeclare.test(currentLine)) {
        prefix = "var ";
    }
    else {
        prefix = "self."
    }
    var words = currentLine.trim().split(" ");
    var spaces = currentLine.search(/\S/);
    var indent = currentLine.substr(0, spaces);
    className = words[2] === "class" ? words[3] : words[2];
    if(mainClassName === "") mainClassName = className;
    currentLine = createFunctionSentence(indent + prefix + getCreateName(className))
        + "\n\n"
        + indent + tab + "const self = {};"
    ;
    inFunction = true;
    inParamDeclaration = true;
};

const isConstructor = function() {
    var identifier = currentLine.trim().split(" ")[1];
    if(identifier && (
            (identifier.substr(0, className.length+1) === className + "(")
        ||  (identifier.substr(0, mainClassName.length+1) === mainClassName + "(")
        )
      ) {
        inFunction = true;
        inParamDeclaration = true;
        return true;
    }
    else {
        return false;
    }
};

const constructorTranslator = function() {
    var words = currentLine.trim().split(" ");
    var spaces = currentLine.search(/\S/);
    var indent = currentLine.substr(0, spaces);
    currentLine = createFunctionSentence(indent + "var " + extractFunctionName(words[1]), getParams(currentLine), isMultiline());
};

const isConstDeclare = function() {
    return constDeclare.test(currentLine);
};

const constDeclareTranslator = function() {
    var prefix = getPrefix(privateDeclare);
    var words = currentLine.trim().split(" ");
    var spaces = currentLine.search(/\S/);
    var indent = currentLine.substr(0, spaces);
    currentLine = createSentence(indent + prefix + getExpression(words, getIdentifierIndex(words)));
};

const isFuncDeclare = function() {
    if((privateFuncDeclareWithParams.test(currentLine)
        || internalFuncDeclareWithParams.test(currentLine)
        || publicFuncDeclareWithParams.test(currentLine)
        || protectedFuncDeclareWithParams.test(currentLine)
        ) && currentLine.indexOf("=") === -1) {
        inFunction = true;
        inParamDeclaration = true;
        return true;
    }
    else {
        return false;
    }
};

const getPrefix = function(privateRegex, isFunction) {
    if(privateRegex.test(currentLine) || isLocalVarDeclare()) {
        if(isFunction) 
	     return "const ";
	else return "let ";
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
    var prefix = getPrefix(privateFuncDeclare, true);
    var words = currentLine.trim().split(" ");
    var spaces = currentLine.search(/\S/);
    var indent = currentLine.substr(0, spaces);
    var functionName = (words[1] === "static" || words[1] === "virtual") ? words[3] : words[2];
    currentLine = createFunctionSentence(indent + prefix + extractFunctionName(functionName), getParams(currentLine), isMultiline());
};

const isMultiline = function() {
    var multiLineParam = currentLine.split("//")[0].trim().endsWith("(");
    inMultiLineParameter = multiLineParam && inParamDeclaration;
    return multiLineParam;
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

const isMultilineParam = function() {
    var line = currentLine.split("//")[0].trim();
    return (
        line.endsWith(",")
            || line.endsWith(")")
            || line.endsWith(");")
            || line.substr(0, 2) === "+ "
            || line.substr(0, 1) === '"'
        );
};

const isPPorMM = function() {
    var line = currentLine.split("//")[0].trim();
    return line.endsWith("++;") || line.endsWith("--;");
};

const isArrayAssign = function() {
    var line = currentLine.split("//")[0].trim();
    return arrayAssignment.test(line);
};

const multilineParamTranslator = function() {
    if(inMultiLineParameter) {
        var spaces = currentLine.search(/\S/);
        var indent = currentLine.substr(0, spaces);
        var line = currentLine.trim().split("//");
        var comments = line[1] ? "//" + line[1] : "";
        var isLastParameter = line[0].endsWith(")");
        currentLine = indent + getParams(removeGenerics(currentLine.trim()));
        if(isLastParameter) {
            currentLine += ")";
            addStartBlock();
        }
        currentLine += comments;
    }
};

const removeGenerics = function(line) {
    var deep = 0;
    var newLine = "";
    var temp = "";
    for(var i = 0, count = line.length; i < count; i += 1) {
        var c = line.substr(i,1);
        if(c === "<") {
            deep++;
        }
        if(deep === 0) {
            newLine += c;
            temp = "";
        }
        else {
            temp += c;
            if(!validIdentifierCharacter.test(c) && ", <>".indexOf(c) === -1) {
                newLine += temp;
                temp = "";
                deep = 0;
            }
        }
        if(c === ">") {
            deep--;
        }
        if(deep < 0) {
            deep = 0;
        }
    }
    return newLine;
};

const varDeclareTranslator = function() {
    var prefix = getPrefix(privateDeclare);
    var words = removeGenerics(currentLine.trim()).split(" ");
    var spaces = currentLine.search(/\S/);
    var indent = currentLine.substr(0, spaces);
    currentLine = createSentence(indent + prefix + getExpression(words, getIdentifierIndex(words)));
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
    if(words.indexOf("=") === -1) {
        for(var i = 0; i < words.length; i++) {
            if(words[i].indexOf(";") !== -1) {
                words[i] = words[i].replace(";", " = null;");
                break;
            }
        }
    }
    return words.slice(index).join(" ");
};

const isAssignment = function() {
    var line = currentLine.trim().split(" ");
    return line[1] === "=" || line[1] === "+=" || line[1] === "-=";
};

const assignmentTranslator = function() {
    var words = removeGenerics(currentLine.trim()).split(" ");
    var spaces = currentLine.search(/\S/);
    var indent = currentLine.substr(0, spaces);
    currentLine = indent + words.join(" ");
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
    return (currentLine.trim().substr(0, 4) === "if (" || currentLine.trim().substr(0, 3) === "if(");
};

const isMultiLineBlock = function() {
    return openParenthesis > 0;
};

const countParenthesis = function() {
    for(var i = 0, count = currentLine.length; i < count; i++) {
        var c = currentLine.substr(i,1);
        if(c === "(") {
            openParenthesis++;
        }
        else if(c === ")") {
            openParenthesis--;
        }
    }
};

const addStartBlock = function() {
    var line = currentLine.trim().split("//")[0];
    if(! line.endsWith("}") && ! line.endsWith("{")) {
        currentLine += " {";
        removeNextBracket = 0;
    }
};

const inBlockTranslator = function() {
    countParenthesis();
    if(openParenthesis === 0) {
        addStartBlock();
        inMultiLineBlock = false;
    }
    if(isMultilineParam()) multilineParamTranslator();
};

const beginBlockTranslator = function() {
    openParenthesis = 0;
    countParenthesis();
    if(isMultiLineBlock()) {
        inMultiLineBlock = true;
    }
    else {
        addStartBlock();
    }
};

const isElse = function() {
    return (currentLine.trim().substr(0, 5) === "else " || currentLine.trim().substr(0, 4) === "else");
};

const isReturn = function() {
    return (currentLine.trim().substr(0, 7) === "return " || currentLine.trim().substr(0, 7) === "return;");
};

const isFor = function() {
    return (currentLine.trim().substr(0, 5) === "for (" || currentLine.trim().substr(0, 4) === "for(");
};

const isSwitch = function() {
    return (currentLine.trim().substr(0, 8) === "switch (" || currentLine.trim().substr(0, 7) === "switch(");
};

const isCase = function() {
    return (currentLine.trim().substr(0, 5) === "case " || currentLine.trim().substr(0, 8) === "default:");
};

const isBreak = function() {
    return currentLine.trim().substr(0, 6) === "break;";
};

const forTranslator = function() {
    currentLine = currentLine.replace("for (int ", "for(var ");
    currentLine = currentLine.replace("for(int ", "for(var ");
    beginBlockTranslator();
};

const isForEach = function() {
    if(currentLine.trim().substr(0, 8) === "foreach ") {
        inForEach = true;
        loopIndex++;
        loopBracketIndex[loopIndex] = openBrackets + 1;
        return true;
    }
    else {
        return false;
    }
};

const forEachTranslator = function() {
    var words = removeGenerics(currentLine.trim()).split(" ");
    var x = loopIdentifiers[loopIndex];
    var spaces = currentLine.search(/\S/);
    var indent = currentLine.substr(0, spaces);
    currentLine = indent + "for(var " + x + " = 0; " + x + " < " + words[4].replace(")","").trim() + ".length; " + x + "++)";
    beginBlockTranslator();
};

const isWhile = function() {
    return (currentLine.trim().substr(0, 7) === "while (" || currentLine.trim().substr(0, 6) === "while(");
};

const isDo = function() {
    return (currentLine.trim().substr(0, 2) === "do");
};

const isCall = function() {
    var line = currentLine.trim();
    return line.indexOf("(") != -1
        && (line.endsWith(";")
            || line.endsWith(",")
            || line.endsWith("(")
            || line.endsWith('"')
            );
};

const isTry = function() {
    var line = currentLine.trim();
    return line === "try" || line.substr(0, 5) === "try {";
};

const isCatch = function() {
    var line = currentLine.trim();
    return line.substr(0, 5) === "catch";
};

const catchTranslator = function() {
    var line = currentLine.split("(");
    if(line.length === 1) {
        var i = currentLine.indexOf("{");
        if(i > -1) {
            currentLine = currentLine.substr(0,i) + " (ex) " + currentLine.substring(i);
        }
        else {
            currentLine += "(ex)";
        }
    }
    else {
        var identifier = line[1].split(" ");
        currentLine = line[0] + "(" + identifier.slice(1).join(" ");
    }
    beginBlockTranslator();
};

const isDelegate = function() {
    return currentLine.indexOf(" delegate ") !== -1;
};

const isEvent = function() {
    return currentLine.indexOf(" event ") !== -1;
};

const commentLineTranslator = function() {
    currentLine = "// " + currentLine;
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

const numbersOrEmpty = new RegExp('^[0-9]*$');

const sanitize = function() {
    var words = [];
    var word = "";
    for(var i = 0, count = currentLine.length; i < count; i += 1) {
        var c = currentLine.substr(i,1);
        if(c === "(") {
            words.push(word)
            word = c;
        }
        else {
            word += c;
            if(c === ")") {
                words.push(word)
                word = "";
            }
        }
    }
    words.push(word);
    for(var i = 0, count = words.length; i < count; i += 1) {
        if(words[i].substr(0, 1) === "("
            && words[i].endsWith(")")
            && words[i].trim() === words[i]
            && words[i].indexOf(",") === -1
            && ! numbersOrEmpty.test(words[i])
            ) {
            word = replaceAll(words[i], " ", "");
            if(word !== "()"
                && word !== "(\"\")"
                && (
                    "<>(=+-*".indexOf(words[i-1].trim().slice(-1)) !== -1
                    || words[i-1].trim().endsWith("return")
                    || words[i-1].trim().endsWith("case")
                    )
                && words[i+1].trim().substr(0,1) !== "?"
                && words[i+1].trim().substr(0,1) !== ";"
                ) {
                console.log(words);
                words[i] = "";
            }
        }
    }
    currentLine = words.join("");

    currentLine = replaceAll(currentLine, "(ref ", "(");
    currentLine = replaceAll(currentLine, "(out ", "(");
    currentLine = replaceAll(currentLine, " out ", " ");
    currentLine = replaceAll(currentLine, " ref ", " ");
    currentLine = replaceAll(currentLine, " == ", " === ");
    currentLine = replaceAll(currentLine, " != ", " !== ");
};

const printOriginalLine = function() {
    var line = originalLine.trim();
    if(line.length > 0)
        currentLine += " // " + line;
};

const transpile = function(sourceFile, outputFolder, next, printOriginalCode) {
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
            checkParenthesis();
            sanitize();
            if(printOriginalCode)
                printOriginalLine();
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
};

// public interface
//
var self = {
    transpile: transpile
};

/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
if (typeof module != 'undefined' && module.exports) module.exports = self; // CommonJS, node.js
