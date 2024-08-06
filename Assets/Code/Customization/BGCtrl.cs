using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BGCtrl
	:
	MonoBehaviour
{
	void Start()
	{
		cam = Camera.main;
		eventSys = EventSystem.current;
		rayMask = LayerMask.GetMask( "Default" );
		uiMask = LayerMask.GetMask( "UI" );
	}

	void Update()
	{
		bool hoveringMenu = false;
		if( !moving && !rotating )
		{
			var pointerData = new PointerEventData( eventSys );
			pointerData.position = Input.mousePosition;
			var results = new List<RaycastResult>();
			eventSys.RaycastAll( pointerData,results );
			// foreach( var result in results ) print( result );
			hoveringMenu = ( results.Count > 0 );
		}

		{
			var mousePos = ( Vector2 )Input.mousePosition;
			
			var diff = mousePos - mouseStart;

			// rotation
			if( Input.GetMouseButton( 1 ) )
			{
				if( !hoveringMenu )
				{
					if( moveBG )
					{
						var rot = bgModel.eulerAngles;
						rot.y -= diff.x * rotSpd.y;
						bgModel.eulerAngles = rot;
						bgModel.Rotate( Vector3.right,-diff.y * rotSpd.x,Space.World );
					}
					rotating = true;
				}
			}
			else rotating = false;

			// movement
			if( Input.GetMouseButton( 0 ) )
			{
				if( !hoveringMenu )
				{
					if( moveBG )
					{
						var move = Vector3.right * diff.x * -1.0f * moveSpd + Vector3.up * diff.y * moveSpd;
						bgModel.Translate( move,Space.World );
					}
					moving = true;
				}
			}
			else moving = false;

			mouseStart = mousePos;
		}
			
		// zoom/scale
		if( Mathf.Abs( Input.mouseScrollDelta.y ) > 0.0f && !hoveringMenu )
		{
			var scale = transform.localScale;

			float newScroll = 0.0f;
			if( moveBG )
			{
				scale = bgModel.localScale;
				newScroll = scale.x + ( Input.mouseScrollDelta.y > 0.0f ? scrollSpd : -scrollSpd );
			}

			if( newScroll > scrollSpd )
			{
				scale = Vector3.one * newScroll;
				if( moveBG ) bgModel.localScale = scale;
			}
		}
	}

	public void SetMoveBG( bool move )
	{
		moveBG = move;
	}

	public void SetBGModel( Transform model )
	{
		bgModel = model;
	}

	Camera cam;
	EventSystem eventSys;
	LayerMask rayMask;
	LayerMask uiMask;

	Vector2 mouseStart = Vector2.zero;
	bool moving = false;
	bool rotating = false;

	[Header( "Translation Speed")]
	[SerializeField] float moveSpd = 1.0f;
	[SerializeField] Vector2 rotSpd = Vector2.one;
	[SerializeField] float scrollSpd = 0.1f;

	bool moveBG = false;
	Transform bgModel = null;
}