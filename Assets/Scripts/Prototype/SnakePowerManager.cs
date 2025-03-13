using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SnakePowerManager : NetworkBehaviour
{
    // Maximum length of the combo sequence to track.
    [SerializeField] private int maxSequenceLength = 10;
    
    // The current food combo sequence, for example "XOXO".
    private string currentCombo = "";

    // List of all power definitions. Assign these via the Inspector.
    [SerializeField] private List<PowerDefinition> powerDefinitions;

    /// <summary>
    /// Call this method when the snake eats food.
    /// For example, pass in 'X' or 'O' (or any letter that represents the food type).
    /// </summary>
    public void AddFoodToCombo(string foodLetter)
    {
        // Append new food type letter.
        currentCombo += foodLetter.ToUpper();

        // Trim to the maximum length.
        if (currentCombo.Length > maxSequenceLength)
        {
            currentCombo = currentCombo.Substring(currentCombo.Length - maxSequenceLength);
        }

        Debug.Log("Current combo: " + currentCombo);

        // Check for any power matches.
        CheckForPowerActivation();
    }

    private void CheckForPowerActivation()
    {
        // Loop over power definitions and see if the current combo ends with any of them.
        foreach (PowerDefinition powerDef in powerDefinitions)
        {
            if (!string.IsNullOrEmpty(powerDef.comboPattern) &&
                currentCombo.EndsWith(powerDef.comboPattern.ToUpper()))
            {
                // Activate power on the server (or delegate to the server).
                ActivatePower(powerDef);
                // Optionally clear the combo (or remove only the matched part).
                currentCombo = "";
                break;
            }
        }
    }

    // Activate the power. This method should be called on the server.
    private void ActivatePower(PowerDefinition powerDef)
    {
        Debug.Log("Activating power: " + powerDef.powerName);

        // Here you could call the Activate method on the power definition.
        // For a networked game, you may also want to replicate the power state to clients,
        // for example by using NetworkVariables or a ClientRpc.
        powerDef.Activate(gameObject);
    }
}
