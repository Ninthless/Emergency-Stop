using System.Windows;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;

namespace EmergencyStop;

public sealed class CrosshairElement : FrameworkElement
{
    public static readonly DependencyProperty KindProperty = DependencyProperty.Register(
        nameof(Kind),
        typeof(CrosshairStyle),
        typeof(CrosshairElement),
        new FrameworkPropertyMetadata(CrosshairStyle.Classic, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty CrosshairBrushProperty = DependencyProperty.Register(
        nameof(CrosshairBrush),
        typeof(Brush),
        typeof(CrosshairElement),
        new FrameworkPropertyMetadata(Brushes.Cyan, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty OutlineBrushProperty = DependencyProperty.Register(
        nameof(OutlineBrush),
        typeof(Brush),
        typeof(CrosshairElement),
        new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty CrosshairSizeProperty = DependencyProperty.Register(
        nameof(CrosshairSize),
        typeof(double),
        typeof(CrosshairElement),
        new FrameworkPropertyMetadata(4d, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty CrosshairThicknessProperty = DependencyProperty.Register(
        nameof(CrosshairThickness),
        typeof(double),
        typeof(CrosshairElement),
        new FrameworkPropertyMetadata(2d, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty CrosshairGapProperty = DependencyProperty.Register(
        nameof(CrosshairGap),
        typeof(double),
        typeof(CrosshairElement),
        new FrameworkPropertyMetadata(2d, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty OutlineThicknessProperty = DependencyProperty.Register(
        nameof(OutlineThickness),
        typeof(double),
        typeof(CrosshairElement),
        new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty OutlineOpacityProperty = DependencyProperty.Register(
        nameof(OutlineOpacity),
        typeof(double),
        typeof(CrosshairElement),
        new FrameworkPropertyMetadata(0.65d, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty ShowCenterDotProperty = DependencyProperty.Register(
        nameof(ShowCenterDot),
        typeof(bool),
        typeof(CrosshairElement),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty CenterDotSizeProperty = DependencyProperty.Register(
        nameof(CenterDotSize),
        typeof(double),
        typeof(CrosshairElement),
        new FrameworkPropertyMetadata(2d, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty CenterDotOpacityProperty = DependencyProperty.Register(
        nameof(CenterDotOpacity),
        typeof(double),
        typeof(CrosshairElement),
        new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty ShowOuterLinesProperty = DependencyProperty.Register(
        nameof(ShowOuterLines),
        typeof(bool),
        typeof(CrosshairElement),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty OuterLineSizeProperty = DependencyProperty.Register(
        nameof(OuterLineSize),
        typeof(double),
        typeof(CrosshairElement),
        new FrameworkPropertyMetadata(2d, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty OuterLineThicknessProperty = DependencyProperty.Register(
        nameof(OuterLineThickness),
        typeof(double),
        typeof(CrosshairElement),
        new FrameworkPropertyMetadata(2d, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty OuterLineGapProperty = DependencyProperty.Register(
        nameof(OuterLineGap),
        typeof(double),
        typeof(CrosshairElement),
        new FrameworkPropertyMetadata(10d, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty OuterLineOpacityProperty = DependencyProperty.Register(
        nameof(OuterLineOpacity),
        typeof(double),
        typeof(CrosshairElement),
        new FrameworkPropertyMetadata(0.35d, FrameworkPropertyMetadataOptions.AffectsRender));

    public CrosshairStyle Kind
    {
        get => (CrosshairStyle)GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    public Brush CrosshairBrush
    {
        get => (Brush)GetValue(CrosshairBrushProperty);
        set => SetValue(CrosshairBrushProperty, value);
    }

    public Brush OutlineBrush
    {
        get => (Brush)GetValue(OutlineBrushProperty);
        set => SetValue(OutlineBrushProperty, value);
    }

    public double CrosshairSize
    {
        get => (double)GetValue(CrosshairSizeProperty);
        set => SetValue(CrosshairSizeProperty, value);
    }

    public double CrosshairThickness
    {
        get => (double)GetValue(CrosshairThicknessProperty);
        set => SetValue(CrosshairThicknessProperty, value);
    }

    public double CrosshairGap
    {
        get => (double)GetValue(CrosshairGapProperty);
        set => SetValue(CrosshairGapProperty, value);
    }

    public double OutlineThickness
    {
        get => (double)GetValue(OutlineThicknessProperty);
        set => SetValue(OutlineThicknessProperty, value);
    }

    public double OutlineOpacity
    {
        get => (double)GetValue(OutlineOpacityProperty);
        set => SetValue(OutlineOpacityProperty, value);
    }

    public bool ShowCenterDot
    {
        get => (bool)GetValue(ShowCenterDotProperty);
        set => SetValue(ShowCenterDotProperty, value);
    }

    public double CenterDotSize
    {
        get => (double)GetValue(CenterDotSizeProperty);
        set => SetValue(CenterDotSizeProperty, value);
    }

    public double CenterDotOpacity
    {
        get => (double)GetValue(CenterDotOpacityProperty);
        set => SetValue(CenterDotOpacityProperty, value);
    }

    public bool ShowOuterLines
    {
        get => (bool)GetValue(ShowOuterLinesProperty);
        set => SetValue(ShowOuterLinesProperty, value);
    }

    public double OuterLineSize
    {
        get => (double)GetValue(OuterLineSizeProperty);
        set => SetValue(OuterLineSizeProperty, value);
    }

    public double OuterLineThickness
    {
        get => (double)GetValue(OuterLineThicknessProperty);
        set => SetValue(OuterLineThicknessProperty, value);
    }

    public double OuterLineGap
    {
        get => (double)GetValue(OuterLineGapProperty);
        set => SetValue(OuterLineGapProperty, value);
    }

    public double OuterLineOpacity
    {
        get => (double)GetValue(OuterLineOpacityProperty);
        set => SetValue(OuterLineOpacityProperty, value);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        var center = new Point(ActualWidth / 2, ActualHeight / 2);
        var arm = Math.Clamp(CrosshairSize, 1, 24);
        var thickness = Math.Clamp(CrosshairThickness, 1, 6);
        var gap = Math.Clamp(CrosshairGap, 0, 16);
        var outline = Math.Clamp(OutlineThickness, 0, 3);
        var outlineOpacity = Math.Clamp(OutlineOpacity, 0, 1);

        DrawMainShape(drawingContext, center, arm, thickness, gap, outline, outlineOpacity);

        if (ShowOuterLines)
        {
            DrawOuterLines(drawingContext, center, outline, outlineOpacity);
        }

        if (ShowCenterDot && Kind != CrosshairStyle.Dot)
        {
            DrawDot(drawingContext, center, CenterDotSize, outline, outlineOpacity, CenterDotOpacity);
        }
    }

    private void DrawMainShape(DrawingContext drawingContext, Point center, double arm, double thickness, double gap, double outline, double outlineOpacity)
    {
        switch (Kind)
        {
            case CrosshairStyle.Dot:
                DrawDot(drawingContext, center, CenterDotSize, outline, outlineOpacity, CenterDotOpacity);
                break;
            case CrosshairStyle.Circle:
                DrawCircle(drawingContext, center, arm, thickness, gap, outline, outlineOpacity);
                break;
            case CrosshairStyle.TShape:
                DrawCross(drawingContext, center, arm, thickness, gap, outline, outlineOpacity, false);
                break;
            case CrosshairStyle.Box:
                DrawBox(drawingContext, center, arm, thickness, gap, outline, outlineOpacity);
                break;
            case CrosshairStyle.Corners:
                DrawCorners(drawingContext, center, arm, thickness, gap, outline, outlineOpacity);
                break;
            case CrosshairStyle.Diamond:
                DrawDiamond(drawingContext, center, arm, thickness, gap, outline, outlineOpacity);
                break;
            case CrosshairStyle.Chevron:
                DrawChevron(drawingContext, center, arm, thickness, gap, outline, outlineOpacity);
                break;
            default:
                DrawCross(drawingContext, center, arm, thickness, gap, outline, outlineOpacity, true);
                break;
        }
    }

    private void DrawCross(DrawingContext drawingContext, Point center, double arm, double thickness, double gap, double outline, double outlineOpacity, bool includeTop)
    {
        if (outline > 0 && outlineOpacity > 0)
        {
            drawingContext.PushOpacity(outlineOpacity);
            DrawCrossParts(drawingContext, center, arm + outline * 2, thickness + outline * 2, gap - outline, OutlineBrush, includeTop);
            drawingContext.Pop();
        }

        DrawCrossParts(drawingContext, center, arm, thickness, gap, CrosshairBrush, includeTop);
    }

    private static void DrawCrossParts(DrawingContext drawingContext, Point center, double arm, double thickness, double gap, Brush brush, bool includeTop)
    {
        var safeGap = Math.Max(0, gap);
        var half = thickness / 2;

        if (includeTop)
        {
            drawingContext.DrawRectangle(brush, null, new Rect(center.X - half, center.Y - safeGap - arm, thickness, arm));
        }

        drawingContext.DrawRectangle(brush, null, new Rect(center.X - half, center.Y + safeGap, thickness, arm));
        drawingContext.DrawRectangle(brush, null, new Rect(center.X - safeGap - arm, center.Y - half, arm, thickness));
        drawingContext.DrawRectangle(brush, null, new Rect(center.X + safeGap, center.Y - half, arm, thickness));
    }

    private void DrawDot(DrawingContext drawingContext, Point center, double size, double outline, double outlineOpacity, double opacity)
    {
        var radius = Math.Clamp(size, 1, 8);

        if (outline > 0 && outlineOpacity > 0)
        {
            drawingContext.PushOpacity(outlineOpacity);
            drawingContext.DrawEllipse(OutlineBrush, null, center, radius + outline, radius + outline);
            drawingContext.Pop();
        }

        drawingContext.PushOpacity(Math.Clamp(opacity, 0.05, 1));
        drawingContext.DrawEllipse(CrosshairBrush, null, center, radius, radius);
        drawingContext.Pop();
    }

    private void DrawCircle(DrawingContext drawingContext, Point center, double arm, double thickness, double gap, double outline, double outlineOpacity)
    {
        var radius = Math.Max(3, gap + arm * 0.85);

        if (outline > 0 && outlineOpacity > 0)
        {
            drawingContext.PushOpacity(outlineOpacity);
            drawingContext.DrawEllipse(null, CreatePen(OutlineBrush, thickness + outline * 2), center, radius, radius);
            drawingContext.Pop();
        }

        drawingContext.DrawEllipse(null, CreatePen(CrosshairBrush, thickness), center, radius, radius);
    }

    private void DrawBox(DrawingContext drawingContext, Point center, double arm, double thickness, double gap, double outline, double outlineOpacity)
    {
        var size = Math.Max(5, gap * 2 + arm);
        var rect = new Rect(center.X - size / 2, center.Y - size / 2, size, size);

        if (outline > 0 && outlineOpacity > 0)
        {
            drawingContext.PushOpacity(outlineOpacity);
            drawingContext.DrawRectangle(null, CreatePen(OutlineBrush, thickness + outline * 2), rect);
            drawingContext.Pop();
        }

        drawingContext.DrawRectangle(null, CreatePen(CrosshairBrush, thickness), rect);
    }

    private void DrawCorners(DrawingContext drawingContext, Point center, double arm, double thickness, double gap, double outline, double outlineOpacity)
    {
        if (outline > 0 && outlineOpacity > 0)
        {
            drawingContext.PushOpacity(outlineOpacity);
            DrawCornerParts(drawingContext, center, arm + outline * 2, thickness + outline * 2, gap - outline, OutlineBrush);
            drawingContext.Pop();
        }

        DrawCornerParts(drawingContext, center, arm, thickness, gap, CrosshairBrush);
    }

    private static void DrawCornerParts(DrawingContext drawingContext, Point center, double arm, double thickness, double gap, Brush brush)
    {
        var safeGap = Math.Max(0, gap);
        var size = Math.Max(3, arm);
        var left = center.X - safeGap - size;
        var right = center.X + safeGap;
        var top = center.Y - safeGap - size;
        var bottom = center.Y + safeGap;

        drawingContext.DrawRectangle(brush, null, new Rect(left, top, size, thickness));
        drawingContext.DrawRectangle(brush, null, new Rect(left, top, thickness, size));
        drawingContext.DrawRectangle(brush, null, new Rect(right, top, size, thickness));
        drawingContext.DrawRectangle(brush, null, new Rect(right + size - thickness, top, thickness, size));
        drawingContext.DrawRectangle(brush, null, new Rect(right, bottom + size - thickness, size, thickness));
        drawingContext.DrawRectangle(brush, null, new Rect(right + size - thickness, bottom, thickness, size));
        drawingContext.DrawRectangle(brush, null, new Rect(left, bottom + size - thickness, size, thickness));
        drawingContext.DrawRectangle(brush, null, new Rect(left, bottom, thickness, size));
    }

    private void DrawDiamond(DrawingContext drawingContext, Point center, double arm, double thickness, double gap, double outline, double outlineOpacity)
    {
        var radius = Math.Max(4, gap + arm);

        if (outline > 0 && outlineOpacity > 0)
        {
            drawingContext.PushOpacity(outlineOpacity);
            DrawPolygon(drawingContext, center, radius, thickness + outline * 2, OutlineBrush, 45);
            drawingContext.Pop();
        }

        DrawPolygon(drawingContext, center, radius, thickness, CrosshairBrush, 45);
    }

    private void DrawChevron(DrawingContext drawingContext, Point center, double arm, double thickness, double gap, double outline, double outlineOpacity)
    {
        if (outline > 0 && outlineOpacity > 0)
        {
            drawingContext.PushOpacity(outlineOpacity);
            DrawChevronParts(drawingContext, center, arm + outline * 2, thickness + outline * 2, gap - outline, OutlineBrush);
            drawingContext.Pop();
        }

        DrawChevronParts(drawingContext, center, arm, thickness, gap, CrosshairBrush);
    }

    private static void DrawChevronParts(DrawingContext drawingContext, Point center, double arm, double thickness, double gap, Brush brush)
    {
        var safeGap = Math.Max(0, gap);
        var pen = CreatePen(brush, thickness);
        var length = Math.Max(4, arm);
        var spread = Math.Max(3, arm * 0.75);

        drawingContext.DrawLine(pen, new Point(center.X - safeGap - length, center.Y - spread), new Point(center.X - safeGap, center.Y));
        drawingContext.DrawLine(pen, new Point(center.X - safeGap - length, center.Y + spread), new Point(center.X - safeGap, center.Y));
        drawingContext.DrawLine(pen, new Point(center.X + safeGap + length, center.Y - spread), new Point(center.X + safeGap, center.Y));
        drawingContext.DrawLine(pen, new Point(center.X + safeGap + length, center.Y + spread), new Point(center.X + safeGap, center.Y));
    }

    private void DrawOuterLines(DrawingContext drawingContext, Point center, double outline, double outlineOpacity)
    {
        var size = Math.Clamp(OuterLineSize, 1, 12);
        var thickness = Math.Clamp(OuterLineThickness, 1, 6);
        var gap = Math.Clamp(OuterLineGap, 4, 28);
        var opacity = Math.Clamp(OuterLineOpacity, 0.05, 1);

        drawingContext.PushOpacity(opacity);

        if (outline > 0 && outlineOpacity > 0)
        {
            drawingContext.PushOpacity(outlineOpacity);
            DrawCrossParts(drawingContext, center, size + outline * 2, thickness + outline * 2, gap - outline, OutlineBrush, true);
            drawingContext.Pop();
        }

        DrawCrossParts(drawingContext, center, size, thickness, gap, CrosshairBrush, true);
        drawingContext.Pop();
    }

    private static void DrawPolygon(DrawingContext drawingContext, Point center, double radius, double thickness, Brush brush, double rotationDegrees)
    {
        var pen = CreatePen(brush, thickness);
        var points = new Point[4];

        for (var i = 0; i < points.Length; i++)
        {
            var angle = (rotationDegrees + i * 90) * Math.PI / 180d;
            points[i] = new Point(center.X + Math.Cos(angle) * radius, center.Y + Math.Sin(angle) * radius);
        }

        for (var i = 0; i < points.Length; i++)
        {
            drawingContext.DrawLine(pen, points[i], points[(i + 1) % points.Length]);
        }
    }

    private static Pen CreatePen(Brush brush, double thickness)
    {
        return new Pen(brush, thickness)
        {
            StartLineCap = PenLineCap.Square,
            EndLineCap = PenLineCap.Square,
            LineJoin = PenLineJoin.Miter
        };
    }
}
