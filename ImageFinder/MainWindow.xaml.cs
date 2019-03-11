using ImageFinder.Domain;
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

namespace ImageFinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BodyTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchTermsArea.Children.Clear();

            string[] words = BodyTxt.Text.Split(null);

            foreach(var word in words)
            {
                TextBlock txtBlock = new TextBlock();
                txtBlock.Text = word;
                txtBlock.Margin = new Thickness { Left = 0, Top = 0, Right = 10, Bottom = 10 };
                txtBlock.FontSize = 16;
                txtBlock.Background = new SolidColorBrush(Color.FromRgb(143, 21, 214));
                txtBlock.Foreground = new SolidColorBrush(Color.FromRgb(245, 245, 245));
                txtBlock.MouseLeftButtonDown += OnWordClick;
                txtBlock.MouseRightButtonDown += OnWordDeselect;

                SearchTermsArea.Children.Add(txtBlock);
            }
        }

        private void OnWordClick(object sender, RoutedEventArgs e)
        {
            var txtBlock = (TextBlock)sender;
            txtBlock.FontWeight = FontWeights.Bold;

            txtBlock.Focusable = true;
            txtBlock.Focus();

            if(!ImageRepository.EnabledBodySearchTerms.Contains(txtBlock.Text) && !ImageRepository.TitleSearchTerms.Contains(txtBlock.Text))
                ImageRepository.EnabledBodySearchTerms.Add(txtBlock.Text);

            LoadImageResults();
        }

        private void OnWordDeselect(object sender, RoutedEventArgs e)
        {
            var txtBlock = (TextBlock)sender;
            txtBlock.FontWeight = FontWeights.Normal;
            ImageRepository.EnabledBodySearchTerms.Remove(txtBlock.Text);

            txtBlock.Focus();

            LoadImageResults();
        }

        private void LoadImageResults()
        {
            var combined = ImageRepository.TitleSearchTerms.Concat(ImageRepository.EnabledBodySearchTerms);
            var parsedSearchTerms = HelperMethods.ParsedSearchTerms(combined.ToList());
            var jsonString = HelperMethods.SearchForImages(parsedSearchTerms);
            var images = HelperMethods.GetImagesFromJson(jsonString);

            ImageCombobox.Items.Clear();

            foreach(var image in images)
            {
                ImageCombobox.Items.Add(image);
                ImageCombobox.DisplayMemberPath = "Tags";
            }
        }

        private void ImageCombobox_Selected(object sender, RoutedEventArgs e)
        {
            if (ImageCombobox.SelectedItem is null)
            {
                InstructionTxt.Visibility = Visibility.Visible;
                DisplayImg.Source = null;
                return;
            }
                

            InstructionTxt.Visibility = Visibility.Hidden;

            var image = ImageCombobox.SelectedItem as WebImage;

            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(image.WebFormatURL, UriKind.Absolute);
            bmp.EndInit();

            DisplayImg.Source = bmp;
        }

        private void TitleTxt_LostFocus(object sender, RoutedEventArgs e)
        {
            string[] words = TitleTxt.Text.Split(null);

            ImageRepository.TitleSearchTerms.Clear();

            foreach (var word in words)
            {
                if (!ImageRepository.TitleSearchTerms.Contains(word) && !ImageRepository.EnabledBodySearchTerms.Contains(word))
                    ImageRepository.TitleSearchTerms.Add(word);
            }

            LoadImageResults();
        }

        private void BodyTxt_LostFocus(object sender, RoutedEventArgs e)
        {
            ImageRepository.EnabledBodySearchTerms.Clear();
            LoadImageResults();
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(TitleTxt.Text) || string.IsNullOrEmpty(BodyTxt.Text) || DisplayImg.Source is null)
            {
                MessageBox.Show("Must enter a title, body, and select an image!", "Error", MessageBoxButton.OK);
                return;
            }

            HelperMethods.ExportSlide(TitleTxt.Text, BodyTxt.Text, (BitmapImage)DisplayImg.Source);
        }
    }
}
