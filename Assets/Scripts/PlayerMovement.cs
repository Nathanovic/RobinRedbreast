using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	[SerializeField] private float flyForce;
	[SerializeField] private float distanceToFloor;
	[SerializeField] private float maxDistanceToTargetPosition;
	[SerializeField] private GameObject targetVisuals;
	[SerializeField] private LayerMask floorLayerMask;

	private new Rigidbody2D rigidbody;
	private bool isMovingToTargetPosition;
	private Vector2 targetPosition;

	private void Start() {
		rigidbody = GetComponent<Rigidbody2D>();
		targetVisuals.SetActive(false);
		targetVisuals.transform.SetParent(null);
	}

	private void Update() {
		if (Input.GetMouseButtonDown(0)) {
			targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			isMovingToTargetPosition = true;
			targetVisuals.transform.position = targetPosition;
			targetVisuals.SetActive(true);
		}
		
		if(isMovingToTargetPosition && Vector2.Distance(transform.position, targetPosition) < maxDistanceToTargetPosition) {
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

		Vector2 directionToTarget = targetPosition - (Vector2)transform.position;
		Vector2 force = directionToTarget.normalized * flyForce;
		rigidbody.AddForce(force);
	}

	private void StopMoving() {
		isMovingToTargetPosition = false;
		targetVisuals.SetActive(false);
	}

	private void OnDrawGizmos() {
		Vector3 centerPosition = transform.position + Vector3.down * distanceToFloor * 0.5f;
		Vector3 size = new Vector3(0.1f, distanceToFloor);
		Gizmos.color = Color.blue;
		Gizmos.DrawCube(centerPosition, size);
	}

}