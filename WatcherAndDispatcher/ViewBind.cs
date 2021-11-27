using System;
using System.Linq;
using System.Windows.Forms;

namespace WatcherAndDispatcher
{

    /// <summary>
    /// 我们的规则是这样的 
    ///
    ///绑定属性： data-控件要绑定的属性名-模型对应的属性名  ，比如给 控件的 Text 属性绑定 模型对象 的 Name 属性 则 data-Text-Name.
    ///
    ///绑定事件：ev-控件的事件名-模型中的方法名 ，如控件的 Click 事件 绑定模型中 Change 方法 则 ev-Click-Change
    ///
    ///一个控件绑定多个属性或者事件用|隔开 ，如 data-Text-Name|ev-Click-Change
    /// </summary>
    public class ViewBind
    {
        /// <summary>
        /// 默认绑定事件
        /// </summary>
        private string DefaultBindingEvents = "CollectionChange|SelectedValueChanged|ValueChanged|TextChanged";
        //private string Perpertis = "DataSource|Value|Text";

        /// <summary>
        /// 绑定视图
        /// </summary>
        /// <param name="parentControl">父控件</param>
        /// <param name="model">模型（对象）</param>
        public ViewBind(Control parentControl, object model)
        {
            this.BindingParentControl(parentControl, model);
        }

        /// <summary>
        /// 绑定控件
        /// </summary>
        /// <param name="parentControl">父控件</param>
        /// <param name="model">实体</param>
        private void BindingParentControl(Control parentControl, object model)
        {
            this.BindControl(parentControl, model, parentControl.Controls);
        }

        /// <summary>
        /// 绑定控件
        /// </summary>
        /// <param name="parentControl">父控件</param>
        /// <param name="model">实体</param>
        /// <param name="controls">子控件列表</param>
        private void BindControl(Control parentControl, object model, Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                var tag = control.Tag;
                if (tag == null) continue;
                foreach (var tagInfo in tag.ToString().Split('|'))
                {
                    var tagInfoArr = tagInfo.Split('-');
                    if (tagInfoArr[0].Equals("data") && tagInfoArr.Length == 3)
                    {
                        //数目绑定
                        string propertyName = tagInfoArr[tagInfoArr.Length - 1];
                        this.BindingProperty(parentControl, control, model, propertyName, tagInfoArr[1]);
                        this.BindListener(control, model, propertyName, tagInfoArr[1]);
                    }
                    else if (tagInfoArr[0].Equals("ev") && tagInfoArr.Length == 3)
                    {
                        //事件绑定
                        BindEvent(parentControl, control, model, tagInfoArr[1], tagInfoArr[2]);
                    }
                    else
                    {
                        if (control.Controls.Count > 0)
                        {
                            this.BindControl(parentControl, model, control.Controls);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 绑定事件
        /// </summary>
        /// <param name="parentControl">父控件</param>
        /// <param name="control">要绑定事件的控件</param>
        /// <param name="model">实体</param>
        /// <param name="eventName">事件名称</param>
        /// <param name="methodName">绑定到的方法</param>
        private void BindEvent(Control parentControl, Control control, object model, string eventName,
            string methodName)
        {
            var Event = control.GetType().GetEvent(eventName);
            if (Event != null)
            {
                var methodInfo = model.GetType().GetMethod(methodName);
                if (methodInfo != null)
                {
                    Event.AddEventHandler(control, new EventHandler((s, e) =>
                    {
                        parentControl.Invoke(new Action(() =>
                        {
                            switch (methodInfo.GetParameters().Count())
                            {
                                case 0:
                                    methodInfo.Invoke(model, null);
                                    break;
                                case 1:
                                    methodInfo.Invoke(model, new object[] {new {s = s, e = e}});
                                    break;
                                case 2:
                                    methodInfo.Invoke(model, new object[] {s, e});
                                    break;
                                default: break;
                            }
                        }));
                    }));
                }
            }
        }

        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="control">要监听的控件</param>
        /// <param name="model">实体</param>
        /// <param name="mPropertyName">变化的实体属性</param>
        /// <param name="controlPropertyName">对应变化的控件属性</param>
        private void BindListener(Control control, object model, string mPropertyName, string controlPropertyName)
        {
            var property = model.GetType().GetProperty(mPropertyName);

            var events = this.DefaultBindingEvents.Split('|');
            foreach (var ev in events)
            {
                var Event = control.GetType().GetEvent(ev);
                if (Event != null)
                {
                    Event.AddEventHandler(control, new EventHandler((s, e) =>
                    {
                        try
                        {
                            var controlProperty = control.GetType().GetProperty(controlPropertyName);
                            if (controlProperty != null)
                            {
                                property.SetValue(model,
                                    Convert.ChangeType(controlProperty.GetValue(control, null), property.PropertyType),
                                    null);
                            }
                        }
                        catch (Exception ex)
                        {
                            //TODO
                        }
                    }));
                }
            }
        }

        /// <summary>
        /// 绑定属性
        /// </summary>
        /// <param name="parentControl">父控件</param>
        /// <param name="control">绑定属性的控件</param>
        /// <param name="model">实体</param>
        /// <param name="mPropertyName">绑定的实体属性名称</param>
        /// <param name="controlPropertyName">绑定到的控件的属性名称</param>
        private void BindingProperty(Control parentControl, Control control, object model, string mPropertyName,
            string controlPropertyName)
        {
            Action<object> action = info =>
            {
                try
                {
                    var controlType = control.GetType();
                    var mType = info.GetType().GetProperty(mPropertyName);
                    var controlProperty = controlType.GetProperty(controlPropertyName);
                    if (controlProperty != null)
                    {
                        controlProperty.SetValue(control,
                            Convert.ChangeType(mType.GetValue(info, null), controlProperty.PropertyType), null);
                    }
                }
                catch (Exception ex)
                {
                    //TODO
                }
            };
            //添加到监听
            new Watcher(parentControl, model.GetType(), model, mPropertyName, action);
            //初始化数据（将实体数据赋给控件属性）
            action(model);
        }
    }
}