using System.Collections;
using System.Collections.Generic;
using NumSharp;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public VolumeDataReader.Data activeData;
    private ArrayList worlds;
    public GameObject worldPrefab;
    public Transform centerEye;
    [SerializeField]
    private OVRInput.RawButton shrinkButton;
    [SerializeField]
    private OVRInput.RawButton growButton;
    [SerializeField]
    private OVRInput.RawButton swapButton;
    

    public void ShowData() {
        GameObject currentWorld = (int) activeData >= worlds.Count ? null : (GameObject) worlds[(int) activeData];
        if (currentWorld == null) {
            currentWorld = Instantiate(worldPrefab, centerEye.position + centerEye.forward * 2, Quaternion.identity);
            currentWorld.name = ((int) activeData) + "_World";
            currentWorld.GetComponent<World>().dataset = activeData;
            currentWorld.GetComponent<World>().Init();
            worlds[(int) activeData] = currentWorld;
        }
        currentWorld.SetActive(true);
        foreach (GameObject world in worlds) {
            if (world != null && world != currentWorld) {
                world.SetActive(false);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        worlds = new ArrayList();
        for (int i = 0; i < VolumeDataReader.numDataOptions; i++) {
            worlds.Add(null);
        }
        ShowData();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || OVRInput.GetDown(shrinkButton)) {
            World currentWorld = ((GameObject) worlds[(int) activeData]).GetComponent<World>(); 
            currentWorld.ResizeWorld(0.3f);
            currentWorld.EnablePhysics();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || OVRInput.GetDown(growButton)) {
            World currentWorld = ((GameObject) worlds[(int) activeData]).GetComponent<World>();
            currentWorld.transform.position = centerEye.position + centerEye.forward * 2;
            currentWorld.transform.forward = centerEye.forward; 
            currentWorld.ResizeWorld(3);
            currentWorld.DisablePhysics();
        }

        if (OVRInput.GetDown(swapButton)) {
            activeData = (VolumeDataReader.Data) (((int) (activeData + 1)) % VolumeDataReader.numDataOptions);
            ShowData();
        }
    }
}
