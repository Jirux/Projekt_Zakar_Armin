using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace projekt_zakar_armin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            taskBox.ItemsSource = feladatok;
            deletedTaskBox.ItemsSource = toroltek;

            if (File.Exists("feladatok.txt"))
            {
                string[] lines = File.ReadAllLines("feladatok.txt");
                foreach (string line in lines)
                {
                    string[] parts = line.Split(new char[] { ':' }, 3);
                    if (parts.Length == 3)
                    {
                        bool deleted = parts[0] == "deleted";
                        bool done = parts[1] == "checked";
                        string text = parts[2];
                        CheckBox uj = new CheckBox();
                        uj.IsChecked = done;
                        uj.Content = text;
                        uj.Checked += new RoutedEventHandler(onCheckboxChecked);
                        uj.Unchecked += new RoutedEventHandler(onCheckboxUnchecked);
                        if (deleted)
                        {
                            feladatok.Add(uj);
                            taskBox.Items.Refresh();

                        } else
                        {
                            toroltek.Add(uj);
                            deletedTaskBox.Items.Refresh();
                        }
                    }
                }
            }
        }
        List<CheckBox> feladatok = new List<CheckBox>();
        List<CheckBox> toroltek = new List<CheckBox>();

        private void onClickCreate(object sender, RoutedEventArgs e)
        {
            if (textBox.Text != "")
            {
                CheckBox uj = new CheckBox();
                uj.Content = textBox.Text;
                uj.Checked += new RoutedEventHandler(onCheckboxChecked);
                uj.Unchecked += new RoutedEventHandler(onCheckboxUnchecked);
                feladatok.Add(uj);
                taskBox.Items.Refresh();
                textBox.Text = "";
            }
        }

        private void onCheckboxChecked(object sender, RoutedEventArgs e)
        {
            CheckBox aktualis = (CheckBox)sender;
            aktualis.FontStyle = FontStyles.Italic;
            aktualis.Foreground = Brushes.Gray;
        }

        private void onCheckboxUnchecked(object sender, RoutedEventArgs e)
        {
            CheckBox aktualis = (CheckBox)sender;
            aktualis.FontStyle = FontStyles.Normal;
            aktualis.Foreground = Brushes.Black;
        }

        private void onClickDelete(object sender, RoutedEventArgs e)
        {
            CheckBox kijelolt = (CheckBox)taskBox.SelectedItem;
            toroltek.Add(kijelolt);
            feladatok.Remove(kijelolt);
            taskBox.Items.Refresh();
            deletedTaskBox.Items.Refresh();
        }

        private void onClickRestore(object sender, RoutedEventArgs e)
        {
            CheckBox kijelolt = (CheckBox)deletedTaskBox.SelectedItem;
            feladatok.Add(kijelolt);
            toroltek.Remove(kijelolt);
            taskBox.Items.Refresh();
            deletedTaskBox.Items.Refresh();
        }

        private void onClickModify(object sender, RoutedEventArgs e)
        {
            if (taskBox.SelectedIndex != -1)
            {
                int i = taskBox.SelectedIndex;

                feladatok[i].Content = textBox.Text;
                taskBox.Items.Refresh();
            }
        }

        private void onClickPermaDelete(object sender, RoutedEventArgs e)
        {
            if (deletedTaskBox.SelectedIndex != -1)
            {
                int i = deletedTaskBox.SelectedIndex;

                toroltek.RemoveAt(i);
                deletedTaskBox.Items.Refresh();
            }
            deletedTaskBox.Items.Refresh();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            StreamWriter writer = new StreamWriter("feladatok.txt");
            foreach (CheckBox b in feladatok)
            {
                string done = (bool)b.IsChecked ? "checked" : "unchecked";
                writer.WriteLineAsync("undeleted:" + done + ":" + b.Content);
            }
            foreach (CheckBox b in toroltek)
            {
                string done = (bool)b.IsChecked ? "checked" : "unchecked";
                writer.WriteLineAsync("deleted:" + done + ":" + b.Content);
            }
            writer.Close();
        }
    }
}
