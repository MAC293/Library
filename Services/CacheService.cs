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

        public T? Get<T>(String key)
        {
            var json = CacheDB.StringGet(key);

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

        //public T? GetAlt<T>(String key)
        //{
        //    var json = CacheDB.StringGet(key);

        //    if (!String.IsNullOrEmpty(json))
        //    {
        //        return JsonSerializer.Deserialize<T>(json);
        //    }

        //    return default;
        //}

        public void Set<T>(String check, T value)
        {
            String key = $"book:{check}";

            CacheDB.StringSet(key, JsonSerializer.Serialize(value));
        }

        //public Boolean SetAlt<T>(String userID, String vehicle, T value)
        //{
        //    String key = $"user:{userID}:vehicle:{vehicle}";

        //    var valueToCache = value is ActionResult<T> actionResult ? actionResult.Value : value;
        //    var isSet = CacheDB.StringSet(key, JsonSerializer.Serialize(valueToCache));

        //    return isSet;
        //}

        //public void SetAlt1(String userID, String vehicle, List<VehicleService> value)
        //{
        //    String key = $"user:{userID}:vehicle:{vehicle}";
        //    CacheDB.StringSet(key, JsonSerializer.Serialize(value));
        //}
        
        public Boolean Remove(String key)
        {
            var exist = CacheDB.KeyExists(key);

            if (exist)
            {
                return CacheDB.KeyDelete(key);
            }

            return false;
        }
    }
}
