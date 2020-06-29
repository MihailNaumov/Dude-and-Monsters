using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : MonoBehaviour
{
	public LayerMask collisionMask;

	public RaycastOrigins raycastOrigins;
	public const float skinWidth = .015f;
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;

	float maxClimbAngle = 80;

	[HideInInspector]
	public float horizontalRaySpacing;
	[HideInInspector]
	public float verticalRaySpacing;
	

	new private BoxCollider2D collider;

	Rigidbody2D rb;
	public CollisionInfo collisions;

	private PlayerController player;

	
	
	void Start()
	{
		player = GetComponent<PlayerController>();
		rb = GetComponent<Rigidbody2D>();
		collider = GetComponent<BoxCollider2D>();
		CalculateRaySpacing();
	}

	private void Update()
	{
		Vector2 velocity = rb.velocity;

		collisions.Reset();
		UpdateRaycastOrigins();
		
			VerticalCollisions(ref velocity);
		
		
			HorizontalCollisions(ref velocity);
		


	}
	void HorizontalCollisions(ref Vector2 velocity)
	{
		float directionX = Mathf.Sign(velocity.x);
		float rayLength = Mathf.Abs(velocity.x) + skinWidth;

		for (int i = 0; i < horizontalRayCount; i++)
		{
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX * Time.deltaTime, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.right * directionX * Time.deltaTime * rayLength, Color.green);
			if (hit)
			{
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

				
				collisions.left = directionX == -1;
				collisions.right = directionX == 1;
				rayLength = hit.distance;


			}
		}
	}
	void VerticalCollisions(ref Vector2 velocity)
	{
		float directionX = Mathf.Sign(velocity.x);
		float directionY = Mathf.Sign(velocity.y);
		float rayLength = Mathf.Abs(velocity.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++)
		{
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + (velocity.x * Time.deltaTime));
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY * Time.deltaTime, rayLength, collisionMask);


			Debug.DrawRay(rayOrigin, Vector2.up * directionY * Time.deltaTime * rayLength, Color.green);
			if (hit)
			{
				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
				rayLength = hit.distance;
			}
		}
	}

	void UpdateRaycastOrigins()
	{
		Bounds bounds = collider.bounds;
		bounds.Expand(skinWidth * -2);
		raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
	}

	void CalculateRaySpacing()
	{
		Bounds bounds = collider.bounds;
		bounds.Expand(skinWidth * -2);

		horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (horizontalRayCount - 1);
	}

	public struct RaycastOrigins
	{
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	public struct CollisionInfo
	{
		public bool above, below;
		public bool left, right;

		public void  Reset()
		{
			above = below = false;
			left = right = false;
		}
	}
}
