using Unity.Netcode.Components;
using UnityEngine;

namespace SnakePowerByte.Utilities
{
    [DisallowMultipleComponent]
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}