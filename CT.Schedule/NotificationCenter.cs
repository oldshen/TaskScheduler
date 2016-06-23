using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CT.Schedule.Core
{
    /// <summary>
    /// 订阅者
    /// </summary>
    public class Subscribe
    {
        public string TypeName { get; set; }

        public string NotificationName { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is Subscribe)
            {
                Subscribe subscribe = (Subscribe)obj;
                return subscribe.TypeName == TypeName && subscribe.NotificationName == NotificationName;
            }
            return false;

        }

        public override int GetHashCode()
        {
            // return base.GetHashCode();
            return this.TypeName.GetHashCode() ^ this.NotificationName.GetHashCode();
        }
    }

    /// <summary>
    /// 消息通知中心
    /// </summary>
    public class NotificationCenter
    {
        public static readonly NotificationCenter Instance = new NotificationCenter();

        ConcurrentDictionary<string, ConcurrentDictionary<object, ActionBlock<object>>> subscribeList = new ConcurrentDictionary<string, ConcurrentDictionary<object, ActionBlock<object>>>();
        ConcurrentDictionary<string, BroadcastBlock<object>> broadcastList = new ConcurrentDictionary<string, BroadcastBlock<object>>();
        /// <summary>
        /// 发送一个通知
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="notificationName"></param>
        /// <param name="obj"></param>
        public void PostNotificationName<T>(string notificationName, T obj)
        {
            BroadcastBlock<object> broadcast;
            if (broadcastList.TryGetValue(notificationName, out broadcast))
            {
                broadcast.Post(obj);
            }
        }

        /// <summary>
        /// 新增观察者
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddObserver(object observer, string notificationName, Action<object> action)
        {

            ConcurrentDictionary<object, ActionBlock<object>> dicActionBlock;
            if (!subscribeList.TryGetValue(notificationName, out dicActionBlock)) {
                dicActionBlock = new ConcurrentDictionary<object, ActionBlock<object>>();
                subscribeList.TryAdd(notificationName, dicActionBlock);
            }
            ActionBlock<object> handler;
            if (dicActionBlock.TryGetValue(observer, out handler)) {
                return;
            }

            handler = new ActionBlock<object>(obj =>
            {
                if (action != null)
                {
                    action(obj);
                }
            });
            dicActionBlock.TryAdd(observer, handler);

     
            BroadcastBlock<object> broadcast;
            if (!broadcastList.TryGetValue(notificationName, out broadcast)) {
                broadcast = new BroadcastBlock<object>(msg => msg);
                broadcastList.TryAdd(notificationName, broadcast);
            }
                       
            broadcast.LinkTo(handler);
           
        }

        /// <summary>
        /// 移除观察者
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="observerObj"></param>
        public void RemoveObserver(object observer)
        {
            foreach (var kv in subscribeList) {
                ActionBlock<object> handler;
                if (kv.Value.TryRemove(observer, out handler)) {
                    handler.Complete();
                }
            }
        }
    }
}
