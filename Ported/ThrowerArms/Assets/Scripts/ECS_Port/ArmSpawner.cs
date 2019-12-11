﻿using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ArmSpawner : MonoBehaviour
{
    public GameObject ArmPrefab;
    public int Count = 100;
    
    private static (bool IsCalculated, int Value) _spawnCount;

    public static (bool IsCalculated, int Value) SpawnCount
    {
        get
        {
            if (!_spawnCount.IsCalculated)
            {
                _spawnCount = (IsCalculated: true, _spawnCount: Count)
            }
        }
        private set => _spawnCount = value;
    }

    public static float ArmRowWidth{ get; private set;}
    
    private const float Spacing = 1;
    
    private void Awake()
    {
        ArmRowWidth = (Count - 1) * Spacing;
        SpawnCount = Count;
    }

    private void Start()
    {
        GameObjectConversionSettings settings = 
            GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore: null);
        
        Entity prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(ArmPrefab, settings);
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        for (var i = 0; i < Count; i++)
        {
            Entity instance = entityManager.Instantiate(prefab);
            
            entityManager.AddComponentData(instance, new ArmComponent());
            entityManager.AddComponentData(instance, new Finger());
            
            entityManager.AddComponentData(instance, new IdleState());
            entityManager.AddComponentData(instance, new FindGrabbableTargetState());
            
            entityManager.SetComponentData(instance, new Translation
            {
                Value = Spacing * i * new float3(1, 0, 0)
            });
            entityManager.SetComponentData(instance, new Rotation
            {
                Value = quaternion.identity
            });
        }
    }
}