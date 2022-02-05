using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using RevitAPITrainingLibrary_6_3;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPITraining_6_3
{
    public class MainViewViewModel
    {

        private ExternalCommandData _commandData;
        public List<XYZ> Points { get; } = new List<XYZ>();
        public DelegateCommand SaveCommand { get; }
        public List<FamilySymbol> FamilyTypes { get; }
        public FamilySymbol SelectedFamily { get; set; }
        public int ElementNumber { get; set; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;

            Points = SelectionsUtils.GetPoints(_commandData, "Выберите 2 точки", ObjectSnapTypes.Endpoints, 2);

            FamilyTypes = FamilySymbolUtils.GetFamilySymbols(_commandData);

            SaveCommand = new DelegateCommand(OnSaveCommand);

        }
        private void OnSaveCommand()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            List<XYZ> PointEl = new List<XYZ>();
            if (ElementNumber == 1)
            {
                XYZ xYZ = new XYZ(((Points[1].X - Points[0].X) / 2), ((Points[1].Y - Points[0].Y) / 2), 0);
                PointEl.Add(xYZ);
            }
            else
            {
                double X = (Points[1].X - Points[0].X) / (ElementNumber - 1);
                double Y = (Points[1].Y - Points[0].Y) / (ElementNumber - 1);
                for (int i = 0; i < ElementNumber; i++)
                {
                    XYZ yYZ = new XYZ(Points[0].X+X*i, Points[0].Y+Y*i, 0);
                    PointEl.Add(yYZ);
                }
            }
            foreach (var iPoint in PointEl)
            {
                FamilyInstanceUtils.CreateFamilyInstance(_commandData, SelectedFamily, iPoint, doc.ActiveView.GenLevel);
            }
            RaiseCloseRequest();
        }
        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }

}