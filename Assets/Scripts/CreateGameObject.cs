using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateGameObject : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    private GameObject go;
    private TextMeshProUGUI text;
    
    // Start is called before the first frame update
    void Start()
    {
        var button = GameObject.Find("ButtonAgain");
        button.GetComponent<Button>().onClick.AddListener(Restart);
        text = button.GetComponentInChildren<TextMeshProUGUI>();
    }

    void Restart()
    {
        if (go != null)
        {
            text.text = "\uf04b";
            Destroy(go);
        } else 
        {
            text.text = "\uf04d";
            go = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
}
