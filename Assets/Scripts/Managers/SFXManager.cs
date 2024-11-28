using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{

    public List<AudioClip> SFX_List = new List<AudioClip>();
    public List<string> SFX_Names = new List<string>();

    public Dictionary<string, AudioClip> SFX_Lib =
        new Dictionary<string, AudioClip>();

    public static SFXManager TheSFXManager;

    public GameObject SFXPrefab;

    void Start()
    {
        for (int i = 0; i < SFX_List.Count; i++)
        {
            SFX_Lib.Add(SFX_Names[i], SFX_List[i]);
        }

        TheSFXManager = this;
    }

    public void PlaySFX(string SFXName)
    {
        if (SFX_Lib.ContainsKey(SFXName))
        {
            GameObject LeSFX = Instantiate(SFXPrefab);
            LeSFX.GetComponent<AudioSource>().clip = SFX_Lib[SFXName];
            LeSFX.GetComponent<AudioSource>().Play();
        }
    }

}
