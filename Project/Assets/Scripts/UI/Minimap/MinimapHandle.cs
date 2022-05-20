using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapHandle : MonoBehaviour //handles opening and closing map, zooming and setting up minimap camera
{
    public Camera MinimapCamera;
    public float VisionRange; //proportional to minimap zoom
    ScreenHandle screenHandle; //handler script
    public float OpenTime; //time it takes to open map
    public GameObject canvas; //main UI canvas object
    bool corout; //if opening/closing coroutine is running
    GameObject Player; //player instance
    LayerMask layerMask; //what objects stop player vision
    bool MapOpen; //if map is open
    RectTransform rectTrans; //transform of minimap box
    Vector3 origPos; //mapbox position when closed
    Vector3 targetPos; //mapbox position when open
    Vector2 origDelta; //sizeDelta when closed
    Vector2 targetDelta; //sizeDelta when open
    float finalScale; //how bigger map is when open compared to closed

    IEnumerator OpenMap()
    {
        Time.timeScale = 0; //stop time
        foreach (MainControls controlScript in Player.GetComponents<MainControls>())
        {
            controlScript.enabled = false; //disable player control
        }
        screenHandle.enabled = false; //disable pausing
        corout = true;

        float timer = OpenTime;
        float timerQ;
        while (timer > 0)
        {
            timer -= Time.unscaledDeltaTime;
            timerQ = timer / OpenTime;
            gameObject.transform.position = origPos + (targetPos - origPos) * (1 - timerQ);
            rectTrans.sizeDelta = targetDelta + (origDelta - targetDelta) * timerQ;
            yield return null;
        }
        gameObject.transform.position = targetPos;
        rectTrans.sizeDelta = targetDelta;

        corout = false;
    }
    IEnumerator CloseMap()
    {
        corout = true;

        float timer = OpenTime;
        float timerQ;
        while (timer > 0)
        {
            timer -= Time.unscaledDeltaTime;
            timerQ = timer / OpenTime;
            gameObject.transform.position = origPos + (targetPos - origPos) * timerQ;
            rectTrans.sizeDelta = targetDelta + (origDelta - targetDelta) * (1 - timerQ);
            yield return null;
        }
        gameObject.transform.position = origPos;
        rectTrans.sizeDelta = origDelta;

        corout = false;
        Time.timeScale = 1; //start time
        foreach (MainControls controlScript in Player.GetComponents<MainControls>())
        {
            controlScript.enabled = true; //return player control
        }
        screenHandle.enabled = true;
    }
    void Start()
    {
        rectTrans = gameObject.GetComponent<RectTransform>();
        Player = GameObject.FindWithTag("Player");
        layerMask = LayerMask.GetMask("Minimap");
        screenHandle = GlobalStats.handler.GetComponent<ScreenHandle>();
        MapOpen = false;
        origPos = gameObject.transform.position;
        targetPos = canvas.transform.position;
        origDelta = rectTrans.sizeDelta;
        targetDelta = new Vector2(canvas.GetComponent<RectTransform>().rect.width - rectTrans.rect.width, canvas.GetComponent<RectTransform>().rect.height - rectTrans.rect.height) + origDelta;
        finalScale = Mathf.Max((rectTrans.rect.width + targetDelta.x) / rectTrans.rect.width, (rectTrans.rect.height + targetDelta.y) / rectTrans.rect.height);
        gameObject.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta =
                    new Vector2(canvas.GetComponent<RectTransform>().rect.width, canvas.GetComponent<RectTransform>().rect.height);
        float ratio = Camera.main.aspect;
        Rect rect = MinimapCamera.rect;
        if (ratio > 1)
        {
            rect.width = 1;
            rect.height = ratio;
        }
        else
        {
            rect.width = 1 / ratio;
            rect.height = 1;
        }
        rect.x = 0;
        rect.y = 0;
        MinimapCamera.orthographicSize = VisionRange * finalScale;
        MinimapCamera.rect = rect;
    }
    void Update()
    {
        RaycastHit hit;
        for (int i = 0; i < 120; i++) //cast a ray every 3 degrees
        {
            Physics.Raycast(Player.transform.position, Quaternion.Euler(0, i * 6, 0) * Vector3.forward, out hit, Mathf.Infinity, layerMask);
            if (hit.collider != null && LayerMask.LayerToName(hit.collider.gameObject.layer) == "Minimap")
            {
                hit.collider.gameObject.GetComponent<AddTileQuad>().Show();
            }
        }
        //map controls (open, close, zoom)
        if (Input.GetButtonDown("Map") && !corout)
        {
            if (MapOpen)
            {
                MapOpen = false;
                StartCoroutine(CloseMap());
            }
            else
            {
                MapOpen = true;
                StartCoroutine(OpenMap());
            }
        }
        if (Input.GetButtonDown("ZoomIn") && VisionRange > 2)
        {
            VisionRange -= 2;
            MinimapCamera.orthographicSize = VisionRange * finalScale;
        }
        if (Input.GetButtonDown("ZoomOut") && VisionRange < 30)
        {
            VisionRange += 2;
            MinimapCamera.orthographicSize = VisionRange * finalScale;
        }
    }
}
