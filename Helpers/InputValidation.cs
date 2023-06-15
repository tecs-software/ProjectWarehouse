using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace WarehouseManagement.Helpers
{
    internal class InputValidation
    {
        public static void Decimal(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string currentText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            if (currentText == ".")
            {
                e.Handled = true;
                textBox.Text = "0.";
                textBox.SelectionStart = textBox.Text.Length;
                return;
            }
            else if (currentText.StartsWith("."))
            {
                e.Handled = true;
                currentText = "0" + currentText;
            }

            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text) || currentText.Split('.').Length > 2;
        }

        public static void Integer(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        public static void Percentage(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!char.IsDigit(e.Text[0])) // check if entered text is a digit
            {
                e.Handled = true; // prevent the input from being added to the TextBox
                return;
            }

            int number;
            if (!int.TryParse(textBox.Text + e.Text, out number) || number > 100)
            {
                e.Handled = true; // prevent the input from being added to the TextBox
            }
        }

    }
}
