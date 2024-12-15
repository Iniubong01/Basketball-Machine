using System;
using UnityEngine;

namespace Assets.SimpleLocalization.Scripts
{
	[Serializable]
	public class Sheet
	{
		public string Name;
		public long Id;
        [HideInInspector] public TextAsset TextAsset;
    }
}