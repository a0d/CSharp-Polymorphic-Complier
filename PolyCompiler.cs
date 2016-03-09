using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;

public class PolyCompiler
{
   public static Dictionary<string, CompilerResults> Compiled { get; set; } = new Dictionary<string, CompilerResults>();

    public void Compile(string source)
    {
        //Set parameters to generate in memory as opposed to creating an executable file
        var myParams = new CompilerParameters
        {
            GenerateExecutable = false,
            GenerateInMemory = true
        };
        var references = GetRefrences(source);
        foreach (string reference in references)
        {
            myParams.ReferencedAssemblies.Add(reference);
        }
        //Declare compiler vars
        var compilerProvider = CodeDomProvider.CreateProvider("CSharp");
        //Polymorph the source code
        var polymorphicCode = PolyGeneration(source);
        //Compile the code and check for errors
        var codeDomResults = compilerProvider.CompileAssemblyFromSource(myParams, polymorphicCode[0]);
        if (!codeDomResults.Errors.HasErrors)
        {
            //If there are no errors invoke the method passed in the args
            var ty = codeDomResults.CompiledAssembly.GetType(polymorphicCode[1]);
            var o = Activator.CreateInstance(ty);
            ty.InvokeMember(polymorphicCode[2], BindingFlags.InvokeMethod, null, o, null);
        }
        else
        {
            foreach (CompilerError error in codeDomResults.Errors)
            {
                //Handle errors here
                Console.WriteLine(error.ErrorText);
            }
        }
    }

    private static Array GetRefrences(string source)
    {
        //Match all text begging with 'using' and ending with ';'\
        var reg = new Regex(@"using (.*);", RegexOptions.Compiled);
        //Loop through the matches
        return (from Match reference in reg.Matches(source) let library = reference.Groups[1].ToString() select reference.Groups[1].ToString().Replace(";", string.Empty) into library select library + ".dll").ToArray();
    }
    private string[] PolyGeneration(string source)
    {
        //Match all poly declarations
        var reg = new Regex(@"{(.+?)}", RegexOptions.Compiled);
        //Array to hold new polymorphic source code and entry methods
        var polysource = new List<string>{ source, null, null }.ToArray();
        var numberofpolys = reg.Matches(source).Count;
        var polydeclartionmatch = reg.Matches(source);
        //Make sure there is at least one poly declaration
        if (numberofpolys <= 0) return polysource;
        for (var i = 0; i <= numberofpolys - 1; i++)
        {
            var polydeclaration = polydeclartionmatch[i].Groups[0].ToString();
            var rand = generate_alphanumeric();
            //If this poly declaration is an entry point method, save the new name in the poly source array
            switch (polydeclaration)
            {
                case @"{0}":
                    polysource[1] = rand;
                break;
                case @"{1}":
                    polysource[2] = rand;
                break;
            }
            //Replace poly declaration group with the new generated poly name
            var regex = new Regex(Regex.Escape(polydeclaration));
            polysource[0] = regex.Replace(polysource[0], rand);
        }
            return polysource;
    }

    private Random _polyrand = new Random();
    private string generate_alphanumeric()
    {
        _polyrand = new Random(Guid.NewGuid().GetHashCode());
        //Generate a random number between 1 and 49 for the length of the poly name
        var polylength = _polyrand.Next(10, 50);
        //Define acceptable set of characters to generate a random sequence from
        const string polychars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        //Generate a random string using the poly chars char set and randomly generated poly length using Linq
        var poly = new string(Enumerable.Repeat(polychars, polylength).Select(s => s[_polyrand.Next(s.Length)]).ToArray());
        return poly;
    }
}
