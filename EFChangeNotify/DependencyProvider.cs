using System.Data.SqlClient;
using System.Linq;

namespace EFChangeNotify
{
    public class DependencyProvider
    {
        public static void ReleaseAllDependency()
        {
            SafeCountDictionary.GetAllDependencieis().ToList().ForEach(c => SqlDependency.Stop(c));
        }
    }
}
