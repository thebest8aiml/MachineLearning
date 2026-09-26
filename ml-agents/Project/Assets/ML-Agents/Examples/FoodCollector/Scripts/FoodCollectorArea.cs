using UnityEngine;
using Unity.MLAgentsExamples;
using System.Collections.Generic;

public class FoodCollectorArea : Area
{
    public GameObject food;
    public GameObject badFood;
    public int numFood;
    public int numBadFood;
    public bool respawnFood;
    public float range;
    public List<FoodCollectorAgent> agents;
    public List<GameObject> foods;
    public float timer;

    public void Start()
    {
        Time.timeScale = 1.0f;
    }
    public void Update()
    {
        timer += Time.deltaTime;
    }
    void CreateFood(int num, GameObject type)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject f = Instantiate(type, new Vector3(Random.Range(-range, range), 1f,
                Random.Range(-range, range)) + transform.position,
                Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));
            f.GetComponent<FoodLogic>().respawn = respawnFood;
            f.GetComponent<FoodLogic>().myArea = this;
            foods.Add(f);
        }
    }

    public void ResetFoodArea(GameObject[] agents)
    {
        foreach (GameObject agent in agents)
        {
            if (agent.transform.parent == gameObject.transform)
            {
                agent.transform.position = new Vector3(Random.Range(-range, range), 2f,
                    Random.Range(-range, range))
                    + transform.position;
                agent.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
            }
        }
        foreach (GameObject food in foods)
        {
            Destroy(food);
        }
        foods.Clear();
        CreateFood(numFood, food);
        CreateFood(numBadFood, badFood);
    }
    public void CheckAllDead()
    {
        foreach (FoodCollectorAgent agent in agents)
        {
            if (!agent.dead) return;
        }
        foreach (FoodCollectorAgent agent in agents)
        {
            agent.EndEpisode();
        }

        GameObject[] agentObjects = new GameObject[agents.Count];
        for (int i = 0; i < agents.Count; i++) agentObjects[i] = agents[i].gameObject;
        ResetFoodArea(agentObjects); // respawns food + repositions agents
    }
    public override void ResetArea()
    {
    }
}
