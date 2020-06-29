using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	[Header("Передвижение")]
	public float _speed;
	private float speed = 10f;  //скорость героя
	public float _startMoveTime;
	private float startMoveTime;
	public float _stopMoveTime;
	private float stopMoveTime;
	private float movement = 0;
	public float Movement { get { return movement; } set { movement = value; } }
	private enum LastKey { Left, Right };
	private LastKey lastKey;
	[HideInInspector]
	public bool isMove = false;

	[Header("Передвижение в воздухе")]
	public float _airStartMoveTime;
	private float airStartMoveTime;
	public float _airStopMoveTime;
	private float airStopMoveTime;
	private float airMoveCof;

	[HideInInspector]
	public bool smoothWalk = false;
	[Header("Прыжок")]
	public float _jumpPower;
	private float jumpPower;
	public float _jumpHeight;
	private float jumpHeight;
	public float _minJumpHeight;
	private float minJumpHeight;
	public float _gravity;
	private float gravity;
	public float Gravity { get { return gravity; } set { gravity = value; } }
	private float walkGravity = 100f;
	private float airGravity = 20f;
	
	private Vector2 gravityNormal;
	[Range(0f, 5f)]
	public float _fallMultiplier;
	private float fallMultiplier;
	public float FallMultiplier { get { return fallMultiplier; } set { fallMultiplier = value; } }
	[Range(0f, 5f)]
	public float _lowJumpMultiplier;
	private float lowJumpMultiplier;

	[Range(0f, 0.2f)]
	public float _coyoteTime;
	private float coyoteTime;
	private float coyoteTimer;
	[HideInInspector]
	public bool isJump = false;
	private bool canJump = false;
	public bool CanJump { get { return canJump; } set { canJump = value; } }
	
	public bool IsGrounded { get { return isGrounded; } set { isGrounded = value; } }
	[SerializeField]
	public bool isGrounded = false;
	[HideInInspector]
	public bool isCeiling = false; // флаг касания потолка
	
	private double slopeDownAngleOld;
	private double slopeDonwAngle;
	[Header("Движение по уклонам")]
	[SerializeField]
	private float slopeCheckDistance;
	private float slopeSideAngle;

	private bool isOnSlope;

	private Vector2 slopeNormalPerp;
	private Vector2 colliderSize;

	public Vector2 NewVelocity { get { return newVelocity; } set { newVelocity = value; } }
	private Vector2 newVelocity;
	private Rigidbody2D rb; // rigidbody героя

	private Jetpack jetpack;
	private Controller2D controller;
	private new BoxCollider2D  collider;
	public LayerMask whatIsGround;

	void Start () {
		rb = GetComponent<Rigidbody2D>();
		jetpack = GetComponent<Jetpack>();
		controller = GetComponent<Controller2D>();
		collider = GetComponent<BoxCollider2D>();
		colliderSize = collider.size;

		//test stuff
		speed = _speed;
		gravity = _gravity;
		jumpPower = _jumpPower;
		jumpHeight = _jumpHeight;
		minJumpHeight = _minJumpHeight;
		coyoteTime = _coyoteTime;
		startMoveTime = _startMoveTime;
		stopMoveTime = _stopMoveTime;
		fallMultiplier = _fallMultiplier;
		lowJumpMultiplier = _lowJumpMultiplier;
		airStartMoveTime = _airStartMoveTime;
		airStopMoveTime = _airStopMoveTime;
		//
		coyoteTimer = coyoteTime;
		airMoveCof = 1.2f;


	}

	void Update()
	{
		//test stuff
		speed = _speed;
		//gravity = _gravity;
		jumpPower = _jumpPower;
		jumpHeight = _jumpHeight;
		minJumpHeight = _minJumpHeight;
		coyoteTime = _coyoteTime;
		startMoveTime = _startMoveTime;
		stopMoveTime = _stopMoveTime;
		fallMultiplier = _fallMultiplier;
		lowJumpMultiplier = _lowJumpMultiplier;
		airStartMoveTime = _airStartMoveTime;
		airStopMoveTime = _airStopMoveTime;
		//

	}
	
	void FixedUpdate()
	{

		IsOnGround();
		IsCeiling();
		SlopeCheck();
		PlayerRun(speed);
		Jump(jumpPower);

		jetpack.Fly(jetpack.FlySpeed);
		jetpack.FlyRotation(jetpack.RotationSpeed, jetpack.NormalizeRotationSpeed);
		jetpack.Fuel = jetpack.SetFuel(jetpack.Fuel);


		if(isGrounded && (Mathf.Abs(rb.velocity.x) > 5f || isOnSlope || hitGround.normal != Vector2.up))
        {
			PlayerGravity(hitGround.normal);
		}
		else
        {
			PlayerGravity(Vector2.up);
		}
		
		
		
			
		
	
		//test stuff

		//			
	}

	
	void PlayerRun(float speed)
	{
		float move = movement;
		float startMove = 0;
		float stopMove = 0;
		float groundStartMove = startMoveTime;
		float groundStopMove = stopMoveTime;
		float airStartMove = airStartMoveTime;
		float airStopMove = airStopMoveTime;
		float jetpackStartMove = jetpack.StartMoveTime;
		float jetpackStopMove = jetpack.StopMoveTime; 

		if (IsGrounded)
		{
			startMove = groundStartMove;
			stopMove = groundStopMove;
		}
		else if (!IsGrounded && jetpack.isFly)
		{
			startMove = jetpackStartMove;
			stopMove = jetpackStopMove;
		}
		else if (!IsGrounded)
		{
			startMove = airStartMove;
			stopMove = airStopMove;
		}


		float posMove = Mathf.Lerp(move, 1, startMove / 100); // движение вправо
		float negMove = Mathf.Lerp(move, -1, startMove / 100); // движение влево
		float endMove = Mathf.Lerp(move, 0, stopMove / 100); // остановка

		if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
		{
			lastKey = LastKey.Left;
		}
		else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
		{
			lastKey = LastKey.Right;
		}
		
		if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
		{
			lastKey = LastKey.Right;
		}
		else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
		{
			lastKey = LastKey.Left;
		}

		if ((Input.GetKey(KeyCode.LeftArrow) ||
			Input.GetKey(KeyCode.A)))
		{
			if (lastKey == LastKey.Left)
			{
				movement = negMove;
			}
		}
		if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)))
		{
			if (lastKey == LastKey.Right)
			{
				movement = posMove;
			}
		}

		if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow) &&
		!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) // если  отпустил кнопки движения
		{
			if(movement <= 0.1f && movement >= -0.1f )
			{
				movement = 0;
			}

			else
			{
				movement = endMove;
			}
			
		}

		newVelocity.Set(movement * speed, rb.velocity.y);
		rb.velocity = newVelocity;
		

		if (IsGrounded && !isOnSlope && hitGround.normal !=Vector2.up)
		{
			newVelocity.Set(movement * speed, 0);
			rb.velocity = newVelocity;
		}
		else if(isGrounded && isOnSlope)
		{
			newVelocity.Set( speed * slopeNormalPerp.x * -movement, speed * slopeNormalPerp.y * -movement);
			rb.velocity = newVelocity;
		}
		else if(!isGrounded)
		{
			newVelocity.Set(movement * airMoveCof * speed, rb.velocity.y);
			rb.velocity = newVelocity;
		}

		if(isOnSlope && movement == 0)
		{
			rb.constraints = RigidbodyConstraints2D.FreezePositionX|RigidbodyConstraints2D.FreezeRotation;
		}
		else
		{
			rb.constraints = RigidbodyConstraints2D.FreezeRotation;
		}

		if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow)
			&& !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
		{
			isMove = false;
		}
		else
		{
			isMove = true;
		}
	}

	float needJumpHeight;
	float needMinJumpHeight;
	public void Jump(float jumpPower)
	{

		Vector2 jumpForce = new Vector2(0, jumpPower);

		if (IsGrounded)
		{
			canJump = true;
			coyoteTimer = coyoteTime;
			needMinJumpHeight = transform.position.y + minJumpHeight;
			needJumpHeight = transform.position.y + jumpHeight;
		}
		if(!IsGrounded)
		{
			if (coyoteTimer > 0)
			{
				coyoteTimer -= Time.deltaTime;
			}

			else if (!isJump && coyoteTimer <= 0)
			{
				canJump = false;
			}
		}


		if (transform.position.y < needMinJumpHeight - Mathf.Epsilon && isJump && !isCeiling && !jetpack.isFly) // прыгать в любом случае пока герой не достиг минимальной высоты 
		{
			rb.AddForce(jumpForce , ForceMode2D.Impulse);
		}
		else
		{
			isJump = false;
		}

		if (Input.GetButton("Jump") && canJump)
		{
			if (transform.position.y < needJumpHeight - Mathf.Epsilon && !isCeiling) // прыгать пока герой не достиг максимальной высоты
			{
				isJump = true;
				if (transform.position.y >= needMinJumpHeight - Mathf.Epsilon)
				{
					
					rb.AddForce(jumpForce , ForceMode2D.Impulse);
				}
			}

			else
			{
				isJump = false;
				canJump = false;
			}
		}

		else if (Input.GetButtonUp("Jump"))
		{
			canJump = false;
		}

	}

	public void PlayerGravity(Vector2 direction)
	{
		if (!isJump && isGrounded)
		{
			gravity = walkGravity;
		}
		else
		{
			gravity = airGravity;
		}
		if (rb.velocity.y <= 0)
		{
			rb.AddForce(direction * -Mathf.Pow(gravity, 2) * fallMultiplier * Time.deltaTime, ForceMode2D.Impulse);
		}
		else if (rb.velocity.y > 0 && !isJump)
		{
			rb.AddForce(direction * -Mathf.Pow(gravity, 2)  * lowJumpMultiplier * Time.deltaTime, ForceMode2D.Impulse);
		}

		
	}

	/// <summary>
	/// Возвращает true, если герой коснулся земли
	/// </summary>
	/// <returns></returns>
	public bool IsOnGround()
	{
		bool ground = false;
		for (int i = 0; i < controller.verticalRayCount; i++)
		{
			float directionX = Mathf.Sign(rb.velocity.x);
			Vector2 rayOrigin = (directionX == -1) ? controller.raycastOrigins.bottomLeft : controller.raycastOrigins.bottomRight;
			rayOrigin += Vector2.right * (controller.verticalRaySpacing * -directionX * i );
		//	Debug.DrawRay(rayOrigin, -Vector2.up, Color.yellow);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, 30f, whatIsGround);

			if (hit.distance <= 0.1f)
			{
				ground = true;
			}
		}
		return IsGrounded = ground;
	}

	/// <summary>
	/// Возвращает true, если герой коснулся потолка
	/// </summary>
	/// <returns></returns>
	public bool IsCeiling()
	{
		bool celling = false;
		for (int i = 0; i < controller.verticalRayCount; i++)
		{
			Vector2 rayOrigin = controller.raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (controller.verticalRaySpacing * i + (rb.velocity.x * Time.deltaTime));
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, 10f, whatIsGround);
			if (hit.distance <= 0.05f)
			{
				celling = hit;
			}

		}
		return isCeiling = celling;
	}

	#region Slopes
	/// <summary>
	/// Поведение героя на наклонных поверхностях
	/// </summary>
	/// <returns></returns>
	/// 
	RaycastHit2D hitGround;
	private void SlopeCheck()
	{
		Vector2 checkPos = transform.position - new Vector3(0.0f, (colliderSize.y) / 2 + -Controller2D.skinWidth );
		SlopeCheckVertical(checkPos);
		SlopeCheckHorizontal(checkPos);
	}
	int lastMove;
	void SlopeCheckVertical(Vector2 checkPos)
	{
		float skinWidth;

		if(movement == 0)
        {
			skinWidth = Controller2D.skinWidth;
		}
		else
        {
			skinWidth = Controller2D.skinWidth* 2;
		}
		float moveX = rb.velocity.x;
		RaycastHit2D hitMiddle = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);
		RaycastHit2D hitLeft = Physics2D.Raycast(checkPos - new Vector2(colliderSize.x/2 + skinWidth , 0), Vector2.down, slopeCheckDistance, whatIsGround);
		RaycastHit2D hitRight = Physics2D.Raycast(checkPos + new Vector2(colliderSize.x /2 + skinWidth, 0), Vector2.down, slopeCheckDistance, whatIsGround);
		RaycastHit2D hit = hitMiddle;
		

		if(Math.Sign(moveX) > 0)
        {
			lastMove = 1;
			if (hitRight)
			{
				hit = hitRight;
			}
		}
		else if(Math.Sign(moveX) < 0)
        {
			lastMove = -1;
			if (hitLeft)
			{
				hit = hitLeft;
			}
		}
		
		else if(Math.Sign(moveX) == 0)
        {
			if (lastMove > 0)
			{
				if (hitRight)
				{
					hit = hitRight;
				}
			}
			else
            {
				if (hitLeft)
				{
					hit = hitLeft;
				}
			}
		}

		if(!hit)
        {
			if(hitMiddle)
            {
				hit = hitMiddle;
            }
			else if (hitLeft)
			{
				hit = hitLeft;
			}
			else if (hitRight)
			{
				hit = hitRight;
			}
		}

		hitGround = hit;
		slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
		slopeDonwAngle = Vector2.Angle(hit.normal, Vector2.up);
		if(slopeDonwAngle != slopeDownAngleOld)
		{
			isOnSlope = true;
		}

		slopeDownAngleOld = slopeDonwAngle;

		Debug.DrawRay(hit.point, hit.normal, Color.yellow);
		Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);

	}

	void SlopeCheckHorizontal(Vector2 checkPos)
	{
		RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right , slopeCheckDistance , whatIsGround);
		RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance , whatIsGround);
		if(slopeHitFront)
		{
			isOnSlope = true;
			slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
		}
		else if(slopeHitBack)
		{
			isOnSlope = true;
			slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
		}
		else
		{
			slopeSideAngle = 0.0f;
			isOnSlope = false;
		}
	}
    #endregion
	
	
}
