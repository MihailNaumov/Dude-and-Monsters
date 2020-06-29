using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MonoBehaviour {

	private ObjectPooler objectPooler;
	private float damage;
	public float Damage { get { return damage; } set { 	damage = value;} }

	
	private Vector2 bulletSize;
	public Vector2 BulletSize { get { return bulletSize; } set {bulletSize = value;} }

	private Transform shooter;
	public Transform Shooter { get { return shooter; } set { shooter = value; } }

	public LayerMask colMask;
	public LayerMask ColMask { get { return colMask; } set { colMask = value; } }



	void Awake()
	{
		objectPooler = ObjectPooler.Instance;
		
	}

	public virtual void OnCollisionEnter2D(Collision2D col)
	{
		if ((ColMask.value & 1 << col.gameObject.layer) == 1 << col.gameObject.layer)
		{
			gameObject.SetActive(false);
		}
	}

	public void SetBullet(float damage, Vector2 bulletSize, Transform shooter)
	{
		Damage = damage;
		BulletSize = bulletSize;
		Shooter = shooter;

		transform.localScale = BulletSize;
	}

	void OnEnable()
	{
		if(Shooter !=null)
		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), Shooter.GetComponent<Collider2D>());
	}
}
