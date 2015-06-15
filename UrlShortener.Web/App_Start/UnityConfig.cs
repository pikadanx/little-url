using System.Web.Http;
using Microsoft.Practices.Unity;
using UrlShortener.DataAccess;
using UrlShortener.Web.Controllers;

namespace UrlShortener.Web
{
    public class UnityConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var container = new UnityContainer();

            const string shortUrlDataStoreSingletonName = "ShortUrlDataStore-Singleton";
            const string shortUrlGeneratorSingletonName = "ShortUrlGenerator-Singleton";
            const string urlShortenerSingletonName = "UrlShortener-Singleton";
            const string shortUrlResolverSingletonName = "ShortUrlResolver-Singleton";

            container.RegisterType<IShortUrlDataStore, RedisShortUrlDataStore>(shortUrlDataStoreSingletonName,
                new ExternallyControlledLifetimeManager());
            container.RegisterType<IShortUrlGenerator, ShortUrlGenerator>(shortUrlGeneratorSingletonName,
                new ExternallyControlledLifetimeManager(),
                new InjectionConstructor(new ResolvedParameter<IShortUrlDataStore>(shortUrlDataStoreSingletonName)));
            container.RegisterType<IUrlShortener, UrlShortener>(urlShortenerSingletonName,
                new ExternallyControlledLifetimeManager(),
                new InjectionConstructor(new ResolvedParameter<IShortUrlDataStore>(shortUrlDataStoreSingletonName),
                    new ResolvedParameter<IShortUrlGenerator>(shortUrlGeneratorSingletonName)));
            container.RegisterType<IShortUrlResolver, ShortUrlResolver>(shortUrlResolverSingletonName,
                new ExternallyControlledLifetimeManager(),
                new InjectionConstructor(new ResolvedParameter<IShortUrlDataStore>(shortUrlDataStoreSingletonName)));
            container.RegisterType<LittleUrlController>(
                new InjectionConstructor(new ResolvedParameter<IUrlShortener>(urlShortenerSingletonName),
                    new ResolvedParameter<IShortUrlResolver>(shortUrlResolverSingletonName)));

            config.DependencyResolver = new UnityResolver(container);
        }
    }
}