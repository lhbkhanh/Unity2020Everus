using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{

    private Gameplay m_GP;
    //[SerializeField] 
    private Image m_fullImage, m_generationImage;
    [SerializeField] private bool isAttacker;

    private const int MAX_POINT = 6;
    private float m_energyAmount, m_energyRegenAmount;
    private int m_points;


    private void Awake()
    {
        m_energyAmount = 0.0f;
        m_energyAmount = 6.0f; // debug-Cheat
        if(isAttacker)
            m_energyRegenAmount = AttDef.EnergyRegen;
        else
            m_energyRegenAmount = EneDef.EnergyRegen;

        m_generationImage = transform.Find("Generation").gameObject.GetComponent<Image>();
        m_generationImage.fillAmount = 0.0f;
        
        m_fullImage = transform.Find("Full").gameObject.GetComponent<Image>();
        m_fullImage.fillAmount = 0.0f;
    }
    // Start is called before the first frame update
    void Start()
    {
        m_GP = GameObject.Find("Gameplay").GetComponent<Gameplay>();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_GP.PauseGame) return;

        if(m_points < MAX_POINT)
            m_energyAmount += m_energyRegenAmount * Time.deltaTime;

        float GetEnergy = GetEnergyNormalized();

        m_points = (int)m_energyAmount;
        //Debug.Log(string.Format("energyAmount: {0}, GetEnergy: {1}, points: {2}", m_energyAmount, GetEnergy, m_points));
        m_generationImage.fillAmount = GetEnergy;    
        m_fullImage.fillAmount = ((float)m_points / MAX_POINT) ;    

    }
    
    private float GetEnergyNormalized()
    {
        return m_energyAmount / MAX_POINT;
    }
    private int GetPointEnergyNormalized()
    {
        return (int)(m_energyAmount / MAX_POINT);
    }
    public bool SpawnPlayer(int point)
    {
        if(m_energyAmount >= point)
        {
            m_energyAmount -= point;
            m_points -= point;
            return true; 
        }
        return false;
    }
}
