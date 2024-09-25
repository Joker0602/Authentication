using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace URAuth.Helpers
{

    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response,string message){
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
        
    }
    // public static DateTime IndianTime(this DateTime value){
    //         TimeZoneInfo IndianZone = TZConvert.GetTimeZoneInfo("Inidan standard time");
    //     }
    
}