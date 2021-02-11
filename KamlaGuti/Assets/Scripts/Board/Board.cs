using System;
using Board.Guti;
using Board.View;
using Unity.Barracuda;
using UnityEngine;

namespace Board
{
	public class Board : MonoBehaviour
	{
		public AddressIndexTranslator addressIndexTranslator;
		public BoardGui boardGui;
		private GutiMap _gutiMap; // logical state of the board
		private GutiNode[] _gutiNodesArray;

		private void Awake()
		{
			_gutiMap = new GutiMap();
			boardGui = gameObject.GetComponent<BoardGui>();
			boardGui.Init();
			LoadFromJson();
			Init();
		}

		private void Init()
		{
			// Storing Guti in dictionary using Map as address
			// Creating Game objects to visually represent condition of GutiMap
			foreach (var gutiNodeParent in _gutiNodesArray)
			{
				var gutiNode = gutiNodeParent.GetCopy();
				// All gutis, empty, green or red MUST be added to the logical GutiMap
				_gutiMap.AddGuti(gutiNode.Address, gutiNode);
				// if the gutiType is empty, No need to create Visual Game object
				if (gutiNode.gutiType == GutiType.NoGuti) continue;
				boardGui.CreateGutiGo(gutiNode);
			}
			addressIndexTranslator = new AddressIndexTranslator(_gutiNodesArray);
		}

		private void LoadFromJson()
		{
			var textAsset = Resources.Load<TextAsset>("Board");
			var data = textAsset.text;
			var fromJson = JsonUtility.FromJson<GutiNodes>(data);
			_gutiNodesArray = fromJson.gutiArray;
		}

		public void Restart()
		{
			boardGui.DeleteAllGutiGo();
			boardGui.ClearHighlightedNodes();
			_gutiMap = new GutiMap();
			Init();
		}


		public void MoveGuti(Move move)
		{
			var sourceAddress = move.sourceAddress;
			var targetAddress = move.targetAddress;
			// updating logical map
			_gutiMap.CaptureGuti(sourceAddress, targetAddress);
			if (RuleBook.CanCaptureGuti(move, _gutiMap))
			{
				var capturedGutiAddress = _gutiMap.GetCapturedGutiAddress(sourceAddress, targetAddress);
				boardGui.ClearCapturedGuti(capturedGutiAddress);
			}
			// moving and updating visual map
			boardGui.UpdateGutiGo(move);
			boardGui.ClearHighlightedNodes();
		}

	
		public void HighlightWalkableNodes(Address address)
		{
			var walkableNeighbours = _gutiMap.GetWalkableNodes(address);
			boardGui.HighlightWalkableNodes(walkableNeighbours);
			boardGui.SpawnHighlightNode(address, Color.black);
		}
	
		// returns COPY of the Logical Map of the Game
		public GutiMap GetGutiMapCopy() => new GutiMap(_gutiMap);
		public GutiMap GetGutiMapRef() => _gutiMap;
		
		
	}
}
