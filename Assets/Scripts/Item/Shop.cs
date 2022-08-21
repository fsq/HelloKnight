using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] public List<Item> Stock;

    // Keep singleton in Scene.
    public static Shop Instance;

    private Transform _container;
    private Transform _template;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // TBD: rogue-like? Revoke all purchases after death. 
            // DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _template = transform.Find("ItemTemplate");
        Debug.Assert(_template != null);
        _template.gameObject.SetActive(false);
        ShowMenu();
    }

    void Update()
    {

    }

    private void ShowMenu()
    {
        for (int i = 0; i < Stock.Count; ++i)
        {
            var item = Stock[i];
            CreateEntry(i, item.Icon, item.Name, item.Description, item.Price);
        }
        // CreateEntry(0, 92, "test item", "Hollow Knight/buff_melee_range");
    }

    private void CreateEntry(int index, Sprite icon, string name, string description, int price)
    {
        var entry = Instantiate(_template, transform);
        var rect_trans = entry.GetComponent<RectTransform>();
        float height = rect_trans.sizeDelta.y + 20;
        rect_trans.anchoredPosition = new Vector2(0, height * index);

        entry.Find("Icon").GetComponent<Image>().sprite = icon;
        entry.Find("Name").GetComponent<TextMeshProUGUI>().text = name;
        entry.Find("Description").GetComponent<TextMeshProUGUI>().text = description;
        entry.Find("Price").GetComponent<TextMeshProUGUI>().text = price.ToString();

        entry.gameObject.SetActive(true);
    }
}
