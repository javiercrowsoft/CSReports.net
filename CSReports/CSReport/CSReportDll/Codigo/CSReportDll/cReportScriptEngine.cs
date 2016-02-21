using System;
using System.Reflection;
using System.CodeDom.Compiler;
using CSKernelClient;
using CSReportScript;

namespace CSReportDll
{
    internal static class cReportScriptEngine
    {

        private static string getFunctionCall(string code, cReportFormula formula)
        { 
            int n = code.IndexOf("(");
            var functionName = cUtil.subString(code, 8, n-8);
            var parameters = "";
            for (int _i = 0; _i < formula.getFormulasInt().count(); _i++)
            {
                var fint = formula.getFormulasInt().item(_i);
                parameters += "globals.getVar(\"p__" + _i + "__\").getValue(),";
            }
            if (parameters.Length > 0)
            {
                parameters = parameters.Substring(0, parameters.Length - 1);
            }
            return functionName + "(" + parameters + ")";
        }

        private static string putCodeInClass(string code, cReportFormula formula)
        {
            if (cUtil.subString(code, 0, 8).ToLower() == "function")
            {
                return "Public Class util\r\n"
                     + "Implements CSReportScript.cIReportScriptType\r\n"
                     + code + "\r\n"
                     + "Public Function RunScript(globals As CSReportScript.cReportCompilerGlobals) As String Implements CSReportScript.cIReportScriptType.RunScript\r\n"
                     + "  dim value__ = " + getFunctionCall(code, formula) + "\r\n"

                     // TODO: remove debug info
                     /*
                     + "  System.Console.WriteLine(\"" + formula.getName() + "\")\r\n"
                     + "  Dim var__\r\nFor Each var__ In globals\r\n System.Console.WriteLine(var__.ToString() + \" : \" + globals.getVar(var__).getValue().ToString())\r\nNext\r\n"
                     + "  System.Console.WriteLine(value__.ToString())\r\n"
                     + "  System.Console.WriteLine(\"---------------\")\r\n"
                      */ 
                     // end debug info

                     + "  Select Case Microsoft.VisualBasic.Information.VarType(value__)\r\n"
                     + "    Case 11\r\n"
                     + "      RunScript = System.Convert.ToInt32(value__)\r\n"
                     + "    Case 7\r\n"
                     + "      RunScript = String.Format(\"{0:MM/dd/yyyy}\", value__)\r\n"
                     + "    Case Else\r\n"
                     + "      RunScript = value__\r\n"
                     + "  End Select\r\n"
                     + "End Function\r\n"
                     + "Function Now()\r\n"
                     + "  Now = Microsoft.VisualBasic.Now\r\n"
                     + "End Function\r\n"
                     + "Function FormatDateTime(aDate, format) As String\r\n"
                     + "    FormatDateTime = Microsoft.VisualBasic.Strings.FormatDateTime(aDate, format)\r\n"
                     + "End Function\r\n"
                     + "End Class";
            }
            else
            {
                // TODO: implement c# scripting
                //
                return "public class util: cIReportScriptType { public " + code + "}";
            }            
        }

        internal static Assembly compileCode(string code, cReportFormula formula)
        {
            // Create a code provider
            // This class implements the 'CodeDomProvider' class as its base. All of the current .Net languages (at least Microsoft ones)
            // come with thier own implemtation, thus you can allow the user to use the language of thier choice (though i recommend that
            // you don't allow the use of c++, which is too volatile for scripting use - memory leaks anyone?)

            CodeDomProvider provider;

            if (cUtil.subString(code, 0, 8).ToLower() == "function")
            {
                provider = new Microsoft.VisualBasic.VBCodeProvider();
            }
            else 
            {
                provider = new Microsoft.CSharp.CSharpCodeProvider();
            }

            // Setup our options
            CompilerParameters options = new CompilerParameters();
            options.GenerateExecutable = false; // we want a Dll (or "Class Library" as its called in .Net)
            options.GenerateInMemory = true; // Saves us from deleting the Dll when we are done with it, though you could set this to false and save start-up time by next time by not having to re-compile
            // And set any others you want, there a quite a few, take some time to look through them all and decide which fit your application best!

            // Add any references you want the users to be able to access, be warned that giving them access to some classes can allow
            // harmful code to be written and executed. I recommend that you write your own Class library that is the only reference it allows
            // thus they can only do the things you want them to.
            // (though things like "System.Xml.dll" can be useful, just need to provide a way users can read a file to pass in to it)
            // Just to avoid bloatin this example to much, we will just add THIS program to its references, that way we don't need another
            // project to store the interfaces that both this class and the other uses. Just remember, this will expose ALL public classes to
            // the "script"

            var assemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies();

            foreach (AssemblyName assemblyName in assemblies)
            {
                if (assemblyName.Name == "CSReportScript")
                {
                    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (assembly.GetName().Name == assemblyName.Name)
                        {
                            options.ReferencedAssemblies.Add(assembly.Location);
                            break;                        
                        }
                    }
                    break;
                }
            }            

            // Compile our code
            CompilerResults result;
            string classCode = putCodeInClass(code, formula);
            result = provider.CompileAssemblyFromSource(options, classCode);

            if (result.Errors.HasErrors)
            {
                var errors = "";

                for (int i = 0; i < result.Errors.Count; i++)
                {
                    errors += result.Errors[0].ErrorText + "\r\n";
                }

                cWindow.msgError(errors + "\r\n\r\nSource code:\r\n\r\n" + classCode + "\r\n\r\n");

                return null;
            }

            if (result.Errors.HasWarnings)
            {
                // TODO: tell the user about the warnings, might want to prompt them if they want to continue
                // runnning the "script"
            }

            return result.CompiledAssembly;
        }

        internal static object eval(Assembly script, cReportCompilerGlobals globals)
        {
            // Now that we have a compiled script, lets run them
            foreach (Type type in script.GetExportedTypes())
            {
                foreach (Type iface in type.GetInterfaces())
                {
                    if (iface == typeof(cIReportScriptType))
                    {
                        // yay, we found a script interface, lets create it and run it!

                        // Get the constructor for the current type
                        // you can also specify what creation parameter types you want to pass to it,
                        // so you could possibly pass in data it might need, or a class that it can use to query the host application
                        ConstructorInfo constructor = type.GetConstructor(System.Type.EmptyTypes);
                        if (constructor != null && constructor.IsPublic)
                        {
                            // lets be friendly and only do things legitimitely by only using valid constructors

                            // we specified that we wanted a constructor that doesn't take parameters, so don't pass parameters
                            cIReportScriptType scriptObject = constructor.Invoke(null) as cIReportScriptType;
                            if (scriptObject != null)
                            {
                                //Lets run our script and display its results
                                return scriptObject.RunScript(globals);
                            }
                            else
                            {
                                // hmmm, for some reason it didn't create the object
                                // this shouldn't happen, as we have been doing checks all along, but we should
                                // inform the user something bad has happened, and possibly request them to send
                                // you the script so you can debug this problem
                            }
                        }
                        else
                        {
                            // and even more friendly and explain that there was no valid constructor
                            // found and thats why this script object wasn't run
                        }
                    }
                }
            }
            return null;
        }
    }
}
