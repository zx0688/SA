using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonBuilder : MonoBehaviour
{
    [SerializeField] private GameObject[] _items;

    [SerializeField] private Sprite[] _backSprites;
    [SerializeField] private Sprite[] _topSprites;
    // private Dictionary<int, Dictionary<int, List<BackBuildVO>>> backBuild => Services.Player.GetPlayerVO.BackBuild;

    // public void BuildDungeon()
    // {

    //     if (backBuild == null)
    //     {
    //         Services.Player.GetPlayerVO.BackBuild = new Dictionary<int, Dictionary<int, List<BackBuildVO>>>();
    //     }

    //     Dictionary<int, List<BackBuildVO>> block = null;
    //     backBuild.TryGetValue(0, out block);
    //     if (block == null)
    //     {
    //         CreateObjects(0);
    //         backBuild.TryGetValue(0, out block);
    //     }

    //     for (int j = 0; j < _items.Length; j++)
    //     {
    //         GameObject go = _items[j];
    //         List<BackBuildVO> bo = block[j];
    //         BuildObjects(go.transform.Find("BackLayerUp").gameObject, true, bo.GetRange(0, 3));
    //         BuildObjects(go.transform.Find("BackLayerDown").gameObject, true, bo.GetRange(3, 3));
    //         BuildObjects(go.transform.Find("TopLayerUp").gameObject, false, bo.GetRange(6, 3));
    //         BuildObjects(go.transform.Find("TopLayerDown").gameObject, false, bo.GetRange(9, 3));
    //     }
    // }

    // private void CreateObjects(int location)
    // {
    //     Dictionary<int, List<BackBuildVO>> d = new Dictionary<int, List<BackBuildVO>>();
    //     for (int i = 0; i < 3; i++)
    //     {
    //         List<BackBuildVO> b = new List<BackBuildVO>();
    //         for (int j = 0; j < 12; j++)
    //         {
    //             BackBuildVO bvo = new BackBuildVO();
    //             bvo.Mirrored = Random.Range(0, 1f) > 0.5f;
    //             bvo.Off = Random.Range(0, 1f) > 0.8f;
    //             bvo.SpriteNumber = j < 6 ? Random.Range(0, _backSprites.Length) : Random.Range(0, _topSprites.Length);
    //             b.Add(bvo);
    //         }
    //         d.Add(i, b);
    //     }
    //     Services.Player.GetPlayerVO.BackBuild.Add(location, d);

    //     //Save
    //     string json = JsonUtility.ToJson(Services.Player.GetPlayerVO.BackBuild);
    //     SecurePlayerPrefs.SetString("buildBack", json);
    // }

    // private void BuildObjects(GameObject panel, bool back, List<BackBuildVO> data)
    // {
    //     for (int i = 0; i < 3; i++)
    //     {
    //         GameObject o = panel.transform.Find(i.ToString()).gameObject;
    //         Image image = o.GetComponent<Image>();
    //         if (data[i].Off == true)
    //         {
    //             image.enabled = false;
    //             continue;
    //         }
    //         image.enabled = true;
    //         Sprite element = back ? _backSprites[data[i].SpriteNumber] : _topSprites[data[i].SpriteNumber];
    //         image.transform.localScale = new Vector3(data[i].Mirrored ? -1f : 1f, 1f, 1f);
    //         image.sprite = element;

    //     }
    // }
}
