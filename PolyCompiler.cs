using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;
    class PolyCompiler
    {

        private static Dictionary<string, CompilerResults> Compiled = new Dictionary<string, CompilerResults>();
        public void Compile(string source)
        {
            //Set paramters to generate in memory as opposed to creating an excutable file
            CompilerParameters Params = new CompilerParameters();
            Params.GenerateExecutable = false;
            Params.GenerateInMemory = true;
            var references = (Array)GetRefrences(source);
            foreach(String reference in references)
            {
                Params.ReferencedAssemblies.Add(reference);
            }
            //Decalare compiler vars
            CodeDomProvider CompilerProvider = CodeDomProvider.CreateProvider("CSharp");
            //Polymorph-ize the source code
            string[] polymorphic_code = PolyGeneration(source);
            //Compile the code and check for errors
            CompilerResults CodeDomResults = CompilerProvider.CompileAssemblyFromSource(Params, polymorphic_code[0]);
            if (!CodeDomResults.Errors.HasErrors)
            {
                //If there are no errors invoke the method passed in the args
                Type ty = CodeDomResults.CompiledAssembly.GetType(polymorphic_code[1]);
                Object o = Activator.CreateInstance(ty);
                ty.InvokeMember(polymorphic_code[2], BindingFlags.InvokeMethod, null, o, null);
            }
            else
            {
                foreach (CompilerError error in CodeDomResults.Errors)
                {
                    //Handle errors here
                    
                }
            }
        }

       
        private Array GetRefrences (String source)
        {
            List<string> refs = new List<string>();
            //Match all text begging with 'using' and ending with ';'\
            Regex reg = new Regex(@"using (.*);", RegexOptions.Compiled);
            //Loop through the matches
            foreach (Match reference in reg.Matches(source))
            {
                string library = reference.Groups[1].ToString();
                //Get rid of the ';' character
                library = reference.Groups[1].ToString().Replace(";", string.Empty);
                //Add the assembly name to the refs list
                refs.Add(library.ToString() + ".dll");
            }
            return refs.ToArray(); 
        }
        private string[] PolyGeneration(string source)
        {
            //Match all poly declarations
            Regex reg = new Regex(@"{(.+?)}", RegexOptions.Compiled);
            //Array to hold new polymorphic source code and entry methods
            string [] polysource = new string[]{source,null,null};
            int numberofpolys = reg.Matches(source).Count;
            MatchCollection polydeclartionmatch_ = reg.Matches(source);
            //Make sure there is at least one poly declartaion
            if(numberofpolys > 0)
            {
                for (int i = 0; i <= numberofpolys -1; i++)
                {
                    string polydeclaration = polydeclartionmatch_[i].Groups[0].ToString();
                    string rand = generate_alphanumeric();
                    //If this poly declaration is an entry point method, save the new name in the polysource array
                   if (polydeclaration == @"{0}")
                   {
                        polysource[1] = rand;
                   }
                   else if (polydeclaration == @"{1}")
                   {
                        polysource[2] = rand;
                   }
                    //Replace polydeclration group with the new generated poly name
                    var regex = new Regex(Regex.Escape(polydeclaration));
                    polysource[0] = regex.Replace(polysource[0], rand);
                }
                   
            }
            return polysource;
        }

        private Random polyrand = new Random();
        private string generate_alphanumeric()
        {
            polyrand =  new Random(Guid.NewGuid().GetHashCode());
            //Generate a random number betwween 1 and 49 for the length of the polyname
            int polylength = polyrand.Next(10, 50);
            //Define acceptable set of characters to generate a random sequence from
            const string polychars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            //Generate a random string using the polychars char set and randomly generated polylength using Linq
            string poly = new string(Enumerable.Repeat(polychars, polylength).Select(s => s[polyrand.Next(s.Length)]).ToArray());
            return poly;
        }
    }