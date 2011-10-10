﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TheAirline.Model.AirlinerModel;
using TheAirline.Model.AirlinerModel.RouteModel;
using TheAirline.Model.AirlineModel;
using TheAirline.Model.AirportModel;
using TheAirline.Model.GeneralModel;
using TheAirline.GraphicsModel.Converters;
using TheAirline.GraphicsModel.PageModel.GeneralModel;
using TheAirline.GraphicsModel.PageModel.PageRouteModel.PanelRoutesModel;
using TheAirline.GraphicsModel.UserControlModel.PopUpWindowsModel;

namespace TheAirline.GraphicsModel.PageModel.PageFleetAirlinerModel.PanelFleetAirlinerModel
{
    /// <summary>
    /// Interaction logic for PageFleetRoute.xaml
    /// </summary>
    public partial class PageFleetRoute : Page
    {
        private FleetAirliner Airliner;
        private TextBlock txtStatus, txtDestination, txtPosition, txtPassengers;
        private Button btnStopFlight, btnStartFlight;
        public PageFleetRoute(FleetAirliner airliner)
        {
            InitializeComponent();

            this.Airliner = airliner;

            // Start + stop plane og Update flight distance

            InitializeComponent();

            StackPanel panelRoute = new StackPanel();
            panelRoute.Margin = new Thickness(0, 10, 50, 0);

            TextBlock txtHeader = new TextBlock();
            txtHeader.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            txtHeader.SetResourceReference(TextBlock.BackgroundProperty, "HeaderBackgroundBrush2");
            txtHeader.FontWeight = FontWeights.Bold;
            txtHeader.Text = "The airliner is not assigned to a route";



            if (this.Airliner.RouteAirliner != null)
            {
                panelRoute.Children.Add(createRouteInfo());
                panelRoute.Children.Add(createFlightInfo());

                if (this.Airliner.Airline == GameObject.GetInstance().HumanAirline) panelRoute.Children.Add(createFlightButtons());
            }
            else panelRoute.Children.Add(txtHeader);
            /*
            TextBlock txtRouteHeader = new TextBlock();
            txtRouteHeader.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            txtRouteHeader.SetResourceReference(TextBlock.BackgroundProperty, "HeaderBackgroundBrush2");
            txtRouteHeader.Margin = new System.Windows.Thickness(0, 5, 0, 0);
            txtRouteHeader.Text = "Route Information";

            panelRoute.Children.Add(txtRouteHeader);

            if (this.Airliner.Airline == GameObject.GetInstance().HumanAirline) panelRoute.Children.Add(createHumanRouteInfo());
            else panelRoute.Children.Add(createComputerRouteInfo());
          **/

            this.Content = panelRoute;

            GameTimer.GetInstance().OnTimeChanged += new GameTimer.TimeChanged(PageFleetRoute_OnTimeChanged);

        }

        private void PageFleetRoute_OnTimeChanged()
        {


            if (this.IsLoaded)
            {
                if (this.Airliner.HasRoute)
                {
                    Airport airport = Airports.GetAirport(this.Airliner.RouteAirliner.CurrentPosition);

                    txtStatus.Text = new TextUnderscoreConverter().Convert(this.Airliner.RouteAirliner.Status, null, null, null).ToString();
                    txtDestination.Text = this.Airliner.RouteAirliner.CurrentFlight == null ? "Not started" : this.Airliner.RouteAirliner.CurrentFlight.Entry.Destination.Airport.Profile.Name;
                    txtPosition.Text = this.Airliner.HasRoute ? (airport == null ? this.Airliner.RouteAirliner.CurrentPosition.ToString() : airport.Profile.Name) : this.Airliner.Homebase.Profile.Name;
                    txtPassengers.Text = string.Format("{0}", this.Airliner.RouteAirliner.CurrentFlight == null ? 0 : this.Airliner.RouteAirliner.CurrentFlight.getTotalPassengers());
                }
            }
        }
        //creates the buttons for the flight
        private WrapPanel createFlightButtons()
        {
            WrapPanel buttonsPanel = new WrapPanel();
            buttonsPanel.Margin = new Thickness(0, 5, 0, 0);
            buttonsPanel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

            btnStartFlight = new Button();
            btnStartFlight.SetResourceReference(Button.StyleProperty, "RoundedButton");
            btnStartFlight.Height = 20;
            btnStartFlight.Width = 100;
            btnStartFlight.Content = "Start flight";
            btnStartFlight.IsEnabled = this.Airliner.RouteAirliner.Status == RouteAirliner.AirlinerStatus.Stopped;
            btnStartFlight.Click += new RoutedEventHandler(btnStartFligth_Click);
            btnStartFlight.SetResourceReference(Button.BackgroundProperty, "ButtonBrush");

            buttonsPanel.Children.Add(btnStartFlight);

            btnStopFlight = new Button();
            btnStopFlight.SetResourceReference(Button.StyleProperty, "RoundedButton");
            btnStopFlight.Width = 100;
            btnStopFlight.Height = 20;
            btnStopFlight.Content = "Stop flight";
            btnStopFlight.IsEnabled = this.Airliner.RouteAirliner.Status != RouteAirliner.AirlinerStatus.Stopped;
            btnStopFlight.SetResourceReference(Button.BackgroundProperty, "ButtonBrush");
            btnStopFlight.Margin = new Thickness(5, 0, 0, 0);
            btnStopFlight.Click += new RoutedEventHandler(btnStopFlight_Click);
            buttonsPanel.Children.Add(btnStopFlight);

            return buttonsPanel;


        }

        private void btnStopFlight_Click(object sender, RoutedEventArgs e)
        {
            this.Airliner.RouteAirliner.Status = RouteAirliner.AirlinerStatus.To_homebase;

            btnStartFlight.IsEnabled = true;

            btnStopFlight.IsEnabled = false;
        }

        private void btnStartFligth_Click(object sender, RoutedEventArgs e)
        {
            this.Airliner.RouteAirliner.Status = RouteAirliner.AirlinerStatus.To_route_start;

            btnStartFlight.IsEnabled = false;

            btnStopFlight.IsEnabled = true;
        }
        //creates the flight details
        private StackPanel createFlightInfo()
        {

            StackPanel panelFlight = new StackPanel();
            panelFlight.Margin = new Thickness(0, 5, 0, 0);

            TextBlock txtHeader = new TextBlock();
            txtHeader.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            txtHeader.SetResourceReference(TextBlock.BackgroundProperty, "HeaderBackgroundBrush2");
            txtHeader.FontWeight = FontWeights.Bold;
            txtHeader.Text = "Flight Information";
            panelFlight.Children.Add(txtHeader);


            ListBox lbFlightInfo = new ListBox();
            lbFlightInfo.ItemContainerStyleSelector = new ListBoxItemStyleSelector();
            lbFlightInfo.SetResourceReference(ListBox.ItemTemplateProperty, "QuickInfoItem");


            txtDestination = UICreator.CreateTextBlock(this.Airliner.RouteAirliner.CurrentFlight == null ? "Not started" : this.Airliner.RouteAirliner.CurrentFlight.Entry.Destination.Airport.Profile.Name);
            txtStatus = UICreator.CreateTextBlock((new TextUnderscoreConverter().Convert(this.Airliner.RouteAirliner.Status, null, null, null).ToString()));
            txtPosition = UICreator.CreateTextBlock(this.Airliner.HasRoute ? this.Airliner.RouteAirliner.CurrentPosition.ToString() : this.Airliner.Homebase.Profile.Name);
            txtPassengers = UICreator.CreateTextBlock(string.Format("{0}", this.Airliner.RouteAirliner.CurrentFlight == null ? 0 : this.Airliner.RouteAirliner.CurrentFlight.getTotalPassengers()));

            lbFlightInfo.Items.Add(new QuickInfoValue("Current status", txtStatus));
            lbFlightInfo.Items.Add(new QuickInfoValue("Current position", txtPosition));
            lbFlightInfo.Items.Add(new QuickInfoValue("Next destination", txtDestination));
            lbFlightInfo.Items.Add(new QuickInfoValue("Passengers", txtPassengers));

            panelFlight.Children.Add(lbFlightInfo);

            return panelFlight;
        }
        //creates the route details
        private StackPanel createRouteInfo()
        {

            Route route = this.Airliner.RouteAirliner.Route;

            StackPanel panelRoute = new StackPanel();

            TextBlock txtHeader = new TextBlock();
            txtHeader.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            txtHeader.SetResourceReference(TextBlock.BackgroundProperty, "HeaderBackgroundBrush2");
            txtHeader.FontWeight = FontWeights.Bold;
            txtHeader.Text = "Route Information";
            panelRoute.Children.Add(txtHeader);


            ListBox lbRouteInfo = new ListBox();
            lbRouteInfo.ItemContainerStyleSelector = new ListBoxItemStyleSelector();
            lbRouteInfo.SetResourceReference(ListBox.ItemTemplateProperty, "QuickInfoItem");

            panelRoute.Children.Add(lbRouteInfo);

            double distance = MathHelpers.GetDistance(route.Destination1.Profile.Coordinates, route.Destination2.Profile.Coordinates);


            lbRouteInfo.Items.Add(new QuickInfoValue("Destination 1", UICreator.CreateTextBlock(route.Destination1.Profile.Name)));
            lbRouteInfo.Items.Add(new QuickInfoValue("Destination 2", UICreator.CreateTextBlock(route.Destination2.Profile.Name)));
            // chs, 2011-10-10 added missing conversion
            lbRouteInfo.Items.Add(new QuickInfoValue("Distance", UICreator.CreateTextBlock(string.Format("{0:0.00} {1}", new NumberToUnitConverter().Convert(distance), new StringToLanguageConverter().Convert("km.")))));

            //foreach (AirlinerClass.ClassType type in Enum.GetValues(typeof(AirlinerClass.ClassType)))
            foreach (AirlinerClass aClass in this.Airliner.Airliner.Classes)
            {
                RouteAirlinerClass rClass = this.Airliner.RouteAirliner.Route.getRouteAirlinerClass(aClass.Type);


                Image imgInfo = new Image();
                imgInfo.Width = 16;
                imgInfo.Source = new BitmapImage(new Uri(@"/Data/images/info.png", UriKind.RelativeOrAbsolute));
                imgInfo.Margin = new Thickness(5, 0, 0, 0);
                RenderOptions.SetBitmapScalingMode(imgInfo, BitmapScalingMode.HighQuality);

                Border brdToolTip = new Border();
                brdToolTip.Margin = new Thickness(-4, 0, -4, -3);
                brdToolTip.Padding = new Thickness(5);
                brdToolTip.SetResourceReference(Border.BackgroundProperty, "HeaderBackgroundBrush2");


                ContentControl lblClass = new ContentControl();
                lblClass.SetResourceReference(ContentControl.ContentTemplateProperty, "RouteAirlinerClassItem");
                lblClass.Content = rClass;

                brdToolTip.Child = lblClass;


                imgInfo.ToolTip = brdToolTip;


                lbRouteInfo.Items.Add(new QuickInfoValue(new TextUnderscoreConverter().Convert(aClass.Type, null, null, null).ToString(), imgInfo));
            }
            //lbRouteInfo.Items.Add(new QuickInfoValue("Fare Price", UICreator.CreateTextBlock(string.Format("{0:c}", route.FarePrice))));
            // lbRouteInfo.Items.Add(new QuickInfoValue("Food on board", UICreator.CreateTextBlock(route.FoodFacility.Name)));
            // lbRouteInfo.Items.Add(new QuickInfoValue("Drinks on board", UICreator.CreateTextBlock(route.DrinksFacility.Name)));
            //lbRouteInfo.Items.Add(new QuickInfoValue("Cabin crew on board", UICreator.CreateTextBlock(string.Format("{0}", route.CabinCrew))));

            WrapPanel buttonsPanel = new WrapPanel();
            buttonsPanel.Margin = new Thickness(0, 5, 0, 0);

            Button btnTimeTable = new Button();
            btnTimeTable.SetResourceReference(Button.StyleProperty, "RoundedButton");
            btnTimeTable.Height = 20;
            btnTimeTable.Width = 100;
            btnTimeTable.Content = "Timetable";
            btnTimeTable.Visibility = this.Airliner.RouteAirliner.Airliner.Airline.IsHuman ? Visibility.Collapsed : Visibility.Visible;
            btnTimeTable.Click += new RoutedEventHandler(btnTimeTable_Click);
            btnTimeTable.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            btnTimeTable.SetResourceReference(Button.BackgroundProperty, "ButtonBrush");

            buttonsPanel.Children.Add(btnTimeTable);

            Button btnMap = new Button();
            btnMap.SetResourceReference(Button.StyleProperty, "RoundedButton");
            btnMap.Width = 100;
            btnMap.Height = 20;
            btnMap.Content = "Route map";
            btnMap.Margin = new Thickness(2, 0, 0, 0);
            btnMap.SetResourceReference(Button.BackgroundProperty, "ButtonBrush");
            btnMap.Click += new RoutedEventHandler(btnMap_Click);

            buttonsPanel.Children.Add(btnMap);

            panelRoute.Children.Add(buttonsPanel);

            return panelRoute;

        }

        private void btnMap_Click(object sender, RoutedEventArgs e)
        {
            PopUpMap.ShowPopUp(this.Airliner.RouteAirliner.Route);
        }

        private void btnTimeTable_Click(object sender, RoutedEventArgs e)
        {
            PopUpTimeTable.ShowPopUp(this.Airliner.Airline, this.Airliner.RouteAirliner.Route);
        }

    }
}