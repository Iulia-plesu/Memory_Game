using System.Windows;
using System.Windows.Controls;

namespace MAP_Game.View
{
    public partial class CategorySelectionWindow : Window
    {
        public string SelectedCategory { get; private set; }

        public CategorySelectionWindow()
        {
            InitializeComponent();
        }

        private void SelectCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = CategoryComboBox.SelectedItem as ComboBoxItem;
            SelectedCategory = selectedItem?.Content.ToString();

            if (string.IsNullOrWhiteSpace(SelectedCategory))
            {
                // Set default if nothing is selected
                SelectedCategory = "Rustic";
            }

            this.Close(); // Close the category selection window
        }

    }
}
