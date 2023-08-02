using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DJFrameWork.Log
{
    public static class DJLog
    {
        public static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        public static void Warning(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        public static void Error(string message)
        {
            UnityEngine.Debug.LogError(message);
        }
    }
}

