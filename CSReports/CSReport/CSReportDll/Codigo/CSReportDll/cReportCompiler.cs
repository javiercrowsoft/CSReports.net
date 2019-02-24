﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Globalization;
using CSKernelClient;
using CSKernelNumberToString;
using CSReportGlobals;
using CSReportScript;

namespace CSReportDll
{

    public class cReportCompiler
    {

        // all the functions (c# code or internal functions) use colons as
        // a separator for parameters. The Spanish and other configurations 
        // use the colon as the decimal separator.
        //
        // because of that we need to replace every colon by a pipe
        // and after compiling every parameter and internal function
        // we replace every pipe by a colon and then send the code to
        // c# engine. (Microsoft.CSharp.CSharpCodeProvider)

        // http://stackoverflow.com/questions/137933/what-is-the-best-scripting-language-to-embed-in-a-c-desktop-application

        private const String C_MODULE = "cReportCompiler";

        private const String C_TEMPFUNCTIONB = "Option explicit";
        private const String C_TEMPFUNCTIONE = "\r\n";

        private const String C_MACRO_CTRL = "@@";

        private const String C_AVERAGE_SUM = "AverageSum";
        private const String C_AVERAGE_COUNT = "AverageCount";
        private const String C_SUM = "Sum";
        private const String C_SUM_TIME = "SumTime";
        private const String C_MAX = "Max";
        private const String C_MIN = "Min";
        private const String C_COUNT = "Count";
        private const String C_NUMBER_TO_STRING = "NumberToString";
        private const String C_GET_BARCODE = "GetBarcode";
        private const String C_GET_DATA_FROM_RS_AD = "GetDataFromRsAd";

        private const String C_ISEQUAL = "IsEqual";
        private const String C_ISNOTEQUAL = "IsNotEqual";
        private const String C_ISGREATERTHAN = "IsGreaterThan";
        private const String C_ISLESSTHAN = "IsLessThan";

        private const String C_GETDATAFROMRS = "GetDataFromRs";

        private const String C_GROUPTOTAL = "GroupTotal";
        private const String C_GROUPMIN = "GroupMin";
        private const String C_GROUPMAX = "GroupMax";
        private const String C_GROUPAVERAGE = "GroupAverage";
        private const String C_GROUPPERCENT = "GroupPercent";
        private const String C_GROUPPERCENTT = "GroupPercentT";
        private const String C_GROUPCOUNT = "GroupCount";
        private const String C_GROUPLINENUMBER = "GroupLineNumber";

        private const String C_ISINRS = "IsInRs";

        private const int C_SPANISH = 1;
        private const int C_ENGLISH = 2;
        private const int C_FRENCH = 3;

        private const String C_KEYFUNCINT = "$$$";

        private cReportFormulaTypes m_formulaTypes = new cReportFormulaTypes();
        private cReport m_report;
        private cReportVariables m_variables = new cReportVariables();

        // the current formula we are compiling
        //
        private cReportFormula m_formula;
        // the current internal formula we are compiling
        //
        private cReportFormulaInt m_fint;

        private cReportCompilerGlobals m_objGlobals = new cReportCompilerGlobals();

        private Dictionary<string, List<String>> m_collTextReplace = new Dictionary<string, List<String>>();
        private String m_ctrlName = "";

        private bool m_bCompile;
        private int m_idxFormula = -1;

        internal cReport getReport()
        {
            return m_report;
        }

        internal void setReport(cReport rhs)
        {
            m_report = rhs;
        }

        public void clearVariables()
        {
            m_variables.clear();
        }

        public void initGlobalObject()
        {
            m_objGlobals.clear();
            m_collTextReplace.Clear();
        }

        // it compiles the code of every formula
        // first it replaces every internal function by 
        // dummy return values (of the type of the internal function)
        // if after the replace there is code it call cReportScriptEngine.compileCode
        // if there are no errors it returns true
        // 
        public bool checkSyntax(cReportFormula formula)
        {
            try
            {
                String code = "";

                m_formula = formula;
                m_formula.getFormulasInt().clear();

                // check syntax
                code = formula.getText();
                m_formula.setTextC(code);

                pCheckSyntax(code);

                return true;
            }
            catch (Exception ex)
            {
                cError.mngError(ex, "checkSyntax", C_MODULE, "");

                m_formula = null;
                m_fint = null;

                return false;
            }

        }

        public void initVariable(cReportFormula formula)
        {
            cReportVariable var = null;
            cReportFormulaInt fint = null;
            cStructTime st = null;

            for (int _i = 0; _i < formula.getFormulasInt().count(); _i++)
            {
                fint = formula.getFormulasInt().item(_i);
                for (int _j = 0; _j < fint.getVariables().count(); _j++)
                {
                    var = fint.getVariables().item(_j);

                    System.TypeCode typeCode = System.Type.GetTypeCode(var.getValue().GetType());
                    switch (typeCode)
                    {

                        case System.TypeCode.DBNull:
                            break;
                        case System.TypeCode.Decimal:
                        case System.TypeCode.Double:
                        case System.TypeCode.Int16:
                        case System.TypeCode.Int32:
                        case System.TypeCode.Int64:
                        case System.TypeCode.Single:
                        case System.TypeCode.UInt16:
                        case System.TypeCode.UInt32:
                        case System.TypeCode.UInt64:
                        case System.TypeCode.Byte:
                        case System.TypeCode.SByte:
                        case System.TypeCode.DateTime:
                        case System.TypeCode.Boolean:
                            var.setValue(0);
                            break;
                        case System.TypeCode.Char:
                        case System.TypeCode.String:
                            var.setValue("");
                            break;
                        case System.TypeCode.Object:
                            if (var.getValue() is cStructTime)
                            {
                                st = (cStructTime)var.getValue();
                                st.setHour(0);
                                st.setMinute(0);
                                st.setSecond(0);
                            }
                            break;
                        case System.TypeCode.Empty:
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void pEvalGroupFunctions(cReportFormula formula)
        {
            cReportFormulaInt fint = null;

            for (int _i = 0; _i < formula.getFormulasInt().count(); _i++)
            {
                fint = formula.getFormulasInt().item(_i);

                switch (fint.getFormulaType())
                {
                    case csRptFormulaType.CSRPTF_GROUP_TOTAL:
                        evalGroupTotal(fint);

                        break;
                    case csRptFormulaType.CSRPTF_GROUP_MAX:
                        evalGroupMax(fint);

                        break;
                    case csRptFormulaType.CSRPTF_GROUP_MIN:
                        evalGroupMin(fint);

                        break;
                    case csRptFormulaType.CSRPTF_GROUP_AVERAGE:
                        evalGroupAverage(fint);

                        break;
                    case csRptFormulaType.CSRPTF_GROUP_PERCENT:
                        evalGroupPercent(fint);

                        break;
                    case csRptFormulaType.CSRPTF_GROUP_COUNT:
                        evalGroupCount(fint);

                        break;
                    case csRptFormulaType.CSRPTF_GROUP_LINE_NUMBER:
                        evalGroupLineNumber(fint);

                        break;
                }
            }
        }

        public object resultFunction(cReportFormula formula)
        {
            String code = "";
            object[] vResult = null;

            m_objGlobals.setMode(eReportCompilerMode.C_RESULT);
            m_ctrlName = formula.getControlName();

            vResult = new object[formula.getFormulasInt().count()];

            cReportFormulaInt fint = null;

            for (int _i = 0; _i < formula.getFormulasInt().count(); _i++)
            {
                fint = formula.getFormulasInt().item(_i);
                vResult[_i] = pResultFunctionInt(fint);
            }

            // we check if the code has scripting or is only 
            // calls to internal functions
            //
            code = formula.getTextC().Replace(C_KEYFUNCINT, "");
            code = code.Replace(" ", "");

            // if after removing calls to internal functions and spaces
            // there is only a number we don't have scripting
            //
            if (G.isNumeric(code))
            {
                if (vResult.Length > 0)
                {
                    formula.setLastResult(vResult[0]);
                    formula.setHaveToEval(false);
                    return formula.getLastResult();
                }
                // the function can be only a constant (it is used in Cairo navigation)
                //
                else
                {
                    return code;
                }
            }
            else
            {
                code = formula.getTextC();
                var parameters = "";

                for (int i = 0; i < vResult.Length; i++)
                {
                    // if one argument is null it means we don't have a row for this formula
                    // so we don't need to compile the code
                    //
                    if (vResult[i] == null) 
                    { 
                        return null; 
                    }

                    /* TODO: remove me
                    code = code.Replace(C_KEYFUNCINT + cReportGlobals.format(i + 1, "000"), 
                                            getNumericVal(vResult[i].ToString()));
                     * */

                    var parameter = "p__" + i + "__";
                    parameters += parameter + ",";
                    code = code.Replace(C_KEYFUNCINT + cReportGlobals.format(i + 1, "000"), parameter);

                    var paramValue = m_objGlobals.getVar(parameter);
                    if (paramValue == null) {
                        paramValue = m_objGlobals.addVar(parameter);
                    }
                    paramValue.setValue(vResult[i]);
                }

                if (parameters.Length > 0)
                {
                    parameters = parameters.Substring(0, parameters.Length - 1);
                    code = insertParametersIntoFunction(code, parameters);
                }

                formula.setLastResult(pExecScriptCode(code, formula));
                formula.setHaveToEval(false);
                return formula.getLastResult();
            }
        }

        private string insertParametersIntoFunction(string code, string parameters)
        {
            int n = code.IndexOf("(") + 1;
            return code = code.Substring(0, n) + parameters + code.Substring(n);
        }

        private void pEvalFunctionGroup(cReportFormulaInt fint)
        {
            double value = 0;
            double total = 0;

            if (fint.getVariables().count() > 0)
            {
                if (fint.getParameters().item(cReportGlobals.C_KEYINDEXCOL2) == null)
                {
                    value = 0;
                }
                else
                {
                    int columnIndex = int.Parse(fint.getParameters().item(cReportGlobals.C_KEYINDEXCOL2).getValue());
                    value = cUtil.val(m_report.getValueFromRs(columnIndex).ToString());
                }

                cReportVariable var = fint.getVariables().item(C_GROUPPERCENTT);
                total = cUtil.val(var.getValue().ToString());
                value = cUtil.divideByZero(value, total);
                var.setValue(value);

            }

        }

        public void evalFunctionGroup(cReportFormula formula)
        {
            cReportFormulaInt fint = null;

            for (int _i = 0; _i < formula.getFormulasInt().count(); _i++)
            {
                fint = formula.getFormulasInt().item(_i);
                pEvalFunctionGroup(fint);
            }
        }

        public void evalFunction(cReportFormula formula)
        {
            String codeC = "";

            m_objGlobals.setMode(eReportCompilerMode.C_EVAL);

            pEvalGroupFunctions(formula);

            cReportError.gDebugSection = formula.getSectionName();
            cReportError.gDebugSectionLine = formula.getSectionLineIndex();
            cReportError.gDebugControl = formula.getControlName();

            m_formula = formula;

            pCompile(formula.getText(), false, codeC);

            if (formula.getFormulasInt().count() > 0)
            {
                if (G.isNumeric(codeC))
                {
                    pEvalSyntax("", codeC, false, formula);
                }
                else
                {
                    if (cUtil.subString(codeC.Trim(), 0, 8).ToLower() == "function")
                    {
                        pEvalSyntax("", codeC, false, formula);
                    }
                }
            }
            else
            {
                pEvalSyntax("", codeC, false, formula);
            }
            m_formula = null;
        }

        private bool pCompile(String code, bool bCompile, String codeC)
        {
            m_bCompile = bCompile;
            m_idxFormula = -1;

            code = pColonToPipe(code);

            return pCompileAux(code, out codeC);
        }

        private String pColonToPipe(String code)
        {
            return code.Replace(",", "|");
        }

        private String pPipeToColon(String code)
        {
            return code.Replace("|", ",");
        }

        private bool pIsFunction(String word)
        {
            cReportFormulaType f = null;

            for (int _i = 0; _i < m_formulaTypes.count(); _i++)
            {
                f = m_formulaTypes.item(_i);
                if (word.ToLower() == f.getName().ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        private object pAddFormulaInt(String functionName, String code)
        {
            // the firs thing we need to do is add this internal formula 
            // to the internal formula collection of this formula
            //
            m_fint = m_formula.getFormulasInt().add();
            return pEvalSyntax(functionName, code, true, null);
        }

        private void pEvalFunctionInt(cReportFormulaInt fint)
        {
            switch (fint.getFormulaType())
            {
                case csRptFormulaType.CSRPTF_AVERAGE:
                    evalAverage(fint);
                    break;

                case csRptFormulaType.CSRPTF_SUM:
                    evalSum(fint);
                    break;

                case csRptFormulaType.CSRPTF_SUM_TIME:
                    evalSumTime(fint);
                    break;

                case csRptFormulaType.CSRPTF_MAX:
                    evalMax(fint);
                    break;

                case csRptFormulaType.CSRPTF_MIN:
                    evalMin(fint);
                    break;

                case csRptFormulaType.CSRPTF_COUNT:
                    evalCount(fint);
                    break;

                case csRptFormulaType.CSRPTF_NUMBER_TO_STRING:
                    evalNumberToString(fint);
                    break;

                case csRptFormulaType.CSRPTF_IS_EQUAL:
                    evalIsEqual(fint);
                    break;

                case csRptFormulaType.CSRPTF_IS_NOT_EQUAL:
                    evalIsNotEqual(fint);
                    break;

                case csRptFormulaType.CSRPTF_IS_GREATER_THAN:
                    evalIsGreaterThan(fint);
                    break;

                case csRptFormulaType.CSRPTF_IS_LESS_THAN:
                    evalIsLessThan(fint);
                    break;

                case csRptFormulaType.CSRPTF_CALCULO:
                    // nothing to do
                    break;

                case csRptFormulaType.CSRPTF_DECLARE_VAR:
                    evalDeclareVar(fint);
                    break;

                case csRptFormulaType.CSRPTF_GET_VAR:
                    // nothing to do
                    break;

                case csRptFormulaType.CSRPTF_GET_PARAM:
                    // nothing to do
                    break;

                case csRptFormulaType.CSRPTF_SET_VAR:
                    evalSetVar(fint);
                    break;

                case csRptFormulaType.CSRPTF_GET_BARCODE:
                    evalGetBarcode(fint);
                    break;

                case csRptFormulaType.CSRPTF_ADD_TO_VAR:
                    evalAddToVar(fint);
                    break;

                case csRptFormulaType.CSRPTF_GET_DATA_FROM_RS_AD:
                    evalGetDataFromRsAd(fint);
                    break;

                case csRptFormulaType.CSRPTF_GET_DATA_FROM_RS:
                    evalGetDataFromRs(fint);
                    break;

                case csRptFormulaType.CSRPTF_IS_IN_RS:
                    evalIsInRs(fint);
                    break;
            }
        }

        private object pResultFunctionInt(cReportFormulaInt fint)
        {
            switch (fint.getFormulaType())
            {
                case csRptFormulaType.CSRPTF_AVERAGE:
                    return resultAverage(fint);

                case csRptFormulaType.CSRPTF_SUM:
                    return resultSum(fint);

                case csRptFormulaType.CSRPTF_GET_STRING:
                    return resultGetString(fint);

                case csRptFormulaType.CSRPTF_SUM_TIME:
                    return resultSumTime(fint);

                case csRptFormulaType.CSRPTF_MAX:
                    return resultMax(fint);

                case csRptFormulaType.CSRPTF_MIN:
                    return resultMin(fint);

                case csRptFormulaType.CSRPTF_COUNT:
                    return resultCount(fint);

                case csRptFormulaType.CSRPTF_NUMBER_TO_STRING:
                    return resultNumberToString(fint);

                case csRptFormulaType.CSRPTF_IS_EQUAL:
                    return resultIsEqual(fint);

                case csRptFormulaType.CSRPTF_IS_NOT_EQUAL:
                    return resultIsNotEqual(fint);

                case csRptFormulaType.CSRPTF_IS_GREATER_THAN:
                    return resultIsGreaterThan(fint);

                case csRptFormulaType.CSRPTF_IS_LESS_THAN:
                    return resultIsLessThan(fint);

                case csRptFormulaType.CSRPTF_PAGE_NUMBER:
                    return resultPageNumber();

                case csRptFormulaType.CSRPTF_TOTAL_PAGES:
                    return resultTotalPages();

                case csRptFormulaType.CSRPTF_VAL:
                    return resultValue(fint);

                case csRptFormulaType.CSRPTF_LENGTH:
                    return resultLength(fint);

                case csRptFormulaType.CSRPTF_TEXT_REPLACE:
                    return resultTextReplace(fint);

                case csRptFormulaType.CSRPTF_CALCULO:
                    return resultCalculo(fint);

                case csRptFormulaType.CSRPTF_DECLARE_VAR:
                    // nothing to do
                    break;

                case csRptFormulaType.CSRPTF_GET_VAR:
                    return resultGetVar(fint);

                case csRptFormulaType.CSRPTF_GET_PARAM:
                    return resultGetParam(fint);

                case csRptFormulaType.CSRPTF_SET_VAR:
                    // nothing to do
                    break;

                case csRptFormulaType.CSRPTF_ADD_TO_VAR:
                    // nothing to do
                    break;

                case csRptFormulaType.CSRPTF_GET_DATA_FROM_RS_AD:
                    return resultGetDataFromRsAd(fint);

                case csRptFormulaType.CSRPTF_GET_DATA_FROM_RS:
                    return resultGetDataFromRs(fint);

                case csRptFormulaType.CSRPTF_GROUP_TOTAL:
                    return resultGroupTotal(fint);

                case csRptFormulaType.CSRPTF_GROUP_MAX:
                    return resultGroupMax(fint);

                case csRptFormulaType.CSRPTF_GROUP_MIN:
                    return resultGroupMin(fint);

                case csRptFormulaType.CSRPTF_GROUP_AVERAGE:
                    return resultGroupAverage(fint);

                case csRptFormulaType.CSRPTF_GROUP_PERCENT:
                    return resultGroupPercent(fint);

                case csRptFormulaType.CSRPTF_GROUP_COUNT:
                    return resultGroupCount(fint);

                case csRptFormulaType.CSRPTF_GROUP_LINE_NUMBER:
                    return resultGroupLineNumber(fint);

                case csRptFormulaType.CSRPTF_IS_IN_RS:
                    return resultIsInRs(fint);

                case csRptFormulaType.CSRPTF_GET_BARCODE:
                    return resultGetBarcode(fint);
            }
            return null;
        }

        private object pEvalSyntax(String functionName, String code, bool bParam, cReportFormula formula)
        {
            int i = 0;
            String s = "";

            code = removeReturns(code);

            if (functionName.Length > 0)
            {
                return pCheckInternalFunction(functionName, code);
            }
            else if (code.Length == 0)
            {
                return "";
            }
            else if (code == "\"\"")
            {
                return "";
            }
            else if (G.isNumeric(code))
            {
                return code;
            }
            else if (cReportGlobals.isDate(code))
            {
                return code;
            }
            else if (pIsTime(code))
            {
                return code;
            }
            else if (!bParam)
            {
                pExecScriptCode(code, formula);
                return code;
            }
            else
            {

                String[] vParams = null;
                String parameters = "";

                parameters = code.Trim();
                if (parameters.Length > 2)
                {
                    parameters = parameters.Substring(2, parameters.Length - 2);
                    parameters = parameters.Trim();
                    vParams = parameters.Split('|');
                }

                try
                {
                    for (i = 0; i < vParams.Length; i++)
                    {
                        try
                        {
                            // if it is a number we don't need to evaluate it
                            //
                            if (!G.isNumeric(vParams[i]))
                            {

                                if (!pIsControl(vParams[i]))
                                {
                                    // Si se produce un error es por que se trata
                                    // de un parametro a la funcion, la asignacion
                                    // no se llevara a cabo, y no perdere el valor
                                    // del parametro
                                    s = C_TEMPFUNCTIONB + vParams[i] + C_TEMPFUNCTIONE;
                                    vParams[i] = pExecScriptCode(s, formula).ToString();
                                }
                            }
                            code = vParams[i] + "|";
                        }
                        catch 
                        {
                            // we don't care about errors here
                        }
                    }

                    code = cUtil.removeLastColon(code);
                    return code;
                }
                catch
                {
                    // we don't care about errors here
                }
            }
            return null;
        }

        private bool pIsTime(String code)
        {
            String[] vTime = null;

            code = code.Trim();
            if (code.IndexOf(":", 0) == 0) 
            { 
                return false; 
            }

            vTime = code.Split(':');
            if (vTime.Length != 1) 
            { 
                return false; 
            }

            if (!(G.isNumeric(vTime[0]) && G.isNumeric(vTime[1]))) 
            { 
                return false; 
            }
            return true;
        }

        private void pCheckSyntax(String code)
        {
            pCompile(code, true, "");
        }

        private object pExecScriptCode(String code, cReportFormula formula)
        {
            try
            {
                code = pPipeToColon(code);
                if (formula.getCompiledScript() == null)
                {
                    formula.setCompiledScript(cReportScriptEngine.compileCode(code, formula));
                }
                return cReportScriptEngine.eval(formula.getCompiledScript(), m_objGlobals);
            }
            catch (Exception ex)
            {
                String msg = ex.Source
                            + ex.Message + "\n\nCode:\n=====\n\n" + code + "\n\n"
                            + ex.HelpLink;
                throw new ReportException(csRptErrors.ERROR_IN_SCRIPT, C_MODULE, msg);
            }
        }

        private bool pIsControl(String param)
        {
            cReportControl ctrl = null;
            for (int _i = 0; _i < m_report.getControls().count(); _i++)
            {
                ctrl = m_report.getControls().item(_i);
                if (ctrl.getName().ToUpper() == param.ToUpper())
                {
                    return true;
                }
            }
            return false;
        }

        private cReportControl pGetControl(String param)
        {
            cReportControl ctrl = null;
            for (int _i = 0; _i < m_report.getControls().count(); _i++)
            {
                ctrl = m_report.getControls().item(_i);
                if (ctrl.getName().ToUpper() == param.ToUpper())
                {
                    return ctrl;
                }
            }
            return null;
        }

        private String pGetSubName(String code)
        {
            int pos = 0;
            int i = 0;
            String c = "";

            pos = code.IndexOf(" ", 0) + 1;
            i = pos;
            while (i < code.Length)
            {
                c = code.Substring(i, 1);
                if (pIsSeparator(c))
                {
                    break;
                }
                i++;
            }
            return code.Substring(pos, i - pos);
        }

        private String pGetParameter(String parameters, int paramIndex, String function)
        {
            String param = "";
            String[] vParam = null;

            vParam = parameters.Split('|');

            if (paramIndex > vParam.Length + 1)
            {
                throw new ReportArgumentMissingException(
                    C_MODULE,
                    cReportError.errGetDescript(
                                    csRptErrors.CSRPTERRMISSINGPARAM,
                                    paramIndex.ToString(),
                                    function));
            }
            else
            {
                param = vParam[paramIndex];
            }

            return param.Replace(")", "").Trim();
        }

        private object pCheckInternalFunction(String functionName, String code)
        {
            String name = "";
            String parameters = "";
            csRptFormulaType idFunction = 0;

            int r = 0;
            int q = 0;
            String tc = "";

            name = functionName;
            parameters = code.Trim();
            if (parameters.Length > 2)
            {
                parameters = parameters.Substring(1, parameters.Length - 2);
            }

            // we need to replace in m_formula.getTextC() the function name by its key
            // 
            tc = m_formula.getTextC();
            q = name.Length;
            r = tc.ToLower().IndexOf(name.ToLower(), 0);
            q = tc.ToLower().IndexOf(")".ToLower(), r) + 1;

            m_formula.setTextC((tc.Substring(0, r)).ToString()
                                + C_KEYFUNCINT
                                + cReportGlobals.format(m_formula.getFormulasInt().count(), "000")
                                + tc.Substring(q));

            idFunction = pGetIdFunction(name);
            m_fint.setFormulaType(idFunction);

            switch (idFunction)
            {

                case csRptFormulaType.CSRPTF_PAGE_NUMBER:

                    // in compiling time we need to return a value which is consistent
                    // with the return type of the internal function
                    //
                    if (m_report == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return m_report.getCurrenPage();
                    }

                case csRptFormulaType.CSRPTF_TEXT_REPLACE:
                    // in compiling time we need to return a value which is consistent
                    // with the return type of the internal function
                    //
                    return "";

                case csRptFormulaType.CSRPTF_TOTAL_PAGES:
                    return m_report.getTotalPages();

                // all this functions have the same amount of parameters
                //
                case csRptFormulaType.CSRPTF_AVERAGE:
                case csRptFormulaType.CSRPTF_SUM:
                case csRptFormulaType.CSRPTF_MAX:
                case csRptFormulaType.CSRPTF_MIN:
                case csRptFormulaType.CSRPTF_LENGTH:
                case csRptFormulaType.CSRPTF_VAL:
                    // in this evaluation we load the parameters of the function
                    //
                    pCheckParameters(1, parameters, name);
                    // in compiling time we need to return a value which is consistent
                    // with the return type of the internal function
                    //
                    return 0;

                case csRptFormulaType.CSRPTF_GROUP_TOTAL:
                case csRptFormulaType.CSRPTF_GROUP_MAX:
                case csRptFormulaType.CSRPTF_GROUP_MIN:
                case csRptFormulaType.CSRPTF_GROUP_AVERAGE:
                    // in this evaluation we load the parameters of the function
                    //
                    pCheckParameters(2, parameters, name);
                    // in compiling time we need to return a value which is consistent
                    // with the return type of the internal function
                    //
                    return 0;

                // all this functions have the same amount of parameters
                //
                case csRptFormulaType.CSRPTF_GROUP_COUNT:
                case csRptFormulaType.CSRPTF_GROUP_PERCENT:
                    // in this evaluation we load the parameters of the function
                    //
                    pCheckParameters(3, parameters, name);
                    // in compiling time we need to return a value which is consistent
                    // with the return type of the internal function
                    //
                    return 0;

                case csRptFormulaType.CSRPTF_GET_STRING:
                    // in this evaluation we load the parameters of the function
                    //
                    pCheckParameters(1, parameters, name);
                    // in compiling time we need to return a value which is consistent
                    // with the return type of the internal function
                    //
                    return "\"\"";

                case csRptFormulaType.CSRPTF_SUM_TIME:
                    // in this evaluation we load the parameters of the function
                    //
                    pCheckParameters(2, parameters, name);
                    // in compiling time we need to return a value which is consistent
                    // with the return type of the internal function
                    //
                    return 0;

                case csRptFormulaType.CSRPTF_COUNT:
                    // in compiling time we need to return a value which is consistent
                    // with the return type of the internal function
                    //
                    return 0;

                case csRptFormulaType.CSRPTF_NUMBER_TO_STRING:
                    // in this evaluation we load the parameters of the function
                    //
                    pCheckParameters(2, parameters, name);
                    // in compiling time we need to return a value which is consistent
                    // with the return type of the internal function
                    //
                    return "\"\"";

                // all this functions have the same amount of parameters
                //
                case csRptFormulaType.CSRPTF_IS_EQUAL:
                case csRptFormulaType.CSRPTF_IS_NOT_EQUAL:
                case csRptFormulaType.CSRPTF_IS_GREATER_THAN:
                case csRptFormulaType.CSRPTF_IS_LESS_THAN:
                case csRptFormulaType.CSRPTF_IS_IN_RS:
                    // in this evaluation we load the parameters of the function
                    //
                    pCheckParameters(2, parameters, name);
                    // in compiling time we need to return a value which is consistent
                    // with the return type of the internal function
                    //
                    return 0;

                // all this functions have the same amount of parameters
                //
                case csRptFormulaType.CSRPTF_CALCULO:
                case csRptFormulaType.CSRPTF_GET_DATA_FROM_RS_AD:
                case csRptFormulaType.CSRPTF_GET_DATA_FROM_RS:
                    // in this evaluation we load the parameters of the function
                    //
                    pCheckParameters(4, parameters, name);
                    // in compiling time we need to return a value which is consistent
                    // with the return type of the internal function
                    //
                    return 0;

                // all this functions have the same amount of parameters
                //
                case csRptFormulaType.CSRPTF_DECLARE_VAR:
                case csRptFormulaType.CSRPTF_GET_VAR:
                    // in this evaluation we load the parameters of the function
                    //
                    pCheckParameters(1, parameters, name);
                    // in compiling time we need to return a value which is consistent
                    // with the return type of the internal function
                    //
                    return 0;

                case csRptFormulaType.CSRPTF_GET_PARAM:
                    // in this evaluation we load the parameters of the function
                    //
                    pCheckParameters(1, parameters, name);
                    // in compiling time we need to return a value which is consistent
                    // with the return type of the internal function
                    //
                    return 0;

                // all this functions have the same amount of parameters
                //
                case csRptFormulaType.CSRPTF_ADD_TO_VAR:
                case csRptFormulaType.CSRPTF_SET_VAR:
                    // in this evaluation we load the parameters of the function
                    //
                    pCheckParameters(2, parameters, name);
                    // in compiling time we need to return a value which is consistent
                    // with the return type of the internal function
                    //
                    return 0;

                case csRptFormulaType.CSRPTF_GET_BARCODE:
                    // in this evaluation we load the parameters of the function
                    //
                    pCheckParameters(1, parameters, name);
                    // in compiling time we need to return a value which is consistent
                    // with the return type of the internal function
                    //
                    return "\"\"";

                default:
                    throw new ReportNotDefinedFunctionException(
                        C_MODULE,
                        cReportError.errGetDescript(csRptErrors.CSRPTERRINDEFINEDFUNCTION,name));
            }
        }

        private String resultGetString(cReportFormulaInt fint)
        {
            String param = "";

            param = fint.getParameters().item(0).getValue();

            if (param == "\"\"")
            {
                return param;
            }
            else
            {
                if (pIsControl(param))
                {
                    return "\"" + m_report.getValueString(param).Replace("\"", "\"\"") + "\"";
                }
                else
                {
                    return "\"" + param.Replace("\"", "\"\"") + "\"";
                }
            }
        }

        private String resultSumTime(cReportFormulaInt fint)
        {
            if (fint.getVariables().count() == 0) 
            { 
                return ""; 
            }
            cStructTime st = null;
            st = (cStructTime)fint.getVariables().item(C_SUM_TIME).getValue();
            if (cUtil.val(fint.getParameters().item(1).getValue()) != 0)
            {
                return cReportGlobals.format(st.getHour(), "00")
                        + ":" + cReportGlobals.format(st.getMinute(), "00")
                        + ":" + cReportGlobals.format(st.getSecond(), "00");
            }
            else
            {
                return cReportGlobals.format(st.getHour(), "00") + ":" + cReportGlobals.format(st.getMinute(), "00");
            }
        }

        private double resultSum(cReportFormulaInt fint)
        {
            if (fint.getVariables().count() == 0) { return 0; }
            return Convert.ToDouble(fint.getVariables().item(C_SUM).getValue());
        }

        private object resultGetDataFromRsAd(cReportFormulaInt fint)
        {
            return null;
        }

        private object resultGetDataFromRs(cReportFormulaInt fint)
        {
            return null;
        }

        private object resultGetVar(cReportFormulaInt fint)
        {
            String varName = "";
            varName = fint.getParameters().item(0).getValue();

            if (m_variables.item(varName) == null)
            {
                throw new ReportArgumentMissingException(
                    C_MODULE,
                    cReportError.errGetDescript(
                                    csRptErrors.CSRPTERRMISSINGPARAM,
                                    varName,
                                    "_getVar()"));
            }
            return m_variables.item(varName).getValue();
        }

        private object resultGetParam(cReportFormulaInt fint)
        {
            cParameter param = null;
            String paramName = "";

            paramName = fint.getParameters().item(0).getValue();

            for (int _i = 0; _i < m_report.getConnect().getParameters().count(); _i++)
            {
                param = m_report.getConnect().getParameters().item(_i);
                if (param.getName().ToLower() == paramName.ToLower())
                {
                    break;
                }
            }

            if (param == null)
            {
                throw new ReportArgumentMissingException(
                    C_MODULE,
                    cReportError.errGetDescript(
                                    csRptErrors.CSRPTERRMISSINGPARAM,
                                    paramName,
                                    "_getParameter()"));
            }

            return param.getValue();
        }

        private double resultMax(cReportFormulaInt fint)
        {
            if (fint.getVariables().count() == 0) { return 0; }
            return (double)fint.getVariables().item(C_MAX).getValue();
        }

        private double resultMin(cReportFormulaInt fint)
        {
            if (fint.getVariables().count() == 0) { return 0; }
            return (double)fint.getVariables().item(C_MIN).getValue();
        }

        private object resultCount(cReportFormulaInt fint)
        {
            if (fint.getVariables().count() == 0) { return null; }
            return fint.getVariables().item(C_COUNT).getValue();
        }

        private object resultNumberToString(cReportFormulaInt fint)
        {
            if (fint.getVariables().count() > 0)
            {
                return fint.getVariables().item(C_NUMBER_TO_STRING).getValue();
            }
            else
            {
                return "";
            }
        }

        private object resultGetBarcode(cReportFormulaInt fint)
        {
            if (fint.getVariables().count() > 0)
            {
                return fint.getVariables().item(C_GET_BARCODE).getValue();
            }
            else
            {
                return "";
            }
        }

        private object resultIsEqual(cReportFormulaInt fint)
        {
            if (fint.getVariables().count() > 0)
            {
                return fint.getVariables().item(C_ISEQUAL).getValue();
            }
            else
            {
                return 0;
            }
        }

        private object resultIsNotEqual(cReportFormulaInt fint)
        {
            if (fint.getVariables().count() > 0)
            {
                return fint.getVariables().item(C_ISNOTEQUAL).getValue();
            }
            else
            {
                return 0;
            }
        }

        private object resultIsGreaterThan(cReportFormulaInt fint)
        {
            if (fint.getVariables().count() > 0)
            {
                return fint.getVariables().item(C_ISGREATERTHAN).getValue();
            }
            else
            {
                return 0;
            }
        }

        private object resultIsLessThan(cReportFormulaInt fint)
        {
            if (fint.getVariables().count() > 0)
            {
                return fint.getVariables().item(C_ISLESSTHAN).getValue();
            }
            else
            {
                return 0;
            }
        }

        private double resultAverage(cReportFormulaInt fint)
        {
            if (fint.getVariables().count() == 0) 
            { 
                return 0; 
            }
            double sum = (double)fint.getVariables().item(C_AVERAGE_SUM).getValue();
            double count = (double)fint.getVariables().item(C_AVERAGE_COUNT).getValue();
            return sum / count;
        }

        private double resultCalculo(cReportFormulaInt fint)
        {
            String control = "";
            double value1 = 0;
            double value2 = 0;
            int oper = 0;

            control = fint.getParameters().item(1).getValue();

            value1 = Convert.ToDouble(m_report.getValue(fint.getParameters().item(0).getValue(), true));

            if (control != "\"\"")
            {
                value2 = Convert.ToDouble(m_report.getValue(control, true));
            }
            else
            {
                value2 = double.Parse(fint.getParameters().item(2).getValue());
            }

            oper = int.Parse(fint.getParameters().item(3).getValue());

            switch (oper)
            {
                // addition
                case 1:
                    return value1 + value2;
                    
                // substraction
                case 2:
                    return value1 - value2;
                    
                // multiplication
                case 3:
                    return value1 * value2;
                    
                // division
                case 4:
                    return cUtil.divideByZero(value1, value2);
                    
                // power
                case 5:
                    return Math.Pow(value1, ((int)value2));
                    
                default:
                    return 0;                    
            }
        }

        private int resultLength(cReportFormulaInt fint)
        {
            return m_report.getValueString(fint.getParameters().item(0).getValue()).Length;
        }

        private String resultTextReplace(cReportFormulaInt fint)
        {
            int i = 0;
            cReportControl ctrl = null;
            String text = "";
            List<String> collCtrlsToReplace = null;

            ctrl = pGetControl(m_ctrlName);
            if (ctrl == null)
            {
                return "";
            }

            text = ctrl.getLabel().getText();

            try
            {
                collCtrlsToReplace = m_collTextReplace[m_ctrlName];
            }
            catch
            {
                int lenText = 0;
                int pos = 0;
                int endpos = 0;

                collCtrlsToReplace = new List<String>();

                lenText = text.Length;
                while (i < lenText)
                {
                    pos = text.IndexOf(C_MACRO_CTRL, i + 1);
                    if (pos > 0)
                    {
                        endpos = text.IndexOf(C_MACRO_CTRL, pos + 1);

                        if (endpos > 0)
                        {
                            collCtrlsToReplace.Add(text.Substring(pos + 2, endpos - pos - 2));
                        }
                        i = endpos + 1;
                    }
                    else
                    {
                        i = lenText + 1;
                    }
                }

                m_collTextReplace.Add(m_ctrlName, collCtrlsToReplace);
            }

            cReportControl ctrlValue = null;
            for (i = 0; i < collCtrlsToReplace.Count; i++)
            {
                ctrlValue = pGetControl(collCtrlsToReplace[i]);
                if (ctrlValue != null)
                {
                    text = text.Replace(C_MACRO_CTRL + collCtrlsToReplace[i] + C_MACRO_CTRL,
                                        m_report.getValue(ctrlValue.getName(), false).ToString());
                }
            }
            return text;
        }

        private object resultValue(cReportFormulaInt fint)
        {
            return m_report.getValue(fint.getParameters().item(0).getValue(), true);
        }

        private int resultPageNumber()
        {
            return m_report.getCurrenPage();
        }

        private object resultTotalPages()
        {
            return m_report.getTotalPages();
        }

        private object resultGroupTotal(cReportFormulaInt fint)
        {
            if (fint.getVariables().count() > 0)
            {
                return fint.getVariables().item(C_GROUPTOTAL).getValue();
            }
            else
            {
                return 0;
            }
        }

        private object resultGroupMax(cReportFormulaInt fint)
        {
            if (fint.getVariables().count() > 0)
            {
                return fint.getVariables().item(C_GROUPMAX).getValue();
            }
            else
            {
                return 0;
            }
        }

        private object resultGroupMin(cReportFormulaInt fint)
        {
            if (fint.getVariables().count() > 0)
            {
                return fint.getVariables().item(C_GROUPMIN).getValue();
            }
            else
            {
                return 0;
            }
        }

        private object resultGroupAverage(cReportFormulaInt fint)
        {
            if (fint.getVariables().count() > 0)
            {
                return fint.getVariables().item(C_GROUPAVERAGE).getValue();
            }
            else
            {
                return 0;
            }
        }

        private object resultGroupPercent(cReportFormulaInt fint)
        {
            if (fint.getVariables().count() > 0)
            {
                return fint.getVariables().item(C_GROUPPERCENT).getValue();
            }
            else
            {
                return 0;
            }
        }

        private object resultGroupCount(cReportFormulaInt fint)
        {
            if (fint.getVariables().count() > 0)
            {
                return fint.getVariables().item(C_GROUPCOUNT).getValue();
            }
            else
            {
                return 0;
            }
        }

        private object resultGroupLineNumber(cReportFormulaInt fint)
        {
            if (fint.getVariables().count() > 0)
            {
                return fint.getVariables().item(C_GROUPLINENUMBER).getValue();
            }
            else
            {
                return 0;
            }
        }

        private object resultIsInRs(cReportFormulaInt fint)
        {
            if (fint.getVariables().count() > 0)
            {
                return fint.getVariables().item(C_ISINRS).getValue();
            }
            else
            {
                return 0;
            }
        }

        private void evalAverage(cReportFormulaInt fint)
        {
            if (fint.getVariables().item(C_AVERAGE_SUM) == null)
            {
                fint.getVariables().add(null, C_AVERAGE_SUM);
                fint.getVariables().add(null, C_AVERAGE_COUNT);
            }

            cReportVariable item = fint.getVariables().item(C_AVERAGE_SUM);
            // the average function is for numbers
            //
            item.setValue((double)item.getValue()
                + pGetNumber(m_report.getValue(fint.getParameters().item(0).getValue(), true)));

            item = fint.getVariables().item(C_AVERAGE_COUNT);
            // the average function is for numbers
            //
            item.setValue((double)item.getValue() + 1);
        }

        private double pGetNumber(object number)
        {
            String strNumber = number.ToString();
            double rtn = 0;
            String sepDecimal = "";

            if (G.isNumeric(strNumber))
            {
                sepDecimal = cUtil.getSepDecimal();
                if (sepDecimal != ".")
                {
                    strNumber = strNumber.Replace(".", sepDecimal);
                }
                rtn = cUtil.val(strNumber);

            }

            return rtn;
        }

        private void evalSum(cReportFormulaInt fint)
        {
            if (fint.getVariables().item(C_SUM) == null)
            {
                fint.getVariables().add(null, C_SUM).setValue(0);
            }

            cReportVariable item = fint.getVariables().item(C_SUM);
            // the sum function is for numbers
            //
            item.setValue(Convert.ToDouble(item.getValue())
                + pGetNumber(m_report.getValue(fint.getParameters().item(0).getValue(), true)));
        }

        private void evalDeclareVar(cReportFormulaInt fint)
        {
            String varName = "";

            varName = fint.getParameters().item(0).getValue();

            if (m_variables.item(varName) == null)
            {
                m_variables.add(null, varName);
            }
        }

        private void evalSetVar(cReportFormulaInt fint)
        {
            String varName = "";

            varName = fint.getParameters().item(0).getValue();

            if (m_variables.item(varName) == null)
            {
                throw new ReportArgumentMissingException(
                    C_MODULE,
                    cReportError.errGetDescript(
                                    csRptErrors.CSRPTERRMISSINGPARAM,
                                    varName,
                                    "_setVar"));
            }

            cReportVariable item = m_variables.item(varName);
            item.setValue(fint.getParameters().item(1).getValue());
        }

        private void evalGetDataFromRsAd(cReportFormulaInt fint)
        {

        }

        private void evalGetDataFromRs(cReportFormulaInt fint)
        {

        }

        private void evalAddToVar(cReportFormulaInt fint)
        {
            String varName = "";

            varName = fint.getParameters().item(0).getValue();

            if (m_variables.item(varName) == null)
            {
                throw new ReportArgumentMissingException(
                    C_MODULE,
                    cReportError.errGetDescript(
                                    csRptErrors.CSRPTERRMISSINGPARAM,
                                    varName,
                                    "_evalAddToVar"));
            }

            cReportVariable item = m_variables.item(varName);
            // the EvalAddToVar function is for numbers
            //
            item.setValue((double)item.getValue() 
                                + pGetNumber(fint.getParameters().item(1).getValue()));
        }

        private void evalSumTime(cReportFormulaInt fint)
        {
            if (fint.getVariables().item(C_SUM_TIME) == null)
            {
                fint.getVariables().add(null, C_SUM_TIME).setValue(new cStructTime());
            }

            cReportVariable item = fint.getVariables().item(C_SUM_TIME);
            // the SumTime if for dates
            //
            pSumTimes((cStructTime)item.getValue(),
                        DateTime.Parse(m_report.getValue(fint.getParameters().item(0).getValue(), true).ToString()));
        }

        private void evalMax(cReportFormulaInt fint)
        {
            object value = null;

            if (fint.getVariables().item(C_MAX) == null)
            {
                fint.getVariables().add(null, C_MAX);
            }

            cReportVariable item = fint.getVariables().item(C_MAX);
            // the Max function if for numbers and strings
            //
            value = m_report.getValue(fint.getParameters().item(0).getValue());

            if (value.GetType() == typeof(String))
            {
                if (String.Compare(item.getValue().ToString(), 
                                    value.ToString(), 
                                    StringComparison.CurrentCulture) < 0)
                {
                    item.setValue(value);
                }
            }
            else
            {
                if ((double)item.getValue() < (double)value)
                {
                    item.setValue(value);
                }
            }
        }

        private void evalMin(cReportFormulaInt fint)
        {
            object value = null;

            if (fint.getVariables().item(C_MIN) == null)
            {
                fint.getVariables().add(null, C_MIN);
            }

            cReportVariable item = fint.getVariables().item(C_MIN);
            // The Min function is for numbers and strings
            //
            value = m_report.getValue(fint.getParameters().item(0).getValue());

            if (value.GetType() == typeof(String))
            {
                if (String.Compare(item.getValue().ToString(),
                                    value.ToString(),
                                    StringComparison.CurrentCulture) > 0)
                {
                    item.setValue(value);
                }
            }
            else
            {
                if ((double)item.getValue() > (double)value)
                {
                    item.setValue(value);
                }
            }
        }

        private void evalCount(cReportFormulaInt fint)
        {
            if (fint.getVariables().item(C_COUNT) == null)
            {
                fint.getVariables().add(null, C_COUNT);
            }

            cReportVariable item = fint.getVariables().item(C_COUNT);
            // the Count functio is for numbers
            //
            item.setValue((double)item.getValue() + 1);
        }

        private void evalNumberToString(cReportFormulaInt fint)
        {
            if (fint.getVariables().item(C_NUMBER_TO_STRING) == null)
            {
                fint.getVariables().add(null, C_NUMBER_TO_STRING);
            }

            cReportVariable item = fint.getVariables().item(C_NUMBER_TO_STRING);
            // the NumberToString funciton is for numbres
            //
            double iNumber = 0;
            int iLenguage = 0;

            iNumber = pGetNumber(m_report.getValue(fint.getParameters().item(0).getValue(), true));
            iLenguage = cUtil.valAsInt(fint.getParameters().item(1).getValue());

            cNumberToString ntos = new cNumberToString();

            switch (iLenguage)
            {
                case C_SPANISH:
                    item.setValue(ntos.spanishNumberToString(iNumber));
                    break;
                case C_ENGLISH:
                    item.setValue(ntos.englishNumberToString(iNumber));
                    break;
                case C_FRENCH:
                    item.setValue(ntos.frenchNumberToString(iNumber));
                    break;
            }
        }

        private void evalIsEqual(cReportFormulaInt fint)
        {
            if (fint.getVariables().item(C_ISEQUAL) == null)
            {
                fint.getVariables().add(null, C_ISEQUAL);
            }

            cReportVariable item = fint.getVariables().item(C_ISEQUAL);
            // the IsEqual function is for numbers
            //
            String strValue = "";
            String strConstValue = "";

            strValue = m_report.getValue(fint.getParameters().item(0).getValue(), true).ToString();
            strConstValue = fint.getParameters().item(1).getValue();

            item.setValue(strValue == strConstValue);
        }

        private void evalIsNotEqual(cReportFormulaInt fint)
        {
            if (fint.getVariables().item(C_ISNOTEQUAL) == null)
            {
                fint.getVariables().add(null, C_ISNOTEQUAL);
            }

            cReportVariable item = fint.getVariables().item(C_ISNOTEQUAL);
            // the IsNotEqual function is for numbers
            //
            String strValue = "";
            String strConstValue = "";

            strValue = (String)m_report.getValue(fint.getParameters().item(0).getValue(), true);
            strConstValue = fint.getParameters().item(1).getValue();

            item.setValue(strValue != strConstValue);
        }

        private void evalIsGreaterThan(cReportFormulaInt fint)
        {
            if (fint.getVariables().item(C_ISGREATERTHAN) == null)
            {
                fint.getVariables().add(null, C_ISGREATERTHAN);
            }

            cReportVariable item = fint.getVariables().item(C_ISGREATERTHAN);
            // the IsGreaterThan function is for numbers
            //
            object value = m_report.getValue(fint.getParameters().item(0).getValue(), true);
            object constValue = fint.getParameters().item(1).getValue();

            if (value.GetType() == typeof(String))
            {
                String strValue = value.ToString();
                String strConstValue = constValue.ToString();

                if (String.Compare(strValue.ToString(),
                                    strConstValue.ToString(),
                                    StringComparison.CurrentCulture) > 0)
                {
                    item.setValue(true);
                }
                else
                {
                    item.setValue(false);
                }
            }
            else
            {
                if ((double)value > (double)constValue)
                {
                    item.setValue(true);
                }
                else 
                {
                    item.setValue(false);
                }
            }
        }

        private void evalIsLessThan(cReportFormulaInt fint)
        {
            if (fint.getVariables().item(C_ISLESSTHAN) == null)
            {
                fint.getVariables().add(null, C_ISLESSTHAN);
            }

            cReportVariable item = fint.getVariables().item(C_ISLESSTHAN);
            // the IsLessThan function is for numbers
            //
            object value = m_report.getValue(fint.getParameters().item(0).getValue(), true);
            object constValue = fint.getParameters().item(1).getValue();

            if (value.GetType() == typeof(String))
            {
                String strValue = value.ToString();
                String strConstValue = constValue.ToString();

                if (String.Compare(strValue.ToString(),
                                    strConstValue.ToString(),
                                    StringComparison.CurrentCulture) < 0)
                {
                    item.setValue(true);
                }
                else
                {
                    item.setValue(false);
                }
            }
            else
            {
                if ((double)value < (double)constValue)
                {
                    item.setValue(true);
                }
                else
                {
                    item.setValue(false);
                }
            }
        }

        private void evalGroupTotal(cReportFormulaInt fint)
        {
            if (fint.getVariables().item(C_GROUPTOTAL) == null)
            {
                fint.getVariables().add(null, C_GROUPTOTAL);
            }

            cReportVariable item = fint.getVariables().item(C_GROUPTOTAL);
            // the Total function is for numbres

            // if param1 doesn't contain an index column is because we haven't
            // process the formulas yet. It happens because compilereport
            // is called before the InitColIndex in cReport's Launch function
            // and the order can not be changed because the function GetData 
            // is executed after the CompileReport function, and we don't want
            // to change this order because we are afraid of the collateral damage
            // it could produce :(
            //
            // In the future we can analize it and modify the order and if this
            // doesn't produce any error we will remove this if :)
            //
            if (fint.getParameters().item(cReportGlobals.C_KEYINDEXCOL) == null)
            {
                item.setValue(0);
            }
            else
            {
                item.setValue(
                    m_report.getGroupTotal(
                        int.Parse(fint.getParameters().item(cReportGlobals.C_KEYINDEXCOL).getValue()),
                        int.Parse(fint.getParameters().item(cReportGlobals.C_KEYINDEXGROUP).getValue())));
            }
        }

        private void evalGroupMax(cReportFormulaInt fint)
        {
            if (fint.getVariables().item(C_GROUPMAX) == null)
            {
                fint.getVariables().add(null, C_GROUPMAX);
            }

            cReportVariable item = fint.getVariables().item(C_GROUPMAX);
            // the Group Max function is for numbers and strings

            // if param1 doesn't contain an index column is because we haven't
            // process the formulas yet. It happens because compilereport
            // is called before the InitColIndex in cReport's Launch function
            // and the order can not be changed because the function GetData 
            // is executed after the CompileReport function, and we don't want
            // to change this order because we are afraid of the collateral damage
            // it could produce :(
            //
            // In the future we can analize it and modify the order and if this
            // doesn't produce any error we will remove this if :)
            //
            if (fint.getParameters().item(cReportGlobals.C_KEYINDEXCOL) == null)
            {
                item.setValue(0);
            }
            else
            {
                item.setValue(
                    m_report.getGroupMax(
                                int.Parse(fint.getParameters().item(cReportGlobals.C_KEYINDEXCOL).getValue()),
                                int.Parse(fint.getParameters().item(cReportGlobals.C_KEYINDEXGROUP).getValue())));
            }
        }

        private void evalGroupMin(cReportFormulaInt fint)
        {
            if (fint.getVariables().item(C_GROUPMIN) == null)
            {
                fint.getVariables().add(null, C_GROUPMIN);
            }

            cReportVariable item = fint.getVariables().item(C_GROUPMIN);
            // the Group Min function is for numbers and strings

            // if param1 doesn't contain an index column is because we haven't
            // process the formulas yet. It happens because compilereport
            // is called before the InitColIndex in cReport's Launch function
            // and the order can not be changed because the function GetData 
            // is executed after the CompileReport function, and we don't want
            // to change this order because we are afraid of the collateral damage
            // it could produce :(
            //
            // In the future we can analize it and modify the order and if this
            // doesn't produce any error we will remove this if :)
            //
            if (fint.getParameters().item(cReportGlobals.C_KEYINDEXCOL) == null)
            {
                item.setValue(0);
            }
            else
            {
                item.setValue(
                    m_report.getGroupMin(
                        int.Parse(fint.getParameters().item(cReportGlobals.C_KEYINDEXCOL).getValue()),
                        int.Parse(fint.getParameters().item(cReportGlobals.C_KEYINDEXGROUP).getValue())));
            }
        }

        private void evalGroupAverage(cReportFormulaInt fint)
        { 
            if (fint.getVariables().item(C_GROUPAVERAGE) == null)
            {
                fint.getVariables().add(null, C_GROUPAVERAGE);
            }

            cReportVariable item = fint.getVariables().item(C_GROUPAVERAGE);
            // the Average function is for numbers

            // if param1 doesn't contain an index column is because we haven't
            // process the formulas yet. It happens because compilereport
            // is called before the InitColIndex in cReport's Launch function
            // and the order can not be changed because the function GetData 
            // is executed after the CompileReport function, and we don't want
            // to change this order because we are afraid of the collateral damage
            // it could produce :(
            //
            // In the future we can analize it and modify the order and if this
            // doesn't produce any error we will remove this if :)
            //
            if (fint.getParameters().item(cReportGlobals.C_KEYINDEXCOL) == null)
            {
                item.setValue(0);
            }
            else
            {
                item.setValue(
                    m_report.getGroupAverage(
                        int.Parse(fint.getParameters().item(cReportGlobals.C_KEYINDEXCOL).getValue()),
                        int.Parse(fint.getParameters().item(cReportGlobals.C_KEYINDEXGROUP).getValue())));
            }
        }

        // this function only get the total of the group
        // the percent is calculated in the function ResultGroupPercent
        //
        private void evalGroupPercent(cReportFormulaInt fint)
        {
            if (fint.getVariables().item(C_GROUPPERCENTT) == null)
            {
                fint.getVariables().add(null, C_GROUPPERCENTT);
            }

            if (fint.getVariables().item(C_GROUPPERCENT) == null)
            {
                fint.getVariables().add(null, C_GROUPPERCENT);
            }

            cReportVariable item = fint.getVariables().item(C_GROUPPERCENTT);
            // the Percent function is for numbers

            // if param1 doesn't contain an index column is because we haven't
            // process the formulas yet. It happens because compilereport
            // is called before the InitColIndex in cReport's Launch function
            // and the order can not be changed because the function GetData 
            // is executed after the CompileReport function, and we don't want
            // to change this order because we are afraid of the collateral damage
            // it could produce :(
            //
            // In the future we can analize it and modify the order and if this
            // doesn't produce any error we will remove this if :)
            //
            if (fint.getParameters().item(cReportGlobals.C_KEYINDEXCOL) == null)
            {
                item.setValue(0);
            }
            else
            {
                item.setValue(
                    m_report.getGroupTotal(
                        int.Parse(fint.getParameters().item(cReportGlobals.C_KEYINDEXCOL).getValue()), 
                        int.Parse(fint.getParameters().item(cReportGlobals.C_KEYINDEXGROUP).getValue())));
            }
            pEvalFunctionGroup(fint);
        }

        private void evalGroupCount(cReportFormulaInt fint)
        {
            if (fint.getVariables().item(C_GROUPCOUNT) == null)
            {
                fint.getVariables().add(null, C_GROUPCOUNT);
            }

            cReportVariable item = fint.getVariables().item(C_GROUPCOUNT);
            // the Count function is for numbers

            // if param1 doesn't contain an index column is because we haven't
            // process the formulas yet. It happens because compilereport
            // is called before the InitColIndex in cReport's Launch function
            // and the order can not be changed because the function GetData 
            // is executed after the CompileReport function, and we don't want
            // to change this order because we are afraid of the collateral damage
            // it could produce :(
            //
            // In the future we can analize it and modify the order and if this
            // doesn't produce any error we will remove this if :)
            //
            if (fint.getParameters().item(cReportGlobals.C_KEYINDEXCOL) == null)
            {
                item.setValue(0);
            }
            else
            {
                item.setValue(
                    m_report.getGroupCount(
                        int.Parse(fint.getParameters().item(cReportGlobals.C_KEYINDEXCOL).getValue()),
                        int.Parse(fint.getParameters().item(cReportGlobals.C_KEYINDEXGROUP).getValue())));
            }
        }

        private void evalGroupLineNumber(cReportFormulaInt fint)
        {
            if (fint.getVariables().item(C_GROUPLINENUMBER) == null)
            {
                fint.getVariables().add(null, C_GROUPLINENUMBER);
            }

            cReportVariable item = fint.getVariables().item(C_GROUPLINENUMBER);
            // the LineNumber function is for numbers
            item.setValue(
                m_report.getGroupLineNumber(
                    int.Parse(fint.getParameters().item(cReportGlobals.C_KEYINDEXGROUP).getValue())));
        }

        private void evalIsInRs(cReportFormulaInt fint)
        {
            if (fint.getVariables().item(C_ISINRS) == null)
            {
                fint.getVariables().add(null, C_ISINRS);
            }

            cReportVariable item = fint.getVariables().item(C_ISINRS);
            // TODO: finish coding evalIsInRs
            //
            item.setValue(true);
        }

        private void evalGetBarcode(cReportFormulaInt fint)
        {
            if (fint.getVariables().item(C_GET_BARCODE) == null)
            {
                fint.getVariables().add(null, C_GET_BARCODE);
            }

            cReportVariable item = fint.getVariables().item(C_GET_BARCODE);

            var barcodeGen = new CSReportBarcode.cReportBarcode();
            var value = fint.getParameters().item(0).getValue();
            var barcode = barcodeGen.encodeTo128(value);

            if (barcode.Contains("Ã‚")) barcode = barcodeGen.code128a(value);

            item.setValue(barcode);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        private void pCheckParameters(int cantParams, String parameters, String name)
        {
            for (int i = 0; i < cantParams; i++)
            {
                // It must receive the control name
                //
                string param = pGetParameter(parameters, i, name);

                if (param.Length == 0)
                {
                    throw new ReportArgumentMissingException(
                        C_MODULE,
                        cReportError.errGetDescript(
                                        csRptErrors.CSRPTERRMISSINGPARAM,
                                        i.ToString(),
                                        name));
                }

                m_fint.getParameters().add(param);
            }
        }

        private csRptFormulaType pGetIdFunction(String name)
        {
            cReportFormulaType f = null;

            name = name.ToLower();
            for (int _i = 0; _i < m_formulaTypes.count(); _i++)
            {
                f = m_formulaTypes.item(_i);
                if (name == f.getName().ToLower())
                {
                    return f.getId();
                }
            }
            return 0;
        }

        private bool pIsSeparator(String c)
        {
            return " |:+()/-*=\r\n".IndexOf(c, 0) > -1 && c != "";
        }

        private String removeReturns(String code)
        {
            String c = "";
            for (int i = 0; i < code.Length; i++)
            {
                c = code.Substring(i, 1);
                if (c != " " && c != "\r" && c != "\n") {
                    code = code.Substring(i);
                    break; 
                }
            }

            return code;
        }

        // Dates start 1-1-1900 00:00:00
        //
        private void pSumTimes(cStructTime st, DateTime date2)
        {
            int n2 = 0;
            int h2 = 0;
            int s2 = 0;

            int n = 0;
            int h = 0;
            int s = 0;
            int d = 0;

            s2 = date2.Second;
            n2 = date2.Minute;
            h2 = date2.Hour;

            // get seconds
            //
            s = (st.getSecond() + s2) % 60;

            // get minutes
            //
            n = (int)((st.getSecond() + s2) / 60);
            n = n + (st.getMinute() + n2) % 60;

            // get hours
            //
            h = (int)((st.getMinute() + n2) / 60);
            h = h + st.getHour() + h2;

            st.setSecond(s);
            st.setMinute(n);
            st.setHour(h);
        }

        private bool pCompileAux(String code, out String codeC)
        {
            String codeCallFunction = "";
            String codeCallFunctionC = "";
            String functionName = "";
            String word = "";

            int nStart = 0;
            int nLenCode = code.Length;

            codeC = "";

            do
            {
                word = pGetWord(code, ref nStart);
                if (pIsFunctionAux(word, ref functionName))
                {

                    codeCallFunction = pGetCallFunction(code, ref nStart);

                    if (!pCompileAux(codeCallFunction, out codeCallFunctionC))
                    {
                        return false;
                    }

                    codeC = codeC + pExecFunction(functionName, codeCallFunctionC);
                }
                else
                {
                    codeC = codeC + word;
                }
            } while (nStart < nLenCode);

            return true;
        }

        private String pGetWord(String code, ref int nStart)
        {
            String c = "";
            int nLenCode = 0;
            String word = "";

            nLenCode = code.Length;

            c = code.Substring(nStart, 1);

            do
            {
                word += c;
                nStart += 1;
                if (pIsSeparator(c)) break;
                c = cUtil.subString(code, nStart, 1);
            } while (!pIsSeparator(c) && nStart < nLenCode);

            return word;
        }

        private bool pIsFunctionAux(String word, ref String functionName)
        {
            if (!pIsFunction(word)) { return false; }
            functionName = word;
            return true;
        }

        private String pGetCallFunction(String code, ref int nStart)
        {
            String c = "";
            int nLenCode = 0;
            String word = "";
            int nInner = 0;

            nLenCode = code.Length;
            nInner = -1;

            do
            {
                c = code.Substring(nStart, 1);
                word = word + c;
                nStart = nStart + 1;
            } while (!pIsEndCallFunction(c, ref nInner) && nStart < nLenCode);

            return word;
        }

        private bool pIsEndCallFunction(String c, ref int nInner)
        {
            bool _rtn = false;
            if (c == ")")
            {
                if (nInner == 0)
                {
                    _rtn = true;
                }
                else
                {
                    nInner = nInner - 1;
                }
            }
            else if (c == "(")
            {
                nInner = nInner + 1;
            }
            return _rtn;
        }

        private String pExecFunction(String functionName, String parameters)
        {
            if (m_bCompile)
            {
                return pAddFormulaInt(functionName, parameters).ToString();
            }
            else
            {
                cReportFormulaInt fint = null;
                m_idxFormula = m_idxFormula + 1;
                fint = m_formula.getFormulasInt().item(m_idxFormula);
                pSetParams(fint, parameters);
                pEvalFunctionInt(fint);
                object value = pResultFunctionInt(fint);
                if (value != null)
                {
                    return getNumericVal(value.ToString());
                }
                else
                {
                    return "";
                }
            }
        }

        private void pSetParams(cReportFormulaInt fint, String parameters)
        {
            String[] vParams = null;
            int i = 0;

            parameters = parameters.Trim();
            if (parameters.Length > 2)
            {
                parameters = parameters.Substring(1, parameters.Length - 2);
                parameters = parameters.Trim();
                vParams = parameters.Split('|');

                for (i = 0; i < vParams.Length; i++)
                {
                    fint.getParameters().item(i).setValue(vParams[i].Trim());
                }
            }
        }

        private String getNumericVal(String value)
        {
            int decimalDigit = 0;
            decimalDigit = value.IndexOf(",", 0);
            if (decimalDigit > 0)
            {
                value = value.Replace(",", ".");
            }
            return value;
        }

    }

}
