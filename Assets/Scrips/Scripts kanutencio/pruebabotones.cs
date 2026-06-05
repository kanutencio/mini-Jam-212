using UnityEngine;

public class pruebabotones : MonoBehaviour
{
     public TopButtonPanel p;
     public GameObject a;
     public GameObject b;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void puchaleplay(){
        p.AddButton(a,b);
    }
}
