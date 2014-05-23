﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TheAirline.GraphicsModel.UserControlModel.MessageBoxModel;
using TheAirline.GraphicsModel.UserControlModel.PopUpWindowsModel;
using TheAirline.Model.GeneralModel;
using TheAirline.Model.GeneralModel.Helpers;

namespace TheAirline.GUIModel.PagesModel.AirlinePageModel
{
    /// <summary>
    /// Interaction logic for PageAirlineFinances.xaml
    /// </summary>
    public partial class PageAirlineFinances : Page
    {
        public AirlineMVVM Airline { get; set; }
        public PageAirlineFinances(AirlineMVVM airline)
        {
            this.Airline = airline;
            this.DataContext = this.Airline;

            InitializeComponent();
        }

        private void btnApplyLoan_Click(object sender, RoutedEventArgs e)
        {
            double amount = slAmount.Value;
            int length = Convert.ToInt16(slLenght.Value)*12;

            Loan loan = new Loan(GameObject.GetInstance().GameTime, amount, length, this.Airline.LoanRate);

            if (AirlineHelpers.CanApplyForLoan(GameObject.GetInstance().HumanAirline, loan))
            {

                this.Airline.addLoan(loan);

                AirlineHelpers.AddAirlineInvoice(this.Airline.Airline, GameObject.GetInstance().GameTime, Invoice.InvoiceType.Loans, loan.Amount);
            }
            else
                WPFMessageBox.Show(Translator.GetInstance().GetString("MessageBox", "2124"), Translator.GetInstance().GetString("MessageBox", "2124", "message"), WPFMessageBoxButtons.Ok);
   
            


        }

        private void btnPayLoan_Click(object sender, RoutedEventArgs e)
        {
            TextBox txtPay = (TextBox)((Button)sender).Tag;
            LoanMVVM loan = (LoanMVVM)txtPay.Tag;

            double amount = Convert.ToDouble(txtPay.Text);

            if (amount <= 0 || amount > this.Airline.Money)
            {
                WPFMessageBox.Show(Translator.GetInstance().GetString("MessageBox", "2104"), Translator.GetInstance().GetString("MessageBox", "2104", "message"), WPFMessageBoxButtons.Ok);
            }
            else
            {
                WPFMessageBoxResult result = WPFMessageBox.Show(Translator.GetInstance().GetString("MessageBox", "2105"), string.Format(Translator.GetInstance().GetString("MessageBox", "2105", "message"), amount), WPFMessageBoxButtons.YesNo);

                if (result == WPFMessageBoxResult.Yes)
                {
                    double payingAmount = Math.Min(amount, loan.PaymentLeft);

                    loan.payOnLoan(payingAmount);

                    AirlineHelpers.AddAirlineInvoice(this.Airline.Airline, GameObject.GetInstance().GameTime, Invoice.InvoiceType.Loans, -payingAmount);

                    if (loan.PaymentLeft <= 0)
                        this.Airline.Loans.Remove(loan);
                }
            }
        }
    }
}
