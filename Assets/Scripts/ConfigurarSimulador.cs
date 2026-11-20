
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

public class ConfigurarSimulador : MonoBehaviour
{
    private bool configurado = false;

    void Update()
    {
        if (configurado) return;

        XRInteractionSimulator simulador =
            FindFirstObjectByType<XRInteractionSimulator>();

        if (simulador != null)
        {
            simulador.translateXSpeed = 0.8f;
            simulador.translateZSpeed = 0.8f;
            simulador.translateYSpeed = 0.8f;

            configurado = true;

            Debug.Log("Velocidade do simulador configurada!");
        }
    }
}
