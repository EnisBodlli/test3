using System.Collections;
using System.Collections.Generic;
using PolymindGames;
using PolymindGames.ProceduralMotion;
using UnityEngine;

namespace PolymindGames
{
    public interface IWieldableMotionHandler : ICharacterModule
    {
        IMotionMixer MotionMixer { get; }
        IMotionDataHandler DataHandler { get; }
    }
}
