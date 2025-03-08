using System;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using Microsoft.JScript;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

class Program
{
    static List<string> references = new List<string>();  // Holds referenced assemblies
    static Thread runningThread = null;  // Thread that runs the compiled code

    [STAThread] // Ensure Windows Forms components work correctly.
    static void Main()
    {
        string code = @"using System;
using System.Threading;

class Program
{
    public static void Main()
    {
        const int width = 80, height = 20;
        double t = 0;

        while (true)
        {
            Console.Clear();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double wave = Math.Sin(x * 0.15 + t) + Math.Sin(y * 0.3 + t);
                    char c = wave > 1 ? '@' : wave > 0.5 ? '#' : wave > 0 ? '*' : wave > -0.5 ? '.' : ' ';
                    Console.Write(c);
                }
                Console.WriteLine();
            }
            t += 0.2;
            Thread.Sleep(50);
        }
    }
}";
        ShowEditor(code);
    }

    static void ShowEditor(string initialCode)
    {
        // Define code templates for each language.
        // Use the initial code as the C# template.
        string csharpTemplate = initialCode;
        string vbTemplate = @"Imports System
Imports System.Threading

Module Program
    Sub Main()
        Const width As Integer = 80
        Const height As Integer = 20
        Dim t As Double = 0

        While True
            Console.Clear()
            For y As Integer = 0 To height - 1
                For x As Integer = 0 To width - 1
                    Dim wave As Double = Math.Sin(x * 0.15 + t) + Math.Sin(y * 0.3 + t)
                    Dim c As Char
                    If wave > 1 Then
                        c = ""@""c
                    ElseIf wave > 0.5 Then
                        c = ""#""c
                    ElseIf wave > 0 Then
                        c = ""*""c
                    ElseIf wave > -0.5 Then
                        c = "".""c
                    Else
                        c = "" ""c
                    End If
                    Console.Write(c)
                Next
                Console.WriteLine()
            Next
            t += 0.2
            Thread.Sleep(50)
        End While
    End Sub
End Module";
        string jscriptTemplate = @"import System
import System.Threading

class Program {
    static function Main() {
        const width = 80
        const height = 20
        var t = 0

        while (true) {
            Console.Clear()
            for (var y = 0; y < height; y++) {
                for (var x = 0; x < width; x++) {
                    var wave = Math.sin(x * 0.15 + t) + Math.sin(y * 0.3 + t)
                    var c

                    if (wave > 1) {
                        c = '@'
                    } else if (wave > 0.5) {
                        c = '#'
                    } else if (wave > 0) {
                        c = '*'
                    } else if (wave > -0.5) {
                        c = '.'
                    } else {
                        c = ' '
                    }
                    Console.Write(c)
                }
                Console.WriteLine()
            }
            t += 0.2
            Thread.Sleep(50)
        }
    }
}";
        var form = new Form
        {
            Text = "CodeDom Editor",
            Size = new Size(1280, 1024),
            BackColor = Color.FromArgb(38, 38, 38)
        };

        // Font and color settings used for consistency
        var font = new Font("Segoe UI", 10);

        // Create labels and textboxes for class name and method name
        var classLabel = new Label
        {
            Text = "Class:",
            Width = 60,
            Height = 35,
            ForeColor = Color.White,
            Font = font,
            TextAlign = ContentAlignment.MiddleRight,
        };

        var classTextBox = new TextBox
        {
            Text = "Program",  // Default class name
            Width = 100,
            Height = 25,
            ForeColor = Color.White,
            BackColor = Color.FromArgb(48, 48, 48),
            Font = font,
        };

        var methodLabel = new Label
        {
            Text = "Public Method:",
            Width = 60,
            Height = 35,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 8),
            TextAlign = ContentAlignment.MiddleRight,

        };

        var methodTextBox = new TextBox
        {
            Text = "Main",  // Default method name
            Width = 100,
            Height = 25,
            ForeColor = Color.White,
            BackColor = Color.FromArgb(48, 48, 48),
            Font = font,
        };

        // Create a ComboBox for language selection
        var languageLabel = new Label
        {
            Text = "Language:",
            Width = 70,
            Height = 35,
            ForeColor = Color.White,
            Font = font,
            TextAlign = ContentAlignment.MiddleRight,
        };

        var languageComboBox = new ComboBox
        {
            Width = 100,
            Height = 25,
            ForeColor = Color.White,
            BackColor = Color.FromArgb(48, 48, 48),
            Font = font,
            DropDownStyle = ComboBoxStyle.DropDownList,
        };

        languageComboBox.Items.Add("C#");
        languageComboBox.Items.Add("VB.NET");
        languageComboBox.Items.Add("JScript.NET");
        languageComboBox.SelectedIndex = 0;  // Default to C#

        // Create the editor now so it can be referenced by the language change handler.
        var editor = new RichTextBox
        {
            Text = initialCode,
            Dock = DockStyle.Fill,
            Font = new Font("Consolas", 11),
            WordWrap = false,
            BorderStyle = BorderStyle.None,
            ForeColor = Color.White,
            BackColor = Color.FromArgb(40, 40, 40)
        };

        // When language selection changes, update the code in the editor.
        languageComboBox.SelectedIndexChanged += (s, e) =>
        {
            if (languageComboBox.SelectedItem.ToString() == "C#")
            {
                editor.Text = csharpTemplate;
            }
            else if (languageComboBox.SelectedItem.ToString() == "VB.NET")
            {
                editor.Text = vbTemplate;
            }
            else if (languageComboBox.SelectedItem.ToString() == "JScript.NET")
            {
                editor.Text = jscriptTemplate;
            }
        };

        // Header Panel for class name, method name, and language selection
        var namePanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Height = 40,
            Dock = DockStyle.Top,
            AutoSize = false,
            Width = 400,
            Padding = new Padding(5)
        };

        namePanel.Controls.AddRange(new Control[] { classLabel, classTextBox, methodLabel, methodTextBox, languageLabel, languageComboBox });

        // Add Choose File button next to the language dropdown box
        var chooseFileButton = new Button
        {
            Text = "Choose File",
            Width = 100,
            Height = 25,
            Font = font,
            ForeColor = Color.White,
            BackColor = Color.FromArgb(48, 48, 48),
            Margin = new Padding(30, 3, 5, 5)
        };

        chooseFileButton.Click += (s, e) =>
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string fileContent = File.ReadAllText(openFileDialog.FileName);
                        editor.Text = fileContent;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error reading file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        };

        namePanel.Controls.Add(chooseFileButton);

        // Editor Panel for code editor and line numbers
        var editorPanel = new Panel { Dock = DockStyle.Fill };

        var lineNumbers = new RichTextBox
        {
            Dock = DockStyle.Left,
            Width = 50,
            Font = new Font("Consolas", 11),
            ReadOnly = true,
            BackColor = Color.FromArgb(50, 50, 50),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.None,
            ScrollBars = RichTextBoxScrollBars.None
        };

        EventHandler updateLineNumbersHandler = (s, e) => UpdateLineNumbers(editor, lineNumbers);
        editor.VScroll += updateLineNumbersHandler;
        editor.TextChanged += updateLineNumbersHandler;
        UpdateLineNumbers(editor, lineNumbers);

        editorPanel.Controls.Add(editor);
        editorPanel.Controls.Add(lineNumbers);

        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, namePanel.Height));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        mainLayout.Controls.Add(namePanel, 0, 0);
        mainLayout.Controls.Add(editorPanel, 0, 1);

        var runButton = new Button
        {
            Text = "Run",
            Dock = DockStyle.Bottom,
            Font = new Font("Segoe UI", 11),
            Height = 30,
            BackColor = Color.Green,
            ForeColor = Color.White
        };
        runButton.Click += (s, e) =>
        {
            StopRunningThread();
            CompileAndRun(editor.Text, classTextBox.Text, methodTextBox.Text, languageComboBox.SelectedItem.ToString());
        };

        var stopButton = new Button
        {
            Text = "Stop",
            Dock = DockStyle.Bottom,
            Font = new Font("Segoe UI", 11),
            Height = 30,
            BackColor = Color.Red,
            ForeColor = Color.White
        };
        stopButton.Click += (s, e) => StopRunningThread();

        var referencePanel = new Panel
        {
            Dock = DockStyle.Right,
            Width = 195
        };

        var referenceLabel = new Label
        {
            Text = "Referenced Assemblies:",
            ForeColor = Color.White,
            Font = font,
            Dock = DockStyle.Top,
            TextAlign = ContentAlignment.MiddleCenter,
            Height = 30,
            BackColor = Color.FromArgb(48, 48, 48)
        };

        var referenceList = new CheckedListBox
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(48, 48, 48),
            ForeColor = Color.White,
            Font = font,
            BorderStyle = BorderStyle.FixedSingle,
            ItemHeight = 20,
            Padding = new Padding(5),
            IntegralHeight = false
        };

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var assemblyNames = new List<string>();

        foreach (var assembly in assemblies)
        {
            string assemblyName = assembly.GetName().Name + ".dll";
            if (assemblyName.StartsWith("System"))
            {
                assemblyNames.Add(assemblyName);
            }
        }

        assemblyNames.Sort((x, y) =>
        {
            int lengthComparison = x.Length.CompareTo(y.Length);
            return lengthComparison != 0 ? lengthComparison : x.CompareTo(y);
        });

        foreach (string assemblyName in assemblyNames)
        {
            referenceList.Items.Add(assemblyName, assemblyName == "System.dll");
        }

        referenceList.ItemCheck += (s, e) =>
        {
            var newReferences = new List<string>();
            for (int i = 0; i < referenceList.Items.Count; i++)
            {
                var state = (i == e.Index) ? e.NewValue : referenceList.GetItemCheckState(i);
                if (state == CheckState.Checked)
                {
                    newReferences.Add(referenceList.Items[i].ToString());
                }
            }
            references = newReferences;
        };

        referencePanel.Controls.Add(referenceList);
        referencePanel.Controls.Add(referenceLabel);

        form.Controls.AddRange(new Control[] { mainLayout, runButton, stopButton, referencePanel });
        form.ShowDialog();
    }

    static void UpdateLineNumbers(RichTextBox editor, RichTextBox lineNumbers)
    {
        int firstLine = editor.GetLineFromCharIndex(editor.GetCharIndexFromPosition(new Point(0, 0)));
        int lineCount = editor.Lines.Length;
        var sb = new StringBuilder();
        for (int i = firstLine; i < lineCount; i++)
        {
            sb.AppendLine((i + 1).ToString());
        }
        lineNumbers.Text = sb.ToString();
    }

    static void CompileAndRun(string code, string className, string methodName, string language)
    {
        CodeDomProvider codeProvider;

        if (language == "C#")
        {
            codeProvider = new CSharpCodeProvider();
        }
        else if (language == "VB.NET")
        {
            codeProvider = new VBCodeProvider();
        }
        else if (language == "JScript.NET")
        {
            codeProvider = new JScriptCodeProvider();
        }
        else
        {
            return;
        }

        var parameters = new CompilerParameters
        {
            GenerateInMemory = true,
            GenerateExecutable = false
        };

        foreach (var reference in references)
        {
            parameters.ReferencedAssemblies.Add(reference);
        }

        var results = codeProvider.CompileAssemblyFromSource(parameters, code);

        if (results.Errors.HasErrors)
        {
            var errorMsg = new StringBuilder();
            foreach (CompilerError error in results.Errors)
            {
                string fileName = error.FileName.Substring(error.FileName.LastIndexOf('\\') + 1);
                errorMsg.AppendLine(fileName + "(" + error.Line + "," + error.Column + "): " + "error " + error.ErrorNumber + ": " + error.ErrorText);
            }

            MessageBox.Show(errorMsg.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        else
        {
            var assembly = results.CompiledAssembly;

            var type = assembly.GetType(className);
            if (type == null)
            {
                MessageBox.Show("Class '" + className + "' not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var method = type.GetMethod(methodName);
            if (method == null)
            {
                MessageBox.Show("Public method '" + methodName + "' not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            runningThread = new Thread(() =>
            {
                Console.Clear();
                method.Invoke(null, null);
            });
            runningThread.Start();
        }
    }

    static void StopRunningThread()
    {
        if (runningThread != null && runningThread.IsAlive)
        {
            runningThread.Abort();
        }
    }
}
