using System.Windows;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace EmergencyStop;

public sealed class OuterRingElement : FrameworkElement
{
    public static readonly DependencyProperty KindProperty = DependencyProperty.Register(
        nameof(Kind),
        typeof(OuterRingStyle),
        typeof(OuterRingElement),
        new FrameworkPropertyMetadata(OuterRingStyle.Brackets, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
        nameof(Stroke),
        typeof(Brush),
        typeof(OuterRingElement),
        new FrameworkPropertyMetadata(Brushes.Cyan, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty RingSizeProperty = DependencyProperty.Register(
        nameof(RingSize),
        typeof(double),
        typeof(OuterRingElement),
        new FrameworkPropertyMetadata(126d, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty RingThicknessProperty = DependencyProperty.Register(
        nameof(RingThickness),
        typeof(double),
        typeof(OuterRingElement),
        new FrameworkPropertyMetadata(3d, FrameworkPropertyMetadataOptions.AffectsRender));

    public OuterRingStyle Kind
    {
        get => (OuterRingStyle)GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    public Brush Stroke
    {
        get => (Brush)GetValue(StrokeProperty);
        set => SetValue(StrokeProperty, value);
    }

    public double RingSize
    {
        get => (double)GetValue(RingSizeProperty);
        set => SetValue(RingSizeProperty, value);
    }

    public double RingThickness
    {
        get => (double)GetValue(RingThicknessProperty);
        set => SetValue(RingThicknessProperty, value);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        var size = Math.Clamp(RingSize, 72, Math.Min(ActualWidth, ActualHeight));
        var thickness = Math.Clamp(RingThickness, 1, 8);
        var radius = Math.Max(1, size / 2 - thickness / 2);
        var center = new Point(ActualWidth / 2, ActualHeight / 2);
        var pen = new Pen(Stroke, thickness)
        {
            StartLineCap = PenLineCap.Round,
            EndLineCap = PenLineCap.Round,
            LineJoin = PenLineJoin.Round
        };

        switch (Kind)
        {
            case OuterRingStyle.Circle:
                drawingContext.DrawEllipse(null, pen, center, radius, radius);
                break;
            case OuterRingStyle.Corners:
                DrawCorners(drawingContext, center, radius, pen);
                break;
            case OuterRingStyle.Ticks:
                DrawTicks(drawingContext, center, radius, pen);
                break;
            default:
                DrawBrackets(drawingContext, center, radius, pen);
                break;
        }
    }

    private static void DrawBrackets(DrawingContext drawingContext, Point center, double radius, Pen pen)
    {
        DrawArc(drawingContext, center, radius, -122, 64, pen);
        DrawArc(drawingContext, center, radius, -32, 64, pen);
        DrawArc(drawingContext, center, radius, 58, 64, pen);
        DrawArc(drawingContext, center, radius, 148, 64, pen);
    }

    private static void DrawCorners(DrawingContext drawingContext, Point center, double radius, Pen pen)
    {
        var corner = radius * 0.44;
        var left = center.X - radius;
        var right = center.X + radius;
        var top = center.Y - radius;
        var bottom = center.Y + radius;

        drawingContext.DrawLine(pen, new Point(left, top + corner), new Point(left, top));
        drawingContext.DrawLine(pen, new Point(left, top), new Point(left + corner, top));
        drawingContext.DrawLine(pen, new Point(right - corner, top), new Point(right, top));
        drawingContext.DrawLine(pen, new Point(right, top), new Point(right, top + corner));
        drawingContext.DrawLine(pen, new Point(right, bottom - corner), new Point(right, bottom));
        drawingContext.DrawLine(pen, new Point(right, bottom), new Point(right - corner, bottom));
        drawingContext.DrawLine(pen, new Point(left + corner, bottom), new Point(left, bottom));
        drawingContext.DrawLine(pen, new Point(left, bottom), new Point(left, bottom - corner));
    }

    private static void DrawTicks(DrawingContext drawingContext, Point center, double radius, Pen pen)
    {
        var tickLength = Math.Max(7, radius * 0.16);

        for (var i = 0; i < 8; i++)
        {
            var angle = i * 45d * Math.PI / 180d;
            var start = new Point(
                center.X + Math.Cos(angle) * (radius - tickLength),
                center.Y + Math.Sin(angle) * (radius - tickLength));
            var end = new Point(
                center.X + Math.Cos(angle) * radius,
                center.Y + Math.Sin(angle) * radius);
            drawingContext.DrawLine(pen, start, end);
        }
    }

    private static void DrawArc(DrawingContext drawingContext, Point center, double radius, double startDegrees, double sweepDegrees, Pen pen)
    {
        var start = PointOnCircle(center, radius, startDegrees);
        var end = PointOnCircle(center, radius, startDegrees + sweepDegrees);
        var geometry = new StreamGeometry();

        using (var context = geometry.Open())
        {
            context.BeginFigure(start, false, false);
            context.ArcTo(end, new Size(radius, radius), 0, sweepDegrees > 180, SweepDirection.Clockwise, true, false);
        }

        geometry.Freeze();
        drawingContext.DrawGeometry(null, pen, geometry);
    }

    private static Point PointOnCircle(Point center, double radius, double degrees)
    {
        var radians = degrees * Math.PI / 180d;
        return new Point(center.X + Math.Cos(radians) * radius, center.Y + Math.Sin(radians) * radius);
    }
}
