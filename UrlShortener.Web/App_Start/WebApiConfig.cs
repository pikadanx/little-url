using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace UrlShortener.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            RemoveXmlSerialization(config);
            SetupJsonSerializerSettings(config.Formatters.JsonFormatter.SerializerSettings);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        private static void RemoveXmlSerialization(HttpConfiguration config)
        {
            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
        }

        private static void SetupJsonSerializerSettings(JsonSerializerSettings jsonSerializerSetttings)
        {
            jsonSerializerSetttings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonSerializerSetttings.NullValueHandling = NullValueHandling.Ignore;
        }
    }
}
