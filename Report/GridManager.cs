using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Markup;

namespace Testing.Report {

    public class GridManager
    {

        public Grid Grid { get; set; }

        public GridManager(Grid Grid) => this.Grid = Grid;

        public void Clear() {
            Grid.Children.Clear();
            Grid.RowDefinitions.Clear();
        }

        public int RowCount() { 
            return Grid.RowDefinitions.Count;
        } 

        public void MoveRowTo(int oldRow, int newRow, bool also = false)
        {
            var data = GetRow(oldRow);
            RemoveRow(data.Lista);
            InsertRow(data, newRow);
            if (also) RemoveRowDefinition(oldRow);
        }
        
        
        public void MoveRowTo(int RowToBeMoved, int RowMovedAt, Grid grid, bool RemoveRowDefinitions = false)
        {
            var data = GetRow(RowToBeMoved);
            RemoveRow(data.Lista);
            InsertRow(data, RowMovedAt, grid);
            if (RemoveRowDefinitions) RemoveRowDefinition(RowToBeMoved);
        }

        public void RemoveRowDefinition(int index) {
            Grid.RowDefinitions.Remove(Grid.RowDefinitions[index]);
        }


        public Data GetRow(int row) {
            Data Data = new();
            Data.Lista = Grid.Children.Cast<UIElement>().Where(i => Grid.GetRow(i) == row).ToList();
            Data.Row = Grid.RowDefinitions[row];
            return Data;
        }

        public void RemoveRow(List<UIElement> lista)
        {

            foreach (var el in lista)
            {
                Grid.Children.Remove(el);
            }

        }

        public void InsertRow(Data data, int rowIndex)
        {
            int columns = 0;


            if (data.Row.Height.IsStar)
            {
                Grid.RowDefinitions[rowIndex].Height = new GridLength(data.Row.Height.Value, GridUnitType.Star);
            }
            if (data.Row.Height.IsAuto)
            {
                Grid.RowDefinitions[rowIndex].Height = new GridLength(data.Row.Height.Value, GridUnitType.Auto);
            }
            if (data.Row.Height.IsAbsolute)
            {
                Grid.RowDefinitions[rowIndex].Height = new GridLength(data.Row.Height.Value);
            }

            foreach (var element in data.Lista)
            {
                Grid.SetRow(element, rowIndex);
                Grid.SetColumn(element, columns);
                Grid.Children.Add(element);
                columns++;
            }
        }


        public void InsertRow(Data data, int rowIndex, Grid grid)
        {
            int columns = 0;

            GridLength GridLength = new GridLength();

            if (data.Row.Height.IsStar)
            {
                GridLength = new GridLength(data.Row.Height.Value, GridUnitType.Star);
            }
            if (data.Row.Height.IsAuto)
            {
                GridLength = new GridLength(data.Row.Height.Value, GridUnitType.Auto);
            }
            if (data.Row.Height.IsAbsolute)
            {
                GridLength = new GridLength(data.Row.Height.Value);
            }

            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength });

            foreach (var element in data.Lista)
            {
                Grid.SetRow(element, rowIndex);
                Grid.SetColumn(element, columns);
                grid.Children.Add(element);
                columns++;
            }
            
        }

        public void SortRows() {
            int row = 0;
            List<Data> Lista = new();
            foreach (var x in Grid.RowDefinitions) {
               Lista.Add(GetRow(row));
                row++;
            }

            row = Lista.Count()-1;
            foreach (var x in Lista) {
                foreach (var element in x.Lista)
                {
                    Grid.SetRow(element, row);
                }
                row--;
            }
        }
    }

    public class Data
    {
        public List<UIElement> Lista { get; set; }
        public RowDefinition Row { get; set; }
    }
}
