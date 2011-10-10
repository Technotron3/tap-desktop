﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TheAirline.Model.AirportModel;
using TheAirline.Model.GeneralModel;
using TheAirline.Model.GeneralModel.StatisticsModel;
using TheAirline.GraphicsModel.PageModel.GeneralModel;
using TheAirline.GraphicsModel.PageModel.PageAirportModel;

namespace TheAirline.GraphicsModel.PageModel.PageAirportsModel.PanelAirportsModel
{
    /// <summary>
    /// Interaction logic for PageAirportStatistics.xaml
    /// </summary>
    public partial class PageAirportsStatistics : Page
    {
        private ListBox lbAirports;
        public PageAirportsStatistics()
        {
            InitializeComponent();

            StackPanel panelStatistics = new StackPanel();
            panelStatistics.Margin = new Thickness(0, 10, 50, 0);

            TextBlock txtHeader = new TextBlock();
            txtHeader.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            txtHeader.SetResourceReference(TextBlock.BackgroundProperty, "HeaderBackgroundBrush2");
            txtHeader.FontWeight = FontWeights.Bold;
            txtHeader.Text = "Largest Airports / Passengers";

            panelStatistics.Children.Add(txtHeader);

            lbAirports = new ListBox();
            lbAirports.ItemTemplate = this.Resources["AirportItem"] as DataTemplate;
            lbAirports.Height = 500;
            lbAirports.ItemContainerStyleSelector = new ListBoxItemStyleSelector();

            panelStatistics.Children.Add(lbAirports);

            GameTimer.GetInstance().OnTimeChanged += new GameTimer.TimeChanged(PageAirportsStatistics_OnTimeChanged);

            this.Content = panelStatistics;

            showAirports();

        }

        private void PageAirportsStatistics_OnTimeChanged()
        {
            if (this.IsLoaded)
               showAirports();
        }
        //shows the list of the largest airport
        private void showAirports()
        {
            StatisticsType statType = StatisticsTypes.GetStatisticsType("Passengers");
            lbAirports.Items.Clear();

            List<Airport> airports = Airports.GetAirports();
            airports.Sort(delegate(Airport a1, Airport a2) { return a2.Statistics.getTotalValue(statType).CompareTo(a1.Statistics.getTotalValue(statType)); });

            foreach (Airport airport in airports.GetRange(0, 20))
                lbAirports.Items.Add(new AirportTotalItem(airport, airport.Statistics.getTotalValue(statType)));
        }
        private void LnkAirport_Click(object sender, RoutedEventArgs e)
        {
            Airport airport = (Airport)((Hyperlink)sender).Tag;

            PageNavigator.NavigateTo(new PageAirport(airport));


        }
        //the class for the total of an airport
        private class AirportTotalItem
        {
            public Airport Airport { get; set; }
            public long Total { get; set; }
            public AirportTotalItem(Airport airport, long total)
            {
                this.Airport = airport;
                this.Total = total;
            }
        }
    }
}