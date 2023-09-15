namespace dezi.UiElements.StackPanel
{
    public class StackPanelElement
    {
        public UiElement UiElement { get; }

        public int Index { get; set; }

        public StackPanelElement(UiElement uiElement, int index)
        {
            this.UiElement = uiElement;
            this.Index = index;
        }
    }
}
