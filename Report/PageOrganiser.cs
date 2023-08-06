using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Testing.Customs;

namespace Testing.Report {

    public class PageOrganiser : List<ReportPage>
    {
        private FixedDocument Doc;

        public PageOrganiser(FixedDocument Doc)=>this.Doc = Doc;

        public void AddPages(params ReportPage[] pages) {
            foreach (var page in pages)
            {
                page.Paper.GrowEvent += Element_GrowEvent;
                Doc.Pages.Add(page.GetPage());
            }
            AddRange(pages);
        }

        private void Element_GrowEvent(object? sender, MyArgs e) {
            e.UpdateLayouts();
            int row = 0;

            GridManager newGrid = null;
            PaperBase newPaper = e.NewPaper;
            
            if (e.BodyChild is Grid) {
                newGrid = new((Grid)newPaper.BodyChild);
                newGrid.Clear();
            }

            while (e.HasContentToMove) {
                if (e.BodyChildIsStackPanel)
                {
//                    var last = bodyPanel.Children[bodyPanel.Children.Count - 1];
 //                   bodyPanel.Children.Remove(last);
                    Panel newbody = new StackPanel();
 //                   newbody.Children.Add(last);
                    newPaper.BodyChild = newbody;
                }

                if (e.BodyChildIsGrid)
                {                    
                    GridManager oldgrid = new(e.GetBodyChildAsGrid);                    
                    oldgrid.MoveRowTo(oldgrid.RowCount() - 1, row, newGrid.Grid, true);
                }
                row++;
                e.UpdateLayouts();
            }

            newGrid.SortRows();
            ReportPage pg = new(newPaper, (e.PageNumber + 1));
            newPaper.GrowEvent += Element_GrowEvent;
            Doc.Pages.Add(pg.GetPage());
        }
    }
}