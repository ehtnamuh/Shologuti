using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "GutiVectors", menuName = "Kamla/GutiVectors")]
	public class GutiVectors : ScriptableObject
	{
		public List<GutiAddressVectorSet> set;
	}

	[System.Serializable]
	public class GutiAddressVectorSet
	{
		public Address address;
		public Vector3 worldPosition;
	}
	
}