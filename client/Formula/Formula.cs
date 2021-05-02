using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities {
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula {

        /// <summary>
        /// Contains all tokens in this formula
        /// </summary>
        private LinkedList<Token> tokens;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true) {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid) {
            // Eliminate = at front
            if (formula.Length > 0 & formula[0] == '=') {
                formula = formula.Substring(1);
            }

            // Init tokens
            tokens = new LinkedList<Token>();
            // Track last variable for operator following rule and extra following rule
            bool lastWasOpeningOrOperator = false;
            bool lastWasClosingNumberOrVar = false;
            // Track parenthesis counts
            int closedParens = 0;
            int openParens = 0;
            foreach (string token in GetTokens(formula)) {
                Token newToken = new Token(token, normalize, isValid);
                // Validate based on previous token
                if (newToken.Type == "op") {
                    if (newToken.Content == "(") {
                        openParens++;
                        lastWasOpeningOrOperator = true;
                        if (lastWasClosingNumberOrVar) {
                            throw new FormulaFormatException("( cannot follow a number, variable, or closing parenthesis");
                        }
                    } else if (newToken.Content == ")") {
                        closedParens++;
                        lastWasClosingNumberOrVar = true;
                        if (lastWasOpeningOrOperator) {
                            throw new FormulaFormatException(") cannot follow ( or an operator");
                        }
                    } else {
                        if (lastWasOpeningOrOperator) {
                            throw new FormulaFormatException(newToken.Content + " cannot follow ( or an operator");
                        }
                        lastWasOpeningOrOperator = true;
                        lastWasClosingNumberOrVar = false;
                    }
                } else if (newToken.Type == "var") {
                    if (lastWasClosingNumberOrVar) {
                        throw new FormulaFormatException("Error on: " + newToken.Content + " Variables cannot follow ), numbers, or other variables");
                    }
                    lastWasOpeningOrOperator = false;
                    lastWasClosingNumberOrVar = true;
                } else if (newToken.Type == "val") {
                    if (lastWasClosingNumberOrVar) {
                        throw new FormulaFormatException("Error on: " + newToken.Content + " Numbers cannot follow ), numbers, or other variables");
                    }
                    lastWasClosingNumberOrVar = true;
                    lastWasOpeningOrOperator = false;
                }

                if (closedParens > openParens) {
                    throw new FormulaFormatException("The number of ) should never exceed the number of (");
                }

                tokens.AddLast(newToken);
            }

            // Make sure parethensis counts are equal
            if (openParens != closedParens) {
                throw new FormulaFormatException("Closing parenthesis and open parenthesis should be equal");
            }

            // Make sure we have at least one token
            if (tokens.Count == 0) {
                throw new FormulaFormatException("Formula must contain at least one token");
            }
            // Make sure start & end tokens are correct
            if (tokens.First.Value.Type == "op" && tokens.First.Value.Content != "(") {
                throw new FormulaFormatException("First token must be a number, variable, or (");
            } else if (tokens.Last.Value.Type == "op" && tokens.Last.Value.Content != ")") {
                throw new FormulaFormatException("Last token must be a number, variable, or )");
            }
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup) {
            // Init stacks
            Stack<double> values = new Stack<double>();
            Stack<string> ops = new Stack<string>();
            foreach (Token s in tokens) {
                double value;
                // Evaluate if token is a number or variable
                if (s.Type != "op") {
                    // Assign value if token is a variable
                    if (s.Type == "var") {
                        try {
                            value = lookup(s.Content);
                        } catch (Exception e) {
                            return new FormulaError("Error: " + s.Content + " is not a double");
                        }
                    } else {
                        value = Double.Parse(s.Content);
                    }
                    // Evaluate / or *
                    if (ops.Count != 0 && (string.Equals(ops.Peek(), "/") || string.Equals(ops.Peek(), "*"))) {
                        if (values.Count == 0) {
                            return new FormulaError("Error: Operators must be preceded by numbers or variables");
                        } else {
                            try {
                                values.Push(runOperation(value, values.Pop(), ops.Pop()));
                            } catch (DivideByZeroException) {
                                return new FormulaError("Error: Divide by zero");
                            }
                        }
                    } else {
                        values.Push(value);
                    }
                    // Evaluate if token is + or -
                } else if (s.Content.Equals("+") || s.Content.Equals("-")) {
                    if (values.Count > 1 && ops.Count != 0 && (string.Equals(ops.Peek(), "+") || string.Equals(ops.Peek(), "-"))) {
                        values.Push(runOperation(values.Pop(), values.Pop(), ops.Pop()));
                    }
                    ops.Push(s.Content);
                    // Push *, /, or ( to ops stack
                } else if (s.Content.Equals("*") || s.Content.Equals("/") || s.Content.Equals("(")) {
                    ops.Push(s.Content);
                    // Evaluate closing parenthesis
                } else if (s.Content.Equals(")")) {
                    // See if parenthesis is matched
                    if (ops.Count != 0 && string.Equals(ops.Peek(), "(")) {
                        ops.Pop();
                    }
                    // Make sure we have enough values
                    if (values.Count <= 1) {
                        if (ops.Count != 0 && string.Equals(ops.Peek(), "(")) {
                            ops.Pop();
                            continue;
                        }
                    }
                    // Run + or -
                    if (ops.Count != 0 && (string.Equals(ops.Peek(), "+") || string.Equals(ops.Peek(), "-"))) {
                        values.Push(runOperation(values.Pop(), values.Pop(), ops.Pop()));
                        if (ops.Count != 0 && string.Equals(ops.Peek(), "(")) {
                            ops.Pop();
                        } 
                    }
                    // Run * or /
                    if (ops.Count != 0 && (string.Equals(ops.Peek(), "*") || string.Equals(ops.Peek(), "/"))) {
                        values.Push(runOperation(values.Pop(), values.Pop(), ops.Pop()));
                    }
                }
            }

            double result = 0;

            // Final check & return
            if (ops.Count == 0) {
                if (values.Count == 1) {
                    result = values.Pop();
                }
            } else if (ops.Count == 1) {
                if (values.Count == 2) {
                    result = runOperation(values.Pop(), values.Pop(), ops.Pop());
                }
            }

            // Check for divide by zero
            if (result == double.PositiveInfinity) {
                return new FormulaError("Divide by zero occured");
            } else {
                return result;
            }
        }

        /// <summary>
        /// Runs an algebraic operation on two integers
        /// </summary>
        /// <param name="op">Operator to use. Accepts +-*/</param>
        /// <param name="a">Int before the operator</param>
        /// <param name="a">Int after the operator</param>
        /// <exception cref="System.ArgumentException">Thrown when op isn't one of the four
        /// valid operators </exception>
        private static double runOperation(double a, double b, string op) {
            switch (op) {
                case "+": return a + b;
                case "-": return b - a;
                case "*": return a * b;
                default: return b / a;
            }
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables() {
            HashSet<String> result = new HashSet<string>();

            // Loop through tokens, add vars to result
            foreach (Token t in tokens) {
                if (t.Type == "var") {
                    result.Add(t.Content);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString() {
            StringBuilder result = new StringBuilder();

            result.Append("=");

            // Loop through tokens, add each to result
            foreach (Token t in tokens) {
                result.Append(t.Content);
            }

            return result.ToString();
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj) {
            if (typeof(Formula).IsInstanceOfType(obj)) {
                Formula other = (Formula) obj;
                // Make sure token counts are equal
                if (tokens.Count != other.tokens.Count) {
                    return false;
                }
                // Make sure tokens are equal
                IEnumerator<Token> enumOther = other.tokens.GetEnumerator();
                enumOther.MoveNext();
                foreach (Token t in tokens) {
                    if (!t.Equals(enumOther.Current)) {
                        return false;
                    }
                    enumOther.MoveNext();
                }

                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2) {
            try {
                return f1.Equals(f2);
            } catch (NullReferenceException) {
                return false;
            }
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2) {
            try {
                return !f1.Equals(f2);
            } catch (NullReferenceException) {
                return false;
            }
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode() {
            int result = 1;

            // Multiply all token hash codes together
            foreach (Token t in tokens) {
                result *= t.GetHashCode();
            }

            return result;
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula) {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace)) {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline)) {
                    yield return s;
                }
            }

        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message) {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this() {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }

        public override string ToString() {
            return Reason;
        }
    }

    /// <summary>
    /// Represents a single token in the list of tokens used to represent a formula
    /// </summary>
    internal class Token {

        /// <summary>
        /// Creates a new Token.
        /// </summary>
        /// <param name="token">
        /// String content of the token. Should be a variable starting with a letter and
        /// ending with a number, or an operator in +-*/, or a double, or a parenthesis
        /// </param>
        /// <param name="normalize">Function to normalize variable names</param>
        /// <param name="isValid">Function to determine if a variable is valid</param>
        /// <exception cref="FormulaFormatException">Thrown if token isn't valid</exception>
        public Token(String token, Func<string, string> normalize, Func<string, bool> isValid) {
            Double output;
            // Check if token is value, normalize if so
            if (Double.TryParse(token, out output)) {
                Content = output.ToString();
                Type = "val";
            // Check if token is operator
            } else if (token == "*" || token == "/" || token == "-" || token == "+" || token == "(" || token == ")") {
                Content = token;
                Type = "op";
            // Check if token is variable, normalize if so
            } else if (isValid(token)) {
                try {
                    Content = normalize(token);
                } catch (Exception e) {
                    throw new FormulaFormatException(e.Message);
                }
                Type = "var";
            } else {
                throw new FormulaFormatException("Token " + token + " is not a valid double, operator, or variable");
            }
        }

        /// <summary>
        /// The type of this token. Either "val", "op", or "var", 
        /// </summary>
        public String Type { get; private set; }

        public String Content { get; private set; }

        public override bool Equals(object obj) {
            if (typeof(Token).IsInstanceOfType(obj)) {
                Token other = (Token) obj;
                return this.Content == other.Content && this.Type == other.Type;
            } else {
                return false;
            }
        }

        public override int GetHashCode() {
            return Type.GetHashCode() * Content.GetHashCode();
        }
    }
}

