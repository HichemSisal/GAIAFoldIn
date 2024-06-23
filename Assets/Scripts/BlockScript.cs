using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

/// <summary>
/// Represents a block in the game.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class BlockScript : MonoBehaviour
{
    [SerializeField]
    private Transform rightPivot; // right pivot of the block
    [SerializeField]
    private Transform leftPivot; // left pivot of the block
    [SerializeField]
    private Transform upPivot; // up pivot of the block
    [SerializeField]
    private Transform downPivot;    // down pivot of the block

    private BoxCollider boxCollider; // box collider of the block 

    private List<BlockScript> blockChildren = new List<BlockScript>();  // children blocks of the block 

    private BlockScript blockParent;  // parent block of the block 

    [SerializeField]
    private int blockLayer; // O = first level , 1 = second level , 2 = third level

    private Vector3 blockInitialPosition; // initial position of the block in the scene  

    /// <summary>
    /// Adds a child block to this block.
    /// </summary>
    /// <param name="block">The child block to add.</param>
    public void AddBlockChild(BlockScript block)
    {
        blockChildren.Add(block);
    }

    /// <summary>
    /// Removes a child block from this block.
    /// </summary>
    /// <param name="block">The child block to remove.</param>
    public void RemoveBlockChild(BlockScript block)
    {
        blockChildren.Remove(block);
    }

    /// <summary>
    /// Gets the list of child blocks of this block.
    /// </summary>
    /// <returns>The list of child blocks.</returns>
    public List<BlockScript> GetBlockChildren()
    {
        return blockChildren;
    }

    /// <summary>
    /// Gets the number of child blocks of this block.
    /// </summary>
    /// <returns>The number of child blocks.</returns>
    public int GetBlockChildrenCount()
    {
        return blockChildren.Count;
    }

    /// <summary>
    /// Gets the layer of this block.
    /// </summary>
    /// <returns>The layer of the block.</returns>
    public int Getlayer()
    {
        return blockLayer;
    }

    /// <summary>
    /// Sets the layer of this block.
    /// </summary>
    /// <param name="layer">The layer to set.</param>
    public void Setlayer(int layer)
    {
        blockLayer = layer;
    }

    /// <summary>
    /// Sets the parent block of this block.
    /// </summary>
    /// <param name="parent">The parent block.</param>
    public void SetParent(BlockScript parent)
    {
        blockParent = parent;
    }

    /// <summary>
    /// Gets the parent block of this block.
    /// </summary>
    /// <returns>The parent block.</returns>
    public BlockScript GetParent()
    {
        return blockParent;
    }

    /// <summary>
    /// Sets the initial position of this block.
    /// </summary>
    /// <param name="position">The initial position.</param>
    public void SetInitialPosition(Vector3 position)
    {
        blockInitialPosition = position;
    }

    /// <summary>
    /// Gets the initial position of this block.
    /// </summary>
    /// <returns>The initial position.</returns>
    public Vector3 GetInitialPosition()
    {
        return blockInitialPosition;
    }   

    /// <summary>
    /// Sets the state of the box collider of this block.
    /// </summary>
    /// <param name="state">The state to set.</param>
    public void SetColliderState(bool state)
    {
        boxCollider.enabled = state;
    }

    /// <summary>
    /// Gets the X position of this block.
    /// </summary>
    /// <returns>The X position.</returns>
    public float GetPosX()
    {
        return transform.position.x;
    }

    /// <summary>
    /// Gets the Z position of this block.
    /// </summary>
    /// <returns>The Z position.</returns>
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

    /// <summary>
    /// Folds the block in the specified direction.
    /// </summary>
    /// <param name="direction">The direction to fold.</param>
    /// <param name="count">The number of folds.</param>
    /// <param name="success">The action to call when folding is successful.</param>
    /// <param name="rotationDuration">The duration of the rotation animation.</param>
    public void Fold(Direction direction, int count, Action success, float rotationDuration)
    {
        StartCoroutine(IEFold(direction, count, success, rotationDuration));
    }

    /// <summary>
    /// Unfolds the block in the specified direction.
    /// </summary>
    /// <param name="direction">The direction to unfold.</param>
    /// <param name="success">The action to call when unfolding is successful.</param>
    /// <param name="rotationDuration">The duration of the rotation animation.</param>
    /// <param name="timeToWait">The time to wait before setting the position.</param>
    public void Unfold(Direction direction, Action success, float rotationDuration, float timeToWait)
    {
        StartCoroutine(IEUnfold(direction, success, rotationDuration, timeToWait));
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
    }

    private IEnumerator IEUnfold(Direction direction, Action success, float rotationDuration, float timeToWait)
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

        SetPosition(GetInitialPosition(), timeToWait);
        yield return new WaitUntil(() => transform.position == blockInitialPosition);
        success();
    }
}