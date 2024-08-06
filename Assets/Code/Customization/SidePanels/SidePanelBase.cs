using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidePanelBase
	:
	MonoBehaviour
{
	protected virtual void Awake()
	{
		pos = GetComponent<RectTransform>();
		startPos = pos.anchoredPosition;
	}

	public void TogglePanel( bool enabled )
	{
		if( pos == null ) print( transform.name );
		pos.anchoredPosition = ( enabled ? startPos : noPos );
	}

	RectTransform pos;
	Vector3 startPos;
	static readonly Vector2 noPos = Vector2.left * 9999.0f;
}