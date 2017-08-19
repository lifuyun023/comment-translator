﻿using System;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;
using System.Windows;

namespace CommentTranslator.Presentation
{
    /// <summary>
    /// Adornment class that draws a square box in the top right hand corner of the viewport
    /// </summary>
    internal sealed class TranslatePopupAdornment
    {
        private readonly ITextBuffer _buffer;
        private readonly IWpfTextView _view;
        private readonly IAdornmentLayer _layer;
        private TranslatePopup _popup;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslatePopupAdornment"/> class.
        /// Creates a square image and attaches an event handler to the layout changed event that
        /// adds the the square in the upper right-hand corner of the TextView via the adornment layer
        /// </summary>
        /// <param name="view">The <see cref="IWpfTextView"/> upon which the adornment will be drawn</param>
        public TranslatePopupAdornment(IWpfTextView view)
        {
            this._view = view ?? throw new ArgumentNullException("view");
            this._layer = view.GetAdornmentLayer("TransplatePopupAdornment");
            this._buffer = _view.TextBuffer;

            this._view.LayoutChanged += this.OnLayoutChanged;
            this._view.Closed += Closed;
        }

        /// <summary>
        /// Creates the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns></returns>
        public static TranslatePopupAdornment Create(IWpfTextView view)
        {
            return view.Properties.GetOrCreateSingletonProperty(() => new TranslatePopupAdornment(view));
        }

        /// <summary>
        /// Translates the specified text.
        /// </summary>
        /// <param name="span">The span.</param>
        /// <param name="text">The text.</param>
        public void Translate(SnapshotSpan span, string text)
        {
            //Get viewport size
            var viewportSize = new Size(_view.ViewportWidth, _view.ViewportHeight);

            //Close popup if exist
            if (_popup != null)
            {
                _popup.Close();
            }

            //Create new popup
            _popup = CreatePopup(span, text, viewportSize);

            //Attach popup
            AttachPopup(_popup);

            //Focus
            _popup.Focus();
        }

        /// <summary>
        /// Creates the popup.
        /// </summary>
        /// <param name="span">The span.</param>
        /// <param name="text">The text.</param>
        /// <param name="viewportSize">Size of the viewport.</param>
        /// <returns></returns>
        private TranslatePopup CreatePopup(SnapshotSpan span, string text, Size viewportSize)
        {
            var popup = new TranslatePopup(span, text, viewportSize);

            AttachEventPopup(popup);
            return popup;
        }

        /// <summary>
        /// Attaches the event popup.
        /// </summary>
        /// <param name="popup">The popup.</param>
        private void AttachEventPopup(TranslatePopup popup)
        {
            popup.OnClosed += PopupClosed;
        }

        /// <summary>
        /// Detaches the event popup.
        /// </summary>
        /// <param name="popup">The popup.</param>
        private void DetachEventPopup(TranslatePopup popup)
        {
            popup.OnClosed -= PopupClosed;
        }

        /// <summary>
        /// Attaches the popup.
        /// </summary>
        /// <param name="popup">The popup.</param>
        private void AttachPopup(TranslatePopup popup)
        {
            var span = popup.Span.GetSpan(_view.TextSnapshot);
            var g = _view.TextViewLines.GetMarkerGeometry(span);

            if (g != null)
            {
                popup.Margin = new Thickness(g.Bounds.BottomLeft.X, g.Bounds.BottomLeft.Y, 0, 0);
                _layer.AddAdornment(span, null, popup);
            }
        }

        private void PopupClosed(object sender, EventArgs e)
        {
            if (sender is TranslatePopup popup)
            {
                _layer.RemoveAdornment(popup);
                DetachEventPopup(popup);
            }
        }

        /// <summary>
        /// Closeds the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <exception cref="NotImplementedException"></exception>
        private void Closed(object sender, EventArgs e)
        {
            if (_popup != null)
            {
                _popup.Close();
            }
        }

        /// <summary>
        /// Event handler for viewport layout changed event. Adds adornment at the top right corner of the viewport.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void OnLayoutChanged(object sender, EventArgs e)
        {
        }
    }
}
