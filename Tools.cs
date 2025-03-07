﻿using GTA;
using GTA.Native;

namespace iFruitAddon2;

public static class Helper
{
    public static string SetBold(this string text, bool bold)
    {
        if (bold) return text.ToLower();
        return text.ToUpper();
    }
}

public static class Tools
{
    internal static class Scripts
    {
        internal static void DestroyPhone(int handle)
        {
            Function.Call(Hash.DESTROY_MOBILE_PHONE, handle);
        }

        internal static void StartScript(string scriptName, int buffer)
        {
            Function.Call(Hash.REQUEST_SCRIPT, scriptName);

            while (!Function.Call<bool>(Hash.HAS_SCRIPT_LOADED, scriptName))
            {
                Function.Call(Hash.REQUEST_SCRIPT, scriptName);
                Script.Yield();
            }

            Function.Call(Hash.START_NEW_SCRIPT, scriptName, buffer);
            Function.Call(Hash.SET_SCRIPT_AS_NO_LONGER_NEEDED, scriptName);
        }

        internal static void TerminateScript(string scriptName)
        {
            Function.Call(Hash.TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME, scriptName);
        }

    }
}