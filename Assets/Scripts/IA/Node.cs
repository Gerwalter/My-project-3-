using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] Material _myBaseMaterial;
    [SerializeField] Material _myBlockMaterial;
    [SerializeField] Material _myStartMaterial;
    [SerializeField] Material _myGoalMaterial;
    public MeshRenderer myMesh;
    int _xPos, _yPos;
    [SerializeField] float neighborRadius = 1.1f;
    [SerializeField] List<Node> _neighbors = new List<Node>();


    [SerializeField] private bool block;
    [SerializeField] private int cost;

    public bool Block
    {
        get => block;
        set => block = value;
    }

    public int Cost
    {
        get => cost;
        set => cost = value;
    }
    [SerializeField] TextMeshProUGUI _costText;

    private void Start()
    {
        Initialize();
    }
    public void Initialize()
    {
        myMesh = GetComponent<MeshRenderer>();
        Cost = 1;
        _costText.text = Cost.ToString();
        PathfindingGameManager.instance.OnResetActivated += ResetNode;
    }

    void ResetNode()
    {
        if (Block) return;

        if (PathfindingGameManager.instance.startNode == this) ChangeColor(_myStartMaterial.color);
        else if (PathfindingGameManager.instance.goalNode == this) ChangeColor(_myGoalMaterial.color);
        else ChangeColor(_myBaseMaterial.color);
    }

    public void ChangeColor(Color color)
    {
        myMesh.material.color = color;
    }

    public List<Node> GetNeighbors
    {
        get
        {
            if (_neighbors.Count > 0) return _neighbors;

            Collider[] hits = Physics.OverlapSphere(transform.position, neighborRadius);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<Node>(out Node neighbor) && neighbor != this && !neighbor.Block)
                {
                    _neighbors.Add(neighbor);
                }
            }

            return _neighbors;
        }
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, neighborRadius);

        foreach (var neighbor in GetNeighbors)
        {
            Gizmos.DrawLine(transform.position, neighbor.transform.position);
        }
    }
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(2)) //Rueda del medio
        {
            Blocker();
        }

        if (Input.GetMouseButtonUp(0)) //Click Izquierdo
        {
            if (PathfindingGameManager.instance.startNode != null)
                PathfindingGameManager.instance.startNode.myMesh.material = _myBaseMaterial;

            PathfindingGameManager.instance.startNode = this;
            myMesh.material = _myStartMaterial;
        }

        if (Input.GetMouseButtonUp(1)) //Click Derecho
        {
            if (PathfindingGameManager.instance.goalNode != null)
                PathfindingGameManager.instance.goalNode.myMesh.material = _myBaseMaterial;

            PathfindingGameManager.instance.goalNode = this;
            myMesh.material = _myGoalMaterial;
        }

        if (Input.mouseScrollDelta.y > 0)
        {
            SetCost(+1);
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            SetCost(-1);
        }
    }

    private void Blocker()
    {
        Block = !Block;

        myMesh.material = Block ? _myBlockMaterial : _myBaseMaterial;

        if (Block) gameObject.layer = 3;
        else gameObject.layer = 0;
    }

    void SetCost(int value)
    {
        Cost = Mathf.Clamp(Cost + value, 1, int.MaxValue);
        _costText.text = Cost.ToString();
    }
}
