using System.Collections.Generic;
using GTA;
using GTA.Native;

namespace iFruitAddon2
{
    public class IFruitContactCollection : List<iFruitContact>
    {
        public static int CurrentIndex = 40;
        private bool _shouldDraw = true;
        private readonly int _mScriptHash;

        public IFruitContactCollection()
        {
            _mScriptHash = Game.GenerateHash("appcontacts");
        }

        
        internal void Update(int handle)
        {
            var selectedIndex = 0;

            // If we are in the Contacts menu
            if (Function.Call<int>(Hash._GET_NUMBER_OF_REFERENCES_OF_SCRIPT_WITH_NAME_HASH, _mScriptHash) > 0)
            {
                _shouldDraw = true;

                if (Game.IsControlPressed(Control.PhoneSelect))
                    selectedIndex = GetSelectedIndex(handle);  // We must use this function only when necessary since it contains Script.Wait(0)
            }
            else
                selectedIndex = -1;

            // Browsing every added contacts
            foreach (var contact in this)
            {
                contact.Update(); // Update sounds or Answer call when _callTimer has ended.

                if (_shouldDraw)
                    contact.Draw(handle);

                if (selectedIndex != -1 && selectedIndex == contact.Index)
                {
                    // Prevent original contact to be called
                    Tools.Scripts.TerminateScript("appcontacts");

                    contact.Call();
                    DisplayCallUi(handle, contact.Name, "CELL_211", contact.Icon.Name.SetBold(contact.Bold));

                    Script.Wait(10);
                    
                    RemoveActiveNotification();
                    
                }

            }
            _shouldDraw = false;
        }
       

        /// <summary>
        /// Display the current call on the phone.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="contactName"></param>
        /// <param name="statusText">CELL_211 = "DIALING..." / CELL_219 = "CONNECTED"</param>
        /// <param name="picName"></param>
        public static void DisplayCallUi(int handle, string contactName, string statusText = "CELL_211", string picName = "CELL_300")
        {
            var dialText = Function.Call<string>(Hash._GET_LABEL_TEXT, statusText); // "DIALING..." translated in current game's language

            Function.Call(Hash.BEGIN_SCALEFORM_MOVIE_METHOD, handle, "SET_DATA_SLOT");
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, 4);
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, 0);
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, 3);

            Function.Call(Hash.BEGIN_TEXT_COMMAND_SCALEFORM_STRING, "STRING");
            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PHONE_NUMBER, contactName, -1);       //UI::_ADD_TEXT_COMPONENT_APP_TITLE
            Function.Call(Hash.END_TEXT_COMMAND_SCALEFORM_STRING);

            Function.Call(Hash.BEGIN_TEXT_COMMAND_SCALEFORM_STRING, "CELL_2000");
            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, picName);
            Function.Call(Hash.END_TEXT_COMMAND_SCALEFORM_STRING);

            Function.Call(Hash.BEGIN_TEXT_COMMAND_SCALEFORM_STRING, "STRING");
            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PHONE_NUMBER, dialText, -1);      //UI::_ADD_TEXT_COMPONENT_APP_TITLE
            Function.Call(Hash.END_TEXT_COMMAND_SCALEFORM_STRING);

            Function.Call(Hash.END_SCALEFORM_MOVIE_METHOD);

            Function.Call(Hash.BEGIN_SCALEFORM_MOVIE_METHOD, handle, "DISPLAY_VIEW");
            Function.Call(Hash.SCALEFORM_MOVIE_METHOD_ADD_PARAM_INT, 4);
            Function.Call(Hash.END_SCALEFORM_MOVIE_METHOD);
        }

        /// <summary>
        /// Get the index of the current highlighted contact.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        internal int GetSelectedIndex(int handle)
        {
            Function.Call(Hash.BEGIN_SCALEFORM_MOVIE_METHOD, handle, "GET_CURRENT_SELECTION");
            var num = Function.Call<int>(Hash.END_SCALEFORM_MOVIE_METHOD_RETURN_VALUE);
            while (!Function.Call<bool>(Hash.IS_SCALEFORM_MOVIE_METHOD_RETURN_VALUE_READY, num))         //UI::_GET_SCALEFORM_MOVIE_FUNCTION_RETURN_BOOL
                Script.Wait(0);
            var data = Function.Call<int>(Hash.GET_SCALEFORM_MOVIE_METHOD_RETURN_VALUE_INT, num);       //UI::_GET_SCALEFORM_MOVIE_FUNCTION_RETURN_INT
            return data;
        }

        /// <summary>
        /// Remove the current notification.
        /// Useful to remove "The selected contact is no longer available" when you try to call a contact that shouldn't exist (ie: contacts added by iFruitAddon).
        /// </summary>
        internal void RemoveActiveNotification()
        {
            Function.Call(Hash.BEGIN_TEXT_COMMAND_THEFEED_POST, "STRING");
            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, "temp");
            var temp = Function.Call<int>(Hash.END_TEXT_COMMAND_THEFEED_POST_TICKER, false, 1);
            Function.Call(Hash.THEFEED_REMOVE_ITEM, temp);
            Function.Call(Hash.THEFEED_REMOVE_ITEM, temp - 1);
        }
    }
}