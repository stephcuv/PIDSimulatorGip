using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace PIDSimulatorGip.model
{
    class VisualizationClass
    {
        List <double> _dataPointsY = new List<double>(); 
        List <double> _dataPointsX = new List<double>();
        public List <double> DataPointsY
        {  
            get { return _dataPointsY; }
            set { _dataPointsY = value; }
        }
        public List<double> DataPointsX
        {
            get { return _dataPointsX; }
            set { _dataPointsX = value; }
        }
        double _canvasHeight;
        double _canvasWidth;

        public double CanvasHeight{ set { _canvasHeight = value; } }
        public double CanvasWidth {  set { _canvasWidth = value; } }
        double maxY = 100;
        double minY = 0;
        double scale;
        
        

        public void BerekeningGraph()
        {
            double spacing = _canvasHeight / 100;
            for(int i = 0 ; i < _dataPointsX.Count; i++)
            {
                double x = spacing * i;
                double y = _canvasHeight - (_dataPointsX[i] * scale);
                _dataPointsX.Add(x);
                _dataPointsX.Add(y);
                _dataPointsX.RemoveAt(0);
            }
            for (int i = 0; i < _dataPointsY.Count; i++)
            {
                double x = spacing * i;
                double y = _canvasHeight - (_dataPointsY[i] * scale);
                _dataPointsY.Add(x);
                _dataPointsY.Add(y);
                _dataPointsY.RemoveAt(0);
            }
        }
    }
}
