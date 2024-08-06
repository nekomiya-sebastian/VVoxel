using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTriggerObj
	:
	AnimTriggerBase
{
	protected void Start()
	{
		animCtrl = GetComponent<Animator>();

		animCtrl.speed = 0.0f;
	}

	public override void PerformAction()
	{
		if( curAnim < playAnims.Count - 1 )
		{
			++curAnim;
			animCtrl.speed = 1.0f;
			animCtrl.GetCurrentAnimatorClipInfo( 0 )[0].clip.wrapMode = WrapMode.ClampForever;
			animCtrl.Play( playAnims[curAnim].name );
		}
	}

	Animator animCtrl;

	int curAnim = -1;

	[SerializeField] List<AnimationClip> playAnims = new List<AnimationClip>();
}