//
// SimpleTest.cs
//
// Author:
//       Lluis Sanchez Gual <lluis@novell.com>
//
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Diagnostics;
using System.IO;
using System.Threading;
using MonoDevelop.Core;
using MonoDevelop.Ide.Commands;
using NUnit.Framework;

namespace UserInterfaceTests
{
	public class SimpleTest: UITestBase
	{
		[Test]
		public void OpenEditCompile ()
		{
			var slnFile = Ide.OpenTestSolution ("ConsoleApp-VS2010/ConsoleApplication.sln");
			var slnDir = slnFile.ParentDirectory;

			var exe = slnDir.Combine ("bin", "Debug", "ConsoleApplication.exe");
			Assert.IsFalse (File.Exists (exe));

			Ide.OpenFile (slnFile.ParentDirectory.Combine ("Program.cs"));
	

			//select text editor, move down 10 lines, and insert a statement
			Session.SelectActiveWidget ();
			Session.ExecuteCommand (TextEditorCommands.DocumentStart);
			// Delete all the code already existing
			for(int i=0;i<20;i++){
				Session.ExecuteCommand (TextEditorCommands.DeleteLine);
			}

			// Entering Program With an Error

			Session.TypeText ("\nusing System;");
			Session.TypeText ("\nnamespace ConsoleProject {");
			Session.TypeText ("\nclass MainClass {");
			Session.TypeText ("\npublic static void Main (string[] args) {");
			Session.TypeText ("\nList<string> s = new List<string> () {\"one\", \"two\", \"three\"};");
			Session.TypeText ("\nConsole.WriteLine (\"Hello Xamarin!\"); }");
			Session.TypeText ("\n} }");

			Ide.BuildSolution ();


			AssertExeHasOutput (exe, "Hello Xamarin!");


				Ide.CloseAll ();
		}

		void AssertExeHasOutput (string exe, string expectedOutput)
		{
			var sw = new StringWriter ();
			var p = ProcessUtils.StartProcess (new ProcessStartInfo (exe), sw, sw, CancellationToken.None);
			Assert.AreEqual (0, p.Result);
			string output = sw.ToString ();

			Assert.AreEqual (expectedOutput, output.Trim ());
		}
		[Test]
		public void ZCorrectIt(){
			var slnFile = Ide.OpenTestSolution ("ConsoleApp-VS2010/ConsoleApplication.sln");
			var slnDir = slnFile.ParentDirectory;

			var exe = slnDir.Combine ("bin", "Debug", "ConsoleApplication.exe");
			Assert.IsFalse (File.Exists (exe));

			Ide.OpenFile (slnFile.ParentDirectory.Combine ("Program.cs"));
			Session.SelectActiveWidget ();
			for(int i=0;i<20;i++){
				Session.ExecuteCommand (TextEditorCommands.DeleteLine);
			}

			Session.ExecuteCommand (TextEditorCommands.DocumentStart);
			Session.ExecuteCommand (TextEditorCommands.InsertNewLine);

			// Entering the Correct Program
			Session.TypeText ("\nusing System;");
			Session.TypeText ("\nusing System.Collections.Generic;");
			Session.TypeText ("\nnamespace ConsoleProject {");
			Session.TypeText ("\nclass MainClass {");
			Session.TypeText ("\npublic static void Main (string[] args) {");
			Session.TypeText ("\nList<string> s = new List<string> () {\"one\", \"two\", \"three\"};");
			Session.TypeText ("\nConsole.WriteLine (\"Hello Xamarin!\"); }");
		
			Session.TypeText ("\n} }");

			Session.ExecuteCommand (TextEditorCommands.LineDown);
			Ide.BuildSolution ();

			AssertExeHasOutput (exe, "Hello Xamarin!");


			Ide.CloseAll ();


		}
		[Test]
		public void CreateBuildProject ()
		{
			string projectName = "TestFoo";
			string projectCategory = "C#";
			string projectKind = "Console Project";

			var projectDirectory = Util.CreateTmpDir (projectName);

			Ide.CreateProject (projectName, projectCategory, projectKind, projectDirectory);

			Ide.BuildSolution ();

			Ide.CloseAll ();
		}
	}
}
