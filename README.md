
# Simple Compiler in C# 🚀

## 📌 Description

This project is a mini compiler developed using C#. It implements the core phases of a compiler including lexical analysis, syntax analysis (parsing), and semantic analysis. The compiler processes input code, builds an Abstract Syntax Tree (AST), and evaluates expressions with type checking.

## 🧠 Compiler Phases

### 1. Lexical Analysis

* Removes comments from the source code
* Splits input into tokens (keywords, identifiers, numbers, operators, punctuation)
* Handles multi-character operators (==, <=, >=, etc.)

### 2. Syntax Analysis (Parsing)

* Uses recursive descent parsing
* Builds an Abstract Syntax Tree (AST)
* Supports:

  * Arithmetic expressions (+, -, *, /)
  * Relational expressions (>, <, ==)
  * Variable assignment
  * Print statements

### 3. Semantic Analysis

* Stores variables and their types (int, float)
* Performs type checking
* Detects semantic errors (e.g., assigning float to int)
* Evaluates expressions

## 🌳 Abstract Syntax Tree (AST)

The compiler builds a structured tree representation of the code, allowing proper evaluation and analysis.

## 🚀 Features

* Tokenization system
* Recursive descent parser
* AST generation and visualization
* Expression evaluation
* Semantic error detection
* Variable storage and execution

## 🛠️ Technologies Used

* C#
* .NET
* Visual Studio

## ▶️ How to Run

1. Open the project in Visual Studio
2. Build the solution
3. Run the program
4. Modify the input code inside `Main()`:

```csharp
string code = "int x = 6.8 ;";
```

## 📄 Example

### Input:

```csharp
int x = 6.8;
```

### Output:

```
Semantic Error: Cannot assign float 6.8 to int variable 'x'
```

---

### Another Example:

### Input:

```csharp
int x = 5 + 3;
```

### Output:

```
x = 8
```

## 🎯 Purpose

This project was developed as part of a university assignment to understand compiler design concepts such as tokenization, parsing, AST construction, and semantic validation.

## 📌 Future Improvements

* Add support for if / while statements
* Improve error handling
* Add a simple GUI interface
* Support more data types

## 👩‍💻 Author

Hadeel
