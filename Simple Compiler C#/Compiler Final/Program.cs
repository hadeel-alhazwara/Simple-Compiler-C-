using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class Token
{
    public string Type;
    public string Value;
    public Token(string type, string value) { Type = type; Value = value; }
}

class Node
{
    public string Type;
    public string Operator;
    public string Var;
    public Node Left;
    public Node Right;
    public Node Condition;
    public Node ExprValue;
    public List<Node> Body;
    public Node() { Body = new List<Node>(); }
}

class SimpleCompiler
{
    static List<string> keywords = new List<string> { "if", "else", "while", "return", "int", "float", "print" };
    static List<string> operators = new List<string> { "+", "-", "*", "/", "=", "<", ">", "==", "!=", "<=", ">=" };
    static List<char> punctuation = new List<char> { ',', ';', ':', '!', '?', '(', ')', '[', ']', '{', '}', '"', '\'' };

    static Dictionary<string, double> variables = new Dictionary<string, double>();
    static Dictionary<string, string> variableTypes = new Dictionary<string, string>();

   
    static bool IsIdentifier(string word)
    {
        if (string.IsNullOrEmpty(word)) return false;
        if (char.IsLetter(word[0]) || word[0] == '_') return true;
        return false;
    }

    static bool IsNumber(string word)
    {
        double n;
        return double.TryParse(word, out n);
    }

    public static List<Token> Lexical(string input_code)
    {
        Console.WriteLine("Code With Comment:\n" + input_code);
        string code_no_comments = Regex.Replace(input_code, @"#.*", " ");
        code_no_comments = Regex.Replace(code_no_comments, @"/\*.*?\*/", " ", RegexOptions.Singleline);

        List<Token> tokens = new List<Token>();
        string current_word = "";

        for (int i = 0; i < code_no_comments.Length; i++)
        {
            char c = code_no_comments[i];
            if (char.IsWhiteSpace(c))
            {
                if (current_word != "") { tokens.Add(CreateToken(current_word)); current_word = ""; }
                continue;
            }
        
            if (operators.Contains(c.ToString()))
            {
                if (current_word != "") { tokens.Add(CreateToken(current_word)); current_word = ""; }
                if (i + 1 < code_no_comments.Length && operators.Contains(c.ToString() + code_no_comments[i + 1].ToString()))
                {
                    tokens.Add(new Token("OPERATOR", c.ToString() + code_no_comments[i + 1].ToString()));
                    i++;
                }
                else tokens.Add(new Token("OPERATOR", c.ToString()));
                continue;
            }
            if (punctuation.Contains(c))
            {
                if (current_word != "") { tokens.Add(CreateToken(current_word)); current_word = ""; }
                tokens.Add(new Token("PUNCTUATION", c.ToString()));
                continue;
            }
            current_word += c;
        }
        if (current_word != "") tokens.Add(CreateToken(current_word));

        Console.WriteLine("\nTokens:");
        foreach (var t in tokens) Console.WriteLine($"{t.Type} : {t.Value}");
        return tokens;
    }

    static Token CreateToken(string word)
    {
        if (keywords.Contains(word)) return new Token("KEYWORD", word);
        if (IsNumber(word)) return new Token("NUMBER", word);
        if (IsIdentifier(word)) return new Token("IDENTIFIER", word);
        return new Token("UNKNOWN", word);
    }
    
    static Node ParseFactor(List<Token> tokens)
    {
        if (tokens.Count == 0) throw new Exception("Unexpected end of tokens");
        Token token = tokens[0]; tokens.RemoveAt(0);

        if (token.Type == "NUMBER") return new Node { Type = "NUMBER", Var = token.Value };
        if (token.Type == "IDENTIFIER") return new Node { Type = "IDENTIFIER", Var = token.Value };
        if (token.Value == "(")
        {
            Node expr = ParseExpression(tokens);
            if (tokens.Count > 0 && tokens[0].Value == ")") tokens.RemoveAt(0);
            return expr;
        }
        throw new Exception("Unexpected token " + token.Value);
    }

    static Node ParseTerm(List<Token> tokens)
    {
        Node node = ParseFactor(tokens);
        while (tokens.Count > 0 && (tokens[0].Value == "*" || tokens[0].Value == "/"))
        {
            Token op = tokens[0]; tokens.RemoveAt(0);
            node = new Node { Type = "BINARY_OP", Operator = op.Value, Left = node, Right = ParseFactor(tokens) };
        }
        return node;
    }

    static Node ParseArithmetic(List<Token> tokens)
    {
        Node node = ParseTerm(tokens);
        while (tokens.Count > 0 && (tokens[0].Value == "+" || tokens[0].Value == "-"))
        {
            Token op = tokens[0]; tokens.RemoveAt(0);
            node = new Node { Type = "BINARY_OP", Operator = op.Value, Left = node, Right = ParseTerm(tokens) };
        }
        return node;
    }

    static Node ParseExpression(List<Token> tokens)
    {
        Node node = ParseArithmetic(tokens);
        while (tokens.Count > 0 && (tokens[0].Value == ">" || tokens[0].Value == "<" || tokens[0].Value == "=="))
        {
            Token op = tokens[0]; tokens.RemoveAt(0);
            node = new Node { Type = "BINARY_OP", Operator = op.Value, Left = node, Right = ParseArithmetic(tokens) };
        }
        return node;
    }

    static Node ParseAssignment(List<Token> tokens)
    {
        string varName = tokens[0].Value;
        tokens.RemoveAt(0); 
        if (tokens.Count > 0 && tokens[0].Value == "=")
        {
            tokens.RemoveAt(0); 
            return new Node { Type = "ASSIGNMENT", Var = varName, ExprValue = ParseExpression(tokens) };
        }
        return new Node { Type = "IDENTIFIER", Var = varName };
    }

    static Node ParseStatement(List<Token> tokens)
    {
        if (tokens.Count == 0) return null;
        if (tokens[0].Value == "}") { tokens.RemoveAt(0); return null; }

        Node node = null;
        if (tokens[0].Type == "KEYWORD")
        {
            string word = tokens[0].Value;
            if (word == "int" || word == "float")
            {
                string type = tokens[0].Value;
                tokens.RemoveAt(0);
                string varName = tokens[0].Value;
                variableTypes[varName] = type; 
                node = ParseAssignment(tokens);
            }
            else if (word == "print")
            {
                tokens.RemoveAt(0);
                node = new Node { Type = "PRINT", ExprValue = ParseExpression(tokens) };
            }
        }
        else
        {
            node = ParseAssignment(tokens);
        }
        
        if (tokens.Count > 0 && tokens[0].Value == ";") tokens.RemoveAt(0);

        return node;
    }

    static Node ParseProgram(List<Token> tokens)
    {
        Node program = new Node { Type = "PROGRAM" };
        while (tokens.Count > 0)
        {
            Node stmt = ParseStatement(tokens);
            if (stmt != null) program.Body.Add(stmt);
        }
        return program;
    }

    static void PrintTree(Node node, string indent = "")
    {
        if (node == null) return;
        Console.WriteLine(indent + node.Type + (node.Var != null ? " (" + node.Var + ")" : "") + (node.Operator != null ? " [" + node.Operator + "]" : ""));
        if (node.ExprValue != null) PrintTree(node.ExprValue, indent + "  ");
        if (node.Left != null) PrintTree(node.Left, indent + "  ");
        if (node.Right != null) PrintTree(node.Right, indent + "  ");
        if (node.Body != null) foreach (var child in node.Body) PrintTree(child, indent + "  ");
    }

    static double? Evaluate(Node node)
    {
        if (node == null) return null;
        switch (node.Type)
        {
            case "PROGRAM": foreach (var s in node.Body) Evaluate(s); return null;
            case "NUMBER": return double.Parse(node.Var);
            case "IDENTIFIER": return variables.ContainsKey(node.Var) ? variables[node.Var] : 0;
            case "ASSIGNMENT":
                double? val = Evaluate(node.ExprValue);
                if (val != null)
                {
                    if (variableTypes.ContainsKey(node.Var) && variableTypes[node.Var] == "int" && val.Value % 1 != 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"\nSemantic Error: Cannot assign float {val} to int variable '{node.Var}'");
                        Console.ResetColor();
                        return null;
                    }
                    variables[node.Var] = val.Value;
                    Console.WriteLine($"{node.Var} = {val}");
                }
                return val;
        }
        return null;
    }

    static void Main()
    {
        string code = "int x = 6.8 ;";

        List<Token> tokens = Lexical(code);
        Node tree = ParseProgram(tokens);

        Console.WriteLine("\nAST Tree:");
        PrintTree(tree);

        Console.WriteLine("\nSemantic Analysis:");
        Evaluate(tree);

        Console.WriteLine("\nFinal Variables:");
        foreach (var kv in variables) Console.WriteLine($"{kv.Key} = {kv.Value}");

        Console.ReadKey();
    }
}