using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using JinRi.Notify.Frame;
using Metrics;
using System.Configuration;

namespace JinRi.Notify.Frame.Metrics
{
    public static class JMetricsHelper
    {
        private static MetricTags DEFAULT_TAGS = new MetricTags(string.Format("AppId={0}", ConfigurationManager.AppSettings["AppId"]), string.Format("ServerIP={0}", IPHelper.GetLocalIP()));

        #region Meter

        public static void MeterMark(this string key)
        {
            MeterMark(key, null, null);
        }

        public static void MeterMark(this string key, string[] tagArr)
        {
            MeterMark(key, null, tagArr);
        }

        public static void MeterMark(this string key, string item)
        {
            MeterMark(key, item, null);
        }

        public static void MeterMark(this string key, string item, string[] tagArr, string unit = "次")
        {
            try
            {
                MetricTags tags = DEFAULT_TAGS;
                if (tagArr != null && tagArr.Length > 0)
                {
                    List<string> tagList = new List<string>(tags.Tags.Length + DEFAULT_TAGS.Tags.Length);
                    tagList.AddRange(DEFAULT_TAGS.Tags);
                    tagList.AddRange(tagArr);
                    tags = new MetricTags(tagList);
                }
                Meter meter = JMetric.Meter(key, Unit.Custom(unit), TimeUnit.Seconds, tags);

                if (string.IsNullOrWhiteSpace(item))
                {
                    meter.Mark();
                }
                else
                {
                    meter.Mark(item);
                }
            }
            catch { }
        }

        #endregion

        #region Histogram

        public static void HistogramUpdate(this string key, long value)
        {
            HistogramUpdate(key, null, value, null);
        }

        public static void HistogramUpdate(this string key, long value, string[] tagArr)
        {
            HistogramUpdate(key, null, value, tagArr);
        }

        public static void HistogramUpdate(this string key, string userValue, long value)
        {
            HistogramUpdate(key, userValue, value, null);
        }

        public static void HistogramUpdate(this string key, string userValue, long value, string[] tagArr, string unit = "毫秒")
        {
            try
            {
                MetricTags tags = DEFAULT_TAGS;
                if (tagArr != null && tagArr.Length > 0)
                {
                    List<string> tagList = new List<string>(tags.Tags.Length + DEFAULT_TAGS.Tags.Length);
                    tagList.AddRange(DEFAULT_TAGS.Tags);
                    tagList.AddRange(tagArr);
                    tags = new MetricTags(tagList);
                }
                Histogram histogram = JMetric.Histogram(key, Unit.Custom(unit), SamplingType.FavourRecent, tags);
                if (string.IsNullOrWhiteSpace(userValue))
                {
                    histogram.Update(value);
                }
                else
                {
                    histogram.Update(value, userValue);
                }
            }
            catch { }
        }

        #endregion
    }
}
