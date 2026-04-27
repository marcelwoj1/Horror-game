using UnityEngine;
using System.Collections.Generic;


public class HealthHeartManager : MonoBehaviour
{
    public GameObject HeartPrefab;
    public PlayerHealth playerHealth;
    List<HealthHeart> hearts = new List<HealthHeart>();

    //Draws hearts whenever health changes
    private void OnEnable()
    {
        playerHealth.OnHealthChanged += DrawHearts;
    }

    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= DrawHearts;
    }

    //Draws hearts when player starts
    public void Start(){
        DrawHearts();
    }

    public void DrawHearts()
    {
        //Clears all hearts
        ClearHearts();
        
        //Draws hearts
        int HeartsToDraw = playerHealth.MaxHealth;
        for (int i = 0; i < HeartsToDraw; i++)
        {
            CreateHearts();
        }
        //Updates the hearts to match the players health
        for(int i = 0; i < hearts.Count; i++)
        {
            //Checks if heart should be full or empty
            int heartStatusRemainder = Mathf.Clamp(playerHealth.Health - (i*1), 0, 1);
            hearts[i].SetHeartState((HealthHeart.HeartState)heartStatusRemainder);
        }
    }

    //Clears all hearts
    public void ClearHearts()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        hearts = new List<HealthHeart>();
    }

    //Creates a new heart
    public void CreateHearts()
    {
        GameObject NewHeart = Instantiate(HeartPrefab);
        NewHeart.transform.SetParent(transform);
        //Gets the heart component
        HealthHeart HeartComponent = NewHeart.GetComponent<HealthHeart>();
        //Adds the heart to the list
        HeartComponent.SetHeartState(HealthHeart.HeartState.Full);
        hearts.Add(HeartComponent);
    }
}
