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
                txtBlock.Background = new SolidColorBrush(Color.FromRgb(238, 238, 238));
                txtBlock.TextWrapping = TextWrapping.Wrap;
                txtBlock.HorizontalAlignment = HorizontalAlignment.Left;
                txtBlock.MouseLeftButtonDown += OnWordClick;
                txtBlock.MouseRightButtonDown += OnWordDeselect;

                SearchTermsArea.Children.Add(txtBlock);
            }
        }

        private void OnWordClick(object sender, RoutedEventArgs e)
        {
            var txtBlock = (TextBlock)sender;
            txtBlock.FontWeight = FontWeights.Bold;

            if(!ImageRepository.EnabledSearchTerms.Contains(txtBlock.Text))
                ImageRepository.EnabledSearchTerms.Add(txtBlock.Text);
        }

        private void OnWordDeselect(object sender, RoutedEventArgs e)
        {
            var txtBlock = (TextBlock)sender;
            txtBlock.FontWeight = FontWeights.Normal;
            ImageRepository.EnabledSearchTerms.Remove(txtBlock.Text);
        }

        private void TitleTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            string[] words = TitleTxt.Text.Split(null);

            foreach(var word in words)
            {
                if (!ImageRepository.EnabledSearchTerms.Contains(word.Trim()))
                    ImageRepository.EnabledSearchTerms.Add(word.Trim());
            }
        }

        private void LoadImageResults()
        {
            var parsedSearchTerms = HelperMethods.ParsedSearchTerms(ImageRepository.EnabledSearchTerms);
            var jsonString = HelperMethods.SearchForImages(parsedSearchTerms);
            var images = HelperMethods.GetImagesFromJson(jsonString);

            ImageCombobox.Items.Clear();

            foreach(var image in images)
            {
                ImageCombobox.Items.Add(image);
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
            bmp.UriSource = new Uri(image.URL, UriKind.Absolute);
            bmp.EndInit();

            DisplayImg.Source = bmp;
        }

        private void ImageCombobox_GotFocus(object sender, RoutedEventArgs e)
        {
            LoadImageResults();
        }
    }
}
