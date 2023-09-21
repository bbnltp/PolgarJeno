using SimCity.Model.Table.Field;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SimCity.ViewModel
{
    internal class MenuField : ViewModelBase
    {
        private string text;
        private string name;
        private string color;
        private string image;
		private TableFieldType fieldType;
		private int size;
        private int x;
        private int y;
        private int index;
        private bool enabled;

        public string Text { get => text; set { text = value; OnPropertyChanged(); } }

		public TableFieldType FieldType { get => fieldType; set { fieldType = value; OnPropertyChanged(); } }

		public string Color { get => color; set { color = value; OnPropertyChanged(); } }
		public string Image { get => image; set { image = value; OnPropertyChanged(); } }

        public string Name { get => name; set { name = value; OnPropertyChanged(); } }

        public bool Enabled { get => enabled; set { enabled = value; OnPropertyChanged(); } }

        public int Size { get => size; set { size = value; OnPropertyChanged(); } }

        public int X { get => x; set { x = value; OnPropertyChanged(); } }

        public int Y { get => y; set { y = value; OnPropertyChanged(); } }

        public int Index { get; set; }

        public DelegateCommand? PushCommand { get; set; }
    }
}
