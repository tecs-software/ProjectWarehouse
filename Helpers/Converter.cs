using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WarehouseManagement.Helpers
{
    internal class Converter
    {
        public static void ToMoney(TextBox textBox)
        {
            decimal num1 = 0;

            if (decimal.TryParse(textBox.Text, out decimal value))
            {
                num1 = value;
            }

            textBox.Text = num1.ToString("N2");
        }

        public static void ToNumWithComma(TextBox textBox)
        {
            decimal num1 = 0;

            if (decimal.TryParse(textBox.Text, out decimal value))
            {
                num1 = value;
            }

            textBox.Text = num1.ToString("N0");
        }

        public static void ToPercent(TextBox textBox)
        {
            double value;
            if (double.TryParse(textBox.Text, out value))
            {
                textBox.Text = value.ToString("0.##") + "%";
            }
        }

        public static void ToNumber(TextBox textBox)
        {
            string text = textBox.Text.Replace("%", "").Replace(",", "");
            textBox.Text = text;
        }

        public static void ToDecimal(TextBox textBox)
        {
            decimal value = 0;

            if (decimal.TryParse(textBox.Text, out value))
            {
                if (value == Math.Floor(value))
                {
                    textBox.Text = value.ToString("0");
                }
                else
                {
                    textBox.Text = value.ToString();
                }
            }
            else
            {
                textBox.Text = "";
            }
        }

        public static decimal StringToDecimal(string num)
        {
            num = num.Replace("%", "").Replace(",", "");

            if (decimal.TryParse(num, out decimal result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }


        public static int StringToInteger(string num)
        {
            num = num.Replace(",", "");

            if (int.TryParse(num, out int result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        public static string StringToPercentage(string num)
        {
            num = num.Replace("%", "").Replace(",", "");

            if (decimal.TryParse(num, out decimal result))
            {
                string formattedValue = result.ToString();

                if (formattedValue.EndsWith(".00"))
                {
                    formattedValue = formattedValue.Substring(0, formattedValue.Length - 3);
                }

                return formattedValue + "%";
            }
            else
            {
                return "";
            }
        }

        public static string StringToMoney(string num)
        {
            if (decimal.TryParse(num, out decimal result))
            {
                return result.ToString("#,##0.00");
            }
            else
            {
                return "0";
            }
        }


        public static string CapitalizeWords(string input, int mode)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            switch (mode)
            {
                case 1:
                    return char.ToUpper(input[0]) + input.Substring(1).ToLower();
                case 2:
                    var words = input.Split(' ');
                    for (int i = 0; i < words.Length; i++)
                    {
                        string word = words[i];
                        if (word.Length > 0)
                        {
                            if (char.IsLetter(word[0]))
                            {
                                words[i] = char.ToUpper(word[0]) + word.Substring(1).ToLower();
                            }
                            else
                            {
                                words[i] = word;
                            }
                        }
                    }
                    return string.Join(" ", words);
                default:
                    throw new ArgumentException("Invalid mode specified");
            }
        }

    }
}
