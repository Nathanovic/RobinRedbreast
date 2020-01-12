using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	[SerializeField] private GameObject targetVisuals;
	[SerializeField] private Animator animator;
	[SerializeField] private new SpriteRenderer renderer;
	[Space]
	[SerializeField] private float flyForce;
	[Range(0f, 1f)][SerializeField] private float verticalFlyForceMultiplier;
	[Space]
	[SerializeField] private float startLandingOffset;
	[Range(0f, 1f)][SerializeField] private float landingWingStrokeSpeedMultiplier;
	[Range(0f, 1f)][SerializeField] private float flyUpWingStrokeSpeedMultiplier;
	[Space]
	[SerializeField] private float distanceToFloor;
	[SerializeField] private float maxDistanceToTargetPosition;
	[SerializeField] private LayerMask floorLayerMask;

	private new Rigidbody2D rigidbody;
	private bool isMovingToTargetPosition;
	private Vector2 targetPosition;
	private Vector2 force;
	private float verticalFactor;

	private void Awake() {
		rigidbody = GetComponent<Rigidbody2D>();
		targetVisuals.SetActive(false);
		targetVisuals.transform.SetParent(null);
	}

	private void Update() {
		if (Input.GetMouseButtonDown(0)) {
			targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			bool targetIsLeft = targetPosition.x < transform.position.x;
			renderer.flipX = targetIsLeft;
			isMovingToTargetPosition = true;
			targetVisuals.transform.position = targetPosition;
			targetVisuals.SetActive(true);

			Vector2 direction = targetPosition - (Vector2)transform.position;
			verticalFactor = 0f;
			if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {
				verticalFactor = Mathf.Abs(direction.y) / Mathf.Abs(direction.x);
			}
			else {
				float horizontalFactor = Mathf.Abs(direction.x) / Mathf.Abs(direction.y);
				verticalFactor = 1f - horizontalFactor;
			}

			float finalFlyForce = flyForce * (1f - verticalFactor) + flyForce * verticalFlyForceMultiplier * verticalFactor;
			force = direction.normalized * finalFlyForce;

			float wingStrokeSpeed = 1f;
			if(targetPosition.y > transform.position.y) {
				wingStrokeSpeed = 1f + verticalFactor * flyUpWingStrokeSpeedMultiplier;
			}
			animator.SetFloat("wingStrokeSpeed", wingStrokeSpeed);
		}

		if (!isMovingToTargetPosition) { return; }

		float distanceToTarget = Vector2.Distance(transform.position, targetPosition);
		if (targetPosition.y < transform.position.y && distanceToTarget < startLandingOffset) {
			float wingStrokeSpeed = 1f - verticalFactor * landingWingStrokeSpeedMultiplier * startLandingOffset / distanceToTarget;
			animator.SetFloat("wingStrokeSpeed", wingStrokeSpeed);
		}

		if (distanceToTarget < maxDistanceToTargetPosition) {
			StopMoving();
		}
	}

	private void FixedUpdate() {
		if (!isMovingToTargetPosition) { return; }
		RaycastHit2D hitFloor = Physics2D.Raycast(transform.position, Vector2.down, distanceToFloor, floorLayerMask);
		if (hitFloor && hitFloor.point.y > targetPosition.y) {
			StopMoving();
			return;
		}
		
		rigidbody.AddForce(force);
	}

	private void StopMoving() {
		isMovingToTargetPosition = false;
		targetVisuals.SetActive(false);
		animator.SetFloat("wingStrokeSpeed", 1f);
	}

	private void OnDrawGizmos() {
		Vector3 centerPosition = transform.position + Vector3.down * distanceToFloor * 0.5f;
		Vector3 size = new Vector3(0.1f, distanceToFloor);
		Gizmos.color = Color.blue;
		Gizmos.DrawCube(centerPosition, size);
	}

}