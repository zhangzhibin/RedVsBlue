using UnityEngine;
using System.Collections;

/// <summary>
/// Game player.
/// 比赛参与者 （可以是本地玩家，网络玩家，也可以是AI）
/// </summary>
public class GamePlayer : MonoBehaviour, IGamePlayer {
	private bool _isMyTurn = false;
	private IGameReferee _currentReferee = null;
	protected IGameReferee CurrentReferee
	{
		get{
			return _currentReferee;
		}
	}

	public int PlayerId{
		get;
		set;
	}

	// Use this for initialization
	public virtual void Start () {
		
	}
	
	// Update is called once per frame
	public virtual void Update () {
		if(IsInTurn)
		{
			HandleInput ();
		}
	}

	[SerializeField]
	private string _playerName;
	public virtual string PlayerName {
		get{
			return _playerName;
		}
		set{
			_playerName = value;
		}
	}

	public virtual void BeginTurn(IGameReferee referee)
	{
		Debug.LogFormat ("Player [{0}] turn begins", PlayerId);

		_isMyTurn = true;
		_currentReferee = referee;
	}

	public virtual void EndTurn(IGameReferee referee)
	{
		Debug.LogFormat ("Player [{0}] turn ends", PlayerId);

		_isMyTurn = false;

		referee.EndPlayerTurn (this);
		_currentReferee = null;
	}

	public bool IsInTurn{
		get{
			return _isMyTurn;	
		}
	}

	public virtual void HandleInput()
	{
//		if(Input.GetMouseButtonUp(0))
//		{
//			var mouseScreenPosition = Input.mousePosition;
//
//			var mouseWorldPosition = Camera.main.ScreenToWorldPoint (mouseScreenPosition);
//
////			if(_currentReferee)
//		}
	}
}
