// XPManager.cs
using Unity.Netcode;
using UnityEngine;

namespace SnakePowerByte.Prototype
{
    public class XPManager : NetworkBehaviour
    {
        public NetworkVariable<int> XP = new NetworkVariable<int>(0);
        public int level = 1;
        public int xpToLevelUp = 100;

        public void AddExperience(int amount)
        {
            if (!IsServer) return;
            XP.Value += amount;
            if (XP.Value >= xpToLevelUp)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            level++;
            XP.Value = 0;
            Debug.Log($"Snake leveled up to {level}!");
            // Optionally increase snake stats here.
        }
    }
}
