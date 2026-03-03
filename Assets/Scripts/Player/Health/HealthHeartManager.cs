using UnityEngine;
using System.Collections.Generic;


public class HealthHeartManager : MonoBehaviour
{
    public GameObject HeartPrefab;
    public PlayerHealth playerHealth;
    List<HealthHeart> hearts = new List<HealthHeart>();

    private void OnEnable()
    {
        playerHealth.OnHealthChanged += DrawHearts;
    }

    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= DrawHearts;
    }

    public void Start(){
        DrawHearts();
    }

    public void DrawHearts()
    {
        ClearHearts();
        int HeartsToDraw = playerHealth.MaxHealth;
        for (int i = 0; i < HeartsToDraw; i++)
        {   
            CreateHearts();
        }
        for(int i = 0; i < hearts.Count; i++)
        {
            int heartStatusRemainder = Mathf.Clamp(playerHealth.Health - (i*1), 0, 1);
            hearts[i].SetHeartState((HealthHeart.HeartState)heartStatusRemainder);
        }
    }
    public void ClearHearts()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        hearts = new List<HealthHeart>();
    }

    public void CreateHearts()
    {
        GameObject NewHeart = Instantiate(HeartPrefab);
        NewHeart.transform.SetParent(transform);
        
        HealthHeart HeartComponent = NewHeart.GetComponent<HealthHeart>();
        HeartComponent.SetHeartState(HealthHeart.HeartState.Full);
        hearts.Add(HeartComponent);
    }
}
