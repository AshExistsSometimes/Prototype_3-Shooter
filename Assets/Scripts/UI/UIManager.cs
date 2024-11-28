using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance { get; private set; }

    [Header("World Map")]
    public GameObject Map;
    public bool IsMapEnabled;

    [Header("Pause Logic")]
    public PauseMenuManager pauseLogic;

    [Header("Items")]
    public GameObject[] Items;

    private void Start()
    {
        Equip(0);
        TurnOffWorldMap();
    }
    private void Update()
    {
        MapUIControl();
    }

    /////////////////////////////////
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

    }


    // ITEM EQUIPPING LOGIC //

    public void Equip(int index)
    {
        for (int I = 0; I < Items.Length; I++)
        {
            if (I == index)// If the item has the ID number of the equipped item
            {
                Items[I].SetActive(true);// Hold the item
            }
            else
            {
                Items[I].SetActive(false);// Do not hold items that arent equipped
            }
        }

    }
    // WORLD MAP LOGIC //
    public void TurnOffWorldMap()
    {
        pauseLogic.IsGamePaused = false;
        IsMapEnabled = false;
    }

    public void MapUIControl()
    {
        // Inputs and switching states
        if (IsMapEnabled && Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
        {
            pauseLogic.IsGamePaused = false;
            IsMapEnabled = false;
        }
        else if (!IsMapEnabled && (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Tab)))
        {
            IsMapEnabled = true;
            pauseLogic.IsGamePaused = true;
        }

        // Switching if active or not (independant of Input if anything else needs to access it)
        if (IsMapEnabled)
        {
            Map.SetActive(true);
        }
        else if (!IsMapEnabled)
        {
            Map.SetActive(false);
        }
    }
}
