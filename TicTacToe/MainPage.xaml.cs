using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TicTacToe
{
    public sealed partial class MainPage : Page
    {
        char[] symbol = { 'X', 'O' };                   //toggling symbol on every move
        char[,] value;                                  //hold values of n*n matrix as entered

        String pName1 = "";
        String pName2 = "";                             //hold player names
        
        bool[,] status;                                 //if button is slready clicked, mark on/true 
        int count, m, n, s_X, s_O;                      //count : no of hits, m: grid size, n:streak size, s_X, s_O:scores 
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Main_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            switch (ApplicationView.Value)
            {
                case ApplicationViewState.Filled:
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            String[] ar = e.Parameter as String[];                  //receiving values from StartPage
            m = Convert.ToInt16(ar[0]);
            n = Convert.ToInt16(ar[1]);
            count = 0;

            pName1 = ar[2];
            pName2 = ar[3];

            show_X.Text = pName1 + " (X) : ";
            show_O.Text = pName2 + " (O) : ";

            status = new bool[m, m];
            value = new char[m, m];
            
            for (int i = 0; i < m; i++)                             //initializing status & value matrix
                for (int j = 0; j < m; j++)
                {
                    value[i, j] = ' ';
                    status[i, j] = false ;
                }            
        }

        void check(object sender)                                   //the parent of all check functions, all functions called here
        {
            int win_X = 0, win_O = 0;                               //total count of wins of each symbol

            var button = sender as Button;
            Button[] childrenChild = new Button[49];                            //forming button 2D array to get controls

            Grid childrenMain = VisualTreeHelper.GetParent(button) as Grid;     //getting the parent grid of the button clicked
            for (int i = 0; i < 49; i++)
                childrenChild[i] = childrenMain.Children[i] as Button;          //extracting button controls from parent grid

            win_O += checkRowCol('O', childrenChild);                           //row&column check
            win_X += checkRowCol('X', childrenChild);

            win_O += checkLeftDiagonal('O', childrenChild);                     //left diagonal check
            win_X += checkLeftDiagonal('X', childrenChild);

            win_O += checkRightDiagonal('O', childrenChild);                    //right diagonal check
            win_X += checkRightDiagonal('X', childrenChild);

            s_O = win_O;
            s_X = win_X;
            score_X.Text = "" + win_X;                                          //showing present score
            score_O.Text = "" + win_O;

        }

        void toggleChance()
        {
            if (count % 2 == 0)                                         //higlighting the symbol having chance
            {
                show_O.Foreground = new SolidColorBrush(Colors.Gray);
                show_X.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                show_X.Foreground = new SolidColorBrush(Colors.Gray);
                show_O.Foreground = new SolidColorBrush(Colors.Red);
            }
        }
        

        private void start_click(object sender, RoutedEventArgs e)
        {
            start.IsEnabled = false;
            start.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            var button = sender as Button;
            Button[] childrenChild = new Button[49];                            //forming button 2D array to get controls

            Grid childrenMain = VisualTreeHelper.GetParent(button) as Grid;     //getting the parent grid of the button clicked
            for (int i = 0; i < 49; i++)
                childrenChild[i] = childrenMain.Children[i] as Button;          //extracting button controls from parent grid

            for (int i = 0; i < m; i++)                                         //enabling buttons in sub-grid of size m*m
                for (int j = 0; j < m; j++)
                    for (int k = 0; k < 49; k++)
                        if (childrenChild[k].Name == "b" + i + j || childrenChild[k].Name == "b" + j + i)
                        {
                            childrenChild[k].IsEnabled = true;
                            childrenChild[k].Visibility = Windows.UI.Xaml.Visibility.Visible;
                        }

            toggleChance();
        }

        void endOrNot()                                 //function to check end of game
        {
            if (count == m * m)
            {
                string w = "";
                if (s_X > s_O)
                {
                    show_X.Foreground = score_X.Foreground = new SolidColorBrush(Colors.BlueViolet);
                    w = pName1;
                }
                else if (s_X < s_O)
                {
                    show_O.Foreground = score_O.Foreground = new SolidColorBrush(Colors.BlueViolet);
                    w = pName2;
                }
                end.Text = "END||" + w + " WINS";
                if (s_X == s_O)
                {
                    extend.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    extend.IsEnabled = true;
                    end.Text = "TIE";
                }
            }
        }

        bool seqCheck(char[] c)                         //check for a sequence of n to be completed
        {
            if (c[0] != 'X' && c[0] != 'O')
                return false;

            for (int i = 1; i < n; i++)
                if (c[i] != c[0])
                    return false;

            return true;
        }

        void changeColor(char move, Button[] childrenChild, String[] ckName)    //change color on completion of the streak
        {
            for (int h = 0; h < n; h++)
                for (int u = 0; u < 49; u++)
                    if (childrenChild[u].Name == ckName[h])
                    {
                        if (move == 'X')
                            childrenChild[u].Background = new SolidColorBrush(Colors.DarkCyan);  // blue in case of X

                        else
                            childrenChild[u].Background = new SolidColorBrush(Colors.DarkSeaGreen); //green in case of O                        
                    }

        }

        private void b_click(object sender, RoutedEventArgs e)              //funcion to be invoked on each button click
        {
            String name="";
            var button = sender as Button;                                  //getting present button
            if(button !=null)
                name= button.Name;          

            int x = Convert.ToInt16(name.Substring(1,1));                   //getting X and Y co-ordinates from the names.
            int y = Convert.ToInt16(name.Substring(2,1));
            if(!status[x,y])
            {
                button.Content = value[x, y] = symbol[count%2];             //setting X or O
                check(sender);                                              //checking for a streak
                count++;
                status[x, y] = true;
                toggleChance();
            }
            endOrNot();                                                     //checking for end of game
        }

        int checkRowCol(char move, Button[] childrenChild)                      //check for Row&Column
        {
            char[] ck = new char[n];
            int lim = m - n + 1, countR = 0, countC = 0, flagRow = 0, flagCol = 0, y;         /* lim=size of inner grid
                                                                                         * countR, countC = count streak of row/col
                                                                                         * flarRow, flogCol = 1 if streak is done
                                                                                         * y = counter to increment ckName array
                                                                                         */

            String[] ckNameR = new String[n], ckNameC = new String[n];          //ckName = array to hold streak, R:row, C:Col
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < lim; j++)
                {
                    y = 0;
                    for (int temp = j; temp < n + j; temp++)
                    {
                        //Row check
                        ckNameR[y] = "b" + Convert.ToString(i) + Convert.ToString(temp);    // form button name for checkinh
                        if (value[i, temp] != move)
                            flagRow = 1;                                //if flagRow always 0, means streak formed in row

                        //Column check
                        ckNameC[y] = "b" + Convert.ToString(temp) + Convert.ToString(i);    // form button name for checking
                        if (value[temp, i] != move)
                            flagCol = 1;                                //if flagCol always 0, means streak formed in col

                        //optimization
                        if (flagRow == 1 && flagCol == 1)               //if streak isnt formed in either case, then break out
                            break;

                        y++;
                    }
                    if (flagRow == 0)                                   //row streak formed
                    {
                        changeColor(move, childrenChild, ckNameR);      //change color of the buttons
                        countR++;
                    }
                    else
                        flagRow = 0;                                    //reset flag value to 0 for next iteration

                    if (flagCol == 0)
                    {
                        changeColor(move, childrenChild, ckNameC);      //change color of the buttons
                        countC++;                                       //column streak formed
                    }
                    else
                        flagCol = 0;                                    //reset flag value to 0 for next iteration

                }
            }
            return (countC + countR);                                     //return total count for a single symbol 'X' & 'O'

        }

        int checkLeftDiagonal(char move, Button[] childrenChild)    //function to check left diagonal
        {
            int lim = m - n + 1, countLD = 0;                         //lim = inner grid size,countLD = count of streak in left diag
            char[] ck = new char[n];                                //array to store streak of size n
            String[] ckName = new String[n];                        //array to store names of buttons
            for (int i = 0; i < lim; i++)
                for (int j = 0; j < lim; j++)
                {
                    for (int k = 0; k < n; k++)
                    {
                        ck[k] = value[i + k, j + k];
                        ckName[k] = "b" + (i + k) + (j + k);        //forming names of buttons
                    }

                    if (seqCheck(ck))
                        if (ck[0] == move)
                        {
                            changeColor(move, childrenChild, ckName);
                            countLD++;
                        }

                }

            return countLD;
        }

        int checkRightDiagonal(char move, Button[] childrenChild)
        {
            int lim = m - n + 1, countRD = 0, p, q;                 //lim = inner grid size,countRD = count of streak in right diag
            char[] ck = new char[n];                                //array to store streak of size n
            String[] ckName = new String[n];                        //array to store names of buttons

            p = 0;
            for (int i = 0; i < lim; i++)
            {
                q = m - 1;
                for (int j = 0; j < lim; j++)
                {
                    for (int k = 0; k < n; k++)
                    {
                        ckName[k] = "b" + (p + k) + (q - k);        //forming names of buttons
                        ck[k] = value[p + k, q - k];
                    }

                    if (seqCheck(ck))
                        if (ck[0] == move)
                        {
                            changeColor(move, childrenChild, ckName);
                            countRD++;
                        }

                    q--;
                }
                p++;
            }
            return countRD;
        }
        

        private void new_click(object sender, RoutedEventArgs e)            //start a new game from StartPage
        {
            Frame.Navigate(typeof(StartPage));
        }

        private void extendGame(object sender, RoutedEventArgs e)
        {
            extend.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            extend.IsEnabled = false;
            if (m < 7)
            {
                end.Text = "";
                int decide = new Random().Next(0, 2), i;
                var button = sender as Button;
                Button[] childrenChild = new Button[49];                            //forming button 2D array to get controls

                Grid childrenMain = VisualTreeHelper.GetParent(button) as Grid;     //getting the parent grid of the button clicked
                for (i = 0; i < 49; i++)
                    childrenChild[i] = childrenMain.Children[i] as Button;          //extracting button controls from parent grid


                char[,] value1 = new char[m + 1, m + 1];                        //creating new extended value & status matrix
                bool[,] status1 = new bool[m + 1, m + 1];

                if (decide == 1)
                {
                    for (i = 0; i < m; i++)                                     //copying old values in exact places
                        for (int j = 0; j < m; j++)
                        {
                            value1[i, j] = value[i, j];
                            status1[i, j] = status[i, j];
                        }

                    i = m;
                    for (int j = 0; j < m + 1; j++)
                    {
                        for (int k = 0; k < 49; k++)                            //enabling new set of buttons
                            if (childrenChild[k].Name == "b" + i + j || childrenChild[k].Name == "b" + j + i)
                            {
                                childrenChild[k].IsEnabled = true;
                                childrenChild[k].Visibility = Windows.UI.Xaml.Visibility.Visible;
                            }

                        value1[i, j] = ' ';
                        status1[i, j] = false;
                    }
                }
                else
                {                                                               //extending on left-top corner
                    i = m;
                    for (int j = 0; j < m + 1; j++)
                        for (int k = 0; k < 49; k++)                            //enabling new set of buttons
                            if (childrenChild[k].Name == "b" + i + j || childrenChild[k].Name == "b" + j + i)
                            {
                                childrenChild[k].IsEnabled = true;
                                childrenChild[k].Visibility = Windows.UI.Xaml.Visibility.Visible;
                            }

                    for (i = 1; i < m + 1; i++)                               //copying old values in exact places
                        for (int j = 1; j < m + 1; j++)
                        {
                            value1[i, j] = value[i - 1, j - 1];
                            status1[i, j] = status[i - 1, j - 1];
                            for (int k = 0; k < 49; k++)                        //painting button back to black
                                if (childrenChild[k].Name == "b" + i + j)
                                {
                                    childrenChild[k].Background = new SolidColorBrush(Colors.Black);
                                    childrenChild[k].Content = value[i - 1, j - 1];
                                }
                        }

                    i = 0;
                    for (int j = 0; j < m + 1; j++)
                    {
                        for (int k = 0; k < 49; k++)                        //painting button back to black
                            if (childrenChild[k].Name == "b" + i + j || childrenChild[k].Name == "b" + j + i)
                            {
                                childrenChild[k].Background = new SolidColorBrush(Colors.Black);
                                childrenChild[k].Content = "";
                            }
                        value1[i, j] = ' ';
                        status1[i, j] = false;
                    }

                }
                m += 1; value = value1;
                status = status1;

                check(sender);                                                  //repainting buttons acc to streaks
            }
            else
            {
                show_X.Foreground = score_X.Foreground = new SolidColorBrush(Colors.BlueViolet);
                show_O.Foreground = score_O.Foreground = new SolidColorBrush(Colors.BlueViolet);
                end.Text = "No More";
            }
        }

    }
}
