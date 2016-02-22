# Polymorphic Complier For CSharp
Simple on-the-fly polymorphic compiling in CSharp

This is a simple class to take raw CSharp code, compile it in memory and run it polymorphicly. It generates a random lower and upper case string between 10 and 50 characters for text flagged with a {*} (where * is a integer). 

Integers 0 & 1 are reserved for the entry points of your code so it can be ran after it compiles. An example code snippet that compiles using this method is as follows:

```C#
using System;
using System.Windows.Forms;
    public class {0}
    {
        public static void {1}()
        {
            MessageBox.Show("{3}");
        }
    }
```

Poly flags with the same identifier are grouped together so variable and referenced names will work.

```C#
using System;
using System.Windows.Forms;
    public class {0}
    {
        public static void {1}()
        {
			String {3} = "{4}";
            MessageBox.Show({3});
        }
    }
```

A simple two lines creates a PolyCompiler to create, compile, and run polymorphic code

```C#
PolyCompiler CodeCompiler = new PolyCompiler();        
CodeCompiler.Compile("source_code_here");
```
