using Avalonia.Styling;
using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.Avalonia.Visuals.ThemingAndL10N
{
    public class StyleReference : Styles
    {
        public StyleReference()
        {
        }

        private IStyle _style = null;
        public IStyle TheStyle
        {
            get => _style;

            set
            {
                if (_style == value)
                    return;

                if (_style != null)
                {
                    this.Remove(_style);
                }

                _style = value;

                if (_style != null)
                {
                    this.Add(_style);
                }
            }
        }
    }
}
