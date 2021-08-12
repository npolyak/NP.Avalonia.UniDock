// (c) Nick Polyak 2021 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.
//
using NP.Utilities;
using System.ComponentModel;

namespace NP.Demos.MultiPlatformWindowDemo
{
    public class CustomWindowViewModel : VMBase
    {
        #region TheText Property
        private string? _text;
        public string? TheText
        {
            get
            {
                return this._text;
            }
            set
            {
                if (this._text == value)
                {
                    return;
                }

                this._text = value;
                this.OnPropertyChanged(nameof(TheText));
            }
        }
        #endregion TheText Property
    }
}
