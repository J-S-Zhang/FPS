using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerCanvas : NetworkBehaviour
{
    public GameObject UI;
    public RawImage Crosshair;
    public Slider HP_slider;
    public override void OnStartLocalPlayer()
    {
        UI.SetActive(true);
    }
}
