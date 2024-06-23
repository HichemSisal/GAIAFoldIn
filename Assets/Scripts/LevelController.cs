using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum Direction  // directions of the swipe
{
    Right,
    Left,
    Up,
    Down
}
public class LevelController : MonoBehaviour 
{
    [SerializeField]
    private List<BlockScript> blocks = new List<BlockScript>();  // list of blocks in the scene
    [SerializeField]
    private BlockScript blockHit; // the block that is hit by the swipe
    [SerializeField]
    private bool isSwiping = false; // to prevent multiple swipes at the same time
    [SerializeField]
    private bool isBusy = false; // to prevent multiple moves at the same time
    private float rotationDuration = 0.75f; // duration of the rotation

    private float rewindRotationDuration = 0.15f; // duration of the rewind rotation
    [SerializeField]
  private List<BlockScript> blocksToRewind = new List<BlockScript>(); // list of blocks to rewind


    private void OnEnable()
    {
        EasyTouch.On_SwipeStart += OnSwipeStart;
        EasyTouch.On_Swipe += OnSwipe;
        EasyTouch.On_SwipeEnd += OnSwipeEnd;
        GameManager.Instance.OnFullRewind += OnFullRewind;
    }

    private void OnDisable()
    {
        EasyTouch.On_SwipeStart -= OnSwipeStart;
        EasyTouch.On_Swipe -= OnSwipe;
        EasyTouch.On_SwipeEnd -= OnSwipeEnd;
        GameManager.Instance.OnFullRewind -= OnFullRewind;
    }


    private void OnSwipeStart(Gesture gesture)
    {
        if (isSwiping || isBusy)
        {
            return;
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            blockHit = hit.collider.GetComponent<BlockScript>();
        }
    }

private void OnSwipe  (Gesture gesture)
{
    if (blockHit == null)
    {
        return;
    }

    if (isSwiping || isBusy)
    {
        return;
    }

    isSwiping = true;
    isBusy = true;
    if (gesture.swipe == EasyTouch.SwipeType.Left)
    {
        FindBlockParentToFolddInto(blockHit, Direction.Left,out BlockScript blockParentToFoldInto);
        if(blockParentToFoldInto != null && CheckHasMaxOneNeighbor(blockHit))
        {
            Debug.Log("Fold");
            FoldBlock(blockHit, Direction.Left, blockParentToFoldInto);
        }
        else 
        { 
            FindBlockChildToUnfoldToDirection(blockHit, Direction.Left, out BlockScript blockChildToUnfold);
            if(blockChildToUnfold != null)
            {
                Debug.Log("Unfold");
                UnfoldBlock(blockHit, Direction.Left , blockChildToUnfold,rotationDuration,0.2f);
            }
            else
            {
                Debug.Log("isBusy = false");
                isBusy = false;
            }
        }
    }
    else if (gesture.swipe == EasyTouch.SwipeType.Right)
    {
       FindBlockParentToFolddInto(blockHit, Direction.Right,out BlockScript blockParentToFoldInto);
        if(blockParentToFoldInto != null && CheckHasMaxOneNeighbor(blockHit))
        {
            FoldBlock(blockHit, Direction.Right, blockParentToFoldInto);
        }
        else 
        { 
            FindBlockChildToUnfoldToDirection(blockHit, Direction.Right, out BlockScript blockChildToUnfold);
            if(blockChildToUnfold != null)
            {
                UnfoldBlock(blockHit, Direction.Right , blockChildToUnfold,rotationDuration,0.2f);
            }
            else
            {
                isBusy = false;
            }
        }
    }
    else if (gesture.swipe == EasyTouch.SwipeType.Up)
    {
       FindBlockParentToFolddInto(blockHit, Direction.Up,out BlockScript blockParentToFoldInto);
        if(blockParentToFoldInto != null && CheckHasMaxOneNeighbor(blockHit))
        {
            FoldBlock(blockHit, Direction.Up, blockParentToFoldInto);
        }
        else 
        { 
            FindBlockChildToUnfoldToDirection(blockHit, Direction.Up, out BlockScript blockChildToUnfold);
            if(blockChildToUnfold != null)
            {
                UnfoldBlock(blockHit, Direction.Up , blockChildToUnfold,rotationDuration,0.2f);
            }
            else
            {
                isBusy = false;
            }
        }
    }
    else if (gesture.swipe == EasyTouch.SwipeType.Down)
    {
        FindBlockParentToFolddInto(blockHit, Direction.Down,out BlockScript blockParentToFoldInto);
        if(blockParentToFoldInto != null && CheckHasMaxOneNeighbor(blockHit))
        {
            FoldBlock(blockHit, Direction.Down, blockParentToFoldInto);
        }
        else 
        { 
            FindBlockChildToUnfoldToDirection(blockHit, Direction.Down, out BlockScript blockChildToUnfold);
            if(blockChildToUnfold != null)
            {
                UnfoldBlock(blockHit, Direction.Down , blockChildToUnfold,rotationDuration,0.2f);
            }
            else
            {
                isBusy = false;
            }
        }
    } 
  

}

private void OnSwipeEnd(Gesture gesture)
{
    isSwiping = false;

}


private void FindBlockParentToFolddInto (BlockScript block , Direction direction, out BlockScript blockParentToFoldInto)
{
    // Find the block to fold into
    // If the block has a neighbor in the direction of the swipe, then fold into that neighbor
    blockParentToFoldInto = null;
    if (direction == Direction.Left)
    {           
            blockParentToFoldInto = blocks.Find(x => x.GetPosX() == block.GetPosX() - 1 && x.GetPosZ() == block.GetPosZ());   
    }
    else if (direction == Direction.Right)
    {        
            blockParentToFoldInto = blocks.Find(x => x.GetPosX() == block.GetPosX() + 1 && x.GetPosZ() == block.GetPosZ());    
    }
    else if (direction == Direction.Up)
    {      
            blockParentToFoldInto = blocks.Find(x => x.GetPosX() == block.GetPosX() && x.GetPosZ() == block.GetPosZ() + 1);     
    }
    else if (direction == Direction.Down)
    {
            blockParentToFoldInto = blocks.Find(x => x.GetPosX() == block.GetPosX() && x.GetPosZ() == block.GetPosZ() - 1);
    }
}

   private void FindBlockChildToUnfoldToDirection(BlockScript block, Direction direction, out BlockScript blockChildToUnfold)
{
    blockChildToUnfold = null;
    
    if(block.GetBlockChildrenCount() == 0 || block.GetBlockChildren() == null)
    {
        return;
    }
    List<BlockScript> blockChildren = block.GetBlockChildren();
    if (direction == Direction.Left)
    {
        blockChildToUnfold = blockChildren.Find(x => x.GetInitialPosition().x == block.GetPosX() - 1 && blockChildren.IndexOf(x) == blockChildren.Count - 1);
    }
    else if (direction == Direction.Right)
    {
        blockChildToUnfold = blockChildren.Find(x => x.GetInitialPosition().x == block.GetPosX() + 1 && blockChildren.IndexOf(x) == blockChildren.Count - 1);
    }
    else if (direction == Direction.Up)
    {
        blockChildToUnfold = blockChildren.Find(x => x.GetInitialPosition().z == block.GetPosZ() + 1 && blockChildren.IndexOf(x) == blockChildren.Count - 1);
    }
    else if (direction == Direction.Down)
    {
        blockChildToUnfold = blockChildren.Find(x => x.GetInitialPosition().z == block.GetPosZ() - 1 && blockChildren.IndexOf(x) == blockChildren.Count - 1);
    }
    
}


private bool CheckHasMaxOneNeighbor (BlockScript block )

{
    List<BlockScript> neighbors = new List<BlockScript>();
    neighbors.Add(blocks.Find(x => x.GetPosX() == block.GetPosX() + 1 && x.GetPosZ() == block.GetPosZ()));
    neighbors.Add(blocks.Find(x => x.GetPosX() == block.GetPosX() - 1 && x.GetPosZ() == block.GetPosZ()));
    neighbors.Add(blocks.Find(x => x.GetPosX() == block.GetPosX() && x.GetPosZ() == block.GetPosZ() + 1));
    neighbors.Add(blocks.Find(x => x.GetPosX() == block.GetPosX() && x.GetPosZ() == block.GetPosZ() - 1));
    neighbors.RemoveAll(item => item == null);

    if(neighbors.Count>1)
    {
        return false;
    }
    else
    {
        return true;
    }
}
private void FoldBlock(BlockScript block , Direction direction, BlockScript blockParentToFoldInto)
{
    int blockChildrensCount = GetBlockChildrensCount(block);
    int blockParentToFoldIntoChildrensCount = GetBlockChildrensCount(blockParentToFoldInto);
    block.Fold(direction, blockChildrensCount + blockParentToFoldIntoChildrensCount   + 1  ,    () =>
    {
        block.SetParent(blockParentToFoldInto);
        block.transform.SetParent(blockParentToFoldInto.transform);
        blockParentToFoldInto.AddBlockChild(block);
        SetLayersAfterFold(block, blockParentToFoldInto);
        isBusy = false;
        block.SetColliderState(false);
        blockHit = null;
        blocksToRewind.Add(block);
        CheckWin(GameManager.Instance.LevelCompleted);
    }, rotationDuration);

}

private void UnfoldBlock(BlockScript block, Direction direction, BlockScript blockChildToUnfold,float rotationDuration,float timeToWait)
{
    blockChildToUnfold.Unfold(direction,  () =>
    {
        blockChildToUnfold.SetParent(null);
        blockChildToUnfold.transform.SetParent(GameManager.Instance.GetLevelTransform());
        block.RemoveBlockChild(blockChildToUnfold);
        SetLayersAfterUnfold(blockChildToUnfold , block   );
        isBusy = false;
        blockChildToUnfold.SetColliderState(true);
        blockHit = null;
        blocksToRewind.Remove(blockChildToUnfold);
        CheckWin(GameManager.Instance.LevelCompleted);
    }, rotationDuration, timeToWait);
}

private void SetLayersAfterFold(BlockScript block, BlockScript blockParentToFoldInto)
{
    int blockChildrensCount = GetBlockChildrensCount(block);
    blockParentToFoldInto.Setlayer(blockParentToFoldInto.Getlayer() + blockChildrensCount + 1);
    block.Setlayer(0);
    SetParentChildrenLayerAfterFold(block ,  blockParentToFoldInto,blockChildrensCount);
    SetBlockChildrenLayerAfterFold(block, blockChildrensCount);
}

private void SetLayersAfterUnfold(BlockScript blockChildToUnfold, BlockScript block)
{
    int blockChildToUnfoldChildrensCount = GetBlockChildrensCount(blockChildToUnfold);
    block.Setlayer(block.Getlayer() - blockChildToUnfoldChildrensCount - 1  );
    blockChildToUnfold.Setlayer(blockChildToUnfoldChildrensCount);
    SetBlockParentChildrenLayerAfterUnfold( blockChildToUnfold, block  ,blockChildToUnfoldChildrensCount);
    SetBlockChildChildrenLayerAfterUnfold(blockChildToUnfold, blockChildToUnfoldChildrensCount);
}

private int GetBlockChildrensCount(BlockScript block)
{
    if(block.GetBlockChildrenCount() == 0 || block.GetBlockChildren() == null)
    {
        return 0;
    }
    
    int count = block.GetBlockChildrenCount();
    foreach (BlockScript child in block.GetBlockChildren())
    {
        count += GetBlockChildrensCount(child);
    }
    return count;
}

private void SetParentChildrenLayerAfterFold(BlockScript block , BlockScript blockParentToFoldInto, int blockChildrensCount)
{
    if(blockParentToFoldInto.GetBlockChildrenCount() == 0 || blockParentToFoldInto.GetBlockChildren() == null)
    {
        return;
    }
    foreach (BlockScript child in blockParentToFoldInto.GetBlockChildren())
    {
        if(child == block)
        {
            continue;
        }
        child.Setlayer(child.Getlayer() + blockChildrensCount+1);
        SetParentChildrenLayerAfterFold(block, child, blockChildrensCount);
    }
}

private void SetBlockChildrenLayerAfterFold(BlockScript block, int blockChildrensCount)
{
    if(block.GetBlockChildrenCount() == 0 || block.GetBlockChildren() == null)
    {
        return;
    }
    foreach (BlockScript child in block.GetBlockChildren())
    {
        child.Setlayer(blockChildrensCount - child.Getlayer());
        SetBlockChildrenLayerAfterFold(child, blockChildrensCount);
    }
}

private void SetBlockParentChildrenLayerAfterUnfold(BlockScript blockChildToUnfold , BlockScript block, int blockChildToUnfoldChildrensCount)
{
    if(block.GetBlockChildrenCount() == 0 || block.GetBlockChildren() == null)
    {
        return;
    }
    foreach (BlockScript child in block.GetBlockChildren())
    {
        if(child == blockChildToUnfold)
        {
            continue;
        }
        child.Setlayer(child.Getlayer() - blockChildToUnfoldChildrensCount - 1);
        SetBlockChildChildrenLayerAfterUnfold(child, blockChildToUnfoldChildrensCount);
    }



}

private void SetBlockChildChildrenLayerAfterUnfold(BlockScript blockChildToUnfold, int blockChildToUnfoldChildrensCount)
{
    if(blockChildToUnfold.GetBlockChildrenCount() == 0 || blockChildToUnfold.GetBlockChildren() == null)
    {
        return;
    }
    foreach (BlockScript child in blockChildToUnfold.GetBlockChildren())
    {
        child.Setlayer( blockChildToUnfoldChildrensCount  - child.Getlayer());
        SetBlockChildChildrenLayerAfterUnfold(child, blockChildToUnfoldChildrensCount);
    }
}

private void CheckWin(Action levelCompleted)
{
    foreach(BlockScript block in blocks)
    {
       WinningParams winningParams = block.GetComponent<WinningParams>();

       if(winningParams.GetWinningLayers().Count != 0)
       {
         List<int> winningLayers = winningParams.GetWinningLayers();
         if(!winningLayers.Contains(block.Getlayer()))
         {
             return;
         }
       }
     if (winningParams.GetWinningPositions().Count != 0) 
     {
         List<Position> winningPositions = winningParams.GetWinningPositions();
                if(!winningPositions.Exists(x => x.x == block.GetPosX() && x.z == block.GetPosZ()))
                {
                        return;
                }
             
     }

    }
    levelCompleted();
    this.enabled = false;
}

private void OnFullRewind(object sender, EventArgs e)
{
   if(isBusy)
    {
       return;
    }

if(blocksToRewind.Count == 0 || blocksToRewind == null)
    {
         return;
    }

StartCoroutine(IEOnFullRewind());

}

private IEnumerator IEOnFullRewind()
{
    int blocksToRewindCount = blocksToRewind.Count;
    isBusy = true;
    for(int i =0 ; i <   blocksToRewindCount ; i++)
    {
        FirstBlockToRewind(blocksToRewind[blocksToRewindCount-i-1].GetParent(),out BlockScript nextBlockToRewind,out Direction direction);
        
        if(nextBlockToRewind != null)
        {
            ReversBlock(nextBlockToRewind.GetParent(), direction, nextBlockToRewind,rotationDuration,0.0f);
            yield return new WaitForSeconds(rotationDuration);
        }
        else
        {
            isBusy = false ;
            break;
        }

    }

    isBusy = false;
}

private void ReversBlock  (BlockScript block, Direction direction, BlockScript blockChildToUnfold, float rotationDuration, float timeToWait)
{
    blockChildToUnfold.Unfold(direction,  () =>
    {
        
        blocksToRewind.RemoveAt(blocksToRewind.Count - 1);
        blockChildToUnfold.SetParent(null);
        blockChildToUnfold.transform.SetParent(GameManager.Instance.GetLevelTransform());
        block.RemoveBlockChild(blockChildToUnfold);
        SetLayersAfterUnfold(blockChildToUnfold , block   );
        blockChildToUnfold.SetColliderState(true);
    }, rotationDuration, timeToWait);
}

//
private void FirstBlockToRewind(BlockScript block, out BlockScript nextBlockToRewind, out Direction direction)
{
    nextBlockToRewind = null;
     direction = Direction.Right;
    List<BlockScript> blockChildren = block.GetBlockChildren();
    if(blockChildren.Count == 0 || blockChildren == null)
    {
        return ;
    }

nextBlockToRewind = blockChildren.Find(x => x.GetInitialPosition().x == block.GetPosX()+1 && blockChildren.IndexOf(x) == blockChildren.Count - 1);

if(nextBlockToRewind != null)
{
    direction = Direction.Right;
    return;


}
nextBlockToRewind = blockChildren.Find(x => x.GetInitialPosition().x == block.GetPosX()-1 && blockChildren.IndexOf(x) == blockChildren.Count - 1);

if(nextBlockToRewind != null)
{
    direction = Direction.Left;
    return;
}
nextBlockToRewind = blockChildren.Find(x => x.GetInitialPosition().z == block.GetPosZ()+1 && blockChildren.IndexOf(x) == blockChildren.Count - 1);

if(nextBlockToRewind != null)
{
    direction = Direction.Up;
    return;
}

nextBlockToRewind = blockChildren.Find(x => x.GetInitialPosition().z == block.GetPosZ()-1 && blockChildren.IndexOf(x) == blockChildren.Count - 1);


if(nextBlockToRewind != null)
{
    direction = Direction.Down;
    return;
}

}


}



 





