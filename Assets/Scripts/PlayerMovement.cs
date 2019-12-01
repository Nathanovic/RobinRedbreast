using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	[SerializeField] private float moveSpeed;
	[SerializeField] private float maxDistanceToTargetPosition;
	[SerializeField] private GameObject targetVisuals;

	private bool isMovingToTargetPosition;
	private Vector2 targetPosition;

	private void Start() {
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

		if (isMovingToTargetPosition == false) { return; }

		Vector2 newPosition = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
		transform.position = newPosition;

		if(Vector2.Distance(newPosition, targetPosition) < maxDistanceToTargetPosition) {
			isMovingToTargetPosition = false;
			targetVisuals.SetActive(false);
		}
	}

}