using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class BuildingUIListController : MonoBehaviour
{
    private GridManager gridManager;
    private BuildingHandler buildingHandler;
    [SerializeField] private GameObject ButtonPrefab;
    [SerializeField] private List<PlaceableObjectSO> placeableObjects;
    private float btnWidth;
    private float btnSpacing;
    private float btnWidthPadding;

    private void Awake()
    {
        btnWidth = ButtonPrefab.GetComponent<RectTransform>().sizeDelta.x;
        btnSpacing = this.GetComponent<HorizontalLayoutGroup>().spacing;
        btnWidthPadding = this.GetComponent<HorizontalLayoutGroup>().padding.left + this.GetComponent<HorizontalLayoutGroup>().padding.right;
    }

    private void Start()
    {
        buildingHandler = BuildingHandler._instance;
        gridManager = GridManager._instance;
        placeableObjects = gridManager.GetPlaceableObjects();
        foreach (PlaceableObjectSO pO in placeableObjects)
        {
            InstantiateButton(pO);
        }
        float currentHeight = this.GetComponent<RectTransform>().sizeDelta.y;
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(placeableObjects.Count * btnWidth + btnSpacing + btnWidthPadding, currentHeight);
    }

    private void InstantiateButton(PlaceableObjectSO pO)
    {
        GameObject newButton = Instantiate(ButtonPrefab, this.transform);
        newButton.transform.SetParent(this.transform);
        newButton.GetComponentInChildren<TextMeshProUGUI>().text = pO.name;
        newButton.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = pO.itemPicture;
        Button btnEl = newButton.GetComponent<Button>();
        btnEl.onClick.AddListener(() => buildingHandler.SetCurrentPlaceableObject(pO));
    }
}
