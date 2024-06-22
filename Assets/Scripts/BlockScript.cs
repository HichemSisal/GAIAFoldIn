using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;


[RequireComponent(typeof(BoxCollider))]
public class BlockScript : MonoBehaviour
{
    [SerializeField]
    private Transform rightPivot;
    [SerializeField]
    private Transform leftPivot;
    [SerializeField]
    private Transform upPivot;
    [SerializeField]
    private Transform downPivot;  

    private BoxCollider boxCollider; // box collider of the block 

    private List<BlockScript> blockChildren = new List<BlockScript>();  // children blocks of the block 

    private BlockScript blockParent;  // parent block of the block 

     [SerializeField]
    private int blockLayer; // O = first level , 1 = second level , 2 = third level

    private Vector3 blockInitialPosition; // initial position of the block in the scene  

    public void AddBlockChild(BlockScript block)
    {
        blockChildren.Add(block);
    }
    public void RemoveBlockChild(BlockScript block)
    {
        blockChildren.Remove(block);
    }
    public List<BlockScript> GetBlockChildren()
    {
        return blockChildren;
    }
    public int GetBlockChildrenCount()
    {
        return blockChildren.Count;
    }
    public int Getlayer()
    {
        return blockLayer;
    }
    public void Setlayer(int layer)
    {
        blockLayer = layer;
    }
    public void SetParent(BlockScript parent)
    {
        blockParent = parent;
    }
    public BlockScript GetParent()
    {
        return blockParent;
    }
    public void SetInitialPosition(Vector3 position)
    {
        blockInitialPosition = position;
    }
    public Vector3 GetInitialPosition()
    {
        return blockInitialPosition;
    }   
    public void SetColliderState(bool state)
    {
        boxCollider.enabled = state;
    }
    public float GetPosX()
    {
        return transform.position.x;
    }
    public float GetPosZ()
    {
        return transform.position.z;
    }   

    private void SetPosition(Vector3 position, float duration)
    {
        transform.DOMove(position, duration);
    }

    private void Awake () 
    {
        boxCollider = GetComponent<BoxCollider>();
        SetInitialPosition(transform.position);
        Setlayer(0);
    }

    public void Fold(Direction direction, int count, Action success, float rotationDuration)
    {
        StartCoroutine(IEFold(direction, count, success, rotationDuration));
    }

public void Unfold(Direction direction,  Action success, float rotationDuration)
{
    StartCoroutine(IEUnfold(direction,  success, rotationDuration));
}

private IEnumerator IEFold(Direction direction, int count, Action success, float rotationDuration)
{
   transform.position += new Vector3(0, count * 0.05f, 0);

        Transform parent = transform.parent;

        Transform dynamicPivotParent = GameManager.Instance.GetDynamicPivot();

        int x = 0;
        int z = 0;

        if (direction == Direction.Right)
        {
            dynamicPivotParent.SetPositionAndRotation(rightPivot.position, Quaternion.Euler(0, 0, 0));
            x = 1;
            z = 0;
        }
        else if (direction == Direction.Left)
        {
            dynamicPivotParent.SetPositionAndRotation(leftPivot.position, Quaternion.Euler(0, 0, 0));
            x = -1;
            z = 0;
        }
        else if (direction == Direction.Up)
        {
            dynamicPivotParent.SetPositionAndRotation(upPivot.position, Quaternion.Euler(0, 0, 0));
            x = 0;
            z = -1;
        }
        else if (direction == Direction.Down)
        {
            dynamicPivotParent.SetPositionAndRotation(downPivot.position, Quaternion.Euler(0, 0, 0));
            x = 0;
            z = 1;

        }
        
        transform.parent = dynamicPivotParent;

        Tween myTween = dynamicPivotParent.DORotate(new Vector3(z * -180, 0, x * -180), rotationDuration);

        yield return myTween.WaitForCompletion();

        transform.parent = parent;

        success();
        Debug.Log("Success is called    1");
}


private IEnumerator IEUnfold (Direction direction , Action success, float rotationDuration)
{

    Transform parent = transform.parent;

    Transform dynamicPivotParent = GameManager.Instance.GetDynamicPivot();

    int x = 0;
    int z = 0;

    if (direction == Direction.Right)
    {
        dynamicPivotParent.SetPositionAndRotation(leftPivot.position, Quaternion.Euler(0, 0, 0));
        x = 1;
        z = 0;
    }
    else if (direction == Direction.Left)
    {
        dynamicPivotParent.SetPositionAndRotation(rightPivot.position, Quaternion.Euler(0, 0, 0));
        x = -1;
        z = 0;
    }
    else if (direction == Direction.Up)
    {
        dynamicPivotParent.SetPositionAndRotation(downPivot.position, Quaternion.Euler(0, 0, 0));
        x = 0;
        z = -1;
    }
    else if (direction == Direction.Down)
    {
        dynamicPivotParent.SetPositionAndRotation(upPivot.position, Quaternion.Euler(0, 0, 0));
        x = 0;
        z = 1;

    }
    
    transform.parent = dynamicPivotParent;

    Tween myTween = dynamicPivotParent.DORotate(new Vector3(z * -180, 0, x * -180), rotationDuration);

    yield return myTween.WaitForCompletion();

    transform.parent = parent;
//
    SetPosition(GetInitialPosition() , 0.2f);
    yield return new WaitUntil(() => transform.position == blockInitialPosition);
    success();
    Debug.Log("Success is called    1");





}

}