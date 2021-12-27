using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using System.Linq;
using System.Reactive.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NP.Avalonia.Visuals.MarkupExtensions
{
    public class DynamicResourceBinding : Binding, IBinding
	{
        public DynamicResourceBinding()
        {

		}

		public DynamicResourceBinding(string path) => Path = path;

		public static Type ResolveType(IServiceProvider ctx, string namespacePrefix, string type)
		{
			var tr = (IXamlTypeResolver) ctx.GetService(typeof(IXamlTypeResolver));
			string name = string.IsNullOrEmpty(namespacePrefix) ? type : $"{namespacePrefix}:{type}";
			return tr?.Resolve(name);
		}

		public IBinding ProvideValue(IServiceProvider serviceProvider)
        {
			var descriptorContext = (ITypeDescriptorContext)serviceProvider;

			TypeResolver = (namespacePrefix, type) => ResolveType(descriptorContext, namespacePrefix, type);

			NameScope = new WeakReference<INameScope>((INameScope)serviceProvider.GetService(typeof(INameScope)));
			return this;
		}

		Dictionary<IDisposable, object> dict = new Dictionary<IDisposable, object>();

		public new InstancedBinding Initiate(
			IAvaloniaObject target,
			AvaloniaProperty targetProperty,
			object? anchor = null,
			bool enableDataValidation = false)
		{
			var target1 = target as IResourceHost ?? anchor;
			if (target1 is IResourceHost resourceHost)
			{
				var instBinding = base.Initiate(target, targetProperty, anchor, enableDataValidation);

				return 
					InstancedBinding.OneWay
					(
						instBinding.Observable
							.Select(value => value != null ? resourceHost.GetResourceObservable(value.ToString()) : Observable.Never<object>())
							.Switch());
			}

			return null;
		}
    }
}
