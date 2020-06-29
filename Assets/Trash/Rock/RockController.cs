using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockController : MonoBehaviour {

	public float _speed;
	private float speed;

	private Rigidbody2D rb;


	// Use this for initialization
	void Start () 
	{
		rb = GetComponent<Rigidbody2D>();
		//test stuff
		speed = _speed;
		//
	}
	
	
	void FixedUpdate () 
	{
		Movement(speed);
		// test stuff
		speed = _speed;
		//
	}

	void Movement(float speed)
	{
		Vector2 cursor = Input.mousePosition;
		Vector2 cursorPos = Camera.main.ScreenToWorldPoint(cursor);
		Vector2 moveToCursor = Vector2.Lerp(transform.position, cursorPos, Time.deltaTime * speed);
		rb.MovePosition(moveToCursor);
		Debug.DrawLine(transform.position, cursorPos, Color.red);
	}
}
