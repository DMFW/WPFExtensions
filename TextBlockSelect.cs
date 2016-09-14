using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WPFExtension
{
    public partial class TextBlockSelect : TextBlock
    {
        TextPointer StartSelectPosition;
        TextPointer EndSelectPosition;
        bool isDoubleClick;

        public String SelectedText = "";

        public delegate void TextSelectedHandler(string SelectedText);
        public event TextSelectedHandler TextSelected;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            Point mouseDownPoint = e.GetPosition(this);
            StartSelectPosition = this.GetPositionFromPoint(mouseDownPoint, true);

            isDoubleClick = false;
            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
            {
                isDoubleClick = true;
                SelectWord(StartSelectPosition);
                HighlightSelection();
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {

            if (isDoubleClick) { return; }

            base.OnMouseUp(e);
            Point mouseUpPoint = e.GetPosition(this);
            EndSelectPosition = this.GetPositionFromPoint(mouseUpPoint, true);
            HighlightSelection();

        }

        private void SelectWord(TextPointer ClickPosition)
        {
            String stringBeforeCaret = ClickPosition.GetTextInRun(LogicalDirection.Backward);   // extract the text in the current run from the caret to the left
            String stringAfterCaret = ClickPosition.GetTextInRun(LogicalDirection.Forward);     // extract the text in the current run from the caret to the left

            Int32 countToMoveLeft = 0;  // we record how many positions we move to the left until a non-letter character is found
            Int32 countToMoveRight = 0; // we record how many positions we move to the right until a non-letter character is found

            for (Int32 i = stringBeforeCaret.Length - 1; i >= 0; --i)
            {
                // if the character at the location CaretPosition-LeftOffset is a letter, we move more to the left
                if (Char.IsLetter(stringBeforeCaret[i]))
                    ++countToMoveLeft;
                else break; // otherwise we have found the beginning of the word
            }

            for (Int32 i = 0; i < stringAfterCaret.Length; ++i)
            {
                // if the character at the location CaretPosition+RightOffset is a letter, we move more to the right
                if (Char.IsLetter(stringAfterCaret[i]))
                    ++countToMoveRight;
                else break; // otherwise we have found the end of the word
            }

            StartSelectPosition = ClickPosition.GetPositionAtOffset(-countToMoveLeft);    // modify the start pointer by the offset we have calculated
            EndSelectPosition = ClickPosition.GetPositionAtOffset(countToMoveRight);      // modify the end pointer by the offset we have calculated

        }

        private void HighlightSelection()
        {
            TextRange otr = new TextRange(this.ContentStart, this.ContentEnd);
            otr.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Black));
            otr.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.White));

            TextRange ntr = new TextRange(StartSelectPosition, EndSelectPosition);
            ntr.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.White));
            ntr.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.Black));

            SelectedText = ntr.Text;
            if (!(TextSelected == null))
            {
                TextSelected(SelectedText);
            }
        }
    }
}
