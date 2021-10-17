using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace H3VRAnimator.Logging
{
    public static class AnimLogger
    {

        public static ManualLogSource BepLog;

        public static void Init()
        {
            BepLog = Logger.CreateLogSource("H3VRAnimator");
        }

        public static void Log(string log)
        {
            BepLog.LogInfo(log);
        }

        public static void LogWarning(string log)
        {
            BepLog.LogWarning(log);
        }

        public static void LogError(string log)
        {
            BepLog.LogError(log);
        }

    }
}
