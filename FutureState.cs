using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataFeedRowing
    
{
    class FutureState
    {
        public string SessionHeaderId = string.Empty;
        private string directoryOutput = string.Empty;
        private float Windspeed = 0;
        private float Windgust = 0;
        private float ChanceOfPpt = 0;

        public FutureState(string SessionHeaderIdOverall, string directoryOutputOverall)
        {

            SessionHeaderId = SessionHeaderIdOverall;
            directoryOutput = directoryOutputOverall;
        }

        public void ReturnData()
        {
            using (WebClient wcSH = new WebClient())
            {
                wcSH.Headers["SessionHeaderId"] = SessionHeaderId;
                string result =
                wcSH.DownloadString("http://riverlevelsapi.shoothill.com/ThreeDayFloodForecast/GetCurrentForecastByRegions?regions=Midlands&region=Northwest");
                System.IO.File.AppendAllText(directoryOutput, " Three Day Forecast :" + result + "\n");
                dynamic OBHIds = JsonConvert.DeserializeObject<dynamic>(result);

                // Sample MO web call 
                using (WebClient wcMO = new WebClient())
                {
                    wcMO.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string uri = "http://datapoint.metoffice.gov.uk/public/data/val/wxfcs/all/json/3265?res=3hourly&key=c8b020fb-bc34-49cd-a30f-683dbd5c2d20";
                    string fileName = "3265.json";
                    wcMO.DownloadFile(uri, fileName);
                    String resultJSON = wcMO.DownloadString(uri);
                    String uri2 = "http://datapoint.metoffice.gov.uk/public/data/val/wxfcs/all/xml/3265?res=3hourly&key=c8b020fb-bc34-49cd-a30f-683dbd5c2d20";
                    String resultMO = wcMO.DownloadString(uri2);
                    System.IO.File.AppendAllText(directoryOutput, " Forecast at Topcliffe XML: " + resultMO + "\n");
                    dynamic TopCliffeForecast = JsonConvert.DeserializeObject<dynamic>(resultJSON);
                    System.IO.File.AppendAllText(directoryOutput, "\n" + " Forecast at Topcliffe JSON: " + "\n" +  "Full forecast: " + TopCliffeForecast.SiteRep.DV.Location + "\n");
                    System.IO.File.AppendAllText(directoryOutput, "\n" + " Forecast at Topcliffe JSON: " + "\n" + " Windspeed: " + TopCliffeForecast.SiteRep.DV.Location.Period[0].Rep[0].S + "\n" + "Wind direction: " + TopCliffeForecast.SiteRep.DV.Location.Period[0].Rep[0].D);

                }

            }
        }
        public float ReturnWindspeed()
        {
            using (WebClient wcMO = new WebClient())
            {
                wcMO.Headers[HttpRequestHeader.ContentType] = "application/json";
                string uri = "http://datapoint.metoffice.gov.uk/public/data/val/wxfcs/all/json/3265?res=3hourly&key=c8b020fb-bc34-49cd-a30f-683dbd5c2d20";
                String resultJSON = wcMO.DownloadString(uri);
                dynamic TopCliffeForecast = JsonConvert.DeserializeObject<dynamic>(resultJSON);
                Windspeed = TopCliffeForecast.SiteRep.DV.Location.Period[0].Rep[0].S;
                System.IO.File.AppendAllText(directoryOutput, "\n" + " Forecast at Topcliffe JSON:-  Windspeed is: " + Windspeed);
                return Windspeed;
            }
        }
        public float ReturnWindgust()
        {
            using (WebClient wcMO = new WebClient())
            {
                wcMO.Headers[HttpRequestHeader.ContentType] = "application/json";
                string uri = "http://datapoint.metoffice.gov.uk/public/data/val/wxfcs/all/json/3265?res=3hourly&key=c8b020fb-bc34-49cd-a30f-683dbd5c2d20";
                String resultJSON = wcMO.DownloadString(uri);
                dynamic TopCliffeForecast = JsonConvert.DeserializeObject<dynamic>(resultJSON);
                Windgust = TopCliffeForecast.SiteRep.DV.Location.Period[0].Rep[0].G;
                System.IO.File.AppendAllText(directoryOutput, "\n" + " Forecast at Topcliffe JSON:-  Windgust is: " + Windgust);
                return Windgust;
            }
        }


        public float ReturnPrecipitationLocal()
        {
            using (WebClient wcMO = new WebClient())
            {
                wcMO.Headers[HttpRequestHeader.ContentType] = "application/json";
                string uri = "http://datapoint.metoffice.gov.uk/public/data/val/wxfcs/all/json/3265?res=3hourly&key=c8b020fb-bc34-49cd-a30f-683dbd5c2d20";
                String resultJSON = wcMO.DownloadString(uri);
                dynamic TopCliffeForecast = JsonConvert.DeserializeObject<dynamic>(resultJSON);
                ChanceOfPpt = TopCliffeForecast.SiteRep.DV.Location.Period[0].Rep[0].Pp;
                System.IO.File.AppendAllText(directoryOutput, "\n" + " Forecast at Topcliffe JSON:-  Chance of rain is: " + ChanceOfPpt);
                return ChanceOfPpt;
            }
        }


    }
}
