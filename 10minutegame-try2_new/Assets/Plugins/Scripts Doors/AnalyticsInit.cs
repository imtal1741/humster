using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Collections.Generic;

public class AnalyticsInit : MonoBehaviour
{

    public string applicationName;
    bool errorInit;

    async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            List<string> consentIdentifiers = await AnalyticsService.Instance.CheckForRequiredConsents();
        }
        catch (ConsentCheckException e)
        {
            // Something went wrong when checking the GeoIP, check the e.Reason and handle appropriately.

            errorInit = true;
        }
    }



    public void SendAnalyticsEvent(string Name, Dictionary<string, object> Dict)
    {
        if (errorInit)
            return;

        AnalyticsService.Instance.CustomData(applicationName + "_" + Name, Dict);
        AnalyticsService.Instance.Flush();
    }

}