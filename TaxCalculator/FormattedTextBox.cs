using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace TaxCalculator
{
    public class FormattedTextBox : TextBox
    {
        public FormattedTextBox()
        {
            PreviewTextInput += TextBox_PreviewTextInput;
            LostFocus += TextBox_LostFocus;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsNumericInput(e.Text))
            {
                e.Handled = true;
                return;
            }

            string currentText = Text;
            int caretIndex = CaretIndex;
            string newText = currentText.Insert(caretIndex, e.Text);

            if (decimal.TryParse(newText, NumberStyles.Currency, new CultureInfo("pt-BR"), out decimal formattedValue))
            {
                Text = formattedValue.ToString("N2", new CultureInfo("pt-BR"));
                CaretIndex = Text.Length;
            }

            e.Handled = true;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(Text, NumberStyles.Currency, new CultureInfo("pt-BR"), out decimal unformattedValue))
            {
                Text = unformattedValue.ToString("N2", new CultureInfo("pt-BR"));
            }
        }

        private bool IsNumericInput(string input)
        {
            return decimal.TryParse(input, out _);
        }
    }
}
