using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MAP_Game.Helpers;


namespace MAP_Game.ViewModel
{
    public class GameViewModel : INotifyPropertyChanged
    {
        private static readonly Dictionary<string, string> CategoryPaths = new()
        {
            { "Category 1", @"C:\Users\Plesu\Desktop\WPF_Game\MAP_Game\Categories\Category_1" },
            { "Category 2", @"C:\Users\Plesu\Desktop\WPF_Game\MAP_Game\Categories\Category_2" },
            { "Category 3", @"C:\Users\Plesu\Desktop\WPF_Game\MAP_Game\Categories\Category_3" }
        };
       

        public GameViewModel(string selectedCategory)
        {
            LoadCards(selectedCategory);
        }

        private void LoadCards(string selectedCategory)
        {
            var imagePaths = LoadImagesFromCategory(selectedCategory);
        }


        private List<string> LoadImagesFromCategory(string category)
        {
            var categoryPath = CategoryPaths[category]; 
            return System.IO.Directory.GetFiles(categoryPath, "*.png").ToList();
        }

        
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
