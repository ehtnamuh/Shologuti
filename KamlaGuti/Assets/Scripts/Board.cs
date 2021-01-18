using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
	[SerializeField] private GameObject gutiPrefab;
	private GutiMap _gutiMap; // logical state of the board
	private Dictionary<Address, Guti> _gutiGoMap;
	private List<GameObject> _highlightedNodes;
	private GutiNode[] _gutiNodesArray;

	private List<Move> _moveLog; // Can be used to log moves till the end of episode, to enable Ctrl+Z
	
	private void Awake()
	{
		_gutiMap = new GutiMap();
		_gutiGoMap = new Dictionary<Address, Guti>();
		_highlightedNodes = new List<GameObject>();
		LoadFromJson();
		Init();
	}

	private void Init()
	{
		// Storing Gutis in dictionary using Map as address
		// Creating Game objects to visually represent condition of GutiMap
		foreach (var gutiNodeParent in _gutiNodesArray)
		{
			var gutiNode = gutiNodeParent.GetInstance();
			// All gutis, empty, green or red MUST be added to the logical GutiMap
			_gutiMap.AddGuti(gutiNode.Address, gutiNode);
			// if the gutiType is empty, No need to create Visual Game object
			if (gutiNode.gutiType == GutiType.NoGuti) continue;
			var gutiGo = GameObject.Instantiate(gutiPrefab, gameObject.transform, true);
			// attach logical guti to Guti Object
			var guti = gutiGo.GetComponent<Guti>();
			guti.SetAddress(gutiNode.Address);
			guti.SetGutiType(gutiNode.gutiType);
			_gutiGoMap[guti.address] = guti;
		}
	}

	private void LoadFromJson()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("Board");
		string data = textAsset.text;
		GutiNodes fromJson = JsonUtility.FromJson<GutiNodes>(data);
		_gutiNodesArray = fromJson.gutiArray;
	}

	public void Restart()
	{
		DeleteAllGutiGo();
		_gutiMap = new GutiMap();
		Init();
	}

	public void DeleteAllGutiGo()
	{
		foreach (var gutiGo in _gutiGoMap.ToList())
		{
			Destroy(gutiGo.Value.gameObject);
			_gutiGoMap.Remove(gutiGo.Key);
		}
	}

	public bool HasCapturableGuti(Address address) => _gutiMap.HasCapturableGuti(address);

	public bool HasCapturedGuti(Move move)
	{
		var capturedGutiAddress = _gutiMap.GetCapturedGutiAddress(move.sourceAddress, move.targetAddress);
		return capturedGutiAddress != move.targetAddress;
	}

	public void MoveGuti(Move move)
	{
		// TODO: moveLog to enable Ctrl+Z and Playtrace
		// _moveLog.Add(move);
		var sourceAddress = move.sourceAddress;
		var targetAddress = move.targetAddress;
		// updating logical map
		_gutiMap.CaptureGuti(sourceAddress, targetAddress);
		if (HasCapturedGuti(move))
		{
			var capturedGutiAddress = _gutiMap.GetCapturedGutiAddress(sourceAddress, targetAddress);
			ClearCapturedGuti(capturedGutiAddress);
		}
		// moving and updating visual map
		try
		{
			_gutiGoMap[sourceAddress].SetAddress(targetAddress);
			_gutiGoMap[targetAddress] = _gutiGoMap[sourceAddress];
			_gutiGoMap.Remove(sourceAddress);
		}
		catch (Exception e)
		{
			Debug.Log($"Breaker: {_gutiMap.GetGutiType(sourceAddress)}");
			Debug.Log($"Breaker Address: {sourceAddress}");
			throw;
		}
		ClearHighlightedNodes();
	}

	// returns COPY of the Logical Map of the Game
	public GutiMap GetGutiMap() => new GutiMap(_gutiMap);
	
	private void ClearCapturedGuti(Address capturedGutiAddress)
	{
		try
		{
			Destroy(_gutiGoMap[capturedGutiAddress].gameObject);
			_gutiGoMap.Remove(capturedGutiAddress);
		}
		catch (Exception e)
		{
			Debug.Log("From Board.ClearCapturedGuti: Invalid Address, no Game Object to Delete");
		}
		
	}

	public GutiType getGutiType(Address address)
	{
		return _gutiMap.GetGutiType(address);
	}
	
	public void ReverseLastMove()
	{
		throw new NotImplementedException();
	}

	public void GetLastMove()
	{
		throw new NotImplementedException();
	}

	public void ClearHighlightedNodes()
	{
		// very inefficiently clearing highligts
		// TODO: Have a enabled and disabled highlightedNode Stack and queue 
		// enabled nodes go from disabled stack to enabled stack and vice versa
		foreach (var node in _highlightedNodes) Destroy(node);
	}

	public void HighlightWalkableNeighbours(Address address)
	{
		ClearHighlightedNodes();
		var walkableNeighbours = _gutiMap.GetWalkableNeighbours(address);
		foreach (var neighbourAddress in walkableNeighbours)
		{
			var gutiGo = GameObject.Instantiate(gutiPrefab, gameObject.transform, true);
			var guti = gutiGo.GetComponent<Guti>();
			guti.SetAddress(neighbourAddress, scale: 1.0f);
			guti.SetGutiType(GutiType.Highlight);
			_highlightedNodes.Add(gutiGo);
		}
	}
}