using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace XWizardGames.AI
{
    public abstract class SimpleAIBase : NetworkBehaviour
    {
        public abstract void FindTarget();
        public abstract void Attack();
        public abstract void MoveToTarget();
        public abstract void Update();

    }
}