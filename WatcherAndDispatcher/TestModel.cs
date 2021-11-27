using System;
using System.Collections.Generic;

namespace WatcherAndDispatcher
{
    public class TestModel
    {

        List<Dispatcher> dep = Dispatcher.InitDeps(3);

        public int Value
        {
            get => dep[0].Get<int>();
            set => dep[0].Set(value);
        }

        public string Text
        {
            get => dep[1].Get<string>();
            set => dep[1].Set(value);
        }

        public string BtnName
        {
            get => dep[2].Get<string>();
            set => dep[2].Set(value);
        }

        public void BtnClick(object o)
        {
            this.BtnName = string.Format("绑定事件{0}", DateTime.Now.Millisecond);

        }
    }
}