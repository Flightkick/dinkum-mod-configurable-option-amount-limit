using System.Collections;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using TMPro;
using UnityEngine;

// ReSharper disable InconsistentNaming -- Parameter naming coupled to Harmony injection (reflection): https://harmony.pardeike.net/articles/patching-injections.html

namespace ConfigurableOptionAmountLimit.patches;

internal static class ConfigurableOptionAmountLimit {
    [HarmonyPatch(typeof(OptionAmount), "selectedAmountUp")]
    private static class SelectedAmountUpPatch {
        [HarmonyPrefix]
        [HarmonyPriority(1)]
        private static bool PrefixSelectedAmountUp(
            ref OptionAmount __instance,
            ref int ___selectedAmount,
            ref TextMeshProUGUI ___selectedAmountText,
            ref TextMeshProUGUI ___moneyAmount,
            ref int ___itemIdBeingShown
        ) {
            Plugin plugin = Plugin.Instance;


            if (___selectedAmount == plugin.MaxItemBuyLimit) {
                ___selectedAmount = 1;
            } else {
                int increment = GetIncrement(InputMaster.input, 1);
                ___selectedAmount = Mathf.Clamp(___selectedAmount + increment, 1, plugin.MaxItemBuyLimit);
                __instance.StartCoroutine("holdUpOrDown", 1);
            }

            ___selectedAmountText.text = ___selectedAmount.ToString();
            ___moneyAmount.text = "<sprite=11>" + (___selectedAmount * Inventory.inv.allItems[___itemIdBeingShown].value * 2).ToString("n0");
            SoundManager.manage.play2DSound(SoundManager.manage.inventorySound);
            return false;
        }
    }

    [HarmonyPatch(typeof(OptionAmount), "selectedAmountDown")]
    private static class SelectedAmountDownPatch {
        [HarmonyPrefix]
        [HarmonyPriority(1)]
        private static bool PrefixSelectedAmountDown(ref OptionAmount __instance,
                                                     ref int ___selectedAmount,
                                                     ref TextMeshProUGUI ___selectedAmountText,
                                                     ref TextMeshProUGUI ___moneyAmount,
                                                     ref int ___itemIdBeingShown
        ) {
            Plugin plugin = Plugin.Instance;

            if (___selectedAmount == 1) {
                ___selectedAmount = plugin.MaxItemBuyLimit;
            } else {
                int decrement = GetIncrement(InputMaster.input, 1);
                ___selectedAmount = Mathf.Clamp(___selectedAmount - decrement, 1, plugin.MaxItemBuyLimit);
                __instance.StartCoroutine("holdUpOrDown", -1);
            }

            ___selectedAmountText.text = ___selectedAmount.ToString();
            ___moneyAmount.text = "<sprite=11>" + (___selectedAmount * Inventory.inv.allItems[___itemIdBeingShown].value * 2).ToString("n0");
            SoundManager.manage.play2DSound(SoundManager.manage.inventorySound);
            return false;
        }
    }


    [HarmonyPatch(typeof(OptionAmount), "holdUpOrDown")]
    private sealed class HoldUpOrDownPatch {
        private sealed class HoldUpOrDownEnumerator : IEnumerable {
            private readonly Plugin _plugin;
            private int _selectedAmount;
            private readonly TextMeshProUGUI _selectedAmountText;
            private readonly TextMeshProUGUI _moneyAmount;
            private readonly int _itemIdBeingShown;
            private readonly int _modifier;
            private readonly InputMaster _input;
            private readonly Traverse _selectedAmountField;

            public HoldUpOrDownEnumerator(Plugin plugin,
                                          OptionAmount instance,
                                          int selectedAmount,
                                          TextMeshProUGUI selectedAmountText,
                                          TextMeshProUGUI moneyAmount,
                                          int itemIdBeingShown,
                                          int modifier) {
                _plugin = plugin;
                _selectedAmount = selectedAmount;
                _selectedAmountText = selectedAmountText;
                _moneyAmount = moneyAmount;
                _itemIdBeingShown = itemIdBeingShown;
                _modifier = modifier;
                _input = InputMaster.input;
                _selectedAmountField = Traverse.Create(instance).Field("selectedAmount");
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }

            public IEnumerator GetEnumerator() {
                float increaseCheck = 0.0f;
                float holdTimer = 0.0f;
                while (InputMaster.input.UISelectHeld()) {
                    if (increaseCheck < 0.15000000596046448 - holdTimer) {
                        increaseCheck += Time.deltaTime;
                    } else {
                        increaseCheck = 0.0f;
                        int increment = GetIncrement(_input, _modifier);
                        if (_selectedAmount + increment == Mathf.Clamp(_selectedAmount + increment, 1, _plugin.MaxItemBuyLimit))
                            SoundManager.manage.play2DSound(SoundManager.manage.inventorySound);
                        _selectedAmount = Mathf.Clamp(_selectedAmount + increment, 1, _plugin.MaxItemBuyLimit);
                        _selectedAmountField.SetValue(_selectedAmount);
                        _selectedAmountText.text = _selectedAmount.ToString();
                        _moneyAmount.text = "<sprite=11>" + (_selectedAmount * Inventory.inv.allItems[_itemIdBeingShown].value * 2).ToString("n0");
                    }

                    holdTimer = Mathf.Clamp(holdTimer + Time.deltaTime / 8f, 0.0f, 0.14f);
                    yield return null;
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPriority(1)]
        private static bool PrefixHoldUpOrDown(
            // ReSharper disable once RedundantAssignment -- Used by Harmony reflection.
            ref IEnumerator __result,
            OptionAmount __instance,
            int ___selectedAmount,
            TextMeshProUGUI ___selectedAmountText,
            TextMeshProUGUI ___moneyAmount,
            int ___itemIdBeingShown,
            int dif
        ) {
            HoldUpOrDownEnumerator myEnumerator = new(
                plugin: Plugin.Instance,
                instance: __instance,
                selectedAmount: ___selectedAmount,
                selectedAmountText: ___selectedAmountText,
                moneyAmount: ___moneyAmount,
                itemIdBeingShown: ___itemIdBeingShown,
                modifier: dif
            );
            __result = myEnumerator.GetEnumerator();
            return false;
        }
    }

    [SuppressMessage("ReSharper", "RedundantIfElseBlock")] // Readability
    private static int GetIncrement(InputMaster input, int modifier) {
        int invSlotNumber = input.getInvSlotNumber();
        bool modifierTimes100 = input.RBHeld() || invSlotNumber == 1;
        bool modifierTimes10 = input.LBHeld() || invSlotNumber == 0;

        if (modifierTimes100) {
            return 100 * modifier;
        } else if (modifierTimes10) {
            return 10 * modifier;
        } else {
            return 1 * modifier;
        }
    }
}