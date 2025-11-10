using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Calculator
{
    public partial class Form1 : Form
    {
        private static readonly char[] Ops = new[] { '+', '-', '*', '/' };

        public Form1()
        {
            InitializeComponent();
            // Wire everything here so it works even if Designer events are missing
            WireEvents();
            InitUi();
        }

        // In case your project already uses Form1_Load, keep it harmless
        private void Form1_Load(object sender, EventArgs e)
        {
            // No-op; we wire in constructor
        }

        private void WireEvents()
        {
            // Digits
            btn0.Click += NumberButton_Click;
            btn1.Click += NumberButton_Click;
            btn2.Click += NumberButton_Click;
            btn3.Click += NumberButton_Click;
            btn4.Click += NumberButton_Click;
            btn5.Click += NumberButton_Click;
            btn6.Click += NumberButton_Click;
            btn7.Click += NumberButton_Click;
            btn8.Click += NumberButton_Click;
            btn9.Click += NumberButton_Click;

            // Operators
            btnAdd.Click += btnAdd_Click;
            btnSub.Click += btnSub_Click;
            btnMul.Click += btnMul_Click;
            btnDiv.Click += btnDiv_Click;

            // Equals & Clear
            btnEqual.Click += btnEqual_Click;
            btnClear.Click += btnClear_Click;
        }

        private void InitUi()
        {
            if (string.IsNullOrWhiteSpace(txtNumber.Text))
                txtNumber.Text = "0";
            lblResult.Text = "Result:";
            MoveCaretToEnd();
        }

        // ===== DIGITS =====
        private void NumberButton_Click(object sender, EventArgs e)
        {
            var digit = ((Button)sender).Text; // "0".."9"

            // If starting from "0", replace it
            if (txtNumber.Text == "0")
                txtNumber.Text = digit;
            else
                txtNumber.Text += digit;

            MoveCaretToEnd();
        }

        // ===== OPERATORS =====
        private void btnAdd_Click(object sender, EventArgs e) => AppendOperator('+');
        private void btnSub_Click(object sender, EventArgs e) => AppendOperator('-');
        private void btnMul_Click(object sender, EventArgs e) => AppendOperator('*');
        private void btnDiv_Click(object sender, EventArgs e) => AppendOperator('/');

        private void AppendOperator(char op)
        {
            string expr = txtNumber.Text.Trim();

            // Allow negative first number (leading '-')
            if (expr.Length == 0 || expr == "0")
            {
                if (op == '-')
                    txtNumber.Text = "-";
                else
                    MessageBox.Show("Enter the first number before choosing an operator.");
                MoveCaretToEnd();
                return;
            }

            // Replace trailing operator (e.g., '12+' -> change to '12-')
            if (Ops.Contains(expr.Last()))
            {
                txtNumber.Text = expr.Substring(0, expr.Length - 1) + op;
                MoveCaretToEnd();
                return;
            }

            // Only one operation supported (A op B)
            int existing = FindOperatorIndex(expr);
            if (existing != -1)
            {
                MessageBox.Show("Only one operation is allowed (format: firstNumber operator secondNumber).");
                return;
            }

            txtNumber.Text += op;
            MoveCaretToEnd();
        }

        // ===== EQUALS =====
        private void btnEqual_Click(object sender, EventArgs e)
        {
            string expr = txtNumber.Text.Trim();

            int opIdx = FindOperatorIndex(expr);
            if (opIdx == -1)
            {
                MessageBox.Show("Please enter an expression like 12+3, 8-4, 5*6, or 10/2.");
                return;
            }

            string left = expr.Substring(0, opIdx).Trim();
            string right = expr.Substring(opIdx + 1).Trim();
            char op = expr[opIdx];

            if (!TryParseDouble(left, out double a))
            {
                MessageBox.Show("First number is not valid.");
                return;
            }
            if (!TryParseDouble(right, out double b))
            {
                MessageBox.Show("Second number is not valid.");
                return;
            }

            try
            {
                double result = Compute(a, b, op);
                lblResult.Text = "Result: " + result.ToString(CultureInfo.CurrentCulture);
            }
            catch (DivideByZeroException)
            {
                MessageBox.Show("Cannot divide by zero.");
            }
        }

        // ===== CLEAR =====
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtNumber.Text = "0";
            lblResult.Text = "Result:";
            MoveCaretToEnd();
        }

        // ===== Helpers =====
        private void MoveCaretToEnd()
        {
            txtNumber.SelectionStart = txtNumber.TextLength;
            txtNumber.Focus();
        }

        // Find first operator, ignoring a leading '-' (negative first number)
        private int FindOperatorIndex(string expr)
        {
            if (string.IsNullOrWhiteSpace(expr)) return -1;

            for (int i = 0; i < expr.Length; i++)
            {
                char c = expr[i];
                if (Ops.Contains(c))
                {
                    if (i == 0 && c == '-') continue; // leading '-' is sign
                    return i;
                }
            }
            return -1;
        }

        private bool TryParseDouble(string s, out double value)
        {
            return double.TryParse(
                s,
                NumberStyles.Float,
                CultureInfo.CurrentCulture,
                out value
            );
        }

        private double Compute(double a, double b, char op)
        {
            switch (op)
            {
                case '+': return a + b;
                case '-': return a - b;
                case '*': return a * b;
                case '/':
                    if (Math.Abs(b) < double.Epsilon) throw new DivideByZeroException();
                    return a / b;
                default: throw new InvalidOperationException("Unknown operator");
            }
        }

        // (Optional) If Designer referenced these old stubs, keep them to avoid errors
        private void button1_Click(object sender, EventArgs e) { }
        private void button3_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
    }
}
