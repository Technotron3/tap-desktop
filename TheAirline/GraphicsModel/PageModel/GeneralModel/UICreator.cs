﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using TheAirline.Model.AirlineModel;

namespace TheAirline.GraphicsModel.PageModel.GeneralModel
{
    public class UICreator
    {
         //creates a standard text block. Font size 0 for default
        public static TextBlock CreateTextBlock(string text)
        {
            TextBlock txtText = new TextBlock();
            txtText.Text = text;
            
          
            return txtText;
        }
        //creates a color rect
        public static Rectangle CreateColorRect(string color)
        {
            TypeConverter colorConverter = new ColorConverter();
            Color c = (Color)colorConverter.ConvertFromString(color);

            Color c2 = Color.FromArgb(25, c.R, c.G, c.B);

            LinearGradientBrush colorBrush = new LinearGradientBrush();
            colorBrush.StartPoint = new Point(0, 0);
            colorBrush.EndPoint = new Point(0, 1);
            colorBrush.GradientStops.Add(new GradientStop(c2, 0.2));
            colorBrush.GradientStops.Add(new GradientStop(c, 0.85));
            colorBrush.GradientStops.Add(new GradientStop(c2, 1));


            Rectangle rectColor = new Rectangle();
            rectColor.Width = 50;
            rectColor.Height = 20;
            rectColor.Stroke = Brushes.Black;
            rectColor.StrokeThickness = 2;
            rectColor.Fill = colorBrush;
            rectColor.Margin = new Thickness(0, 0, 5, 0);
            rectColor.RadiusX = 10;
            rectColor.RadiusY = 10;


            return rectColor;

        }
        //creates the game logo
        public static Panel CreateGameLogo()
        {
            //Silver Gradient

            StackPanel panelLogo = new StackPanel();

            LinearGradientBrush brushGold = new LinearGradientBrush();
            brushGold.StartPoint = new Point(0, 0);
            brushGold.EndPoint = new Point(0,1);
            brushGold.GradientStops.Add(new GradientStop(Colors.Gold, 0.0));
            brushGold.GradientStops.Add(new GradientStop(Colors.Black, 0.05));
            brushGold.GradientStops.Add(new GradientStop(Colors.Gold, 1.0));

            LinearGradientBrush silverBrush = new LinearGradientBrush();
            silverBrush.StartPoint = new Point(0, 0);
            silverBrush.EndPoint = new Point(0, 1);
            silverBrush.GradientStops.Add(new GradientStop(Colors.Silver, 0.0));
            silverBrush.GradientStops.Add(new GradientStop(Colors.White, 0.15));
            silverBrush.GradientStops.Add(new GradientStop(Colors.Silver, 1.0));

            DropShadowEffect shadowEffect = new DropShadowEffect();
            shadowEffect.ShadowDepth = 5;
            shadowEffect.Direction = 330;
            shadowEffect.Color = Colors.Black;
            shadowEffect.Opacity = 0.5;
            shadowEffect.BlurRadius = 4;
        
            TextBlock txtHeader = new TextBlock();
            txtHeader.FontFamily = new FontFamily("Segoe Print");
            txtHeader.HorizontalAlignment = HorizontalAlignment.Center;
            txtHeader.FontSize = 56;
            txtHeader.Foreground = brushGold;
            txtHeader.FontWeight = FontWeights.Bold;
            txtHeader.Text = "TheAirline";
            txtHeader.Effect = shadowEffect;

            panelLogo.Children.Add(txtHeader);

            TextBlock txtSubHeader = new TextBlock();
            txtSubHeader.FontFamily = new FontFamily("Segoe Print");
            txtSubHeader.Margin = new Thickness(0, -30, 0, 0);
            txtSubHeader.HorizontalAlignment = HorizontalAlignment.Center;
            txtSubHeader.FontSize = 16;
            txtSubHeader.Foreground = silverBrush;
            txtSubHeader.Text = "Come fly with us!";

            panelLogo.Children.Add(txtSubHeader);

            return panelLogo;
        }
        //creates an image button
        public static Button CreateImageButton(string name, string image, int size)
        {

           
                Button btnButton = new Button();
                btnButton.Name = name;
                btnButton.Width = size;
                btnButton.Height = size;
                btnButton.Background = null;
                //btnButton.BorderBrush = null;
                //btnButton.BorderThickness = new Thickness(0);
                //btnButton.SetResourceReference(Button.StyleProperty,"TransparentButton");
         

                Image img = new Image();

                img.Source = new BitmapImage(
                    new Uri(image, UriKind.Relative));
                img.Stretch = Stretch.Fill;
                RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.HighQuality);

                btnButton.Content = img;

                return btnButton;



        }
        //creates a link
        public static TextBlock CreateLink(string text)
        {
           
            TextBlock txtBlock = new TextBlock();
        
            Run run = new Run(text);
           
            Hyperlink hyperLink = new Hyperlink(run);
            //hyperLink.FontSize = fontsize;
            txtBlock.Inlines.Add(hyperLink);
           // txtBlock.VerticalAlignment = VerticalAlignment.Center;
            return txtBlock;

        }
        //creates a grid with x-columns
        public static Grid CreateGrid(int columns)
        {
            Grid grid = new Grid();
            for (int i = 0; i < columns; i++)
            {
                ColumnDefinition columnDef = new ColumnDefinition();
                grid.ColumnDefinitions.Add(columnDef);
            }
            return grid;
        }
        //creates a grid with x-columns and y-rows
        public static Grid CreateGrid(int columns, int rows)
        {
            Grid grid = new Grid();
            for (int i = 0; i < columns; i++)
            {
                ColumnDefinition columnDef = new ColumnDefinition();
                //columnDef.Width = new GridLength(100);
                grid.ColumnDefinitions.Add(columnDef);
            }
            for (int i = 0; i < rows; i++)
            {
                RowDefinition rowDef = new RowDefinition();
                grid.RowDefinitions.Add(rowDef);
            }
            return grid;
        }
        //returns the object from a listbox
        public static object GetDataFromListBox(ListBox source, Point point)
        {
            UIElement element = source.InputHitTest(point) as UIElement;
            if (element != null)
            {
                object data = DependencyProperty.UnsetValue;
                while (data == DependencyProperty.UnsetValue)
                {
                    data = source.ItemContainerGenerator.ItemFromContainer(element);

                    if (data == DependencyProperty.UnsetValue)
                    {
                        element = VisualTreeHelper.GetParent(element) as UIElement;
                    }

                    if (element == source)
                    {
                        return null;
                    }
                }

                if (data != DependencyProperty.UnsetValue)
                {
                    return data;
                }
            }

            return null;
        }
    }
    public class QuickInfoValue
    {
        public string Name { get; set; }
        public UIElement Value { get; set; }
        public QuickInfoValue(string name, UIElement value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
    public class ListBoxItemStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item,
           DependencyObject container)
        {

            Trigger trigger = new Trigger();
            trigger.Property = ListBoxItem.IsFocusedProperty;
            trigger.Value = false;

            Style st = new Style();
            st.TargetType = typeof(ListBoxItem);
            Setter backGroundSetter = new Setter();
            backGroundSetter.Property = ListBoxItem.BackgroundProperty;

            Setter focusVisualSetter = new Setter();
            focusVisualSetter.Property = ListBoxItem.FocusVisualStyleProperty;
            focusVisualSetter.Value = null;

            ListBox listBox =
                ItemsControl.ItemsControlFromItemContainer(container)
                  as ListBox;
            int index =
                listBox.ItemContainerGenerator.IndexFromContainer(container);
            if (index % 2 == 0)
            {


                //Brush brush = new SolidColorBrush(Color.FromRgb(33, 59, 84));
                Brush brush = new SolidColorBrush(Colors.Gray);
                brush.Opacity = 0.50;

                backGroundSetter.Value = brush;

                st.Resources.Add(SystemColors.HighlightBrushKey, brush);
                st.Resources.Add(SystemColors.ControlBrushKey, brush);
                
            }
            else
            {
                //Brush brush = new SolidColorBrush(Color.FromRgb(15, 36, 65));
                Brush brush = new SolidColorBrush(Colors.DarkGray);
                brush.Opacity = 0.50;

                backGroundSetter.Value = brush;
                //backGroundSetter.Value = Brushes.Beige;

                st.Resources.Add(SystemColors.HighlightBrushKey, brush);
                st.Resources.Add(SystemColors.ControlBrushKey, brush);
            }
            trigger.Setters.Add(backGroundSetter);

            st.Triggers.Add(trigger);
            st.Setters.Add(backGroundSetter);
            st.Setters.Add(focusVisualSetter);

            return st;
        }
    }
    
}