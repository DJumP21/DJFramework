using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;

namespace Controller
{
    public class StoreController : Singletion<StoreController>
    {
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="prop"></param>
        public void SaveProp(Prop prop)
        {
            StoreModel.Instance.Add(prop);
        }

        /// <summary>
        /// 获取Prop
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Prop GetProp(int id)
        {
            return StoreModel.Instance.GetProp(id);
        }
    }
}

