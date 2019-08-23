#if NET40
using System;
using System.Linq;
using System.Text;
using Memcached.ClientLibrary;
using System.Collections.Generic;
using System.Collections;
using ServiceStack.Redis;
using ServiceStack.Redis.Pipeline;
using ServiceStack.Redis.Generic;
using ServiceStack.Model;

namespace JinRi.Notify.Frame
{
    public class RedisProvider : IRedisProvider
    {
        private static RedisSockIOPool m_pool;

        public RedisProvider()
        {
            m_pool = new RedisSockIOPool();
        }

        #region ICacheProvider 成员

        public bool Add(string key, object value)
        {
            using (var client = m_pool.GetClient())
            {
                return client.Set<object>(key, value);
            }
        }

        public bool Add(string key, object value, DateTime expiry)
        {
            using (var client = m_pool.GetClient())
            {
                return client.Set<object>(key, value, expiry);
            }
        }

        public object Get(string key)
        {
            using (var client = m_pool.GetClient())
            {
                return client.Get<object>(key);
            }
        }

        public bool Delete(string key)
        {
            using (var client = m_pool.GetClient())
            {
                return client.Remove(key);
            }
        }

        public bool KeyExists(string key)
        {
            using (var client = m_pool.GetClient())
            {
                return client.ContainsKey(key);
            }
        }

        public bool Set(string key, object value)
        {
            using (var client = m_pool.GetClient())
            {
                return client.Set<object>(key, value);
            }
        }

        public bool Set(string key, object value, DateTime expiry)
        {
            using (var client = m_pool.GetClient())
            {
                return client.Set<object>(key, value, expiry);
            }
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IRedisClient

        public int ConnectTimeout { get { using (var client = m_pool.GetClient()) { return client.ConnectTimeout; } } set { using (var client = m_pool.GetClient()) { client.ConnectTimeout = value; } } }
        public long Db { get { using (var client = m_pool.GetClient()) { return client.Db; } } set { using (var client = m_pool.GetClient()) { client.Db = value; } } }
        public long DbSize { get { using (var client = m_pool.GetClient()) { return client.Db; } } }
        public bool HadExceptions { get { using (var client = m_pool.GetClient()) { return client.HadExceptions; } } }
        public IHasNamed<IRedisHash> Hashes { get { using (var client = m_pool.GetClient()) { return client.Hashes; } } set { using (var client = m_pool.GetClient()) { client.Hashes = value; } } }
        public string Host { get { using (var client = m_pool.GetClient()) { return client.Host; } } }
        public Dictionary<string, string> Info { get { using (var client = m_pool.GetClient()) { return client.Info; } } }
        public DateTime LastSave { get { using (var client = m_pool.GetClient()) { return client.LastSave; } } }
        public IHasNamed<IRedisList> Lists { get { using (var client = m_pool.GetClient()) { return client.Lists; } } set { using (var client = m_pool.GetClient()) { client.Lists = value; } } }
        public string Password { get { using (var client = m_pool.GetClient()) { return client.Password; } } set { using (var client = m_pool.GetClient()) { client.Password = value; } } }
        public int Port { get { using (var client = m_pool.GetClient()) { return client.Port; } } }
        public int RetryCount { get { using (var client = m_pool.GetClient()) { return client.RetryCount; } } set { using (var client = m_pool.GetClient()) { client.RetryCount = value; } } }
        public int RetryTimeout { get { using (var client = m_pool.GetClient()) { return client.RetryTimeout; } } set { using (var client = m_pool.GetClient()) { client.RetryTimeout = value; } } }
        public int SendTimeout { get { using (var client = m_pool.GetClient()) { return client.SendTimeout; } } set { using (var client = m_pool.GetClient()) { client.SendTimeout = value; } } }
        public IHasNamed<IRedisSet> Sets { get { using (var client = m_pool.GetClient()) { return client.Sets; } } set { using (var client = m_pool.GetClient()) { client.Sets = value; } } }
        public IHasNamed<IRedisSortedSet> SortedSets { get { using (var client = m_pool.GetClient()) { return client.SortedSets; } } set { using (var client = m_pool.GetClient()) { client.SortedSets = value; } } }

        public string this[string key] { get { using (var client = m_pool.GetClient()) { return client[key]; } } set { using (var client = m_pool.GetClient()) { client[key] = value; } } }

        public IDisposable AcquireLock(string key) { using (var client = m_pool.GetClient()) { return client.AcquireLock(key); } }
        public IDisposable AcquireLock(string key, TimeSpan timeOut) { using (var client = m_pool.GetClient()) { return client.AcquireLock(key, timeOut); } }
        public void AddItemToList(string listId, string value) { using (var client = m_pool.GetClient()) { client.AddItemToList(listId, value); } }
        public void AddItemToSet(string setId, string item) { using (var client = m_pool.GetClient()) { client.AddItemToSet(setId, item); } }
        public bool AddItemToSortedSet(string setId, string value) { using (var client = m_pool.GetClient()) { return client.AddItemToSortedSet(setId, value); } }
        public bool AddItemToSortedSet(string setId, string value, double score) { using (var client = m_pool.GetClient()) { return client.AddItemToSortedSet(setId, value, score); } }
        public void AddRangeToList(string listId, List<string> values) { using (var client = m_pool.GetClient()) { client.AddRangeToList(listId, values); } }
        public void AddRangeToSet(string setId, List<string> items) { using (var client = m_pool.GetClient()) { client.AddRangeToSet(setId, items); } }
        public bool AddRangeToSortedSet(string setId, List<string> values, double score) { using (var client = m_pool.GetClient()) { return client.AddRangeToSortedSet(setId, values, score); } }
        public bool AddRangeToSortedSet(string setId, List<string> values, long score) { using (var client = m_pool.GetClient()) { return client.AddRangeToSortedSet(setId, values, score); } }
        public long AppendToValue(string key, string value) { using (var client = m_pool.GetClient()) { return client.AppendToValue(key, value); } }
        public IRedisTypedClient<T> As<T>() { using (var client = m_pool.GetClient()) { return client.As<T>(); } }
        public string BlockingDequeueItemFromList(string listId, TimeSpan? timeOut) { using (var client = m_pool.GetClient()) { return client.BlockingDequeueItemFromList(listId, timeOut); } }
        public ItemRef BlockingDequeueItemFromLists(string[] listIds, TimeSpan? timeOut) { using (var client = m_pool.GetClient()) { return client.BlockingDequeueItemFromLists(listIds, timeOut); } }
        public string BlockingPopAndPushItemBetweenLists(string fromListId, string toListId, TimeSpan? timeOut) { using (var client = m_pool.GetClient()) { return client.BlockingPopAndPushItemBetweenLists(fromListId, toListId, timeOut); } }
        public string BlockingPopItemFromList(string listId, TimeSpan? timeOut) { using (var client = m_pool.GetClient()) { return client.BlockingPopItemFromList(listId, timeOut); } }
        public ItemRef BlockingPopItemFromLists(string[] listIds, TimeSpan? timeOut) { using (var client = m_pool.GetClient()) { return client.BlockingPopItemFromLists(listIds, timeOut); } }
        public string BlockingRemoveStartFromList(string listId, TimeSpan? timeOut) { using (var client = m_pool.GetClient()) { return client.BlockingRemoveStartFromList(listId, timeOut); } }
        public ItemRef BlockingRemoveStartFromLists(string[] listIds, TimeSpan? timeOut) { using (var client = m_pool.GetClient()) { return client.BlockingRemoveStartFromLists(listIds, timeOut); } }
        public string CalculateSha1(string luaBody) { using (var client = m_pool.GetClient()) { return client.CalculateSha1(luaBody); } }
        public bool ContainsKey(string key) { using (var client = m_pool.GetClient()) { return client.ContainsKey(key); } }
        public IRedisPipeline CreatePipeline() { using (var client = m_pool.GetClient()) { return client.CreatePipeline(); } }
        public IRedisSubscription CreateSubscription() { using (var client = m_pool.GetClient()) { return client.CreateSubscription(); } }
        public IRedisTransaction CreateTransaction() { using (var client = m_pool.GetClient()) { return client.CreateTransaction(); } }
        public long DecrementValue(string key) { using (var client = m_pool.GetClient()) { return client.DecrementValue(key); } }
        public long DecrementValueBy(string key, int count) { using (var client = m_pool.GetClient()) { return client.DecrementValueBy(key, count); } }
        public string DequeueItemFromList(string listId) { using (var client = m_pool.GetClient()) { return client.DequeueItemFromList(listId); } }
        public void EnqueueItemOnList(string listId, string value) { using (var client = m_pool.GetClient()) { client.EnqueueItemOnList(listId, value); } }
        public long ExecLuaAsInt(string luaBody, params string[] args) { using (var client = m_pool.GetClient()) { return client.ExecLuaAsInt(luaBody, args); } }
        public long ExecLuaAsInt(string luaBody, string[] keys, string[] args) { using (var client = m_pool.GetClient()) { return client.ExecLuaAsInt(luaBody, keys, args); } }
        public List<string> ExecLuaAsList(string luaBody, params string[] args) { using (var client = m_pool.GetClient()) { return client.ExecLuaAsList(luaBody, args); } }
        public List<string> ExecLuaAsList(string luaBody, string[] keys, string[] args) { using (var client = m_pool.GetClient()) { return client.ExecLuaAsList(luaBody, keys, args); } }
        public string ExecLuaAsString(string luaBody, params string[] args) { using (var client = m_pool.GetClient()) { return client.ExecLuaAsString(luaBody, args); } }
        public string ExecLuaAsString(string luaBody, string[] keys, string[] args) { using (var client = m_pool.GetClient()) { return client.ExecLuaAsString(luaBody, keys, args); } }
        public long ExecLuaShaAsInt(string sha1, params string[] args) { using (var client = m_pool.GetClient()) { return client.ExecLuaShaAsInt(sha1, args); } }
        public long ExecLuaShaAsInt(string sha1, string[] keys, string[] args) { using (var client = m_pool.GetClient()) { return client.ExecLuaShaAsInt(sha1, keys, args); } }
        public List<string> ExecLuaShaAsList(string sha1, params string[] args) { using (var client = m_pool.GetClient()) { return client.ExecLuaShaAsList(sha1, args); } }
        public List<string> ExecLuaShaAsList(string sha1, string[] keys, string[] args) { using (var client = m_pool.GetClient()) { return client.ExecLuaShaAsList(sha1, keys, args); } }
        public string ExecLuaShaAsString(string sha1, params string[] args) { using (var client = m_pool.GetClient()) { return client.ExecLuaShaAsString(sha1, args); } }
        public string ExecLuaShaAsString(string sha1, string[] keys, string[] args) { using (var client = m_pool.GetClient()) { return client.ExecLuaShaAsString(sha1, keys, args); } }
        public bool ExpireEntryAt(string key, DateTime expireAt) { using (var client = m_pool.GetClient()) { return client.ExpireEntryAt(key, expireAt); } }
        public bool ExpireEntryIn(string key, TimeSpan expireIn) { using (var client = m_pool.GetClient()) { return client.ExpireEntryIn(key, expireIn); } }
        public void FlushDb() { using (var client = m_pool.GetClient()) { client.FlushDb(); } }
        public Dictionary<string, string> GetAllEntriesFromHash(string hashId) { using (var client = m_pool.GetClient()) { return client.GetAllEntriesFromHash(hashId); } }
        public List<string> GetAllItemsFromList(string listId) { using (var client = m_pool.GetClient()) { return client.GetAllItemsFromList(listId); } }
        public HashSet<string> GetAllItemsFromSet(string setId) { using (var client = m_pool.GetClient()) { return client.GetAllItemsFromSet(setId); } }
        public List<string> GetAllItemsFromSortedSet(string setId) { using (var client = m_pool.GetClient()) { return client.GetAllItemsFromSortedSet(setId); } }
        public List<string> GetAllItemsFromSortedSetDesc(string setId) { using (var client = m_pool.GetClient()) { return client.GetAllItemsFromSortedSetDesc(setId); } }
        public List<string> GetAllKeys() { using (var client = m_pool.GetClient()) { return client.GetAllKeys(); } }
        public IDictionary<string, double> GetAllWithScoresFromSortedSet(string setId) { using (var client = m_pool.GetClient()) { return client.GetAllWithScoresFromSortedSet(setId); } }
        public string GetAndSetEntry(string key, string value) { using (var client = m_pool.GetClient()) { return client.GetAndSetEntry(key, value); } }
        public HashSet<string> GetDifferencesFromSet(string fromSetId, params string[] withSetIds) { using (var client = m_pool.GetClient()) { return client.GetDifferencesFromSet(fromSetId, withSetIds); } }
        public RedisKeyType GetEntryType(string key) { using (var client = m_pool.GetClient()) { return client.GetEntryType(key); } }
        public T GetFromHash<T>(object id) { using (var client = m_pool.GetClient()) { return client.GetFromHash<T>(id); } }
        public long GetHashCount(string hashId) { using (var client = m_pool.GetClient()) { return client.GetHashCount(hashId); } }
        public List<string> GetHashKeys(string hashId) { using (var client = m_pool.GetClient()) { return client.GetHashKeys(hashId); } }
        public List<string> GetHashValues(string hashId) { using (var client = m_pool.GetClient()) { return client.GetHashValues(hashId); } }
        public HashSet<string> GetIntersectFromSets(params string[] setIds) { using (var client = m_pool.GetClient()) { return client.GetIntersectFromSets(setIds); } }
        public string GetItemFromList(string listId, int listIndex) { using (var client = m_pool.GetClient()) { return client.GetItemFromList(listId, listIndex); } }
        public long GetItemIndexInSortedSet(string setId, string value) { using (var client = m_pool.GetClient()) { return client.GetItemIndexInSortedSet(setId, value); } }
        public long GetItemIndexInSortedSetDesc(string setId, string value) { using (var client = m_pool.GetClient()) { return client.GetItemIndexInSortedSetDesc(setId, value); } }
        public double GetItemScoreInSortedSet(string setId, string value) { using (var client = m_pool.GetClient()) { return client.GetItemScoreInSortedSet(setId, value); } }
        public long GetListCount(string listId) { using (var client = m_pool.GetClient()) { return client.GetListCount(listId); } }
        public string GetRandomItemFromSet(string setId) { using (var client = m_pool.GetClient()) { return client.GetRandomItemFromSet(setId); } }
        public string GetRandomKey() { using (var client = m_pool.GetClient()) { return client.GetRandomKey(); } }
        public List<string> GetRangeFromList(string listId, int startingFrom, int endingAt) { using (var client = m_pool.GetClient()) { return client.GetRangeFromList(listId, startingFrom, endingAt); } }
        public List<string> GetRangeFromSortedList(string listId, int startingFrom, int endingAt) { using (var client = m_pool.GetClient()) { return client.GetRangeFromSortedList(listId, startingFrom, endingAt); } }
        public List<string> GetRangeFromSortedSet(string setId, int fromRank, int toRank) { using (var client = m_pool.GetClient()) { return client.GetRangeFromSortedSet(setId, fromRank, toRank); } }
        public List<string> GetRangeFromSortedSetByHighestScore(string setId, double fromScore, double toScore) { using (var client = m_pool.GetClient()) { return client.GetRangeFromSortedSetByHighestScore(setId, fromScore, toScore); } }
        public List<string> GetRangeFromSortedSetByHighestScore(string setId, long fromScore, long toScore) { using (var client = m_pool.GetClient()) { return client.GetRangeFromSortedSetByHighestScore(setId, fromScore, toScore); } }
        public List<string> GetRangeFromSortedSetByHighestScore(string setId, string fromStringScore, string toStringScore) { using (var client = m_pool.GetClient()) { return client.GetRangeFromSortedSetByHighestScore(setId, fromStringScore, toStringScore); } }
        public List<string> GetRangeFromSortedSetByHighestScore(string setId, double fromScore, double toScore, int? skip, int? take) { using (var client = m_pool.GetClient()) { return client.GetRangeFromSortedSetByHighestScore(setId, fromScore, toScore, skip, take); } }
        public List<string> GetRangeFromSortedSetByHighestScore(string setId, long fromScore, long toScore, int? skip, int? take) { using (var client = m_pool.GetClient()) { return client.GetRangeFromSortedSetByHighestScore(setId, fromScore, toScore, skip, take); } }
        public List<string> GetRangeFromSortedSetByHighestScore(string setId, string fromStringScore, string toStringScore, int? skip, int? take) { using (var client = m_pool.GetClient()) { return client.GetRangeFromSortedSetByHighestScore(setId, fromStringScore, toStringScore, skip, take); } }
        public List<string> GetRangeFromSortedSetByLowestScore(string setId, double fromScore, double toScore) { using (var client = m_pool.GetClient()) { return client.GetRangeFromSortedSetByLowestScore(setId, fromScore, toScore); } }
        public List<string> GetRangeFromSortedSetByLowestScore(string setId, long fromScore, long toScore) { using (var client = m_pool.GetClient()) { return client.GetRangeFromSortedSetByLowestScore(setId, fromScore, toScore); } }
        public List<string> GetRangeFromSortedSetByLowestScore(string setId, string fromStringScore, string toStringScore) { using (var client = m_pool.GetClient()) { return client.GetRangeFromSortedSetByLowestScore(setId, fromStringScore, toStringScore); } }
        public List<string> GetRangeFromSortedSetByLowestScore(string setId, double fromScore, double toScore, int? skip, int? take) { using (var client = m_pool.GetClient()) { return client.GetRangeFromSortedSetByLowestScore(setId, fromScore, toScore, skip, take); } }
        public List<string> GetRangeFromSortedSetByLowestScore(string setId, long fromScore, long toScore, int? skip, int? take) { using (var client = m_pool.GetClient()) { return client.GetRangeFromSortedSetByLowestScore(setId, fromScore, toScore, skip, take); } }
        public List<string> GetRangeFromSortedSetByLowestScore(string setId, string fromStringScore, string toStringScore, int? skip, int? take) { using (var client = m_pool.GetClient()) { return client.GetRangeFromSortedSetByLowestScore(setId, fromStringScore, toStringScore, skip, take); } }
        public List<string> GetRangeFromSortedSetDesc(string setId, int fromRank, int toRank) { using (var client = m_pool.GetClient()) { return client.GetRangeFromSortedSetDesc(setId, fromRank, toRank); } }
        public IDictionary<string, double> GetRangeWithScoresFromSortedSet(string setId, int fromRank, int toRank) { using (var client = m_pool.GetClient()) { return client.GetRangeWithScoresFromSortedSet(setId, fromRank, toRank); } }
        public IDictionary<string, double> GetRangeWithScoresFromSortedSetByHighestScore(string setId, double fromScore, double toScore) { using (var client = m_pool.GetClient()) { return client.GetRangeWithScoresFromSortedSetByHighestScore(setId, fromScore, toScore); } }
        public IDictionary<string, double> GetRangeWithScoresFromSortedSetByHighestScore(string setId, long fromScore, long toScore) { using (var client = m_pool.GetClient()) { return client.GetRangeWithScoresFromSortedSetByHighestScore(setId, fromScore, toScore); } }
        public IDictionary<string, double> GetRangeWithScoresFromSortedSetByHighestScore(string setId, string fromStringScore, string toStringScore) { using (var client = m_pool.GetClient()) { return client.GetRangeWithScoresFromSortedSetByHighestScore(setId, fromStringScore, toStringScore); } }
        public IDictionary<string, double> GetRangeWithScoresFromSortedSetByHighestScore(string setId, double fromScore, double toScore, int? skip, int? take) { using (var client = m_pool.GetClient()) { return client.GetRangeWithScoresFromSortedSetByHighestScore(setId, fromScore, toScore, skip, take); } }

        public IDictionary<string, double> GetRangeWithScoresFromSortedSetByHighestScore(string setId, long fromScore, long toScore, int? skip, int? take) { using (var client = m_pool.GetClient()) { return client.GetRangeWithScoresFromSortedSetByHighestScore(setId, fromScore, toScore, skip, take); } }
        public IDictionary<string, double> GetRangeWithScoresFromSortedSetByHighestScore(string setId, string fromStringScore, string toStringScore, int? skip, int? take) { using (var client = m_pool.GetClient()) { return client.GetRangeWithScoresFromSortedSetByHighestScore(setId, fromStringScore, toStringScore, skip, take); } }
        public IDictionary<string, double> GetRangeWithScoresFromSortedSetByLowestScore(string setId, double fromScore, double toScore) { using (var client = m_pool.GetClient()) { return client.GetRangeWithScoresFromSortedSetByLowestScore(setId, fromScore, toScore); } }

        public IDictionary<string, double> GetRangeWithScoresFromSortedSetByLowestScore(string setId, long fromScore, long toScore)
        { using (var client = m_pool.GetClient()) { return client.GetRangeWithScoresFromSortedSetByLowestScore(setId, fromScore, toScore); } }
        public IDictionary<string, double> GetRangeWithScoresFromSortedSetByLowestScore(string setId, string fromStringScore, string toStringScore)
        { using (var client = m_pool.GetClient()) { return client.GetRangeWithScoresFromSortedSetByLowestScore(setId, fromStringScore, toStringScore); } }
        public IDictionary<string, double> GetRangeWithScoresFromSortedSetByLowestScore(string setId, double fromScore, double toScore, int? skip, int? take)
        {
            using (var client = m_pool.GetClient())
            {
                return client.GetRangeWithScoresFromSortedSetByLowestScore(setId, fromScore, toScore, skip, take);
            }
        }
        IDictionary<string, double> GetRangeWithScoresFromSortedSetByLowestScore(string setId, long fromScore, long toScore, int? skip, int? take)
        {
            using (var client = m_pool.GetClient())
            {
                return client.GetRangeWithScoresFromSortedSetByLowestScore(setId, fromScore, toScore, skip, take);
            }
        }

        public IDictionary<string, double> GetRangeWithScoresFromSortedSetByLowestScore(string setId, string fromStringScore, string toStringScore, int? skip, int? take) { using (var client = m_pool.GetClient()) { return client.GetRangeWithScoresFromSortedSetByLowestScore(setId, fromStringScore, toStringScore, skip, take); } }
        public IDictionary<string, double> GetRangeWithScoresFromSortedSetDesc(string setId, int fromRank, int toRank) { using (var client = m_pool.GetClient()) { return client.GetRangeWithScoresFromSortedSetDesc(setId, fromRank, toRank); } }
        public long GetSetCount(string setId) { using (var client = m_pool.GetClient()) { return client.GetSetCount(setId); } }
        public List<string> GetSortedEntryValues(string key, int startingFrom, int endingAt) { using (var client = m_pool.GetClient()) { return client.GetSortedEntryValues(key, startingFrom, endingAt); } }
        public List<string> GetSortedItemsFromList(string listId, SortOptions sortOptions) { using (var client = m_pool.GetClient()) { return client.GetSortedItemsFromList(listId, sortOptions); } }
        public long GetSortedSetCount(string setId) { using (var client = m_pool.GetClient()) { return client.GetSortedSetCount(setId); } }
        public long GetSortedSetCount(string setId, double fromScore, double toScore) { using (var client = m_pool.GetClient()) { return client.GetSortedSetCount(setId, fromScore, toScore); } }
        public long GetSortedSetCount(string setId, long fromScore, long toScore) { using (var client = m_pool.GetClient()) { return client.GetSortedSetCount(setId, fromScore, toScore); } }
        public long GetSortedSetCount(string setId, string fromStringScore, string toStringScore) { using (var client = m_pool.GetClient()) { return client.GetSortedSetCount(setId, fromStringScore, toStringScore); } }
        public TimeSpan GetTimeToLive(string key) { using (var client = m_pool.GetClient()) { return client.GetTimeToLive(key); } }
        public HashSet<string> GetUnionFromSets(params string[] setIds) { using (var client = m_pool.GetClient()) { return client.GetUnionFromSets(setIds); } }
        public string GetValue(string key) { using (var client = m_pool.GetClient()) { return client.GetValue(key); } }
        public string GetValueFromHash(string hashId, string key) { using (var client = m_pool.GetClient()) { return client.GetValueFromHash(hashId, key); } }
        public List<string> GetValues(List<string> keys) { using (var client = m_pool.GetClient()) { return client.GetValues(keys); } }
        public List<T> GetValues<T>(List<string> keys) { using (var client = m_pool.GetClient()) { return client.GetValues<T>(keys); } }
        public List<string> GetValuesFromHash(string hashId, params string[] keys) { using (var client = m_pool.GetClient()) { return client.GetValuesFromHash(hashId, keys); } }
        public Dictionary<string, T> GetValuesMap<T>(List<string> keys) { using (var client = m_pool.GetClient()) { return client.GetValuesMap<T>(keys); } }
        public Dictionary<string, string> GetValuesMap(List<string> keys) { using (var client = m_pool.GetClient()) { return client.GetValuesMap(keys); } }
        public bool HashContainsEntry(string hashId, string key) { using (var client = m_pool.GetClient()) { return client.HashContainsEntry(hashId, key); } }
        public bool HasLuaScript(string sha1Ref) { using (var client = m_pool.GetClient()) { return client.HasLuaScript(sha1Ref); } }
        public double IncrementItemInSortedSet(string setId, string value, double incrementBy) { using (var client = m_pool.GetClient()) { return client.IncrementItemInSortedSet(setId, value, incrementBy); } }
        public double IncrementItemInSortedSet(string setId, string value, long incrementBy) { using (var client = m_pool.GetClient()) { return client.IncrementItemInSortedSet(setId, value, incrementBy); } }
        public long IncrementValue(string key) { using (var client = m_pool.GetClient()) { return client.IncrementValue(key); } }
        public long IncrementValueBy(string key, int count) { using (var client = m_pool.GetClient()) { return client.IncrementValueBy(key, count); } }
        public long IncrementValueInHash(string hashId, string key, int incrementBy) { using (var client = m_pool.GetClient()) { return client.IncrementValueInHash(hashId, key, incrementBy); } }
        public void KillRunningLuaScript() { using (var client = m_pool.GetClient()) { client.KillRunningLuaScript(); } }
        public string LoadLuaScript(string body) { using (var client = m_pool.GetClient()) { return client.LoadLuaScript(body); } }
        public void MoveBetweenSets(string fromSetId, string toSetId, string item) { using (var client = m_pool.GetClient()) { client.MoveBetweenSets(fromSetId, toSetId, item); } }
        public string PopAndPushItemBetweenLists(string fromListId, string toListId) { using (var client = m_pool.GetClient()) { return client.PopAndPushItemBetweenLists(fromListId, toListId); } }
        public string PopItemFromList(string listId) { using (var client = m_pool.GetClient()) { return client.PopItemFromList(listId); } }
        public string PopItemFromSet(string setId) { using (var client = m_pool.GetClient()) { return client.PopItemFromSet(setId); } }
        public string PopItemWithHighestScoreFromSortedSet(string setId) { using (var client = m_pool.GetClient()) { return client.PopItemWithHighestScoreFromSortedSet(setId); } }
        public string PopItemWithLowestScoreFromSortedSet(string setId) { using (var client = m_pool.GetClient()) { return client.PopItemWithLowestScoreFromSortedSet(setId); } }
        public void PrependItemToList(string listId, string value) { using (var client = m_pool.GetClient()) { client.PrependItemToList(listId, value); } }
        public void PrependRangeToList(string listId, List<string> values) { using (var client = m_pool.GetClient()) { client.PrependRangeToList(listId, values); } }
        public long PublishMessage(string toChannel, string message) { using (var client = m_pool.GetClient()) { return client.PublishMessage(toChannel, message); } }
        public void PushItemToList(string listId, string value) { using (var client = m_pool.GetClient()) { client.PushItemToList(listId, value); } }
        public void RemoveAllFromList(string listId) { using (var client = m_pool.GetClient()) { client.RemoveAllFromList(listId); } }
        public void RemoveAllLuaScripts() { using (var client = m_pool.GetClient()) { client.RemoveAllLuaScripts(); } }
        public string RemoveEndFromList(string listId) { using (var client = m_pool.GetClient()) { return client.RemoveEndFromList(listId); } }
        public bool RemoveEntry(params string[] args) { using (var client = m_pool.GetClient()) { return client.RemoveEntry(args); } }
        public bool RemoveEntryFromHash(string hashId, string key) { using (var client = m_pool.GetClient()) { return client.RemoveEntryFromHash(hashId, key); } }
        public long RemoveItemFromList(string listId, string value) { using (var client = m_pool.GetClient()) { return client.RemoveItemFromList(listId, value); } }
        public long RemoveItemFromList(string listId, string value, int noOfMatches) { using (var client = m_pool.GetClient()) { return client.RemoveItemFromList(listId, value, noOfMatches); } }
        public void RemoveItemFromSet(string setId, string item) { using (var client = m_pool.GetClient()) { client.RemoveItemFromSet(setId, item); } }
        public bool RemoveItemFromSortedSet(string setId, string value) { using (var client = m_pool.GetClient()) { return client.RemoveItemFromSortedSet(setId, value); } }
        public long RemoveRangeFromSortedSet(string setId, int minRank, int maxRank) { using (var client = m_pool.GetClient()) { return client.RemoveRangeFromSortedSet(setId, minRank, maxRank); } }
        public long RemoveRangeFromSortedSetByScore(string setId, double fromScore, double toScore) { using (var client = m_pool.GetClient()) { return client.RemoveRangeFromSortedSetByScore(setId, fromScore, toScore); } }
        public long RemoveRangeFromSortedSetByScore(string setId, long fromScore, long toScore) { using (var client = m_pool.GetClient()) { return client.RemoveRangeFromSortedSetByScore(setId, fromScore, toScore); } }
        public string RemoveStartFromList(string listId) { using (var client = m_pool.GetClient()) { return client.RemoveStartFromList(listId); } }
        public void RenameKey(string fromName, string toName) { using (var client = m_pool.GetClient()) { client.RenameKey(fromName, toName); } }
        public void RewriteAppendOnlyFileAsync() { using (var client = m_pool.GetClient()) { client.RewriteAppendOnlyFileAsync(); } }
        public void Save() { using (var client = m_pool.GetClient()) { client.Save(); } }
        public void SaveAsync() { using (var client = m_pool.GetClient()) { client.SaveAsync(); } }
        public List<string> SearchKeys(string pattern) { using (var client = m_pool.GetClient()) { return client.SearchKeys(pattern); } }
        public void SetAll(Dictionary<string, string> map) { using (var client = m_pool.GetClient()) { client.SetAll(map); } }
        public void SetAll(IEnumerable<string> keys, IEnumerable<string> values) { using (var client = m_pool.GetClient()) { client.SetAll(keys, values); } }
        public bool SetContainsItem(string setId, string item) { using (var client = m_pool.GetClient()) { return client.SetContainsItem(setId, item); } }
        public void SetEntry(string key, string value) { using (var client = m_pool.GetClient()) { client.SetEntry(key, value); } }
        public void SetEntry(string key, string value, TimeSpan expireIn) { using (var client = m_pool.GetClient()) { client.SetEntry(key, value, expireIn); } }
        public bool SetEntryIfNotExists(string key, string value) { using (var client = m_pool.GetClient()) { return client.SetEntryIfNotExists(key, value); } }
        public bool SetEntryInHash(string hashId, string key, string value) { using (var client = m_pool.GetClient()) { return client.SetEntryInHash(hashId, key, value); } }
        public bool SetEntryInHashIfNotExists(string hashId, string key, string value) { using (var client = m_pool.GetClient()) { return client.SetEntryInHashIfNotExists(hashId, key, value); } }
        public void SetItemInList(string listId, int listIndex, string value) { using (var client = m_pool.GetClient()) { client.SetItemInList(listId, listIndex, value); } }
        public void SetRangeInHash(string hashId, IEnumerable<KeyValuePair<string, string>> keyValuePairs) { using (var client = m_pool.GetClient()) { client.SetRangeInHash(hashId, keyValuePairs); } }
        public void Shutdown() { using (var client = m_pool.GetClient()) { client.Shutdown(); } }
        public bool SortedSetContainsItem(string setId, string value) { using (var client = m_pool.GetClient()) { return client.SortedSetContainsItem(setId, value); } }
        public void StoreAsHash<T>(T entity) { using (var client = m_pool.GetClient()) { client.StoreAsHash<T>(entity); } }
        public void StoreDifferencesFromSet(string intoSetId, string fromSetId, params string[] withSetIds) { using (var client = m_pool.GetClient()) { client.StoreDifferencesFromSet(intoSetId, fromSetId, withSetIds); } }
        public void StoreIntersectFromSets(string intoSetId, params string[] setIds) { using (var client = m_pool.GetClient()) { client.StoreIntersectFromSets(intoSetId, setIds); } }
        public long StoreIntersectFromSortedSets(string intoSetId, params string[] setIds) { using (var client = m_pool.GetClient()) { return client.StoreIntersectFromSortedSets(intoSetId, setIds); } }
        public object StoreObject(object entity) { using (var client = m_pool.GetClient()) { return client.StoreObject(entity); } }
        public void StoreUnionFromSets(string intoSetId, params string[] setIds) { using (var client = m_pool.GetClient()) { client.StoreUnionFromSets(intoSetId, setIds); } }
        public long StoreUnionFromSortedSets(string intoSetId, params string[] setIds) { using (var client = m_pool.GetClient()) { return client.StoreUnionFromSortedSets(intoSetId, setIds); } }
        public void TrimList(string listId, int keepStartingFrom, int keepEndingAt) { using (var client = m_pool.GetClient()) { client.TrimList(listId, keepStartingFrom, keepEndingAt); } }
        public void UnWatch() { using (var client = m_pool.GetClient()) { client.UnWatch(); } }
        public void Watch(params string[] keys) { using (var client = m_pool.GetClient()) { client.Watch(keys); } }
        public Dictionary<string, bool> WhichLuaScriptsExists(params string[] sha1Refs) { using (var client = m_pool.GetClient()) { return client.WhichLuaScriptsExists(sha1Refs); } }
        public void WriteAll<TEntity>(IEnumerable<TEntity> entities) { using (var client = m_pool.GetClient()) { client.WriteAll<TEntity>(entities); } }

        IDictionary<string, double> IRedisClient.GetRangeWithScoresFromSortedSetByLowestScore(string setId, long fromScore, long toScore, int? skip, int? take)
        {
            using (var client = m_pool.GetClient())
            {
                return client.GetRangeWithScoresFromSortedSetByLowestScore(setId, fromScore, toScore, skip, take);
            }
        }

        public void Delete<T>(T entity)
        {
            using (var client = m_pool.GetClient())
            {
                client.Delete<T>(entity);
            }
        }

        public void DeleteAll<TEntity>()
        {
            using (var client = m_pool.GetClient())
            {
                client.DeleteAll<TEntity>();
            }
        }

        public void DeleteById<T>(object id)
        {
            using (var client = m_pool.GetClient())
            {
                client.DeleteById<T>(id);
            }
        }

        public void DeleteByIds<T>(ICollection ids)
        {
            using (var client = m_pool.GetClient())
            {
                client.DeleteByIds<T>(ids);
            }
        }

        public T GetById<T>(object id)
        {
            using (var client = m_pool.GetClient())
            {
                return client.GetById<T>(id);
            }
        }

        public IList<T> GetByIds<T>(ICollection ids)
        {
            using (var client = m_pool.GetClient())
            {
                return client.GetByIds<T>(ids);
            }
        }

        public T Store<T>(T entity)
        {
            using (var client = m_pool.GetClient())
            {
                return client.Store<T>(entity);
            }
        }

        public void StoreAll<TEntity>(IEnumerable<TEntity> entities)
        {
            using (var client = m_pool.GetClient())
            {
                client.StoreAll<TEntity>(entities);
            }
        }

        public void Dispose()
        {
            using (var client = m_pool.GetClient())
            {
                client.Dispose();
            }
        }

        public bool Add<T>(string key, T value, TimeSpan expiresIn)
        {
            using (var client = m_pool.GetClient())
            {
                return client.Add<T>(key, value, expiresIn);
            }
        }

        public bool Add<T>(string key, T value, DateTime expiresAt)
        {
            using (var client = m_pool.GetClient())
            {
                return client.Add<T>(key, value, expiresAt);
            }
        }

        public bool Add<T>(string key, T value)
        {
            using (var client = m_pool.GetClient())
            {
                return client.Add<T>(key, value);
            }
        }

        public long Decrement(string key, uint amount)
        {
            using (var client = m_pool.GetClient())
            {
                return client.Decrement(key, amount);
            }
        }

        public void FlushAll()
        {
            using (var client = m_pool.GetClient())
            {
                client.FlushAll();
            }
        }

        public T Get<T>(string key)
        {
            using (var client = m_pool.GetClient())
            {
                return client.Get<T>(key);
            }
        }

        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            using (var client = m_pool.GetClient())
            {
                return client.GetAll<T>(keys);

            }
        }

        public long Increment(string key, uint amount)
        {
            using (var client = m_pool.GetClient())
            {
                return client.Increment(key, amount);

            }
        }

        public bool Remove(string key)
        {
            using (var client = m_pool.GetClient())
            {
                return client.Remove(key);
            }
        }

        public void RemoveAll(IEnumerable<string> keys)
        {
            using (var client = m_pool.GetClient())
            {
                client.RemoveAll(keys);
            }
        }

        public bool Replace<T>(string key, T value, TimeSpan expiresIn)
        {
            using (var client = m_pool.GetClient())
            {
                return client.Replace<T>(key, value, expiresIn);
            }
        }

        public bool Replace<T>(string key, T value, DateTime expiresAt)
        {
            using (var client = m_pool.GetClient())
            {
                return client.Replace<T>(key, value, expiresAt);
            }
        }

        public bool Replace<T>(string key, T value)
        {
            using (var client = m_pool.GetClient())
            {
                return client.Replace<T>(key, value);
            }
        }

        public bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            using (var client = m_pool.GetClient())
            {
                return client.Set<T>(key, value, expiresIn);
            }
        }

        public bool Set<T>(string key, T value, DateTime expiresAt)
        {
            using (var client = m_pool.GetClient())
            {
                return client.Set<T>(key, value, expiresAt);
            }
        }

        public bool Set<T>(string key, T value)
        {
            using (var client = m_pool.GetClient())
            {
                return client.Set<T>(key, value);
            }
        }

        public void SetAll<T>(IDictionary<string, T> values)
        {
            using (var client = m_pool.GetClient())
            {
                client.SetAll<T>(values);
            }
        }
        #endregion
    }
}
#endif
