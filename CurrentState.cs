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
    public class CurrentState
    {
        public string SessionHeaderId = string.Empty;
        private string directoryOutput = string.Empty;
        private float Windspeed = 0;
        private float Windgust = 0;
        private float Riverstage = 0;
        public CurrentState(string SessionHeaderIdOverall, string directroyOutputOverall)
        {
            SessionHeaderId = SessionHeaderIdOverall;
            directoryOutput = directroyOutputOverall;
            using (WebClient wcSH = new WebClient())
            {
                wcSH.Headers["SessionHeaderId"] = SessionHeaderId;
                string result =
                wcSH.DownloadString("http://riverlevelsapi.shoothill.com/TimeSeries/GetTimeSeriesStations?dataTypes=3");
                //Each TimeSeriesStation object in the result contains a list of its gauges.
                dynamic RiverGaugeIds = JsonConvert.DeserializeObject<dynamic>(result);

                System.IO.File.WriteAllText(directoryOutput, " Number of River Gauges is: " + Convert.ToString(RiverGaugeIds.Count) + "\n");
                
            }
        }

        public void ReturnData()
        {
            using (WebClient wcSH = new WebClient())
            {
                wcSH.Headers["SessionHeaderId"] = SessionHeaderId;

                string GaugeData =
                    wcSH.DownloadString("http://riverlevelsapi.shoothill.com/TimeSeries/GetTimeSeriesRecentDatapoints/?stationId=2059&datatype=3&numberDays=5");
                dynamic RiverLevelsFossBarrier = JsonConvert.DeserializeObject<dynamic>(GaugeData);
                System.IO.File.AppendAllText(directoryOutput, Convert.ToString(RiverLevelsFossBarrier) + "\n");
                System.IO.File.AppendAllText(directoryOutput, " River gauge id:" + Convert.ToString(RiverLevelsFossBarrier.gauge.id) + "\n");
                System.IO.File.AppendAllText(directoryOutput, " Q95:" + Convert.ToString(RiverLevelsFossBarrier.gauge.additionalDataObject.percentile95) + "\n");
                System.IO.File.AppendAllText(directoryOutput, " Q5:" + Convert.ToString(RiverLevelsFossBarrier.gauge.additionalDataObject.percentile5) + "\n");
                System.IO.File.AppendAllText(directoryOutput, " No. values " + Convert.ToString(RiverLevelsFossBarrier.values.Count) + "\n");

                for (int i = 0; i < RiverLevelsFossBarrier.values.Count; i++)
                {
                    System.IO.File.AppendAllText(directoryOutput, Convert.ToString(RiverLevelsFossBarrier.values[i].time) + "," + Convert.ToString(RiverLevelsFossBarrier.values[i].value) + "\n");
                }

                GaugeData =
                    wcSH.DownloadString("http://riverlevelsapi.shoothill.com/TimeSeries/GetTimeSeriesRecentDatapoints/?stationId=2060&datatype=3&numberDays=5");
                dynamic RiverLevelsViking = JsonConvert.DeserializeObject<dynamic>(GaugeData);
                System.IO.File.AppendAllText(directoryOutput, Convert.ToString(RiverLevelsViking) + "\n");
                // Sample MO web call 
                using (WebClient wcMO = new WebClient())
                {
                    wcMO.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string uri = "http://datapoint.metoffice.gov.uk/public/data/val/wxobs/all/json/3265?res=hourly&key=c8b020fb-bc34-49cd-a30f-683dbd5c2d20";
                    string fileName = "3265.json";
                    wcMO.DownloadFile(uri, fileName);
                    String resultMOJson = wcMO.DownloadString(uri);
                    String uri2 = "http://datapoint.metoffice.gov.uk/public/data/val/wxobs/all/xml/3265?res=hourly&key=c8b020fb-bc34-49cd-a30f-683dbd5c2d20";
                    String resultMOxml = wcMO.DownloadString(uri2);
                    System.IO.File.AppendAllText(directoryOutput, "Observed at Topcliffe:" + resultMOxml + "\n");
                    dynamic TopCliffeObservations = JsonConvert.DeserializeObject<dynamic>(resultMOJson);
                    System.IO.File.AppendAllText(directoryOutput, "Observed at Topcliffe JSON:" + Convert.ToString(TopCliffeObservations.SiteRep.DV.Location.Period) + "\n");
                    System.IO.File.AppendAllText(directoryOutput, "Last observed at Topcliffe JSON: Period: " + Convert.ToString(TopCliffeObservations.SiteRep.DV.Location.Period.Count) + " Day: " + Convert.ToString(TopCliffeObservations.SiteRep.DV.Location.Period[1].Rep.Count) + "\n");

                }

            }
        }

        public float ReturnWindspeed()
        {
            using (WebClient wcMO = new WebClient())
            {
                wcMO.Headers[HttpRequestHeader.ContentType] = "application/json";
                string uri = "http://datapoint.metoffice.gov.uk/public/data/val/wxobs/all/json/3265?res=hourly&key=c8b020fb-bc34-49cd-a30f-683dbd5c2d20";
                String resultJSON = wcMO.DownloadString(uri);
                dynamic TopCliffeObserved = JsonConvert.DeserializeObject<dynamic>(resultJSON);
                int valueDay = TopCliffeObserved.SiteRep.DV.Location.Period.Count;
                int valueHour = TopCliffeObserved.SiteRep.DV.Location.Period[valueDay-1].Rep.Count;
                Windspeed = TopCliffeObserved.SiteRep.DV.Location.Period[valueDay-1].Rep[valueHour-1].S;
                System.IO.File.AppendAllText(directoryOutput, "\n" + " Observation at Topcliffe JSON:-  Windspeed is: " + Windspeed +"\n");
                return Windspeed;
            }
        }
        public float ReturnWindgust()
        {
            using (WebClient wcMO = new WebClient())
            {
                wcMO.Headers[HttpRequestHeader.ContentType] = "application/json";
                string uri = "http://datapoint.metoffice.gov.uk/public/data/val/wxobs/all/json/3265?res=hourly&key=c8b020fb-bc34-49cd-a30f-683dbd5c2d20";
                String resultJSON = wcMO.DownloadString(uri);
                dynamic TopCliffeObserved = JsonConvert.DeserializeObject<dynamic>(resultJSON);
                int valueDay = TopCliffeObserved.SiteRep.DV.Location.Period.Count;
                int valueHour = TopCliffeObserved.SiteRep.DV.Location.Period[valueDay-1].Rep.Count;
                if(TopCliffeObserved.SiteRep.DV.Location.Period[0].Rep[0].G != null)
                {
                    Windgust = TopCliffeObserved.SiteRep.DV.Location.Period[0].Rep[0].G;
                }
                
                System.IO.File.AppendAllText(directoryOutput, "\n" + " Observation at Topcliffe JSON:-  Windgust is: " + Windgust + "\n");
                return Windgust;
            }
        }

        public float ReturnRiverStage()
        {
            using (WebClient wcSH = new WebClient())
            {
                wcSH.Headers["SessionHeaderId"] = SessionHeaderId;

                string GaugeData =
                    wcSH.DownloadString("http://riverlevelsapi.shoothill.com/TimeSeries/GetTimeSeriesRecentDatapoints/?stationId=2060&datatype=3&numberDays=5");
                dynamic RiverLevelsViking = JsonConvert.DeserializeObject<dynamic>(GaugeData);
                Riverstage = RiverLevelsViking.values[RiverLevelsViking.values.Count-1].value;
                System.IO.File.AppendAllText(directoryOutput, "\n" + " Observation at Viking Recorder JSON:-  Riverstage is: " + Riverstage + "\n");
                return Riverstage;
            }
        }

            ~ CurrentState()
        {
            Console.WriteLine(" Removing object from memory");
        }
    }

}
