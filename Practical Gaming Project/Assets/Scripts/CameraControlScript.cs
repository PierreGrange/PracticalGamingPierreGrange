using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlScript : MonoBehaviour
{
    float cameraDistance = 10;
    float focusDistance = 5;
    float turningSensitivity = 15;
    float maxHeight = 8;
    public Quaternion desiredPosition;

    internal void SetCameraPosition(Transform character)
    {
        CameraRotateX(character, Input.GetAxis("Horizontal"));
        CameraRotateY(character, Input.GetAxis("Vertical"));
        transform.LookAt(character.position + focusDistance * character.up);
    }

    private void CameraRotateX(Transform character, float inputAxisValue)
    {
        Camera.main.transform.RotateAround(character.position, Vector3.up, turningSensitivity * inputAxisValue * Time.deltaTime);
        Vector3 v = (Camera.main.transform.position - character.position).normalized * cameraDistance + character.position;
        Vector3 desiredPosition = new Vector3(v.x, Camera.main.transform.position.y, v.z);
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, desiredPosition, 0.1f);
    }

    private void CameraRotateY(Transform character, float inputAxisValue)
    {
        Camera.main.transform.RotateAround(character.position, character.right, turningSensitivity * inputAxisValue * Time.deltaTime);

        if (Camera.main.transform.position.y > maxHeight + character.position.y)
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, maxHeight + character.position.y, Camera.main.transform.position.z);

        if (Camera.main.transform.position.y < character.position.y)
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, character.position.y, Camera.main.transform.position.z);
    }}
