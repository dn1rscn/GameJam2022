using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSystem : MonoBehaviour
{
    public ControlPlayer player;
    //public float totalPorcentaje = 100;
    [System.Serializable]
    public class DropCurrency
    {
        public string name;
        //public GameObject item;
        public int municion;
        [Range(0,100)]
        public int dropRarity;
    }

    public List<DropCurrency> LootTable = new List<DropCurrency>();
    [Range(0, 100)]
    int dropChance=100;

    public void calculateLoot()
    {
        int calc_dropChance = Random.Range(0, 101);

        if(calc_dropChance > dropChance)
        {
            Debug.Log("No Loot");
            return;
        }

        if(calc_dropChance<=dropChance)
        {
            int itemWeight = 0;

            for (int i = 0; i<LootTable.Count;i++)
            {
                itemWeight += LootTable[i].dropRarity;
            }
            //Debug.Log("ItemWeight = " + itemWeight);

            int randomValue = Random.Range(0, itemWeight);

            for(int j=0;j<LootTable.Count;j++)
            {
                if(randomValue <= LootTable[j].dropRarity)
                {
                    //Instantiate(LootTable[j].item, transform.position, Quaternion.identity);
                    print(LootTable[j].name);
                    player.municion = LootTable[j].municion;
                    player.armaSeleccionada = j;
                    GameObject.Find("ControlHub").GetComponent<ControlHub>().ActualizarHub(player.municion,player.armaSeleccionada);
                    player.uiHubMunicion.SetActive(true);
                    return;
                }
                randomValue -= LootTable[j].dropRarity;
                //Debug.Log("Random Value decreased" + randomValue);
            }
        }
    }
}
