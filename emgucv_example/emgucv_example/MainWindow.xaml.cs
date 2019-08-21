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
        Image<Bgr, Byte> image;
        Bgr bgr = new Bgr(System.Drawing.Color.Red);
        System.Drawing.Point[] PointPoly = new System.Drawing.Point[100];
        static int count1 = 0;
        static double ZoomScale = 1.0;//缩放系数默认1.0

        public System.Drawing.Point rec_startpoint;
        public System.Drawing.Point rec_point;
        public System.Drawing.Point rec_endpoint;
        string BasePath = string.Empty;//项目debug目录
        string path_SourceImg =string.Empty;//源图片路径
        string path_LineImg = string.Empty;//画线保存路径
        string path_RecImg = string.Empty;//画矩形保存路径
        string path_PolyImg = string.Empty;//画多边形保存路径
        string path_PolyTmpImg = string.Empty;//画多边形临时存放路径

        //System.Drawing.Point[] PointLine = new System.Drawing.Point[2];       
        System.Drawing.Point StartPoint = new System.Drawing.Point();
        System.Drawing.Point EndPoint = new System.Drawing.Point();
        System.Drawing.Point ActualPoint = new System.Drawing.Point();
        bool LineStart = true;//标记是否为线的起点
        List<LineList> lineLists = new List<LineList>();
        LineList Line1 = new LineList(new System.Drawing.Point(0,0),new System.Drawing.Point(0,0));
        public MainWindow()
        {
            InitializeComponent();

            BasePath = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine(BasePath);
            path_SourceImg = string.Format(@"{0}\img\source.jpg", BasePath);
            path_LineImg = string.Format(@"{0}\img\line.jpg", BasePath);
            path_RecImg = string.Format(@"{0}\img\rec.jpg", BasePath);
            path_PolyImg = string.Format(@"{0}\img\poly.jpg", BasePath);
            path_PolyTmpImg = string.Format(@"{0}\img\polytmp.jpg",BasePath);
            image = new Image<Bgr, byte>(path_SourceImg);

            ibox.Image = image;
            ibox.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;//取消自带功能
            ibox.HorizontalScrollBar.Visible = false;//水平滚动条不可见
            ibox.VerticalScrollBar.Visible = false;//垂直滚动条不可见

            ibox.MouseWheel += new System.Windows.Forms.MouseEventHandler(OnMouseWheel);//
            Console.WriteLine("image height = {0}", image.Height.ToString());
            Console.WriteLine("image width = {0}", image.Width.ToString());
            ibox.Height = image.Height;
            ibox.Width = image.Width;
        }
        

        private void OnMouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        { 
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

        private void Ibox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ActualPoint = e.Location;
            ActualPoint.X = (int)((double)ActualPoint.X / ZoomScale);//计算缩放之后点的实际位置
            ActualPoint.Y = (int)((double)ActualPoint.Y / ZoomScale);
            Console.WriteLine("ActualPoint.x={0},ActualPoint.y={1}",ActualPoint.X,ActualPoint.Y);
            if (e.Button == MouseButtons.Left)//左键按下点连线
            {
                Console.WriteLine("left");
                int x = e.Location.X;
                int y = e.Location.Y;
                Console.WriteLine("x={0},y={1}", x, y);
                if (RB_DrawLine.IsChecked == true)//画线
                {
                    if (LineStart == true)
                    {
                        StartPoint = ActualPoint;
                        Line1.StartPoint = ActualPoint;
                        //Line1.StartPoint.X = ActualPoint.X;
                        //Line1.StartPoint.X = ActualPoint.Y;
                    }
                    else if (LineStart == false)//线的终点
                    {
                        EndPoint = ActualPoint;
                        Line1.EndPoint = ActualPoint;
                        lineLists.Add(Line1);
                        CvInvoke.Line(image, Line1.StartPoint, Line1.EndPoint, new MCvScalar(255, 0, 0));
                        ibox.Image.Save(path_LineImg);
                        image = new Image<Bgr, byte>(path_LineImg);
                        ibox.Image = image;
                    }
                    LineStart = !LineStart;
                }
                else if (RB_DrawRec.IsChecked == true)
                {
                    StartPoint = e.Location;
                    Console.WriteLine("startpoint.x = {0},startpoint.y = {1}", StartPoint.X, StartPoint.Y);
                }
                else if (RB_DrawPoly.IsChecked == true)
                {
                    PointPoly[count1] = e.Location;
                    PointPoly[count1].X = (int)((double)e.Location.X / ZoomScale);
                    PointPoly[count1].Y = (int)((double)e.Location.Y / ZoomScale);
                    Console.WriteLine("point.x = {0},point.y = {1}", PointPoly[count1].X, PointPoly[count1].Y);
                    if (count1 > 0)
                    {
                        System.Drawing.Point[] point2 = new System.Drawing.Point[2];
                        point2[0] = PointPoly[count1 - 1];
                        point2[1] = PointPoly[count1];
                        image.Draw(point2, bgr);
                    }

                    CvInvoke.PutText(image, "test", PointPoly[count1], FontFace.HersheySimplex, 0.5, new MCvScalar(255, 255, 0), 1);
                    ibox.FunctionalMode = ImageBox.FunctionalModeOption.Minimum;
                    ibox.Image.Save(path_PolyImg);
                    image = new Image<Bgr, byte>(path_PolyImg);
                    ibox.Image = image;
                    count1++;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                Console.WriteLine("right");
                if (RB_DrawRec.IsChecked == true)
                {
                    EndPoint = e.Location;
                    CvInvoke.Rectangle(image, new System.Drawing.Rectangle(StartPoint, new System.Drawing.Size(EndPoint.X - StartPoint.X, EndPoint.Y - StartPoint.Y)), new MCvScalar(0, 0, 255), 1);
                    ibox.Image.Save(path_RecImg);
                    image = new Image<Bgr, byte>(path_RecImg);
                    ibox.Image = image;
                }
                else if (RB_DrawPoly.IsChecked == true)
                {
                    System.Drawing.Point[] pointpoly = new System.Drawing.Point[count1];
                    Array.Copy(PointPoly, pointpoly, count1);
                    Image<Bgr, Byte> image = new Image<Bgr, byte>(path_SourceImg);
                    ibox.Image = image;
                    image.DrawPolyline(pointpoly, true, bgr);
                    ibox.Image.Save(path_RecImg);
                    image = new Image<Bgr, byte>(path_RecImg);
                    ibox.Image = image;
                }
            }

        }

        private void Setzoom_Click(object sender, RoutedEventArgs e)
        {
            ZoomScale = Convert.ToDouble(zoomsize.Text.ToString());
            ibox.SetZoomScale(ZoomScale, System.Drawing.Point.Empty);
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
            Image<Bgr, Byte> image = new Image<Bgr, byte>(path_SourceImg);
            ibox.Image = image;
            count1 = 0;
        }

        private void Ibox_MouseEnter(object sender, EventArgs e)//鼠标位于imagebox时改变样式
        {
            Console.WriteLine("mouse enter");
            Cursor = System.Windows.Input.Cursors.Cross;

        }

        private void Ibox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //ibox.Height = image.Height;
            //ibox.Width = image.Width;
            if ((e.Button == MouseButtons.Left) && (RB_DrawRec.IsChecked == true))
            {              
                CvInvoke.PutText(image, "rec", rec_startpoint, FontFace.HersheySimplex, 0.5, new MCvScalar(255, 255, 0), 1);
                rec_point = e.Location;
                Console.WriteLine("rec_x = {0},rec_y = {1}", rec_point.X, rec_point.Y);
                Rectangle rec = new Rectangle(StartPoint, new System.Drawing.Size(rec_point.X - StartPoint.X, rec_point.Y - StartPoint.Y));
                //ibox.DrawReversibleRectangle(rec);
                image = new Image<Bgr, byte>(path_SourceImg);
                ibox.Image = image;
                CvInvoke.Rectangle(image, rec, new MCvScalar(0, 255, 0), 2, LineType.AntiAlias);
                ibox.Image.Save(path_PolyTmpImg);
                image = new Image<Bgr, byte>(path_PolyTmpImg);
                ibox.Image = image;
            }
        }

        private void Ibox_MouseLeave(object sender, EventArgs e)
        {
            Cursor = System.Windows.Input.Cursors.Arrow;
        }

        private void Ibox_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if ((RB_DrawRec.IsChecked == true) && (e.Button == MouseButtons.Left))
            {
                rec_endpoint = e.Location;
                CvInvoke.PutText(image, "rec", rec_startpoint, FontFace.HersheySimplex, 0.5, new MCvScalar(255, 255, 0), 1);
                Console.WriteLine("rec_endpoint = {0},rec_endpoint = {1}", rec_endpoint.X, rec_endpoint.Y);
                Rectangle rec = new Rectangle(StartPoint, new System.Drawing.Size(rec_endpoint.X - StartPoint.X, rec_endpoint.Y - StartPoint.Y));
                image = new Image<Bgr, byte>(path_SourceImg);
                ibox.Image = image;
                CvInvoke.Rectangle(image, rec, new MCvScalar(0, 255, 0), 2, LineType.AntiAlias);
                ibox.Image.Save(path_PolyImg);
                image = new Image<Bgr, byte>(path_PolyImg);
                ibox.Image = image;
            }
        }

    }
}
