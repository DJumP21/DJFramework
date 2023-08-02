using DJFrameWork.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DJFrameWork.StateMachine
{
    /// <summary>
    /// ״̬��
    /// </summary>
    public class DJStateMachine
    {
        //����״̬�ڵ�
        private readonly Dictionary<string, IStateNode> nodes = new Dictionary<string, IStateNode>();
        //��ǰ�ڵ�
        private IStateNode curNode;
        //��һ���ڵ�
        private IStateNode preNode;
        /// <summary>
        /// ״̬��������
        /// </summary>
        public System.Object Owner { private set; get; }

        /// <summary>
        /// ��ǰ�ڵ�����
        /// </summary>
        public string CurrentNode
        {
            get
            {
                return curNode != null ? curNode.GetType().FullName : string.Empty;
            }
        }
        /// <summary>
        /// ǰһ���ڵ������
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
        /// ����״̬��
        /// </summary>
        public void Update()
        {
            if(curNode!=null)
            {
                curNode.OnUpdate();
            }
        }

        /// <summary>
        /// ����״̬��
        /// </summary>
        /// <typeparam name="TNode">״̬���ڵ�</typeparam>
        public void Run<TNode>() where TNode : IStateNode
        {
            var nodeType = typeof(TNode);
            var nodeName = nodeType.FullName;
            Run(nodeName);
        }
        /// <summary>
        /// ����״̬��
        /// </summary>
        /// <param name="entryNode">����Ľڵ�</param>
        public void Run(Type entryNode)
        {
            var nodeName = entryNode.FullName;
            Run(nodeName);
        }
        /// <summary>
        /// ����״̬��
        /// </summary>
        /// <param name="entryNode">�����ڵ�</param>
        /// <exception cref="Exception"></exception>
        public void Run(string entryNode)
        {
            curNode = TryGetNode(entryNode);
            preNode = curNode;
            if(curNode == null) 
            {
                throw new Exception($"{entryNode}״̬�ڵ㲻����");
            }
            curNode.OnEnter();
        }

        /// <summary>
        /// ��ӽڵ�
        /// </summary>
        /// <typeparam name="TNode">�ڵ�</typeparam>
        public void AddNode<TNode>() where TNode:IStateNode
        {
            System.Type nodeType = typeof(TNode);
            IStateNode node = Activator.CreateInstance(nodeType) as IStateNode;
            AddNode(node);
        }

        /// <summary>
        /// ���һ���ڵ�
        /// </summary>
        /// <param name="node">�ڵ�</param>
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
                DJLog.Error($"{nodeName}�ڵ��Ѿ�����");
            }
        }

        /// <summary>
        /// �л�״̬��״̬
        /// </summary>
        /// <typeparam name="TNode">Ŀ��״̬</typeparam>
        public void ChangeStateNode<TNode>() where TNode : IStateNode
        {
            System.Type nodeType = typeof (TNode);
            string nodeName = nodeType.FullName;
            ChangeStateNode(nodeName);
        }
        /// <summary>
        /// �л�״̬��״̬
        /// </summary>
        /// <param name="nodeType">Ŀ��״̬�ڵ�����</param>
        public void ChangeStateNode(Type nodeType)
        {
            string nodeName = nodeType.FullName;
            ChangeStateNode(nodeName);
        }
        /// <summary>
        /// �л�״̬���ڵ�
        /// </summary>
        /// <param name="nodeName">Ŀ��ڵ�����</param>
        public void ChangeStateNode(string nodeName)
        {
            if(string.IsNullOrEmpty(nodeName))
            {
                throw new ArgumentNullException();
            }
            IStateNode node = TryGetNode(nodeName);
            if(node==null)
            {
                DJLog.Error($"Ŀ��ڵ�:{nodeName}������");
                return;
            }
            //�л��ڵ� ִ�ж�Ӧ�¼�
            preNode = curNode;
            curNode.OnExit();
            curNode = node;
            curNode.OnEnter();
        }

        /// <summary>
        /// ��ȡ״̬�ڵ�
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

