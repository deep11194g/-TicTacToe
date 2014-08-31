using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;

namespace TicTacToe
{
    public sealed partial class StartPage : TicTacToe.Common.LayoutAwarePage
    {
        int mm = 0, nn = 0;
        String name1 = "X", name2 = "O";
        
        public StartPage()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }
        
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private void click_go(object sender, RoutedEventArgs e)         //function invoked on GO button click
        {           
            try
            {
                mm = Convert.ToInt16(m_box.Text);                       //get grid size
                nn = Convert.ToInt16(n_box.Text);                       //get streak size

                String n1 = player1_name.Text, n2 = player2_name.Text;

                if (n1.Length > 4)                                      //forming names to send
                    name1 = n1.Substring(0, 4);
                else if (n1.Length == 0)
                    name1 = "X";
                else
                    name1 = n1;

                if (n2.Length > 4)
                    name2 = n2.Substring(0, 4);
                else if (n2.Length == 0)
                    name2 = "X";
                else
                    name2 = n2;

                String[] ar = { mm+"", nn+"", name1, name2 };

                if (mm < 3 || nn < 2 || nn > mm || mm > 7)              //raise exception if m & n not according to rules
                    throw (new FormatException());

                Frame.Navigate(typeof(MainPage), ar);
            }
            catch (FormatException)                                     //exception handling when rules violated or number not entered                               
            {
                Help.Background = new SolidColorBrush(Colors.Red);
                grid_size.Foreground = new SolidColorBrush(Colors.Red); //change color to red to indicate error
                m_box.Text = "";
                streak.Foreground = new SolidColorBrush(Colors.Red);
                n_box.Text = "";
                msg.Text = "Read rules !! ";                            //remind user about rules
                        
            }
        }

        private void reset(object sender, RoutedEventArgs e)
        {
            grid_size.Foreground = new SolidColorBrush(Colors.White);   //reset colors back to normal after user tries fresh
            streak.Foreground = new SolidColorBrush(Colors.White);
            Help.Background = new SolidColorBrush(Colors.DarkSeaGreen);
            m_box.Foreground = new SolidColorBrush(Colors.Black);
            n_box.Foreground = new SolidColorBrush(Colors.Black);
            msg.Text = "";                                              //reset error box
        }

        private void reset_name1(object sender, RoutedEventArgs e)
        {
            player1_name.Text = "";
            player1_name.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void reset_name2(object sender, RoutedEventArgs e)
        {
            player2_name.Text = "";
            player2_name.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void help_click(object sender, RoutedEventArgs e)
        {
            Help.Background = new SolidColorBrush(Colors.DarkSeaGreen);
            Frame.Navigate(typeof(HelpPage));
        }

        private void about_click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }

        private void Main_SizeChanged1(object sender, SizeChangedEventArgs e)
        {
            switch (ApplicationView.Value)
            {
                case ApplicationViewState.Filled :
                    VisualStateManager.GoToState(this, "Filled", false);
                    break;

                case ApplicationViewState.FullScreenLandscape:
                    VisualStateManager.GoToState(this, "FullScreenLandscape", false);
                    break;

                case ApplicationViewState.FullScreenPortrait:
                    VisualStateManager.GoToState(this, "FullScreenPortrait", false);
                    break;

                case ApplicationViewState.Snapped:
                    VisualStateManager.GoToState(this, "Snapped", false);
                    break;

                default:
                    break;

            }
        }

    }
}
