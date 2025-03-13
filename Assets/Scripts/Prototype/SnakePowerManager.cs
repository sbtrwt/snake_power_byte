// SnakePowerManager.cs
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace SnakePowerByte.Prototype
{
    public class SnakePowerManager : NetworkBehaviour
    {
        [SerializeField] private int maxSequenceLength = 10;
        private string currentCombo = "";

        // List of available power definitions assigned via the Inspector.
        [SerializeField] private List<PowerDefinition> powerDefinitions;

        public void AddFoodToCombo(string foodLetter)
        {
            currentCombo += foodLetter.ToUpper();
            if (currentCombo.Length > maxSequenceLength)
            {
                currentCombo = currentCombo.Substring(currentCombo.Length - maxSequenceLength);
            }
            Debug.Log("Current Combo: " + currentCombo);
            CheckForPowerActivation();
        }

        private void CheckForPowerActivation()
        {
            foreach (PowerDefinition powerDef in powerDefinitions)
            {
                if (!string.IsNullOrEmpty(powerDef.comboPattern) &&
                    currentCombo.EndsWith(powerDef.comboPattern.ToUpper()))
                {
                    ActivatePower(powerDef);
                    currentCombo = "";
                    break;
                }
            }
        }

        private void ActivatePower(PowerDefinition powerDef)
        {
            Debug.Log("Activating power: " + powerDef.powerName);
            powerDef.Activate(gameObject);
            StartCoroutine(HandlePowerDuration(powerDef.duration, powerDef));
        }

        private IEnumerator HandlePowerDuration(float duration, PowerDefinition powerDef)
        {
            yield return new WaitForSeconds(duration);
            Debug.Log($"Power {powerDef.powerName} ended on {gameObject.name}");
            // Reset power effects here if needed.
        }
    }
}
