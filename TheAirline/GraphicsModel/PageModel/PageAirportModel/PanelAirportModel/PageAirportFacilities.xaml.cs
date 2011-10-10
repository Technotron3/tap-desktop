﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TheAirline.Model.AirportModel;
using TheAirline.Model.AirlineModel;
using TheAirline.Model.GeneralModel;
using TheAirline.GraphicsModel.PageModel.GeneralModel;
using TheAirline.GraphicsModel.PageModel.PageAirlineModel;
using TheAirline.GraphicsModel.UserControlModel.MessageBoxModel;

namespace TheAirline.GraphicsModel.PageModel.PageAirportModel.PanelAirportModel
{
    /// <summary>
    /// Interaction logic for PageAirportFacilities.xaml
    /// </summary>
    public partial class PageAirportFacilities : Page
    {
        private Airport Airport;
        private StackPanel panelFacilities;
        public PageAirportFacilities(Airport airport)
        {
            this.Language = XmlLanguage.GetLanguage(new CultureInfo(GameObject.GetInstance().getLanguage().CultureInfo, true).IetfLanguageTag); 

            this.Airport = airport;

            InitializeComponent();

            panelFacilities = new StackPanel();
            panelFacilities.Margin = new Thickness(0, 10, 50, 0);

            showFacilitiesInformation();

            this.Content = panelFacilities;
        }
        //shows the facilities information
        private void showFacilitiesInformation()
        {
            panelFacilities.Children.Clear();



            TextBlock txtFacilitiesHeader = new TextBlock();
            txtFacilitiesHeader.Margin = new Thickness(0, 10, 0, 0);
            txtFacilitiesHeader.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            txtFacilitiesHeader.SetResourceReference(TextBlock.BackgroundProperty, "HeaderBackgroundBrush2");
            txtFacilitiesHeader.FontWeight = FontWeights.Bold;
            txtFacilitiesHeader.Text = "Airport Facilities";

            panelFacilities.Children.Add(txtFacilitiesHeader);

            ListBox lbAirportFacilities = new ListBox();
            lbAirportFacilities.ItemContainerStyleSelector = new ListBoxItemStyleSelector();
            lbAirportFacilities.ItemTemplate = this.Resources["AirlineFacilityItem"] as DataTemplate;


            panelFacilities.Children.Add(lbAirportFacilities);

            Array types = Enum.GetValues(typeof(AirportFacility.FacilityType));

            List<Airline> airlines = Airlines.GetAirlines();
            airlines.Sort((delegate(Airline a1, Airline a2) { return a1.Profile.Name.CompareTo(a2.Profile.Name); }));


            foreach (Airline airline in airlines)
            {
                //foreach (AirportFacility.FacilityType type in Enum.GetValues(typeof(AirportFacility.FacilityType)))
                for (int i = 0; i < types.Length; i++)
                {

                    lbAirportFacilities.Items.Add(new AirlineFacilityType(i == 0 ? airline : null, this.Airport.getAirportFacility(airline, (AirportFacility.FacilityType)types.GetValue(i))));
                }
            }

            TextBlock txtHumanFacilitiesHeader = new TextBlock();
            txtHumanFacilitiesHeader.Margin = new Thickness(0, 10, 0, 0);
            txtHumanFacilitiesHeader.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            txtHumanFacilitiesHeader.SetResourceReference(TextBlock.BackgroundProperty, "HeaderBackgroundBrush2");
            txtHumanFacilitiesHeader.FontWeight = FontWeights.Bold;
            txtHumanFacilitiesHeader.Text = GameObject.GetInstance().HumanAirline.Profile.Name + " Facilities";

            panelFacilities.Children.Add(txtHumanFacilitiesHeader);

            ListBox lbHumanFacilities = new ListBox();
            lbHumanFacilities.ItemContainerStyleSelector = new ListBoxItemStyleSelector();
            lbHumanFacilities.ItemTemplate = this.Resources["HumanFacilityItem"] as DataTemplate;

        
            panelFacilities.Children.Add(lbHumanFacilities);

            foreach (AirportFacility.FacilityType type in Enum.GetValues(typeof(AirportFacility.FacilityType)))
            {
                lbHumanFacilities.Items.Add(new HumanFacilityType(this.Airport, this.Airport.getAirportFacility(GameObject.GetInstance().HumanAirline, type), getHumanNextAirportFacility(type)));
            }

            

           

        }
        //returns the next facility item for the specific type for the human airline
        private AirportFacility getHumanNextAirportFacility(AirportFacility.FacilityType type)
        {
            List<AirportFacility> facilities = AirportFacilities.GetFacilities(type);

            facilities.Sort((delegate(AirportFacility f1, AirportFacility f2) { return f1.TypeLevel.CompareTo(f2.TypeLevel); }));

            int index = facilities.IndexOf(this.Airport.getAirportFacility(GameObject.GetInstance().HumanAirline, type));

            if (index < facilities.Count - 1)
                return facilities[index + 1];
            else
                return facilities[0];
        }
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Airline airline = (Airline)((Hyperlink)sender).Tag;

            PageNavigator.NavigateTo(new PageAirline(airline));


        }

        //the class for a facility type for an airline
        private class AirlineFacilityType
        {
            public Airline Airline { get; set; }
            public AirportFacility Facility { get; set; }
            public AirlineFacilityType(Airline airline, AirportFacility facility)
            {
                this.Airline = airline;
                this.Facility = facility;

            }
        }

        private void ButtonBuy_Click(object sender, RoutedEventArgs e)
        {
            HumanFacilityType type = (HumanFacilityType)((Button)sender).Tag;

            if (type.NextFacility.Price > GameObject.GetInstance().HumanAirline.Money)
                WPFMessageBox.Show("Not enough money", "You don't have any money to buy these facilities", WPFMessageBoxButtons.Ok);
            else
            {
                WPFMessageBoxResult result = WPFMessageBox.Show("Upgrade facility", string.Format("Are you sure you want to upgrade the facilities to {0} for {1:c}?", type.NextFacility.Name, type.NextFacility.Price), WPFMessageBoxButtons.YesNo);

                if (result == WPFMessageBoxResult.Yes)
                {
                    GameObject.GetInstance().HumanAirline.addInvoice(new Invoice(GameObject.GetInstance().GameTime, Invoice.InvoiceType.Purchases, -type.NextFacility.Price));

                    this.Airport.setAirportFacility(GameObject.GetInstance().HumanAirline, type.NextFacility);

                    showFacilitiesInformation();
                }
            }
        }

        private void ButtonSell_Click(object sender, RoutedEventArgs e)
        {
               HumanFacilityType type = (HumanFacilityType)((Button)sender).Tag;

               if (type.CurrentFacility.TypeLevel == 1 && this.Airport.hasAsHomebase(GameObject.GetInstance().HumanAirline))
                   WPFMessageBox.Show("Error", "The airport serves as homebase for a fleet airliner, so it need to have at least basic service facilities", WPFMessageBoxButtons.Ok);
               else
               {

                   WPFMessageBoxResult result = WPFMessageBox.Show("Downgrade facility", string.Format("Are you sure you want to downgrade the facilities from {0}?", type.CurrentFacility.Name), WPFMessageBoxButtons.YesNo);

                   if (result == WPFMessageBoxResult.Yes)
                   {


                       this.Airport.downgradeFacility(GameObject.GetInstance().HumanAirline, type.NextFacility.Type);

                       showFacilitiesInformation();
                   }
               }
        }
    }


    //the class for a facility type for the human airline
    public class HumanFacilityType
    {
        public AirportFacility CurrentFacility { get; set; }
        public AirportFacility NextFacility { get; set; }
        public Airport Airport { get; set; }
        public HumanFacilityType(Airport airport, AirportFacility current, AirportFacility next)
        {
            this.Airport = airport;
            this.CurrentFacility = current;
            this.NextFacility = next;
        }
    }
    //the converter for a facility button being enabled
    public class HumanFacilityButtonEnabled : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Boolean isEnabled = true;

            Airport airport = ((HumanFacilityType)value).Airport;

            AirportFacility.FacilityType type = ((HumanFacilityType)value).CurrentFacility.Type;

            string buttonType = (string)parameter;

            List<AirportFacility> facilities = AirportFacilities.GetFacilities(type);

            facilities.Sort((delegate(AirportFacility f1, AirportFacility f2) { return f1.TypeLevel.CompareTo(f2.TypeLevel); }));

            int index = facilities.IndexOf(airport.getAirportFacility(GameObject.GetInstance().HumanAirline, type));

            if (buttonType == "Buy")
            {
                isEnabled = index < facilities.Count - 1 && airport.Gates.getNumberOfGates(GameObject.GetInstance().HumanAirline)>0; 
            }
            if (buttonType == "Sell")
            {
                isEnabled = index > 0;
            }

            return isEnabled;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}