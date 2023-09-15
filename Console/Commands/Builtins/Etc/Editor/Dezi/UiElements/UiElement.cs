using dezi.Input;
using System.Collections.Generic;

namespace dezi.UiElements
{
    public abstract class UiElement
    {
        private static int idCounter = 0;

        private int width;
        private int height;

        public int Width
        {
            get
            {
                return this.width;
            }
            set
            {
                int widthDelta = value - this.width;
                this.width = value;
                this.UpdateSubmoduleSizesAndPositions(widthDelta, 0);
            }
        }

        public int Height
        {
            get
            {
                return this.height;
            }
            set
            {
                int heightDelta = value - this.height;
                this.height = value;
                this.UpdateSubmoduleSizesAndPositions(0, heightDelta);
            }
        }

        public int MinHeight { get; set; }

        public int MaxHeight { get; set; }

        public int MinWidth { get; set; }

        public int MaxWidth { get; set; }

        public bool IsInFocus { get; set; }

        public bool IsInteractiveElement { get; set; }

        public int ID { get; }

        public int CoordinateX { get; set; }

        public int CoordinateY { get; set; }

        public bool ShowElement { get; set; }

        protected UiElement()
        {
            this.IsInFocus = false;

            this.IsInteractiveElement = false;

            this.ID = idCounter++;
            this.ShowElement = true;

            // Use -1 for deactivating min and max setting for UiElement
            this.MinHeight = -1;
            this.MaxHeight = -1;
            this.MinWidth = -1;
            this.MaxWidth = -1;
        }

        /// <summary>
        /// Informs UiElement that size has changed and therefore the sizes of the submodules change too.
        /// When there are no submodules, let it empty.
        /// </summary>
        /// <param name="widthDelta"></param>
        /// <param name="heightDelta"></param>
        protected abstract void UpdateSubmoduleSizesAndPositions(int widthDelta, int heightDelta);

        public abstract void Render(IList<string> uiOutput);

        public abstract void HandleInput(InputAction inputAction);

        public virtual IList<Cursor> GetCursors()
        {
            return new List<Cursor>();
        }
    }
}
