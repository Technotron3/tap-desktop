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
using TheAirline.Model.AirlineModel;
using TheAirline.Model.AirlinerModel.RouteModel;
using TheAirline.GraphicsModel.Converters;
using TheAirline.GraphicsModel.PageModel.GeneralModel;
using TheAirline.GraphicsModel.PageModel.PageAirportModel.PanelAirportModel;
using TheAirline.GraphicsModel.UserControlModel.PopUpWindowsModel;

namespace TheAirline.GraphicsModel.PageModel.PageAirportModel
{
    /// <summary>
    /// Interaction logic for PageAirport.xaml
    /// </summary>
    public partial class PageAirport : StandardPage
    {
        public Airport Airport { get; set; }
        private TextBlock txtWind, txtLocalTime;
        private ListBox lbArrivals, lbDepartures;
        public PageAirport(Airport airport)
        {
            this.Airport = airport;

            InitializeComponent();

            StackPanel airportPanel = new StackPanel();
            airportPanel.Margin = new Thickness(10, 0, 10, 0);

            airportPanel.Children.Add(createQuickInfoPanel());
            airportPanel.Children.Add(createWeatherPanel());
            airportPanel.Children.Add(createArrivalsPanel());
            airportPanel.Children.Add(createDeparturesPanel());

            showFlights();

            StandardContentPanel panelContent = new StandardContentPanel();

            panelContent.setContentPage(airportPanel, StandardContentPanel.ContentLocation.Left);


            StackPanel panelSideMenu = new PanelAirport(this.Airport);

            panelContent.setContentPage(panelSideMenu, StandardContentPanel.ContentLocation.Right);



            base.setContent(panelContent);

            base.setHeaderContent(this.Airport.Profile.Name);


            showPage(this);

            GameTimer.GetInstance().OnTimeChanged += new GameTimer.TimeChanged(PageAirport_OnTimeChanged);
        }

        private void PageAirport_OnTimeChanged()
        {
            if (this.IsLoaded)
            {
                //txtWind.Text = string.Format("{0} ({1} km/h) in {2} direction", new Converters.TextUnderscoreConverter().Convert(this.Airport.Weather.WindSpeed, null, null, null), (int)this.Airport.Weather.WindSpeed, this.Airport.Weather.Direction);
                txtWind.Text = string.Format("{0} ({1:0.##} {2} in {3} direction)", new Converters.TextUnderscoreConverter().Convert(this.Airport.Weather.WindSpeed, null, null, null),new NumberToUnitConverter().Convert((int)this.Airport.Weather.WindSpeed), new StringToLanguageConverter().Convert("km/t"),this.Airport.Weather.Direction);

                showFlights();

                GameTimeZone tz = this.Airport.Profile.TimeZone;
        
               
                txtLocalTime.Text =string.Format("{0} {1}",MathHelpers.ConvertDateTimeToLoalTime(GameObject.GetInstance().GameTime,tz).ToShortTimeString(), tz.ShortName);

            }
        }
        //creates the panel for the weather
        private Panel createWeatherPanel()
        {
            StackPanel panelWeather = new StackPanel();
            panelWeather.Margin = new Thickness(0, 10, 0, 0);

            TextBlock txtHeader = new TextBlock();
            txtHeader.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            txtHeader.SetResourceReference(TextBlock.BackgroundProperty, "HeaderBackgroundBrush");
            txtHeader.TextAlignment = TextAlignment.Left;
            txtHeader.FontWeight = FontWeights.Bold;
            txtHeader.Text = "Weather Information";

            panelWeather.Children.Add(txtHeader);

            WrapPanel panelWind = new WrapPanel();
            panelWeather.Children.Add(panelWind);

            Image imgWind = new Image();
            imgWind.Source = new BitmapImage(new Uri(@"/Data/images/wind.png", UriKind.RelativeOrAbsolute));
            imgWind.Height = 24;
            imgWind.Width = 24;
            RenderOptions.SetBitmapScalingMode(imgWind, BitmapScalingMode.HighQuality);

            panelWind.Children.Add(imgWind);
            
            txtWind = UICreator.CreateTextBlock(string.Format("{0} ({1:0.##} {2} in {3} direction)", new Converters.TextUnderscoreConverter().Convert(this.Airport.Weather.WindSpeed, null, null, null),new NumberToUnitConverter().Convert((int)this.Airport.Weather.WindSpeed), new StringToLanguageConverter().Convert("km/t"),this.Airport.Weather.Direction));//string.Format("{0} ({1} km/h) in {2} direction", new Converters.TextUnderscoreConverter().Convert(this.Airport.Weather.WindSpeed, null, null, null), (int)this.Airport.Weather.WindSpeed, this.Airport.Weather.Direction));
            txtWind.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            txtWind.Margin = new Thickness(10, 0, 0, 0);

            panelWind.Children.Add(txtWind);

            return panelWeather;
        }

        //creates the panel for arrivals
        private Panel createArrivalsPanel()
        {
            StackPanel panelArrivals = new StackPanel();
            panelArrivals.Margin = new Thickness(0, 10, 0, 0);

            Grid grdType = UICreator.CreateGrid(2);
            grdType.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            panelArrivals.Children.Add(grdType);

            Image imgLogo = new Image();
            imgLogo.Source = new BitmapImage(new Uri(@"/Data/images/Arrivals.png", UriKind.RelativeOrAbsolute));
            imgLogo.Height = 20;
            RenderOptions.SetBitmapScalingMode(imgLogo, BitmapScalingMode.HighQuality);

            Grid.SetColumn(imgLogo, 0);
            grdType.Children.Add(imgLogo);

            TextBlock txtType = UICreator.CreateTextBlock("Arrivals");
            txtType.FontStyle = FontStyles.Oblique;
            txtType.FontSize = 16;

            Grid.SetColumn(txtType, 1);
            grdType.Children.Add(txtType);



            ContentControl txtHeader = new ContentControl();
            txtHeader.ContentTemplate = this.Resources["FlightHeader"] as DataTemplate;
            txtHeader.Content = "From";
            txtHeader.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

            panelArrivals.Children.Add(txtHeader);

            lbArrivals = new ListBox();
            lbArrivals.ItemContainerStyleSelector = new ListBoxItemStyleSelector();
            lbArrivals.ItemTemplate = this.Resources["FlightItem"] as DataTemplate;

            panelArrivals.Children.Add(lbArrivals);


            return panelArrivals;
        }
        //creates the panel for departures
        private Panel createDeparturesPanel()
        {
            StackPanel panelDepartures = new StackPanel();
            panelDepartures.Margin = new Thickness(0, 10, 0, 0);

            Grid grdType = UICreator.CreateGrid(2);
            grdType.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            panelDepartures.Children.Add(grdType);

            Image imgLogo = new Image();
            imgLogo.Source = new BitmapImage(new Uri(@"/Data/images/Departures.png", UriKind.RelativeOrAbsolute));
            imgLogo.Height = 20;
            RenderOptions.SetBitmapScalingMode(imgLogo, BitmapScalingMode.HighQuality);

            Grid.SetColumn(imgLogo, 0);
            grdType.Children.Add(imgLogo);

            TextBlock txtType = UICreator.CreateTextBlock("Departures");
            txtType.FontSize = 16;
            txtType.FontStyle = FontStyles.Oblique;

            Grid.SetColumn(txtType, 1);
            grdType.Children.Add(txtType);

            ContentControl txtHeader = new ContentControl();
            txtHeader.ContentTemplate = this.Resources["FlightHeader"] as DataTemplate;
            txtHeader.Content = "Destination";
            txtHeader.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

            panelDepartures.Children.Add(txtHeader);

            lbDepartures = new ListBox();
            lbDepartures.ItemContainerStyleSelector = new ListBoxItemStyleSelector();
            lbDepartures.ItemTemplate = this.Resources["FlightItem"] as DataTemplate;

            panelDepartures.Children.Add(lbDepartures);


            return panelDepartures;

        }
        //shows the departures and arrivals
        private void showFlights()
        {
            lbArrivals.Items.Clear();
            lbDepartures.Items.Clear();


            GameTimeZone tz = this.Airport.Profile.TimeZone;
   
            foreach (RouteTimeTableEntry entry in GeneralHelpers.GetAirportDepartures(this.Airport))
            {

                if (entry.TimeTable.Route.Airliner.CurrentFlight != null && entry.TimeTable.Route.Airliner.CurrentFlight.Entry == entry)
                    lbDepartures.Items.Add(new AirportFlightItem(MathHelpers.ConvertDateTimeToLoalTime(MathHelpers.ConvertEntryToDate(entry),tz), entry.TimeTable.Route.Airliner.Airliner.Airline, entry.Destination.Airport, entry.Destination.FlightCode, "Planned"));
                else
                    lbDepartures.Items.Add(new AirportFlightItem(MathHelpers.ConvertDateTimeToLoalTime(MathHelpers.ConvertEntryToDate(entry),tz), entry.TimeTable.Route.Airliner.Airliner.Airline, entry.Destination.Airport, entry.Destination.FlightCode, "Planned"));
                

            }
            foreach (RouteTimeTableEntry entry in GeneralHelpers.GetAirportArrivals(this.Airport))
            {

                TimeSpan flightTime = MathHelpers.GetFlightTime(entry.getDepartureAirport().Profile.Coordinates, entry.Destination.Airport.Profile.Coordinates, entry.TimeTable.Route.Airliner.Airliner.Airliner.Type);

                foreach (RouteTimeTableEntry e in entry.TimeTable.Entries)
                    Console.WriteLine(e.Time + " " + flightTime);

                if (entry.TimeTable.Route.Airliner.CurrentFlight != null && entry == entry.TimeTable.Route.Airliner.CurrentFlight.Entry && entry.TimeTable.Route.Airliner.Status == RouteAirliner.AirlinerStatus.On_route)
                    lbArrivals.Items.Add(new AirportFlightItem(MathHelpers.ConvertDateTimeToLoalTime(MathHelpers.ConvertEntryToDate(entry).Add(flightTime),tz), entry.TimeTable.Route.Airliner.Airliner.Airline, entry.Destination.Airport == entry.TimeTable.Route.Destination1 ? entry.TimeTable.Route.Destination2 : entry.TimeTable.Route.Destination1, entry.Destination.FlightCode, string.Format("{0:HH:mm}", entry.TimeTable.Route.Airliner.CurrentFlight.getExpectedLandingTime())));// string.Format("{0:HH:mm}",entry.TimeTable.Route.Airliner.CurrentFlight.getExpectedLandingTime())));
                else
                    lbArrivals.Items.Add(new AirportFlightItem(MathHelpers.ConvertDateTimeToLoalTime(MathHelpers.ConvertEntryToDate(entry).Add(flightTime),tz), entry.TimeTable.Route.Airliner.Airliner.Airline, entry.Destination.Airport == entry.TimeTable.Route.Destination1 ? entry.TimeTable.Route.Destination2 : entry.TimeTable.Route.Destination1, entry.Destination.FlightCode, string.Format("{0:HH:mm}", "Planned")));

            }
        }
        //creates the quick info panel for the airport
        private Panel createQuickInfoPanel()
        {
            StackPanel panelInfo = new StackPanel();
            //panelInfo.Margin = new Thickness(5, 0, 10, 10);

            TextBlock txtHeader = new TextBlock();
            txtHeader.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            txtHeader.SetResourceReference(TextBlock.BackgroundProperty, "HeaderBackgroundBrush");
            txtHeader.TextAlignment = TextAlignment.Left;
            txtHeader.FontWeight = FontWeights.Bold;
            txtHeader.Text = "Profile";

            panelInfo.Children.Add(txtHeader);

            DockPanel grdQuickInfo = new DockPanel();
            grdQuickInfo.Margin = new Thickness(0, 5, 0, 0);

            panelInfo.Children.Add(grdQuickInfo);


            Image imgAirport = new Image();
            imgAirport.Source = this.Airport.Profile.Logo.Length > 0 ? new BitmapImage(new Uri(this.Airport.Profile.Logo, UriKind.RelativeOrAbsolute)) : new BitmapImage(new Uri(@"/Data/images/airport.png", UriKind.Relative));
            imgAirport.Width = 110;
            imgAirport.Margin = new Thickness(0, 0, 5, 0);
            imgAirport.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            RenderOptions.SetBitmapScalingMode(imgAirport, BitmapScalingMode.HighQuality);
            grdQuickInfo.Children.Add(imgAirport);

            StackPanel panelQuickInfo = new StackPanel();

            grdQuickInfo.Children.Add(panelQuickInfo);

            ListBox lbQuickInfo = new ListBox();
            lbQuickInfo.ItemContainerStyleSelector = new ListBoxItemStyleSelector();
            lbQuickInfo.SetResourceReference(ListBox.ItemTemplateProperty, "QuickInfoItem");

            panelQuickInfo.Children.Add(lbQuickInfo);

            lbQuickInfo.Items.Add(new QuickInfoValue("Name", UICreator.CreateTextBlock(this.Airport.Profile.Name)));
            lbQuickInfo.Items.Add(new QuickInfoValue("IATA code", UICreator.CreateTextBlock(this.Airport.Profile.IATACode)));
            lbQuickInfo.Items.Add(new QuickInfoValue("Airport type", UICreator.CreateTextBlock(this.Airport.Profile.Type.ToString())));
            lbQuickInfo.Items.Add(new QuickInfoValue("Town", UICreator.CreateTextBlock(this.Airport.Profile.Town)));

            ContentControl lblFlag = new ContentControl();
            lblFlag.SetResourceReference(ContentControl.ContentTemplateProperty, "CountryFlagLongItem");
            lblFlag.Content = this.Airport.Profile.Country;

            lbQuickInfo.Items.Add(new QuickInfoValue("Country", lblFlag));

         
            //TimeZoneInfo timeZone = TimeZoneInfo.GetSystemTimeZones().ToList().Find((delegate(TimeZoneInfo tzInfo) { return tzInfo.BaseUtcOffset == this.Airport.Profile.OffsetDST; }));
            GameTimeZone tz = this.Airport.Profile.TimeZone;
                        
            TextBlock txtTimeZone = UICreator.CreateTextBlock(tz.DisplayName);//(string.Format("UTC{0}{1}", this.Airport.Profile.OffsetDST < 0 ? "" : "+", this.Airport.Profile.OffsetDST));
            lbQuickInfo.Items.Add(new QuickInfoValue("Time zone", txtTimeZone));

            txtLocalTime = UICreator.CreateTextBlock(string.Format("{0} {1}",MathHelpers.ConvertDateTimeToLoalTime(GameObject.GetInstance().GameTime,tz).ToShortTimeString(), tz.ShortName));

            lbQuickInfo.Items.Add(new QuickInfoValue("Local time", txtLocalTime));

            WrapPanel panelCoordinates = new WrapPanel();
            
            Image imgMap = new Image();
            imgMap.Source = new BitmapImage(new Uri(@"/Data/images/map.png", UriKind.RelativeOrAbsolute));
            imgMap.Height = 16;
            imgMap.MouseDown += new MouseButtonEventHandler(imgMap_MouseDown);
            RenderOptions.SetBitmapScalingMode(imgMap, BitmapScalingMode.HighQuality);

            imgMap.Margin = new Thickness(2, 0, 0, 0);
            panelCoordinates.Children.Add(imgMap);


            TextBlock txtCoordinates = UICreator.CreateLink(this.Airport.Profile.Coordinates.ToString());
            ((Hyperlink)txtCoordinates.Inlines.FirstInline).Click += new RoutedEventHandler(PageAirport_Click);
            txtCoordinates.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            panelCoordinates.Children.Add(txtCoordinates);


            lbQuickInfo.Items.Add(new QuickInfoValue("Coordinates", panelCoordinates));
            lbQuickInfo.Items.Add(new QuickInfoValue("Airport size", UICreator.CreateTextBlock(new TextUnderscoreConverter().Convert(this.Airport.Profile.Size, null, null, null).ToString())));
            lbQuickInfo.Items.Add(new QuickInfoValue("Number of gates", UICreator.CreateTextBlock(this.Airport.Profile.Gates.ToString())));




            return panelInfo;

        }

        private void PageAirport_Click(object sender, RoutedEventArgs e)
        {
            PopUpMap.ShowPopUp(this.Airport);
        }

        private void imgMap_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PopUpMap.ShowPopUp(this.Airport);
        }
        //the class for a flight at the airport
        private class AirportFlightItem
        {
            public Airline Airline { get; set; }
            public string Flight { get; set; }
            public DateTime Time { get; set; }
            public Airport Airport { get; set; }
            public string Status { get; set; }
            public AirportFlightItem(DateTime time, Airline airline, Airport airport, string flight, string status)
            {
                this.Time = time;
                this.Airport = airport;
                this.Airline = airline;
                this.Flight = flight;
                this.Status = status;
            }
        }
    }

}