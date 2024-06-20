using Il2CppWorld;
using Il2CppWorld.SceneObject;
using MelonLoader;
using UnityEngine;
using UGAR_Mod;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(MyMod), "My Mod Name", "0.1", "Savantic")]
[assembly: MelonGame("Game Labs", "Ultimate General: American Revolution")]

namespace UGAR_Mod;

public class MyMod : MelonMod
{
    private static DepartmentManager? _deptManager;
    private static ZoneManager? _zoneManager;

    private static KeyCode _researchKey;
    private static KeyCode _buildKey;
    private static KeyCode _initKey;
    
    private static bool _isInitialized;

    public override void OnEarlyInitializeMelon()
    {
        _researchKey = KeyCode.R;
        _buildKey = KeyCode.B;
        _initKey = KeyCode.I;
        _isInitialized = false;
        
    }

    public override void OnLateUpdate()
    {
        if (Input.GetKeyDown(_researchKey))
        {
            Research();
        }
        if (Input.GetKeyDown(_buildKey))
        {
            Build();
        }
        if (Input.GetKeyDown(_initKey))
        {
            Init();
        }
    }

    private static void Build()
    {
        if (!_isInitialized) return;
        if (_zoneManager?.conditionZones == null) return;
        foreach (var zone in _zoneManager.conditionZones)
        {
            ZoneBuilder(zone);
        }
    }

    private static void ZoneBuilder(ConditionZone zone)
    {
        foreach (var region in zone.RegionArray)
        {
            if (!region.IsPlayerNation || region.GetOwner().ToString() != "USA" || !region.isActiveAndEnabled) continue;
            RegionBuilder(region);
        }

        foreach (var locality in zone.LocalityArray)
        {
            if (!locality.IsPlayerNation || locality.ownerNation.ToString() != "USA" || !locality.Active) continue;
            LocalityBuilder(locality);
        }
    }

    private static void LocalityBuilder(RegionLocality locality)
    {
        Melon<MyMod>.Logger.Msg($"locality: {locality.Name}");
        foreach (var construction in locality.constructionSlots)
        {
            if (construction == null) continue;
            InstaBuildConstruction(construction);
        }
    }

    private static void RegionBuilder(Region region)
    {
        Melon<MyMod>.Logger.Msg($"Region: {region.Name}");
        foreach (var construction in region.constructionQueue)
        {
            if (construction == null) continue;
            InstaBuildConstruction(construction);
        }
    }
    
    private static void InstaBuildConstruction(Construction construction)
    {
        if (construction is { constructed: true, upgrade: null } and not {damage: true}) return;
        Melon<MyMod>.Logger.Msg($"{construction.settings.buildingType.ToString()}");
        construction.constructionProgress = 9999;

        if (construction.upgrade == null) return;
        Melon<MyMod>.Logger.Msg($"{construction.upgrade.settings.buildingType.ToString()}");
        construction.upgrade.constructionProgress = 9999;
    }

    private static void Research()
    {
        if (!_isInitialized) return;
        if (_deptManager?.departmentArray == null) return;
        foreach (var department in _deptManager.departmentArray)
        {
            if (department.assignedProject == null) continue;
            department.assignedProject.progress = 1;
        }
    }
    
    private static void Init()
    {
        _deptManager = Object.FindObjectOfType<DepartmentManager>();
        _zoneManager = Object.FindObjectOfType<ZoneManager>();
        _isInitialized = true;
    }
}