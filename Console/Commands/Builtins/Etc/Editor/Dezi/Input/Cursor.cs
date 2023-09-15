using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace dezi.Input
{
    public class Cursor : INotifyPropertyChanged
    {
        private int column;
        private int row;

        public int Row
        {
            get { return this.row; }
        }

        public int Column
        {
            get { return this.column; }
        }

        public Cursor(int row, int column)
        {
            this.row = row;
            this.column = column;
        }

        public void UpdateColumn(int delta, IList<string> linesInFile)
        {
            if (delta != 0)
            {
                int previousColumn = this.column;
                bool rowChanged = false;
                this.column += delta;
                if (this.column < 0)
                {
                    if (this.row == 0)
                    {
                        this.column = 0;
                    }
                    else
                    {
                        this.row--;
                        rowChanged = true;
                        this.column = linesInFile[this.row].Length + this.column + 1;
                        if (this.column < 0)
                        {
                            int newDelta = this.column;
                            this.column = 0;
                            this.UpdateColumn(newDelta, linesInFile);
                        }
                    }
                }
                else if (this.column > linesInFile[this.row].Length)
                {
                    if (this.row == linesInFile.Count - 1)
                    {
                        // last line
                        this.column = linesInFile[this.row].Length;
                    }
                    else
                    {
                        this.column -= linesInFile[this.row].Length;
                        this.row++;
                        rowChanged = true;
                        if (this.column > linesInFile[this.row].Length)
                        {
                            int newDelta = this.row;
                            this.row = 0;
                            this.UpdateRow(newDelta, linesInFile);
                        }
                    }
                }

                if (previousColumn != this.Column)
                {
                    NotifyPropertyChanged(nameof(this.Column));
                }
                if (rowChanged)
                {
                    NotifyPropertyChanged(nameof(this.Row));
                }
            }
        }

        public void UpdateRow(int delta, IList<string> linesInFile)
        {
            if (delta != 0)
            {
                if (this.row == 0 && delta < 0)
                {
                    this.UpdateColumnAbsolute(0, linesInFile);
                }
                else if (this.row == linesInFile.Count - 1 && delta > 0)
                {
                    this.UpdateColumnAbsolute(-1, linesInFile);
                }
                else
                {
                    this.row = Math.Max(0, Math.Min(this.row + delta, linesInFile.Count - 1));
                    if (this.column >= linesInFile[this.row].Length)
                    {
                        this.UpdateColumnAbsolute(-1, linesInFile);
                    }
                    NotifyPropertyChanged(nameof(this.Row));
                }
            }
        }

        /// <summary>
        /// Use -1 for end of line
        /// </summary>
        /// <param name="absoluteColumnIndex"></param>
        /// <param name="linesInFile"></param>
        public void UpdateColumnAbsolute(int absoluteColumnIndex, IList<string> linesInFile)
        {
            if (absoluteColumnIndex == -1)
            {
                absoluteColumnIndex = linesInFile[this.Row].Length;
                this.UpdateColumnAbsolute(absoluteColumnIndex, linesInFile);
            }
            this.UpdateColumn(absoluteColumnIndex - this.column, linesInFile);
        }

        public void UpdateRowAbsolute(int absoluteRowIndex, IList<string> linesInFile)
        {
            this.UpdateRow(absoluteRowIndex - this.row, linesInFile);
        }

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static IList<Cursor> RemoveDuplicateCursors(IList<Cursor> cursors)
        {
            return cursors
                .Distinct()
                .ToList();
        }

        public override bool Equals(object other)
        {
            if (this == other)
            {
                return true;
            }
            else if (!(other is Cursor))
            {
                return false;
            }
            Cursor otherCursor = (Cursor)other;
            return this.Row == otherCursor.Row &&
                this.Column == otherCursor.Column;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Column, this.Row);
        }
    }
}
