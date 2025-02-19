﻿using UnityEngine;

namespace PolymindGames
{
	public interface ISleepingPlace
	{
		GameObject gameObject { get; }
		Vector3 SleepPosition { get; }
		Vector3 SleepRotation { get; }
	}
}