namespace FluentSearch.Core
{
    public class FluentDefaults<T>
    {

        public FluentConfig<T> DefaultConfig()
        {
            var defaultConfig = new FluentConfig<T>
            {
                ResultMapper = FluentMapper<T>.ReflectMap,
                Key = "Id"
            };

            return defaultConfig;
        }

    }
}
