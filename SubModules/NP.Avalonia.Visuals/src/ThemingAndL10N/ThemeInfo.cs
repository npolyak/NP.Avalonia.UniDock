using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using NP.ViewModelInterfaces.ThemingAndL10N;
using System;

namespace NP.Avalonia.Visuals.ThemingAndL10N
{
    public class ThemeInfo: IThemeInfo
    {
        public object Id { get; set; }

        public Uri ResourceUrl { get; set; }

        internal IResourceProvider Resource { get; set;}

        public Uri StyleUrl { get; set; }

        internal IStyle Style { get; set; }

        internal void TryLoad(Uri baseUrl)
        {
            if (baseUrl == null)
            {
                return;
            }
            if (ResourceUrl != null)
            {
                Resource =
                    (ResourceDictionary)AvaloniaXamlLoader.Load(ResourceUrl, baseUrl);
            }

            if (StyleUrl != null)
            {
                Style = (IStyle)AvaloniaXamlLoader.Load(StyleUrl, baseUrl);
            }
        }
    }
}
