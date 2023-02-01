using System;
using System.Collections.Generic;
using System.Linq;
using Board.Guti;
using UnityEngine;

namespace Board.View
{
    public class BoardGui : MonoBehaviour
    {
        [SerializeField] private GameObject gutiPrefab;
        public Dictionary<Address, Guti> _gutiGoMap;
        private List<GameObject> _highlightedNodes;

        // private void Awake() => Init();

        public void Init()
        {
            _gutiGoMap = new Dictionary<Address, Guti>();
            _highlightedNodes = new List<GameObject>();
        }

        internal void ClearCapturedGuti(Address capturedGutiAddress)
        {
            Destroy(_gutiGoMap[capturedGutiAddress].gameObject);
            _gutiGoMap.Remove(capturedGutiAddress);
        }

        public void ClearHighlightedNodes()
        {
            foreach (var node in _highlightedNodes) Destroy(node);
        }

        public void SpawnHighlightNode(Address neighbourAddress, Color color)
        {
            var gutiGo = GameObject.Instantiate(gutiPrefab, gameObject.transform, true);
            var guti = gutiGo.GetComponent<Guti>();
            guti.SetAddress(neighbourAddress, scale: 1.0f);
            guti.SetGutiType(GutiType.Highlight);
            guti.SetGutiColor(color);
            _highlightedNodes.Add(gutiGo);
        }

        public void HighlightMove(Move move)
        {
            ClearHighlightedNodes();
            SpawnHighlightNode(move.sourceAddress, Color.white);
            SpawnHighlightNode(move.targetAddress, Color.blue);
        }

        public void HighlightWalkableNodes( IEnumerable<Address> walkableNeighbours)
        {
            ClearHighlightedNodes();
            foreach (var neighbourAddress in walkableNeighbours) SpawnHighlightNode(neighbourAddress, Color.yellow);
        }

        internal void DeleteAllGutiGo()
        {
            foreach (var gutiGo in _gutiGoMap.ToList())
            {
                Destroy(gutiGo.Value.gameObject);
                _gutiGoMap.Remove(gutiGo.Key);
            }
        }

        public void CreateGutiGo(GutiNode gutiNode)
        {
            var gutiGo = Instantiate(gutiPrefab, gameObject.transform, true);
            var guti = gutiGo.GetComponent<Guti>();
            guti.SetAddress(gutiNode.Address);
            guti.SetGutiType(gutiNode.gutiType);
            _gutiGoMap[guti.address] = guti; ;
        }
	
        public void UpdateGutiGo(Move move)
        {
            var sourceAddress = move.sourceAddress;
            var targetAddress = move.targetAddress;
            try
            {
                _gutiGoMap[sourceAddress].SetAddress(targetAddress);
                _gutiGoMap[targetAddress] = _gutiGoMap[sourceAddress];
                _gutiGoMap.Remove(sourceAddress);
            }
            catch (Exception e)
            {
                // Debug.Log(gameObject.name + "  " + _gutiGoMap.Count);
                throw;
            }
        }

    }
}