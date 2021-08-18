﻿using System.Windows;
using System.Windows.Media;

namespace ImageSelector.Anchors
{
    public class RoundAnchor : Anchor
    {
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            Point center = new Point(base.Position.X, base.Position.Y);
            Pen pen = new Pen(Brushes.Red, 2.0 / base.Magnification);
            drawingContext.DrawEllipse(Brushes.Transparent, pen, center, 5.0 / base.Magnification, 5.0 / base.Magnification);
        }
    }
}
