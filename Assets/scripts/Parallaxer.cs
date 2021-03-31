using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Parallaxer : MonoBehaviour
{
    class PoolObject
    {
        public Transform transform;
        public bool inUse;

        public PoolObject(Transform transform)
        {
            this.transform = transform;
        }

        public void Use()
        {
            inUse = true;
        }

        public void Dispose()
        {
            inUse = false;
        }
    }

    public GameObject Prefab;
    public int poolSize;
    public float shiftSpeed;
    public float spawnRate;

    [System.Serializable]
    public struct YSpawnRange
    {
        public float min;
        public float max;
    }

    public YSpawnRange ySpawnRange;
    public Vector3 defaultSpawnPosition;
    public bool spawnImmediate;
    public Vector3 immediateSpawnPosition;
    public Vector2 targetAspectRatio;

    float spawnTimer;
    float targetAspect;
    PoolObject[] poolObjects;
    CanvasManager game;

    private void Awake()
    {
        Configure();
    }

    private void Start()
    {
        game = CanvasManager.Instance;
    }

    private void OnEnable()
    {
        CanvasManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    private void OnDisable()
    {
        CanvasManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    private void OnGameOverConfirmed()
    {
        foreach(var obj in poolObjects)
        {
            obj.Dispose();
            obj.transform.position = Vector3.one * 1000;
        }

        if (spawnImmediate)
            SpawnImmediate();
    }

    private void Update()
    {
        if (game.GameOver) return;

        Shift();
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnRate)
        {
            Spawn();
            spawnTimer = 0;
        }
    }

    void Configure()
    {
        targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        poolObjects = Create<PoolObject>(poolSize,
            () =>
            {
                GameObject go = Instantiate(Prefab);
                Transform t = go.transform;
                t.SetParent(transform);
                t.position = Vector2.one * 1000;
                return new PoolObject(t);
            }).ToArray();

        if (spawnImmediate)
            SpawnImmediate();
    }

    IEnumerable<T> Create<T>(int count, Func<T> creator)
    {
        for (int i = 0; i < count; i++)
        {
            yield return creator();
        }
    }

    void Spawn()
    {
        Transform t = GetPoolObject();
        if (t == null) return;
        Vector3 position = Vector3.zero;
        position = defaultSpawnPosition * Camera.main.aspect / targetAspect;
        position.y = UnityEngine.Random.Range(ySpawnRange.min, ySpawnRange.max);
        t.position = position;
    }
    
    void SpawnImmediate()
    {
        Transform t = GetPoolObject();
        if (t == null) return;
        Vector3 position = Vector3.zero;
        position = immediateSpawnPosition * Camera.main.aspect / targetAspect;
        position.y = UnityEngine.Random.Range(ySpawnRange.min, ySpawnRange.max);
        t.position = position;
        Spawn();
    }

    void Shift()
    {
        foreach(var obj in poolObjects)
        {
            obj.transform.position += -Vector3.right * shiftSpeed * Time.deltaTime;
            CheckDisposeObject(obj);
        }
    }

    void CheckDisposeObject(PoolObject poolObject)
    {
         if (poolObject.transform.position.x < -defaultSpawnPosition.x * Camera.main.aspect / targetAspect)
        {
            poolObject.Dispose();
            poolObject.transform.position = Vector3.one * 1000;
        }
    }

    Transform GetPoolObject()
    {
        foreach(var obj in poolObjects)
        {
            if (!obj.inUse)
            {
                obj.Use();
                return obj.transform;
            }
        }

        return null;
    }
}