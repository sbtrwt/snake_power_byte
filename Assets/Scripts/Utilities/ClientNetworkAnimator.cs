using Unity.Netcode.Components;
using UnityEngine;

namespace SnakePowerByte.Utilities
{

    public class ClientNetworkAnimator : NetworkAnimator
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}