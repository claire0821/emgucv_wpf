using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Drawing;


namespace emgucv_example
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        Image<Bgr, Byte> image = new Image<Bgr, byte>(@"H:\github\emgucv_wpf\emgucv_example\emgucv_example\bin\Debug\img\1.jpg");
        Bgr bgr = new Bgr(System.Drawing.Color.Red);
        System.Drawing.Point[] Points = new System.Drawing.Point[100];
        static int count1 = 0;
        static double zoomflag = 1.0;

        public System.Drawing.Point rec_startpoint;
        public System.Drawing.Point rec_point;
        public System.Drawing.Point rec_endpoint;
        public MainWindow()
        {
            InitializeComponent();

            string path = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine(path);

            ibox.Image = image;
            ibox.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;
            ibox.HorizontalScrollBar.Visible = false;
            ibox.VerticalScrollBar.Visible = false;

            ibox.MouseWheel += new System.Windows.Forms.MouseEventHandler(OnMouseWheel);
            Console.WriteLine("image height = {0}", image.Height.ToString());
            Console.WriteLine("image width = {0}", image.Width.ToString());
            ibox.Height = image.Height;
            ibox.Width = image.Width;
        }
        
        private void OnMouseEnter(object sender, EventArgs e)
        {  //set this as the active control
            //Form f = ibox as Form;
            //if (f != null) f.ActiveControl = this;
        }

        private void OnMouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {  //handle the mouse whell scroll (for zooming)
            double scale = 1.0;
            if (e.Delta > 0)
            {
                scale = 2.0;
            }
            else if (e.Delta < 0)
            {
                scale = 0.5;
            }
            else
                return;

            Console.WriteLine("mousewheel");
        }


        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
        System.Drawing.Point[] pointline = new System.Drawing.Point[2];
        System.Drawing.Point startpoint = new System.Drawing.Point();
        System.Drawing.Point endpoint = new System.Drawing.Point();
        bool linestart = true;
        private void Ibox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Console.WriteLine("left");
                int x = e.Location.X;
                int y = e.Location.Y;
                Console.WriteLine("x={0},y={1}", x, y);
                if (drawline.IsChecked == true)
                {
                    if (linestart == true)
                    {
                        pointline[0] = e.Location;
                    }
                    else if (linestart == false)
                    {
                        pointline[1] = e.Location;
                        CvInvoke.Line(image, pointline[0], pointline[1], new MCvScalar(255, 0, 0));
                        ibox.Image.Save("H:/wpf_opencv/WpfApp2/new.jpg");
                        image = new Image<Bgr, byte>("H:/wpf_opencv/WpfApp2/new.jpg");
                        ibox.Image = image;
                    }
                    linestart = !linestart;
                }
                else if (drawrec.IsChecked == true)
                {
                    //startpoint = e.Location;
                    //CvInvoke.PutText(image, "rec", startpoint, FontFace.HersheySimplex, 0.5, new MCvScalar(255, 255, 0), 1);
                    //ibox.Image.Save("H:/wpf_opencv/WpfApp2/new.jpg");
                    //image = new Image<Bgr, byte>("H:/wpf_opencv/WpfApp2/new.jpg");
                    //ibox.Image = image;
                    startpoint = e.Location;
                    Console.WriteLine("startpoint.x = {0},startpoint.y = {1}", startpoint.X, startpoint.Y);
                }
                else if (drawpoly.IsChecked == true)
                {
                    Points[count1] = e.Location;
                    Points[count1].X = (int)((double)e.Location.X / zoomflag);
                    Points[count1].Y = (int)((double)e.Location.Y / zoomflag);
                    Console.WriteLine("point.x = {0},point.y = {1}", Points[count1].X, Points[count1].Y);
                    if (count1 > 0)
                    {
                        System.Drawing.Point[] point2 = new System.Drawing.Point[2];
                        point2[0] = Points[count1 - 1];
                        point2[1] = Points[count1];
                        image.Draw(point2, bgr);
                    }

                    CvInvoke.PutText(image, "test", Points[count1], FontFace.HersheySimplex, 0.5, new MCvScalar(255, 255, 0), 1);
                    ibox.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;
                    ibox.Image.Save("H:/wpf_opencv/WpfApp2/new.jpg");
                    image = new Image<Bgr, byte>("H:/wpf_opencv/WpfApp2/new.jpg");
                    ibox.Image = image;
                    count1++;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                Console.WriteLine("right");
                if (drawrec.IsChecked == true)
                {
                    endpoint = e.Location;
                    CvInvoke.Rectangle(image, new System.Drawing.Rectangle(startpoint, new System.Drawing.Size(endpoint.X - startpoint.X, endpoint.Y - startpoint.Y)), new MCvScalar(0, 0, 255), 1);
                    ibox.Image.Save("H:/wpf_opencv/WpfApp2/new.jpg");
                    image = new Image<Bgr, byte>("H:/wpf_opencv/WpfApp2/new.jpg");
                    ibox.Image = image;
                }
                else if (drawpoly.IsChecked == true)
                {
                    System.Drawing.Point[] pointpoly = new System.Drawing.Point[count1];
                    Array.Copy(Points, pointpoly, count1);
                    Image<Bgr, Byte> image = new Image<Bgr, byte>("H:/wpf_opencv/WpfApp2/1.jpg");
                    ibox.Image = image;
                    image.DrawPolyline(pointpoly, true, bgr);
                    ibox.Image.Save("H:/wpf_opencv/WpfApp2/new.jpg");
                    image = new Image<Bgr, byte>("H:/wpf_opencv/WpfApp2/new.jpg");
                    ibox.Image = image;
                }
            }

        }

        private void DrawLine(System.Drawing.Point startpoint, System.Drawing.Point endpoint)
        {

        }
        static int ret = 0;
        private void Setzoom_Click(object sender, RoutedEventArgs e)
        {
            zoomflag = Convert.ToDouble(zoomsize.Text.ToString());
            ibox.SetZoomScale(zoomflag, System.Drawing.Point.Empty);
            //ibox.SetZoomScale(ImageBox.ZoomLevels[ret], System.Drawing.Point.Empty);
            //string strzoom = ImageBox.ZoomLevels[ret].ToString();
            //Console.WriteLine("zoomsize : {0}",ImageBox.ZoomLevels[ret].ToString());
            ////zoomflag = Convert.ToDouble(strzoom);
            //ibox.HorizontalScrollBar.Visible = false;
            //ibox.VerticalScrollBar.Visible = false;
            //ret++;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Image<Bgr, Byte> image = new Image<Bgr, byte>("H:/wpf_opencv/WpfApp2/1.jpg");
            ibox.Image = image;
            count1 = 0;
        }

        private void Ibox_MouseEnter(object sender, EventArgs e)
        {
            Console.WriteLine("mouse enter");
            Cursor = System.Windows.Input.Cursors.Cross;

        }

        private void Ibox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ibox.Height = image.Height;
            ibox.Width = image.Width;
            if ((e.Button == MouseButtons.Left) && (drawrec.IsChecked == true))
            {
                ////reverse the previous highlighted rectangle, if there is any
                //if (!_bufferPoint.IsEmpty)
                //{
                //    DrawReversibleRectangle(GetRectanglePreserveAspect(_bufferPoint, _mouseDownPosition));
                //}

                ////draw the newly selected area
                //DrawReversibleRectangle(GetRectanglePreserveAspect(e.Location, _mouseDownPosition));

                //_bufferPoint = e.Location;               
                CvInvoke.PutText(image, "rec", rec_startpoint, FontFace.HersheySimplex, 0.5, new MCvScalar(255, 255, 0), 1);
                rec_point = e.Location;
                Console.WriteLine("rec_x = {0},rec_y = {1}", rec_point.X, rec_point.Y);
                Rectangle rec = new Rectangle(startpoint, new System.Drawing.Size(rec_point.X - startpoint.X, rec_point.Y - startpoint.Y));
                //ibox.DrawReversibleRectangle(rec);
                image = new Image<Bgr, byte>("H:/wpf_opencv/WpfApp2/1.jpg");
                ibox.Image = image;
                CvInvoke.Rectangle(image, rec, new MCvScalar(0, 255, 0), 2, LineType.AntiAlias);
                ibox.Image.Save("H:/wpf_opencv/WpfApp2/tmp.jpg");
                image = new Image<Bgr, byte>("H:/wpf_opencv/WpfApp2/tmp.jpg");
                ibox.Image = image;
            }
        }

        private void Ibox_MouseLeave(object sender, EventArgs e)
        {
            Cursor = System.Windows.Input.Cursors.Arrow;
        }

        private void Ibox_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if ((drawrec.IsChecked == true) && (e.Button == MouseButtons.Left))
            {
                rec_endpoint = e.Location;
                CvInvoke.PutText(image, "rec", rec_startpoint, FontFace.HersheySimplex, 0.5, new MCvScalar(255, 255, 0), 1);
                Console.WriteLine("rec_endpoint = {0},rec_endpoint = {1}", rec_endpoint.X, rec_endpoint.Y);
                Rectangle rec = new Rectangle(startpoint, new System.Drawing.Size(rec_endpoint.X - startpoint.X, rec_endpoint.Y - startpoint.Y));
                image = new Image<Bgr, byte>("H:/wpf_opencv/WpfApp2/1.jpg");
                ibox.Image = image;
                CvInvoke.Rectangle(image, rec, new MCvScalar(0, 255, 0), 2, LineType.AntiAlias);
                ibox.Image.Save("H:/wpf_opencv/WpfApp2/end.jpg");
                image = new Image<Bgr, byte>("H:/wpf_opencv/WpfApp2/end.jpg");
                ibox.Image = image;
            }
        }

    }
}
