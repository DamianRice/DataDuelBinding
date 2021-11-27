using System;
using System.Reflection;
using System.Windows.Forms;

namespace WatcherAndDispatcher
{
    /// <summary>
    /// 监听者
    /// </summary>
    public class Watcher : IWatcher
    {
        /// <summary>
        /// 实体类型
        /// </summary>
        private Type type = null;

        /// <summary>
        /// 属性变化触发的委托
        /// </summary>
        private Action<object> Action = null;

        /// <summary>
        /// 属性名称
        /// </summary>
        private string propertyName = null;

        /// <summary>
        /// 父控件
        /// </summary>
        private Control ParentControl = null;

        /// <summary>
        /// 实体
        /// </summary>
        private object model = null;

        /// <summary>
        /// 初始化监听者
        /// </summary>
        /// <param name="parentControl">父控件</param>
        /// <param name="type">实体类型</param>
        /// <param name="model">实体</param>
        /// <param name="propertyName">要监听的属性名称</param>
        /// <param name="action">属性变化触发的委托</param>
        public Watcher(Control parentControl, Type type, object model, string propertyName, Action<object> action)
        {
            this.type = type;
            this.Action = action;
            this.propertyName = propertyName;
            this.ParentControl = parentControl;
            this.model = model;
            this.AddToDip();
        }

        /// <summary>
        /// 添加监听者到属性的订阅者列表（Dispatcher）
        /// </summary>
        private void AddToDip()
        {
            PropertyInfo property = this.type.GetProperty(this.propertyName);
            if (property == null) return;
            Dispatcher.Target = this;
            object value = property.GetValue(this.model, null);
            Dispatcher.Target = null;
        }

        /// <summary>
        /// 更新数据（监听触发的方法）
        /// </summary>
        public void Update()
        {
            this.ParentControl.Invoke(new Action(delegate { this.Action(this.model); }));
        }
    }
}