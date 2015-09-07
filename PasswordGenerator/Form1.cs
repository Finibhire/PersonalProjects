using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace PasswordGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            char[] invalid = new char[] {'=', '`', '~', '$', '&', '(', ')', '[', ']', '{', '}', '\\', '|', ' ', ';', '\'', '"', ',',
            '<', '>', '/' };
            char[] specialCharacters = new char[] {'+', '_', '-', '!', '@', '#', '%', '^', '*', ':', '.', '?' };
            char[] normalCharacters = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                                                   'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                                                   '1', '2', '3', '4', '5', '6', '7', '8', '9', '0',
                                                   '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };

            char[] pickChars = new char[specialCharacters.Length * 2 + normalCharacters.Length];
            
            Array.Copy(normalCharacters, pickChars, normalCharacters.Length);
            Array.Copy(specialCharacters, 0, pickChars, normalCharacters.Length, specialCharacters.Length);
            Array.Copy(specialCharacters, 0, pickChars, normalCharacters.Length + specialCharacters.Length, specialCharacters.Length);

            int pickCharLength = pickChars.Length;
            Random rng = new Random();
            StringBuilder sb = new StringBuilder((int)numericUpDown1.Value);

            for (int i = 0; i < (int)numericUpDown1.Value; i++)
            {
                sb.Append(pickChars[rng.Next(pickCharLength)]);
            }

            textBox1.Text = sb.ToString();
        }
    }
}
