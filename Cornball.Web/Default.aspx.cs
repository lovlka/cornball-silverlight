using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using Cornball.Web.Resources;

namespace Cornball.Web
{
    public partial class Default : Page
    {
        protected string Version
        {
            get
            {
                var assembly = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
                return assembly.Version.ToString();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.UserLanguages != null)
            {
                string language = Request.UserLanguages.FirstOrDefault();
                if (language != null)
                {
                    try
                    {
                        Strings.Culture = CultureInfo.GetCultureInfo(language);
                    }
                    catch (ArgumentException)
                    {
                    }
                }
            }
        }
    }
}