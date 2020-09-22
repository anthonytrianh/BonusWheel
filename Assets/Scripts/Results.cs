using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Results : MonoBehaviour
{
    #region Properties
    bool input = false;
    Item prizeItem;

    public GameObject claimButton;
    #endregion

    #region Mono
    private void Start()
    {
        prizeItem = GetComponentInChildren<Item>();
        claimButton.SetActive(false);
    }
    #endregion

    #region Public
    public IEnumerator ShowResults(Item prize)
    {
        Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("Wheel"));
        Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("Result");

        claimButton.SetActive(true);

        prizeItem.SetItem(prize);
        prizeItem.ShowPrize();

        yield return new WaitUntil(() => input);
        input = false;

        Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("Result"));
        Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("Wheel");

        claimButton.SetActive(false);

    }

    public void Claim()
    {
        if (!input)
            input = true;
    }
    #endregion
}
