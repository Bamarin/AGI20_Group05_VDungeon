using UnityEngine;
using Mirror;
using System.Collections;

// A GameObject whose position is regulated by a Grid
public class Entity : NetworkBehaviour
{
    // *** PROPERTY FIELDS ***

    public bool hasCollision;
    public Vector2Int coordinates;
    public float verticalPosition;

    public Grid ParentGrid { get; private set; }

    // *** INTERNAL VARIABLES ***

    public Vector2Int BookmarkedCoordinates { get; protected set; }
    public Grid.CollisionFlags BookmarkedCollision { get; protected set; }

    // *** UTILITY FUNCTIONS ***

    // Moves the entity to a new set of grid coordinates.
    public virtual void Move(Vector2Int newCoordinates)
    {
        coordinates = newCoordinates;
        UpdatePosition();
    }

    // Updates the entity's local position to match its current grid coordinates.
    public virtual void UpdatePosition()
    {
        transform.localPosition = Grid.GridToLocal(coordinates, verticalPosition);
    }

    // Moves the entity to the nearest grid coordinates based on its current local position.
    public virtual void MoveToNearest()
    {
        Move(Grid.LocalToGrid(transform.localPosition));
    }

    // Ensures the entity is correctly positioned and oriented within the grid.
    public virtual void UpdateEntity()
    {
        UpdatePosition();
    }

    // Get the collision flags for this wall piece based on its type and orientation.
    public virtual Grid.CollisionFlags GetCollisionFlags()
    {
        if (hasCollision)
        {
            return Grid.CollisionFlags.Center;
        }

        return Grid.CollisionFlags.None;
    }

    // Bookmarks this Entity's current status for easy fallback at a later point.
    // Also relevant for collision data maintenance.
    public virtual void SetBookmark()
    {
        BookmarkedCoordinates = coordinates;
        BookmarkedCollision = GetCollisionFlags();
    }

    // Returns this Entity to a previously bookmarked status.
    public virtual void LoadBookmark()
    {
        Move(BookmarkedCoordinates);
    }

    // Removes the collision data from the bookmarked position.
    public void ClearCollisionAtBookmark()
    {
        ParentGrid.RemoveCollisionFlags(BookmarkedCoordinates, BookmarkedCollision);
    }

    // Updates collision data according to this Entity's current status.
    public void UpdateCollision()
    {
        ParentGrid.AddCollisionFlags(coordinates, GetCollisionFlags());
    }

    // *** GENERAL FUNCTIONS ***

    // Initializes this Entity to a Grid. Only call once.
    public void Initialize(Grid parentGrid)
    {
        if (ParentGrid == null)
        {
            ParentGrid = parentGrid;
            transform.SetParent(ParentGrid.transform, true);
        }
        else
        {
            Debug.LogWarning("Entity.Initialize() was called more than once on Entity: " + name);
        }
    }

    // *** MONOBEHAVIOUR FUNCTIONS ***

    // Start is called before the first frame update
    void Start()
    {
        SetBookmark();
    }

    // Update is called once per frame
    void Update()
    {
    }





    #region Start & Stop Callbacks

    /// <summary>
    /// This is invoked for NetworkBehaviour objects when they become active on the server.
    /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
    /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
    /// </summary>
    public override void OnStartServer() { }

    /// <summary>
    /// Invoked on the server when the object is unspawned
    /// <para>Useful for saving object data in persistant storage</para>
    /// </summary>
    public override void OnStopServer() { }

    /// <summary>
    /// Called on every NetworkBehaviour when it is activated on a client.
    /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
    /// </summary>
    public override void OnStartClient() { }

    /// <summary>
    /// This is invoked on clients when the server has caused this object to be destroyed.
    /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
    /// </summary>
    public override void OnStopClient() { }

    /// <summary>
    /// Called when the local player object has been set up.
    /// <para>This happens after OnStartClient(), as it is triggered by an ownership message from the server. This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStartLocalPlayer() { }

    /// <summary>
    /// This is invoked on behaviours that have authority, based on context and <see cref="NetworkIdentity.hasAuthority">NetworkIdentity.hasAuthority</see>.
    /// <para>This is called after <see cref="OnStartServer">OnStartServer</see> and before <see cref="OnStartClient">OnStartClient.</see></para>
    /// <para>When <see cref="NetworkIdentity.AssignClientAuthority"/> is called on the server, this will be called on the client that owns the object. When an object is spawned with <see cref="NetworkServer.Spawn">NetworkServer.Spawn</see> with a NetworkConnection parameter included, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStartAuthority() { }

    /// <summary>
    /// This is invoked on behaviours when authority is removed.
    /// <para>When NetworkIdentity.RemoveClientAuthority is called on the server, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStopAuthority() { }

    #endregion
}
