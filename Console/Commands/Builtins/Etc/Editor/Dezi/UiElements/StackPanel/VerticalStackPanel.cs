using System.Linq;

namespace dezi.UiElements.StackPanel
{
    /// <summary>
    /// Can be used to display UiElements above each other.
    /// Start is on top and fills up on the bottom.
    /// </summary>
    public class VerticalStackPanel : StackPanel
    {
        public void AddTop(UiElement uiElement)
        {
            this.Add(uiElement, 0);
        }

        public void AddBottom(UiElement uiElement)
        {
            this.Add(uiElement, this.uiElements.Count);
        }

        protected override void UpdateSubmoduleSizesAndPositions(int widthDelta, int heightDelta)
        {
            // TODO: finalize this method with all min and max height cases
            int numberOfElementsWithDynamicHeight = this.uiElements.Count(ue => ue.UiElement.MinHeight == -1 && ue.UiElement.MaxHeight == -1);
            int maxHeightFromAllElements = this.uiElements.Where(ue => ue.UiElement.MaxHeight != -1).Select(ue => ue.UiElement.MaxHeight).Sum();
            int minHeightFromAllElements = this.uiElements.Where(ue => ue.UiElement.MinHeight != -1).Select(ue => ue.UiElement.MinHeight).Sum();
            int yCoordinateForSubElement = 0;
            foreach (UiElement uiElement in this.uiElements.OrderBy(ue => ue.Index).Select(ue => ue.UiElement))
            {
                uiElement.CoordinateY = this.CoordinateY + yCoordinateForSubElement;
                if (uiElement.MaxHeight == -1)
                {
                    uiElement.Height = (this.Height - maxHeightFromAllElements) / numberOfElementsWithDynamicHeight;
                    yCoordinateForSubElement += uiElement.Height;
                }
                else
                {
                    uiElement.Height = uiElement.MaxHeight;
                }
                uiElement.CoordinateX = this.CoordinateX;
                uiElement.Width = this.Width;
            }
        }
    }
}
