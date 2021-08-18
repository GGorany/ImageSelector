using System.Windows.Data;

using ImageSelector.ROIs;

namespace ImageSelector.Anchors
{
    public static class AnchorsFactory
    {
        public static Anchor Create(AnchorType type, ROI roi)
        {
            Anchor anchor = null;
            switch (type)
            {
                case AnchorType.Move: anchor = new CrossAnchor(); break;
                case AnchorType.Resize: anchor = new RoundAnchor(); break;
                default: return null;
            }

            Binding binding = new Binding(Anchor.MagnificationProperty.Name);
            binding.Source = roi;
            anchor.SetBinding(Anchor.MagnificationProperty, binding);

            return anchor;

        }
    }
}
