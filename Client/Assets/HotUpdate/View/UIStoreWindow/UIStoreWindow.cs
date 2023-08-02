using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Controller;
namespace View
{
    public class UIStoreWindow : BaseWindow
    {
        public UIStoreWindow()
        {
            resName = "UIStoreWindow";
            resident = true;
            windowType = WindowType.StoreWindow;
            scenesType = ScenesType.Login;
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void RegisterUIEvent()
        {
            base.RegisterUIEvent();
            for (int i = 0; i < buttonList.Length; i++)
            {
                switch (buttonList[i].name)
                {
                    case "BuyBtn":
                        buttonList[i].onClick.AddListener(OnClickBuy);
                        break;
                }
            }
        }

        protected override void OnAddListener()
        {
            base.OnAddListener();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            if (Input.GetKeyDown(KeyCode.C))
            {
                Close();
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                Prop prop = StoreController.Instance.GetProp(2);
                Debug.Log($"使用了道具{prop.Name},作用是:{prop.describe}");
            }
        }

        protected override void OnRemoveListener()
        {
            base.OnRemoveListener();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        private void OnClickBuy()
        {
            Debug.Log("点击了BuyButton");
            StoreController.Instance.SaveProp(new Prop()
                { id = 2, Name = "红瓶", describe = "使用恢复100点生命值", price = 100 });
        }
    }
}