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
            // Get the selected category from the ComboBox
            SelectedCategory = (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrWhiteSpace(SelectedCategory))
            {
                MessageBox.Show("Please select a category.");
                return;
            }

            this.Close(); // Close the category selection window
        }
    }
}
