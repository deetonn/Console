using dezi.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace dezi.UiElements.StackPanel
{
    public abstract class StackPanel : UiElement, Layout
    {
        protected IList<StackPanelElement> uiElements;

        public StackPanel()
        {
            this.uiElements = new List<StackPanelElement>();
        }

        public void Remove(int index)
        {
            this.uiElements.RemoveAt(index);
        }

        public void Add(UiElement uiElement, int index)
        {
            this.uiElements.Add(new StackPanelElement(uiElement, index));
        }

        public override void Render(IList<string> uiOutput)
        {
            foreach (UiElement uiElement in this.uiElements.OrderBy(ue => ue.Index).Select(ue => ue.UiElement))
            {
                uiElement.Render(uiOutput);
            }
        }

        public IList<UiElement> GetInteractiveUiElements()
        {
            List<UiElement> uiElements = new List<UiElement>();
            this.uiElements
                .Where(ue => ue.GetType().IsInstanceOfType(typeof(Layout)))
                .Select(l => ((Layout)l).GetInteractiveUiElements())
                .ToList()
                .ForEach(ue => uiElements.AddRange(ue));
            uiElements.AddRange(
                this.uiElements
                    .Where(ue => ue.UiElement.IsInteractiveElement)
                    .Select(iue => iue.UiElement)
                    .ToList());
            return uiElements;
        }

        public override void HandleInput(InputAction inputAction)
        {
            throw new NotImplementedException();
        }
    }
}
