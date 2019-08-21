using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace emgucv_example
{
    public class LineList
    {
        private System.Drawing.Point _StartPoint;
        private System.Drawing.Point _EndPoint;

        public LineList(Point StartPoint,Point EndPoint)
        {
            this._StartPoint = StartPoint;
            this._EndPoint = EndPoint;
        }

        public Point StartPoint
        {
            get { return _StartPoint; }
            set { _StartPoint = value; }
        }

        public Point EndPoint
        {
            get { return _EndPoint; }
            set { _EndPoint = value; }
        }
    }
}
