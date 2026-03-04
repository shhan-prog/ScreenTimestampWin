using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ScreenTimestampWin.Capture
{
    public partial class OverlayWindow : Window
    {
        private Point _startPoint;
        private bool _isDragging;
        private readonly RectangleGeometry _selectionGeometry = new();

        public Rect SelectedRect { get; private set; }
        public bool Cancelled { get; private set; } = true;

        public OverlayWindow()
        {
            InitializeComponent();
            Cursor = Cursors.Cross;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            _startPoint = e.GetPosition(this);
            _isDragging = true;
            CaptureMouse();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!_isDragging) return;

            var current = e.GetPosition(this);
            var rect = new Rect(
                Math.Min(_startPoint.X, current.X),
                Math.Min(_startPoint.Y, current.Y),
                Math.Abs(current.X - _startPoint.X),
                Math.Abs(current.Y - _startPoint.Y)
            );

            UpdateOverlay(rect);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (!_isDragging) return;

            _isDragging = false;
            ReleaseMouseCapture();

            var current = e.GetPosition(this);
            var rect = new Rect(
                Math.Min(_startPoint.X, current.X),
                Math.Min(_startPoint.Y, current.Y),
                Math.Abs(current.X - _startPoint.X),
                Math.Abs(current.Y - _startPoint.Y)
            );

            if (rect.Width < 2 || rect.Height < 2)
            {
                Cancelled = true;
                Close();
                return;
            }

            SelectedRect = rect;
            Cancelled = false;
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Cancelled = true;
                _isDragging = false;
                ReleaseMouseCapture();
                Close();
            }
        }

        private void UpdateOverlay(Rect selectionRect)
        {
            OverlayCanvas.Children.Clear();

            // 반투명 배경 + 선택 영역 cutout
            var fullRect = new RectangleGeometry(new Rect(0, 0, ActualWidth, ActualHeight));
            _selectionGeometry.Rect = selectionRect;

            var combined = new CombinedGeometry(GeometryCombineMode.Exclude, fullRect, _selectionGeometry);

            var overlay = new Path
            {
                Data = combined,
                Fill = new SolidColorBrush(Color.FromArgb(77, 28, 28, 30)) // 30% opacity
            };
            OverlayCanvas.Children.Add(overlay);

            // 선택 테두리
            var border = new Rectangle
            {
                Width = selectionRect.Width,
                Height = selectionRect.Height,
                Stroke = new SolidColorBrush(Color.FromArgb(204, 255, 255, 255)), // 80% opacity
                StrokeThickness = 1,
                Fill = Brushes.Transparent
            };
            Canvas.SetLeft(border, selectionRect.X);
            Canvas.SetTop(border, selectionRect.Y);
            OverlayCanvas.Children.Add(border);
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            // 초기 반투명 오버레이 표시
            var overlay = new Rectangle
            {
                Width = ActualWidth,
                Height = ActualHeight,
                Fill = new SolidColorBrush(Color.FromArgb(77, 28, 28, 30))
            };
            OverlayCanvas.Children.Add(overlay);
        }
    }
}
