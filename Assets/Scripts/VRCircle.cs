using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRCircle : MonoBehaviour
{
    public Image imgCircle;
    public float totalTime = 2.0f;

    bool gvrStatus;
    float gvrTimer;

    private RaycastHit hit;
    private Transform _movePos;

    // Update is called once per frame
    void Update()
    {
        if (gvrStatus)
        {
            gvrTimer += Time.deltaTime;
            imgCircle.fillAmount = gvrTimer / totalTime;
        }

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out hit, 10))
        {
            if (imgCircle.fillAmount == 1 && hit.transform.CompareTag("Teleport"))
            {
                _movePos = hit.transform.gameObject.transform;
                //hit.transform.gameObject.GetComponent<Teleport>().TeleportPlayer();
            }
        }
    }

    public void GVROn()
    {
        gvrStatus = true;
    }

    public void GVROff()
    {
        gvrStatus = false;
        gvrTimer = 0;
        imgCircle.fillAmount = 0;
    }

    private void Awake()
    {
        VoiceManager.OnResult += OnResult;
    }

    void OnResult(string result)
    {
        Debug.Log(result);

        if (result.Contains("move"))
        {
            Move();
        }
    }

    public void Move()
    {
        transform.position = new Vector3(_movePos.position.x, _movePos.position.y, _movePos.position.z);
    }


}
