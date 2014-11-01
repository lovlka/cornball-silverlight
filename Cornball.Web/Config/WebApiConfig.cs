using System.Net.Http.Headers;
using System.Web.Http;

namespace Cornball.Web.Config
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "HighScores",
                routeTemplate: "api/{controller}/{limit}/{startDate}/{endDate}",
                defaults: new { startDate = RouteParameter.Optional, endDate = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "Default",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
        }
    }
}
