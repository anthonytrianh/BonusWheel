using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;

public class Wheel : MonoBehaviour
{
    #region Properties
    GameManager gm;

    /// Text-based specifications
    /// *Number of items*
    /// *Sprite name* *item count* *drop rate*
    /// *Sprite name* *item count* *drop rate*
    /// ...
    /// </summary>
    public TextAsset text;
    public Transform container;
    public Item itemPrefab;

    List<Item> items = new List<Item>();
    Dictionary<Item, ValueRange> dropRates = new Dictionary<Item, ValueRange>();

    public static float radius = 3.5f;
    public static float angleOffset = 360 / 16.0f;

    int dropIndex;

    [Header("Animation Settings")]
    [Range(0, 0.5f)] public float tiltBackAmount = 0.21f;
    [Range(0, 1.0f)] public float tiltBackDuration = 0.5f;
    public float rotationDuration = 5f;
    [Range(0, 6)] public int spinCycles = 5;

    bool isSpinning = false;

    [Header("Button")]
    public GameObject button;
    #endregion

    #region MonoBehavior
    // Start is called before the first frame update
    void Start()
    {
        gm = GetComponentInParent<GameManager>();
        Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("Result"));
        ProcessText();   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Enter"))
        {
            UnitTest();
        }
    }
    #endregion

    #region Public
    public void Spin(bool skipAnimation = false)
    {
        if (isSpinning)
            return;

        // Roll
        int roll = Random.Range(0, 101);
        for (int i = 0; i < items.Count; i++)
        {
            if (roll >= dropRates[items[i]].min && roll < dropRates[items[i]].max)
            {
                dropIndex = i;
                break;
            } 
        }

        Item item = items[dropIndex];
        float itemAngle = Item.ClampEulers(item.transform.eulerAngles.z + transform.eulerAngles.z);
        float destinationAngle = (-itemAngle) - 360 * spinCycles;
        if (skipAnimation)
            destinationAngle = item.transform.eulerAngles.z;

        Debug.Log("Item Drop: " + item.ToString());

        StartCoroutine(Animate(destinationAngle, skipAnimation));


    }
    #endregion

    #region Private
    IEnumerator Animate(float endAngle, bool skipAnimation = false)
    {
        isSpinning = true;

        float clock = 0;
        float startAngle = (transform.eulerAngles.z);
        endAngle = endAngle + startAngle;

        if (skipAnimation)
        {
            transform.eulerAngles = new Vector3(0, 0, endAngle);
            isSpinning = false;
            yield break;
        }


        // Tilt backwards
        while (clock < tiltBackDuration)
        {
            transform.Rotate(0, 0, tiltBackAmount);

            clock += Time.deltaTime;
            yield return null;
        }

        // Main Rotation
        clock = 0;
        float angleBeforeDeceleration = endAngle;
        while (clock < rotationDuration)
        {
            float angle = EasingEquations.EaseOutCubic(startAngle, endAngle, clock / rotationDuration);
            transform.eulerAngles = new Vector3(0, 0, startAngle + angle);

            clock += Time.deltaTime;
            yield return null;
        }

        transform.eulerAngles = new Vector3(0, 0, Item.ClampEulers(transform.eulerAngles.z));

        button.SetActive(false);
        // Show Results
        yield return StartCoroutine(gm.results.ShowResults(items[dropIndex]));

        button.SetActive(true);
        // Enable spin again
        isSpinning = false;
    }

    void ProcessText()
    {
        if (text == null)
            return;

        items.Clear();

        string script = text.ToString();

        string[] lines = script.Split(new string[] { "\n", "\r" }, System.StringSplitOptions.RemoveEmptyEntries);

        int percentage = 0;

        int count = int.Parse(lines[0]);

        // Process each line and instantiate item drops
        for (int i = 0; i < count; i++)
        {
            string[] contents = lines[i + 1].Split(new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);

            Sprite sprite = Resources.Load<Sprite>("Sprites/" + contents[0]);
            int itemCount = int.Parse(contents[1]);
            int dropChance = int.Parse(contents[2]);

            // Instantiate item
            Item item = Instantiate(itemPrefab, container, true);
            item.gameObject.name = string.Format("{0} {1}", sprite.name, itemCount);
            SetLayerRecursive(item.gameObject, this.gameObject.layer); 
            item.InitItem(sprite, itemCount);
            item.SetPositionRotation(i, count);

            // Add to array
            items.Add(item);

            ValueRange dropRange = new ValueRange();
            dropRange.min = percentage;
            dropRange.max = percentage + dropChance;

            dropRates[item] = dropRange;
            percentage += dropChance;

            if (percentage > 100)
                Debug.LogWarning("Total Drop Chance over 100!");

        }
    }

    void SetLayerRecursive(GameObject obj, int layer)
    {
        if (obj == null)
            return;

        obj.layer = layer;
        foreach (Transform o in obj.transform)
        {
            if (o == null)
                return;

            SetLayerRecursive(o.gameObject, layer);
        }
    }
    #endregion

    #region Unit Testing
    public Dictionary<Item, int> dropCount = new Dictionary<Item, int>();
    public void UnitTest()
    {
        for (int i = 0; i < items.Count; i++)
            dropCount[items[i]] = 0;

        for (int i = 0; i < 1000; i++)
        {
            Spin(true);
            dropCount[items[dropIndex]]++;
        }

        string path = "Assets/Resources/Text/Unit Test.txt";
        File.WriteAllText(path, string.Empty);

        StreamWriter sw = new StreamWriter(path, true);

        sw.WriteLine("Results from 1000 simulations:");

        for (int i = 0; i < items.Count; i++)
        {
            StringBuilder sb = new StringBuilder(items[i].GetPrizeName());
            sb.Append(": ");
            sb.Append(dropCount[items[i]]);

            sw.WriteLine(sb.ToString());
        }

        sw.Close();
    }
    #endregion
}

public struct ValueRange
{
    public int min;
    public int max;
}