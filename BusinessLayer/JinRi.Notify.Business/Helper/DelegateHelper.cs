using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

using JinRi.Notify.Frame;
using JinRi.Notify.Frame.Metrics;
using JinRi.Notify.DTO;
using JinRi.Notify.Utility;
using JinRi.Notify.ServiceModel.Profile;
using Newtonsoft.Json;

namespace JinRi.Notify.Business
{
    public class DelegateHelper
    {
        private static readonly ILog m_logger = LoggerSource.Instance.GetLogger(typeof(DelegateHelper));

        /// <summary>
        /// 委托调用方法
        /// </summary>
        /// <typeparam name="TParam">传入参数</typeparam>
        /// <typeparam name="TResult">返回参数</typeparam>
        /// <param name="func">方法</param>
        /// <param name="request">请求信息</param>
        /// <returns>返回信息</returns>
        public static TResult Invoke<TParam, TResult>(Func<TParam, TResult> func, TParam request)
            where TResult : BaseResult, new()
        {
            return Invoke<TParam, TResult>(func, request, "");
        }
        /// <summary>
        /// 委托调用方法
        /// </summary>
        /// <typeparam name="TParam">传入参数</typeparam>
        /// <typeparam name="TResult">返回参数</typeparam>
        /// <param name="func">方法</param>
        /// <param name="request">请求信息</param>
        /// <param name="metricsKey">度量键</param>
        /// <returns>返回信息</returns>
        public static TResult Invoke<TParam, TResult>(Func<TParam, TResult> func, TParam request, string metricsKey)
            where TResult : BaseResult, new()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            TResult response = new TResult();
            try
            {
                Handle.Info(RequestProfile.RequestType, "JinRi.Notify.Business.DelegateHelper.Invoke<TParam, TResult>", string.Format("请求：{0}", request != null ? JsonConvert.SerializeObject(request) : ""), "请求");
                response = func.Invoke(request);
                if (response == null)
                {
                    response = new TResult() { Success = true };
                }
                if (!string.IsNullOrEmpty(metricsKey)) metricsKey.MeterMark("Success");
            }
            catch (BusinessException ex)
            {
                response.Success = false;
                response.ErrMsg = ex.Message;
                Process.Info(RequestProfile.RequestType, "JinRi.Notify.Business.DelegateHelper.Invoke<TParam, TResult>", string.Format("参数业务异常：{0}", response.ErrMsg), "");
            }
            catch (RabbitMQException ex)
            {
                response.Success = false;
                response.ErrMsg = ex.GetString();
                Process.Error(RequestProfile.RequestType, "JinRi.Notify.Business.DelegateHelper.Invoke<TParam, TResult>", string.Format("消息队列异常：{0}", response.ErrMsg), "");
                m_logger.Error(string.Format("消息队列异常：{0}", response.ErrMsg));
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrMsg = ex.GetString();
                if (!string.IsNullOrEmpty(metricsKey)) metricsKey.MeterMark("Error");
                Process.Error(RequestProfile.RequestType, "JinRi.Notify.Business.DelegateHelper.Invoke<TParam, TResult>", string.Format("系统产生严重异常：{0}", response.ErrMsg), "");
                m_logger.Error(string.Format("系统产生严重异常：{0}", response.ErrMsg));
            }
            finally
            {
                sw.Stop();
                if (!string.IsNullOrEmpty(metricsKey)) metricsKey.HistogramUpdate(sw.ElapsedMilliseconds);
                Handle.Info(RequestProfile.RequestType, "JinRi.Notify.Business.DelegateHelper.Invoke<TParam, TResult>", string.Format("返回：{0}，运行：{1}", response != null ? JsonConvert.SerializeObject(response) : "", sw.ElapsedMilliseconds), "返回");
            }

            return response;
        }
    }
}