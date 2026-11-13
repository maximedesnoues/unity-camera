using System.Collections.Generic;
using UnityEngine;

public class ViewVolumeBlender : MonoBehaviour
{
    public static ViewVolumeBlender Instance { get; private set; }

    private readonly List<AViewVolume> activeViewVolumes = new List<AViewVolume>();
    private readonly Dictionary<AView, List<AViewVolume>> volumesPerView = new Dictionary<AView, List<AViewVolume>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        UpdateBlend();
    }

    public void AddVolume(AViewVolume volume)
    {
        if (volume == null || volume.view == null || activeViewVolumes.Contains(volume))
        {
            return;
        }

        activeViewVolumes.Add(volume);

        if (!volumesPerView.TryGetValue(volume.view, out List<AViewVolume> volumes))
        {
            volumes = new List<AViewVolume>();
            volumesPerView.Add(volume.view, volumes);
        }

        volumes.Add(volume);

        if (volumes.Count == 1)
        {
            UpdateBlend();
            volume.view.SetActive(true);
        }
    }

    public void RemoveVolume(AViewVolume volume)
    {
        if (volume == null || volume.view == null || !activeViewVolumes.Contains(volume))
        {
            return;
        }

        if (!volumesPerView.TryGetValue(volume.view, out List<AViewVolume> volumes))
        {
            return;
        }

        activeViewVolumes.Remove(volume);
        volumes.Remove(volume);

        if (volumes.Count == 0)
        {
            volumesPerView.Remove(volume.view);

            UpdateBlend();
            volume.view.SetActive(false);
        }
    }

    public void UpdateBlend()
    {
        foreach (AView view in volumesPerView.Keys)
        {
            view.weight = 0f;
        }

        activeViewVolumes.Sort(CompareVolumes);

        foreach (AViewVolume volume in activeViewVolumes)
        {
            float weight = Mathf.Clamp01(volume.ComputeSelfWeight());
            float remainingWeight = 1f - weight;

            foreach (AView view in volumesPerView.Keys)
            {
                view.weight *= remainingWeight;
            }

            volume.view.weight += weight;
        }
    }

    private int CompareVolumes(AViewVolume firstVolume, AViewVolume secondVolume)
    {
        int priorityComparison = firstVolume.priority.CompareTo(secondVolume.priority);

        if (priorityComparison != 0)
        {
            return priorityComparison;
        }

        return firstVolume.Uid.CompareTo(secondVolume.Uid);
    }

    private void OnGUI()
    {
        GUILayout.Label("Active View Volumes:");

        foreach (AViewVolume volume in activeViewVolumes)
        {
            GUILayout.Label($"{volume.name} | Priority: {volume.priority} | UID: {volume.Uid}");
        }
    }
}