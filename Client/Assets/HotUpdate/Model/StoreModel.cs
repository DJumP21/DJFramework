using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    /// <summary>
    /// 商店Model
    /// </summary>
    public class StoreModel:Singletion<StoreModel>
    {
        private Dictionary<int, Prop> propDic = new Dictionary<int, Prop>();

        #region 公开接口

        //添加道具
        public void Add(Prop prop)
        {
            if (!propDic.ContainsKey(prop.id))
            {
                propDic.Add(prop.id,prop);
            }
        }

        public Prop GetProp(int id)
        {
            return propDic[id];
        }
        #endregion
    }

}
/// <summary>
/// 道具
/// </summary>
public class Prop
{
    public int id;
    public string Name;
    public string describe;
    public int price;
}
