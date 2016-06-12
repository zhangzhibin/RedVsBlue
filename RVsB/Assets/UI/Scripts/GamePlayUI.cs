using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GamePlayUI : UIBase {
	[SerializeField]
	private Text _p1Text;
	[SerializeField]
	private Text _p2Text;
	[SerializeField]
	private Text _p1ScoreText;
	[SerializeField]
	private Text _p2ScoreText;
	[SerializeField]
	private GameObject _p1TurnMark;
	[SerializeField]
	private GameObject _p2TurnMark;

	[SerializeField]
	private Button _newGameButton;
	[SerializeField]
	private Button _addLocalPlayerButton;
	[SerializeField]
	private Button _addAIPlayerButton;
	[SerializeField]
	private Button _startGameButton;

	[SerializeField]
	private GameObject _resultPanel;
	[SerializeField]
	private Text _winnerNameText;
	[SerializeField]
	private Button _finishGameButton;

	[SerializeField]
	private GameReferee _referee;
	[SerializeField]
	private LocalGamePlayer _p1;
	[SerializeField]
	private LocalGamePlayer _p2;

	void Start()
	{
		Show ();
	}

	protected override void BindUIComponents ()
	{
		base.BindUIComponents ();

		_newGameButton.onClick.AddListener (OnNewGame);
		_startGameButton.onClick.AddListener (OnStartGame);
		_addLocalPlayerButton.onClick.AddListener (OnAddLocalPlayer);
		_addAIPlayerButton.onClick.AddListener (OnAddAIPlayer);

		_finishGameButton.onClick.AddListener (OnFinishGame);
	}

	protected override void UpdateData ()
	{
		base.UpdateData ();
	}

	public void OnNewGame()
	{
		_p1TurnMark.SetActive (false);
		_p2TurnMark.SetActive (false);

		updatePlayerScore (1, 0);
		updatePlayerScore (2, 0);

		_referee.ClearGame ();
		_referee.PrepareGame (5, 5);

		_referee.AddPlayer (_p1);
		_referee.AddPlayer (_p2);
		_referee.StartGame ();
	}

	public void OnStartGame()
	{
		
	}

	public void OnAddLocalPlayer()
	{
		
	}

	public void OnAddAIPlayer()
	{
		
	}

	public void OnFinishGame()
	{
		CloseResult ();
	}

	public void ShowResult()
	{
		int p1Score = _referee.CurrentGameState.Score (1);
		int p2Score = _referee.CurrentGameState.Score (2);

		if(p1Score>p2Score)
		{
			_winnerNameText.text = _referee.GetPlayer (1).PlayerName;
		}
		else
		{
			_winnerNameText.text = _referee.GetPlayer (2).PlayerName;
		}

		_resultPanel.SetActive (true);
	}

	public void CloseResult()
	{
		_resultPanel.SetActive (false);
		_newGameButton.interactable = true;
	}

	private void onPlayerJoined(IGamePlayer player)
	{
		if(player.PlayerId == 1 )
		{
			_p1Text.text = player.PlayerName;
		}
		else if(player.PlayerId == 2)
		{
			_p2Text.text = player.PlayerName;
		}
	}

	private void updatePlayerScore(int playerId, int score)
	{
		if(playerId == 1)
		{
			_p1ScoreText.text = string.Format ("{0}", score);
		}
		else if(playerId == 2)
		{
			_p2ScoreText.text = string.Format ("{0}", score);
		}
	}

	private void onPlayerTurnBegin(IGamePlayer player)
	{
		if(player.PlayerId == 1)
		{
			_p1TurnMark.SetActive (true);
		}
		else if(player.PlayerId == 2)
		{
			_p2TurnMark.SetActive (true);
		}
	}

	private void onPlayerTurnEnd(IGamePlayer player)
	{
		updatePlayerScore (1, _referee.CurrentGameState.Score (1));
		updatePlayerScore (2, _referee.CurrentGameState.Score (2));

		if(player.PlayerId == 1)
		{
			_p1TurnMark.SetActive (false);
		}
		else if(player.PlayerId == 2)
		{
			_p2TurnMark.SetActive (false);
		}
	}

	protected override void addEventsToList ()
	{
		base.addEventsToList ();

		addInterestedEvent (GamePlayEvents.FinishGame);
		addInterestedEvent (GamePlayEvents.StartGame);
		addInterestedEvent (GamePlayEvents.PlayerJoin);
		addInterestedEvent (GamePlayEvents.PlayerTurnBegin);
		addInterestedEvent (GamePlayEvents.PlayerTurnEnd);
	}

	public override bool OnEvent (IGIIEvent giiEvent)
	{
		bool ret = true;

		switch(giiEvent.Name)
		{
		case GamePlayEvents.StartGame:
			_newGameButton.interactable = false;
			break;
		case GamePlayEvents.FinishGame:
			ShowResult ();
			break;
		case GamePlayEvents.PlayerJoin:
			onPlayerJoined ((IGamePlayer)giiEvent.Body);
			break;
		case GamePlayEvents.PlayerTurnBegin:
			onPlayerTurnBegin ((IGamePlayer)giiEvent.Body);
			break;
		case GamePlayEvents.PlayerTurnEnd:
			onPlayerTurnEnd ((IGamePlayer)giiEvent.Body);
			break;
		}
		ret = base.OnEvent (giiEvent);

		return ret;
	}
}