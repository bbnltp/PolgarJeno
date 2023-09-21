using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimCity.Model.Table.Field;

namespace SimCity.ViewModel
{
    internal class FieldModel : ViewModelBase
    {
        private string colour;
        private string text;
        private string image;
        private TableFieldType fieldType;
        private float width;
        private float height;
        private int x;
        private int y;
        private int index;
        private bool choosen;
        public string Colour { get => colour; set { colour = value; OnPropertyChanged(); } }

        public TableFieldType FieldType { get => fieldType; set { fieldType = value; OnPropertyChanged(); } }

        public string Image { get => image; set { image = value; OnPropertyChanged(); } }

		public string Text { get => text; set { text = value; OnPropertyChanged(); } }

        public int X { get => x; set { x = value; OnPropertyChanged(); } }

        public int Y { get => y; set { y = value; OnPropertyChanged(); } }

        public float Width { get => width; set { width = value; OnPropertyChanged(); } }
        public float Height { get => height; set { height = value; OnPropertyChanged(); } }

        public bool Choosen { get => choosen; set { choosen = value; OnPropertyChanged(); } }

        public int Index { get; set; }

        public DelegateCommand? StepCommand { get; set; }

        public DelegateCommand? Inspect { get; set; }

    }
}
