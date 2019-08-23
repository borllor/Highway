#if NET40
using ServiceStack.Model;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using ServiceStack.Redis.Pipeline;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;

namespace JinRi.Notify.Frame
{
    public class RedisCache
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(RedisCache));
        private const string CachePrefix = "JinRi_";

        #region 单例对象实现

        private static readonly IRedisProvider m_providerInstance = new RedisProvider();

        public static IRedisProvider GetProvider()
        {          
            return m_providerInstance;
        }

        #endregion

        #region CacheKey处理方法

        public static string CleanCacheKey(string CacheKey)
        {
            if (String.IsNullOrEmpty(CacheKey))
            {
                throw new ArgumentException("CacheKey不能为空", "CacheKey");
            }
            return CacheKey.Substring(CachePrefix.Length);
        }

        public static string GetCacheKey(string CacheKey)
        {
            if (string.IsNullOrEmpty(CacheKey))
            {
                throw new ArgumentException("CacheKey不能为空", "CacheKey");
            }
            return CachePrefix + CacheKey;
        }

        public static List<string> CleanCacheKey(List<string> CacheKey)
        {
            if (CacheKey == null || CacheKey.Count == 0)
            {
                throw new ArgumentException("CacheKey不能为空", "CacheKey");
            }
            for (int i = 0; i < CacheKey.Count;i++ )
            {
                CacheKey[i] = CacheKey[i].Substring(CachePrefix.Length);
            }
            return CacheKey;
        }

        public static List<string> GetCacheKey(List<string> CacheKey)
        {
            if (CacheKey == null || CacheKey.Count == 0)
            {
                throw new ArgumentException("CacheKey不能为空", "CacheKey");
            }
            for (int i = 0; i < CacheKey.Count; i++)
            {
                CacheKey[i] = CachePrefix + CacheKey[i];
            }
            return CacheKey;
        }

        public static string[] CleanCacheKey(string[] CacheKey)
        {
            if (CacheKey == null || CacheKey.Length == 0)
            {
                throw new ArgumentException("CacheKey不能为空", "CacheKey");
            }
            for (int i = 0; i < CacheKey.Length;i++ )
            {
                CacheKey[i] = CacheKey[i].Substring(CachePrefix.Length);
            }
            return CacheKey;
        }

        public static string[] GetCacheKey(string[] CacheKey)
        {
            if (CacheKey == null || CacheKey.Length == 0)
            {
                throw new ArgumentException("CacheKey不能为空", "CacheKey");
            }
            for (int i = 0; i < CacheKey.Length; i++)
            {
                CacheKey[i] = CachePrefix + CacheKey[i];
            }
            return CacheKey;
        }

        #endregion

        #region ICacheProvider 成员

        public static bool Add(string key, object value)
        {
            return GetProvider().Add(GetCacheKey(key), value);
        }

        public static bool Add(string key, object value, DateTime expiry)
        {
            return GetProvider().Add(GetCacheKey(key), value, expiry);
        }

        public static object Get(string key)
        {
            return GetProvider().Get(GetCacheKey(key));
        }

        public static bool Delete(string key)
        {
            return GetProvider().Delete(GetCacheKey(key));
        }

        public static bool KeyExists(string key)
        {
            return GetProvider().KeyExists(GetCacheKey(key));
        }

        public static bool Set(string key, object value)
        {
            return GetProvider().Set(GetCacheKey(key), value);
        }

        public static bool Set(string key, object value, DateTime expiry)
        {
            return GetProvider().Set(GetCacheKey(key), value, expiry);
        }

        public static IDictionaryEnumerator GetEnumerator()
        {
            return GetProvider().GetEnumerator();
        }

        #endregion

        #region IRedisClient

        public static int ConnectTimeout { get { return GetProvider().ConnectTimeout; } set { GetProvider().ConnectTimeout = value; } }
        public static long Db { get { return GetProvider().Db; } set { GetProvider().Db = value; } }
        public static long DbSize { get { return GetProvider().Db; } }
        public static bool HadExceptions { get { return GetProvider().HadExceptions; } }
        public static IHasNamed<IRedisHash> Hashes { get { return GetProvider().Hashes; } set { GetProvider().Hashes = value; } }
        public static string Host { get { return GetProvider().Host; } }
        public static Dictionary<string, string> Info { get { return GetProvider().Info; } }
        public static DateTime LastSave { get { return GetProvider().LastSave; } }
        public static IHasNamed<IRedisList> Lists { get { return GetProvider().Lists; } set { GetProvider().Lists = value; } }
        public static string Password { get { return GetProvider().Password; } set { GetProvider().Password = value; } }
        public static int Port { get { return GetProvider().Port; } }
        public static int RetryCount { get { return GetProvider().RetryCount; } set { GetProvider().RetryCount = value; } }
        public static int RetryTimeout { get { return GetProvider().RetryTimeout; } set { GetProvider().RetryTimeout = value; } }
        public static int SendTimeout { get { return GetProvider().SendTimeout; } set { GetProvider().SendTimeout = value; } }
        public static IHasNamed<IRedisSet> Sets { get { return GetProvider().Sets; } set { GetProvider().Sets = value; } }
        public static IHasNamed<IRedisSortedSet> SortedSets { get { return GetProvider().SortedSets; } set { GetProvider().SortedSets = value; } }

        public string this[string key] { get { return GetProvider()[GetCacheKey(key)]; } set { GetProvider()[GetCacheKey(key)] = value; } }

        public static IDisposable AcquireLock(string key) { return GetProvider().AcquireLock(GetCacheKey(key)); }
        public static IDisposable AcquireLock(string key, TimeSpan timeOut) { return GetProvider().AcquireLock(GetCacheKey(key), timeOut); }
        public static void AddItemToList(string listId, string value) { GetProvider().AddItemToList(GetCacheKey(listId), value); }
        public static void AddItemToSet(string setId, string item) { GetProvider().AddItemToSet(GetCacheKey(setId), item); }
        public static bool AddItemToSortedSet(string setId, string value) { return GetProvider().AddItemToSortedSet(GetCacheKey(setId), value); }
        public static bool AddItemToSortedSet(string setId, string value, double score) { return GetProvider().AddItemToSortedSet(GetCacheKey(setId), value, score); }
        public static void AddRangeToList(string listId, List<string> values) { GetProvider().AddRangeToList(GetCacheKey(listId), values); }
        public static void AddRangeToSet(string setId, List<string> items) { GetProvider().AddRangeToSet(GetCacheKey(setId), items); }
        public static bool AddRangeToSortedSet(string setId, List<string> values, double score) { return GetProvider().AddRangeToSortedSet(GetCacheKey(setId), values, score); }
        public static bool AddRangeToSortedSet(string setId, List<string> values, long score) { return GetProvider().AddRangeToSortedSet(GetCacheKey(setId), values, score); }
        public static long AppendToValue(string key, string value) { return GetProvider().AppendToValue(GetCacheKey(key), value); }
        public static IRedisTypedClient<T> As<T>() { return GetProvider().As<T>(); }
        public static string BlockingDequeueItemFromList(string listId, TimeSpan? timeOut) { return GetProvider().BlockingDequeueItemFromList(GetCacheKey(listId), timeOut); }
        public static ItemRef BlockingDequeueItemFromLists(string[] listIds, TimeSpan? timeOut) { return GetProvider().BlockingDequeueItemFromLists(GetCacheKey(listIds), timeOut); }
        public static string BlockingPopAndPushItemBetweenLists(string fromListId, string toListId, TimeSpan? timeOut) { return GetProvider().BlockingPopAndPushItemBetweenLists(GetCacheKey(fromListId), GetCacheKey(toListId), timeOut); }
        public static string BlockingPopItemFromList(string listId, TimeSpan? timeOut) { return GetProvider().BlockingPopItemFromList(GetCacheKey(listId), timeOut); }
        public static ItemRef BlockingPopItemFromLists(string[] listIds, TimeSpan? timeOut) { return GetProvider().BlockingPopItemFromLists(GetCacheKey(listIds), timeOut); }
        public static string BlockingRemoveStartFromList(string listId, TimeSpan? timeOut) { return GetProvider().BlockingRemoveStartFromList(GetCacheKey(listId), timeOut); }
        public static ItemRef BlockingRemoveStartFromLists(string[] listIds, TimeSpan? timeOut) { return GetProvider().BlockingRemoveStartFromLists(GetCacheKey(listIds), timeOut); }
        public static string CalculateSha1(string luaBody) { return GetProvider().CalculateSha1(luaBody); }
        public static bool ContainsKey(string key) { return GetProvider().ContainsKey(GetCacheKey(key)); }
        public static IRedisPipeline CreatePipeline() { return GetProvider().CreatePipeline(); }
        public static IRedisSubscription CreateSubscription() { return GetProvider().CreateSubscription(); }
        public static IRedisTransaction CreateTransaction() { return GetProvider().CreateTransaction(); }
        public static long DecrementValue(string key) { return GetProvider().DecrementValue(GetCacheKey(key)); }
        public static long DecrementValueBy(string key, int count) { return GetProvider().DecrementValueBy(GetCacheKey(key), count); }
        public static string DequeueItemFromList(string listId) { return GetProvider().DequeueItemFromList(GetCacheKey(listId)); }
        public static void EnqueueItemOnList(string listId, string value) { GetProvider().EnqueueItemOnList(GetCacheKey(listId), value); }
        public static long ExecLuaAsInt(string luaBody, params string[] args) { return GetProvider().ExecLuaAsInt(luaBody, args); }
        public static long ExecLuaAsInt(string luaBody, string[] keys, string[] args) { return GetProvider().ExecLuaAsInt(luaBody, keys, args); }
        public static List<string> ExecLuaAsList(string luaBody, params string[] args) { return GetProvider().ExecLuaAsList(luaBody, args); }
        public static List<string> ExecLuaAsList(string luaBody, string[] keys, string[] args) { return GetProvider().ExecLuaAsList(luaBody, keys, args); }
        public static string ExecLuaAsString(string luaBody, params string[] args) { return GetProvider().ExecLuaAsString(luaBody, args); }
        public static string ExecLuaAsString(string luaBody, string[] keys, string[] args) { return GetProvider().ExecLuaAsString(luaBody, keys, args); }
        public static long ExecLuaShaAsInt(string sha1, params string[] args) { return GetProvider().ExecLuaShaAsInt(sha1, args); }
        public static long ExecLuaShaAsInt(string sha1, string[] keys, string[] args) { return GetProvider().ExecLuaShaAsInt(sha1, keys, args); }
        public static List<string> ExecLuaShaAsList(string sha1, params string[] args) { return GetProvider().ExecLuaShaAsList(sha1, args); }
        public static List<string> ExecLuaShaAsList(string sha1, string[] keys, string[] args) { return GetProvider().ExecLuaShaAsList(sha1, keys, args); }
        public static string ExecLuaShaAsString(string sha1, params string[] args) { return GetProvider().ExecLuaShaAsString(sha1, args); }
        public static string ExecLuaShaAsString(string sha1, string[] keys, string[] args) { return GetProvider().ExecLuaShaAsString(sha1, keys, args); }
        public static bool ExpireEntryAt(string key, DateTime expireAt) { return GetProvider().ExpireEntryAt(GetCacheKey(key), expireAt); }
        public static bool ExpireEntryIn(string key, TimeSpan expireIn) { return GetProvider().ExpireEntryIn(GetCacheKey(key), expireIn); }
        public static void FlushDb() { GetProvider().FlushDb(); }
        public static Dictionary<string, string> GetAllEntriesFromHash(string hashId) { return GetProvider().GetAllEntriesFromHash(GetCacheKey(hashId)); }
        public static List<string> GetAllItemsFromList(string listId) { return GetProvider().GetAllItemsFromList(GetCacheKey(listId)); }
        public static HashSet<string> GetAllItemsFromSet(string setId) { return GetProvider().GetAllItemsFromSet(GetCacheKey(setId)); }
        public static List<string> GetAllItemsFromSortedSet(string setId) { return GetProvider().GetAllItemsFromSortedSet(GetCacheKey(setId)); }
        public static List<string> GetAllItemsFromSortedSetDesc(string setId) { return GetProvider().GetAllItemsFromSortedSetDesc(GetCacheKey(setId)); }
        public static List<string> GetAllKeys() { return GetProvider().GetAllKeys(); }
        public static IDictionary<string, double> GetAllWithScoresFromSortedSet(string setId) { return GetProvider().GetAllWithScoresFromSortedSet(GetCacheKey(setId)); }
        public static string GetAndSetEntry(string key, string value) { return GetProvider().GetAndSetEntry(GetCacheKey(key), value); }
        public static HashSet<string> GetDifferencesFromSet(string fromSetId, params string[] withSetIds) { return GetProvider().GetDifferencesFromSet(fromSetId, withSetIds); }
        public static RedisKeyType GetEntryType(string key) { return GetProvider().GetEntryType(GetCacheKey(key)); }
        public static T GetFromHash<T>(object id) { return GetProvider().GetFromHash<T>(id); }
        public static long GetHashCount(string hashId) { return GetProvider().GetHashCount(GetCacheKey(hashId)); }
        public static List<string> GetHashKeys(string hashId) { return GetProvider().GetHashKeys(GetCacheKey(hashId)); }
        public static List<string> GetHashValues(string hashId) { return GetProvider().GetHashValues(GetCacheKey(hashId)); }
        public static HashSet<string> GetIntersectFromSets(params string[] setIds) { return GetProvider().GetIntersectFromSets(setIds); }
        public static string GetItemFromList(string listId, int listIndex) { return GetProvider().GetItemFromList(GetCacheKey(listId), listIndex); }
        public static long GetItemIndexInSortedSet(string setId, string value) { return GetProvider().GetItemIndexInSortedSet(GetCacheKey(setId), value); }
        public static long GetItemIndexInSortedSetDesc(string setId, string value) { return GetProvider().GetItemIndexInSortedSetDesc(GetCacheKey(setId), value); }
        public static double GetItemScoreInSortedSet(string setId, string value) { return GetProvider().GetItemScoreInSortedSet(GetCacheKey(setId), value); }
        public static long GetListCount(string listId) { return GetProvider().GetListCount(GetCacheKey(listId)); }
        public static string GetRandomItemFromSet(string setId) { return GetProvider().GetRandomItemFromSet(GetCacheKey(setId)); }
        public static string GetRandomKey() { return GetProvider().GetRandomKey(); }
        public static List<string> GetRangeFromList(string listId, int startingFrom, int endingAt) { return GetProvider().GetRangeFromList(GetCacheKey(listId), startingFrom, endingAt); }
        public static List<string> GetRangeFromSortedList(string listId, int startingFrom, int endingAt) { return GetProvider().GetRangeFromSortedList(GetCacheKey(listId), startingFrom, endingAt); }
        public static List<string> GetRangeFromSortedSet(string setId, int fromRank, int toRank) { return GetProvider().GetRangeFromSortedSet(GetCacheKey(setId), fromRank, toRank); }
        public static List<string> GetRangeFromSortedSetByHighestScore(string setId, double fromScore, double toScore) { return GetProvider().GetRangeFromSortedSetByHighestScore(GetCacheKey(setId), fromScore, toScore); }
        public static List<string> GetRangeFromSortedSetByHighestScore(string setId, long fromScore, long toScore) { return GetProvider().GetRangeFromSortedSetByHighestScore(GetCacheKey(setId), fromScore, toScore); }
        public static List<string> GetRangeFromSortedSetByHighestScore(string setId, string fromStringScore, string toStringScore) { return GetProvider().GetRangeFromSortedSetByHighestScore(GetCacheKey(setId), fromStringScore, toStringScore); }
        public static List<string> GetRangeFromSortedSetByHighestScore(string setId, double fromScore, double toScore, int? skip, int? take) { return GetProvider().GetRangeFromSortedSetByHighestScore(GetCacheKey(setId), fromScore, toScore, skip, take); }
        public static List<string> GetRangeFromSortedSetByHighestScore(string setId, long fromScore, long toScore, int? skip, int? take) { return GetProvider().GetRangeFromSortedSetByHighestScore(GetCacheKey(setId), fromScore, toScore, skip, take); }
        public static List<string> GetRangeFromSortedSetByHighestScore(string setId, string fromStringScore, string toStringScore, int? skip, int? take) { return GetProvider().GetRangeFromSortedSetByHighestScore(GetCacheKey(setId), fromStringScore, toStringScore, skip, take); }
        public static List<string> GetRangeFromSortedSetByLowestScore(string setId, double fromScore, double toScore) { return GetProvider().GetRangeFromSortedSetByLowestScore(GetCacheKey(setId), fromScore, toScore); }
        public static List<string> GetRangeFromSortedSetByLowestScore(string setId, long fromScore, long toScore) { return GetProvider().GetRangeFromSortedSetByLowestScore(GetCacheKey(setId), fromScore, toScore); }
        public static List<string> GetRangeFromSortedSetByLowestScore(string setId, string fromStringScore, string toStringScore) { return GetProvider().GetRangeFromSortedSetByLowestScore(GetCacheKey(setId), fromStringScore, toStringScore); }
        public static List<string> GetRangeFromSortedSetByLowestScore(string setId, double fromScore, double toScore, int? skip, int? take) { return GetProvider().GetRangeFromSortedSetByLowestScore(GetCacheKey(setId), fromScore, toScore, skip, take); }
        public static List<string> GetRangeFromSortedSetByLowestScore(string setId, long fromScore, long toScore, int? skip, int? take) { return GetProvider().GetRangeFromSortedSetByLowestScore(GetCacheKey(setId), fromScore, toScore, skip, take); }
        public static List<string> GetRangeFromSortedSetByLowestScore(string setId, string fromStringScore, string toStringScore, int? skip, int? take) { return GetProvider().GetRangeFromSortedSetByLowestScore(GetCacheKey(setId), fromStringScore, toStringScore, skip, take); }
        public static List<string> GetRangeFromSortedSetDesc(string setId, int fromRank, int toRank) { return GetProvider().GetRangeFromSortedSetDesc(GetCacheKey(setId), fromRank, toRank); }
        public static IDictionary<string, double> GetRangeWithScoresFromSortedSet(string setId, int fromRank, int toRank) { return GetProvider().GetRangeWithScoresFromSortedSet(GetCacheKey(setId), fromRank, toRank); }
        public static IDictionary<string, double> GetRangeWithScoresFromSortedSetByHighestScore(string setId, double fromScore, double toScore) { return GetProvider().GetRangeWithScoresFromSortedSetByHighestScore(GetCacheKey(setId), fromScore, toScore); }
        public static IDictionary<string, double> GetRangeWithScoresFromSortedSetByHighestScore(string setId, long fromScore, long toScore) { return GetProvider().GetRangeWithScoresFromSortedSetByHighestScore(GetCacheKey(setId), fromScore, toScore); }
        public static IDictionary<string, double> GetRangeWithScoresFromSortedSetByHighestScore(string setId, string fromStringScore, string toStringScore) { return GetProvider().GetRangeWithScoresFromSortedSetByHighestScore(GetCacheKey(setId), fromStringScore, toStringScore); }
        public static IDictionary<string, double> GetRangeWithScoresFromSortedSetByHighestScore(string setId, double fromScore, double toScore, int? skip, int? take) { return GetProvider().GetRangeWithScoresFromSortedSetByHighestScore(GetCacheKey(setId), fromScore, toScore, skip, take); }


        public static IDictionary<string, double> GetRangeWithScoresFromSortedSetByHighestScore(string setId, long fromScore, long toScore, int? skip, int? take) { return GetProvider().GetRangeWithScoresFromSortedSetByHighestScore(GetCacheKey(setId), fromScore, toScore, skip, take); }
        public static IDictionary<string, double> GetRangeWithScoresFromSortedSetByHighestScore(string setId, string fromStringScore, string toStringScore, int? skip, int? take) { return GetProvider().GetRangeWithScoresFromSortedSetByHighestScore(GetCacheKey(setId), fromStringScore, toStringScore, skip, take); }
        public static IDictionary<string, double> GetRangeWithScoresFromSortedSetByLowestScore(string setId, double fromScore, double toScore) { return GetProvider().GetRangeWithScoresFromSortedSetByLowestScore(GetCacheKey(setId), fromScore, toScore); }


        public static IDictionary<string, double> GetRangeWithScoresFromSortedSetByLowestScore(string setId, long fromScore, long toScore)
        { return GetProvider().GetRangeWithScoresFromSortedSetByLowestScore(GetCacheKey(setId), fromScore, toScore); }
        public static IDictionary<string, double> GetRangeWithScoresFromSortedSetByLowestScore(string setId, string fromStringScore, string toStringScore)
        { return GetProvider().GetRangeWithScoresFromSortedSetByLowestScore(GetCacheKey(setId), fromStringScore, toStringScore); }
        public static IDictionary<string, double> GetRangeWithScoresFromSortedSetByLowestScore(string setId, double fromScore, double toScore, int? skip, int? take)
        {
            return GetProvider().GetRangeWithScoresFromSortedSetByLowestScore(GetCacheKey(setId), fromScore, toScore, skip, take);
        }
        IDictionary<string, double> GetRangeWithScoresFromSortedSetByLowestScore(string setId, long fromScore, long toScore, int? skip, int? take)
        {
            return GetProvider().GetRangeWithScoresFromSortedSetByLowestScore(GetCacheKey(setId), fromScore, toScore, skip, take);
        }

        public static IDictionary<string, double> GetRangeWithScoresFromSortedSetByLowestScore(string setId, string fromStringScore, string toStringScore, int? skip, int? take) { return GetProvider().GetRangeWithScoresFromSortedSetByLowestScore(GetCacheKey(setId), fromStringScore, toStringScore, skip, take); }
        public static IDictionary<string, double> GetRangeWithScoresFromSortedSetDesc(string setId, int fromRank, int toRank) { return GetProvider().GetRangeWithScoresFromSortedSetDesc(GetCacheKey(setId), fromRank, toRank); }
        public static long GetSetCount(string setId) { return GetProvider().GetSetCount(GetCacheKey(setId)); }
        public static List<string> GetSortedEntryValues(string key, int startingFrom, int endingAt) { return GetProvider().GetSortedEntryValues(GetCacheKey(key), startingFrom, endingAt); }
        public static List<string> GetSortedItemsFromList(string listId, SortOptions sortOptions) { return GetProvider().GetSortedItemsFromList(GetCacheKey(listId), sortOptions); }
        public static long GetSortedSetCount(string setId) { return GetProvider().GetSortedSetCount(GetCacheKey(setId)); }
        public static long GetSortedSetCount(string setId, double fromScore, double toScore) { return GetProvider().GetSortedSetCount(GetCacheKey(setId), fromScore, toScore); }
        public static long GetSortedSetCount(string setId, long fromScore, long toScore) { return GetProvider().GetSortedSetCount(GetCacheKey(setId), fromScore, toScore); }
        public static long GetSortedSetCount(string setId, string fromStringScore, string toStringScore) { return GetProvider().GetSortedSetCount(GetCacheKey(setId), fromStringScore, toStringScore); }
        public static TimeSpan GetTimeToLive(string key) { return GetProvider().GetTimeToLive(GetCacheKey(key)); }
        public static HashSet<string> GetUnionFromSets(params string[] setIds) { return GetProvider().GetUnionFromSets(setIds); }
        public static string GetValue(string key) { return GetProvider().GetValue(GetCacheKey(key)); }
        public static string GetValueFromHash(string hashId, string key) { return GetProvider().GetValueFromHash(GetCacheKey(hashId), GetCacheKey(key)); }
        public static List<string> GetValues(List<string> keys) { return GetProvider().GetValues(GetCacheKey(keys)); }
        public static List<T> GetValues<T>(List<string> keys) { return GetProvider().GetValues<T>(GetCacheKey(keys)); }
        public static List<string> GetValuesFromHash(string hashId, params string[] keys) { return GetProvider().GetValuesFromHash(GetCacheKey(hashId), keys); }
        public static Dictionary<string, T> GetValuesMap<T>(List<string> keys) { return GetProvider().GetValuesMap<T>(GetCacheKey(keys)); }
        public static Dictionary<string, string> GetValuesMap(List<string> keys) { return GetProvider().GetValuesMap(GetCacheKey(keys)); }
        public static bool HashContainsEntry(string hashId, string key) { return GetProvider().HashContainsEntry(GetCacheKey(hashId), GetCacheKey(key)); }
        public static bool HasLuaScript(string sha1Ref) { return GetProvider().HasLuaScript(sha1Ref); }
        public static double IncrementItemInSortedSet(string setId, string value, double incrementBy) { return GetProvider().IncrementItemInSortedSet(GetCacheKey(setId), value, incrementBy); }
        public static double IncrementItemInSortedSet(string setId, string value, long incrementBy) { return GetProvider().IncrementItemInSortedSet(GetCacheKey(setId), value, incrementBy); }
        public static long IncrementValue(string key) { return GetProvider().IncrementValue(GetCacheKey(key)); }
        public static long IncrementValueBy(string key, int count) { return GetProvider().IncrementValueBy(GetCacheKey(key), count); }
        public static long IncrementValueInHash(string hashId, string key, int incrementBy) { return GetProvider().IncrementValueInHash(GetCacheKey(hashId), GetCacheKey(key), incrementBy); }
        public static void KillRunningLuaScript() { GetProvider().KillRunningLuaScript(); }
        public static string LoadLuaScript(string body) { return GetProvider().LoadLuaScript(body); }
        public static void MoveBetweenSets(string fromSetId, string toSetId, string item) { GetProvider().MoveBetweenSets(fromSetId, toSetId, item); }
        public static string PopAndPushItemBetweenLists(string fromListId, string toListId) { return GetProvider().PopAndPushItemBetweenLists(fromListId, toListId); }
        public static string PopItemFromList(string listId) { return GetProvider().PopItemFromList(GetCacheKey(listId)); }
        public static string PopItemFromSet(string setId) { return GetProvider().PopItemFromSet(GetCacheKey(setId)); }
        public static string PopItemWithHighestScoreFromSortedSet(string setId) { return GetProvider().PopItemWithHighestScoreFromSortedSet(GetCacheKey(setId)); }
        public static string PopItemWithLowestScoreFromSortedSet(string setId) { return GetProvider().PopItemWithLowestScoreFromSortedSet(GetCacheKey(setId)); }
        public static void PrependItemToList(string listId, string value) { GetProvider().PrependItemToList(GetCacheKey(listId), value); }
        public static void PrependRangeToList(string listId, List<string> values) { GetProvider().PrependRangeToList(GetCacheKey(listId), values); }
        public static long PublishMessage(string toChannel, string message) { return GetProvider().PublishMessage(toChannel, message); }
        public static void PushItemToList(string listId, string value) { GetProvider().PushItemToList(GetCacheKey(listId), value); }
        public static void RemoveAllFromList(string listId) { GetProvider().RemoveAllFromList(GetCacheKey(listId)); }
        public static void RemoveAllLuaScripts() { GetProvider().RemoveAllLuaScripts(); }
        public static string RemoveEndFromList(string listId) { return GetProvider().RemoveEndFromList(GetCacheKey(listId)); }
        public static bool RemoveEntry(params string[] args) { return GetProvider().RemoveEntry(args); }
        public static bool RemoveEntryFromHash(string hashId, string key) { return GetProvider().RemoveEntryFromHash(GetCacheKey(hashId), GetCacheKey(key)); }
        public static long RemoveItemFromList(string listId, string value) { return GetProvider().RemoveItemFromList(GetCacheKey(listId), value); }
        public static long RemoveItemFromList(string listId, string value, int noOfMatches) { return GetProvider().RemoveItemFromList(GetCacheKey(listId), value, noOfMatches); }
        public static void RemoveItemFromSet(string setId, string item) { GetProvider().RemoveItemFromSet(GetCacheKey(setId), item); }
        public static bool RemoveItemFromSortedSet(string setId, string value) { return GetProvider().RemoveItemFromSortedSet(GetCacheKey(setId), value); }
        public static long RemoveRangeFromSortedSet(string setId, int minRank, int maxRank) { return GetProvider().RemoveRangeFromSortedSet(GetCacheKey(setId), minRank, maxRank); }
        public static long RemoveRangeFromSortedSetByScore(string setId, double fromScore, double toScore) { return GetProvider().RemoveRangeFromSortedSetByScore(GetCacheKey(setId), fromScore, toScore); }
        public static long RemoveRangeFromSortedSetByScore(string setId, long fromScore, long toScore) { return GetProvider().RemoveRangeFromSortedSetByScore(GetCacheKey(setId), fromScore, toScore); }
        public static string RemoveStartFromList(string listId) { return GetProvider().RemoveStartFromList(GetCacheKey(listId)); }
        public static void RenameKey(string fromName, string toName) { GetProvider().RenameKey(fromName, toName); }
        public static void RewriteAppendOnlyFileAsync() { GetProvider().RewriteAppendOnlyFileAsync(); }
        public static void Save() { GetProvider().Save(); }
        public static void SaveAsync() { GetProvider().SaveAsync(); }
        public static List<string> SearchKeys(string pattern) { return GetProvider().SearchKeys(pattern); }
        public static void SetAll(Dictionary<string, string> map) { GetProvider().SetAll(map); }
        public static void SetAll(IEnumerable<string> keys, IEnumerable<string> values) 
        {
            IEnumerator<string> keyOr = keys.GetEnumerator();
            List<string> list = new List<string>();
            keyOr.MoveNext();
            while (keyOr.Current != null)
            {
                list.Add(keyOr.Current);
                keyOr.MoveNext();
            }
            GetProvider().SetAll(GetCacheKey(list), values);
        }
        public static bool SetContainsItem(string setId, string item) { return GetProvider().SetContainsItem(GetCacheKey(setId), item); }
        public static void SetEntry(string key, string value) { GetProvider().SetEntry(GetCacheKey(key), value); }
        public static void SetEntry(string key, string value, TimeSpan expireIn) { GetProvider().SetEntry(GetCacheKey(key), value, expireIn); }
        public static bool SetEntryIfNotExists(string key, string value) { return GetProvider().SetEntryIfNotExists(GetCacheKey(key), value); }
        public static bool SetEntryInHash(string hashId, string key, string value) { return GetProvider().SetEntryInHash(GetCacheKey(hashId), GetCacheKey(key), value); }
        public static bool SetEntryInHashIfNotExists(string hashId, string key, string value) { return GetProvider().SetEntryInHashIfNotExists(GetCacheKey(hashId), GetCacheKey(key), value); }
        public static void SetItemInList(string listId, int listIndex, string value) { GetProvider().SetItemInList(GetCacheKey(listId), listIndex, value); }
        public static void SetRangeInHash(string hashId, IEnumerable<KeyValuePair<string, string>> keyValuePairs) { GetProvider().SetRangeInHash(GetCacheKey(hashId), keyValuePairs); }
        public static void Shutdown() { GetProvider().Shutdown(); }
        public static bool SortedSetContainsItem(string setId, string value) { return GetProvider().SortedSetContainsItem(GetCacheKey(setId), value); }
        public static void StoreAsHash<T>(T entity) { GetProvider().StoreAsHash<T>(entity); }
        public static void StoreDifferencesFromSet(string intoSetId, string fromSetId, params string[] withSetIds) { GetProvider().StoreDifferencesFromSet(GetCacheKey(intoSetId), GetCacheKey(fromSetId), withSetIds); }
        public static void StoreIntersectFromSets(string intoSetId, params string[] setIds) { GetProvider().StoreIntersectFromSets(GetCacheKey(intoSetId), GetCacheKey(setIds)); }
        public static long StoreIntersectFromSortedSets(string intoSetId, params string[] setIds) { return GetProvider().StoreIntersectFromSortedSets(GetCacheKey(intoSetId), setIds); }
        public static object StoreObject(object entity) { return GetProvider().StoreObject(entity); }
        public static void StoreUnionFromSets(string intoSetId, params string[] setIds) { GetProvider().StoreUnionFromSets(GetCacheKey(intoSetId), GetCacheKey(setIds)); }
        public static long StoreUnionFromSortedSets(string intoSetId, params string[] setIds) { return GetProvider().StoreUnionFromSortedSets(GetCacheKey(intoSetId), GetCacheKey(setIds)); }
        public static void TrimList(string listId, int keepStartingFrom, int keepEndingAt) { GetProvider().TrimList(GetCacheKey(listId), keepStartingFrom, keepEndingAt); }
        public static void UnWatch() { GetProvider().UnWatch(); }
        public static void Watch(params string[] keys) { GetProvider().Watch(GetCacheKey(keys)); }
        public static Dictionary<string, bool> WhichLuaScriptsExists(params string[] sha1Refs) { return GetProvider().WhichLuaScriptsExists(sha1Refs); }
        public static void WriteAll<TEntity>(IEnumerable<TEntity> entities) { GetProvider().WriteAll<TEntity>(entities); }

        public static void Delete<T>(T entity)
        {
            GetProvider().Delete<T>(entity);
        }

        public static void DeleteAll<TEntity>()
        {
            GetProvider().DeleteAll<TEntity>();
        }

        public static void DeleteById<T>(object id)
        {
            GetProvider().DeleteById<T>(id);
        }

        public static void DeleteByIds<T>(ICollection ids)
        {
            GetProvider().DeleteByIds<T>(ids);
        }

        public static T GetById<T>(object id)
        {
            return GetProvider().GetById<T>(id);
        }

        public static IList<T> GetByIds<T>(ICollection ids)
        {
            return GetProvider().GetByIds<T>(ids);
        }

        public static T Store<T>(T entity)
        {
            return GetProvider().Store<T>(entity);
        }

        public static void StoreAll<TEntity>(IEnumerable<TEntity> entities)
        {
            GetProvider().StoreAll<TEntity>(entities);
        }

        public static void Dispose()
        {
            GetProvider().Dispose();
        }

        public static bool Add<T>(string key, T value, TimeSpan expiresIn)
        {
            return GetProvider().Add<T>(GetCacheKey(key), value, expiresIn);
        }

        public static bool Add<T>(string key, T value, DateTime expiresAt)
        {
            return GetProvider().Add<T>(GetCacheKey(key), value, expiresAt);
        }

        public static bool Add<T>(string key, T value)
        {
            return GetProvider().Add<T>(GetCacheKey(key), value);
        }

        public static long Decrement(string key, uint amount)
        {
            return GetProvider().Decrement(GetCacheKey(key), amount);
        }

        public static void FlushAll()
        {
            GetProvider().FlushAll();
        }

        public static T Get<T>(string key)
        {
            return GetProvider().Get<T>(GetCacheKey(key));
        }

        public static IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            IEnumerator<string> keyOr = keys.GetEnumerator();
            List<string> list = new List<string>();
            keyOr.MoveNext();
            while (keyOr.Current != null)
            {
                list.Add(keyOr.Current);
                keyOr.MoveNext();
            }
            return GetProvider().GetAll<T>(GetCacheKey(list));

        }

        public static long Increment(string key, uint amount)
        {
            return GetProvider().Increment(GetCacheKey(key), amount);

        }

        public static bool Remove(string key)
        {
            return GetProvider().Remove(GetCacheKey(key));

        }

        public static void RemoveAll(IEnumerable<string> keys)
        {
            IEnumerator<string> keyOr = keys.GetEnumerator();
            List<string> list = new List<string>();
            keyOr.MoveNext();
            while (keyOr.Current != null)
            {
                list.Add(keyOr.Current);
                keyOr.MoveNext();
            }
            GetProvider().RemoveAll(GetCacheKey(list));

        }

        public static bool Replace<T>(string key, T value, TimeSpan expiresIn)
        {
            return GetProvider().Replace<T>(GetCacheKey(key), value, expiresIn);
        }

        public static bool Replace<T>(string key, T value, DateTime expiresAt)
        {
            return GetProvider().Replace<T>(GetCacheKey(key), value, expiresAt);
        }

        public static bool Replace<T>(string key, T value)
        {
            return GetProvider().Replace<T>(GetCacheKey(key), value);
        }

        public static bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            return GetProvider().Set<T>(GetCacheKey(key), value, expiresIn);
        }

        public static bool Set<T>(string key, T value, DateTime expiresAt)
        {
            return GetProvider().Set<T>(GetCacheKey(key), value, expiresAt);
        }

        public static bool Set<T>(string key, T value)
        {
            return GetProvider().Set<T>(GetCacheKey(key), value);
        }

        public static void SetAll<T>(IDictionary<string, T> values)
        {
            GetProvider().SetAll<T>(values);
        }
        #endregion
    }
}
#endif
