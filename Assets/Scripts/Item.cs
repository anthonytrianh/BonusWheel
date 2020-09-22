using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Item : MonoBehaviour
{
    #region Properties
    SpriteRenderer sr;
    TMP_Text text;
    TMP_Text secondaryText;

    Animator animator;

    public string itemName;

    [Header("Graphics")]
    public Sprite sprite;

    [Header("Item Count")]
    public int count;
    #endregion

    #region Public
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();

        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>();
        for (int i = 0; i < texts.Length; i++)
        {
            switch (i)
            {
                case 1:
                    text = texts[i];
                    break;
                case 0:
                    secondaryText = texts[i];
                    break;
            }
        }

        animator = GetComponentInChildren<Animator>();
    }

    [ExecuteInEditMode]
    private void Update()
    {
        SetGraphics();
    }

    public void InitItem(Sprite sprite, int count)
    {
        this.sprite = sprite;
        this.count = count;
    }

    public void SetItem(Item other)
    {
        this.sprite = other.sprite;
        this.count = other.count;
        SetGraphics();
    }

    public void SetPositionRotation(int index, int maxIndex)
    {
        float angle = 360.0f / maxIndex * index + Wheel.angleOffset;

        // Set Rotation
        this.transform.eulerAngles = new Vector3(0, 0, -angle);

        // Set Position
        float x = Wheel.radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        float y = Wheel.radius * Mathf.Cos(angle * Mathf.Deg2Rad);

        transform.position = new Vector3(x, y);
    }

    public string ToString()
    {
        return sprite.name + " " + count + " " + secondaryText.text;
    }

    public string GetPrizeName()
    {
        switch (itemName)
        {
            case "life":
                return string.Format("Life {0} {1}", text.text, secondaryText.text);
            case "gem":
                return string.Format("Gems {0}", count);
            default:
                return string.Format("{0} {1}X", char.ToUpper(itemName[0]) + itemName.Substring(1), count);
        }
    }

    public void ShowPrize()
    {
        animator.SetTrigger("show");
    }
    #endregion

    #region Private
    void SetGraphics()
    {
        if (sprite == null || count == 0)
            return;

        sr.sprite = sprite;

        if (sprite.name.Contains("heart"))
        {
            text.text = string.Format("{0}", count);
            secondaryText.text = "min";
            itemName = "life";
        }
        else
        {
            text.text = string.Format("x{0}", count);
            secondaryText.text = "";
            itemName = sprite.name;
        }
    }
    #endregion

    #region Math
    public static float ClampEulers(float angle, float min = 0, float max = 360)
    {
        float result = angle - Mathf.CeilToInt(angle / 360f) * 360f;
        return result < 0 ? result + 360 : result;
    }
    #endregion
}
