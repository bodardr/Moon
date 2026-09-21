using Bodardr.Databinding.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public IUpgrade Upgrade { get; set; }
    private Button button;
    private BindingNode node;

    private void Awake()
    {
        node = GetComponent<BindingNode>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        UpgradeSystem.Instance.OnUpgradeHovered(Upgrade);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        UpgradeSystem.Instance.OnUpgradeUnhovered(Upgrade);
    }
    
    private void OnClick()
    {
        UpgradeSystem.Instance.OnUpgradeClicked(Upgrade);
        node.UpdateAll();
    }
}
