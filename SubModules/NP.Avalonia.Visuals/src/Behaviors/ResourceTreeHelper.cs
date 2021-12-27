using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using NP.Avalonia.Visuals.ThemingAndL10N;
using NP.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace NP.Avalonia.Visuals.Behaviors
{
    public static class ResourceTreeHelper
    {
        private static IEnumerable<IResourceProvider> GetChildDictionaries(this IResourceProvider dict)
        {
            if (dict is ResourceDictionary resourceDictionary)
            {
                return resourceDictionary.MergedDictionaries;
            }
            else if (dict is ResourceInclude resourceInclude)
            {
                return resourceInclude.Loaded.ToCollection();
            }
            else if (dict is ThemeLoader themeLoader)
            {
                return themeLoader.Loaded.ToCollection();
            }

            return Enumerable.Empty<IResourceProvider>();
        }

        public static IEnumerable<IResourceProvider> GetSelfAndResourceDescendants(this IResourceProvider resourceProvider)
        {
            return TreeUtils.SelfAndDescendants(resourceProvider, GetChildDictionaries);
        }

        public static IEnumerable<IResourceProvider> GetResourceDescendants(this IResourceProvider resourceProvider)
        {
            return TreeUtils.Descendants(resourceProvider, GetChildDictionaries);
        }

        public static ThemeLoader? GetThemeLoader(this IResourceProvider resourceProvider, string name = null)
        {
            return resourceProvider
                        .GetSelfAndResourceDescendants()
                        .OfType<ThemeLoader>()
                        .FirstOrDefault(tl => (name == null) || (tl.Name == name));
        }
    }
}
