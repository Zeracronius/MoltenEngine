using Molten.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.UI
{
    public class UITextbox : UIComponent
    {

        /// <summary>
        /// Occurs when <see cref="Text"/> or <see cref="Font"/> are changed.
        /// </summary>
        public event ObjectHandler<UITextbox> OnTextChanged;

        string _text;
        SpriteFont _font;
        UIHorizontalAlignment _hAlign;
        UIVerticalAlignment _vAlign;
        Vector2F _pos;
        Vector2I _textSize;
        Color _color;
        Input.IKeyboardDevice _keyboard;
        Input.Key? _currentKey;
        Input.Key? _previousKey;
        double _delay;
        int _caretPosition;
        bool _isFocused;
        int _repeatDelay;
        Vector2F _caretDelta;

        public UITextbox()
        {
            _font = Engine.Current.DefaultFont;
            _hAlign = UIHorizontalAlignment.Left;
            _vAlign = UIVerticalAlignment.Bottom;
            _repeatDelay = 33;
            _color = Color.White;

            //OnFocus += UITextbox_OnFocus;
            //OnUnfocus += UITextbox_OnUnfocus;
            OnEnter += UITextbox_OnEnter;
            OnLeave += UITextbox_OnLeave;
        }


        public UITextbox(SpriteFont font, string text = "")
            : this()
        {
            _text = text;
            _font = font;
            _textSize = (Vector2I)_font.MeasureString(_text);
            AlignText();
        }


        private void UITextbox_OnEnter(SceneEventData<Input.MouseButton> data)
        {
            //TODO Set cursor to caret icon.
        }

        private void UITextbox_OnLeave(SceneEventData<Input.MouseButton> data)
        {
            //TODO Set cursor back to default.
        }

        protected override void OnInitialize(SceneObject obj)
        {
            _keyboard = Engine.Current.Input.GetKeyboard();
            _keyboard.OnKeyPressed += keyboard_OnKeyPressed;
            _keyboard.OnKeyReleased += keyboard_OnKeyReleased;
            base.OnInitialize(obj);
        }

        private void UITextbox_OnUnfocus(SceneEventData<Input.MouseButton> data)
        {
            _isFocused = false;
            _currentKey = null;
            _delay = 0;
        }

        private void UITextbox_OnFocus(SceneEventData<Input.MouseButton> data)
        {
            _isFocused = true;
            Vector2F cursorDelta = data.Position - _pos;
            _caretPosition = _font.NearestCharacter(_text, cursorDelta);
            CalculateCaretDeltaPosition();
        }


        private void AlignText()
        {
            Rectangle cBounds = ClippingBounds;

            switch (_hAlign)
            {
                case UIHorizontalAlignment.Left:
                    _pos.X = cBounds.X;
                    break;

                case UIHorizontalAlignment.Center:
                    _pos.X = cBounds.Center.X - (_textSize.X / 2);
                    break;

                case UIHorizontalAlignment.Right:
                    _pos.X = cBounds.Right - _textSize.X;
                    break;
            }

            switch (_vAlign)
            {
                case UIVerticalAlignment.Top:
                    _pos.Y = cBounds.Y;
                    break;

                case UIVerticalAlignment.Center:
                    _pos.Y = cBounds.Center.Y - (_textSize.Y / 2);
                    break;

                case UIVerticalAlignment.Bottom:
                    _pos.Y = cBounds.Bottom - _textSize.Y;
                    break;
            }
        }

        protected override void OnPostUpdateBounds()
        {
            base.OnPostUpdateBounds();
            AlignText();
        }

        public override void OnRenderUi(SpriteBatcher sb)
        {
            base.OnRenderUi(sb);

            if (_color.A > 0)
                sb.DrawString(_font, _text, _pos, _color);

            if (_isFocused)
                sb.DrawLine(new Vector2F(_pos.X + _caretDelta.X, _pos.Y), _pos + _caretDelta, _color, 0.1F);
        }


        public override void OnUpdateUi(Timing time)
        {
            if (_isFocused && _currentKey.HasValue)
            {
                if (_currentKey == _previousKey)
                {
                    _delay += time.ElapsedTime.TotalMilliseconds;
                    if (_delay < RepeatDelay)
                        return;
                }

                switch (_currentKey)
                {
                    case Input.Key.Delete:
                        Text = Text.Remove(_caretPosition, 1);
                        break;

                    case Input.Key.Back:
                        Text = Text.Remove(_caretPosition - 1, 1);
                        break;

                    case Input.Key.Left:
                        _caretPosition = Math.Max(0, _caretPosition - 1);
                        CalculateCaretDeltaPosition();
                        break;

                    case Input.Key.Right:
                        _caretPosition = Math.Min(Text.Length, _caretPosition + 1);
                        CalculateCaretDeltaPosition();
                        break;

                    default:
                        char character = (char)_currentKey.Value;
                        if (Char.IsLetterOrDigit(character))
                        {
                            Text = Text.Insert(_caretPosition, character.ToString());
                            _caretPosition = Text.Length;
                            CalculateCaretDeltaPosition();
                        }
                        break;
                }

                _delay = 0;
                _previousKey = _currentKey;
            }
        }

        private void keyboard_OnKeyReleased(Input.IKeyboardDevice device, Input.Key key)
        {
            // Only reset if the released key is the last one to be pressed.
            if (_isFocused && _currentKey == key)
                _currentKey = null;
        }

        private void keyboard_OnKeyPressed(Input.IKeyboardDevice device, Input.Key key)
        {
            if (_isFocused)
                _currentKey = key;
        }

        private void CalculateCaretDeltaPosition()
        {
            _caretDelta = _font.MeasureString(_text.Substring(0, _caretPosition));
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                _textSize = (Vector2I)_font.MeasureString(_text);
                Width = _textSize.X;
                Height = _textSize.Y;
                AlignText();
                OnTextChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// Gets or sets the font used when rendering the text.
        /// </summary>
        public SpriteFont Font
        {
            get => _font;
            set
            {
                if (_font != value)
                {
                    _font = value;
                    _textSize = (Vector2I)_font.MeasureString(_text);
                    AlignText();
                    OnTextChanged?.Invoke(this);
                }
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment of the text, within it's bounds.
        /// </summary>
        public UIHorizontalAlignment HorizontalAlignment
        {
            get => _hAlign;
            set
            {
                _hAlign = value;
                AlignText();
            }
        }

        /// <summary>
        /// Gets or sets the vertical alignment of the text, within it's bounds.
        /// </summary>
        public UIVerticalAlignment VerticalAlignment
        {
            get => _vAlign;
            set
            {
                _vAlign = value;
                AlignText();
            }
        }

        /// <summary>
        /// Gets or sets the text color.
        /// </summary>
        public Color TextColor
        {
            get => _color;
            set => _color = value;
        }

        /// <summary>
        /// Gets the size of the label text based on it's current font, in pixels.
        /// </summary>
        public Vector2I Size
        {
            get => _textSize;
        }


        public bool IsFocused
        {
            get => _isFocused;
            set => _isFocused = value;
        }

        /// <summary>
        /// <para>
        /// Delay in miliseconds between repeating held key. 33 by default.
        /// </para>
        /// </summary>
        public int RepeatDelay
        {
            get => _repeatDelay;
            set => _repeatDelay = value;
        }
    }
}
