# 📝 CodeDom Dynamic Code Editor

This is a Windows Forms-based **code editor** that allows you to write, edit, and dynamically compile and execute C#, VB.NET, and JScript.NET code using the **CodeDom Compiler**. It is designed for experimentation and rapid prototyping.

---

## 🚀 Features

- **Multi-Language Support**: Compile and run code in C#, VB.NET, and JScript.NET.
- **Dynamic Compilation**: Uses CodeDom to compile code on-the-fly without creating external executables.
- **Interactive Code Editor**: A simple text editor with line numbering.
- **File Integration**: Open existing code files for editing and execution.
- **Assembly Reference Management**: Easily add or remove system assembly references used during compilation.
- **Threaded Execution**: Runs compiled code on a separate thread.

---

## 🛠️ How It Works

1. **Language Selection**  
   - Choose a language (C#, VB.NET, or JScript.NET) from the dropdown menu.

2. **Code Editing**  
   - The editor loads a default template for the selected language.  
   - Modify the code as needed, including updating the class name and entry method.

3. **Compilation & Execution**  
   - Click the **Run** button to compile the code dynamically using CodeDom.  
   - The compiled code is executed on a separate thread, with output displayed in the console.

4. **Stopping Execution**  
   - If the running code needs to be halted, click the **Stop** button to abort execution.

5. **Managing Assembly References**  
   - Use the interface to select which system assemblies to reference during compilation

---

## 📜 License

This project is released under the [MIT License](LICENSE).

---

## 🔧 Future Improvements

- Enhanced text editor features such as syntax highlighting.

---

Happy coding! ✨
