using System.Windows;

namespace MAP_Game.View
{
    public partial class GridSizeInputWindow : Window
    {
        public int Rows { get; private set; } = 4;
        public int Columns { get; private set; } = 4;

        public GridSizeInputWindow()
        {
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(RowsTextBox.Text, out int rows) &&
                int.TryParse(ColumnsTextBox.Text, out int cols))
            {
                if (rows >= 2 && rows <= 6 && cols >= 2 && cols <= 6 &&
                    (rows % 2 == 0 || cols % 2 == 0)) 
                {
                    Rows = rows;
                    Columns = cols;
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Both numbers must be between 2 and 6, and at least one must be even.");
                }
            }
            else
            {
                MessageBox.Show("Please enter valid numbers.");
            }
        }
    }
}
