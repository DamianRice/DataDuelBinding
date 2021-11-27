using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatcherAndDispatcher
{
    class Dispatcher
    {
        /// <summary>
        /// 全局变量，用户将指定属性的订阅者放入通知列表
        /// </summary>
        public static IWatcher Target = null;

        /// <summary>
        /// 保存属性的值，属性的get set将是对该值的操作
        /// </summary>
        private object oValue;

        /// <summary>
        /// 订阅者列表
        /// </summary>
        private List<IWatcher> listWatcher = null;


        public Dispatcher()
        {
            this.listWatcher = new List<IWatcher>();
        }

        /// <summary>
        /// 添加订阅者
        /// </summary>
        /// <param name="watcher"></param>
        private void PushWatcher(IWatcher watcher)
        {
            this.listWatcher.Add(watcher);
        }

        /// <summary>
        /// 通知
        /// </summary>
        public void Notify()
        {
            List<IWatcher> watchers = this.listWatcher;
            watchers.ForEach(watcher => { watcher.Update(); });
        }

        /// <summary>
        /// 返回属性的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>()
        {
            // Dispatcher.Target 不为空时，标识指向这个属性的一个订阅者，需要将它加入到订阅列表
            bool flag = Dispatcher.Target != null;
            if (flag)
            {
                this.PushWatcher(Dispatcher.Target);
            }

            return this.oValue == null ? default(T) : (T) this.oValue;
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="oValue"></param>
        public void Set(object oValue)
        {
            bool flag = this.oValue == null || !this.oValue.Equals(oValue);
            if (flag)
            {
                this.oValue = oValue;
                this.Notify();
            }
        }

        /// <summary>
        /// 初始化队列，分配给每个属性
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<Dispatcher> InitDeps(int count)
        {
            if (count < 1) throw new Exception("wrong number! count must biger than 0");
            var lsDep = new List<Dispatcher>();
            for (var i = 0; i < count; i++)
            {
                lsDep.Add(new Dispatcher());
            }

            return lsDep;
        }
    }
}