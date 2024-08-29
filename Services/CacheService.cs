using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;

namespace Library.Services
{
    public class CacheService
    {
        private readonly IConnectionMultiplexer _Redis;
        private readonly IDatabase _CacheDB;

        public CacheService(IConnectionMultiplexer redisMultiplexer)
        {
            _Redis = redisMultiplexer;
            _CacheDB = _Redis.GetDatabase();
        }

        public IConnectionMultiplexer Redis
        {
            get { return _Redis; }
        }

        public IDatabase CacheDB
        {
            get { return _CacheDB; }
        }

        public T? GetAlt<T>(String key)
        {
            var json = CacheDB.StringGet(key.Trim());

            if (!String.IsNullOrEmpty(json))
            {
                var jsonObject = JsonSerializer.Deserialize<Dictionary<String, JsonElement>>(json);

                if (jsonObject != null && jsonObject.ContainsKey("Value"))
                {
                    var valueJson = jsonObject["Value"].GetRawText();

                    return JsonSerializer.Deserialize<T>(valueJson);
                }
            }

            return default;
        }

        public T? Get<T>(String key)
        {
            var json = CacheDB.StringGet(key.Trim());

            if (!String.IsNullOrEmpty(json))
            {
                return JsonSerializer.Deserialize<T>(json);
            }

            return default;
        }

        public void Set<T>(String check, T value)
        {
            String key = $"book:{check}".Trim();

            CacheDB.StringSet(key.Trim(), JsonSerializer.Serialize(value));
        }

        public Boolean SetAlt<T>(String check, T value)
        {
            String key = $"book:{check}".Trim();

            var valueToCache = value is ActionResult<T> actionResult ? actionResult.Value : value;
            var isSet = CacheDB.StringSet(key, JsonSerializer.Serialize(valueToCache));

            return isSet;
        }

        public Boolean Remove(String key)
        {
            var exist = CacheDB.KeyExists(key.Trim());

            if (exist)
            {
                return CacheDB.KeyDelete(key);
            }

            return false;
        }
    }
}
