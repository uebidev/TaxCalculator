using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Globalization;

namespace TaxCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public double baseCalculoICMS = 0.0;
        double baseCalculoICMSST = 0.0;
        double valorTotalIcms;

        private void BtnMenuShowSideBar_Click(object sender, RoutedEventArgs e)
        {
            if (SideBarMenu.Height == 100)
            {
                while (SideBarMenu.Height != 900)
                {
                    SideBarMenu.Height++;
                }
            }
            else if (SideBarMenu.Height == 900)
            {
                while (SideBarMenu.Height != 100)
                {
                    SideBarMenu.Height--;
                }
            }

        }

        private void BtnShowInput_Click(object sender, RoutedEventArgs e)
        {
            //if(Inputs.Visibility!=Visibility.Visible)
            //Inputs.Visibility = Visibility.Visible;
            //else
            //    Inputs.Visibility = Visibility.Collapsed;
            if (Inputs.Height == 0)
            {
                while (Inputs.Height != 480)
                {
                    Inputs.Height++;
                }
            }
            else if (Inputs.Height == 480)
            {
                while (Inputs.Height != 0)
                {
                    Inputs.Height--;
                }
            }


        }

        private void BtnShowCalculator_Click(object sender, RoutedEventArgs e)
        {
            if (calculatorApp.Visibility != Visibility.Visible)
                calculatorApp.Visibility = Visibility.Visible;
            else
                calculatorApp.Visibility = Visibility.Collapsed;
        }

        private void BtnShowCalculator_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            // Verifica se a tecla pressionada é o ponto (".").
            if (e.Key == Key.OemPeriod || e.Key == Key.Decimal)
            {
                // Impede a inserção do ponto (".").
                e.Handled = true;
            }
            // Verifica se a tecla pressionada é uma vírgula (",").
            else if (e.Key == Key.OemComma || e.Key == Key.OemComma)
            {
                // Permite a inserção da vírgula (",") apenas se ainda não existir na caixa de texto.
                if (textBox.Text.Contains(","))
                {
                    e.Handled = true; // Já existe uma vírgula, então bloqueia a inserção de outra.
                }
                else
                {
                    e.Handled = false; // Permite a inserção da primeira vírgula.
                }
            }
            // Verifica se a tecla pressionada não é um número (0-9) e não é a tecla Backspace.
            else if (!((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                       (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                       e.Key == Key.Back))
            {
                // Impede que a tecla seja processada.
                e.Handled = true;
            }
        }

        public void CalcularICMS()
        {
            double valorProduto = double.Parse(TxtValorDoProduto.Text);
            //double? valorCustoItem = double.Parse(TxtValorCustoItem.Text);
            double valorFrete = double.Parse(TxtValorFrete.Text);
            double valorSeguro = double.Parse(TxtValorSeguro.Text);
            double despesas = double.Parse(TxtDespesas.Text);
            double valorDesconto = double.Parse(TxtValorDesconto.Text);

            // Calcula a base de cálculo do ICMS
            baseCalculoICMS = (valorProduto + valorFrete + valorSeguro + despesas - valorDesconto);

            LblBaseCalculo.Content = baseCalculoICMS;

            // Calcula o valor total do ICMS
            // O valor do ICMS é calculado multiplicando a base de cálculo pela alíquota
            valorTotalIcms = baseCalculoICMS * (double.Parse(TxtAliquotaICMSInterOrigem.Text) / 100);
            LblValorTotalICMS.Content = valorTotalIcms;
        }

        public void CalculaICMSST()
        {  // Calcula a base de cálculo do ICMS ST
           // A base de cálculo do ICMS ST é calculada multiplicando a base de cálculo do ICMS pela MVA
            baseCalculoICMSST = baseCalculoICMS * (1 + (double.Parse(TxtMVA.Text) / 100));
            LblBaseSTCalculo.Content = baseCalculoICMSST;
            // O valor do ICMS ST é calculado multiplicando a base de cálculo do ICMS ST pela alíquota do ICMS ST
            double valorSt = (baseCalculoICMSST * (double.Parse(TxtAliquotaICMSIntraDestino.Text) / 100)) - (valorTotalIcms);
            LblValorTotalICMSST.Content = valorSt;
        }

        public void CalculaPisCofins()
        {
            // O PIS e o COFINS são calculados multiplicando a base de cálculo do ICMS pela alíquota de cada imposto
            LblCofins.Content = (baseCalculoICMS * (double.Parse(TxtAliquotaCofins.Text) / 100)).ToString("F2", CultureInfo.InvariantCulture);
            LblPis.Content = (baseCalculoICMS * (double.Parse(TxtAliquotaPIS.Text) / 100)).ToString("F2", CultureInfo.InvariantCulture);
        }

        public void CalculaDIFALSimples()
        {
            // O DIFAL simples é calculado multiplicando a base de cálculo do ICMS pela diferença das alíquotas do ICMS da origem e do destino

            double aliquotaInterestadual = double.Parse(TxtAliquotaICMSInterOrigem.Text);
            double aliquotaInternaDestino = double.Parse(TxtAliquotaICMSIntraDestino.Text);
            if (aliquotaInterestadual == aliquotaInternaDestino)
            {
                LblDIFAL.Content = "0.00";
            }
            else
            {
                double diferencaAliquotas = aliquotaInternaDestino - aliquotaInterestadual;
                double valorDIFAL = baseCalculoICMS * (diferencaAliquotas / 100);
                LblDIFAL.Content = valorDIFAL.ToString("F2", CultureInfo.InvariantCulture);
            }
        }


        public void CalculaDIFALBaseDupla()
        {
            double valorMercadoria = double.Parse(TxtValorDoProduto.Text);
            double aliquotaInterna = double.Parse(TxtAliquotaICMSInterOrigem.Text); // Alíquota interna do estado de destino (18%)
            double aliquotaInterestadual = double.Parse(TxtAliquotaICMSIntraDestino.Text); // Alíquota interestadual (12%)
            double icmsIntrastadual;
            double icmsInterestadual;
            double difal;


            // Cálculo do ICMS intrastadual
            icmsIntrastadual = valorMercadoria * aliquotaInterna;

            // Cálculo do ICMS interestadual
            icmsInterestadual = valorMercadoria * aliquotaInterestadual;

            // Cálculo do DIFAL
            difal = icmsIntrastadual - icmsInterestadual;

            double baseCalculo = valorMercadoria - icmsInterestadual;
            double baseCalculo2 = baseCalculo / (1 - aliquotaInterna);
            double icmsInterno = baseCalculo2 * aliquotaInterna;

            double difalDuplo = icmsInterno - icmsInterestadual;

            LblDIFALBaseDupla.Content = difalDuplo;
        }




        private void BtnCalcular_Click(object sender, RoutedEventArgs e)
        {
            foreach (var child in WrapProduto.Children)
            {
                if (child is TextBox textBox)
                {
                    if (string.IsNullOrEmpty(textBox.Text))
                    {
                        textBox.Text = "0";
                    }
                }
            }
            foreach (var child in WrapAliquota.Children)
            {
                if (child is TextBox textBox)
                {
                    if (string.IsNullOrEmpty(textBox.Text))
                    {
                        textBox.Text = "0";
                    }
                }
            }


            CalcularICMS();
            CalculaICMSST();
            CalculaDIFALSimples();
            CalculaDIFALBaseDupla();
            CalculaPisCofins();
        }
    }
}
