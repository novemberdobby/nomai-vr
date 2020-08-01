using System.Collections.Generic;
using Valve.VR;

namespace NomaiVR
{
    public class VRActionInput
    {
        public bool HideHand = false;
        // todo make readonly
        public string Hand = null;
        public string Source = null;
        public readonly string Color;
        public readonly List<string> Prefixes = new List<string>();

        private ISteamVR_Action_In _action;

        public VRActionInput(ISteamVR_Action_In action, string color, bool isLongPress = false)
        {
            Color = color;
            _action = action;

            if (isLongPress)
            {
                Prefixes.Add("Long Press");
            }
        }

        public VRActionInput(ISteamVR_Action_In action, bool isLongPress = false) : this(action, TextHelper.ORANGE, isLongPress) { }

        public string GetText()
        {
            InitTextParts();
            var prefix = Prefixes.Count > 0 ? $"{TextHelper.TextWithColor(string.Join(" ", Prefixes.ToArray()), TextHelper.ORANGE)} " : "";
            var hand = HideHand ? "" : $"{Hand} ";
            var result = $"{prefix}{TextHelper.TextWithColor($"{hand}{Source}", Color)}";
            return string.IsNullOrEmpty(result) ? "" : $"[{result}]";
        }

        public bool IsOppositeHandWithSameName(VRActionInput other)
        {
            if (other == this)
            {
                return false;
            }
            if (Hand != other.Hand && Source == other.Source)
            {
                return true;
            }
            return false;
        }

        private void InitTextParts()
        {
            Hand = _action.GetLocalizedOriginPart(SteamVR_Input_Sources.Any, new[] { EVRInputStringBits.VRInputString_Hand });
            Source = _action.GetLocalizedOriginPart(SteamVR_Input_Sources.Any, new[] { EVRInputStringBits.VRInputString_InputSource });
            UpdatePrefixes();
        }

        private void UpdatePrefixes()
        {
            if (!ControllerInput.buttonActions.ContainsValue(this))
            {
                return;
            }
            if (HasAxisWithSameName())
            {
                Prefixes.Add("Click");
            }

            if (!HasOppositeHandButtonWithSameName())
            {
                HideHand = true;
            }
        }

        private bool HasAxisWithSameName()
        {
            foreach (var axisEntry in ControllerInput.axisActions)
            {
                var axis = axisEntry.Value;
                if (Hand == axis.Hand && Source == axis.Source)
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasOppositeHandButtonWithSameName()
        {
            foreach (var buttonEntry in ControllerInput.buttonActions)
            {
                if (IsOppositeHandWithSameName(buttonEntry.Value))
                {
                    return true;
                }
            }
            foreach (var axisEntry in ControllerInput.axisActions)
            {
                if (IsOppositeHandWithSameName(axisEntry.Value))
                {
                    return true;
                }
            }
            foreach (var otherAction in ControllerInput.otherActions)
            {
                if (IsOppositeHandWithSameName(otherAction))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
