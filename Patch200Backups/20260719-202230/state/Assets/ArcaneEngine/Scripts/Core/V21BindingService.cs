using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcaneEngine
{
    public enum V21BindingAction
    {
        MoveForward, MoveBack, MoveLeft, MoveRight, SpellSlot3, Dodge, Interact,
        Workshop, SpellLinks, Inventory, Help, Map, CancelCast
    }

    public enum V21BindingConflictPolicy { Cancel, Swap, Replace }

    public static class V21BindingService
    {
        public static IEnumerable<V21BindingAction> All
        {
            get { return (V21BindingAction[])Enum.GetValues(typeof(V21BindingAction)); }
        }

        public static KeyCode Get(V21BindingAction action)
        {
            ControlSettings value = ProfileManager.Current.controls;
            switch (action)
            {
                case V21BindingAction.MoveForward: return value.moveForward;
                case V21BindingAction.MoveBack: return value.moveBack;
                case V21BindingAction.MoveLeft: return value.moveLeft;
                case V21BindingAction.MoveRight: return value.moveRight;
                case V21BindingAction.SpellSlot3: return value.spellSlot3;
                case V21BindingAction.Dodge: return value.dodge;
                case V21BindingAction.Interact: return value.interact;
                case V21BindingAction.Workshop: return value.workshop;
                case V21BindingAction.SpellLinks: return value.spellLinks;
                case V21BindingAction.Inventory: return value.inventory;
                case V21BindingAction.Help: return value.help;
                case V21BindingAction.Map: return value.map;
                default: return value.cancelCast;
            }
        }

        public static bool TryAssign(V21BindingAction action, KeyCode key, V21BindingConflictPolicy policy,
            out V21BindingAction? conflict, out string message)
        {
            conflict = null;
            if (key == KeyCode.None) { message = "Essential actions cannot be left unbound."; return false; }
            foreach (V21BindingAction candidate in All)
                if (candidate != action && Get(candidate) == key) { conflict = candidate; break; }
            if (conflict.HasValue && policy == V21BindingConflictPolicy.Cancel)
            {
                message = key + " is already bound to " + Friendly(conflict.Value) + ". Choose Swap, Replace, or Cancel.";
                return false;
            }
            KeyCode old = Get(action);
            if (conflict.HasValue)
            {
                if (policy == V21BindingConflictPolicy.Swap) Set(conflict.Value, old);
                else Set(conflict.Value, KeyCode.None);
            }
            Set(action, key);
            ProfileManager.Save();
            message = Friendly(action) + " is now bound to " + key + ".";
            return true;
        }

        public static void ResetDefaults()
        {
            ProfileManager.Current.controls = new ControlSettings();
            ProfileManager.Save();
        }

        public static string Friendly(V21BindingAction action)
        {
            string value = action.ToString();
            return System.Text.RegularExpressions.Regex.Replace(value, "([a-z])([A-Z])", "$1 $2");
        }

        private static void Set(V21BindingAction action, KeyCode key)
        {
            ControlSettings value = ProfileManager.Current.controls;
            switch (action)
            {
                case V21BindingAction.MoveForward: value.moveForward = key; break;
                case V21BindingAction.MoveBack: value.moveBack = key; break;
                case V21BindingAction.MoveLeft: value.moveLeft = key; break;
                case V21BindingAction.MoveRight: value.moveRight = key; break;
                case V21BindingAction.SpellSlot3: value.spellSlot3 = key; break;
                case V21BindingAction.Dodge: value.dodge = key; break;
                case V21BindingAction.Interact: value.interact = key; break;
                case V21BindingAction.Workshop: value.workshop = key; break;
                case V21BindingAction.SpellLinks: value.spellLinks = key; break;
                case V21BindingAction.Inventory: value.inventory = key; break;
                case V21BindingAction.Help: value.help = key; break;
                case V21BindingAction.Map: value.map = key; break;
                default: value.cancelCast = key; break;
            }
        }
    }
}
