using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameManagerParams", menuName = "KamlaGuti/GameManagerParams")]
    public class GameManagerParams : ScriptableObject
    {
        public float timeScale = 5.0f;
        public int maxStepCount;
        public bool autoPlay;
    }
}