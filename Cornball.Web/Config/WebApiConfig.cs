using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Cornball.Web.Config
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            config.Routes.MapHttpRoute(
                name: "HighScore",
                routeTemplate: "api/{controller}/{limit}/{score}"
            );

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
