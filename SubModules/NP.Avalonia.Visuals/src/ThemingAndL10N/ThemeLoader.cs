using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;
using Avalonia.Styling;
using NP.Concepts.Behaviors;
using NP.Utilities;
using NP.ViewModelInterfaces.ThemingAndL10N;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace NP.Avalonia.Visuals.ThemingAndL10N
{
    public class ThemeLoader : IThemeLoader, IResourceProvider
    {
        public string? Name { get; set; }

        public event Action<IThemeLoader> SelectedThemeChangedEvent;

        private ResourceDictionary _resourceDictionary = new ResourceDictionary();

        public IResourceDictionary Loaded => _resourceDictionary;
        public IResourceHost? Owner => Loaded.Owner;

        void IResourceProvider.AddOwner(IResourceHost owner) => Loaded.AddOwner(owner);
        void IResourceProvider.RemoveOwner(IResourceHost owner) => Loaded.RemoveOwner(owner);

        public Uri? BaseUri { get; private set; }

        public Styles TheStyles { get; } = new Styles();

        public ThemeLoader ProvideValue(IServiceProvider serviceProvider)
        {
            BaseUri = ((IUriContext)serviceProvider.GetService(typeof(IUriContext))).BaseUri;

            TryLoadAllThemes();

            return this;
        }

        /// <summary>
        /// Gets or sets the source URL.
        /// </summary>
        public Uri? Source { get; set; }

        object _styleResourceName;
        public object StyleResourceName 
        {
            get => _styleResourceName; 
            set
            {
                if (_styleResourceName.ObjEquals(value))
                    return;

                _styleResourceName = value;
            }
        }

        public bool HasResources => Loaded.HasResources;

        public bool TryGetResource(object key, out object? value)
        {
            if (key.ObjEquals(_styleResourceName))
            {
                value = TheStyles;
                return true;
            }

            return Loaded.TryGetResource(key, out value);
        }

        public T GetResource<T>(object key, T defaultValue = default)
        {
            if (TryGetResource(key, out object result))
            {
                return (T) result;
            }

            return defaultValue;
        }

        public event EventHandler OwnerChanged
        {
            add => Loaded.OwnerChanged += value;
            remove => Loaded.OwnerChanged -= value;
        }

        [Content]
        public ObservableCollection<ThemeInfo> Themes { get; } =
            new ObservableCollection<ThemeInfo>();

        public IThemeInfo? TheSelectedTheme => SelectedTheme;

        private ThemeInfo? _selectedTheme;
        public ThemeInfo? SelectedTheme
        {
            get => _selectedTheme; 
            private set
            {
                if (_selectedTheme == value)
                    return;

                if (_selectedTheme?.Resource != null)
                {
                    _resourceDictionary.MergedDictionaries.Remove(_selectedTheme.Resource);
                }

                if (_selectedTheme?.Style != null)
                {
                    this.TheStyles.Remove(_selectedTheme.Style);
                }

                _selectedTheme = value;

                SetSelectedResourceAndStyle();

                SelectedThemeChangedEvent?.Invoke(this);
            }
        }

        private void SetSelectedResourceAndStyle()
        {
            if (_selectedTheme?.Resource != null)
            {
                _resourceDictionary.MergedDictionaries.Add(_selectedTheme.Resource);
            }

            if (_selectedTheme?.Style != null)
            {
                this.TheStyles.Add(_selectedTheme.Style);
            }
        }

        public void AddDictionary(object themeId, Uri resourcesUri, Uri styleUri = null)
        {
            ThemeInfo themeInfo = new ThemeInfo() { Id = themeId };

            if (resourcesUri != null)
            {
                themeInfo.ResourceUrl = resourcesUri;
            }

            if (styleUri != null)
            {
                themeInfo.StyleUrl = styleUri;
            }

            Themes.Add(themeInfo);
        }

        private object? _selectedThemeId;
        public object? SelectedThemeId
        {
            get => _selectedThemeId;

            set
            {
                if (_selectedThemeId == value)
                {
                    return;
                }

                _selectedThemeId = value;

                SelectThemeImpl(_selectedThemeId!);
            }
        }

        private void SelectThemeImpl(object themeId)
        {
            SelectedTheme = Themes.FirstOrDefault(theme => theme.Id.Equals(themeId));
        }

        private IDisposable? _subscription;
        public ThemeLoader()
        {
            _subscription =
                Themes.AddBehavior(OnThemeAdded, OnThemeRemoved);
        }

        /// <summary>
        /// to be used when the ThemeLoader is not instantiated in XAML.
        /// </summary>
        /// <param name="baseUri"></param>
        public ThemeLoader(Uri baseUri) : this()
        {
            BaseUri = baseUri;
        }

        private void TryLoadAllThemes()
        {
            foreach(var themeInfo in Themes)
            {
                themeInfo.TryLoad(BaseUri);

                if (themeInfo.Id == SelectedThemeId)
                {
                    SetSelectedResourceAndStyle();
                }
            }
        }

        private void OnThemeAdded(ThemeInfo themeInfo)
        {
            if (themeInfo.Id == null)
            {
                throw new Exception("ERROR: theme id cannot be null");
            }

            themeInfo.TryLoad(BaseUri);

            if (SelectedThemeId != null && themeInfo.Id == SelectedThemeId)
            {
                SelectThemeImpl(SelectedThemeId);
            }
        }

        private void OnThemeRemoved(ThemeInfo themeInfo)
        {

        }

        // good for testing of switching when there are only two options
        public void SwitchTheme()
        {
            ThemeInfo? currentTheme = this.SelectedTheme;
            int currentThemeIdx = currentTheme == null ? -1 : this.Themes.IndexOf(currentTheme);
            if(currentThemeIdx == this.Themes.Count - 1)
            {
                currentThemeIdx = -1;
            }

            int nextThemeIdx = currentThemeIdx + 1;

            this.SelectedThemeId = Themes.ElementAt(nextThemeIdx).Id;
        }
    }
}
