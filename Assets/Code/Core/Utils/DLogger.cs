using System;

namespace Code.Core.Utils
{
    public static class DLogger
    {
        private static string GenerateSourceString(Type classSource, string methodSource)
        {
            return $"<b><color=grey>[<color=teal>{classSource.Name}</color>:<color=green>{methodSource}</color>]</color></b>";
        }
    
        public static void Info(Type classSource, string methodSource, string text)
        {
#if UNITY_EDITOR
            var source = GenerateSourceString(classSource, methodSource);
            UnityEngine.Debug.Log($"{source} <color=white>{text}</color>");
#endif
        }
    
        public static void Debug(Type classSource, string methodSource, string text)
        {
#if UNITY_EDITOR
            var source = GenerateSourceString(classSource, methodSource);
            UnityEngine.Debug.Log($"{source} <color=aqua>{text}</color>");
#endif
        }

        public static void Warning(Type classSource, string methodSource, string text)
        {
#if UNITY_EDITOR
            var source = GenerateSourceString(classSource, methodSource);
            UnityEngine.Debug.LogWarning($"{source} <color=yellow>{text}</color>");
#endif
        }
    
        public static void Error(Type classSource, string methodSource, string text)
        {
#if UNITY_EDITOR
            var source = GenerateSourceString(classSource, methodSource);
            UnityEngine.Debug.LogError($"{source} <color=red><b>{text}</b></color>");
#endif
        }
    }
}