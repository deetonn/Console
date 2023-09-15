using System.Linq;

namespace dezi.UiElements.StackPanel
{
    /// <summary>
    /// Can be used to show UiElements next to each other
    /// (start left and filling up to the right)
    /// </summary>
    public class HorizontalStackPanel : StackPanel
    {
        public void AddLeft(UiElement uiElement)
        {
            this.Add(uiElement, 0);
        }

        public void AddRight(UiElement uiElement)
        {
            this.Add(uiElement, this.uiElements.Count);
        }

        protected override void UpdateSubmoduleSizesAndPositions(int widthDelta, int heightDelta)
        {
            // TODO: finalize this method with all min and max width cases
            int numberOfElementsWithDynamicWidth = this.uiElements.Count(ue => ue.UiElement.MinWidth == -1 && ue.UiElement.MaxWidth == -1);
            int maxWidthFromAllElements = this.uiElements.Where(ue => ue.UiElement.MaxWidth != -1).Select(ue => ue.UiElement.MaxWidth).Sum();
            int minWidthFromAllElements = this.uiElements.Where(ue => ue.UiElement.MinWidth != -1).Select(ue => ue.UiElement.MinWidth).Sum();
            int xCoordinateForSubElement = 0;
            foreach (UiElement uiElement in this.uiElements.OrderBy(ue => ue.Index).Select(ue => ue.UiElement))
            {
                uiElement.CoordinateX = this.CoordinateX + xCoordinateForSubElement;
                if (uiElement.MaxWidth == -1)
                {
                    uiElement.Width = (this.Width - maxWidthFromAllElements) / numberOfElementsWithDynamicWidth;
                    xCoordinateForSubElement += uiElement.Width;
                }
                else
                {
                    uiElement.Width = uiElement.MaxWidth;
                }
                uiElement.CoordinateX = this.CoordinateX;
                uiElement.Height = this.Height;
            }
        }
    }
}
