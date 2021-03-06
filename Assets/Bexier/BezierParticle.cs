﻿using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BezierParticle : MonoBehaviour {

	public Transform p1;
	public Transform p2;
	public Transform p3;


	private ParticleSystem _particleSystem;
	private ParticleSystem.Particle[] _particles;

	void LateUpdate () {
		InitializeIfNeeded ();

		int numParticlesAlive = _particleSystem.GetParticles (_particles);

		for (int i = 0; i < numParticlesAlive; i++) {
			QuadraticBezierCurve curve = new QuadraticBezierCurve (p1.position, p2.position, p3.position);
			float lifetimeRate = 1.0f - ( _particles[i].lifetime / _particles[i].startLifetime );
			_particles [i].position = curve.GetPosition(lifetimeRate);
		}

        _particleSystem.SetParticles(_particles, numParticlesAlive);
	}

	void InitializeIfNeeded(){
		if (_particleSystem == null)
			_particleSystem = this.GetComponent<ParticleSystem> ();

		if (_particles == null || _particles.Length < _particleSystem.maxParticles) {
			_particles = new ParticleSystem.Particle[_particleSystem.maxParticles];
		}

		if (p1 == null) {
			p1 = this.transform;
		}

		if (p2 == null) { 
			GameObject obj = new GameObject ("p2");
			p2 = obj.transform;
			p2.position = p1.position + new Vector3(0,3,0);
			p2.SetParent (this.transform);
		}

		if (p3 == null) {
			GameObject obj = new GameObject ("p3");
			p3 = obj.transform;
			p3.position = p1.position + new Vector3 (0, 5, 0);
			p3.SetParent (this.transform);
		}
	}
}
