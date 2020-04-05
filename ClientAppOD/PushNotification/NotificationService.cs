using System;
using System.Collections.Generic;
using ClientAppOD.CustomModels;
using Com.OneSignal;
using Com.OneSignal.Abstractions;

namespace ClientAppOD.PushNotification
{
    public class NotificationService
    {
       
        public static void CreateExternalUserId(string UserId)
        {
            OneSignal.Current.SetExternalUserId(UserId);
        }
        
    }
}
