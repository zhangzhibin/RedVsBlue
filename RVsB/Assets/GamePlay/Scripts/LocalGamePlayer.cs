using UnityEngine;
using System.Collections;

/// <summary>
/// Local game player.
/// 本地玩家
/// </summary>
public class LocalGamePlayer : GamePlayer {
	
//	// Update is called once per frame
//	public override void Update () {
//
//	}


	public override void HandleInput()
	{
		if(Input.GetMouseButtonUp(0))
		{
			var mouseScreenPosition = Input.mousePosition;

			var ray = Camera.main.ScreenPointToRay (mouseScreenPosition);
			var hits = Physics.RaycastAll (ray, 100, LayerMask.GetMask(new string[]{"ChessBoard"}));
			if(hits!=null && hits.Length>0)
			{
				var hit = hits [0];
				ChessUnit u = hit.collider.GetComponent<ChessUnit> ();
				if(u!=null)
				{
					Debug.LogFormat ("Click on unit: {0}", u);

					GamePlayOperation op = new GamePlayOperation();

					op.Op = GamePlayOperation.OpEnum.PLACE;
					op.X = u.X;
					op.Y = u.Y;
					op.PlayerId = this.PlayerId;

					if(CurrentReferee.IsValidMove(op))
					{
						CurrentReferee.PlayMove (op);

						EndTurn (CurrentReferee);
//						CurrentReferee.EndPlayerTurn (this);
					}
				}
				else
				{
					Debug.Log ("Click on invalid unit");
				}
			}
			else
			{
				Debug.LogFormat ("Click ({0}) hit nothing", mouseScreenPosition);
			}
		}
	}
}
