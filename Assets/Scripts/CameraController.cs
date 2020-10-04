using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	const float MOVE_SPEED = 2f;
	const float ZOOM_SPEED = 1f;

	bool dragging = false;
	bool lockDragging = false;
	Vector3 startDragCoord;

	public bool disableKeyboardControls = false;

	public static CameraController instance;

	void Start() {
		if (instance != null) {
			Debug.LogError("Can't have two camera controllers!");
		}
		instance = this;
	}

	void Update() {

		if (!disableKeyboardControls) {
			float x = Input.GetAxis("Horizontal");
			float y = Input.GetAxis("Vertical");

			if (x != 0 || y != 0) {
				transform.Translate(new Vector3(x, y, 0f) * Time.deltaTime * MOVE_SPEED * Camera.main.orthographicSize);
			}
		}

		float scroll = Input.GetAxis("Mouse ScrollWheel");

		if (scroll != 0) {
			Camera.main.orthographicSize -= Camera.main.orthographicSize * ZOOM_SPEED * scroll;
		}

		// When the mouse button is pressed down, store where it was pressed.
		if (Input.GetMouseButtonDown(2) || (Input.GetMouseButtonDown(1) && lockDragging == false)) {
			StartDrag();
		}

		// When the mouse is dragged move the camera to make sure the mouse is still pointing to the same world coordinate.
		if (dragging == true) {
			ContinueDrag();
		}

		if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2)) {
			StopDrag();
		}

	}

	void StartDrag() {
		dragging = true;
		startDragCoord = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}

	void ContinueDrag() {
		Vector3 currentCoord = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		transform.Translate(startDragCoord - currentCoord);
	}

	void StopDrag() {
		dragging = false;
	}

	public void LockDragging() {
		lockDragging = true;
	}

	public void UnlockDragging() {
		lockDragging = false;
	}
}
