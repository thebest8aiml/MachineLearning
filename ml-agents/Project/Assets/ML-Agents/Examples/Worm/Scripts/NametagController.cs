using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
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
            bool uniqueNames = firstNames.Length * lastNames.Length >= nametags.Length; // fallback if there are not enough unique names to go around
            HashSet<(int fName, int lName)> usedNames = new();

            foreach (Nametag nametag in nametags)
            {
                if (nametag.textTag is null) continue;

                int fNameId, lNameId;

                do
                {
                    fNameId = Random.Range(0, firstNames.Length);
                    lNameId = Random.Range(0, lastNames.Length);
                }
                while (usedNames.Contains((fNameId, lNameId)) && uniqueNames);

                usedNames.Add((fNameId, lNameId));

                string firstName = firstNames[fNameId];
                string lastName = lastNames[lNameId];
                nametag.textTag.text = $"{firstName} {lastName}";
            }
        }
    }

    [CanBeNull] public Nametag NametagOf([CanBeNull] Transform followee)
    {
        return followee is null ? null : followeeNametags.GetValueOrDefault(followee);
    }

    public void OnMainTargetChanged(Transform newTarget, int idx)
    {
        if (currentWorm == null) return;
        var wName = NametagOf(newTarget)?.Name() ?? "Unknown";
        currentWorm.text = $"Worm #{idx}: {wName}";
    }
}
