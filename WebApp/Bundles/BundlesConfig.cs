using System.Web.Optimization;
using WebApp.Bundles.Layout;

namespace WebApp.Bundles
{
    public static class BundlesConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            LayoutBundle.GetInstance().RegisterLayout(bundles);
        }
    }
}
