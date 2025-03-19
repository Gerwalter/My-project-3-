using UnityEngine;

public class Lupinranger : MonoBehaviour
{
    [SerializeField] bool _isArmed;
    [SerializeField] bool _isStealing;
    // Start is called before the first frame update

    public bool IsArmed { get { return _isArmed; } }
    public bool IsStealing { get { return _isStealing; } }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
