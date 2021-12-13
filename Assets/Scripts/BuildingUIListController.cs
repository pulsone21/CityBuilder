using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingUIListController : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameObject ButtonPrefab;
    [SerializeField] private List<PlaceableObject> placeableObjects;


    private void Start()
    {
        placeableObjects = gridManager.GetPlaceableObjects();
        foreach (PlaceableObject pO in placeableObjects)
        {
            InstantiateButton(pO);
        }
    }

    private void InstantiateButton(PlaceableObject pO)
    {
        GameObject newButton = Instantiate(ButtonPrefab, this.transform);
        newButton.transform.SetParent(this.transform);
        newButton.GetComponentInChildren<TextMeshProUGUI>().text = pO.name;
        newButton.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = pO.itemPicture;
        Button btnEl = newButton.GetComponent<Button>();
        btnEl.onClick.AddListener(() => gridManager.SetCurrentPlaceableObject(pO));


    }
}
