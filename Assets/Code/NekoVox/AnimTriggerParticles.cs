using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTriggerParticles
	:
	AnimTriggerBase
{
	void Start()
	{
		parts = GetComponentInChildren<ParticleSystem>();
	}

	public override void PerformAction()
	{
		parts.Emit( emitAmount );
	}

	[SerializeField] int emitAmount = 25;

	ParticleSystem parts;
}