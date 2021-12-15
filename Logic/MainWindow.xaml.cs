using System;
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

namespace Logic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int zadanie = 0;
        public MainWindow()
        {
            InitializeComponent();
            bt.IsEnabled = false;
            MainTextBox.IsEnabled = false;
            VtTextBox.IsEnabled = false;
            Label1.IsEnabled = false;
            Label2.IsEnabled = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (zadanie)
            {
                case 0:
                    ParsingTree pt = new ParsingTree(MainTextBox.Text);
                    Knot k = pt.Parsing();
                    tb.Text = "";
                    if (k != null)
                    {
                        //Printing(k);
                        tb.Text = "Формула!";
                    }
                    else tb.Text = "НЕ формула!";
                    break;
                case 1:
                    ParsingTree pt1 = new ParsingTree(MainTextBox.Text);
                    Knot k1 = pt1.Parsing();
                    tb.Text = "";
                    if (k1 != null)
                    {
                        //Printing(k);
                        //tb.Text = "Формула!";
                    }
                    else
                    {
                        tb.Text = "Выражение 1 НЕ формула!";
                        break;
                    }
                    ParsingTree pt2 = new ParsingTree(VtTextBox.Text);
                    Knot k2 = pt2.Parsing();
                    tb.Text = "";
                    if (k2 != null)
                    {
                        //Printing(k);
                        //tb.Text = "Формула!";
                    }
                    else
                    {
                        tb.Text = "Выражение 2 НЕ формула!";
                        break;
                    }

                    bool isEkv = true;

                    if (pt1.PeremenList.Count == pt2.PeremenList.Count)
                    {
                        int count = pt1.PeremenList.Count;
                        bool flag = false;
                        for (int i = 0; i < count; i++)
                        {
                            if (pt1.PeremenList.IndexOf(pt2.PeremenList[i]) == -1)
                            {
                                int prov = Proverka(pt1, k1);
                                if (prov == Proverka(pt2, k2) && prov != -1)
                                {
                                    tb.Text = "Эквивалентны!";
                                }
                                else
                                {
                                    tb.Text = "НЕ эквивалентны!";
                                }
                                
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                        {

                            break;
                        }
                        int kol = (int)Math.Pow(2.0, count);
                        bool[] mas = new bool[count];
                        for (int i = 0; i < count; i++) mas[i] = false;
                        for (int i = 0; i < kol; i++)
                        {
                            Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
                            //Dictionary<string, bool> dictionary2 = new Dictionary<string, bool>();
                            for (int j = 0; j < count; j++)
                            {
                                dictionary.Add(pt1.PeremenList[j], mas[j]);
                                //dictionary.Add(pt2.PeremenList[j], mas[j]);
                            }

                            if (pt1.Сalculation(dictionary, k1) != pt2.Сalculation(dictionary, k2))
                            {
                                isEkv = false;
                                break;
                            }

                            /////////
                            bool one = false;
                            for (int j = count - 1; j >= 0; j--)
                            {
                                if (j == count - 1)
                                {
                                    if (mas[j])
                                    {
                                        mas[j] = false;
                                        one = true;
                                    }
                                    else mas[j] = true;
                                }
                                else
                                {
                                    if (one)
                                    {
                                        if (mas[j]) mas[j] = false;
                                        else
                                        {
                                            mas[j] = true;
                                            one = false;
                                            break;
                                        }
                                    }
                                }
                            }
                            /////////
                        }
                    }
                    else
                    {
                        tb.Text = "НЕ эквивалентны!";
                        break;
                    }
                    if (isEkv) tb.Text = "Эквивалентны!";
                    else tb.Text = "НЕ эквивалентны!";
                    break;
                case 2:
                    ParsingTree pt3 = new ParsingTree(MainTextBox.Text);
                    Knot k3 = pt3.Parsing();
                    tb.Text = "";
                    if (k3 == null)
                    {
                        tb.Text = "Выражение 1 НЕ формула!";
                        break;
                    }
                    //
                    tb.Text = pt3.DNF(k3);
                    VtTextBox.Text = tb.Text;

                    break;
                case 3:
                    ParsingTree pt4 = new ParsingTree(MainTextBox.Text);
                    Knot k4 = pt4.Parsing();
                    tb.Text = "";
                    if (k4 == null)
                    {
                        tb.Text = "Выражение 1 НЕ формула!";
                        break;
                    }
                    //
                    tb.Text = pt4.KNF(k4);
                    VtTextBox.Text = tb.Text;

                    break;
            }
            
        }

        private int Proverka(ParsingTree pt, Knot k)
        {
            int b = -1;
            int count = pt.PeremenList.Count;
            int kol = (int)Math.Pow(2.0, count);
            bool[] mas = new bool[count];
            for (int i = 0; i < count; i++) mas[i] = false;
            for (int i = 0; i < kol; i++)
            {
                Dictionary<string, bool> dictionary = new Dictionary<string, bool>();                
                for (int j = 0; j < count; j++)
                {
                    dictionary.Add(pt.PeremenList[j], mas[j]);
                    //dictionary.Add(pt2.PeremenList[j], mas[j]);
                }

                if (pt.Сalculation(dictionary, k))
                {
                    if (b == -1) b = 1;
                    else if (b == 0) return -1;
                }
                else
                {
                    if (b == -1) b = 0;
                    else if (b == 1) return -1;
                }

                /////////
                bool one = false;
                for (int j = count - 1; j >= 0; j--)
                {
                    if (j == count - 1)
                    {
                        if (mas[j])
                        {
                            mas[j] = false;
                            one = true;
                        }
                        else mas[j] = true;
                    }
                    else
                    {
                        if (one)
                        {
                            if (mas[j]) mas[j] = false;
                            else
                            {
                                mas[j] = true;
                                one = false;
                                break;
                            }
                        }
                    }
                }
                /////////
            }

            return b;
        }
        private void Printing(Knot k)
        {
            tb.Text += k.Name + "\n";
            if (k.LeftKnot != null) Printing(k.LeftKnot);
            if (k.RightKnot != null) Printing(k.RightKnot);
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            zadanie = 0;
            bt.IsEnabled = true;
            MainTextBox.IsEnabled = true;
            VtTextBox.IsEnabled = false;
            Label1.IsEnabled = true;
            Label2.IsEnabled = false;
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            zadanie = 1;
            bt.IsEnabled = true;
            MainTextBox.IsEnabled = true;
            VtTextBox.IsEnabled = true;
            Label1.IsEnabled = true;
            Label2.IsEnabled = true;
        }

        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            zadanie = 2;
            bt.IsEnabled = true;
            MainTextBox.IsEnabled = true;
            VtTextBox.IsEnabled = false;
            Label1.IsEnabled = true;
            Label2.IsEnabled = false;
        }

        private void RadioButton_Checked_3(object sender, RoutedEventArgs e)
        {
            zadanie = 3;
            bt.IsEnabled = true;
            MainTextBox.IsEnabled = true;
            VtTextBox.IsEnabled = false;
            Label1.IsEnabled = true;
            Label2.IsEnabled = false;
        }
    }
}
