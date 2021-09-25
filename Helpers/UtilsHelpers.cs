using Newtonsoft.Json;

namespace HealthCheck.Helpers
{
    public static class UtilsHelpers
    {
        public static string GetConnectionString()
        {
            return "localhost:6379";
        }

        public static string ToJSON(this object @object) => JsonConvert.SerializeObject(@object, Formatting.None);
    }
}
