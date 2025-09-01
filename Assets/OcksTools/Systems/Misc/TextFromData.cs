using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextFromData : MonoBehaviour
{
    public string type = "";
    private TextMeshProUGUI jessie;
    public bool UseFixedUpdate = false;
    // Start is called before the first frame update
    void OnEnable()
    {
        jessie= GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!UseFixedUpdate)
        {
            j();
        }
    }
    void FixedUpdate()
    {
        if (UseFixedUpdate)
        {
            j();
        }
    }
    public void j()
    {

        string a = "";
        switch (type)
        {
            case "Scrap":
                a = "Scrap: " + Converter.NumToRead(GameHandler.Scrap.ToString());
                break;
        }

        jessie.text = a;
    }
}
