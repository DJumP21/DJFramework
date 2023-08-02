using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DJFrameWork.StateMachine
{
    /// <summary>
    /// ×´Ì¬»ú½Úµã
    /// </summary>
    public interface IStateNode
    {
        void OnCreate(DJStateMachine machine);
        void OnEnter();
        void OnUpdate();
        void OnExit();
    }

}
