using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.MLAgentsExamples;
using UnityEngine;

public class NametagController : MonoBehaviour
{
    public Nametag[] nametags;

    Dictionary<Transform, Nametag> followeeNametags = new();

    [CanBeNull] public TextMeshProUGUI currentWorm;

    public bool randomizeNames = true;

    public string[] firstNames = { "Wiggly", "Bobbie", "Sally", "Wormy", "John", "Douglas", "Erica", "Kelly", "Nathan", "Alex", "Alexandra", "Emily", "Mihai", "Minh", "Ilya", "Vlad", "Nikos", "Robbie", "Bob", "Joshua", "Mick", "Hailey", "Pete", "Donald", "Scott", "Elizabeth" };
    public string[] lastNames = { "McWormface", "Wormington", "Wormsworth", "the Wormskey", "the True Worm", "Wormer", "Interwormer", "Wiggleston" };

    void Start()
    {
        if (nametags is null || nametags.Length == 0)
        {
            nametags = FindObjectsOfType<Nametag>();
        }

        foreach (Nametag nametag in nametags)
        {
            if (nametag.follow is not null)
            {
                followeeNametags[nametag.follow] = nametag;
            }
        }

        if (randomizeNames)
        {
            foreach (Nametag nametag in nametags)
            {
                if (nametag.textTag is null) continue;

                string firstName = firstNames[Random.Range(0, firstNames.Length)];
                string lastName = lastNames[Random.Range(0, lastNames.Length)];
                nametag.textTag.text = $"{firstName} {lastName}";
            }
        }
    }

    [CanBeNull] public Nametag NametagOf([CanBeNull] Transform followee)
    {
        if (followee is null)
        {
            return null;
        }

        if (followeeNametags.TryGetValue(followee, out Nametag nametag))
        {
            return nametag;
        }

        return null;
    }

    public void OnMainTargetChanged(Transform newTarget, int idx)
    {
        if (currentWorm != null)
        {
            var wName = NametagOf(newTarget)?.Name() ?? "Unknown";
            currentWorm.text = $"Worm #{idx}: {wName}";
        }
    }
}
