using DJFrameWork.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DJFrameWork.StateMachine
{
    /// <summary>
    /// 状态机
    /// </summary>
    public class DJStateMachine
    {
        //缓存状态节点
        private readonly Dictionary<string, IStateNode> nodes = new Dictionary<string, IStateNode>();
        //当前节点
        private IStateNode curNode;
        //上一个节点
        private IStateNode preNode;
        /// <summary>
        /// 状态机持有者
        /// </summary>
        public System.Object Owner { private set; get; }

        /// <summary>
        /// 当前节点名称
        /// </summary>
        public string CurrentNode
        {
            get
            {
                return curNode != null ? curNode.GetType().FullName : string.Empty;
            }
        }
        /// <summary>
        /// 前一个节点的名称
        /// </summary>
        public string PreviousNode
        {
            get
            {
                return preNode != null ? preNode.GetType().FullName : string.Empty;
            }
        }

        private DJStateMachine()
        {

        }

        public DJStateMachine(System.Object owner)
        {
            this.Owner = owner;
        }

        /// <summary>
        /// 更新状态机
        /// </summary>
        public void Update()
        {
            if(curNode!=null)
            {
                curNode.OnUpdate();
            }
        }

        /// <summary>
        /// 启动状态机
        /// </summary>
        /// <typeparam name="TNode">状态机节点</typeparam>
        public void Run<TNode>() where TNode : IStateNode
        {
            var nodeType = typeof(TNode);
            var nodeName = nodeType.FullName;
            Run(nodeName);
        }
        /// <summary>
        /// 启动状态机
        /// </summary>
        /// <param name="entryNode">进入的节点</param>
        public void Run(Type entryNode)
        {
            var nodeName = entryNode.FullName;
            Run(nodeName);
        }
        /// <summary>
        /// 启动状态机
        /// </summary>
        /// <param name="entryNode">启动节点</param>
        /// <exception cref="Exception"></exception>
        public void Run(string entryNode)
        {
            curNode = TryGetNode(entryNode);
            preNode = curNode;
            if(curNode == null) 
            {
                throw new Exception($"{entryNode}状态节点不存在");
            }
            curNode.OnEnter();
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <typeparam name="TNode">节点</typeparam>
        public void AddNode<TNode>() where TNode:IStateNode
        {
            System.Type nodeType = typeof(TNode);
            IStateNode node = Activator.CreateInstance(nodeType) as IStateNode;
            AddNode(node);
        }

        /// <summary>
        /// 添加一个节点
        /// </summary>
        /// <param name="node">节点</param>
        public void AddNode(IStateNode stateNode)
        {
            if(stateNode==null)
            {
                throw new ArgumentNullException("state");
            }
            System.Type nodeType = stateNode.GetType();
            string nodeName = nodeType.FullName;
            if(!nodes.ContainsKey(nodeName))
            {
                stateNode.OnCreate(this);
                nodes.Add(nodeName, stateNode);
            }
            else
            {
                DJLog.Error($"{nodeName}节点已经存在");
            }
        }

        /// <summary>
        /// 切换状态机状态
        /// </summary>
        /// <typeparam name="TNode">目标状态</typeparam>
        public void ChangeStateNode<TNode>() where TNode : IStateNode
        {
            System.Type nodeType = typeof (TNode);
            string nodeName = nodeType.FullName;
            ChangeStateNode(nodeName);
        }
        /// <summary>
        /// 切换状态机状态
        /// </summary>
        /// <param name="nodeType">目标状态节点类型</param>
        public void ChangeStateNode(Type nodeType)
        {
            string nodeName = nodeType.FullName;
            ChangeStateNode(nodeName);
        }
        /// <summary>
        /// 切换状态机节点
        /// </summary>
        /// <param name="nodeName">目标节点名称</param>
        public void ChangeStateNode(string nodeName)
        {
            if(string.IsNullOrEmpty(nodeName))
            {
                throw new ArgumentNullException();
            }
            IStateNode node = TryGetNode(nodeName);
            if(node==null)
            {
                DJLog.Error($"目标节点:{nodeName}不存在");
                return;
            }
            //切换节点 执行对应事件
            preNode = curNode;
            curNode.OnExit();
            curNode = node;
            curNode.OnEnter();
        }

        /// <summary>
        /// 获取状态节点
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        private IStateNode TryGetNode(string nodeName)
        {
            nodes.TryGetValue(nodeName, out IStateNode result);
            return result;
        }
    }
}

