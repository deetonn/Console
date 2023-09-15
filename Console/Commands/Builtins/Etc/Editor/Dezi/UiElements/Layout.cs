using System.Collections.Generic;

namespace dezi.UiElements
{
    public interface Layout
    {
        public IList<UiElement> GetInteractiveUiElements();
    }
}
