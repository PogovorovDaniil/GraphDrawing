using GraphDrawing.Controllers;
using GraphDrawing.Helpers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GraphDrawing.Views
{
    public partial class MainWindow : Window
    {
        private const double ellipseSize = 10;
        private GraphController graphController;
        private List<Ellipse> ellipseNodes;
        private List<Line> linkLines;
        private DispatcherTimer timer;
        public MainWindow()
        {
            InitializeComponent();
            graphController = new GraphController();
            ellipseNodes = new List<Ellipse>();
            linkLines = new List<Line>();
            SolidColorBrush redBrush = new SolidColorBrush(Colors.Red);
            SolidColorBrush blackBrush = new SolidColorBrush(Colors.Black);
            foreach ((Vector2 node1, Vector2 node2) in graphController.GetLinks())
            {
                Line linkLine = new Line();
                linkLine.Stroke = blackBrush;
                linkLines.Add(linkLine);
                MainCanvas.Children.Add(linkLine);
            }
            foreach (var node in graphController.GetNodes())
            {
                Ellipse ellipseNode = new Ellipse();
                ellipseNode.Width = ellipseSize;
                ellipseNode.Height = ellipseSize;
                ellipseNode.Fill = redBrush;
                ellipseNode.Stroke = blackBrush;
                ellipseNodes.Add(ellipseNode);
                MainCanvas.Children.Add(ellipseNode);
            }

            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Start();
        }

        private Vector2 ToCanvasCoordinate(Vector2 vector, float padding = 20) => new Vector2(
            (vector.X + 1) * (float)(MainCanvas.Width - padding * 2) / 2 + padding,
            (vector.Y + 1) * (float)(MainCanvas.Height - padding * 2) / 2 + padding);

        private void Timer_Tick(object sender, EventArgs e)
        {
            List<Vector2> nodes = graphController.GetNodes();
            for (int i = 0; i < ellipseNodes.Count; i++)
            {
                var nodeCanvas = ToCanvasCoordinate(nodes[i]);
                ellipseNodes[i].SetPosition(nodeCanvas);
            }
            List<(Vector2 node1, Vector2 node2)> links = graphController.GetLinks();
            for (int i = 0; i < linkLines.Count; i++)
            {
                var node1Canvas = ToCanvasCoordinate(links[i].node1);
                var node2Canvas = ToCanvasCoordinate(links[i].node2);

                linkLines[i].X1 = node1Canvas.X;
                linkLines[i].Y1 = node1Canvas.Y;
                linkLines[i].X2 = node2Canvas.X;
                linkLines[i].Y2 = node2Canvas.Y;
            }
        }

        ~MainWindow() => timer.Stop();
    }
}
