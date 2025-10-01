using UnityEngine;

public class Detector : MonoBehaviour
{

    private Boss_Ia IA;

    public GameObject GB;


    void Awake()
    {
      
        IA = GB.GetComponent<Boss_Ia>();
        IA.enabled = false;


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
     void AtivarScriptA()
    {
        IA.enabled = true;
        
    }

     void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Area"))
        {
            AtivarScriptA();
    
            Debug.Log("COLISOR");
        }
        
    }
}
